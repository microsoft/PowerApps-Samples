using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
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
        private static Guid? accountId1;
        private static Guid? accountId2;
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

            EnableDuplicateDetectionForOrg(service);
            EnableDuplicateDetectionForEntity(service, Account.EntityLogicalName);
            PublishRulesForEntity(service, Account.EntityLogicalName);
            CreateAccountRecords(service);
            RetrieveDuplicates(service);
            ;
        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, prompt);
        }

        /// <summary>
        /// Create some account records to retrieve duplicates
        /// </summary>
        private static void CreateAccountRecords(CrmServiceClient service)
        {
            var crmAccount = new Account { Name = "Microsoft" };

            accountId1 = service.Create(crmAccount);
            accountId2 = service.Create(crmAccount);
            Console.WriteLine(String.Concat("Creating duplicate records:\n\taccount 1 - ",
                accountId1.Value, "\n\taccount 2 - ", accountId2.Value));
        }

        /// <summary>
        /// Call the method to retrieve duplicate records.  
        /// </summary>
        private static void RetrieveDuplicates(CrmServiceClient service)
        {
            // PagingInfo is Required. 
            var request = new RetrieveDuplicatesRequest
            {
                BusinessEntity = new Account { Name = "Microsoft" }.ToEntity<Entity>(),
                MatchingEntityName = Account.EntityLogicalName,
                PagingInfo = new PagingInfo() { PageNumber = 1, Count = 50 }
            };

            Console.WriteLine("Retrieving duplicates");
            var response = (RetrieveDuplicatesResponse)service.Execute(request);

            for (int i = 0; i < response.DuplicateCollection.Entities.Count; i++)
            {
                var crmAccount = response.DuplicateCollection.Entities[i]
                    .ToEntity<Account>();
                Console.WriteLine(crmAccount.Name + ", " + crmAccount.AccountId.Value.ToString());
            }
        }

        /// <summary>
        /// Enables duplicate detection for the organization
        /// </summary>
        private static void EnableDuplicateDetectionForOrg(CrmServiceClient service)
        {
            // Retrieve the org ID
            var orgId = RetrieveOrganizationId(service);
            if (!orgId.HasValue)
                return;

            Console.WriteLine(String.Concat("Enabling duplicate detection for organization: ",
                orgId.Value));

            // Enable dupe detection for each type
            var crmOrganization = new Organization
            {
                Id = orgId.Value,
                IsDuplicateDetectionEnabled = true,
                IsDuplicateDetectionEnabledForImport = true,
                IsDuplicateDetectionEnabledForOfflineSync = true,
                IsDuplicateDetectionEnabledForOnlineCreateUpdate = true,
            };

            service.Update(crmOrganization);
        }

        /// <summary>
        /// Enabling the dupe detection unpublishes the rules.  
        /// This will publish them, and wait for them to complete publishing.
        /// </summary>
        /// <param name="entityName"></param>
        private static void PublishRulesForEntity(CrmServiceClient service ,string entityName)
        {
            // Retrieve all rules for the entity
            var rules = service.RetrieveMultiple(
                new QueryByAttribute(DuplicateRule.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet("duplicateruleid"),
                    Attributes = { "matchingentityname" },
                    Values = { entityName }
                });

            List<Guid> crmAsyncOperationIds = new List<Guid>();
            foreach (var item in rules.Entities)
            {
                Console.WriteLine(String.Concat("Publishing duplicate rule: ", item.Id));

                // Publish each rule and get the job id since it is async
                var response = (PublishDuplicateRuleResponse)service.Execute(
                        new PublishDuplicateRuleRequest { DuplicateRuleId = item.Id });

                crmAsyncOperationIds.Add(response.JobId);
            }

            // Wait until all the rules are published before testing the dupe detection
            WaitForAsyncJobCompletion(service, crmAsyncOperationIds);
        }

        /// <summary>
        /// Updates the entity customizations and publishes the entity 
        /// </summary>
        /// <param name="entityName"></param>
        private static void EnableDuplicateDetectionForEntity(CrmServiceClient service ,string entityName)
        {
            Console.WriteLine(String.Format("Retrieving entity metadata for {0}",
                entityName));

            // Retrieve the entity metadata
            var crmEntity = ((RetrieveEntityResponse)service.Execute(
                new RetrieveEntityRequest
                {
                    RetrieveAsIfPublished = true,
                    LogicalName = entityName
                })).EntityMetadata;

            Console.WriteLine(String.Concat("Enabling duplicate for ", entityName));

            // Update the duplicate detection flag
            crmEntity.IsDuplicateDetectionEnabled =
                new BooleanManagedProperty(true);

            // Update the entity metadata
            service.Execute(new UpdateEntityRequest
            {
                Entity = crmEntity
            });

            Console.WriteLine(String.Concat("Publishing ", entityName, " entity"));

            // Publish the entity 
            var publishRequest = new PublishXmlRequest
            {
                ParameterXml = String.Concat("<importexportxml><entities><entity>",
                    entityName, "</entity></entities></importexportxml>"),
            };

            service.Execute(publishRequest);
        }

        /// <summary>
        /// There should only be one organization record per Crm Org
        /// </summary>
        /// <returns></returns>
        private static Guid? RetrieveOrganizationId(CrmServiceClient service)
        {
            // Retrieve the first record in the organization table.  
            // There should only be one organization record. 
            var entities = service.RetrieveMultiple(
                new QueryExpression(Organization.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet("organizationid"),
                    PageInfo = new PagingInfo { PageNumber = 1, Count = 1 },
                });

            if (entities != null && entities.Entities.Count > 0)
                return entities.Entities[0].Id;

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prompt"></param>
        private static void DeleteRequiredRecords(CrmServiceClient service ,bool prompt)
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
                // Delete records created in this sample.
                if (accountId1.HasValue)
                {
                    Console.WriteLine(String.Concat("Deleting account: ", accountId1.Value));
                    service.Delete(Account.EntityLogicalName, accountId1.Value);
                }
                if (accountId2.HasValue)
                {
                    Console.WriteLine(String.Concat("Deleting account: ", accountId2.Value));
                    service.Delete(Account.EntityLogicalName, accountId2.Value);
                }
                Console.WriteLine("Entity records have been deleted.");
            }
        }

        /// <summary>
        /// Waits for async job to complete
        /// </summary>
        /// <param name="asyncJobId"></param>
        public static void WaitForAsyncJobCompletion(CrmServiceClient service ,IEnumerable<Guid> asyncJobIds)
        {
            List<Guid> asyncJobList = new List<Guid>(asyncJobIds);
            ColumnSet cs = new ColumnSet("statecode", "asyncoperationid");
            int retryCount = 100;

            while (asyncJobList.Count != 0 && retryCount > 0)
            {
                // Retrieve the async operations based on the ids
                var crmAsyncJobs = service.RetrieveMultiple(
                    new QueryExpression("asyncoperation")
                    {
                        ColumnSet = cs,
                        Criteria = new FilterExpression
                        {
                            Conditions =
                            {
                                new ConditionExpression("asyncoperationid",
                                    ConditionOperator.In, asyncJobList.ToArray())
                            }
                        }
                    });

                // Check to see if the operations are completed and if so remove them from the Async Guid list
                foreach (var item in crmAsyncJobs.Entities)
                {
                    var crmAsyncJob = item.ToEntity<AsyncOperation>();
                    if (crmAsyncJob.StateCode.HasValue &&
                        crmAsyncJob.StateCode.Value == AsyncOperationState.Completed)
                        asyncJobList.Remove(crmAsyncJob.AsyncOperationId.Value);

                    Console.WriteLine(String.Concat("Async operation state is ",
                        crmAsyncJob.StateCode.Value.ToString(),
                        ", async operation id: ", crmAsyncJob.AsyncOperationId.Value.ToString()));
                }

                // If there are still jobs remaining, sleep the thread.
                if (asyncJobList.Count > 0)
                    System.Threading.Thread.Sleep(2000);

                retryCount--;
            }

            if (retryCount == 0 && asyncJobList.Count > 0)
            {
                for (int i = 0; i < asyncJobList.Count; i++)
                {
                    Console.WriteLine(String.Concat(
                        "The following async operation has not completed: ",
                        asyncJobList[i].ToString()));
                }
            }
        }

    }
}
