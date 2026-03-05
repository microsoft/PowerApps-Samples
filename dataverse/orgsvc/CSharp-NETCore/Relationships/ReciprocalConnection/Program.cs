using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates how to create reciprocal connection roles.
    /// </summary>
    /// <remarks>
    /// This sample shows how to create the reciprocal connection roles. It creates
    /// a connection role for an account and a connection role for a contact, and then
    /// makes them reciprocal by associating them with each other.
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
            // No setup required for this sample
        }

        /// <summary>
        /// Demonstrates how to create reciprocal connection roles
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Console.WriteLine("Creating reciprocal connection roles...");

            // Define the connection role category - Business
            const int BusinessCategory = 1;

            // Create the Connection Role 1
            var newConnectionRole1 = new Entity("connectionrole")
            {
                ["name"] = "Example Connection Role 1",
                ["category"] = new OptionSetValue(BusinessCategory)
            };

            Guid connectionRole1Id = service.Create(newConnectionRole1);
            entityStore.Add(new EntityReference("connectionrole", connectionRole1Id));
            Console.WriteLine("Created {0}.", newConnectionRole1["name"]);

            // Create a related Connection Role 1 Object Type Code record for Account
            var newAccountConnectionRole1TypeCode = new Entity("connectionroleobjecttypecode")
            {
                ["connectionroleid"] = new EntityReference("connectionrole", connectionRole1Id),
                ["associatedobjecttypecode"] = "account"
            };

            service.Create(newAccountConnectionRole1TypeCode);
            Console.WriteLine("Created a related Connection Role 1 Object Type Code record for Account.");

            // Create a related Connection Role 1 Object Type Code record for Contact
            var newContactConnectionRole1TypeCode = new Entity("connectionroleobjecttypecode")
            {
                ["connectionroleid"] = new EntityReference("connectionrole", connectionRole1Id),
                ["associatedobjecttypecode"] = "contact"
            };

            service.Create(newContactConnectionRole1TypeCode);
            Console.WriteLine("Created a related Connection Role 1 Object Type Code record for Contact.");

            // Create the Connection Role 2
            var newConnectionRole2 = new Entity("connectionrole")
            {
                ["name"] = "Example Connection Role 2",
                ["category"] = new OptionSetValue(BusinessCategory)
            };

            Guid connectionRole2Id = service.Create(newConnectionRole2);
            entityStore.Add(new EntityReference("connectionrole", connectionRole2Id));
            Console.WriteLine("Created {0}.", newConnectionRole2["name"]);

            // Create a related Connection Role 2 Object Type Code record for Account
            var newAccountConnectionRole2TypeCode = new Entity("connectionroleobjecttypecode")
            {
                ["connectionroleid"] = new EntityReference("connectionrole", connectionRole2Id),
                ["associatedobjecttypecode"] = "account"
            };

            service.Create(newAccountConnectionRole2TypeCode);
            Console.WriteLine("Created a related Connection Role 2 Object Type Code record for Account.");

            // Create a related Connection Role 2 Object Type Code record for Contact
            var newContactConnectionRole2TypeCode = new Entity("connectionroleobjecttypecode")
            {
                ["connectionroleid"] = new EntityReference("connectionrole", connectionRole2Id),
                ["associatedobjecttypecode"] = "contact"
            };

            service.Create(newContactConnectionRole2TypeCode);
            Console.WriteLine("Created a related Connection Role 2 Object Type Code record for Contact.");

            // Associate the connection roles with each other (make them reciprocal)
            var associateConnectionRoles = new AssociateRequest
            {
                Target = new EntityReference("connectionrole", connectionRole1Id),
                RelatedEntities = new EntityReferenceCollection()
                {
                    new EntityReference("connectionrole", connectionRole2Id)
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
            Console.WriteLine("Associated the connection roles (made them reciprocal).");
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
