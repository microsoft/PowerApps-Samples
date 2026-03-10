using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
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
        private static readonly List<Guid> _newAccountIds = new List<Guid>();
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
            // TODO Create entity records

            Console.WriteLine("Required records have been created.");
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
                var requestWithNoResults = new ExecuteMultipleRequest()
                {
                    // Set the execution behavior to continue and to not return responses.
                    Settings = new ExecuteMultipleSettings()
                    {
                        ContinueOnError = true,
                        ReturnResponses = false
                    },
                    Requests = new OrganizationRequestCollection()
                };

                // Get all the entities into a collection to delete
                EntityCollection delete = GetCollectionOfEntitiesToDelete();
                foreach (var entity in delete.Entities)
                {
                    DeleteRequest deleteRequest = new DeleteRequest { Target = entity.ToEntityReference() };
                    requestWithNoResults.Requests.Add(deleteRequest);
                }

                ExecuteMultipleResponse responseWithNoResults =
                    (ExecuteMultipleResponse)service.Execute(requestWithNoResults);

                // There should be no responses unless there was an error. 
                if (responseWithNoResults.Responses.Count > 0)
                {
                    foreach (var responseItem in responseWithNoResults.Responses)
                    {
                        if (responseItem.Fault != null)
                            DisplayFault(requestWithNoResults.Requests[responseItem.RequestIndex],
                                responseItem.RequestIndex, responseItem.Fault);
                    }
                }
                else
                {
                    Console.WriteLine("All account records have been deleted successfully.");
                }

                
            }
        }

        /// <summary>
        /// Create a collection of new entity objects.
        /// </summary>
        /// <returns>A collection of entity objects.</returns>
        private static EntityCollection GetCollectionOfEntitiesToCreate()
        {
            return new EntityCollection()
            {
                EntityName = Account.EntityLogicalName,
                Entities = {
                    new Account { Name = "ExecuteTransaction Example Account 1" },
                    new Account { Name = "ExecuteTransaction Example Account 2" },
                    new Account { Name = "ExecuteTransaction Example Account 3" },
                    new Account { Name = "ExecuteTransaction Example Account 4" },
                    new Account { Name = "ExecuteTransaction Example Account 5" }
                }
            };
        }

        /// <summary>
        /// Display the response of an organization message request.
        /// </summary>
        /// <param name="organizationRequest">The organization message request.</param>
        /// <param name="organizationResponse">The organization message response.</param>
        private static void DisplayResponse(OrganizationRequest organizationRequest, OrganizationResponse organizationResponse)
        {
            Console.WriteLine("Created " + ((Account)organizationRequest.Parameters["Target"]).Name
                + " with account id as " + organizationResponse.Results["id"].ToString());
            _newAccountIds.Add(new Guid(organizationResponse.Results["id"].ToString()));
        }

        /// <summary>
        /// Create a collection of entity objects for updating. Give these entities a new
        /// name for the update.
        /// </summary>
        /// <returns>An entity collection.</returns>
        private static EntityCollection GetCollectionOfEntitiesToUpdate()
        {
            EntityCollection collection = new EntityCollection()
            {
                EntityName = Account.EntityLogicalName
            };

            for (int i = 1; i <= _newAccountIds.Count; i++)
            {
                collection.Entities.Add(
                    new Account
                    {
                        Name = "Updated Account Name " + i.ToString(),
                        Id = _newAccountIds[i - 1]
                    });
            }

            return collection;
        }

        /// <summary>
        /// Delete a collection of entity objects.
        /// </summary>
        /// <returns>A collection of entity objects</returns>
        private static EntityCollection GetCollectionOfEntitiesToDelete()
        {
            EntityCollection collection = new EntityCollection()
            {
                EntityName = Account.EntityLogicalName
            };

            for (int i = 1; i <= _newAccountIds.Count; i++)
            {
                collection.Entities.Add(
                    new Account
                    {
                        Id = _newAccountIds[i - 1]
                    });
            }

            return collection;
        }

        /// <summary>
        /// Display the fault that resulted from processing an organization message request.
        /// </summary>
        /// <param name="organizationRequest">The organization message request.</param>
        /// <param name="count">nth request number from ExecuteMultiple request</param>
        /// <param name="organizationServiceFault">A WCF fault.</param>
        private static void DisplayFault(OrganizationRequest organizationRequest, int count,
            OrganizationServiceFault organizationServiceFault)
        {
            Console.WriteLine("A fault occurred when processing {1} request, at index {0} in the request collection with a fault message: {2}", count + 1,
                organizationRequest.RequestName,
                organizationServiceFault.Message);
        }
    }
}
