using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
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
        private static Account[] duplicateAccounts = new Account[2];
        private static Account account;
        private static DuplicateRule rule;
        private static BulkDetectDuplicatesResponse response;
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
            

            String accountName = "Contoso, Ltd";
            String websiteUrl = "http://www.contoso.com/";

            Console.WriteLine("  Creating duplicate records (Account name={0}, Website URL={1})", accountName,
                websiteUrl);
            // Create some duplicate records
            for (int i = 0; i < 2; i++)
            {
                var account = new Account()
                {
                    Name = accountName,
                    WebSiteURL = websiteUrl
                };
                account.Id = service.Create(account);
                duplicateAccounts[i] = account;
            }

            accountName = "Contoso Pharmaceuticals";
            Console.WriteLine("  Creating a non-duplicate record (Account name={0}, Website URL={1})",
                accountName, websiteUrl);

            // Create a record that is NOT a duplicate
            var distinctAccount = new Account()
            {
                Name = accountName,
                WebSiteURL = websiteUrl
            };
            distinctAccount.Id = service.Create(distinctAccount);
            account = distinctAccount;

            
            Console.WriteLine("  Creating a duplicate detection rule");
            // Create a duplicate detection rule
            rule = new DuplicateRule()
            {
                Name = "Accounts with the same Account name and website url",
                BaseEntityName = Account.EntityLogicalName,
                MatchingEntityName = Account.EntityLogicalName
            };
            rule.Id = service.Create(rule);

            // Create a duplicate detection rule condition
            var nameCondition = new DuplicateRuleCondition()
            {
                BaseAttributeName = "name",
                MatchingAttributeName = "name",
                OperatorCode = new OptionSetValue(0), // value 0 = 'exact match'
                // set the regarding id to point to the rule created earlier,
                // associating this condition with that rule
                RegardingObjectId = rule.ToEntityReference()
            };
            service.Create(nameCondition);

            var websiteCondition = new DuplicateRuleCondition()
            {
                BaseAttributeName = "websiteurl",
                MatchingAttributeName = "websiteurl",
                OperatorCode = new OptionSetValue(0),
                RegardingObjectId = rule.ToEntityReference()
            };
            service.Create(websiteCondition);

            Console.WriteLine("  Publishing duplicate detection rule");
            // Publish the rule
            var publishRequest = new PublishDuplicateRuleRequest()
            {
                DuplicateRuleId = rule.Id
            };
            var publishResponse = (PublishDuplicateRuleResponse)service.Execute(publishRequest);

            // The PublishDuplicateRule request returns before the publish is completed,
            // so we keep retrieving the async job state until it is "Completed"
            Console.WriteLine("  Checking to see if duplicate detection rule has finished publishing");
            WaitForAsyncJobToFinish(service, publishResponse.JobId, 120);

           
        }

        private static void WaitForAsyncJobToFinish(CrmServiceClient service, Guid jobId, int maxTimeSeconds)
        {
            for (int i = 0; i < maxTimeSeconds; i++)
            {
                var asyncJob = service.Retrieve(AsyncOperation.EntityLogicalName,
                    jobId, new ColumnSet("statecode")).ToEntity<AsyncOperation>();
                if (asyncJob.StateCode.HasValue && asyncJob.StateCode.Value == AsyncOperationState.Completed)
                    return;
                System.Threading.Thread.Sleep(1000);
            }
            throw new Exception(String.Format(
                "  Exceeded maximum time of {0} seconds waiting for asynchronous job to complete",
                maxTimeSeconds
            ));
        }
        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the records created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service,bool prompt)
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
                // Delete the job
                Console.WriteLine("  Deleting the job");
                service.Delete(AsyncOperation.EntityLogicalName, response.JobId);

                // Delete the duplicate detection rule
                Console.WriteLine("  Deleting the duplicate detection rule");
                service.Delete(DuplicateRule.EntityLogicalName, rule.Id);

                // Delete the accounts
                Console.WriteLine("  Deleting the accounts");
                foreach (Account account in duplicateAccounts)
                    service.Delete(Account.EntityLogicalName, account.Id);
                service.Delete(Account.EntityLogicalName, account.Id);
                Console.WriteLine("Entity records have been deleted.");
            }
        }
    }
}
