using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates how to create a queue with various configuration options
    /// </summary>
    /// <remarks>
    /// This sample shows how to:
    /// - Create a queue entity
    /// - Configure queue properties including email delivery and filtering methods
    /// - Set queue view type (public/private)
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

        #region Sample Methods

        /// <summary>
        /// Sets up sample data required for the demonstration
        /// </summary>
        private static void Setup(ServiceClient service)
        {
            // No setup required for this sample
        }

        /// <summary>
        /// Demonstrates creating a queue with configuration options
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Console.WriteLine("Creating a queue...");

            // Create a queue with various property values
            var newQueue = new Entity("queue")
            {
                ["name"] = "Example Queue",
                ["description"] = "This is an example queue.",
                ["incomingemaildeliverymethod"] = new OptionSetValue(0), // None
                ["incomingemailfilteringmethod"] = new OptionSetValue(0), // All Email Messages
                ["outgoingemaildeliverymethod"] = new OptionSetValue(0), // None
                ["queueviewtype"] = new OptionSetValue(1) // Private
            };

            Guid queueId = service.Create(newQueue);
            entityStore.Add(new EntityReference("queue", queueId));

            Console.WriteLine($"Created queue: {newQueue["name"]}");
            Console.WriteLine($"  Queue ID: {queueId}");
            Console.WriteLine($"  Description: {newQueue["description"]}");
            Console.WriteLine($"  Incoming Email Delivery Method: None (0)");
            Console.WriteLine($"  Incoming Email Filtering Method: All Email Messages (0)");
            Console.WriteLine($"  Outgoing Email Delivery Method: None (0)");
            Console.WriteLine($"  Queue View Type: Private (1)");
            Console.WriteLine();

            Console.WriteLine("Queue creation complete.");
        }

        /// <summary>
        /// Cleans up sample data created during execution
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
