using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates how to associate and disassociate table rows.
    /// </summary>
    /// <remarks>
    /// This sample shows how to associate and disassociate tables rows using the
    /// IOrganizationService.Associate and IOrganizationService.Disassociate methods.
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

            // Create a contact
            var contact = new Entity("contact")
            {
                ["firstname"] = "John",
                ["lastname"] = "Doe"
            };
            Guid contactId = service.Create(contact);
            entityStore.Add(new EntityReference("contact", contactId));
            Console.WriteLine("Created contact: {0} {1}", contact["firstname"], contact["lastname"]);

            // Create three accounts
            var account1 = new Entity("account")
            {
                ["name"] = "Example Account 1"
            };
            Guid account1Id = service.Create(account1);
            entityStore.Add(new EntityReference("account", account1Id));
            Console.WriteLine("Created {0}", account1["name"]);

            var account2 = new Entity("account")
            {
                ["name"] = "Example Account 2"
            };
            Guid account2Id = service.Create(account2);
            entityStore.Add(new EntityReference("account", account2Id));
            Console.WriteLine("Created {0}", account2["name"]);

            var account3 = new Entity("account")
            {
                ["name"] = "Example Account 3"
            };
            Guid account3Id = service.Create(account3);
            entityStore.Add(new EntityReference("account", account3Id));
            Console.WriteLine("Created {0}", account3["name"]);
        }

        /// <summary>
        /// Demonstrates how to associate and disassociate table rows
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Console.WriteLine("\nAssociating accounts to contact...");

            // Get references from entityStore
            Guid contactId = entityStore[0].Id;
            Guid account1Id = entityStore[1].Id;
            Guid account2Id = entityStore[2].Id;
            Guid account3Id = entityStore[3].Id;

            // Create a collection of the entities that will be associated to the contact
            var relatedEntities = new EntityReferenceCollection
            {
                new EntityReference("account", account1Id),
                new EntityReference("account", account2Id),
                new EntityReference("account", account3Id)
            };

            // Create an object that defines the relationship between the contact and account
            var relationship = new Relationship("account_primary_contact");

            // Associate the contact with the 3 accounts
            service.Associate("contact", contactId, relationship, relatedEntities);
            Console.WriteLine("The entities have been associated.");

            Console.WriteLine("\nDisassociating accounts from contact...");

            // Disassociate the records
            service.Disassociate("contact", contactId, relationship, relatedEntities);
            Console.WriteLine("The entities have been disassociated.");
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
