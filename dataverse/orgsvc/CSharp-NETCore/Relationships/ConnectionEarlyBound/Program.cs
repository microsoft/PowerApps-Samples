using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates how to create a connection between an account and a contact.
    /// </summary>
    /// <remarks>
    /// This sample shows how to create a connection between an account and a contact
    /// that have matching connection roles.
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

            // Create a Connection Role for account and contact
            var newConnectionRole = new Entity("connectionrole")
            {
                ["name"] = "Example Connection Role",
                ["category"] = new OptionSetValue(1) // Business category
            };
            Guid connectionRoleId = service.Create(newConnectionRole);
            entityStore.Add(new EntityReference("connectionrole", connectionRoleId));
            Console.WriteLine("Created {0}.", newConnectionRole["name"]);

            // Create a related Connection Role Object Type Code record for Account
            var newAccountConnectionRoleTypeCode = new Entity("connectionroleobjecttypecode")
            {
                ["connectionroleid"] = new EntityReference("connectionrole", connectionRoleId),
                ["associatedobjecttypecode"] = "account"
            };
            service.Create(newAccountConnectionRoleTypeCode);
            Console.WriteLine("Created a related Connection Role Object Type Code record for Account.");

            // Create a related Connection Role Object Type Code record for Contact
            var newContactConnectionRoleTypeCode = new Entity("connectionroleobjecttypecode")
            {
                ["connectionroleid"] = new EntityReference("connectionrole", connectionRoleId),
                ["associatedobjecttypecode"] = "contact"
            };
            service.Create(newContactConnectionRoleTypeCode);
            Console.WriteLine("Created a related Connection Role Object Type Code record for Contact.");

            // Associate the connection role with itself
            var associateConnectionRoles = new AssociateRequest
            {
                Target = new EntityReference("connectionrole", connectionRoleId),
                RelatedEntities = new EntityReferenceCollection()
                {
                    new EntityReference("connectionrole", connectionRoleId)
                },
                // The name of the relationship connection role association
                // relationship in MS CRM
                Relationship = new Relationship()
                {
                    PrimaryEntityRole = EntityRole.Referencing, // Referencing or Referenced based on N:1 or 1:N reflexive relationship
                    SchemaName = "connectionroleassociation_association"
                }
            };
            service.Execute(associateConnectionRoles);
            Console.WriteLine("Associated the connection role with itself.");

            // Create an Account
            var account = new Entity("account")
            {
                ["name"] = "Example Account"
            };
            Guid accountId = service.Create(account);
            entityStore.Add(new EntityReference("account", accountId));
            Console.WriteLine("Created {0}.", account["name"]);

            // Create a Contact
            var contact = new Entity("contact")
            {
                ["lastname"] = "Example Contact"
            };
            Guid contactId = service.Create(contact);
            entityStore.Add(new EntityReference("contact", contactId));
            Console.WriteLine("Created {0}.", contact["lastname"]);
        }

        /// <summary>
        /// Demonstrates how to create a connection between an account and a contact
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Console.WriteLine("\nCreating connection between account and contact...");

            // Get references from entityStore
            Guid connectionRoleId = entityStore[0].Id;
            Guid accountId = entityStore[1].Id;
            Guid contactId = entityStore[2].Id;

            // Create a connection between the account and the contact
            // Assign a connection role to a record
            var newConnection = new Entity("connection")
            {
                ["record1id"] = new EntityReference("account", accountId),
                ["record1roleid"] = new EntityReference("connectionrole", connectionRoleId),
                ["record2roleid"] = new EntityReference("connectionrole", connectionRoleId),
                ["record2id"] = new EntityReference("contact", contactId)
            };

            Guid connectionId = service.Create(newConnection);
            entityStore.Add(new EntityReference("connection", connectionId));

            Console.WriteLine("Created a connection between the account and the contact.");
        }

        /// <summary>
        /// Cleans up sample data created during execution
        /// </summary>
        private static void Cleanup(ServiceClient service, bool deleteCreatedRecords)
        {
            if (deleteCreatedRecords && entityStore.Count > 0)
            {
                Console.WriteLine("\nDeleting {0} created record(s)...", entityStore.Count);
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
