using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using MyApp.DataModel;

namespace PowerPlatform_Dataverse_CodeSamples
{
    internal class Program
    {
        ///<snippetAssignChart>
        ///<summary>Assign a chart to a user.</summary>
        ///<param name="service">Authenticated web service connection.</param>
        ///<param name="user">A system user.</param>
        ///<param name="chart">A visualization chart.</param>
        ///<return>Response of the assignment operation.</return>
        static AssignResponse AssignChart(IOrganizationService service, EntityReference user, EntityReference chart)
        {
            var assignRequest = new AssignRequest
            {
                Assignee = new EntityReference
                {
                    LogicalName = SystemUser.EntityLogicalName,
                    Id = user.Id
                },

                Target = new EntityReference
                {
                    LogicalName = UserQueryVisualization.EntityLogicalName,
                    Id = chart.Id
                }
            };
            return (AssignResponse)service.Execute(assignRequest);
        }
        ///</snippetAssignChart>

        /// <summary>
        /// Contains the application's configuration settings. 
        /// </summary>
        static IConfiguration Configuration { get; }

        /// <summary>
        /// Constructor. Loads the application settings from a JSON configuration file.
        /// </summary>
        static Program()
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
            // Entity name and reference collection.
            Dictionary<string, EntityReference> entityStore;

            // Create a Dataverse service client using the default connection string.
            ServiceClient serviceClient =
                new(Configuration.GetConnectionString("default"));

            // Pre-create any table rows that Run() requires.
            Setup(serviceClient, out entityStore);

            // Execute the main logic of this program
            Run(serviceClient, entityStore);

            // Pause program execution before resource cleanup.
            Console.WriteLine("Press any key to undo environment data changes.");
            Console.ReadKey();

            // In Dataverse, delete any created table rows and then dispose the service connection.
            Cleanup(serviceClient, entityStore);
            serviceClient.Dispose();
        }

