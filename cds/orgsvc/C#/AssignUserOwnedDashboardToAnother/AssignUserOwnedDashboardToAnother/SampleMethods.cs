using Microsoft.Xrm.Sdk.Query;
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

        // Define the IDs needed for this sample.
        private static Guid _userDashboardId;
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
        /// Creates the email activity.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            //Grab the default public view for opportunities.
            var mySavedQuery = new QueryExpression
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
            UserForm dashboard = new UserForm
            {
                Name = "Sample User Dashboard",
                Description = "Sample user-owned dashboard.",
                FormXml = String.Format(@"<form>
                                <tabs>
                                    <tab name='Test Dashboard' verticallayout='true'>
                                        <labels>
                                            <label description='Sample User Dashboard' languagecode='{0}' />
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
                                                                        <label description='Top Opportunities - 1'
                                                                        languagecode='{0}' />
                                                                    </labels>
                                                                    <control id='Top10Opportunities'
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
                                                                    <control id='Top10Opportunities2'
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
                visualization.SavedQueryVisualizationId.Value.ToString("B"))
            };
            _userDashboardId = service.Create(dashboard);

            Console.WriteLine("Created {0}.", dashboard.Name);

            // Create another user to whom the dashboard will be assigned.
            _otherUserId = SystemUserProvider.RetrieveSalesManager(service);

            Console.WriteLine("Required records have been created.");
        }


        /// <summary>
        /// Deletes the custom entity record that was created for this sample.
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
                // Delete action doesn't work on the UserForm instance if it is assigned
                // to a user other than current user.
                // So as a workaround, we are impersonating the actual owner of 
                // the UserForm instance to complete the delete action.
                // To impersonate another user, set the OrganizationServiceProxy.CallerId
                // property to the ID of the other user.
                service.CallerId = _otherUserId;

                service.Delete(UserForm.EntityLogicalName, _userDashboardId);

                Console.WriteLine("Entity records have been deleted.");
            }
        }

    }
}
