using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {

        private static Guid _caseCustomerId;
        private static Guid _caseIncidentId;
        private static bool prompt = true;
        /// <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        /// 
        private static void SetUpSample(CrmServiceClient service)
        {
            // Check that the current version is greater than the minimum version
            if (!SampleHelpers.CheckVersion(service, new Version("7.1.0.0")))
            {
                //The environment version is lower than version 7.1.0.0
                return;
            }

            CreateRequiredRecords(service);
        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, prompt);
        }

        /// <summary>
        /// This method creates any entity records that this sample requires.
        /// Creates the email activity.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // Create a customer for a new incident
            var caseCustomer = new Contact();

            // Set the contact properties
            caseCustomer.FirstName = "Christen";
            caseCustomer.LastName = "Anderson";

            // Create the contact in CRM
            _caseCustomerId = service.Create(caseCustomer);
            NotifyEntityCreated(Contact.EntityLogicalName, _caseCustomerId);

            // Retrieve the default subject for a new incident\case
            var query = new QueryExpression()
            {
                EntityName = Subject.EntityLogicalName,
                ColumnSet = new ColumnSet(allColumns: true),
                Criteria = new FilterExpression()
            };
            query.Criteria.AddCondition("title", ConditionOperator.Equal,
                "Default Subject");

            var subjectRecords = service.RetrieveMultiple(query);
            Entity defaultSubject = subjectRecords[0];
            NotifyEntityRetrieved(Subject.EntityLogicalName, defaultSubject.Id);

            // Create a new incident
            Incident newCase = new Incident();

            // Set the incident properties
            newCase.SubjectId = new EntityReference();
            newCase.SubjectId.LogicalName = Subject.EntityLogicalName;
            newCase.SubjectId.Id = defaultSubject.Id;
            newCase.CustomerId = new EntityReference();
            newCase.CustomerId.LogicalName = Contact.EntityLogicalName;
            newCase.CustomerId.Id = _caseCustomerId;
            newCase.Title = "New Case from SDK";

            _caseIncidentId = service.Create(newCase);
            NotifyEntityCreated(Incident.EntityLogicalName, _caseIncidentId);
        }


        /// <summary>
        /// Deletes the custom entity record that was created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the entity created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
            bool deleteRecords = true;

            if (prompt)
            {
                Console.WriteLine("\nDo you want these entity records deleted? (y/n)");
                String answer = Console.ReadLine();

                deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y"));
            }

            if (deleteRecords)
            {

                service.Delete(Incident.EntityLogicalName, _caseIncidentId);
                service.Delete(Contact.EntityLogicalName, _caseCustomerId);
                Console.WriteLine("Entity records have been deleted.");
            }
        }

        private static void CloseIncident(CrmServiceClient service, EntityReference caseReference)
        {
            // First close the Incident

            // Create resolution for the closing incident
            var resolution = new IncidentResolution
            {
                Subject = "Case Closed",
            };

            resolution.IncidentId = caseReference;

            // Create the request to close the incident, and set its resolution to the 
            // resolution created above
            var closeRequest = new CloseIncidentRequest();
            closeRequest.IncidentResolution = resolution;

            // Set the requested new status for the closed Incident
            closeRequest.Status =
                new OptionSetValue((int)incident_statuscode.ProblemSolved);

            // Execute the close request
            var closeResponse =
                (CloseIncidentResponse)service.Execute(closeRequest);

            //Check if the incident was successfully closed
            Incident incident = service.Retrieve(Incident.EntityLogicalName,
                _caseIncidentId, new ColumnSet(allColumns: true)).ToEntity<Incident>();

            if (incident.StateCode.HasValue &&
                incident.StateCode == IncidentState.Resolved)
            {
                Console.WriteLine("The incident was closed out as Resolved.");
            }
            else
            {
                Console.WriteLine("The incident's state was not changed.");
            }
        }

        private static void SetState(CrmServiceClient service, EntityReference caseReference)
        {
            // Updating the incident state
            var state = new Entity("incident");
            state["incidentid"] = _caseIncidentId;
            state["statecode"] = new OptionSetValue((int)IncidentState.Active);
            state["statuscode"] = new OptionSetValue((int)incident_statuscode.WaitingforDetails);
            service.Update(state);

            // Check if the state was successfully set
            Incident incident = service.Retrieve(Incident.EntityLogicalName,
                _caseIncidentId, new ColumnSet(allColumns: true)).ToEntity<Incident>();

            if (incident.StatusCode.Value == (int)incident_statuscode.WaitingforDetails)
            {
                Console.WriteLine("Record state set successfully.");
            }
            else
            {
                Console.WriteLine("The request to set the record state failed.");
            }


        }
        private static void NotifyEntityCreated(String entityName, Guid entityId)
        {
            Console.WriteLine("  {0} created with GUID {{{1}}}",
                entityName, entityId);
        }

        private static void NotifyEntityRetrieved(String entityName, Guid entityId)
        {
            Console.WriteLine("  Retrieved {0} with GUID {{{1}}}",
                entityName, entityId);
        }

    }
}
