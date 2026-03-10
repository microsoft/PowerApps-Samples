using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        // Define the IDs needed for this sample.
        public static Guid _connectionRoleId;
        public static Guid _account1Id;
        public static Guid _account2Id;
        public static Guid _contactId;
        public static Guid _connection1Id;
        public static Guid _connection2Id;
        private static bool prompt = true;


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
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate

                    // This query retrieves all connections this contact is part of.
                    QueryExpression query = new QueryExpression
                    {
                        EntityName = Connection.EntityLogicalName,
                        ColumnSet = new ColumnSet("connectionid"),
                        Criteria = new FilterExpression
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions =
                        {
                            // You can safely query against only record1id or
                            // record2id - CRM will find all connections this 
                            // entity is a part of either way.
                            new ConditionExpression
                            {
                                AttributeName = "record1id",
                                Operator = ConditionOperator.Equal,
                                Values = { _contactId }
                            }
                        }
                        }
                    };
                    

                    EntityCollection results = service.RetrieveMultiple(query);
                    

                    // TODO: Here you could do a variety of tasks with the 
                    // connections retrieved, such as listing the connected entities,
                    // finding reciprocal connections, etc.

                    Console.WriteLine("Retrieved {0} connectionrole instances.", results.Entities.Count);

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
                }
                #endregion Demonstrate
                #endregion Sample Code
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
