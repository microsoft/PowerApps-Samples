using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
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

                    // Create QueryExpression
                    QueryExpression query = new QueryExpression()
                    {
                        EntityName = Opportunity.EntityLogicalName,
                        ColumnSet = new ColumnSet("name", "accountid"),
                        Criteria =
                        {
                            Filters =
                            {
                                new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("name", ConditionOperator.Equal, "Opportunity 1")
                                    },
                                }
                            }
                        },
                    };
                    Console.WriteLine("Created QueryExpression.");
                    #endregion Create QueryExpression

                    #region Create RollupRequest
                    // Create RollupRequest
                    RollupRequest rollupRequest = new RollupRequest();
                    rollupRequest.Query = query;
                    rollupRequest.Target = new EntityReference("account", _accountId);
                    rollupRequest.RollupType = RollupType.Extended;
                    Console.WriteLine("Created RollupRequest.");
                    #endregion Create RollupRequest

                    #region Execute RollupRequest
                    // Execute RollupRequest
                    RollupResponse rollupResponse = (RollupResponse)service.Execute(rollupRequest);
                    Console.WriteLine("Executed RollupRequest.");
                    #endregion Execute RollupRequest

                    #region Show RollupResponse results
                    // Show RollupResponse results
                    Console.WriteLine("RollupResponse Results:");
                    Console.WriteLine("--------------------------------------------------");
                    Console.WriteLine("Count: " + rollupResponse.Results.Count);
                    for (int i = 0; i < rollupResponse.Results.Count; i++)
                    {
                        Console.WriteLine();
                        Console.WriteLine("LogicalName: " + rollupResponse.EntityCollection.Entities[i].LogicalName);
                        Console.WriteLine("Id: " + rollupResponse.EntityCollection.Entities[i].Id);
                    }
                    #endregion Show RollupResponse results
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
