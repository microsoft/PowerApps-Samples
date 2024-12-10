using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using PowerApps.Samples;
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
                    ////////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate
                    #region Check if this user has prvReadPOAA
                    // Get the GUID of the current user.
                    WhoAmIRequest whoAmI = new WhoAmIRequest();
                    Guid userLoggedId =
                        ((WhoAmIResponse)service.Execute(whoAmI)).UserId;
                    Console.WriteLine("User logged: " + userLoggedId);

                    // Check if this user has prvReadPOAA.
                    RetrieveUserPrivilegesRequest userPrivilegesRequest =
                        new RetrieveUserPrivilegesRequest();
                    userPrivilegesRequest.UserId = userLoggedId;
                    RetrieveUserPrivilegesResponse userPrivilegesResponse =
                        (RetrieveUserPrivilegesResponse)service.Execute(userPrivilegesRequest);

                    // Fixed the GUID for prvReadPOAA.
                    Guid prvReadPOAA = new Guid("{68564CD5-2B2E-11DF-80A6-00137299E1C2}");

                    if (userPrivilegesResponse.RolePrivileges.Any(r => r.PrivilegeId.Equals(prvReadPOAA)))
                    {
                        Console.WriteLine("This user DOES have prvReadPOAA");
                    }
                    else
                    {
                        Console.WriteLine("This user DOESN'T have prvReadPOAA");
                    }
                    Console.WriteLine();
                    #endregion Check if this user has prvReadPOAA
                    #region Create an account record

                    // Create an account record
                    Account accountRecord = new Account();
                    accountRecord.Name = "Ane";
                    accountRecord["secret_phone"] = "123456";
                    _accountRecordId = service.Create(accountRecord);
                    Console.WriteLine("Account record created.");

                    #endregion Create an account record

                    #region Create POAA entity for field #1

                    // Create POAA entity for field #1
                    PrincipalObjectAttributeAccess poaa = new PrincipalObjectAttributeAccess
                    {
                        AttributeId = _secretHomeId,
                        ObjectId = new EntityReference
                            (Account.EntityLogicalName, _accountRecordId),
                        PrincipalId = new EntityReference
                            (SystemUser.EntityLogicalName, userLoggedId),
                        ReadAccess = true,
                        UpdateAccess = true
                    };

                    service.Create(poaa);
                    Console.WriteLine("POAA record for custom field Secret_Home created.");

                    #endregion Create POAA entity for field #1

                    #region Create POAA entity for field #2

                    // Create POAA entity for field #2
                    poaa = new PrincipalObjectAttributeAccess
                    {
                        AttributeId = _secretPhoneId,
                        ObjectId = new EntityReference
                            (Account.EntityLogicalName, _accountRecordId),
                        PrincipalId = new EntityReference
                            (SystemUser.EntityLogicalName, userLoggedId),
                        ReadAccess = true,
                        UpdateAccess = true
                    };

                    service.Create(poaa);
                    Console.WriteLine("POAA record for custom field Secret_Phone created.");

                    #endregion Create POAA entity for field #2

                    #region Retrieve User Shared Attribute Permissions
                    // Create the query for retrieve User Shared Attribute permissions.
                    QueryExpression queryPOAA =
                        new QueryExpression("principalobjectattributeaccess");
                    queryPOAA.ColumnSet = new ColumnSet
                        (new string[] { "attributeid", "readaccess", "updateaccess", "principalid" });
                    queryPOAA.Criteria.FilterOperator = LogicalOperator.And;
                    queryPOAA.Criteria.Conditions.Add
                        (new ConditionExpression("objectid", ConditionOperator.Equal, _accountRecordId));
                    queryPOAA.Criteria.Conditions.Add
                        (new ConditionExpression("principalid", ConditionOperator.EqualUserId));

                    EntityCollection responsePOAA = service.RetrieveMultiple(queryPOAA);

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
                }
                #endregion Demonstrate
                #endregion Check if this user has prvReadPOAA
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
