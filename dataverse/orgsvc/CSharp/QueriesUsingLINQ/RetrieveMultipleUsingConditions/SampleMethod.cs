using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial class RetrieveMultiple
    {
        private static bool prompt = true;
        /// <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        /// 
        private static void SetUpSample(CrmServiceClient service)
        {
            // Check that the current version is greater than the minimum version
            if (!SampleHelpers.CheckVersion(service, new Version("9.0.0.0")))
            {
                //The environment version is lower than version 9.0.0.0
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
            // Create an account.
            Account account = new Account
            {
                Name = "Litware, Inc."
            };
            _accountId = service.Create(account);

            // Create the 2 contacts.
            Contact contact = new Contact()
            {
                FirstName = "Ben",
                LastName = "Andrews",
                EMailAddress1 = "sample@example.com",
                Address1_City = "Redmond",
                Address1_StateOrProvince = "WA",
                Address1_Telephone1 = "(206)555-5555",
                ParentCustomerId = new EntityReference
                {
                    Id = _accountId,
                    LogicalName = account.LogicalName
                }
            };
            _contactIdList.Add(service.Create(contact));

            contact = new Contact()
            {
                FirstName = "Colin",
                LastName = "Wilcox",
                EMailAddress1 = "sample@example.com",
                Address1_City = "Bellevue",
                Address1_StateOrProvince = "WA",
                Address1_Telephone1 = "(425)555-5555",
                ParentCustomerId = new EntityReference
                {
                    Id = _accountId,
                    LogicalName = account.LogicalName
                }
            };
            _contactIdList.Add(service.Create(contact));

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
                foreach (Guid contactId in _contactIdList)
                {
                    service.Delete(Contact.EntityLogicalName, contactId);
                }

                service.Delete(Account.EntityLogicalName, _accountId);

                Console.WriteLine("Entity records have been deleted.");
            }
        }

    }
}
