using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
using System.Xml;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates using UpsertRequest to insert or update records based on alternate keys
    /// </summary>
    /// <remarks>
    /// This sample shows how to use UpsertRequest for insert-or-update operations
    /// with alternate keys. The sample processes XML files containing product data
    /// and uses the product code as an alternate key to either create new records
    /// or update existing ones.
    ///
    /// IMPORTANT: This sample requires the UpsertSample managed solution to be installed.
    /// The solution creates a sample_product table with a sample_productcode alternate key.
    ///
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// </remarks>
    class Program
    {
        private static readonly List<EntityReference> entityStore = new();
        private static Guid? asyncJobId = null;

        #region Sample Methods

        private static void Setup(ServiceClient service)
        {
            Console.WriteLine("=== Setup ===");
            Console.WriteLine();

            // Note: This sample requires the UpsertSample managed solution to be installed.
            // The solution creates:
            // - sample_product table
            // - sample_productcode alternate key on the Code field
            //
            // The solution file should be available as UpsertSample_1_0_0_0_managed.zip
            //
            // This sample assumes the solution is already installed.
            // If not installed, you will see errors when trying to upsert records.

            Console.WriteLine("Verifying alternate key is active...");

            if (!VerifyProductCodeKeyIsActive(service))
            {
                throw new Exception(
                    "The sample_productcode alternate key is not active. " +
                    "Please ensure the UpsertSample managed solution is installed " +
                    "and the alternate key indexes are fully built.");
            }

            Console.WriteLine("Alternate key is active and ready.");
            Console.WriteLine("Setup complete.");
            Console.WriteLine();
        }

        private static void Run(ServiceClient service)
        {
            Console.WriteLine("=== Demonstrate UpsertRequest ===");
            Console.WriteLine();

            // First pass: Process newsampleproduct.xml to create 13 new product records
            Console.WriteLine("Processing newsampleproduct.xml...");
            ProcessUpsert(service, "newsampleproduct.xml");
            Console.WriteLine();

            // Second pass: Process updatedsampleproduct.xml to update 6 existing products
            Console.WriteLine("Processing updatedsampleproduct.xml...");
            ProcessUpsert(service, "updatedsampleproduct.xml");
            Console.WriteLine();

            Console.WriteLine("UpsertRequest demonstration complete.");
        }

        private static void Cleanup(ServiceClient service, bool deleteCreatedRecords)
        {
            Console.WriteLine("=== Cleanup ===");
            Console.WriteLine();

            if (deleteCreatedRecords && entityStore.Count > 0)
            {
                Console.WriteLine($"Deleting {entityStore.Count} created record(s)...");

                // Delete in reverse order
                for (int i = entityStore.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        service.Delete(entityStore[i].LogicalName, entityStore[i].Id);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting record {entityStore[i].Id}: {ex.Message}");
                    }
                }

                Console.WriteLine("Records deleted.");
            }
            else if (!deleteCreatedRecords)
            {
                Console.WriteLine("Skipping record deletion.");
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Verifies that the alternate key index for sample_productcode is active
        /// </summary>
        private static bool VerifyProductCodeKeyIsActive(ServiceClient service, int iteration = 0)
        {
            const int maxIterations = 5;

            if (iteration >= maxIterations)
            {
                Console.WriteLine("Maximum verification attempts reached.");
                return false;
            }

            try
            {
                // Query metadata for the sample_product entity to check key status
                var entityQuery = new EntityQueryExpression
                {
                    Criteria = new MetadataFilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new MetadataConditionExpression("LogicalName", MetadataConditionOperator.Equals, "sample_product")
                        }
                    },
                    Properties = new MetadataPropertiesExpression("Keys")
                };

                var metadataRequest = new RetrieveMetadataChangesRequest { Query = entityQuery };
                var metadataResponse = (RetrieveMetadataChangesResponse)service.Execute(metadataRequest);

                if (metadataResponse.EntityMetadata.Count == 0)
                {
                    Console.WriteLine("sample_product entity not found. Ensure UpsertSample solution is installed.");
                    return false;
                }

                var productEntity = metadataResponse.EntityMetadata[0];

                if (productEntity.Keys == null || productEntity.Keys.Length == 0)
                {
                    Console.WriteLine("No alternate keys found on sample_product entity.");
                    return false;
                }

                var productCodeKey = productEntity.Keys.FirstOrDefault(k => k.LogicalName == "sample_productcode");

                if (productCodeKey == null)
                {
                    Console.WriteLine("sample_productcode alternate key not found.");
                    return false;
                }

                if (productCodeKey.EntityKeyIndexStatus == EntityKeyIndexStatus.Active)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine($"Alternate key status: {productCodeKey.EntityKeyIndexStatus}");

                    // If there's an async job building the index, wait and retry
                    if (productCodeKey.AsyncJob != null)
                    {
                        asyncJobId = productCodeKey.AsyncJob.Id;
                        Console.WriteLine($"Waiting 30 seconds for index creation (attempt {iteration + 1}/{maxIterations})...");
                        Thread.Sleep(TimeSpan.FromSeconds(30));
                        return VerifyProductCodeKeyIsActive(service, iteration + 1);
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying alternate key: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Processes an XML file containing product data and performs upsert operations
        /// </summary>
        private static void ProcessUpsert(ServiceClient service, string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"Error: File '{filename}' not found.");
                return;
            }

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(filename);

                XmlNodeList? productNodes = xmlDoc.DocumentElement?.SelectNodes("/products/product");

                if (productNodes == null || productNodes.Count == 0)
                {
                    Console.WriteLine("No product nodes found in XML file.");
                    return;
                }

                int createCount = 0;
                int updateCount = 0;

                foreach (XmlNode productNode in productNodes)
                {
                    string? productCode = productNode.SelectSingleNode("Code")?.InnerText;
                    string? productName = productNode.SelectSingleNode("Name")?.InnerText;
                    string? productCategory = productNode.SelectSingleNode("Category")?.InnerText;
                    string? productMake = productNode.SelectSingleNode("Make")?.InnerText;

                    if (string.IsNullOrEmpty(productCode))
                    {
                        Console.WriteLine("Skipping product with missing code.");
                        continue;
                    }

                    // Create entity using alternate key constructor
                    // This specifies the alternate key name and value to use for upsert
                    var product = new Entity("sample_product", "sample_productcode", productCode)
                    {
                        ["sample_name"] = productName,
                        ["sample_category"] = productCategory,
                        ["sample_make"] = productMake
                    };

                    // Create and execute UpsertRequest
                    var upsertRequest = new UpsertRequest
                    {
                        Target = product
                    };

                    try
                    {
                        var upsertResponse = (UpsertResponse)service.Execute(upsertRequest);

                        if (upsertResponse.RecordCreated)
                        {
                            createCount++;
                            Console.WriteLine($"  Created: {productName} (Code: {productCode})");

                            // Track created records for cleanup
                            // Use the Target entity which now contains the Id after upsert
                            if (product.Id != Guid.Empty)
                            {
                                entityStore.Add(new EntityReference("sample_product", product.Id));
                            }
                        }
                        else
                        {
                            updateCount++;
                            Console.WriteLine($"  Updated: {productName} (Code: {productCode})");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  Error upserting {productName}: {ex.Message}");
                    }
                }

                Console.WriteLine();
                Console.WriteLine($"Summary: {createCount} created, {updateCount} updated");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing XML file: {ex.Message}");
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
                Console.WriteLine();
                Console.WriteLine("An error occurred:");
                Console.WriteLine(ex.Message);

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner exception:");
                    Console.WriteLine(ex.InnerException.Message);
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
