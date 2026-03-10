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
                    System.String layoutXml =
@"<grid name='resultset' jump='name' select='1' preview='1' icon='1' object='1'>
  <row name='result' id='accountid'>
   <cell name='name' width='150' />
      <cell name='telephone1' width='100' />
        </row>
         </grid>";

                    System.String fetchXml =
                    @"<fetch version='1.0' output-format='xml-platform' 
    mapping='logical' distinct='false'>
    <entity name='account'>
    <attribute name='name' /> 
    <attribute name='address1_city' /> 
    <attribute name='primarycontactid' /> 
    <attribute name='telephone1' /> 
    <order attribute='name' descending='false'/> 
    <link-entity name='contact' from='contactid' to='primarycontactid' 
        link-type='outer' visible='false' alias='accountprimarycontactidcontactcontactid'>
        <attribute name='emailaddress1' /> 
    </link-entity>
    <attribute name='accountid' /> 
<attribute name='accountnumber' /> 
    </entity>
</fetch>";

                    var sq = new SavedQuery
                    {
                        Name = "Custom Public View",
                        Description = "A Saved Query created in code",
                        ReturnedTypeCode = "account",
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
                    Values = {Account.EntityTypeCode}
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
                    
                    System.String SavedQueryName = "Custom Public View";
                    var ClosedAccountsViewQuery = new QueryExpression
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
                                    Values = {Account.EntityTypeCode}
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

                    var retrieveAccountsViewRequest = new RetrieveMultipleRequest { Query = ClosedAccountsViewQuery };
                    var retrieveAccountsViewResponse = (RetrieveMultipleResponse)service.Execute(retrieveAccountsViewRequest);
                    SavedQuery AccountView = (SavedQuery)retrieveAccountsViewResponse.EntityCollection.Entities[0];
                    
                    var ssreq = new SetStateRequest
                    {
                        EntityMoniker = new EntityReference(SavedQuery.EntityLogicalName, (Guid)AccountView.SavedQueryId),
                        State = new OptionSetValue((int)SavedQueryState.Inactive),
                        Status = new OptionSetValue(2)
                    };
                    service.Execute(ssreq);
                    _deactivatedViewId = (Guid)AccountView.SavedQueryId;
                    ReactivateDeactivatedView(service, prompt);
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
