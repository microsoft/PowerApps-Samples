using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates querying data using QueryByAttribute
    /// </summary>
    /// <remarks>
    /// This sample shows how to use QueryByAttribute to query records based on attribute values.
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// </remarks>
    class Program
    {
        private static readonly List<EntityReference> entityStore = new();

        #region Sample Methods

        private static void Setup(ServiceClient service)
        {
            Console.WriteLine("Creating sample accounts...");

            var account1 = new Entity("account")
            {
                ["name"] = "A. Datum Corporation",
                ["address1_stateorprovince"] = "Colorado",
                ["address1_telephone1"] = "(206)555-5555",
                ["emailaddress1"] = "info@datum.com"
            };
            Guid account1Id = service.Create(account1);
            entityStore.Add(new EntityReference("account", account1Id));

            var account2 = new Entity("account")
            {
                ["name"] = "Adventure Works Cycle",
                ["address1_stateorprovince"] = "Washington",
                ["address1_city"] = "Redmond",
                ["address1_telephone1"] = "(206)555-5555",
                ["emailaddress1"] = "contactus@adventureworkscycle.com"
            };
            Guid account2Id = service.Create(account2);
            entityStore.Add(new EntityReference("account", account2Id));

            Console.WriteLine("Setup complete.");
            Console.WriteLine();
        }

        private static void Run(ServiceClient service)
        {
            Console.WriteLine("Query Using QueryByAttribute");
            Console.WriteLine("===============================");

            // Create query using QueryByAttribute
            var querybyattribute = new QueryByAttribute("account");
            querybyattribute.ColumnSet = new ColumnSet("name", "address1_city", "emailaddress1");

            // Attribute to query
            querybyattribute.Attributes.AddRange("address1_city");

            // Value of queried attribute to return
            querybyattribute.Values.AddRange("Redmond");

            // Execute query
            EntityCollection retrieved = service.RetrieveMultiple(querybyattribute);

            // Iterate through returned collection
            foreach (var account in retrieved.Entities)
            {
                Console.WriteLine("Name: " + account["name"]);

                if (account.Contains("address1_city"))
                    Console.WriteLine("Address: " + account["address1_city"]);

                if (account.Contains("emailaddress1"))
                    Console.WriteLine("E-mail: " + account["emailaddress1"]);
            }
            Console.WriteLine("===============================");
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
