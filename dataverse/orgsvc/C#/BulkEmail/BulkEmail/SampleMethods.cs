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
            Console.WriteLine();
            Console.WriteLine("Creating and sending SendBulkEmail.");

        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, prompt);
        }

        /// <summary>
        /// Creates any entity records that this sample requires.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            Console.WriteLine("Creating contacts records...");

            var emailContact1 = new Contact()
            {
                FirstName = "Adam",
                LastName = "Carter",
                EMailAddress1 = "someone@example.com"
            };

            // Create the contact1.
            _contactsIds.Add(service.Create(emailContact1));
            Console.WriteLine("Contact1 created.");

            var emailContact2 = new Contact()
            {
                FirstName = "Adina",
                LastName = "Hagege",
                EMailAddress1 = "someone@example.com"
            };

            // Create the contact2.
            _contactsIds.Add(service.Create(emailContact2));
            Console.WriteLine("Contact2 created.");

        }

        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the records created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
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
                // Delete the contacts.
                foreach (var contactId in _contactsIds)
                {
                    service.Delete(Contact.EntityLogicalName, contactId);
                }
                Console.WriteLine("Contacts have been deleted.");
            }
        }
    }
}
