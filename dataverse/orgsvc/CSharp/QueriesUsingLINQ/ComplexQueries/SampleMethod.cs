using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial class LINQ101
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
            Contact contact1 = new Contact
            {
                FirstName = "Colin",
                LastName = "Wilcox",
                Address1_City = "Redmond",
                Address1_StateOrProvince = "WA",
                Address1_PostalCode = "98052",
                Anniversary = new DateTime(2010, 3, 5),
                CreditLimit = new Money(300),
                Description = "Alpine Ski House",
                StatusCode = new OptionSetValue(1),
                AccountRoleCode = new OptionSetValue(1),
                NumberOfChildren = 1,
                Address1_Latitude = 47.6741667,
                Address1_Longitude = -122.1202778,
                CreditOnHold = false
            };
            _contactId1 = service.Create(contact1);

            Console.Write("Created a sample contact 1: {0}, ", contact1.FirstName + " " + contact1.LastName);

            Contact contact2 = new Contact
            {
                FirstName = "Brian",
                LastName = "Smith",
                Address1_City = "Bellevue",
                FamilyStatusCode = new OptionSetValue(3),
                Address1_StateOrProvince = "WA",
                Address1_PostalCode = "98008",
                Anniversary = new DateTime(2010, 4, 5),
                CreditLimit = new Money(30000),
                Description = "Coho Winery",
                StatusCode = new OptionSetValue(1),
                AccountRoleCode = new OptionSetValue(2),
                NumberOfChildren = 2,
                Address1_Latitude = 47.6105556,
                Address1_Longitude = -122.1994444,
                CreditOnHold = false
            };
            _contactId2 = service.Create(contact2);

            Console.Write("Created a sample contact 2: {0}, ", contact2.FirstName + " " + contact2.LastName);

            Contact contact3 = new Contact
            {
                FirstName = "Darren",
                LastName = "Parker",
                Address1_City = "Kirkland",
                FamilyStatusCode = new OptionSetValue(3),
                Address1_StateOrProvince = "WA",
                Address1_PostalCode = "98033",
                Anniversary = new DateTime(2010, 10, 5),
                CreditLimit = new Money(10000),
                Description = "Coho Winery",
                StatusCode = new OptionSetValue(1),
                AccountRoleCode = new OptionSetValue(2),
                NumberOfChildren = 2,
                Address1_Latitude = 47.6105556,
                Address1_Longitude = -122.1994444,
                CreditOnHold = false
            };
            _contactId3 = service.Create(contact3);

            Console.Write("Created a sample contact 3: {0}, ", contact3.FirstName + " " + contact3.LastName);

            Contact contact4 = new Contact
            {
                FirstName = "Ben",
                LastName = "Smith",
                Address1_City = "Kirkland",
                FamilyStatusCode = new OptionSetValue(3),
                Address1_StateOrProvince = "WA",
                Address1_PostalCode = "98033",
                Anniversary = new DateTime(2010, 7, 5),
                CreditLimit = new Money(12000),
                Description = "Coho Winery",
                StatusCode = new OptionSetValue(1),
                AccountRoleCode = new OptionSetValue(2),
                NumberOfChildren = 2,
                Address1_Latitude = 47.6105556,
                Address1_Longitude = -122.1994444,
                CreditOnHold = true
            };
            _contactId4 = service.Create(contact4);

            Console.Write("Created a sample contact 4: {0}, ", contact4.FirstName + " " + contact4.LastName);

            Incident incident1 = new Incident
            {
                Title = "Test Case 1",
                PriorityCode = new OptionSetValue(1), // 1 = High
                CaseOriginCode = new OptionSetValue(1), // 1 = Phone
                CaseTypeCode = new OptionSetValue(2), // 2 = Problem
                Description = "Description for Test Case 1.",
                FollowupBy = DateTime.Now.AddHours(3.0), // follow-up in 3 hours
                CustomerId = new EntityReference(Contact.EntityLogicalName, _contactId2)
            };

            _incidentId1 = service.Create(incident1);

            Console.Write("Created a sample incident 1: {0}, ", incident1.Title);

            Relationship relationship1 = new Relationship("incident_customer_contacts");
            EntityReferenceCollection relatedEntities1 = new EntityReferenceCollection();
            relatedEntities1.Add(new EntityReference(Contact.EntityLogicalName, _contactId1));
            service.Associate(Incident.EntityLogicalName, _incidentId1, relationship1, relatedEntities1);

            Console.Write("Added relationship between incident 1 and contact 1, ");


            Account account1 = new Account
            {
                Name = "Coho Winery",
                Address1_Name = "Coho Vineyard & Winery",
                Address1_City = "Redmond"
            };
            _accountId1 = service.Create(account1);

            Console.Write("Created a sample account 1: {0}, ", account1.Name);

            Incident incident2 = new Incident
            {
                Title = "Test Case 2",
                PriorityCode = new OptionSetValue(1), // 1 = High
                CaseOriginCode = new OptionSetValue(1), // 1 = Phone
                CaseTypeCode = new OptionSetValue(2), // 2 = Problem
                Description = "Description for Sample Case 2.",
                FollowupBy = DateTime.Now.AddHours(3.0), // follow-up in 3 hours
                CustomerId = new EntityReference(Contact.EntityLogicalName, _contactId1)
            };

            _incidentId2 = service.Create(incident2);

            Console.Write("Created a sample incident 2: {0}, ", incident2.Title);

            Relationship relationship2 = new Relationship("incident_customer_accounts");
            EntityReferenceCollection relatedEntities2 = new EntityReferenceCollection();
            relatedEntities2.Add(new EntityReference(Account.EntityLogicalName, _accountId1));
            service.Associate(Incident.EntityLogicalName, _incidentId2, relationship2, relatedEntities2);

            Console.Write("Added relationship between incident 2 and account 1, ");

            Lead lead = new Lead()
            {
                FirstName = "Diogo",
                LastName = "Andrade"
            };
            _leadId = service.Create(lead);
            Console.Write("Created a sample Lead: {0} ", lead.FirstName + " " + lead.LastName);

            Account account2 = new Account
            {
                Name = "Contoso Ltd",
                ParentAccountId = new EntityReference(Account.EntityLogicalName, _accountId1),
                Address1_Name = "Contoso Pharmaceuticals",
                Address1_City = "Redmond",
                OriginatingLeadId = new EntityReference(Lead.EntityLogicalName, _leadId)
            };
            _accountId2 = service.Create(account2);

            Console.Write("Created a sample account 2: {0}, ", account2.Name);

            Relationship relationship3 = new Relationship("account_primary_contact");
            EntityReferenceCollection relatedEntities3 = new EntityReferenceCollection();
            relatedEntities3.Add(new EntityReference(Account.EntityLogicalName, _accountId2));
            service.Associate(Contact.EntityLogicalName, _contactId2, relationship3, relatedEntities3);

            Console.WriteLine("Added relationship between account 2 and contact 2.");

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
                // Delete all records created in this sample.
                service.Delete(Account.EntityLogicalName, _accountId2);
                service.Delete(Incident.EntityLogicalName, _incidentId2);
                service.Delete(Account.EntityLogicalName, _accountId1);
                service.Delete(Lead.EntityLogicalName, _leadId);
                service.Delete(Incident.EntityLogicalName, _incidentId1);
                service.Delete(Contact.EntityLogicalName, _contactId4);
                service.Delete(Contact.EntityLogicalName, _contactId3);
                service.Delete(Contact.EntityLogicalName, _contactId2);
                service.Delete(Contact.EntityLogicalName, _contactId1);

                Console.WriteLine("Entity records have been deleted.");
            }
        }

    }
}