        /// <summary>
        /// Initializes any pre-existing data and resources required by the Run() method.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Entity name and reference collection.</param>
        static public void Setup(IOrganizationService service, out Dictionary<string,
            EntityReference> entityStore)
        {
            entityStore = new Dictionary<string, EntityReference>();

            // Create a sample account
            Account setupAccount = new Account { Name = "Sample Account" };
            _accountId = service.Create(setupAccount);
            Console.WriteLine("Created {0}.", setupAccount.Name);

            // Create some oppotunity records for the visualization
            Opportunity[] setupOpportunities = new Opportunity[]
                {
                    new Opportunity
                    {
                        Name = "Sample Opp 01",
                        EstimatedValue = new Money(120000.00m),
                        ActualValue = new Money(100000.00m),
                        CustomerId = new EntityReference(Account.EntityLogicalName,
                            _accountId),
                        StepName = "Open"
                    },
                    new Opportunity
                    {
                        Name = "Sample Opp 02",
                        EstimatedValue = new Money(240000.00m),
                        ActualValue = new Money(200000.00m),
                        CustomerId = new EntityReference(Account.EntityLogicalName,
                            _accountId),
                        StepName = "Open"
                    },
                    new Opportunity
                    {
                        Name = "Sample Opp 03",
                        EstimatedValue = new Money(360000.00m),
                        ActualValue = new Money(300000.00m),
                        CustomerId = new EntityReference(Account.EntityLogicalName,
                            _accountId),
                        StepName = "Open"
                    },
                    new Opportunity
                    {
                        Name = "Sample Opp 04",
                        EstimatedValue = new Money(500000.00m),
                        ActualValue = new Money(500000.00m),
                        CustomerId = new EntityReference(Account.EntityLogicalName,
                            _accountId),
                        StepName = "Open"
                    },
                    new Opportunity
                    {
                        Name = "Sample Opp 05",
                        EstimatedValue = new Money(110000.00m),
                        ActualValue = new Money(60000.00m),
                        CustomerId = new EntityReference(Account.EntityLogicalName,
                            _accountId),
                        StepName = "Open"
                    },
                    new Opportunity
                    {
                        Name = "Sample Opp 06",
                        EstimatedValue = new Money(90000.00m),
                        ActualValue = new Money(70000.00m),
                        CustomerId = new EntityReference(Account.EntityLogicalName,
                            _accountId),
                        StepName = "Open"
                    },
                    new Opportunity
                    {
                        Name = "Sample Opp 07",
                        EstimatedValue = new Money(620000.00m),
                        ActualValue = new Money(480000.00m),
                        CustomerId = new EntityReference(Account.EntityLogicalName,
                            _accountId),
                        StepName = "Open"
                    },
                    new Opportunity
                    {
                        Name = "Sample Opp 08",
                        EstimatedValue = new Money(440000.00m),
                        ActualValue = new Money(400000.00m),
                        CustomerId = new EntityReference(Account.EntityLogicalName,
                            _accountId),
                        StepName = "Open"
                    },
                    new Opportunity
                    {
                        Name = "Sample Opp 09",
                        EstimatedValue = new Money(410000.00m),
                        ActualValue = new Money(400000.00m),
                        CustomerId = new EntityReference(Account.EntityLogicalName,
                            _accountId),
                        StepName = "Open"
                    },
                    new Opportunity
                    {
                        Name = "Sample Opp 10",
                        EstimatedValue = new Money(650000.00m),
                        ActualValue = new Money(650000.00m),
                        CustomerId = new EntityReference(Account.EntityLogicalName,
                            _accountId),
                        StepName = "Open"
                    }
                };

            _opportunitiyIds = (from opp in setupOpportunities
                                select service.Create(opp)).ToArray();

            Console.WriteLine("Created few opportunity records for {0}.", setupAccount.Name);

            // Create a visualization

            // Set The presentation XML string.
            string presentationXml = @"
                    <Chart Palette='BrightPastel'>
                        <Series>
                            <Series _Template_='All' ShadowOffset='2' 
                                BorderColor='64, 64, 64' BorderDashStyle='Solid'
                                BorderWidth='1' IsValueShownAsLabel='true' 
                                Font='Tahoma, 6.75pt, GdiCharSet=0' 
                                LabelForeColor='100, 100, 100'
                                CustomProperties='FunnelLabelStyle=Outside' 
                                ChartType='Funnel'>
                                <SmartLabelStyle Enabled='True' />
                                <Points />
                            </Series>
                         </Series>
                        <ChartAreas>
                            <ChartArea _Template_='All' BackColor='Transparent'
                                BorderColor='Transparent' 
                                BorderDashStyle='Solid'>
                                <Area3DStyle Enable3D='True' 
                                    IsClustered='True'/>
                            </ChartArea>
                        </ChartAreas>
                        <Legends>
                            <Legend _Template_='All' Alignment='Center' 
                                LegendStyle='Table' Docking='Bottom' 
                                IsEquallySpacedItems='True' BackColor='White'
                                BorderColor='228, 228, 228' BorderWidth='0' 
                                Font='Tahoma, 8pt, GdiCharSet=0' 
                                ShadowColor='0, 0, 0, 0' 
                                ForeColor='100, 100, 100'>
                            </Legend>
                        </Legends>
                        <Titles>
                            <Title _Template_='All'
                                Font='Tahoma, 9pt, style=Bold, GdiCharSet=0'
                                ForeColor='102, 102, 102'>
                            </Title>
                        </Titles>
                        <BorderSkin PageColor='Control'
                            BackColor='CornflowerBlue'
                            BackSecondaryColor='CornflowerBlue' />
                    </Chart>
                    ";

            // Set the data XML string.
            string dataXml = @"
                    <datadefinition>
                        <fetchcollection>
                            <fetch mapping='logical' count='10' 
                                aggregate='true'>
                                <entity name='opportunity'>
                                    <attribute name='actualvalue_base' 
                                        aggregate='sum' 
                                        alias='sum_actualvalue_base' />
                                    <attribute name='stepname' groupby='true' 
                                        alias='stepname' />
                                    <order alias='stepname' descending='false'/>
                                </entity>
                            </fetch>
                        </fetchcollection>
                        <categorycollection>
                            <category>
                                <measurecollection>
                                    <measure alias='sum_actualvalue_base'/>
                                </measurecollection>
                            </category>
                        </categorycollection>
                    </datadefinition>
                    ";

            // Create the visualization entity instance.
            var newUserOwnedVisualization = new UserQueryVisualization
            {
                Name = "Sample User Visualization",
                Description = "Sample user-owned visualization.",
                PresentationDescription = presentationXml,
                DataDescription = dataXml,
                PrimaryEntityTypeCode = Opportunity.EntityLogicalName,
            };
            _userOwnedVisualizationId = service.Create(newUserOwnedVisualization);

            Console.WriteLine("Created {0}.", newUserOwnedVisualization.Name);

            // Create another user to whom the visualization will be assigned.
            _otherUserId = SystemUserProvider.RetrieveSalesManager(service);
        }

