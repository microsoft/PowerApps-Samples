using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates adding security principals to queues
    /// </summary>
    /// <remarks>
    /// This sample shows how to add a team to a queue using AddPrincipalToQueueRequest.
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// </remarks>
    class Program
    {
        private static readonly List<EntityReference> entityStore = new();
        private static Guid queueId;
        private static Guid teamId;

        #region Sample Methods

        private static void Setup(ServiceClient service)
        {
            Console.WriteLine("Creating queue and team...");

            var queue = new Entity("queue") { ["name"] = "Example Queue" };
            queueId = service.Create(queue);
            entityStore.Add(new EntityReference("queue", queueId));

            // Get default business unit
            var query = new QueryExpression("businessunit")
            {
                ColumnSet = new ColumnSet("businessunitid"),
                Criteria = new FilterExpression()
            };
            query.Criteria.AddCondition("parentbusinessunitid", ConditionOperator.Null);
            var defaultBU = service.RetrieveMultiple(query).Entities[0];

            var team = new Entity("team")
            {
                ["name"] = "Example Team",
                ["businessunitid"] = new EntityReference("businessunit", defaultBU.Id)
            };
            teamId = service.Create(team);
            entityStore.Add(new EntityReference("team", teamId));

            Console.WriteLine("Setup complete.");
            Console.WriteLine();
        }

        private static void Run(ServiceClient service)
        {
            Console.WriteLine("Adding team to queue...");

            var teamEntity = service.Retrieve("team", teamId, new ColumnSet("name"));

            service.Execute(new AddPrincipalToQueueRequest
            {
                Principal = teamEntity,
                QueueId = queueId
            });

            Console.WriteLine("Team added to queue.");
        }

        private static void Cleanup(ServiceClient service, bool deleteCreatedRecords)
        {
            Console.WriteLine("Cleaning up...");
            if (deleteCreatedRecords && entityStore.Count > 0)
            {
                Console.WriteLine($"Deleting {entityStore.Count} created record(s)...");
                for (int i = entityStore.Count - 1; i >= 0; i--)
                {
                    service.Delete(entityStore[i].LogicalName, entityStore[i].Id);
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
