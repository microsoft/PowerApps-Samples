using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial class LateBoundQuery
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
            // Create 3 contacts.
            Contact contact = new Contact()
            {
                FirstName = "Ben",
                LastName = "Andrews",
                EMailAddress1 = "sample@example.com",
                Address1_City = "Redmond",
                Address1_StateOrProvince = "WA"
            };
            _contactIds.Add(service.Create(contact));

            contact = new Contact()
            {
                FirstName = "Colin",
                LastName = "Wilcox",
                EMailAddress1 = "sample@example.com",
                Address1_City = "Bellevue",
                Address1_StateOrProvince = "WA"
            };
            _contactIds.Add(service.Create(contact));

            contact = new Contact()
            {
                FirstName = "Ben",
                LastName = "Smith",
                EMailAddress1 = "sample@example.com",
                Address1_City = "Bellevue",
                Address1_StateOrProvince = "WA"
            };
            _contactIds.Add(service.Create(contact));

            // Create 3 leads.
            Lead lead = new Lead()
            {
                FirstName = "Dan",
                LastName = "Wilson",
                EMailAddress1 = "sample@example.com",
                Address1_City = "Redmond",
                Address1_StateOrProvince = "WA"
            };
            _leadIds.Add(service.Create(lead));

            lead = new Lead()
            {
                FirstName = "Jim",
                LastName = "Wilson",
                EMailAddress1 = "sample@example.com",
                Address1_City = "Bellevue",
                Address1_StateOrProvince = "WA"
            };
            _leadIds.Add(service.Create(lead));

            lead = new Lead()
            {
                FirstName = "Denise",
                LastName = "Smith",
                EMailAddress1 = "sample@example.com",
                Address1_City = "Bellevue",
                Address1_StateOrProvince = "WA"
            };
            _leadIds.Add(service.Create(lead));

            // Create 5 customized Accounts for the LINQ samples.
            Account account = new Account
            {
                Name = "A. Datum Corporation",
                Address1_StateOrProvince = "Colorado",
                Address1_Telephone1 = "(206)555-5555",
                PrimaryContactId =
                 new EntityReference(Contact.EntityLogicalName, _contactIds[0])
            };
            _accountIds.Add(service.Create(account));

            account = new Account
            {
                Name = "Adventure Works",
                Address1_StateOrProvince = "Illinois",
                Address1_County = "Lake County",
                Address1_Telephone1 = "(206)555-5555",
                OriginatingLeadId =
                 new EntityReference(Lead.EntityLogicalName, _leadIds[0])
            };
            _accountIds.Add(service.Create(account));

            account = new Account
            {
                Name = "Coho Vineyard",
                Address1_StateOrProvince = "Washington",
                Address1_County = "King County",
                Address1_Telephone1 = "(425)555-5555",
                PrimaryContactId =
                 new EntityReference(Contact.EntityLogicalName, _contactIds[1]),
                OriginatingLeadId =
                 new EntityReference(Lead.EntityLogicalName, _leadIds[0])
            };
            _accountIds.Add(service.Create(account));

            account = new Account
            {
                Name = "Fabrikam",
                Address1_StateOrProvince = "Washington",
                Address1_Telephone1 = "(425)555-5555",
                PrimaryContactId =
                 new EntityReference(Contact.EntityLogicalName, _contactIds[0])
            };
            _accountIds.Add(service.Create(account));

            account = new Account
            {
                Name = "Humongous Insurance",
                Address1_StateOrProvince = "Missouri",
                Address1_County = "Saint Louis County",
                Address1_Telephone1 = "(314)555-5555",
                PrimaryContactId =
                 new EntityReference(Contact.EntityLogicalName, _contactIds[1])
            };
            _accountIds.Add(service.Create(account));

            // Create 10 basic Account records.
            for (int i = 1; i <= 10; i++)
            {
                account = new Account
                {
                    Name = "Fourth Coffee " + i,
                    Address1_StateOrProvince = "California"
                };
                _accountIds.Add(service.Create(account));
            }

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
                foreach (Guid accountId in _accountIds)
                {
                    service.Delete(Account.EntityLogicalName, accountId);
                }
                foreach (Guid contactId in _contactIds)
                {
                    service.Delete(Contact.EntityLogicalName, contactId);
                }
                foreach (Guid leadId in _leadIds)
                {
                    service.Delete(Lead.EntityLogicalName, leadId);
                }

                Console.WriteLine("Entity records have been deleted.");
            }
        }

    }
}