        /// <summary>
        /// The main logic of this program being demonstrated.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Entity name and reference collection.</param>
        /// <returns>True if successful; otherwise false.</returns>
        static public bool Run(IOrganizationService service,
            Dictionary<string, EntityReference> entityStore)
        {
            var otherUser = new EntityReference("SystemUser")
            {
                Id = GetOtherUser()
            };

            AssignResponse response = AssignChart(service, otherUser, entityStore["chart"]);
            if (response.Results.Count > 0)
            {
                Console.WriteLine("Assigned chart {0} to user {1}.",
                    entityStore["chart"].Name, entityStore["user"].Name);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get the system user ID of another existing user.
        /// </summary>
        /// <returns>Unique identifier of the user.</returns>
        private static Guid GetOtherUser()
        {
            Console.WriteLine(
                "Enter the full name of another existing user to assign the chart to.");
            var name = Console.ReadLine();

            // TODO Retrieve the ID of the other user
            return Guid.NewGuid();
        }

        /// <summary>
        /// Dispose of any data and resources created by the this program.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Entity name and reference collection.</param>
        static public void Cleanup(ServiceClient service,
            Dictionary<string, EntityReference> entityStore)
        {
            // Do some checking of the passed parameter values.
            if (service == null || service.IsReady == false)
            {
                Console.WriteLine("Cleanup(): web service connection is not available, cleanup aborted.");
                return;
            }

            if (entityStore == null)
            {
                Console.WriteLine("Cleanup(): entref store collection is null, cleanup aborted.");
                Console.WriteLine("Cleanup(): be sure to run Setup() prior to Cleanup().");
                return;
            }

            // Impersonate the other user in service calls so as to delete
            // entity records they own.
            service.CallerId = entityStore["user"].Id;

            // Collect the keys of entities to be deleted.
            var keysToDelete = new List<string>(entityStore.Keys);

            // Delete in Dataverse each entity in the entity store.
            foreach (var key in keysToDelete)
            {
                var entref = entityStore[key];
                try
                {
                    service.Delete(entref.LogicalName, entref.Id);
                    entityStore.Remove(key);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Cleanup(): exception deleting {key}\n\t{ex.Message}");
                    continue;
                }
            }

            // Output a list of entities that could not be deleted.
            if (entityStore.Count > 0)
            {
                Console.WriteLine("Cleanup(): the following entities could not be deleted:");
                foreach (var item in entityStore)
                {
                    Console.WriteLine($"Cleanup(): name={item.Key}, " +
                        $"logical name={item.Value.LogicalName}, ID={item.Value.Id}");
                }
            }
        }
    }
}
