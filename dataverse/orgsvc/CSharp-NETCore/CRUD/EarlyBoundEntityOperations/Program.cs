using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform_Dataverse_CodeSamples
{
    /// <summary>
    /// Demonstrates Create, Retrieve, Update, and Delete operations on account entities
    /// using various attribute types including OptionSet, Money, Boolean, EntityReference, and Memo.
    /// </summary>
    /// <remarks>
    /// This sample was migrated from a legacy early-bound sample to use late-bound Entity class
    /// for better consistency with modern .NET patterns. It demonstrates the same operations
    /// that were originally done with strongly-typed early-bound classes.
    ///
    /// This sample demonstrates:
    /// - Creating an account entity with basic attributes
    /// - Retrieving entity with specific columns using ColumnSet
    /// - Working with version numbers (BigInt attribute)
    /// - Updating various attribute types:
    ///   * String attributes (postal codes)
    ///   * OptionSet attributes (address type, shipping method, industry code)
    ///   * Money attributes (revenue)
    ///   * Boolean attributes (credit on hold)
    ///   * EntityReference attributes (parent account)
    ///   * Memo attributes (description)
    /// - Setting attributes to null
    /// - Deleting entities
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
        /// Main sample execution method
        /// </summary>
        private void Run(ServiceClient serviceClient)
        {
            Console.WriteLine("=== Early-bound Entity Operations Sample (Converted to Late-bound) ===");
            Console.WriteLine();

            // Setup: Create prerequisite records
            Guid parentAccountId = Setup(serviceClient);

            // Demonstrate: Perform CRUD operations with various attribute types
            Demonstrate(serviceClient, parentAccountId);
        }

        /// <summary>
        /// Creates prerequisite records for the sample
        /// </summary>
        private Guid Setup(ServiceClient serviceClient)
        {
            Console.WriteLine("--- Setup ---");

            // Create a parent account for demonstrating EntityReference
            var parentAccount = new Entity("account");
            parentAccount["name"] = "Sample Parent Account";

            Guid parentAccountId = serviceClient.Create(parentAccount);
            Console.WriteLine("Created parent account with ID: {0}", parentAccountId);

            // Store for cleanup
            entityStore.Add(new EntityReference("account", parentAccountId));

            Console.WriteLine();
            return parentAccountId;
        }

        /// <summary>
        /// Demonstrates Create, Retrieve, Update operations with various attribute types
        /// </summary>
        private void Demonstrate(ServiceClient serviceClient, Guid parentAccountId)
        {
            Console.WriteLine("--- Demonstrate ---");

            // CREATE: Instantiate an account object.
            // See the Entity Metadata documentation to determine
            // which attributes must be set for each entity.
            var account = new Entity("account");
            account["name"] = "Fourth Coffee";

            // Create an account record named Fourth Coffee.
            Guid accountId = serviceClient.Create(account);
            Console.WriteLine("Created account 'Fourth Coffee' with ID: {0}", accountId);
            Console.WriteLine();

            // Store for cleanup
            entityStore.Add(new EntityReference("account", accountId));

            // RETRIEVE: Retrieve the account containing several of its attributes.
            var cols = new ColumnSet(
                new string[] { "name", "address1_postalcode", "lastusedincampaign", "versionnumber" });

            var retrievedAccount = serviceClient.Retrieve("account", accountId, cols);
            Console.WriteLine("Retrieved account:");
            Console.WriteLine("  Name: {0}", retrievedAccount.GetAttributeValue<string>("name"));

            // Retrieve version number of the account. Shows BigInt attribute usage.
            long? versionNumber = retrievedAccount.GetAttributeValue<long?>("versionnumber");
            if (versionNumber != null)
            {
                Console.WriteLine("  Version #: {0}", versionNumber);
            }
            Console.WriteLine();

            // UPDATE: Update the account with various attribute types
            Console.WriteLine("Updating account with various attribute types...");

            // Update the postal code attribute (string type)
            retrievedAccount["address1_postalcode"] = "98052";
            Console.WriteLine("  Set address1_postalcode to '98052'");

            // The address 2 postal code was set accidentally, so set it to null.
            retrievedAccount["address2_postalcode"] = null;
            Console.WriteLine("  Set address2_postalcode to null");

            // Shows usage of option set (picklist) enumerations.
            // In early-bound code, these used generated enum values.
            // In late-bound code, we use OptionSetValue with integer values.

            // address1_addresstypecode: 1 = Primary (originally AccountAddress1_AddressTypeCode.Primary)
            retrievedAccount["address1_addresstypecode"] = new OptionSetValue(1);
            Console.WriteLine("  Set address1_addresstypecode to 1 (Primary)");

            // address1_shippingmethodcode: 5 = DHL (originally AccountAddress1_ShippingMethodCode.DHL)
            retrievedAccount["address1_shippingmethodcode"] = new OptionSetValue(5);
            Console.WriteLine("  Set address1_shippingmethodcode to 5 (DHL)");

            // industrycode: 1 = Agriculture and Non-petrol Natural Resource Extraction
            // (originally AccountIndustryCode.AgricultureandNonpetrolNaturalResourceExtraction)
            retrievedAccount["industrycode"] = new OptionSetValue(1);
            Console.WriteLine("  Set industrycode to 1 (Agriculture and Non-petrol Natural Resource Extraction)");

            // Shows use of a Money value.
            retrievedAccount["revenue"] = new Money(5000000);
            Console.WriteLine("  Set revenue to $5,000,000");

            // Shows use of a Boolean value.
            retrievedAccount["creditonhold"] = false;
            Console.WriteLine("  Set creditonhold to false");

            // Shows use of EntityReference.
            retrievedAccount["parentaccountid"] = new EntityReference("account", parentAccountId);
            Console.WriteLine("  Set parentaccountid to reference parent account");

            // Shows use of Memo attribute (multi-line text).
            retrievedAccount["description"] = "Account for Fourth Coffee.";
            Console.WriteLine("  Set description memo field");

            // Update the account record with all changes.
            serviceClient.Update(retrievedAccount);
            Console.WriteLine();
            Console.WriteLine("Account updated successfully.");
            Console.WriteLine();

            // Verify the updates
            Console.WriteLine("Verifying updates...");
            var verifyColumns = new ColumnSet(
                "name",
                "address1_postalcode",
                "address2_postalcode",
                "address1_addresstypecode",
                "address1_shippingmethodcode",
                "industrycode",
                "revenue",
                "creditonhold",
                "parentaccountid",
                "description"
            );

            var updatedAccount = serviceClient.Retrieve("account", accountId, verifyColumns);

            Console.WriteLine("Updated account values:");
            Console.WriteLine("  Name: {0}", updatedAccount.GetAttributeValue<string>("name"));
            Console.WriteLine("  Postal Code 1: {0}", updatedAccount.GetAttributeValue<string>("address1_postalcode"));
            Console.WriteLine("  Postal Code 2: {0}",
                updatedAccount.Contains("address2_postalcode")
                    ? updatedAccount["address2_postalcode"]
                    : "(null)");
            Console.WriteLine("  Address Type Code: {0}",
                updatedAccount.GetAttributeValue<OptionSetValue>("address1_addresstypecode")?.Value ?? 0);
            Console.WriteLine("  Shipping Method Code: {0}",
                updatedAccount.GetAttributeValue<OptionSetValue>("address1_shippingmethodcode")?.Value ?? 0);
            Console.WriteLine("  Industry Code: {0}",
                updatedAccount.GetAttributeValue<OptionSetValue>("industrycode")?.Value ?? 0);
            Console.WriteLine("  Revenue: ${0:N0}", updatedAccount.GetAttributeValue<Money>("revenue")?.Value ?? 0);
            Console.WriteLine("  Credit On Hold: {0}", updatedAccount.GetAttributeValue<bool>("creditonhold"));
            Console.WriteLine("  Parent Account ID: {0}",
                updatedAccount.GetAttributeValue<EntityReference>("parentaccountid")?.Id ?? Guid.Empty);
            Console.WriteLine("  Description: {0}", updatedAccount.GetAttributeValue<string>("description"));
            Console.WriteLine();
        }

        /// <summary>
        /// Deletes all records created by this sample
        /// </summary>
        private void Cleanup(ServiceClient serviceClient)
        {
            Console.WriteLine();
            Console.WriteLine("--- Cleanup ---");

            // Delete in reverse order to handle parent-child relationships
            // (delete child account before parent account)
            for (int i = entityStore.Count - 1; i >= 0; i--)
            {
                var entityRef = entityStore[i];
                serviceClient.Delete(entityRef.LogicalName, entityRef.Id);
                Console.WriteLine("Deleted {0} with ID: {1}",
                    entityRef.LogicalName, entityRef.Id);
            }

            Console.WriteLine("Cleanup completed.");
        }
    }
}
