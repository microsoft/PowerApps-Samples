using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates querying connection roles by reciprocal role
    /// </summary>
    /// <remarks>
    /// This sample shows how to query for connection roles that have a specific
    /// role listed as a reciprocal role, using QueryExpression with LinkEntity.
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// </remarks>
    class Program
    {
        private static readonly List<EntityReference> entityStore = new();
        private static Guid primaryConnectionRoleId;
        private static Guid reciprocalConnectionRoleId;

        #region Sample Methods

        private static void Setup(ServiceClient service)
        {
            Console.WriteLine("Creating sample data...");

            // Define connection role categories
            int businessCategory = 1;

            // Create the primary connection role
            var primaryConnectionRole = new Entity("connectionrole")
            {
                ["name"] = "Example Primary Connection Role",
                ["category"] = new OptionSetValue(businessCategory)
            };

            primaryConnectionRoleId = service.Create(primaryConnectionRole);
            entityStore.Add(new EntityReference("connectionrole", primaryConnectionRoleId));
            Console.WriteLine("Created primary connection role: {0}", primaryConnectionRole["name"]);

            // Create a related Connection Role Object Type Code record for Account
            // on the primary role
            var accountPrimaryConnectionRoleTypeCode = new Entity("connectionroleobjecttypecode")
            {
                ["connectionroleid"] = new EntityReference("connectionrole", primaryConnectionRoleId),
                ["associatedobjecttypecode"] = "account"
            };

            Guid primaryTypeCodeId = service.Create(accountPrimaryConnectionRoleTypeCode);
            entityStore.Add(new EntityReference("connectionroleobjecttypecode", primaryTypeCodeId));
            Console.WriteLine("Created Connection Role Object Type Code for Account on primary role.");

            // Create the reciprocal connection role
            var reciprocalConnectionRole = new Entity("connectionrole")
            {
                ["name"] = "Example Reciprocal Connection Role",
                ["category"] = new OptionSetValue(businessCategory)
            };

            reciprocalConnectionRoleId = service.Create(reciprocalConnectionRole);
            entityStore.Add(new EntityReference("connectionrole", reciprocalConnectionRoleId));
            Console.WriteLine("Created reciprocal connection role: {0}", reciprocalConnectionRole["name"]);

            // Create a related Connection Role Object Type Code record for Account
            // on the reciprocal role
            var accountReciprocalConnectionRoleTypeCode = new Entity("connectionroleobjecttypecode")
            {
                ["connectionroleid"] = new EntityReference("connectionrole", reciprocalConnectionRoleId),
                ["associatedobjecttypecode"] = "account"
            };

            Guid reciprocalTypeCodeId = service.Create(accountReciprocalConnectionRoleTypeCode);
            entityStore.Add(new EntityReference("connectionroleobjecttypecode", reciprocalTypeCodeId));
            Console.WriteLine("Created Connection Role Object Type Code for Account on reciprocal role.");

            // Associate the connection roles using the connectionroleassociation relationship
            var associateRequest = new AssociateRequest
            {
                Target = new EntityReference("connectionrole", primaryConnectionRoleId),
                RelatedEntities = new EntityReferenceCollection()
                {
                    new EntityReference("connectionrole", reciprocalConnectionRoleId)
                },
                Relationship = new Relationship()
                {
                    PrimaryEntityRole = EntityRole.Referencing,
                    SchemaName = "connectionroleassociation_association"
                }
            };

            service.Execute(associateRequest);
            Console.WriteLine("Associated primary and reciprocal connection roles.");

            Console.WriteLine("Setup complete.");
            Console.WriteLine();
        }

        private static void Run(ServiceClient service)
        {
            Console.WriteLine("Querying for connection roles by reciprocal role...");
            Console.WriteLine();

            // This query retrieves all connection roles that have the specified role
            // listed as a reciprocal role.
            var query = new QueryExpression
            {
                EntityName = "connectionrole",
                ColumnSet = new ColumnSet("connectionroleid", "name"),
                LinkEntities =
                {
                    new LinkEntity
                    {
                        JoinOperator = JoinOperator.Inner,
                        LinkFromEntityName = "connectionrole",
                        LinkFromAttributeName = "connectionroleid",
                        LinkToEntityName = "connectionroleassociation",
                        LinkToAttributeName = "connectionroleid",
                        LinkCriteria = new FilterExpression
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions =
                            {
                                new ConditionExpression
                                {
                                    AttributeName = "associatedconnectionroleid",
                                    Operator = ConditionOperator.Equal,
                                    Values = { reciprocalConnectionRoleId }
                                }
                            }
                        }
                    }
                }
            };

            EntityCollection results = service.RetrieveMultiple(query);

            Console.WriteLine("Retrieved {0} connection role(s) with reciprocal role association.", results.Entities.Count);
            Console.WriteLine();

            foreach (Entity role in results.Entities)
            {
                Console.WriteLine("Connection Role ID: {0}", role["connectionroleid"]);
                Console.WriteLine("Connection Role Name: {0}", role["name"]);
                Console.WriteLine();
            }
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
