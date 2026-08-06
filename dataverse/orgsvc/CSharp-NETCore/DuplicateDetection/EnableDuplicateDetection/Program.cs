using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates how to enable duplicate detection for the organization and entities
    /// </summary>
    /// <remarks>
    /// This sample shows how to:
    /// - Enable duplicate detection at the organization level
    /// - Enable duplicate detection for a specific entity (account)
    /// - Publish duplicate detection rules
    /// - Create duplicate records
    /// - Retrieve duplicate records using RetrieveDuplicatesRequest
    ///
    /// Prerequisites:
    /// - System Administrator or System Customizer role
    /// - At least one duplicate detection rule must exist for the account entity
    ///
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// </remarks>
    class Program
    {
        private static readonly List<EntityReference> entityStore = new();

        #region Sample Methods

        /// <summary>
        /// Sets up duplicate detection by enabling it and publishing rules
        /// </summary>
        private static void Setup(ServiceClient service)
        {
            Console.WriteLine("Enabling duplicate detection...");

            // Enable duplicate detection for the organization
            EnableDuplicateDetectionForOrg(service);

            // Enable duplicate detection for the account entity
            EnableDuplicateDetectionForEntity(service, "account");

            // Publish all duplicate rules for the account entity
            PublishRulesForEntity(service, "account");

            Console.WriteLine("Duplicate detection enabled and rules published.");
            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrates creating duplicate records and retrieving them
        /// </summary>
        private static void Run(ServiceClient service)
        {
            // Create duplicate account records
            Console.WriteLine("Creating duplicate account records...");
            var account1 = new Entity("account")
            {
                ["name"] = "Microsoft"
            };
            Guid accountId1 = service.Create(account1);
            entityStore.Add(new EntityReference("account", accountId1));

            var account2 = new Entity("account")
            {
                ["name"] = "Microsoft"
            };
            Guid accountId2 = service.Create(account2);
            entityStore.Add(new EntityReference("account", accountId2));

            Console.WriteLine($"Created duplicate records:");
            Console.WriteLine($"  Account 1: {accountId1}");
            Console.WriteLine($"  Account 2: {accountId2}");
            Console.WriteLine();

            // Retrieve duplicates
            Console.WriteLine("Retrieving duplicate records...");
            var request = new RetrieveDuplicatesRequest
            {
                BusinessEntity = new Entity("account") { ["name"] = "Microsoft" },
                MatchingEntityName = "account",
                PagingInfo = new PagingInfo { PageNumber = 1, Count = 50 }
            };

            var response = (RetrieveDuplicatesResponse)service.Execute(request);

            if (response.DuplicateCollection.Entities.Count > 0)
            {
                Console.WriteLine($"Found {response.DuplicateCollection.Entities.Count} duplicate(s):");
                foreach (var duplicate in response.DuplicateCollection.Entities)
                {
                    string name = duplicate.GetAttributeValue<string>("name");
                    Guid id = duplicate.Id;
                    Console.WriteLine($"  {name} (ID: {id})");
                }
            }
            else
            {
                Console.WriteLine("No duplicates found.");
                Console.WriteLine("Note: Duplicate detection may take a moment to process after enabling.");
            }

            Console.WriteLine();
            Console.WriteLine("Duplicate detection operations complete.");
        }

        /// <summary>
        /// Cleans up sample data
        /// </summary>
        private static void Cleanup(ServiceClient service, bool deleteCreatedRecords)
        {
            Console.WriteLine("Cleaning up...");
            if (deleteCreatedRecords && entityStore.Count > 0)
            {
                Console.WriteLine($"Deleting {entityStore.Count} created record(s)...");
                foreach (var entityRef in entityStore)
                {
                    service.Delete(entityRef.LogicalName, entityRef.Id);
                }
                Console.WriteLine("Records deleted.");
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Enables duplicate detection for the organization
        /// </summary>
        private static void EnableDuplicateDetectionForOrg(ServiceClient service)
        {
            // Retrieve the organization ID
            Guid? orgId = RetrieveOrganizationId(service);
            if (!orgId.HasValue)
            {
                Console.WriteLine("Could not retrieve organization ID.");
                return;
            }

            Console.WriteLine($"Enabling duplicate detection for organization: {orgId.Value}");

            // Enable duplicate detection for each type
            var organization = new Entity("organization")
            {
                Id = orgId.Value,
                ["isduplicatedetectionenabled"] = true,
                ["isduplicatedetectionenabledforimport"] = true,
                ["isduplicatedetectionenabledforofflinesync"] = true,
                ["isduplicatedetectionenabledforonlinecreateupdate"] = true
            };

            service.Update(organization);
            Console.WriteLine("Organization duplicate detection enabled.");
        }

        /// <summary>
        /// Enables duplicate detection for a specific entity
        /// </summary>
        private static void EnableDuplicateDetectionForEntity(ServiceClient service, string entityName)
        {
            Console.WriteLine($"Retrieving entity metadata for {entityName}...");

            // Retrieve the entity metadata
            var retrieveEntityRequest = new RetrieveEntityRequest
            {
                RetrieveAsIfPublished = true,
                LogicalName = entityName
            };

            var retrieveEntityResponse = (RetrieveEntityResponse)service.Execute(retrieveEntityRequest);
            var entityMetadata = retrieveEntityResponse.EntityMetadata;

            Console.WriteLine($"Enabling duplicate detection for {entityName}...");

            // Update the duplicate detection flag
            entityMetadata.IsDuplicateDetectionEnabled = new BooleanManagedProperty(true);

            // Update the entity metadata
            service.Execute(new UpdateEntityRequest { Entity = entityMetadata });

            Console.WriteLine($"Publishing {entityName} entity...");

            // Publish the entity
            var publishRequest = new PublishXmlRequest
            {
                ParameterXml = $"<importexportxml><entities><entity>{entityName}</entity></entities></importexportxml>"
            };

            service.Execute(publishRequest);
            Console.WriteLine($"Entity {entityName} published.");
        }

        /// <summary>
        /// Publishes all duplicate rules for an entity and waits for completion
        /// </summary>
        private static void PublishRulesForEntity(ServiceClient service, string entityName)
        {
            Console.WriteLine($"Retrieving duplicate rules for {entityName}...");

            // Retrieve all rules for the entity
            var query = new QueryByAttribute("duplicaterule")
            {
                ColumnSet = new ColumnSet("duplicateruleid"),
                Attributes = { "matchingentityname" },
                Values = { entityName }
            };

            var rules = service.RetrieveMultiple(query);

            if (rules.Entities.Count == 0)
            {
                Console.WriteLine($"No duplicate rules found for {entityName}.");
                return;
            }

            Console.WriteLine($"Found {rules.Entities.Count} duplicate rule(s). Publishing...");

            var asyncJobIds = new List<Guid>();
            foreach (var rule in rules.Entities)
            {
                Console.WriteLine($"  Publishing duplicate rule: {rule.Id}");

                // Publish each rule and get the job ID since it is async
                var publishRequest = new PublishDuplicateRuleRequest
                {
                    DuplicateRuleId = rule.Id
                };

                var publishResponse = (PublishDuplicateRuleResponse)service.Execute(publishRequest);
                asyncJobIds.Add(publishResponse.JobId);
            }

            // Wait until all rules are published
            WaitForAsyncJobCompletion(service, asyncJobIds);
            Console.WriteLine("All duplicate rules published successfully.");
        }

        /// <summary>
        /// Retrieves the organization ID
        /// </summary>
        private static Guid? RetrieveOrganizationId(ServiceClient service)
        {
            var query = new QueryExpression("organization")
            {
                ColumnSet = new ColumnSet("organizationid"),
                PageInfo = new PagingInfo { PageNumber = 1, Count = 1 }
            };

            var entities = service.RetrieveMultiple(query);

            if (entities != null && entities.Entities.Count > 0)
            {
                return entities.Entities[0].Id;
            }

            return null;
        }

        /// <summary>
        /// Waits for async jobs to complete
        /// </summary>
        private static void WaitForAsyncJobCompletion(ServiceClient service, IEnumerable<Guid> asyncJobIds)
        {
            var asyncJobList = new List<Guid>(asyncJobIds);
            var columnSet = new ColumnSet("statecode", "asyncoperationid");
            int retryCount = 100;

            Console.WriteLine("Waiting for async operations to complete...");

            while (asyncJobList.Count > 0 && retryCount > 0)
            {
                // Retrieve the async operations based on the IDs
                var query = new QueryExpression("asyncoperation")
                {
                    ColumnSet = columnSet,
                    Criteria = new FilterExpression
                    {
                        Conditions =
                        {
                            new ConditionExpression("asyncoperationid",
                                ConditionOperator.In, asyncJobList.ToArray())
                        }
                    }
                };

                var asyncJobs = service.RetrieveMultiple(query);

                // Check if operations are completed and remove them from the list
                foreach (var job in asyncJobs.Entities)
                {
                    var stateCode = job.GetAttributeValue<OptionSetValue>("statecode");
                    var asyncOpId = job.GetAttributeValue<Guid>("asyncoperationid");

                    // StateCode 3 = Completed
                    if (stateCode != null && stateCode.Value == 3)
                    {
                        asyncJobList.Remove(asyncOpId);
                        Console.WriteLine($"  Async operation completed: {asyncOpId}");
                    }
                }

                // If there are still jobs remaining, wait before checking again
                if (asyncJobList.Count > 0)
                {
                    Thread.Sleep(2000);
                }

                retryCount--;
            }

            if (retryCount == 0 && asyncJobList.Count > 0)
            {
                Console.WriteLine("Warning: Some async operations did not complete:");
                foreach (var jobId in asyncJobList)
                {
                    Console.WriteLine($"  - {jobId}");
                }
            }
        }

        #endregion

        #region Application Setup

        /// <summary>
        /// Contains the application's configuration settings.
        /// </summary>
        IConfiguration Configuration { get; }

        /// <summary>
        /// Constructor. Loads the application configuration settings from a JSON file.
        /// </summary>
        Program()
        {
            // Get the path to the appsettings file. If the environment variable is set,
            // use that file path. Otherwise, use the runtime folder's settings file.
            string? path = Environment.GetEnvironmentVariable("DATAVERSE_APPSETTINGS");
            if (path == null) path = "appsettings.json";

            // Load the app's configuration settings from the JSON file.
            Configuration = new ConfigurationBuilder()
                .AddJsonFile(path, optional: false, reloadOnChange: true)
                .Build();
        }

        static void Main(string[] args)
        {
            Program app = new();

            // Create a Dataverse service client using the default connection string.
            ServiceClient serviceClient =
                new(app.Configuration.GetConnectionString("default"));

            if (!serviceClient.IsReady)
            {
                Console.WriteLine("Failed to connect to Dataverse.");
                Console.WriteLine("Error: {0}", serviceClient.LastError);
                return;
            }

            Console.WriteLine("Connected to Dataverse.");
            Console.WriteLine();

            bool deleteCreatedRecords = true;

            try
            {
                Setup(serviceClient);
                Run(serviceClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred:");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Cleanup(serviceClient, deleteCreatedRecords);

                Console.WriteLine();
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                serviceClient.Dispose();
            }
        }

        #endregion
    }
}
