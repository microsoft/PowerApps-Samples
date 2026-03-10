using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
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
        private static Guid _faxId;
        private static Guid _taskId;
        private static Guid _userId;
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
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // Get the current user.
            WhoAmIRequest userRequest = new WhoAmIRequest();
            WhoAmIResponse userResponse = (WhoAmIResponse)service.Execute(userRequest);
            _userId = userResponse.UserId;

            // Create the activity party for sending and receiving the fax.
            ActivityParty party = new ActivityParty
            {
                PartyId = new EntityReference(SystemUser.EntityLogicalName, _userId)
            };

            // Create the fax object.
            Fax fax = new Fax
            {
                Subject = "Sample Fax",
                From = new ActivityParty[] { party },
                To = new ActivityParty[] { party }
            };
            _faxId = service.Create(fax);
            Console.WriteLine("Created a fax: '{0}'.", fax.Subject);
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

                service.Delete(Fax.EntityLogicalName, _faxId);
                service.Delete(Task.EntityLogicalName, _taskId);

                Console.WriteLine("Entity records have been deleted.");
            }
        }
    }
}
