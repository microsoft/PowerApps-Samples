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
        private static Guid[] _contactIds = new Guid[3];
        private Dictionary<Guid, String> _recordIds = new Dictionary<Guid, String>();
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
        /// Creates any entity records that this sample requires.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // Create 3 Contact records to use in the Run() method.
            var contact1 = new Contact()
            {
                FirstName = "Mary Kay",
                LastName = "Andersen",
                Address1_Line1 = "23 Market St.",
                Address1_City = "Sammamish",
                Address1_StateOrProvince = "MT",
                Address1_PostalCode = "99999",
                Telephone1 = "12345678",
                EMailAddress1 = "marykay@contoso.com"
            };
            _contactIds[0] = service.Create(contact1);

            var contact2 = new Contact()
            {
                FirstName = "Joe",
                LastName = "Andreshak",
                Address1_Line1 = "23 Market St.",
                Address1_City = "Sammamish",
                Address1_StateOrProvince = "MT",
                Address1_PostalCode = "99999",
                Telephone1 = "12345678",
                EMailAddress1 = "joe@contoso.com"
            };
            _contactIds[1] = service.Create(contact2);

            var contact3 = new Contact()
            {
                FirstName = "Denise",
                LastName = "Smith",
                Address1_Line1 = "23 Market St.",
                Address1_City = "Sammamish",
                Address1_StateOrProvince = "MT",
                Address1_PostalCode = "99999",
                Telephone1 = "12345678",
                EMailAddress1 = "denise@contoso.com"
            };
            _contactIds[2] = service.Create(contact3);

        }

        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the records created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service,  bool prompt)
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
                // Delete all records created in this sample.
                foreach (Guid contactId in _contactIds)
                {
                    service.Delete(Contact.EntityLogicalName, contactId);
                }
                Console.WriteLine("Entity record(s) have been deleted.");
            }
        }

    }
}
