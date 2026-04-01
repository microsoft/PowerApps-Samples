using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform_Dataverse_CodeSamples
{
    /// <summary>
    /// Demonstrates Create, Retrieve, Update, and Delete operations using late-bound Entity class.
    /// </summary>
    /// <remarks>
    /// Late-bound entities use the generic Entity class with string-based attribute access.
    /// This approach doesn't require generated early-bound classes and provides flexibility
    /// when working with dynamic entity structures.
    ///
    /// This sample demonstrates:
    /// - Creating an account using late-bound Entity with string indexer syntax
    /// - Retrieving entity with specific columns using ColumnSet
    /// - Updating attributes including Money and Boolean types
    /// - Setting an attribute to null
    /// - Deleting the entity
    ///
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// </remarks>
    /// <see cref="https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-create"/>
    /// <see cref="https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-retrieve"/>
    /// <see cref="https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-update-delete"/>
    /// <permission cref="https://github.com/microsoft/PowerApps-Samples/blob/master/LICENSE"/>
    class Program
    {
        /// <summary>
        /// Contains the application's configuration settings.
        /// </summary>
        IConfiguration Configuration { get; }

        /// <summary>
        /// List of entity references to delete during cleanup
        /// </summary>
        private readonly List<EntityReference> entityStore = new();

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

            try
            {
                // Run the sample operations
                app.Run(serviceClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred:");
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner exception: {0}", ex.InnerException.Message);
                }
            }
            finally
            {
                // Cleanup
                Console.WriteLine();
                Console.WriteLine("Press any key to undo environment data changes.");
                Console.ReadKey();

                app.Cleanup(serviceClient);
                serviceClient.Dispose();

                Console.WriteLine("Program complete. Press any key to exit.");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Main sample execution method demonstrating late-bound entity operations
        /// </summary>
        private void Run(ServiceClient serviceClient)
        {
            Console.WriteLine("=== Late-bound Entity Operations Sample ===");
            Console.WriteLine();

            // Setup: Create initial account
            Guid accountId = Setup(serviceClient);

            // Demonstrate: Perform CRUD operations
            Demonstrate(serviceClient, accountId);
        }

        /// <summary>
        /// Creates the initial account record for the sample
        /// </summary>
        private Guid Setup(ServiceClient serviceClient)
        {
            Console.WriteLine("--- Setup ---");

            // Instantiate an account object using late-bound Entity class.
            // The constructor takes the logical name of the entity.
            var account = new Entity("account");

            // Set the required attributes using string-based indexer.
            // For account entity, only the name is required.
            // See the Entity Metadata documentation to determine
            // which attributes must be set for each entity.
            account["name"] = "Fourth Coffee";

            // Create an account record named Fourth Coffee.
            Guid accountId = serviceClient.Create(account);

            Console.WriteLine("Created account '{0}' with ID: {1}",
                account["name"], accountId);

            // Store the entity reference for cleanup
            entityStore.Add(new EntityReference("account", accountId));

            Console.WriteLine();
            return accountId;
        }

        /// <summary>
        /// Demonstrates Retrieve, Update operations on the account
        /// </summary>
        private void Demonstrate(ServiceClient serviceClient, Guid accountId)
        {
            Console.WriteLine("--- Demonstrate ---");

            // Create a column set to define which attributes should be retrieved.
            // This is more efficient than retrieving all columns.
            var attributes = new ColumnSet(new string[] { "name", "ownerid" });

            // Retrieve the account and its name and ownerid attributes.
            var account = serviceClient.Retrieve("account", accountId, attributes);

            Console.WriteLine("Retrieved account:");
            Console.WriteLine("  Name: {0}", account["name"]);
            Console.WriteLine("  Owner ID: {0}", account.GetAttributeValue<EntityReference>("ownerid").Id);
            Console.WriteLine();

            // Update multiple attributes to demonstrate different data types
            Console.WriteLine("Updating account attributes...");

            // Update the postal code attribute (string type)
            account["address1_postalcode"] = "98052";
            Console.WriteLine("  Set address1_postalcode to '98052'");

            // The address 2 postal code was set accidentally, so set it to null.
            // Setting an attribute to null removes its value in Dataverse.
            account["address2_postalcode"] = null;
            Console.WriteLine("  Set address2_postalcode to null");

            // Shows use of Money data type.
            // Money is a special Dataverse type that includes currency precision.
            account["revenue"] = new Money(5000000);
            Console.WriteLine("  Set revenue to $5,000,000");

            // Shows use of boolean data type.
            account["creditonhold"] = false;
            Console.WriteLine("  Set creditonhold to false");

            // Update the account with all the changes.
            serviceClient.Update(account);
            Console.WriteLine("Account updated successfully.");
            Console.WriteLine();

            // Retrieve again to verify the updates
            Console.WriteLine("Verifying updates...");
            var updatedAccount = serviceClient.Retrieve(
                "account",
                accountId,
                new ColumnSet("name", "address1_postalcode", "address2_postalcode", "revenue", "creditonhold")
            );

            Console.WriteLine("Updated account values:");
            Console.WriteLine("  Name: {0}", updatedAccount.GetAttributeValue<string>("name"));
            Console.WriteLine("  Postal Code 1: {0}", updatedAccount.GetAttributeValue<string>("address1_postalcode"));
            Console.WriteLine("  Postal Code 2: {0}",
                updatedAccount.Contains("address2_postalcode")
                    ? updatedAccount["address2_postalcode"]
                    : "(null)");
            Console.WriteLine("  Revenue: {0}", updatedAccount.GetAttributeValue<Money>("revenue")?.Value ?? 0);
            Console.WriteLine("  Credit On Hold: {0}", updatedAccount.GetAttributeValue<bool>("creditonhold"));
            Console.WriteLine();
        }

        /// <summary>
        /// Deletes all records created by this sample
        /// </summary>
        private void Cleanup(ServiceClient serviceClient)
        {
            Console.WriteLine();
            Console.WriteLine("--- Cleanup ---");

            foreach (var entityRef in entityStore)
            {
                serviceClient.Delete(entityRef.LogicalName, entityRef.Id);
                Console.WriteLine("Deleted {0} with ID: {1}",
                    entityRef.LogicalName, entityRef.Id);
            }

            Console.WriteLine("Cleanup completed.");
        }
    }
}
