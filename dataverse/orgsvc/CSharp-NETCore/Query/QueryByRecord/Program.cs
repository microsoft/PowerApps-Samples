using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates querying connections for a specific record
    /// </summary>
    /// <remarks>
    /// This sample shows how to query connection records to find all connections
    /// that a specific entity record is part of using QueryExpression.
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// </remarks>
    class Program
    {
        private static readonly List<EntityReference> entityStore = new();

        #region Sample Methods

        private static void Setup(ServiceClient service)
        {
            Console.WriteLine("Setting up sample data...");

            // Define connection categories
            var Categories = new
            {
                Business = 1,
                Family = 2,
                Social = 3,
                Sales = 4,
                Other = 5
            };

            // Create a Connection Role
            var setupConnectionRole = new Entity("connectionrole")
            {
                ["name"] = "Example Connection Role",
                ["description"] = "This is an example one sided connection role.",
                ["category"] = new OptionSetValue(Categories.Business)
            };

            Guid connectionRoleId = service.Create(setupConnectionRole);
            entityStore.Add(new EntityReference("connectionrole", connectionRoleId));
            Console.WriteLine("Created connection role: {0}", setupConnectionRole["name"]);

            // Create a related Connection Role Object Type Code record for Account
            var newAccountConnectionRoleTypeCode = new Entity("connectionroleobjecttypecode")
            {
                ["connectionroleid"] = new EntityReference("connectionrole", connectionRoleId),
                ["associatedobjecttypecode"] = "account"
            };

            Guid accountTypeCodeId = service.Create(newAccountConnectionRoleTypeCode);
            entityStore.Add(new EntityReference("connectionroleobjecttypecode", accountTypeCodeId));
            Console.WriteLine("Created a related Connection Role Object Type Code record for Account.");

            // Create a related Connection Role Object Type Code record for Contact
            var newContactConnectionRoleTypeCode = new Entity("connectionroleobjecttypecode")
            {
                ["connectionroleid"] = new EntityReference("connectionrole", connectionRoleId),
                ["associatedobjecttypecode"] = "contact"
            };

            Guid contactTypeCodeId = service.Create(newContactConnectionRoleTypeCode);
            entityStore.Add(new EntityReference("connectionroleobjecttypecode", contactTypeCodeId));
            Console.WriteLine("Created a related Connection Role Object Type Code record for Contact.");

            // Create a few account records for use in the connections
            var setupAccount1 = new Entity("account")
            {
                ["name"] = "Example Account 1"
            };
            Guid account1Id = service.Create(setupAccount1);
            entityStore.Add(new EntityReference("account", account1Id));
            Console.WriteLine("Created {0}.", setupAccount1["name"]);

            var setupAccount2 = new Entity("account")
            {
                ["name"] = "Example Account 2"
            };
            Guid account2Id = service.Create(setupAccount2);
            entityStore.Add(new EntityReference("account", account2Id));
            Console.WriteLine("Created {0}.", setupAccount2["name"]);

            // Create a contact used in the connection
            var setupContact = new Entity("contact")
            {
                ["lastname"] = "Example Contact"
            };
            Guid contactId = service.Create(setupContact);
            entityStore.Add(new EntityReference("contact", contactId));
            Console.WriteLine("Created contact: {0}.", setupContact["lastname"]);

            // Create a new connection between Account 1 and the contact record
            var newConnection1 = new Entity("connection")
            {
                ["record1id"] = new EntityReference("account", account1Id),
                ["record1roleid"] = new EntityReference("connectionrole", connectionRoleId),
                ["record2id"] = new EntityReference("contact", contactId)
            };

            Guid connection1Id = service.Create(newConnection1);
            entityStore.Add(new EntityReference("connection", connection1Id));
            Console.WriteLine("Created a connection between account 1 and the contact.");

            // Create a new connection between the contact and Account 2 record
            var newConnection2 = new Entity("connection")
            {
                ["record1id"] = new EntityReference("contact", contactId),
                ["record1roleid"] = new EntityReference("connectionrole", connectionRoleId),
                ["record2id"] = new EntityReference("account", account2Id)
            };

            Guid connection2Id = service.Create(newConnection2);
            entityStore.Add(new EntityReference("connection", connection2Id));
            Console.WriteLine("Created a connection between the contact and account 2.");

            Console.WriteLine("Setup complete.");
            Console.WriteLine();
        }

        private static void Run(ServiceClient service)
        {
            Console.WriteLine("Query Connections By Record");
            Console.WriteLine("===============================");

            // Get the contact ID from our entity store
            // The contact is at index 5 (after connection role, 2 type codes, and 2 accounts)
            Guid contactId = entityStore[5].Id;

            // This query retrieves all connections this contact is part of
            var query = new QueryExpression
            {
                EntityName = "connection",
                ColumnSet = new ColumnSet("connectionid"),
                Criteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        // You can safely query against only record1id or
                        // record2id - Dataverse will find all connections this
                        // entity is a part of either way.
                        new ConditionExpression
                        {
                            AttributeName = "record1id",
                            Operator = ConditionOperator.Equal,
                            Values = { contactId }
                        }
                    }
                }
            };

            EntityCollection results = service.RetrieveMultiple(query);

            // Here you could do a variety of tasks with the
            // connections retrieved, such as listing the connected entities,
            // finding reciprocal connections, etc.

            Console.WriteLine("Retrieved {0} connection instances for the contact.", results.Entities.Count);

            foreach (var connection in results.Entities)
            {
                Console.WriteLine("  Connection ID: {0}", connection.Id);
            }

            Console.WriteLine("===============================");
        }

        private static void Cleanup(ServiceClient service, bool deleteCreatedRecords)
        {
            Console.WriteLine("Cleaning up...");
            if (deleteCreatedRecords && entityStore.Count > 0)
            {
                Console.WriteLine($"Deleting {entityStore.Count} created record(s)...");
                // Delete in reverse order to handle dependencies
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
