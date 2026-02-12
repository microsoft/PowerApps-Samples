using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates converting between FetchXML and QueryExpression
    /// </summary>
    /// <remarks>
    /// This sample shows how to:
    /// 1. Convert QueryExpression to FetchXML using QueryExpressionToFetchXmlRequest
    /// 2. Convert FetchXML to QueryExpression using FetchXmlToQueryExpressionRequest
    /// 3. Execute queries using both formats and compare results
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

            // Create an account
            var account = new Entity("account")
            {
                ["name"] = "Litware, Inc.",
                ["address1_stateorprovince"] = "Colorado"
            };
            Guid accountId = service.Create(account);
            entityStore.Add(new EntityReference("account", accountId));

            // Create the first contact
            var contact1 = new Entity("contact")
            {
                ["firstname"] = "Ben",
                ["lastname"] = "Andrews",
                ["emailaddress1"] = "sample@example.com",
                ["address1_city"] = "Redmond",
                ["address1_stateorprovince"] = "WA",
                ["address1_telephone1"] = "(206)555-5555",
                ["parentcustomerid"] = new EntityReference("account", accountId)
            };
            Guid contactId1 = service.Create(contact1);
            entityStore.Add(new EntityReference("contact", contactId1));

            // Create the second contact
            var contact2 = new Entity("contact")
            {
                ["firstname"] = "Colin",
                ["lastname"] = "Wilcox",
                ["emailaddress1"] = "sample@example.com",
                ["address1_city"] = "Bellevue",
                ["address1_stateorprovince"] = "WA",
                ["address1_telephone1"] = "(425)555-5555",
                ["parentcustomerid"] = new EntityReference("account", accountId)
            };
            Guid contactId2 = service.Create(contact2);
            entityStore.Add(new EntityReference("contact", contactId2));

            // Create the first opportunity
            var opportunity1 = new Entity("opportunity")
            {
                ["name"] = "Litware, Inc. Opportunity 1",
                ["estimatedclosedate"] = DateTime.Now.AddMonths(6),
                ["customerid"] = new EntityReference("account", accountId)
            };
            Guid opportunityId1 = service.Create(opportunity1);
            entityStore.Add(new EntityReference("opportunity", opportunityId1));

            // Create the second opportunity
            var opportunity2 = new Entity("opportunity")
            {
                ["name"] = "Litware, Inc. Opportunity 2",
                ["estimatedclosedate"] = DateTime.Now.AddYears(4),
                ["customerid"] = new EntityReference("account", accountId)
            };
            Guid opportunityId2 = service.Create(opportunity2);
            entityStore.Add(new EntityReference("opportunity", opportunityId2));

            Console.WriteLine("Setup complete.");
            Console.WriteLine();
        }

        private static void Run(ServiceClient service)
        {
            // Demonstrate QueryExpression to FetchXML conversion
            DoQueryExpressionToFetchXmlConversion(service);

            // Demonstrate FetchXML to QueryExpression conversion
            DoFetchXmlToQueryExpressionConversion(service);
        }

        private static void DoQueryExpressionToFetchXmlConversion(ServiceClient service)
        {
            Console.WriteLine("=== QueryExpression to FetchXML Conversion ===");
            Console.WriteLine();

            // Build a query expression that we will turn into FetchXML
            var queryExpression = new QueryExpression()
            {
                Distinct = false,
                EntityName = "contact",
                ColumnSet = new ColumnSet("fullname", "address1_telephone1"),
                LinkEntities =
                {
                    new LinkEntity
                    {
                        JoinOperator = JoinOperator.LeftOuter,
                        LinkFromAttributeName = "parentcustomerid",
                        LinkFromEntityName = "contact",
                        LinkToAttributeName = "accountid",
                        LinkToEntityName = "account",
                        LinkCriteria =
                        {
                            Conditions =
                            {
                                new ConditionExpression("name", ConditionOperator.Equal, "Litware, Inc.")
                            }
                        }
                    }
                },
                Criteria =
                {
                    Filters =
                    {
                        new FilterExpression
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions =
                            {
                                new ConditionExpression("address1_stateorprovince", ConditionOperator.Equal, "WA"),
                                new ConditionExpression("address1_city", ConditionOperator.In, new String[] {"Redmond", "Bellevue" , "Kirkland", "Seattle"}),
                                new ConditionExpression("createdon", ConditionOperator.LastXDays, 30),
                                new ConditionExpression("emailaddress1", ConditionOperator.NotNull)
                            },
                        },
                        new FilterExpression
                        {
                            FilterOperator = LogicalOperator.Or,
                            Conditions =
                            {
                                new ConditionExpression("address1_telephone1", ConditionOperator.Like, "(206)%"),
                                new ConditionExpression("address1_telephone1", ConditionOperator.Like, "(425)%")
                            }
                        }
                    }
                }
            };

            // Run the query as a query expression
            EntityCollection queryExpressionResult = service.RetrieveMultiple(queryExpression);
            Console.WriteLine("Output for query as QueryExpression:");
            DisplayContactQueryResults(queryExpressionResult);

            // Convert the query expression to FetchXML
            var conversionRequest = new QueryExpressionToFetchXmlRequest
            {
                Query = queryExpression
            };
            var conversionResponse = (QueryExpressionToFetchXmlResponse)service.Execute(conversionRequest);

            // Use the converted query to make a retrieve multiple request
            string fetchXml = conversionResponse.FetchXml;
            Console.WriteLine("Converted FetchXML:");
            Console.WriteLine(fetchXml);
            Console.WriteLine();

            var fetchQuery = new FetchExpression(fetchXml);
            EntityCollection fetchResult = service.RetrieveMultiple(fetchQuery);

            // Display the results
            Console.WriteLine("Output for query after conversion to FetchXML:");
            DisplayContactQueryResults(fetchResult);
            Console.WriteLine();
        }

        private static void DisplayContactQueryResults(EntityCollection result)
        {
            Console.WriteLine("List all contacts matching specified parameters");
            Console.WriteLine("===============================================");
            foreach (Entity entity in result.Entities)
            {
                Console.WriteLine("Contact ID: {0}", entity.Id);
                Console.WriteLine("Contact Name: {0}", entity.GetAttributeValue<string>("fullname"));
                Console.WriteLine("Contact Phone: {0}", entity.GetAttributeValue<string>("address1_telephone1"));
                Console.WriteLine();
            }
            Console.WriteLine("<End of Listing>");
            Console.WriteLine();
        }

        private static void DoFetchXmlToQueryExpressionConversion(ServiceClient service)
        {
            Console.WriteLine("=== FetchXML to QueryExpression Conversion ===");
            Console.WriteLine();

            // Create a Fetch query that we will convert into a query expression
            var fetchXml =
                @"<fetch mapping='logical' version='1.0'>
                    <entity name='opportunity'>
                        <attribute name='name' />
                        <filter>
                            <condition attribute='estimatedclosedate' operator='next-x-fiscal-years' value='3' />
                        </filter>
                        <link-entity name='account' from='accountid' to='customerid'>
                            <link-entity name='contact' from='parentcustomerid' to='accountid'>
                                <attribute name='fullname' />
                                <filter>
                                    <condition attribute='address1_city' operator='eq' value='Bellevue' />
                                    <condition attribute='address1_stateorprovince' operator='eq' value='WA' />
                                </filter>
                            </link-entity>
                        </link-entity>
                    </entity>
                </fetch>";

            Console.WriteLine("Original FetchXML:");
            Console.WriteLine(fetchXml);
            Console.WriteLine();

            // Run the query with the FetchXML
            var fetchExpression = new FetchExpression(fetchXml);
            EntityCollection fetchResult = service.RetrieveMultiple(fetchExpression);
            Console.WriteLine("Output for query as FetchXML:");
            DisplayOpportunityQueryResults(fetchResult);

            // Convert the FetchXML into a query expression
            var conversionRequest = new FetchXmlToQueryExpressionRequest
            {
                FetchXml = fetchXml
            };

            var conversionResponse = (FetchXmlToQueryExpressionResponse)service.Execute(conversionRequest);

            // Use the newly converted query expression to make a retrieve multiple request
            QueryExpression queryExpression = conversionResponse.Query;

            EntityCollection queryResult = service.RetrieveMultiple(queryExpression);

            // Display the results
            Console.WriteLine("Output for query after conversion to QueryExpression:");
            DisplayOpportunityQueryResults(queryResult);
            Console.WriteLine();
        }

        private static void DisplayOpportunityQueryResults(EntityCollection result)
        {
            Console.WriteLine("List all opportunities matching specified parameters.");
            Console.WriteLine("===========================================================================");
            foreach (Entity entity in result.Entities)
            {
                Console.WriteLine("Opportunity ID: {0}", entity.Id);
                Console.WriteLine("Opportunity: {0}", entity.GetAttributeValue<string>("name"));

                // Get the aliased contact name from the linked entity
                if (entity.Contains("contact2.fullname"))
                {
                    var aliased = (AliasedValue)entity["contact2.fullname"];
                    var contactName = (string)aliased.Value;
                    Console.WriteLine("Associated contact: {0}", contactName);
                }
                Console.WriteLine();
            }
            Console.WriteLine("<End of Listing>");
            Console.WriteLine();
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
