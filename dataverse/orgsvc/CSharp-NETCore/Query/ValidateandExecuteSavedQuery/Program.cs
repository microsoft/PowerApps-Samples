using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using System.Text;
using System.Xml;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates validating and executing saved queries (views)
    /// </summary>
    /// <remarks>
    /// This sample shows how to:
    /// 1. Create a saved query (system view) and user query (personal view)
    /// 2. Validate the saved query using ValidateSavedQueryRequest
    /// 3. Execute the saved query using ExecuteByIdSavedQueryRequest
    /// 4. Execute a user query using ExecuteByIdUserQueryRequest
    ///
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
                ["name"] = "Coho Vineyard"
            };
            Guid account1Id = service.Create(account1);
            entityStore.Add(new EntityReference("account", account1Id));
            Console.WriteLine("  Created Account: {0}", account1["name"]);

            var account2 = new Entity("account")
            {
                ["name"] = "Coho Winery"
            };
            Guid account2Id = service.Create(account2);
            entityStore.Add(new EntityReference("account", account2Id));
            Console.WriteLine("  Created Account: {0}", account2["name"]);

            var account3 = new Entity("account")
            {
                ["name"] = "Coho Vineyard & Winery"
            };
            Guid account3Id = service.Create(account3);
            entityStore.Add(new EntityReference("account", account3Id));
            Console.WriteLine("  Created Account: {0}", account3["name"]);

            Console.WriteLine();
            Console.WriteLine("Creating a Saved Query that retrieves all Account names...");

            var savedQuery = new Entity("savedquery")
            {
                ["name"] = "Fetch all Account ids",
                ["returnedtypecode"] = "account",
                ["fetchxml"] = @"
                    <fetch mapping='logical'>
                        <entity name='account'>
                            <attribute name='name' />
                        </entity>
                    </fetch>",
                ["querytype"] = 0
            };
            Guid savedQueryId = service.Create(savedQuery);
            entityStore.Add(new EntityReference("savedquery", savedQueryId));

            Console.WriteLine();
            Console.WriteLine("Creating a User Query that retrieves Account 'Coho Winery'...");

            var userQuery = new Entity("userquery")
            {
                ["name"] = "Fetch Coho Winery",
                ["returnedtypecode"] = "account",
                ["fetchxml"] = @"
                    <fetch mapping='logical'>
                        <entity name='account'>
                            <attribute name='name' />
                            <filter>
                                <condition attribute='name' operator='eq' value='Coho Winery' />
                            </filter>
                        </entity>
                    </fetch>",
                ["querytype"] = 0
            };
            Guid userQueryId = service.Create(userQuery);
            entityStore.Add(new EntityReference("userquery", userQueryId));

            Console.WriteLine("Setup complete.");
            Console.WriteLine();
        }

        private static void Run(ServiceClient service)
        {
            // Get the saved query and user query from entityStore
            var savedQueryRef = entityStore.First(e => e.LogicalName == "savedquery");
            var userQueryRef = entityStore.First(e => e.LogicalName == "userquery");

            // Retrieve the saved query to get its FetchXml
            var savedQuery = service.Retrieve("savedquery", savedQueryRef.Id,
                new Microsoft.Xrm.Sdk.Query.ColumnSet("fetchxml", "querytype"));

            Console.WriteLine("Validating Saved Query");
            Console.WriteLine("======================");

            // Create the validate request
            var validateRequest = new ValidateSavedQueryRequest()
            {
                FetchXml = savedQuery.GetAttributeValue<string>("fetchxml"),
                QueryType = savedQuery.GetAttributeValue<int>("querytype")
            };

            try
            {
                // Execute the validate request (will throw if invalid)
                var validateResponse = (ValidateSavedQueryResponse)service.Execute(validateRequest);
                Console.WriteLine("  Saved Query validated successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("  Invalid Saved Query: {0}", ex.Message);
                throw;
            }

            Console.WriteLine();
            Console.WriteLine("Executing Saved Query");
            Console.WriteLine("=====================");

            // Create the execute saved query request
            var executeSavedQueryRequest = new ExecuteByIdSavedQueryRequest()
            {
                EntityId = savedQueryRef.Id
            };

            // Execute the saved query
            var executeSavedQueryResponse =
                (ExecuteByIdSavedQueryResponse)service.Execute(executeSavedQueryRequest);

            // Check results
            if (string.IsNullOrEmpty(executeSavedQueryResponse.String))
            {
                throw new Exception("Saved Query did not return any results");
            }

            PrintResults(executeSavedQueryResponse.String);

            Console.WriteLine();
            Console.WriteLine("Executing User Query");
            Console.WriteLine("====================");

            // Create the execute user query request
            var executeUserQueryRequest = new ExecuteByIdUserQueryRequest()
            {
                EntityId = userQueryRef
            };

            // Execute the user query
            var executeUserQueryResponse =
                (ExecuteByIdUserQueryResponse)service.Execute(executeUserQueryRequest);

            // Check results
            if (string.IsNullOrEmpty(executeUserQueryResponse.String))
            {
                throw new Exception("User Query did not return any results");
            }

            PrintResults(executeUserQueryResponse.String);
        }

        private static void Cleanup(ServiceClient service, bool deleteCreatedRecords)
        {
            Console.WriteLine();
            Console.WriteLine("Cleaning up...");

            if (deleteCreatedRecords && entityStore.Count > 0)
            {
                Console.WriteLine($"Deleting {entityStore.Count} created record(s)...");

                // Delete in reverse order (important for dependencies)
                for (int i = entityStore.Count - 1; i >= 0; i--)
                {
                    service.Delete(entityStore[i].LogicalName, entityStore[i].Id);
                }

                Console.WriteLine("Records deleted.");
            }
        }

        /// <summary>
        /// Formats and prints the XML results from query execution
        /// </summary>
        private static void PrintResults(string response)
        {
            var output = new StringBuilder();
            using (XmlReader reader = XmlReader.Create(new StringReader(response)))
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = true
                };

                using (XmlWriter writer = XmlWriter.Create(output, settings))
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                writer.WriteStartElement(reader.Name);
                                break;
                            case XmlNodeType.Text:
                                writer.WriteString(reader.Value);
                                break;
                            case XmlNodeType.XmlDeclaration:
                            case XmlNodeType.ProcessingInstruction:
                                writer.WriteProcessingInstruction(reader.Name, reader.Value);
                                break;
                            case XmlNodeType.Comment:
                                writer.WriteComment(reader.Value);
                                break;
                            case XmlNodeType.EndElement:
                                writer.WriteFullEndElement();
                                break;
                        }
                    }
                }
            }

            Console.WriteLine("  Result of query:");
            Console.WriteLine(output.ToString());
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
                Console.WriteLine();
                Console.WriteLine("An error occurred:");
                Console.WriteLine(ex.Message);

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception: {0}", ex.InnerException.Message);
                }
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
