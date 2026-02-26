using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

namespace PowerPlatform_Dataverse_CodeSamples
{
    /// <summary>
    /// Demonstrates using InitializeFromRequest to create new records
    /// initialized from existing records.
    /// </summary>
    /// <remarks>
    /// This sample shows how to:
    /// 1. Create an Account and initialize a new Account from it
    /// 2. Create a Lead and initialize an Opportunity from it
    /// </remarks>
    class Program
    {
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

            // Entity name and reference collection to track created entities.
            Dictionary<string, EntityReference> entityStore;

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
        }

        /// <summary>
        /// Executes the code being demonstrated by this program.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Entity name and reference collection.</param>
        static void Run(IOrganizationService service,
            Dictionary<string, EntityReference> entityStore)
        {
            Console.WriteLine("\nInitializing new Account from the initial Account...");

            // Create the request object for initializing an Account from an existing Account
            var initializeAccount = new InitializeFromRequest
            {
                // Set the target entity type
                TargetEntityName = "account",

                // Create the EntityMoniker from the existing account
                EntityMoniker = entityStore["initialAccount"]
            };

            // Execute the request
            InitializeFromResponse initializedAccount =
                (InitializeFromResponse)service.Execute(initializeAccount);

            if (initializedAccount.Entity != null)
            {
                Console.WriteLine("  New Account initialized successfully");
                Console.WriteLine($"  Initialized account name: {initializedAccount.Entity["name"]}");
            }

            Console.WriteLine("\nInitializing an Opportunity from the initial Lead...");

            // Create the request object for initializing an Opportunity from a Lead
            var initializeLead = new InitializeFromRequest
            {
                // Set the target entity type
                TargetEntityName = "opportunity",

                // Create the EntityMoniker from the existing lead
                EntityMoniker = entityStore["initialLead"]
            };

            // Execute the request
            InitializeFromResponse initializedOpportunity =
                (InitializeFromResponse)service.Execute(initializeLead);

            if (initializedOpportunity.Entity != null &&
                initializedOpportunity.Entity.LogicalName == "opportunity")
            {
                Console.WriteLine("  New Opportunity initialized successfully");
                Console.WriteLine($"  Opportunity name: {initializedOpportunity.Entity["name"]}");
            }
        }

        /// <summary>
        /// Creates any pre-existing entity records required by the Run() method.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Entity name and reference collection.</param>
        static void Setup(IOrganizationService service,
            out Dictionary<string, EntityReference> entityStore)
        {
            // Used to track any entities created by this program.
            entityStore = new Dictionary<string, EntityReference>();

            Console.WriteLine("Creating sample data...");

            // Create an initial Account
            Entity initialAccount = new("account")
            {
                ["name"] = "Contoso, Ltd"
            };

            try
            {
                initialAccount.Id = service.Create(initialAccount);
                entityStore.Add("initialAccount",
                    new EntityReference("account", initialAccount.Id));
                Console.WriteLine($"  Created initial Account (Name={initialAccount["name"]})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Setup(): error creating initial account.\n\t{ex.Message}");
                throw;
            }

            // Create an initial Lead
            Entity initialLead = new("lead")
            {
                ["subject"] = "A Sample Lead",
                ["lastname"] = "Wilcox",
                ["firstname"] = "Colin"
            };

            try
            {
                initialLead.Id = service.Create(initialLead);
                entityStore.Add("initialLead",
                    new EntityReference("lead", initialLead.Id));
                Console.WriteLine($"  Created initial Lead (Subject={initialLead["subject"]}, " +
                    $"Name={initialLead["firstname"]} {initialLead["lastname"]})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Setup(): error creating initial lead.\n\t{ex.Message}");
                throw;
            }

            Console.WriteLine("Sample data created successfully.");
        }

        /// <summary>
        /// Delete any entity records (table rows) created by this program.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Entity name and reference collection.</param>
        static void Cleanup(ServiceClient service,
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

            // Collect the keys of entities to be deleted.
            var keysToDelete = new List<string>(entityStore.Keys);
            keysToDelete.Reverse();

            Console.WriteLine("\nDeleting sample data...");

            // Delete in Dataverse each entity in the entity store.
            foreach (var key in keysToDelete)
            {
                var entref = entityStore[key];
                try
                {
                    service.Delete(entref.LogicalName, entref.Id);
                    entityStore.Remove(key);
                    Console.WriteLine($"  Deleted {entref.LogicalName} with ID {entref.Id}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Cleanup(): exception deleting {key}\n\t{ex.Message}");
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
                    Console.WriteLine($"  name={item.Key}, " +
                        $"logical name={item.Value.LogicalName}, ID={item.Value.Id}");
                }
            }
            else
            {
                Console.WriteLine("Sample data deleted successfully.");
            }
        }
    }
}
