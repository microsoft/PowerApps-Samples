using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

namespace PowerApps.Samples
{
    /// <summary>
    /// Demonstrates creating and updating records with related records in a single operation.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Contains the application's configuration settings.
        /// </summary>
        private static IConfiguration Configuration { get; }

        /// <summary>
        /// Constructor. Loads the application settings from a JSON configuration file.
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
                Console.WriteLine(serviceClient.LastError);
                return;
            }

            // Pre-create any table rows that the Run() method requires.
            Setup(serviceClient, out entityStore);

            // Execute the main logic of this program.
            Run(serviceClient, entityStore);

            // Pause program execution before resource cleanup.
            Console.WriteLine("\nPress any key to undo environment data changes.");
            Console.ReadKey();

            // In Dataverse, delete any created table rows and then dispose the service connection.
            Cleanup(serviceClient, entityStore);
            serviceClient.Dispose();

            Console.WriteLine("Sample completed. Press any key to exit.");
            Console.ReadKey();
        }

        /// <summary>
        /// Executes the code being demonstrated by this program.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Entity name and reference collection.</param>
        static public void Run(IOrganizationService service,
            Dictionary<string, EntityReference> entityStore)
        {
            Console.WriteLine("\n=== Creating Account with Related Letters ===");

            // Define the account for which we will add letters
            var accountToCreate = new Entity("account")
            {
                ["name"] = "Example Account"
            };

            // Define the IDs of the related letters we will create
            Guid[] letterIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            // This acts as a container for each letter we create. Note that we haven't
            // defined the relationship between the letter and account yet.
            var relatedLettersToCreate = new EntityCollection
            {
                EntityName = "letter",
                Entities =
                {
                    new Entity("letter")
                    {
                        ["subject"] = "Letter 1",
                        ["activityid"] = letterIds[0]
                    },
                    new Entity("letter")
                    {
                        ["subject"] = "Letter 2",
                        ["activityid"] = letterIds[1]
                    },
                    new Entity("letter")
                    {
                        ["subject"] = "Letter 3",
                        ["activityid"] = letterIds[2]
                    }
                }
            };

            // Creates the reference between which relationship between Letter and
            // Account we would like to use.
            var letterRelationship = new Relationship("Account_Letters");

            // Adds the letters to the account under the specified relationship
            accountToCreate.RelatedEntities.Add(letterRelationship, relatedLettersToCreate);

            // Passes the Account (which contains the letters)
            Guid accountId = service.Create(accountToCreate);

            Console.WriteLine($"Created account with ID: {accountId}");
            Console.WriteLine($"Created {letterIds.Length} related letter activities.");

            // Store the created entities
            entityStore.Add("account", new EntityReference("account", accountId));
            for (int i = 0; i < letterIds.Length; i++)
            {
                entityStore.Add($"letter{i + 1}", new EntityReference("letter", letterIds[i]));
            }

            Console.WriteLine("\n=== Updating Account with Related Letters ===");

            // Now we run through many of the same steps as the above "Create" example
            var accountToUpdate = new Entity("account")
            {
                ["name"] = "Example Account - Updated",
                Id = accountId
            };

            var relatedLettersToUpdate = new EntityCollection
            {
                EntityName = "letter",
                Entities =
                {
                    new Entity("letter")
                    {
                        ["subject"] = "Letter 1 - Updated",
                        Id = letterIds[0]
                    },
                    new Entity("letter")
                    {
                        ["subject"] = "Letter 2 - Updated",
                        Id = letterIds[1]
                    },
                    new Entity("letter")
                    {
                        ["subject"] = "Letter 3 - Updated",
                        Id = letterIds[2]
                    }
                }
            };

            accountToUpdate.RelatedEntities.Add(letterRelationship, relatedLettersToUpdate);

            // This will update the account as well as all of the related letters
            service.Update(accountToUpdate);

            Console.WriteLine("Updated account and all related letter activities.");
        }

        /// <summary>
        /// Creates any pre-existing entity records required by the Run() method.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Entity name and reference collection.</param>
        static public void Setup(IOrganizationService service,
            out Dictionary<string, EntityReference> entityStore)
        {
            // Used to track any entities created by this program.
            entityStore = new Dictionary<string, EntityReference>();

            Console.WriteLine("=== Setup ===");
            Console.WriteLine("No setup required for this sample.");
        }

        /// <summary>
        /// Delete any entity records (table rows) created by this program.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Entity name and reference collection.</param>
        static public void Cleanup(ServiceClient service,
            Dictionary<string, EntityReference> entityStore)
        {
            Console.WriteLine("\n=== Cleanup ===");

            // Do some checking of the passed parameter values.
            if (service == null || service.IsReady == false)
            {
                Console.WriteLine("Web service connection not available, cleanup aborted.");
                return;
            }

            if (entityStore == null)
            {
                Console.WriteLine("entityStore is null, cleanup aborted.");
                return;
            }

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
                    Console.WriteLine($"Deleted {entref.LogicalName} with ID: {entref.Id}");
                    entityStore.Remove(key);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception deleting {key}: {ex.Message}");
                    continue;
                }
            }

            // Output a list of entities that could not be deleted.
            if (entityStore.Count > 0)
            {
                Console.WriteLine("The following entities could not be deleted:");
                foreach (var item in entityStore)
                {
                    Console.WriteLine($"  Name={item.Key}, LogicalName={item.Value.LogicalName}, ID={item.Value.Id}");
                }
            }
            else
            {
                Console.WriteLine("All created records have been deleted.");
            }
        }
    }
}
