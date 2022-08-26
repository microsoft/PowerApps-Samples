using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        [STAThread] // Added to support UX
        static void Main(string[] args)
        {
            CrmServiceClient service = null;
            try
            {
                service = SampleHelpers.Connect("Connect");
                if (service.IsReady)
                {
                    #region Sample Code
                    //////////////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate

                    // This query retrieves all connection roles that have this role
                    // listed as a reciprocal role.
                    QueryExpression query = new QueryExpression
                    {
                        EntityName = ConnectionRole.EntityLogicalName,
                        ColumnSet = new ColumnSet("connectionroleid"),
                        LinkEntities =
                    {
                        new LinkEntity
                        {
                            JoinOperator = JoinOperator.Inner,
                            LinkFromEntityName =  ConnectionRole.EntityLogicalName,
                            LinkFromAttributeName = "connectionroleid",
                            LinkToEntityName = "connectionroleassociation",
                            LinkToAttributeName = "connectionroleid",
                            LinkCriteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                {
                                    new ConditionExpression
                                    {
                                        AttributeName = "associatedconnectionroleid",
                                        Operator = ConditionOperator.Equal,
                                        Values = { _reciprocalConnectionRoleId }
                                    }
                                }
                            }
                        }
                    }
                    };

                    EntityCollection results = service.RetrieveMultiple(query);

                    // TODO: Here you would perform some operation on the retrieved
                    // roles. 

                    Console.WriteLine("Retrieved {0} connectionrole instance.", results.Entities.Count);

                    
                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up

                }
                #endregion Demonstrate
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
            #endregion Sample Code
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
