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
        private static Guid _accountId;
        private static Guid _opportunityId;
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
            // Create an account
            Account account = new Account
            {
                Name = "Fourth Coffee"
            };
            _accountId = service.Create(account);

            // Create an opportunity
            Opportunity newOpportunity = new Opportunity
            {
                Name = "Opportunity 1",
                CustomerId = new EntityReference
                {
                    Id = _accountId,
                    LogicalName = account.LogicalName
                }
            };
            _opportunityId = service.Create(newOpportunity);
        }

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
                // Delete all records created in this sample.
                service.Delete(Account.EntityLogicalName, _accountId);

                Console.WriteLine("Entity record(s) have been deleted.");
            }

        }

    }
}
