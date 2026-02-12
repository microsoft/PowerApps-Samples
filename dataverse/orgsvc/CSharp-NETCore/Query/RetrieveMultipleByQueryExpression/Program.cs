using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates querying data using QueryExpression with linked entities
    /// </summary>
    /// <remarks>
    /// This sample shows how to use QueryExpression with LinkEntity to retrieve
    /// data from related entities using entity aliases and aliased values.
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// </remarks>
    class Program
    {
        private static readonly List<EntityReference> entityStore = new();

        #region Sample Methods

        private static void Setup(ServiceClient service)
        {
            Console.WriteLine("Creating sample data...");

            // Create a contact
            var contact = new Entity("contact")
            {
                ["firstname"] = "ContactFirstName",
                ["lastname"] = "ContactLastName"
            };
            Guid contactId = service.Create(contact);
            entityStore.Add(new EntityReference("contact", contactId));

            // Create multiple accounts with the same primary contact
            var account1 = new Entity("account")
            {
                ["name"] = "Test Account1",
                ["primarycontactid"] = new EntityReference("contact", contactId)
            };
            Guid accountId1 = service.Create(account1);
            entityStore.Add(new EntityReference("account", accountId1));

            var account2 = new Entity("account")
            {
                ["name"] = "Test Account2",
                ["primarycontactid"] = new EntityReference("contact", contactId)
            };
            Guid accountId2 = service.Create(account2);
            entityStore.Add(new EntityReference("account", accountId2));

            var account3 = new Entity("account")
            {
                ["name"] = "Test Account3",
                ["primarycontactid"] = new EntityReference("contact", contactId)
            };
            Guid accountId3 = service.Create(account3);
            entityStore.Add(new EntityReference("account", accountId3));

            Console.WriteLine("Setup complete.");
            Console.WriteLine();
        }

        private static void Run(ServiceClient service)
        {
            Console.WriteLine("Entering: RetrieveMultipleWithRelatedEntityColumns");
            Console.WriteLine();

            // Create a query expression with link entity
            var qe = new QueryExpression
            {
                EntityName = "account",
                ColumnSet = new ColumnSet("name")
            };

            // Add link to contact entity
            var linkEntity = new LinkEntity(
                "account",
                "contact",
                "primarycontactid",
                "contactid",
                JoinOperator.Inner
            );
            linkEntity.Columns.AddColumns("firstname", "lastname");
            linkEntity.EntityAlias = "primarycontact";
            qe.LinkEntities.Add(linkEntity);

            // Execute query
            EntityCollection ec = service.RetrieveMultiple(qe);

            Console.WriteLine("Retrieved {0} entities", ec.Entities.Count);
            Console.WriteLine();

            foreach (Entity act in ec.Entities)
            {
                Console.WriteLine("Account name: {0}", act["name"]);
                Console.WriteLine("Primary contact first name: {0}",
                    act.GetAttributeValue<AliasedValue>("primarycontact.firstname").Value);
                Console.WriteLine("Primary contact last name: {0}",
                    act.GetAttributeValue<AliasedValue>("primarycontact.lastname").Value);
                Console.WriteLine();
            }
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
