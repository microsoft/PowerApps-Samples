using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates adding records to queues and moving between queues
    /// </summary>
    /// <remarks>
    /// This sample shows how to add records to queues and move them between queues.
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// </remarks>
    class Program
    {
        private static readonly List<EntityReference> entityStore = new();

        #region Sample Methods

        private static void Setup(ServiceClient service)
        {
            Console.WriteLine("Creating queues and letter...");

            var sourceQueue = new Entity("queue") { ["name"] = "Source Queue", ["queueviewtype"] = new OptionSetValue(1) };
            Guid sourceQueueId = service.Create(sourceQueue);
            entityStore.Add(new EntityReference("queue", sourceQueueId));

            var destQueue = new Entity("queue") { ["name"] = "Destination Queue", ["queueviewtype"] = new OptionSetValue(1) };
            Guid destQueueId = service.Create(destQueue);
            entityStore.Add(new EntityReference("queue", destQueueId));

            var letter = new Entity("letter") { ["description"] = "Example Letter" };
            Guid letterId = service.Create(letter);
            entityStore.Add(new EntityReference("letter", letterId));

            service.Execute(new AddToQueueRequest
            {
                DestinationQueueId = sourceQueueId,
                Target = new EntityReference("letter", letterId)
            });
            Console.WriteLine("Setup complete.");
            Console.WriteLine();
        }

        private static void Run(ServiceClient service)
        {
            Console.WriteLine("Moving letter between queues...");

            service.Execute(new AddToQueueRequest
            {
                SourceQueueId = entityStore[0].Id,
                Target = new EntityReference("letter", entityStore[2].Id),
                DestinationQueueId = entityStore[1].Id
            });

            Console.WriteLine("Letter moved to destination queue.");
        }

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
