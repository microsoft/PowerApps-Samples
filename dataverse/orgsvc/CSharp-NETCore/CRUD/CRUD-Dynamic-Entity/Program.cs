using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform_Dataverse_CodeSamples
{
    /// <summary>
    /// Demonstrates basic Create, Retrieve, Update, and Delete operations
    /// using dynamic (late-bound) entities.
    /// </summary>
    /// <remarks>
    /// This sample shows how to work with Dataverse entities using late-bound syntax,
    /// where entity types and attributes are specified as strings rather than
    /// strongly-typed classes.
    ///
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// You will be prompted in the default browser to enter a password.
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
        static IConfiguration Configuration { get; }

        /// <summary>
        /// Constructor. Loads the application configuration settings from a JSON file.
        /// </summary>
        static Program()
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
            // Entity name and reference collection.
            Dictionary<string, EntityReference> entityStore;

            // Create a Dataverse service client using the default connection string.
            ServiceClient serviceClient =
                new(Configuration.GetConnectionString("default"));

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
                // Pre-create any table rows that the Run() method requires.
                Setup(serviceClient, out entityStore);

                // Execute the main logic of this program.
                Run(serviceClient, entityStore);

                // Pause program execution before resource cleanup.
                Console.WriteLine();
                Console.WriteLine("Press any key to undo environment data changes.");
                Console.ReadKey();

                // In Dataverse, delete any created table rows and then dispose the service connection.
                Cleanup(serviceClient, entityStore);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred:");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                serviceClient.Dispose();
            }
        }

        /// <summary>
        /// Executes the code being demonstrated by this program.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Entity name and reference collection.</param>
        /// <returns>True if successful; otherwise false.</returns>
        static public bool Run(IOrganizationService service,
            Dictionary<string, EntityReference> entityStore)
        {
            try
            {
                // Instantiate an account object using late-bound Entity class.
                // Note: With late-bound entities, the entity logical name is passed as a string.
                Entity newAccount = new Entity("account");

                // Set the required attributes. For account, only the name is required.
                // Note: Attributes are accessed using string indexer syntax.
                newAccount["name"] = "Fourth Coffee";

                // Set any other attribute values.
                newAccount["address2_postalcode"] = "98074";

                Console.WriteLine("Creating account entity named '{0}'...", newAccount["name"]);

                // Create an account record named Fourth Coffee.
                Guid accountid = service.Create(newAccount);

                // Store the created entity for cleanup.
                entityStore.Add("account", new EntityReference("account", accountid));

                Console.WriteLine("Created {0} entity named {1}.", newAccount.LogicalName, newAccount["name"]);
                Console.WriteLine();

                // Create a column set to define which attributes should be retrieved.
                ColumnSet attributes = new ColumnSet("name", "ownerid");

                Console.WriteLine("Retrieving account...");

                // Retrieve the account and its name and ownerid attributes.
                newAccount = service.Retrieve(newAccount.LogicalName, accountid, attributes);

                Console.WriteLine("Retrieved entity:");
                Console.WriteLine("  Name: {0}", newAccount.GetAttributeValue<string>("name"));
                Console.WriteLine("  Owner ID: {0}", newAccount.GetAttributeValue<EntityReference>("ownerid").Id);
                Console.WriteLine();

                /*
                IMPORTANT:
                Do not update an entity using a retrieved entity instance.
                Always instantiate a new Entity and
                set the primary key value to match the entity you want to update.
                Only set the attribute values you are changing.
                */

                Console.WriteLine("Updating account...");

                Entity accountToUpdate = new Entity("account");
                accountToUpdate["accountid"] = newAccount.Id;

                // Update the address 1 postal code attribute.
                accountToUpdate["address1_postalcode"] = "98052";

                // The address 2 postal code was set accidentally, so set it to null.
                accountToUpdate["address2_postalcode"] = null;

                // Shows use of Money.
                accountToUpdate["revenue"] = new Money(5000000);

                // Shows use of boolean.
                accountToUpdate["creditonhold"] = false;

                // Perform the update.
                service.Update(accountToUpdate);

                Console.WriteLine("Updated entity:");
                Console.WriteLine("  Address 1 Postal Code: 98052");
                Console.WriteLine("  Address 2 Postal Code: null (cleared)");
                Console.WriteLine("  Revenue: $5,000,000");
                Console.WriteLine("  Credit On Hold: false");
                Console.WriteLine();

                Console.WriteLine("Sample completed successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Run(): an exception occurred: \n\t" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Creates any pre-existing entity records required by the Run() method.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Entity name and reference collection.</param>
        static public void Setup(IOrganizationService service, out Dictionary<string,
            EntityReference> entityStore)
        {
            // Used to track any entities created by this program.
            entityStore = new Dictionary<string, EntityReference>();

            Console.WriteLine("Setup complete - no pre-existing entities required.");
            Console.WriteLine();
        }

        /// <summary>
        /// Delete any entity records (table rows) created by this program.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Entity name and reference collection.</param>
        static public void Cleanup(ServiceClient service,
            Dictionary<string, EntityReference> entityStore)
        {
            // Do some checking of the passed parameter values.
            if (service == null || service.IsReady == false)
            {
                Console.WriteLine(
                    "Cleanup(): web service connection not available, cleanup aborted.");
                return;
            }

            if (entityStore == null)
            {
                Console.WriteLine("Cleanup(): entityStore is null, cleanup aborted.");
                Console.WriteLine("Cleanup(): run Setup() prior to Cleanup().");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Cleaning up entities...");

            // Collect the keys of entities to be deleted.
            var keysToDelete = new List<string>(entityStore.Keys);
            keysToDelete.Reverse();

            // Delete in Dataverse each entity in the entity store.
            foreach (var key in keysToDelete)
            {
                var entref = entityStore[key];
                try
                {
                    service.Delete(entref.LogicalName, entref.Id);
                    Console.WriteLine("Deleted {0} entity.", key);
                    entityStore.Remove(key);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        "Cleanup(): exception deleting {0}\n\t{1}", key, ex.Message);
                    continue;
                }
            }

            // Output a list of entities that could not be deleted.
            if (entityStore.Count > 0)
            {
                Console.WriteLine(
                    "Cleanup(): the following entities could not be deleted:");

                foreach (var item in entityStore)
                {
                    Console.WriteLine("Cleanup(): name={0}, " +
                        "logical name={1}, ID={2}", item.Key, item.Value.LogicalName, item.Value.Id);
                }
            }
            else
            {
                Console.WriteLine("Cleanup complete.");
            }
        }
    }
}
