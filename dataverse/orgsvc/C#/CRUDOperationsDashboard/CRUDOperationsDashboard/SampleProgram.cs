using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;
using System.Xml.Linq;

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

                    //Grab the default public view for opportunities.
                    QueryExpression mySavedQuery = new QueryExpression
                    {
                        ColumnSet = new ColumnSet("savedqueryid"),
                        EntityName = SavedQuery.EntityLogicalName,
                        Criteria = new FilterExpression
                        {
                            Conditions =
                        {
                            new ConditionExpression
                            {
                                AttributeName = "isdefault",
                                Operator = ConditionOperator.Equal,
                                Values = {true}
                            },
                            new ConditionExpression
                            {
                                AttributeName = "querytype",
                                Operator = ConditionOperator.Equal,
                                Values = {0}
                            },
                            new ConditionExpression
                            {
                                AttributeName = "returnedtypecode",
                                Operator = ConditionOperator.Equal,
                                Values = {Opportunity.EntityTypeCode}
                            }
                        }
                        }
                    };

                    //This query should return one and only one result.
                    SavedQuery defaultOpportunityQuery = service.RetrieveMultiple(mySavedQuery)
                        .Entities.Select(x => (SavedQuery)x).FirstOrDefault();

                    // Retrieve visualizations out of the system. 
                    // This sample assumes that you have the "Top Opportunities"
                    // visualization that is installed with Microsoft Dynamics CRM.
                    QueryByAttribute visualizationQuery = new QueryByAttribute
                    {
                        EntityName = SavedQueryVisualization.EntityLogicalName,
                        ColumnSet = new ColumnSet("savedqueryvisualizationid"),
                        Attributes = { "name" },
                        //If you do not have this visualization, you will need to change
                        //this line.
                        Values = { "Top Opportunities" }
                    };


                    SavedQueryVisualization visualization = service.RetrieveMultiple(visualizationQuery).
                        Entities.Select(x => (SavedQueryVisualization)x).FirstOrDefault();
                    //This is the language code for U.S. English. If you are running this code
                    //in a different locale, you will need to modify this value.
                    int languageCode = 1033;

                    //We set up our dashboard and specify the FormXml. Refer to the
                    //FormXml schema in the Microsoft Dynamics CRM SDK for more information.
                    SystemForm dashboard = new SystemForm
                    {
                        Name = "Sample Dashboard",
                        Description = "Sample organization-owned dashboard.",
                        FormXml = String.Format(@"<form>
                                <tabs>
                                    <tab name='Test Dashboard' verticallayout='true'>
                                        <labels>
                                            <label description='Sample Dashboard' languagecode='{0}' />
                                        </labels>
                                        <columns>
                                            <column width='100%'>
                                                <sections>
                                                    <section name='Information Section'
                                                        showlabel='false' showbar='false'
                                                        columns='111'>
                                                        <labels>
                                                            <label description='Information Section'
                                                                languagecode='{0}' />
                                                        </labels>
                                                        <rows>
                                                            <row>
                                                                <cell colspan='1' rowspan='10' 
                                                                    showlabel='false'>
                                                                    <labels>
                                                                        <label description='Top Opportunitiess - 1'
                                                                        languagecode='{0}' />
                                                                    </labels>
                                                                    <control id='TopOpportunities'
                                                                        classid='{{E7A81278-8635-4d9e-8D4D-59480B391C5B}}'>
                                                                        <parameters>
                                                                            <ViewId>{1}</ViewId>
                                                                            <IsUserView>false</IsUserView>
                                                                            <RelationshipName />
                                                                            <TargetEntityType>opportunity</TargetEntityType>
                                                                            <AutoExpand>Fixed</AutoExpand>
                                                                            <EnableQuickFind>false</EnableQuickFind>
                                                                            <EnableViewPicker>false</EnableViewPicker>
                                                                            <EnableJumpBar>false</EnableJumpBar>
                                                                            <ChartGridMode>Chart</ChartGridMode>
                                                                            <VisualizationId>{2}</VisualizationId>
                                                                            <EnableChartPicker>false</EnableChartPicker>
                                                                            <RecordsPerPage>10</RecordsPerPage>
                                                                        </parameters>
                                                                    </control>
                                                                </cell>
                                                                <cell colspan='1' rowspan='10' 
                                                                    showlabel='false'>
                                                                    <labels>
                                                                        <label description='Top Opportunities - 2'
                                                                        languagecode='{0}' />
                                                                    </labels>
                                                                    <control id='TopOpportunities2'
                                                                        classid='{{E7A81278-8635-4d9e-8D4D-59480B391C5B}}'>
                                                                        <parameters>
                                                                            <ViewId>{1}</ViewId>
                                                                            <IsUserView>false</IsUserView>
                                                                            <RelationshipName />
                                                                            <TargetEntityType>opportunity</TargetEntityType>
                                                                            <AutoExpand>Fixed</AutoExpand>
                                                                            <EnableQuickFind>false</EnableQuickFind>
                                                                            <EnableViewPicker>false</EnableViewPicker>
                                                                            <EnableJumpBar>false</EnableJumpBar>
                                                                            <ChartGridMode>Grid</ChartGridMode>
                                                                            <VisualizationId>{2}</VisualizationId>
                                                                            <EnableChartPicker>false</EnableChartPicker>
                                                                            <RecordsPerPage>10</RecordsPerPage>
                                                                        </parameters>
                                                                    </control>
                                                                </cell>
                                                            </row>
                                                            <row />
                                                            <row />
                                                            <row />
                                                            <row />
                                                            <row />
                                                            <row />
                                                            <row />
                                                            <row />
                                                            <row />
                                                        </rows>
                                                    </section>
                                                </sections>
                                            </column>
                                        </columns>
                                    </tab>
                                </tabs>
                            </form>",
                        languageCode,
                        defaultOpportunityQuery.SavedQueryId.Value.ToString("B"),
                        visualization.SavedQueryVisualizationId.Value.ToString("B")),
                        IsDefault = false
                    };
                    _dashboardId = service.Create(dashboard);
                    Console.WriteLine("Created {0}.", dashboard.Name);

                    //Now we will retrieve the dashboard.
                    SystemForm retrievedDashboard = (SystemForm)service.Retrieve(SystemForm.EntityLogicalName, _dashboardId, new ColumnSet(true));
                    Console.WriteLine("Retrieved the dashboard.");

                    // Update the retrieved dashboard. Enable the chart picker on the chart.                                       
                    var xDocument = XDocument.Parse(retrievedDashboard.FormXml);

                    var chartPicker = (from control in xDocument.Descendants("control")
                                       where control.Attribute("id").Value == "TopOpportunities"
                                       select control.Descendants("EnableChartPicker").First()
                                     ).First();
                    chartPicker.Value = "true";

                    //Now we place the updated Xml back into the dashboard, and update it.
                    retrievedDashboard.FormXml = xDocument.ToString();
                    service.Update(retrievedDashboard);

                    // Publish the dashboard changes to the solution. 
                    // This is only required for organization-owned dashboards.
                    var updateRequest = new PublishXmlRequest
                    {
                        ParameterXml = @"<dashboard>_dashboardId</dashboard>"
                    };

                    service.Execute(updateRequest);

                    Console.WriteLine("Updated the dashboard.");
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
