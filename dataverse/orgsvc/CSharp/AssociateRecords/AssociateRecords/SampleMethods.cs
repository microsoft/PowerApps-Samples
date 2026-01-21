using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {

        // Define the IDs needed for this sample.
        public static Guid _contactId;
        public static Guid _account1Id;
        public static Guid _account2Id;
        public static Guid _account3Id;
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
            // Instantiate a contact entity record and set its property values.
            // See the Entity Metadata topic in the SDK documentatio to determine 
            // which attributes must be set for each entity.
            var setupContact = new Contact
            {
                FirstName = "John",
                LastName = "Doe"
            };
            _contactId = service.Create(setupContact);
            Console.WriteLine("Created {0} {1}", setupContact.FirstName,
                setupContact.LastName);

            // Instantiate an account entity record and set its property values.
            var setupAccount1 = new Account
            {
                Name = "Example Account 1"
            };
            _account1Id = service.Create(setupAccount1);
            Console.WriteLine("Created {0}", setupAccount1.Name);

            var setupAccount2 = new Account
            {
                Name = "Example Account 2"
            };
            _account2Id = service.Create(setupAccount2);
            Console.WriteLine("Created {0}", setupAccount2.Name);

            var setupAccount3 = new Account
            {
                Name = "Example Account 3"
            };
            _account3Id = service.Create(setupAccount3);
            Console.WriteLine("Created {0}", setupAccount3.Name);
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
                service.Delete("account", _account1Id);
                service.Delete("account", _account2Id);
                service.Delete("account", _account3Id);
                Console.WriteLine("Entity records have been deleted.");
            }
        }

    }
}
