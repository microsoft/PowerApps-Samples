using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Data;
using System.Text;
using System.Xml;

namespace PowerPlatform.Dataverse.CodeSamples
{
    class Program
    {
        private static readonly List<EntityReference> entityStore = new();
        private static Guid annotationId;

        #region Sample Methods

        private static void Setup(ServiceClient service)
        {
            Console.WriteLine("Setup: Creating sample account records for export...");

            // Create 5 sample accounts to demonstrate data export
            for (int i = 1; i <= 5; i++)
            {
                var account = new Entity("account")
                {
                    ["name"] = $"Sample Export Account {i}",
                    ["emailaddress1"] = $"export{i}@contoso.com",
                    ["telephone1"] = $"555-010{i}",
                    ["address1_city"] = $"City {i}"
                };
                Guid accountId = service.Create(account);
                entityStore.Add(new EntityReference("account", accountId));
            }

            Console.WriteLine($"Created {entityStore.Count} sample accounts.");
            Console.WriteLine();
        }

        private static void Run(ServiceClient service)
        {
            Console.WriteLine("Demonstrating export of query results to annotation (note)...");
            Console.WriteLine();

            // Create a FetchXML query to retrieve accounts
            // This query will select name, email, phone, and city from accounts
            string fetchXml = @"<fetch version='1.0'
                                      mapping='logical'
                                      output-format='xml-platform'>
                                  <entity name='account'>
                                    <attribute name='name' />
                                    <attribute name='emailaddress1' />
                                    <attribute name='telephone1' />
                                    <attribute name='address1_city' />
                                    <filter type='and'>
                                      <condition attribute='name'
                                                 operator='like'
                                                 value='%Sample Export Account%' />
                                    </filter>
                                    <order attribute='name' descending='false' />
                                  </entity>
                                </fetch>";

            Console.WriteLine("Step 1: Executing FetchXML query to retrieve account records...");
            var fetchedRecords = FetchAllDataFromFetchXml(fetchXml, service);
            Console.WriteLine($"Retrieved {fetchedRecords.Count} records.");
            Console.WriteLine();

            Console.WriteLine("Step 2: Converting entity data to CSV format...");
            var csvString = ConvertEntitiesToCsv(fetchedRecords);
            Console.WriteLine("CSV conversion complete.");
            Console.WriteLine();
            Console.WriteLine("CSV Preview (first 500 characters):");
            Console.WriteLine(csvString.Length > 500 ? csvString.Substring(0, 500) + "..." : csvString);
            Console.WriteLine();

            Console.WriteLine("Step 3: Creating annotation (note) record with CSV data...");
            annotationId = CreateAnnotationWithCsvData(service, csvString);
            entityStore.Add(new EntityReference("annotation", annotationId));
            Console.WriteLine($"Created annotation with ID: {annotationId}");
            Console.WriteLine();

            Console.WriteLine("Export complete! The CSV data has been saved as an annotation.");
            Console.WriteLine("You can view this annotation in Dataverse under the Notes section.");
        }

        private static void Cleanup(ServiceClient service, bool deleteCreatedRecords)
        {
            Console.WriteLine("\nCleaning up...");
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

        /// <summary>
        /// Retrieves all records defined by a FetchXML query, handling paging automatically.
        /// </summary>
        /// <param name="fetchXml">The FetchXML query definition.</param>
        /// <param name="service">The ServiceClient instance.</param>
        /// <returns>A list of all entities retrieved by the query.</returns>
        private static List<Entity> FetchAllDataFromFetchXml(string fetchXml, ServiceClient service)
        {
            List<Entity> allRecords = new List<Entity>();
            int pageNumber = 1;
            string? pagingCookie = null;

            // Retrieve first page
            EntityCollection result = service.RetrieveMultiple(new FetchExpression(fetchXml));
            allRecords.AddRange(result.Entities);

            // Continue retrieving pages while more records exist
            while (result.MoreRecords)
            {
                pageNumber++;
                pagingCookie = result.PagingCookie;

                // Modify FetchXML to include paging information
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(fetchXml);
                var attributes = xmlDoc.DocumentElement!.Attributes!;

                if (!string.IsNullOrEmpty(pagingCookie))
                {
                    var cookieAttribute = xmlDoc.CreateAttribute("paging-cookie");
                    cookieAttribute.Value = pagingCookie;
                    attributes.SetNamedItem(cookieAttribute);
                }

                var pageAttribute = attributes.GetNamedItem("page");
                if (pageAttribute != null)
                {
                    pageAttribute.Value = pageNumber.ToString();
                }
                else
                {
                    pageAttribute = xmlDoc.CreateAttribute("page");
                    pageAttribute.Value = pageNumber.ToString();
                    attributes.SetNamedItem(pageAttribute);
                }

                // Retrieve next page
                using (var stringWriter = new StringWriter())
                using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    xmlDoc.WriteTo(xmlTextWriter);
                    xmlTextWriter.Flush();
                    result = service.RetrieveMultiple(new FetchExpression(stringWriter.GetStringBuilder().ToString()));
                    allRecords.AddRange(result.Entities);
                }
            }

            return allRecords;
        }

