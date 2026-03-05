using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates how to convert a Fax activity to a Task activity.
    /// </summary>
    /// <remarks>
    /// This sample shows how to retrieve a Fax activity and create a follow-up Task
    /// based on the fax information.
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
            Console.WriteLine("Setting up sample data...");

            // Get the current user
            var userRequest = new WhoAmIRequest();
            var userResponse = (WhoAmIResponse)service.Execute(userRequest);
            Guid userId = userResponse.UserId;

            // Create the activity party for sending and receiving the fax
            var party = new Entity("activityparty")
            {
                ["partyid"] = new EntityReference("systemuser", userId)
            };

            // Create the fax
            var fax = new Entity("fax")
            {
                ["subject"] = "Sample Fax",
                ["from"] = new EntityCollection(new List<Entity> { party }),
                ["to"] = new EntityCollection(new List<Entity> { party })
            };

            Guid faxId = service.Create(fax);
            entityStore.Add(new EntityReference("fax", faxId));
            Console.WriteLine("Created a fax: 'Sample Fax'.");
        }

        /// <summary>
        /// Demonstrates converting a fax to a task
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Console.WriteLine("\nConverting fax to task...");

            // Get the fax ID from the entityStore
            var faxRef = entityStore.FirstOrDefault(e => e.LogicalName == "fax");
            if (faxRef == null)
            {
                Console.WriteLine("No fax found to convert.");
                return;
            }

            // Retrieve the fax
            var retrievedFax = service.Retrieve("fax", faxRef.Id, new ColumnSet(true));

            // Create a task based on the fax
            var task = new Entity("task")
            {
                ["subject"] = "Follow Up: " + retrievedFax.GetAttributeValue<string>("subject"),
                ["scheduledend"] = retrievedFax.GetAttributeValue<DateTime>("createdon").AddDays(7)
            };

            Guid taskId = service.Create(task);
            entityStore.Add(new EntityReference("task", taskId));

            // Verify that the task has been created
            if (taskId != Guid.Empty)
            {
                Console.WriteLine("Created a task for the fax: '{0}'.", task["subject"]);
            }
        }

        /// <summary>
        /// Cleans up sample data created during execution
        /// </summary>
        private static void Cleanup(ServiceClient service, bool deleteCreatedRecords)
        {
            if (deleteCreatedRecords && entityStore.Count > 0)
            {
                Console.WriteLine("\nDeleting {0} created record(s)...", entityStore.Count);
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
