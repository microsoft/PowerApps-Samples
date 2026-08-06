using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates using duplicate detection with Create and Update operations
    /// </summary>
    /// <remarks>
    /// This sample shows how to:
    /// - Create and publish a duplicate detection rule
    /// - Use SuppressDuplicateDetection parameter with CreateRequest
    /// - Use SuppressDuplicateDetection parameter with UpdateRequest
    /// - Control whether duplicate detection fires during CRUD operations
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
        private static Guid ruleId;

        #region Sample Methods

        /// <summary>
        /// Sets up sample data including an account and duplicate detection rule
        /// </summary>
        private static void Setup(ServiceClient service)
        {
            Console.WriteLine("Creating initial account record...");

            // Create an account record named Fourth Coffee
            var account = new Entity("account")
            {
                ["name"] = "Fourth Coffee",
                ["accountnumber"] = "ACC005"
            };
            Guid accountId = service.Create(account);
            entityStore.Add(new EntityReference("account", accountId));
            Console.WriteLine($"  Created account: {account["name"]} ({account["accountnumber"]})");
            Console.WriteLine();

            Console.WriteLine("Creating duplicate detection rule...");

            // Create a duplicate detection rule for accounts with same account number
            var rule = new Entity("duplicaterule")
            {
                ["name"] = "DuplicateRule: Accounts with the same Account Number",
                ["baseentityname"] = "account",
                ["matchingentityname"] = "account"
            };
            ruleId = service.Create(rule);
            entityStore.Add(new EntityReference("duplicaterule", ruleId));
            Console.WriteLine($"  Rule created: {ruleId}");

            // Create a duplicate detection rule condition
            var condition = new Entity("duplicaterulecondition")
            {
                ["baseattributename"] = "accountnumber",
                ["matchingattributename"] = "accountnumber",
                ["operatorcode"] = new OptionSetValue(0), // Exact match
                ["regardingobjectid"] = new EntityReference("duplicaterule", ruleId)
            };
            Guid conditionId = service.Create(condition);
            entityStore.Add(new EntityReference("duplicaterulecondition", conditionId));
            Console.WriteLine("  Rule condition created");

            Console.WriteLine("Publishing duplicate detection rule...");

            // Publish the duplicate detection rule
            var publishRequest = new PublishDuplicateRuleRequest
            {
                DuplicateRuleId = ruleId
            };
            service.Execute(publishRequest);

            // Wait for the rule to be published (poll statuscode until it's Published = 2)
            Console.WriteLine("  Waiting for rule to publish...");
            int attempts = 0;
            while (attempts < 20)
            {
                var retrievedRule = service.Retrieve("duplicaterule", ruleId, new ColumnSet("statuscode"));
                var statusCode = retrievedRule.GetAttributeValue<OptionSetValue>("statuscode");

                // StatusCode 2 = Published
                if (statusCode != null && statusCode.Value == 2)
                {
                    Console.WriteLine("  Rule published successfully");
                    break;
                }

                attempts++;
                Thread.Sleep(1000);
            }

            if (attempts >= 20)
            {
                Console.WriteLine("  Warning: Rule may still be publishing");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrates using SuppressDuplicateDetection parameter with Create and Update
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Console.WriteLine("Demonstrating duplicate detection control with CRUD operations...");
            Console.WriteLine();

            // Create an account with a duplicate account number
            // Using SuppressDuplicateDetection = true to bypass duplicate detection
            Console.WriteLine("Creating duplicate account with SuppressDuplicateDetection = true...");

            var duplicateAccount = new Entity("account")
            {
                ["name"] = "Proseware, Inc.",
                ["accountnumber"] = "ACC005" // Same as Fourth Coffee
            };

            var createRequest = new CreateRequest
            {
                Target = duplicateAccount
            };
            createRequest.Parameters.Add("SuppressDuplicateDetection", true);

            var createResponse = (CreateResponse)service.Execute(createRequest);
            Guid dupAccountId = createResponse.id;
            entityStore.Add(new EntityReference("account", dupAccountId));

            Console.WriteLine($"  Created: {duplicateAccount["name"]} ({duplicateAccount["accountnumber"]})");
            Console.WriteLine("  Duplicate detection was suppressed, so the duplicate was created");
            Console.WriteLine();

            // Retrieve the account
            Console.WriteLine("Retrieving the account...");
            var retrievedAccount = service.Retrieve("account", dupAccountId,
                new ColumnSet("name", "accountnumber"));
            Console.WriteLine($"  Retrieved: {retrievedAccount["name"]}");
            Console.WriteLine();

            // Update the account with a new account number
            // Using SuppressDuplicateDetection = false to activate duplicate detection
            Console.WriteLine("Updating account with SuppressDuplicateDetection = false...");

            retrievedAccount["accountnumber"] = "ACC006";

            var updateRequest = new UpdateRequest
            {
                Target = retrievedAccount
            };
            updateRequest["SuppressDuplicateDetection"] = false;

            service.Execute(updateRequest);

            Console.WriteLine($"  Updated account number to: {retrievedAccount["accountnumber"]}");
            Console.WriteLine("  Duplicate detection was active, update succeeded (no duplicates found)");
            Console.WriteLine();

            Console.WriteLine("Duplicate detection CRUD operations complete.");
        }

        /// <summary>
        /// Cleans up sample data including unpublishing the rule
        /// </summary>
        private static void Cleanup(ServiceClient service, bool deleteCreatedRecords)
        {
            Console.WriteLine("Cleaning up...");

            if (deleteCreatedRecords && entityStore.Count > 0)
            {
                // Unpublish the duplicate detection rule before deleting
                try
                {
                    Console.WriteLine("Unpublishing duplicate detection rule...");
                    var unpublishRequest = new UnpublishDuplicateRuleRequest
                    {
                        DuplicateRuleId = ruleId
                    };
                    service.Execute(unpublishRequest);
                    Console.WriteLine("  Rule unpublished");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  Warning: Could not unpublish rule: {ex.Message}");
                }

                // Delete records in reverse order to handle dependencies
                Console.WriteLine($"Deleting {entityStore.Count} created record(s)...");
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
