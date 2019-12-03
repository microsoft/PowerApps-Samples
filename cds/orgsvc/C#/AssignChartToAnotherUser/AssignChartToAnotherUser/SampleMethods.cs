using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
   public partial class SampleProgram
    {
        private static Guid _userOwnedVisualizationId;
        private static Guid _accountId;
        private static Guid[] _opportunitiyIds;
        private static Guid _otherUserId;
        private static bool prompt = true;
        /// <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        /// 
        private static void SetUpSample(CrmServiceClient service)
        {
            // Check that the current version is greater than the minimum version
            if (!SampleHelpers.CheckVersion(service, new Version("7.1.0.0")))
            {
                //The environment version is lower than version 7.1.0.0
                return;
            }

            CreateRequiredRecords(service);
        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, prompt);
        }

        /// <summary>
        /// This method creates any entity records that this sample requires.
        /// Creates the visualization.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
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
        /// Deletes the entity record that was created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the entity created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
            bool deleteRecords = true;

            if (prompt)
            {
                Console.WriteLine("\nDo you want these entity records deleted? (y/n)");
                String answer = Console.ReadLine();

                deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y"));
            }

            if (deleteRecords)
            {
                // Delete action doesn't work on the UserQueryVisualization instance if it is assigned
                // to a user other than current user.
                // So as a workaround, we are impersonating the actual owner of 
                // the UserQueryVisualization instance to complete the delete action.
                // To impersonate another user, set the OrganizationServiceProxy.CallerId
                // property to the ID of the other user.
                service.CallerId = _otherUserId;

                service.Delete(UserQueryVisualization.EntityLogicalName, _userOwnedVisualizationId);
                service.Delete(Account.EntityLogicalName, _accountId);
                Console.WriteLine("Entity records have been deleted.");
            }
        }
    }
}
