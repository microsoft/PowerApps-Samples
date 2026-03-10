using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        [STAThread] // Required to support the interactive login experience
        static void Main(string[] args)
        {
            CrmServiceClient service = null;
            try
            {
                service = SampleHelpers.Connect("Connect");
                if (service.IsReady)
                {
                    // Create any entity records that the demonstration code requires
                    SetUpSample(service);

                    #region Demonstrate
                   
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
                    var newOrgOwnedVisualization = new SavedQueryVisualization
                    {
                        Name = "Sample Visualization",
                        Description = "Sample organization-owned visualization.",
                        PresentationDescription = presentationXml,
                        DataDescription = dataXml,
                        PrimaryEntityTypeCode = Opportunity.EntityLogicalName,
                        IsDefault = false
                    };
                    _orgOwnedVisualizationId = service.Create(newOrgOwnedVisualization);
                    Console.WriteLine("Created {0}.", newOrgOwnedVisualization.Name);

                    // Retrieve the visualization
                    var retrievedOrgOwnedVisualization = (SavedQueryVisualization)service.Retrieve(SavedQueryVisualization.EntityLogicalName, _orgOwnedVisualizationId, new ColumnSet(true));
                    Console.WriteLine("Retrieved the visualization.");

                    // Update the retrieved visualization
                    // 1.  Update the name.
                    // 2.  Update the data description string.                    

                    string newDataXml = @"<datadefinition>
                                        <fetchcollection>
                                            <fetch mapping='logical' count='10' 
                                                aggregate='true'>
                                                <entity name='opportunity'>
                                                    <attribute name='estimatedvalue_base' 
                                                        aggregate='sum' 
                                                        alias='sum_estimatedvalue_base' />
                                                    <attribute name='name' 
                                                        groupby='true' 
                                                        alias='name' />
                                                    <order alias='name' 
                                                        descending='false'/>
                                                </entity>
                                            </fetch>
                                        </fetchcollection>
                                        <categorycollection>
                                            <category>
                                                <measurecollection>
                                                    <measure alias='sum_estimatedvalue_base'/>
                                                </measurecollection>
                                            </category>
                                        </categorycollection>
                                    </datadefinition>";

                    retrievedOrgOwnedVisualization.Name = "Updated Sample Visualization";
                    retrievedOrgOwnedVisualization.DataDescription = newDataXml;

                    service.Update(retrievedOrgOwnedVisualization);

                    // Publish the changes to the solution. This step is only required
                    // for organization-owned visualizations.
                    var updateRequest = new PublishAllXmlRequest();
                    service.Execute(updateRequest);

                    Console.WriteLine("Updated the visualization.");
                    #endregion Demonstrate

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
                }
                else
                {
                    const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Microsoft Dataverse";
                    if (service.LastCrmError.Equals(UNABLE_TO_LOGIN_ERROR))
                    {
                        Console.WriteLine("Check the connection string values in cds/App.config.");
                        throw new Exception(service.LastCrmError);
                    }
                    else
                    {
                        throw service.LastCrmException;
                    }
                }
            }
            catch (Exception ex)
            {
                SampleHelpers.HandleException(ex);
            }

            finally
            {
                if (service != null)
                    service.Dispose();

                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }

        }
    }
}
