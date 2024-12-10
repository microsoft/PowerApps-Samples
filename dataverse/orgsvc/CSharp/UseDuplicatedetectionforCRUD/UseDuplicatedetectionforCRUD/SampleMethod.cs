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
        private  static Guid accountId;
        private static Guid ruleId;
        private static Guid dupAccountId;
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
            // Create an account record named Fourth Coffee.
            var account = new Account
            {
                Name = "Fourth Coffee",
                AccountNumber = "ACC005"
            };
            accountId = service.Create(account);
            Console.Write("Account {0} {1} created, ", account.Name, account.AccountNumber);

            // Create a duplicate detection rule
            var accountDuplicateRule = new DuplicateRule
            {
                Name = "DuplicateRule: Accounts with the same Account Number",
                BaseEntityName = "account",
                MatchingEntityName = "account"
            };
            ruleId = service.Create(accountDuplicateRule);

            // Create a duplicate detection rule condition
            DuplicateRuleCondition accountDupCondition = new DuplicateRuleCondition
            {
                BaseAttributeName = "accountnumber",
                MatchingAttributeName = "accountnumber",
                OperatorCode = new OptionSetValue(0), // Exact Match.
                RegardingObjectId = new EntityReference(DuplicateRule.EntityLogicalName, ruleId)
            };
            Guid conditionId = service.Create(accountDupCondition);

            Console.Write("'{0}' created, ", accountDuplicateRule.Name);

            // Execute the publish request.
            var response =
                (PublishDuplicateRuleResponse)service.Execute(new PublishDuplicateRuleRequest() { DuplicateRuleId = ruleId });

            // When the publishDuplicateRule request returns, the state of the rule will still be "Publishing" (StatusCode = 1).
            // we need to wait for the publishing operation to complete, so we keep polling the state of the
            // rule until it becomes "Published" (StatusCode = 2).
            int i = 0;
            var retrievedRule =
                (DuplicateRule)service.Retrieve(DuplicateRule.EntityLogicalName, ruleId, new ColumnSet(new String[] { "statuscode" }));
            while (retrievedRule.StatusCode.Value == 1 && i < 20)
            {
                i++;
                System.Threading.Thread.Sleep(1000);
                retrievedRule =
                    (DuplicateRule)service.Retrieve(DuplicateRule.EntityLogicalName, ruleId, new ColumnSet(new String[] { "statuscode" }));
            }

            Console.Write("published.\n");
        }

        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the records created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
            bool deleteRecords = true;

            if (prompt)
            {
                Console.WriteLine("\nDo you want these entity records deleted? (y/n) [y]: ");
                String answer = Console.ReadLine();

                deleteRecords = (answer.StartsWith("y") ||
                                answer.StartsWith("Y") ||
                                answer == String.Empty);
            }

            if (deleteRecords)
            {
                service.Delete(Account.EntityLogicalName, accountId);
                UnpublishDuplicateRuleRequest unpublishRequest = new UnpublishDuplicateRuleRequest
                {
                    DuplicateRuleId = ruleId
                };
                service.Execute(unpublishRequest);
                service.Delete(DuplicateRule.EntityLogicalName, ruleId);
                service.Delete(Account.EntityLogicalName, dupAccountId);
                Console.WriteLine("Entity records have been deleted.");
            }
        }

    }
}