        /// <summary>
        /// Converts a list of entities to CSV format.
        /// </summary>
        /// <param name="entities">The entities to convert.</param>
        /// <returns>A CSV string representation of the entity data.</returns>
        private static string ConvertEntitiesToCsv(List<Entity> entities)
        {
            if (entities.Count == 0)
            {
                return string.Empty;
            }

            // Build a DataTable to organize the entity data
            DataTable dataTable = new DataTable();

            // Populate the DataTable with entity attributes
            foreach (var entity in entities)
            {
                var dataRow = dataTable.NewRow();
                foreach (var attribute in entity.Attributes)
                {
                    // Add column if it doesn't exist
                    if (!dataTable.Columns.Contains(attribute.Key))
                    {
                        dataTable.Columns.Add(attribute.Key);
                    }

                    // Serialize the attribute value appropriately
                    dataRow[attribute.Key] = SerializeAttributeValue(attribute.Value);
                }
                dataTable.Rows.Add(dataRow);
            }

            // Convert DataTable to CSV
            return ConvertDataTableToCsv(dataTable);
        }

        /// <summary>
        /// Serializes Dataverse attribute values to string format suitable for CSV.
        /// </summary>
        /// <param name="value">The attribute value to serialize.</param>
        /// <returns>A string representation of the value.</returns>
        private static string SerializeAttributeValue(object value)
        {
            return value switch
            {
                Money money => money.Value.ToString(),
                OptionSetValue optionSetValue => optionSetValue.Value.ToString(),
                EntityReference entityReference => entityReference.Id.ToString(),
                OptionSetValueCollection optionSetValueCollection =>
                    string.Join(",", optionSetValueCollection.Select(option => option.Value)),
                DateTime datetime => datetime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                AliasedValue aliasedValue => SerializeAttributeValue(aliasedValue.Value),
                _ => value?.ToString() ?? string.Empty
            };
        }

        /// <summary>
        /// Converts a DataTable to CSV format.
        /// </summary>
        /// <param name="dataTable">The DataTable to convert.</param>
        /// <returns>A CSV string.</returns>
        private static string ConvertDataTableToCsv(DataTable dataTable)
        {
            var csv = new StringBuilder();

            // Add header row with column names
            var columnNames = dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            csv.AppendLine(string.Join(",", columnNames));

            // Add data rows
            foreach (DataRow row in dataTable.Rows)
            {
                var fields = row.ItemArray.Select(field =>
                {
                    // Escape quotes and wrap in quotes for CSV format
                    string fieldValue = field?.ToString() ?? string.Empty;
                    return $"\"{fieldValue.Replace("\"", "\"\"")}\"";
                });
                csv.AppendLine(string.Join(",", fields));
            }

            return csv.ToString();
        }

        /// <summary>
        /// Creates an annotation (note) record with CSV data as an attachment.
        /// </summary>
        /// <param name="service">The ServiceClient instance.</param>
        /// <param name="csvString">The CSV data to store in the annotation.</param>
        /// <returns>The ID of the created annotation record.</returns>
        private static Guid CreateAnnotationWithCsvData(ServiceClient service, string csvString)
        {
            Entity annotation = new Entity("annotation")
            {
                ["subject"] = "Export Data Using FetchXml To Csv",
                ["documentbody"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(csvString)),
                ["filename"] = "exportdatausingfetchxml.csv",
                ["mimetype"] = "text/csv"
            };

            return service.Create(annotation);
        }

        #endregion

        #region Application Setup

        IConfiguration Configuration { get; }

        Program()
        {
            string? path = Environment.GetEnvironmentVariable("DATAVERSE_APPSETTINGS");
            if (path == null) path = "appsettings.json";

            Configuration = new ConfigurationBuilder()
                .AddJsonFile(path, optional: false, reloadOnChange: true)
                .Build();
        }

        static void Main(string[] args)
        {
            Program app = new();

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
