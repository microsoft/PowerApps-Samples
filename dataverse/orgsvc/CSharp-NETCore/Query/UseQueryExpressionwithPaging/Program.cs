using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

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

            var parentAccount = new Entity("account")
            {
                ["name"] = "Root Test Account",
                ["emailaddress1"] = "root@root.com"
            };
            parentAccountId = service.Create(parentAccount);
            entityStore.Add(new EntityReference("account", parentAccountId));

            for (int i = 1; i <= 10; i++)
            {
                var childAccount = new Entity("account")
                {
                    ["name"] = $"Child Test Account {i}",
                    ["emailaddress1"] = $"child{i}@root.com",
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
            int queryCount = 3;
            int pageNumber = 1;
            int recordCount = 0;

            var pagecondition = new ConditionExpression
            {
                AttributeName = "parentaccountid",
                Operator = ConditionOperator.Equal
            };
            pagecondition.Values.Add(parentAccountId);

            var order = new OrderExpression
            {
                AttributeName = "name",
                OrderType = OrderType.Ascending
            };

            var pagequery = new QueryExpression
            {
                EntityName = "account",
                ColumnSet = new ColumnSet("name", "emailaddress1")
            };
            pagequery.Criteria.AddCondition(pagecondition);
            pagequery.Orders.Add(order);

            pagequery.PageInfo = new PagingInfo
            {
                Count = queryCount,
                PageNumber = pageNumber,
                PagingCookie = null
            };

            Console.WriteLine("Retrieving sample account records in pages...\n");
            Console.WriteLine("#\tAccount Name\t\t\tEmail Address");

            while (true)
            {
                EntityCollection results = service.RetrieveMultiple(pagequery);
                if (results.Entities != null)
                {
                    foreach (Entity acct in results.Entities)
                    {
                        string name = acct.Contains("name") ? acct["name"].ToString() : "";
                        string email = acct.Contains("emailaddress1") ? acct["emailaddress1"].ToString() : "";
                        Console.WriteLine("{0}.\t{1}\t{2}", ++recordCount, name, email);
                    }
                }

                if (results.MoreRecords)
                {
                    Console.WriteLine("\n****************\nPage number {0}\n****************", pagequery.PageInfo.PageNumber);
                    Console.WriteLine("#\tAccount Name\t\t\tEmail Address");

                    pagequery.PageInfo.PageNumber++;
                    pagequery.PageInfo.PagingCookie = results.PagingCookie;
                }
                else
                {
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
