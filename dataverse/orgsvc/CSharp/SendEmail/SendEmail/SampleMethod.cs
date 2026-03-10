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
        private static Guid emailId;
        private static Guid contactId;
        private static Guid userId;

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
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // Create a contact to send an email to (To: field)
            var emailContact = new Contact
            {
                FirstName = "Nancy",
                LastName = "Anderson",
                EMailAddress1 = "nancy@contoso.com"
            };
            contactId = service.Create(emailContact);
            Console.WriteLine("Created a contact: {0}.", emailContact.FirstName + " " + emailContact.LastName);

            // Get a system user to send the email (From: field)
            var systemUserRequest = new WhoAmIRequest();
            var systemUserResponse = (WhoAmIResponse)service.Execute(systemUserRequest);
            userId = systemUserResponse.UserId;

            // Create the 'From:' activity party for the email
            var fromParty = new ActivityParty
            {
                PartyId = new EntityReference(SystemUser.EntityLogicalName, userId)
            };

            // Create the 'To:' activity party for the email
            var toParty = new ActivityParty
            {
                PartyId = new EntityReference(Contact.EntityLogicalName, contactId)
            };
            Console.WriteLine("Created activity parties.");

            // Create an e-mail message.
            var email = new Email
            {
                To = new ActivityParty[] { toParty },
                From = new ActivityParty[] { fromParty },
                Subject = "SDK Sample e-mail",
                Description = "SDK Sample for SendEmail Message.",
                DirectionCode = true
            };
            emailId = service.Create(email);
            Console.WriteLine("Create {0}.", email.Subject);
        }


        /// <summary>
        /// Deletes the custom entity record that was created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the entity created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service ,bool prompt)
        {
            bool toBeDeleted = true;

            if (prompt)
            {
                // Ask the user if the created entities should be deleted.
                Console.Write("\nDo you want these entity records deleted? (y/n) [y]: ");
                String answer = Console.ReadLine();
                if (answer.StartsWith("y") ||
                    answer.StartsWith("Y") ||
                    answer == String.Empty)
                {
                    toBeDeleted = true;
                }
                else
                {
                    toBeDeleted = false;
                }
            }

            if (toBeDeleted)
            {
                service.Delete(Email.EntityLogicalName, emailId);
                service.Delete(Contact.EntityLogicalName, contactId); ;
            
                Console.WriteLine("Entity records have been deleted.");
            }
        }

    }
}
