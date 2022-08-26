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

                    // Define some anonymous types to define the range 
                    // of possible connection property values.
                    var Categories = new
                    {
                        Business = 1,
                        Family = 2,
                        Social = 3,
                        Sales = 4,
                        Other = 5
                    };

                    // Create a Connection Role.
                    ConnectionRole setupConnectionRole = new ConnectionRole
                    {
                        Name = "Example Connection Role",
                        Category = new OptionSetValue(Categories.Business),
                    };

                    _connectionRoleId = service.Create(setupConnectionRole);
                    setupConnectionRole.Id = _connectionRoleId;

                    Console.WriteLine("Created {0}.", setupConnectionRole.Name);

                    // Query for all Connection Roles.
                    QueryExpression allQuery = new QueryExpression
                    {
                        EntityName = ConnectionRole.EntityLogicalName,
                        ColumnSet = new ColumnSet("connectionroleid", "name"),
                        Distinct = true,
                        LinkEntities =
                        {
                            new LinkEntity
                            {
                                LinkToEntityName =
                                ConnectionRoleObjectTypeCode.EntityLogicalName,
                                LinkToAttributeName = "connectionroleid",
                                LinkFromEntityName = ConnectionRole.EntityLogicalName,
                                LinkFromAttributeName = "connectionroleid",
                                LinkCriteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    // Set a condition to only get connection roles  
                                    // related to all entities (object type code = 0).
                                    Conditions =
                                    {
                                        new ConditionExpression
                                        {
                                             AttributeName = "associatedobjecttypecode",
                                             Operator = ConditionOperator.Equal,
                                             Values = { 0 }
                                        }
                                    }
                                }
                            }
                        }
                    };

                    EntityCollection results = service.RetrieveMultiple(allQuery);

                    // Here you could perform operations on all of 
                    // the connectionroles found by the query.

                    Console.WriteLine("Retrieved {0} unassociated connectionrole instance(s).",
                        results.Entities.Count);

                    // Query to find roles which apply only to accounts.
                    QueryExpression accountQuery = new QueryExpression
                    {
                        EntityName = ConnectionRole.EntityLogicalName,
                        ColumnSet = new ColumnSet("connectionroleid", "name"),
                        Distinct = true,
                        LinkEntities =
                        {
                            new LinkEntity
                            {
                                LinkToEntityName =
                                ConnectionRoleObjectTypeCode.EntityLogicalName,
                                LinkToAttributeName = "connectionroleid",
                                LinkFromEntityName = ConnectionRole.EntityLogicalName,
                                LinkFromAttributeName = "connectionroleid",
                                LinkCriteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    // Set a condition to only get connection roles  
                                    // related to accounts (object type code = 1).
                                    Conditions =
                                    {
                                        new ConditionExpression
                                        {
                                             AttributeName = "associatedobjecttypecode",
                                             Operator = ConditionOperator.In,
                                             Values = { Account.EntityLogicalName }
                                        }
                                    }
                                }
                            }
                        }
                    };

                    results = service.RetrieveMultiple(accountQuery);

                    Console.WriteLine("Retrieved {0} account-only connectionrole instance(s).",
                        results.Entities.Count);

                    // Create a related Connection Role Object Type Code record for 
                    // Account.
                    ConnectionRoleObjectTypeCode setupAccountConnectionRoleTypeCode
                        = new ConnectionRoleObjectTypeCode
                        {
                            ConnectionRoleId = new EntityReference(
                                ConnectionRole.EntityLogicalName, _connectionRoleId),
                            AssociatedObjectTypeCode = Account.EntityLogicalName
                        };

                    setupAccountConnectionRoleTypeCode.Id =
                        service.Create(setupAccountConnectionRoleTypeCode);

                    Console.Write("Created a related Connection Role Object Type Code");
                    Console.Write(" record for Account.");

                    // Run the query to find unassociated connectionroles again.
                    results = service.RetrieveMultiple(allQuery);

                    Console.WriteLine(@"Retrieved {0} unassociated connectionrole instance(s).",
                        results.Entities.Count);

                    // Run the account-only query again.
                    results = service.RetrieveMultiple(accountQuery);

                    Console.WriteLine("Retrieved {0} account-only connectionrole instance(s).",
                        results.Entities.Count);

                    // Remove the link from account entity.
                    service.Delete(ConnectionRoleObjectTypeCode.EntityLogicalName,
                        setupAccountConnectionRoleTypeCode.Id);

                    Console.WriteLine("Removed link from connectionrole to account entity.");

                    // Run the query to find unassociated connectionroles again.
                    results = service.RetrieveMultiple(allQuery);

                    Console.WriteLine("Retrieved {0} unassociated connectionrole instance(s).",
                        results.Entities.Count);

                    // Run the account-only query again.
                    results = service.RetrieveMultiple(accountQuery);

                    Console.WriteLine("Retrieved {0} account-only connectionrole instance(s).",
                        results.Entities.Count);

                    
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
