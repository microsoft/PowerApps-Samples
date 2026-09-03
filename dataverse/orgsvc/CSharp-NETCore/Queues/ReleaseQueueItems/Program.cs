using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates releasing queue items from workers
    /// </summary>
    /// <remarks>
    /// This sample shows how to release a queue item from a worker using ReleaseToQueueRequest.
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// </remarks>
    class Program
    {
        private static readonly List<EntityReference> entityStore = new();
        private static Guid queueItemId;

        #region Sample Methods

        private static void Setup(ServiceClient service)
        {
            Console.WriteLine("Creating queue, letter, and queue item...");

            var queue = new Entity("queue") { ["name"] = "Example Queue", ["queueviewtype"] = new OptionSetValue(1) };
            Guid queueId = service.Create(queue);
            entityStore.Add(new EntityReference("queue", queueId));

            var letter = new Entity("letter") { ["description"] = "Example Letter" };
            Guid letterId = service.Create(letter);
            entityStore.Add(new EntityReference("letter", letterId));

            var queueItem = new Entity("queueitem")
            {
                ["queueid"] = new EntityReference("queue", queueId),
                ["objectid"] = new EntityReference("letter", letterId)
            };
            queueItemId = service.Create(queueItem);
            entityStore.Add(new EntityReference("queueitem", queueItemId));

            // Get current user and assign as worker
            var whoAmI = (WhoAmIResponse)service.Execute(new WhoAmIRequest());
            var updateItem = new Entity("queueitem")
            {
                Id = queueItemId,
                ["workerid"] = new EntityReference("systemuser", whoAmI.UserId)
            };
            service.Update(updateItem);

            Console.WriteLine("Setup complete - queue item assigned to worker.");
            Console.WriteLine();
        }

        private static void Run(ServiceClient service)
        {
            Console.WriteLine("Releasing queue item from worker...");

            service.Execute(new ReleaseToQueueRequest { QueueItemId = queueItemId });

            Console.WriteLine("Queue item released from worker.");
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
