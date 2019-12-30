using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
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
                    System.String layoutXml =@"<grid name='resultset' object='3' jump='name' select='1' 
    preview='1' icon='1'>
    <row name='result' id='opportunityid'>
    <cell name='name' width='150' /> 
    <cell name='customerid' width='150' /> 
    <cell name='estimatedclosedate' width='150' /> 
    <cell name='estimatedvalue' width='150' /> 
    <cell name='closeprobability' width='150' /> 
    <cell name='opportunityratingcode' width='150' /> 
    <cell name='opportunitycustomeridcontactcontactid.emailaddress1' 
        width='150' disableSorting='1' /> 
    </row>
</grid>";

  System.String fetchXml =@"<fetch version='1.0' output-format='xml-platform' 
    mapping='logical' distinct='false'>
    <entity name='opportunity'>
    <order attribute='estimatedvalue' descending='false' /> 
    <filter type='and'>
        <condition attribute='statecode' operator='eq' 
        value='0' /> 
    </filter>
    <attribute name='name' /> 
    <attribute name='estimatedvalue' /> 
    <attribute name='estimatedclosedate' /> 
    <attribute name='customerid' /> 
    <attribute name='opportunityratingcode' /> 
    <attribute name='closeprobability' /> 
    <link-entity alias='opportunitycustomeridcontactcontactid' 
        name='contact' from='contactid' to='customerid' 
        link-type='outer' visible='false'>
        <attribute name='emailaddress1' /> 
    </link-entity>
    <attribute name='opportunityid' /> 
    </entity>
</fetch>";

                    var sq = new SavedQuery
                    {
                        Name = "A New Custom Public View",
                        Description = "A Saved Query created in code",
                        ReturnedTypeCode = "opportunity",
                        FetchXml = fetchXml,
                        LayoutXml = layoutXml,
                        QueryType = 0
                    };

                    _customViewId = service.Create(sq);
                    Console.WriteLine("A new view with the name {0} was created.", sq.Name);
                    
                    // Retrieve Views
                    
                    var mySavedQuery = new QueryExpression
                    {
                        ColumnSet = new ColumnSet("savedqueryid", "name", "querytype", "isdefault", "returnedtypecode", "isquickfindquery"),
                        EntityName = SavedQuery.EntityLogicalName,
                        Criteria = new FilterExpression
                        {
                            Conditions =
            {
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
                    var retrieveSavedQueriesRequest = new RetrieveMultipleRequest { Query = mySavedQuery };

                    var retrieveSavedQueriesResponse = (RetrieveMultipleResponse)service.Execute(retrieveSavedQueriesRequest);

                    DataCollection<Entity> savedQueries = retrieveSavedQueriesResponse.EntityCollection.Entities;

                    //Display the Retrieved views
                    foreach (Entity ent in savedQueries)
                    {
                        SavedQuery rsq = (SavedQuery)ent;
                        Console.WriteLine("{0} : {1} : {2} : {3} : {4} : {5},", rsq.SavedQueryId, rsq.Name, rsq.QueryType, rsq.IsDefault, rsq.ReturnedTypeCode, rsq.IsQuickFindQuery);
                    }
                    

                    // Deactivate a view
                    
                    System.String SavedQueryName = "Closed Opportunities in Current Fiscal Year";
                    var ClosedOpportunitiesViewQuery = new QueryExpression
                    {
                        ColumnSet = new ColumnSet("savedqueryid", "statecode", "statuscode"),
                        EntityName = SavedQuery.EntityLogicalName,
                        Criteria = new FilterExpression
                        {
                            Conditions =
                            {
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
                                },
                                                new ConditionExpression
                                {
                                    AttributeName = "name",
                                    Operator = ConditionOperator.Equal,
                                    Values = {SavedQueryName}
                                }
                            }
                        }
                    };

                    var retrieveOpportuntiesViewRequest = new RetrieveMultipleRequest { Query = ClosedOpportunitiesViewQuery };

                    var retrieveOpportuntiesViewResponse = (RetrieveMultipleResponse)service.Execute(retrieveOpportuntiesViewRequest);

                    SavedQuery OpportunityView = (SavedQuery)retrieveOpportuntiesViewResponse.EntityCollection.Entities[0];
                    _viewOriginalState = (SavedQueryState)OpportunityView.StateCode;
                    _viewOriginalStatus = OpportunityView.StatusCode;


                    var ssreq = new SetStateRequest
                    {
                        EntityMoniker = new EntityReference(SavedQuery.EntityLogicalName, (Guid)OpportunityView.SavedQueryId),
                        State = new OptionSetValue((int)SavedQueryState.Inactive),
                        Status = new OptionSetValue(2)
                    };
                    service.Execute(ssreq);
                    _deactivatedViewId = (Guid)OpportunityView.SavedQueryId;
                    ReactivateDeactivatedView(service, prompt);
                    #endregion Demonstrate

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
                }
                else
                {
                    const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Common Data Service";
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
