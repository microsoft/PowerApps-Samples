using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates how to update a connection role.
    /// </summary>
    /// <remarks>
    /// This sample shows how to modify the properties of the connection role, such as
    /// a role name, description, and category.
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

            // Define the connection role category - Business
            const int BusinessCategory = 1;

            // Create a Connection Role
            var setupConnectionRole = new Entity("connectionrole")
            {
                ["name"] = "Example Connection Role",
                ["description"] = "This is an example one sided connection role.",
                ["category"] = new OptionSetValue(BusinessCategory)
            };

            Guid connectionRoleId = service.Create(setupConnectionRole);
            entityStore.Add(new EntityReference("connectionrole", connectionRoleId));
            Console.WriteLine("Created {0}.", setupConnectionRole["name"]);
        }

        /// <summary>
        /// Demonstrates how to update a connection role
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Console.WriteLine("\nUpdating connection role...");

            // Get the connection role ID from entityStore
            Guid connectionRoleId = entityStore[0].Id;

            // Define the connection role category - Other
            const int OtherCategory = 5;

            // Update the connection role instance
            var connectionRole = new Entity("connectionrole")
            {
                ["connectionroleid"] = connectionRoleId,
                ["name"] = "Updated Connection Role",
                ["description"] = "This is an updated connection role.",
                ["category"] = new OptionSetValue(OtherCategory)
            };

            service.Update(connectionRole);

            Console.WriteLine("Updated the connection role instance.");
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
