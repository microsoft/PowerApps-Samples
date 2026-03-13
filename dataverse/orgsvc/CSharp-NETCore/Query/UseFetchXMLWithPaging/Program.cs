using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System.Text;
using System.Xml;

namespace PowerPlatform.Dataverse.CodeSamples
{
    class Program
    {
        private static readonly List<EntityReference> entityStore = new();
        private static Guid parentAccountId;

        #region Sample Methods

        private static void Setup(ServiceClient service)
        {
            Console.WriteLine("Creating sample account records...");

            // Create parent account
            var parentAccount = new Entity("account")
            {
                ["name"] = "Root Test Account",
                ["emailaddress1"] = "root@root.com"
            };
            parentAccountId = service.Create(parentAccount);
            entityStore.Add(new EntityReference("account", parentAccountId));

            // Create 10 child accounts
            for (int i = 1; i <= 10; i++)
            {
                var childAccount = new Entity("account")
                {
                    ["name"] = $"Child Test Account {i}",
                    ["emailaddress1"] = $"child{i}@root.com",
                    ["emailaddress2"] = "same@root.com",
                    ["parentaccountid"] = new EntityReference("account", parentAccountId)
                };
                Guid childId = service.Create(childAccount);
                entityStore.Add(new EntityReference("account", childId));
            }

            Console.WriteLine("Created 1 parent and 10 child accounts.");
            Console.WriteLine();
        }

        private static void Run(ServiceClient service)
        {
            // Define the fetch attributes.
            // Set the number of records per page to retrieve.
            int fetchCount = 3;
            // Initialize the page number.
            int pageNumber = 1;
            // Initialize the number of records.
            int recordCount = 0;
            // Specify the current paging cookie. For retrieving the first page,
            // pagingCookie should be null.
            string? pagingCookie = null;

            // Create the FetchXml string for retrieving all child accounts to a parent account.
            // This fetch query is using 1 placeholder to specify the parent account id
            // for filtering out required accounts. Filter query is optional.
            // Fetch query also includes optional order criteria that, in this case, is used
            // to order the results in ascending order on the name data column.
            string fetchXml = string.Format(@"<fetch version='1.0'
                                            mapping='logical'
                                            output-format='xml-platform'>
                                            <entity name='account'>
                                                <attribute name='name' />
                                                <attribute name='emailaddress1' />
                                                <order attribute='name' descending='false'/>
                                                <filter type='and'>
                                                    <condition attribute='parentaccountid'
                                                        operator='eq' value='{0}' uiname='' uitype='' />
                                                </filter>
                                            </entity>
                                        </fetch>",
                                            parentAccountId);

            Console.WriteLine("Retrieving data in pages\n");
            Console.WriteLine("#\tAccount Name\t\t\tEmail Address");

            while (true)
            {
                // Build fetchXml string with the placeholders.
                string xml = CreateXml(fetchXml, pagingCookie, pageNumber, fetchCount);

                // Execute the fetch query and get the xml result.
                var fetchRequest = new RetrieveMultipleRequest
                {
                    Query = new FetchExpression(xml)
                };

                EntityCollection returnCollection = ((RetrieveMultipleResponse)service.Execute(fetchRequest)).EntityCollection;

                foreach (var c in returnCollection.Entities)
                {
                    string name = c.Contains("name") ? c["name"].ToString() ?? "" : "";
                    string email = c.Contains("emailaddress1") ? c["emailaddress1"].ToString() ?? "" : "";
                    Console.WriteLine("{0}.\t{1}\t\t{2}", ++recordCount, name, email);
                }

                // Check for morerecords, if it returns true.
                if (returnCollection.MoreRecords)
                {
                    Console.WriteLine("\n****************\nPage number {0}\n****************", pageNumber);
                    Console.WriteLine("#\tAccount Name\t\t\tEmail Address");

                    // Increment the page number to retrieve the next page.
                    pageNumber++;

                    // Set the paging cookie to the paging cookie returned from current results.
                    pagingCookie = returnCollection.PagingCookie;
                }
                else
                {
                    // If no more records in the result nodes, exit the loop.
                    break;
                }
            }
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
        /// Creates an XML string with paging information added to the FetchXML query.
        /// </summary>
        /// <param name="xml">The base FetchXML query string</param>
        /// <param name="cookie">The paging cookie from the previous page (null for first page)</param>
        /// <param name="page">The page number to retrieve</param>
        /// <param name="count">The number of records per page</param>
        /// <returns>The FetchXML string with paging attributes added</returns>
        private static string CreateXml(string xml, string? cookie, int page, int count)
        {
            StringReader stringReader = new StringReader(xml);
            XmlTextReader reader = new XmlTextReader(stringReader);

            // Load document
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            XmlAttributeCollection attrs = doc.DocumentElement!.Attributes;

            if (cookie != null)
            {
                XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
                pagingAttr.Value = cookie;
                attrs.Append(pagingAttr);
            }

            XmlAttribute pageAttr = doc.CreateAttribute("page");
            pageAttr.Value = page.ToString();
            attrs.Append(pageAttr);

            XmlAttribute countAttr = doc.CreateAttribute("count");
            countAttr.Value = count.ToString();
            attrs.Append(countAttr);

            StringBuilder sb = new StringBuilder(1024);
            StringWriter stringWriter = new StringWriter(sb);

            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            doc.WriteTo(writer);
            writer.Close();

            return sb.ToString();
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
