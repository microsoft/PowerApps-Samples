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
    /// Demonstrates how to enable and retrieve audit records for user access
    /// </summary>
    /// <remarks>
    /// This sample shows how to:
    /// - Enable user access auditing at the organization level
    /// - Create and update records to generate activity
    /// - Retrieve audit records that track user access
    /// - Display user access audit information
    ///
    /// Prerequisites:
    /// - System Administrator role
    /// - Auditing feature available in your Dataverse environment
    ///
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// </remarks>
    class Program
    {
        private static readonly List<EntityReference> entityStore = new();
        private static bool organizationAuditingFlag;
        private static bool userAccessAuditingFlag;
        private static Guid systemUserId;
        private static DateTime sampleStartTime;

        #region Sample Methods

        /// <summary>
        /// Sets up sample data by enabling auditing and creating an account
        /// </summary>
        private static void Setup(ServiceClient service)
        {
            // Record the start time for filtering audit records later
            sampleStartTime = DateTime.UtcNow;

            Console.WriteLine("Enabling auditing on the organization and for user access...");

            // Get the current user and organization IDs
            var whoAmIReq = new WhoAmIRequest();
            var whoAmIRes = (WhoAmIResponse)service.Execute(whoAmIReq);
            Guid orgId = whoAmIRes.OrganizationId;
            systemUserId = whoAmIRes.UserId;

            // Retrieve the organization's record
            var org = service.Retrieve("organization", orgId,
                new ColumnSet("organizationid", "isauditenabled", "isuseraccessauditenabled", "useraccessauditinginterval"));

            // Cache current settings
            organizationAuditingFlag = org.GetAttributeValue<bool>("isauditenabled");
            userAccessAuditingFlag = org.GetAttributeValue<bool>("isuseraccessauditenabled");

            // Enable auditing if not already enabled
            if (!organizationAuditingFlag || !userAccessAuditingFlag)
            {
                var orgToUpdate = new Entity("organization")
                {
                    Id = orgId,
                    ["isauditenabled"] = true,
                    ["isuseraccessauditenabled"] = true
                };
                service.Update(orgToUpdate);

                Console.WriteLine("Enabled auditing for the organization and for user access.");
                int? interval = org.GetAttributeValue<int?>("useraccessauditinginterval");
                Console.WriteLine($"Auditing interval is set to {interval ?? 0} hours.");
            }
            else
            {
                Console.WriteLine("Auditing was already enabled, so no auditing settings were changed.");
            }

            // Enable auditing on the account entity
            bool accountAuditingFlag = EnableEntityAuditing(service, "account", true);

            // Create an account
            Console.WriteLine("Creating an account...");
            var newAccount = new Entity("account")
            {
                ["name"] = "Example Account"
            };
            Guid accountId = service.Create(newAccount);
            entityStore.Add(new EntityReference("account", accountId));

            // Update the account to generate audit activity
            Console.WriteLine("Updating the account...");
            var accountToUpdate = new Entity("account")
            {
                Id = accountId,
                ["accountnumber"] = "1-A",
                ["accountcategorycode"] = new OptionSetValue(1), // Preferred Customer
                ["telephone1"] = "555-555-5555"
            };
            service.Update(accountToUpdate);

            Console.WriteLine("Setup complete. Account created and updated to generate audit records.");
            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrates retrieving and displaying user access audit records
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Console.WriteLine("Retrieving user access audit records...");
            Console.WriteLine();

            // Create query to retrieve user access audit records
            var query = new QueryExpression("audit")
            {
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression(LogicalOperator.And)
            };

            // Filter for user access audit actions
            query.Criteria.AddCondition("action", ConditionOperator.In,
                64, // UserAccessAuditStarted
                65, // UserAccessAuditStopped
                66, // UserAccessviaWebServices
                67  // UserAccessviaWeb
            );

            // Only retrieve records created during this sample run
            query.Criteria.AddCondition("createdon", ConditionOperator.GreaterEqual, sampleStartTime);

            // Optional: Filter to only show current user's records
            var filterAuditsRetrievedByUser = true;
            if (filterAuditsRetrievedByUser)
            {
                var userFilter = new FilterExpression(LogicalOperator.Or);
                userFilter.AddCondition("userid", ConditionOperator.Equal, systemUserId);
                userFilter.AddCondition("useridname", ConditionOperator.Equal, "SYSTEM");
                query.Criteria.AddFilter(userFilter);
            }

            // Execute the query
            var results = service.RetrieveMultiple(query);

            if (results.Entities.Count == 0)
            {
                Console.WriteLine("No user access audit records found for this session.");
                Console.WriteLine("Note: User access audit records may take time to appear based on the");
                Console.WriteLine("configured auditing interval (typically several hours).");
            }
            else
            {
                Console.WriteLine($"Retrieved {results.Entities.Count} audit record(s):");
                Console.WriteLine();

                foreach (Entity audit in results.Entities)
                {
                    var action = audit.GetAttributeValue<OptionSetValue>("action");
                    var userId = audit.GetAttributeValue<EntityReference>("userid");
                    var createdOn = audit.GetAttributeValue<DateTime>("createdon");
                    var operation = audit.GetAttributeValue<OptionSetValue>("operation");
                    var objectId = audit.GetAttributeValue<EntityReference>("objectid");

                    Console.WriteLine($"  Action: {GetAuditActionName(action?.Value)},");
                    Console.WriteLine($"  User: {userId?.Name ?? "Unknown"},");
                    Console.WriteLine($"  Created On: {createdOn.ToLocalTime()},");
                    Console.WriteLine($"  Operation: {GetAuditOperationName(operation?.Value)}");

                    // Display the name of the related object
                    if (objectId != null && !string.IsNullOrEmpty(objectId.Name))
                    {
                        Console.WriteLine($"  Related Record: {objectId.Name}");
                    }

                    Console.WriteLine();
                }
            }

            Console.WriteLine("User access audit retrieval complete.");
        }

        /// <summary>
        /// Cleans up sample data and restores audit settings
        /// </summary>
        private static void Cleanup(ServiceClient service, bool deleteCreatedRecords)
        {
            Console.WriteLine("Restoring audit settings...");

            // Restore organization auditing settings if they were changed
            if (!organizationAuditingFlag || !userAccessAuditingFlag)
            {
                var whoAmIReq = new WhoAmIRequest();
                var whoAmIRes = (WhoAmIResponse)service.Execute(whoAmIReq);
                Guid orgId = whoAmIRes.OrganizationId;

                var orgToUpdate = new Entity("organization")
                {
                    Id = orgId,
                    ["isauditenabled"] = organizationAuditingFlag,
                    ["isuseraccessauditenabled"] = userAccessAuditingFlag
                };
                service.Update(orgToUpdate);

                Console.WriteLine("Reverted organization and user access auditing to their previous values.");
            }
            else
            {
                Console.WriteLine("Auditing was enabled before the sample began, so no auditing settings were reverted.");
            }

            // Restore account entity auditing
            EnableEntityAuditing(service, "account", false);

            // Delete created records
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
        /// Enable or disable auditing on an entity
        /// </summary>
        private static bool EnableEntityAuditing(ServiceClient service, string entityLogicalName, bool flag)
        {
            var entityRequest = new RetrieveEntityRequest
            {
                LogicalName = entityLogicalName,
                EntityFilters = EntityFilters.Attributes
            };

            var entityResponse = (RetrieveEntityResponse)service.Execute(entityRequest);
            EntityMetadata entityMetadata = entityResponse.EntityMetadata;

            bool oldValue = entityMetadata.IsAuditEnabled?.Value ?? false;
            entityMetadata.IsAuditEnabled = new BooleanManagedProperty(flag);

            var updateEntityRequest = new UpdateEntityRequest { Entity = entityMetadata };
            service.Execute(updateEntityRequest);

            return oldValue;
        }

        /// <summary>
        /// Gets a friendly name for audit action codes
        /// </summary>
        private static string GetAuditActionName(int? actionCode)
        {
            return actionCode switch
            {
                1 => "Create",
                2 => "Update",
                3 => "Delete",
                64 => "User Access Audit Started",
                65 => "User Access Audit Stopped",
                66 => "User Access via Web Services",
                67 => "User Access via Web",
                _ => $"Unknown ({actionCode})"
            };
        }

        /// <summary>
        /// Gets a friendly name for audit operation codes
        /// </summary>
        private static string GetAuditOperationName(int? operationCode)
        {
            return operationCode switch
            {
                1 => "Create",
                2 => "Update",
                3 => "Delete",
                4 => "Access",
                _ => $"Unknown ({operationCode})"
            };
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
