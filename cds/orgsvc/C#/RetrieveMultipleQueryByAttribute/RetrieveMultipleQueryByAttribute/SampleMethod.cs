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
        private static List<Guid> _accountIds = new List<Guid>();
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
            // Create two accounts.
            var account = new Account
            {
                Name = "A. Datum Corporation",
                Address1_StateOrProvince = "Colorado",
                Address1_Telephone1 = "(206)555-5555",
                EMailAddress1 = "info@datum.com"
            };
            _accountIds.Add(service.Create(account));

            account = new Account
            {
                Name = "Adventure Works Cycle",
                Address1_StateOrProvince = "Washington",
                Address1_City = "Redmond",
                Address1_Telephone1 = "(206)555-5555",
                EMailAddress1 = "contactus@adventureworkscycle.com"
            };
            _accountIds.Add(service.Create(account));
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
                if (answer.StartsWith("y") || answer.StartsWith("Y") ||
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
                foreach (Guid accountId in _accountIds)
                {
                    service.Delete(Account.EntityLogicalName, accountId);
                }
                Console.WriteLine("Entity record(s) have been deleted.");
            }
        }

    }
}
