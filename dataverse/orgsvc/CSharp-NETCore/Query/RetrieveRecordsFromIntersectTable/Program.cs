using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System.Text;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates retrieving records from an intersect table (many-to-many relationship)
    /// </summary>
    /// <remarks>
    /// This sample shows three different approaches to querying intersect tables:
    /// 1. QueryExpression with LinkEntity
    /// 2. FetchXML with link-entity and intersect="true"
    /// 3. Direct query of the intersect table
    ///
    /// The sample creates a custom role and associates it with the current user,
    /// then demonstrates how to retrieve the association records from the
    /// systemuserroles intersect table.
    ///
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// </remarks>
    class Program
    {
        private static readonly List<EntityReference> entityStore = new();
        private static Guid userId;
        private static Guid roleId;

        #region Sample Methods

        private static void Setup(ServiceClient service)
        {
            Console.WriteLine("Creating sample data...");

            // Retrieve the default business unit needed to create the role
            var queryDefaultBusinessUnit = new QueryExpression
            {
                EntityName = "businessunit",
                ColumnSet = new ColumnSet("businessunitid"),
                Criteria = new FilterExpression()
            };

            // Find the root business unit (parent is null)
            queryDefaultBusinessUnit.Criteria.AddCondition(
                "parentbusinessunitid",
                ConditionOperator.Null);

            EntityCollection businessUnits = service.RetrieveMultiple(queryDefaultBusinessUnit);

            if (businessUnits.Entities.Count == 0)
            {
                throw new Exception("No default business unit found.");
            }

            Entity defaultBusinessUnit = businessUnits.Entities[0];
            Guid businessUnitId = defaultBusinessUnit.Id;

            // Get the GUID of the current user
            var whoRequest = new WhoAmIRequest();
            var whoResponse = (WhoAmIResponse)service.Execute(whoRequest);
            userId = whoResponse.UserId;
            Console.WriteLine($"Current User ID: {userId}");

            // Create a custom role
            var role = new Entity("role")
            {
                ["name"] = "ABC Management Role",
                ["businessunitid"] = new EntityReference("businessunit", businessUnitId)
            };

            roleId = service.Create(role);
            entityStore.Add(new EntityReference("role", roleId));
            Console.WriteLine($"Created Role: {roleId}");

            // Associate the user with the role using the systemuserroles_association relationship
            var associateRequest = new AssociateRequest
            {
                Target = new EntityReference("systemuser", userId),
                RelatedEntities = new EntityReferenceCollection
                {
                    new EntityReference("role", roleId)
                },
                Relationship = new Relationship("systemuserroles_association")
            };

            service.Execute(associateRequest);
            Console.WriteLine($"Associated User {userId} with Role {roleId}");
            Console.WriteLine();
            Console.WriteLine("Setup complete.");
            Console.WriteLine();
        }

        private static void Run(ServiceClient service)
        {
            Console.WriteLine("=== Retrieving Records from Intersect Table ===");
            Console.WriteLine();

            // Approach 1: QueryExpression with LinkEntity
            Console.WriteLine("Approach 1: QueryExpression with LinkEntity");
            Console.WriteLine("--------------------------------------------");
            RetrieveWithQueryExpression(service);
            Console.WriteLine();

            // Approach 2: FetchXML
            Console.WriteLine("Approach 2: FetchXML with intersect link-entity");
            Console.WriteLine("------------------------------------------------");
            RetrieveWithFetchXML(service);
            Console.WriteLine();

            // Approach 3: Direct query of intersect table
            Console.WriteLine("Approach 3: Direct query of intersect table");
            Console.WriteLine("--------------------------------------------");
            RetrieveIntersectTableDirectly(service);
            Console.WriteLine();
        }

        private static void RetrieveWithQueryExpression(ServiceClient service)
        {
            // Create QueryExpression that links from role to systemuserroles intersect table
            var query = new QueryExpression
            {
                EntityName = "role",
                ColumnSet = new ColumnSet("name")
            };

            // Add link to the systemuserroles intersect table
            var linkEntity = new LinkEntity
            {
                LinkFromEntityName = "role",
                LinkFromAttributeName = "roleid",
                LinkToEntityName = "systemuserroles",
                LinkToAttributeName = "roleid",
                LinkCriteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression
                        {
                            AttributeName = "systemuserid",
                            Operator = ConditionOperator.Equal,
                            Values = { userId }
                        }
                    }
                }
            };
            query.LinkEntities.Add(linkEntity);

            // Execute the query
            EntityCollection results = service.RetrieveMultiple(query);

            // Display results
            Console.WriteLine($"QueryExpression retrieved {results.Entities.Count} role(s):");
            foreach (Entity role in results.Entities)
            {
                Console.WriteLine($"  - Role Name: {role.GetAttributeValue<string>("name")}");
            }
        }

        private static void RetrieveWithFetchXML(ServiceClient service)
        {
            // Build FetchXML query with intersect link
            var fetchXml = new StringBuilder();
            fetchXml.Append("<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"true\">");
            fetchXml.Append("  <entity name=\"role\">");
            fetchXml.Append("    <attribute name=\"name\"/>");
            fetchXml.Append("    <link-entity name=\"systemuserroles\" from=\"roleid\" to=\"roleid\" visible=\"false\" intersect=\"true\">");
            fetchXml.Append("      <filter type=\"and\">");
            fetchXml.Append($"        <condition attribute=\"systemuserid\" operator=\"eq\" value=\"{userId}\"/>");
            fetchXml.Append("      </filter>");
            fetchXml.Append("    </link-entity>");
            fetchXml.Append("  </entity>");
            fetchXml.Append("</fetch>");

            // Execute the FetchXML query
            var fetchRequest = new RetrieveMultipleRequest
            {
                Query = new FetchExpression(fetchXml.ToString())
            };
            var fetchResponse = (RetrieveMultipleResponse)service.Execute(fetchRequest);
            EntityCollection results = fetchResponse.EntityCollection;

            // Display results
            Console.WriteLine($"FetchXML retrieved {results.Entities.Count} role(s):");
            foreach (Entity role in results.Entities)
            {
                Console.WriteLine($"  - Role Name: {role.GetAttributeValue<string>("name")}");
            }
        }

        private static void RetrieveIntersectTableDirectly(ServiceClient service)
        {
            // Query the systemuserroles intersect table directly
            var query = new QueryExpression
            {
                EntityName = "systemuserroles",
                ColumnSet = new ColumnSet("systemuserid", "roleid"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression
                        {
                            AttributeName = "systemuserid",
                            Operator = ConditionOperator.Equal,
                            Values = { userId }
                        },
                        new ConditionExpression
                        {
                            AttributeName = "roleid",
                            Operator = ConditionOperator.Equal,
                            Values = { roleId }
                        }
                    }
                }
            };

            EntityCollection results = service.RetrieveMultiple(query);

            // Display results
            Console.WriteLine($"Direct query retrieved {results.Entities.Count} association(s):");
            foreach (Entity association in results.Entities)
            {
                Guid systemUserId = association.GetAttributeValue<Guid>("systemuserid");
                Guid associatedRoleId = association.GetAttributeValue<Guid>("roleid");
                Console.WriteLine($"  - User ID: {systemUserId}, Role ID: {associatedRoleId}");
            }
        }

        private static void Cleanup(ServiceClient service, bool deleteCreatedRecords)
        {
            Console.WriteLine("Cleaning up...");
            if (deleteCreatedRecords && entityStore.Count > 0)
            {
                // Disassociate the user from the role before deleting
                try
                {
                    var disassociateRequest = new DisassociateRequest
                    {
                        Target = new EntityReference("systemuser", userId),
                        RelatedEntities = new EntityReferenceCollection
                        {
                            new EntityReference("role", roleId)
                        },
                        Relationship = new Relationship("systemuserroles_association")
                    };
                    service.Execute(disassociateRequest);
                    Console.WriteLine("Disassociated user from role.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during disassociation: {ex.Message}");
                }

                // Delete created records
                Console.WriteLine($"Deleting {entityStore.Count} created record(s)...");
                for (int i = entityStore.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        service.Delete(entityStore[i].LogicalName, entityStore[i].Id);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting {entityStore[i].LogicalName} {entityStore[i].Id}: {ex.Message}");
                    }
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
                Console.WriteLine();
                Console.WriteLine("Stack Trace:");
                Console.WriteLine(ex.StackTrace);
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
