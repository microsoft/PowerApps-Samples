using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates how to detect multiple duplicate records using BulkDetectDuplicatesRequest
    /// </summary>
    /// <remarks>
    /// This sample shows how to:
    /// - Create duplicate account records
    /// - Create a duplicate detection rule programmatically
    /// - Add conditions to the duplicate rule
    /// - Publish the duplicate rule
    /// - Use BulkDetectDuplicatesRequest to detect duplicates
    /// - Wait for the async job to complete
    /// - Query duplicate records to verify detection
    ///
    /// Prerequisites:
    /// - System Administrator or System Customizer role
    ///
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// </remarks>
    class Program
    {
        private static readonly List<EntityReference> entityStore = new();
        private static Guid bulkDetectJobId;

        #region Sample Methods

        /// <summary>
        /// Sets up sample data including duplicate accounts and a duplicate detection rule
        /// </summary>
        private static void Setup(ServiceClient service)
        {
            Console.WriteLine("Creating duplicate account records...");

            string accountName = "Contoso, Ltd";
            string websiteUrl = "http://www.contoso.com/";

            // Create duplicate accounts
            for (int i = 0; i < 2; i++)
            {
                var account = new Entity("account")
                {
                    ["name"] = accountName,
                    ["websiteurl"] = websiteUrl
                };
                Guid accountId = service.Create(account);
                entityStore.Add(new EntityReference("account", accountId));
            }
            Console.WriteLine($"  Created 2 duplicate accounts (Name={accountName}, Website={websiteUrl})");

            // Create a non-duplicate account
            string nonDuplicateName = "Contoso Pharmaceuticals";
            var distinctAccount = new Entity("account")
            {
                ["name"] = nonDuplicateName,
                ["websiteurl"] = websiteUrl
            };
            Guid distinctAccountId = service.Create(distinctAccount);
            entityStore.Add(new EntityReference("account", distinctAccountId));
            Console.WriteLine($"  Created non-duplicate account (Name={nonDuplicateName}, Website={websiteUrl})");
            Console.WriteLine();

            Console.WriteLine("Creating duplicate detection rule...");
            // Create a duplicate detection rule
            var rule = new Entity("duplicaterule")
            {
                ["name"] = "Accounts with the same Account name and website url",
                ["baseentityname"] = "account",
                ["matchingentityname"] = "account"
            };
            Guid ruleId = service.Create(rule);
            entityStore.Add(new EntityReference("duplicaterule", ruleId));
            Console.WriteLine($"  Rule created: {ruleId}");

            // Create rule conditions
            var nameCondition = new Entity("duplicaterulecondition")
            {
                ["baseattributename"] = "name",
                ["matchingattributename"] = "name",
                ["operatorcode"] = new OptionSetValue(0), // Exact match
                ["regardingobjectid"] = new EntityReference("duplicaterule", ruleId)
            };
            Guid nameConditionId = service.Create(nameCondition);
            entityStore.Add(new EntityReference("duplicaterulecondition", nameConditionId));

            var websiteCondition = new Entity("duplicaterulecondition")
            {
                ["baseattributename"] = "websiteurl",
                ["matchingattributename"] = "websiteurl",
                ["operatorcode"] = new OptionSetValue(0), // Exact match
                ["regardingobjectid"] = new EntityReference("duplicaterule", ruleId)
            };
            Guid websiteConditionId = service.Create(websiteCondition);
            entityStore.Add(new EntityReference("duplicaterulecondition", websiteConditionId));
            Console.WriteLine("  Rule conditions created");

            Console.WriteLine("Publishing duplicate detection rule...");
            var publishRequest = new PublishDuplicateRuleRequest
            {
                DuplicateRuleId = ruleId
            };
            var publishResponse = (PublishDuplicateRuleResponse)service.Execute(publishRequest);

            // Wait for the rule to publish
            Console.WriteLine("  Waiting for rule to publish...");
            WaitForAsyncJobToFinish(service, publishResponse.JobId, 120);
            Console.WriteLine("  Rule published successfully");
            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrates bulk duplicate detection
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Console.WriteLine("Creating BulkDetectDuplicatesRequest...");
            var request = new BulkDetectDuplicatesRequest
            {
                JobName = "Detect Duplicate Accounts",
                Query = new QueryExpression("account")
                {
                    ColumnSet = new ColumnSet(true)
                },
                RecurrencePattern = string.Empty,
                RecurrenceStartTime = DateTime.Now,
                ToRecipients = Array.Empty<Guid>(),
                CCRecipients = Array.Empty<Guid>()
            };

            Console.WriteLine("Executing BulkDetectDuplicatesRequest...");
            var response = (BulkDetectDuplicatesResponse)service.Execute(request);
            bulkDetectJobId = response.JobId;

            // Track the job for cleanup
            entityStore.Add(new EntityReference("asyncoperation", bulkDetectJobId));

            Console.WriteLine($"  Job ID: {bulkDetectJobId}");
            Console.WriteLine("  Waiting for duplicate detection job to complete...");

            WaitForAsyncJobToFinish(service, bulkDetectJobId, 240);

            // Query for duplicate records
            Console.WriteLine("Querying for detected duplicates...");
            var duplicateQuery = new QueryByAttribute("duplicaterecord")
            {
                ColumnSet = new ColumnSet(true)
            };
            duplicateQuery.Attributes.Add("asyncoperationid");
            duplicateQuery.Values.Add(bulkDetectJobId);

            var duplicateResults = service.RetrieveMultiple(duplicateQuery);

            if (duplicateResults.Entities.Count > 0)
            {
                Console.WriteLine($"  Found {duplicateResults.Entities.Count} duplicate record(s):");

                var duplicateIds = new HashSet<Guid>();
                foreach (var duplicate in duplicateResults.Entities)
                {
                    var baseRecordId = duplicate.GetAttributeValue<EntityReference>("baserecordid");
                    if (baseRecordId != null)
                    {
                        duplicateIds.Add(baseRecordId.Id);
                        Console.WriteLine($"    Base Record ID: {baseRecordId.Id}");
                    }
                }

                // Verify that expected duplicates were found
                var expectedDuplicates = entityStore
                    .Where(e => e.LogicalName == "account")
                    .Take(2) // First 2 accounts are duplicates
                    .Select(e => e.Id)
                    .ToList();

                bool allFound = expectedDuplicates.All(id => duplicateIds.Contains(id));
                if (allFound)
                {
                    Console.WriteLine("  All expected duplicate accounts were detected successfully!");
                }
                else
                {
                    Console.WriteLine("  Warning: Not all expected duplicates were detected.");
                }
            }
            else
            {
                Console.WriteLine("  No duplicates found.");
                Console.WriteLine("  Note: Duplicate detection may take time to process.");
            }

            Console.WriteLine();
            Console.WriteLine("Bulk duplicate detection complete.");
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

                // Delete in reverse order to handle dependencies
                for (int i = entityStore.Count - 1; i >= 0; i--)
                {
                    var entityRef = entityStore[i];
                    try
                    {
                        service.Delete(entityRef.LogicalName, entityRef.Id);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  Warning: Could not delete {entityRef.LogicalName} {entityRef.Id}: {ex.Message}");
                    }
                }
                Console.WriteLine("Records deleted.");
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Waits for an async job to complete
        /// </summary>
        private static void WaitForAsyncJobToFinish(ServiceClient service, Guid jobId, int maxTimeSeconds)
        {
            for (int i = 0; i < maxTimeSeconds; i++)
            {
                var asyncJob = service.Retrieve("asyncoperation", jobId, new ColumnSet("statecode"));

                var stateCode = asyncJob.GetAttributeValue<OptionSetValue>("statecode");

                // StateCode 3 = Completed
                if (stateCode != null && stateCode.Value == 3)
                {
                    return;
                }

                Thread.Sleep(1000);
            }

            throw new Exception($"Exceeded maximum time of {maxTimeSeconds} seconds waiting for asynchronous job to complete");
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
