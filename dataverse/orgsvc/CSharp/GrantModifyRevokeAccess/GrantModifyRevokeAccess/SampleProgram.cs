using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
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

                    // Retrieve and display the access that the calling user has to the
                    // created lead.
                    var accountReference = new EntityReference(Account.EntityLogicalName, _accountId);
                    var currentUserReference = new EntityReference(
                        SystemUser.EntityLogicalName, _currentUserId);
                    RetrieveAndDisplayPrincipalAccess(service, accountReference, currentUserReference,
                        "Current User");

                    // Retrieve and display the access that the first user has to the
                    // created lead.
                    var systemUser1Ref = new EntityReference(SystemUser.EntityLogicalName,
                        _systemUserIds[0]);
                    RetrieveAndDisplayPrincipalAccess(service,accountReference, systemUser1Ref,
                        "System User 1");

                    // Grant the first user read access to the created lead.
                    var grantAccessRequest1 = new GrantAccessRequest
                    {
                        PrincipalAccess = new PrincipalAccess
                        {
                            AccessMask = AccessRights.ReadAccess,
                            Principal = systemUser1Ref
                        },
                        Target = accountReference
                    };

                    Console.WriteLine("Granting {0} to {1} ({2}) on the lead...\r\n",
                        AccessRights.ReadAccess, GetEntityReferenceString(service,systemUser1Ref), "System User 1");
                    service.Execute(grantAccessRequest1);


                    // Retrieve and display access information for the lead.
                    RetrieveAndDisplayPrincipalAccess(service, accountReference, systemUser1Ref,
                        "System User 1");
                    RetrieveAndDisplayAccountAccess(service,accountReference);

                    // Grant the team read/write access to the lead.
                    var teamReference = new EntityReference(Team.EntityLogicalName, _teamId);
                    var grantAccessRequest = new GrantAccessRequest
                    {
                        PrincipalAccess = new PrincipalAccess
                        {
                            AccessMask = AccessRights.ReadAccess | AccessRights.WriteAccess,
                            Principal = teamReference
                        },
                        Target = accountReference
                    };

                    Console.WriteLine("Granting {0} to {1} ({2}) on the lead...\r\n",
                        AccessRights.ReadAccess | AccessRights.WriteAccess, GetEntityReferenceString(service,teamReference), "Team");
                    service.Execute(grantAccessRequest);

                    var systemUser2Ref = new EntityReference(SystemUser.EntityLogicalName,
                        _systemUserIds[1]);


                    // Retrieve and display access information for the lead and system user 2.
                    RetrieveAndDisplayPrincipalAccess(service, accountReference, systemUser2Ref,
                        "System User 2");
                    RetrieveAndDisplayAccountAccess(service,accountReference);


                    // Grant the first user delete access to the lead.
                    var modifyUser1AccessReq = new ModifyAccessRequest
                    {
                        PrincipalAccess = new PrincipalAccess
                        {
                            AccessMask = AccessRights.DeleteAccess,
                            Principal = systemUser1Ref
                        },
                        Target = accountReference
                    };

                    Console.WriteLine("Granting delete access to {0} on the account...\r\n",
                        GetEntityReferenceString(service, systemUser1Ref));
                    service.Execute(modifyUser1AccessReq);

                    // Retrieve and display access information for the lead.
                    RetrieveAndDisplayAccountAccess(service, accountReference);


                    // Revoke access to the lead for the second user.
                    var revokeUser2AccessReq = new RevokeAccessRequest
                    {
                        Revokee = systemUser2Ref,
                        Target = accountReference
                    };

                    Console.WriteLine("Revoking access to the lead for {0}...\r\n",
                        GetEntityReferenceString(service, systemUser2Ref));
                    service.Execute(revokeUser2AccessReq);

                    // Retrieve and display access information for the lead.
                    RetrieveAndDisplayPrincipalAccess(service, accountReference, systemUser2Ref,
                        "System User 2");

                    RetrieveAndDisplayAccountAccess(service, accountReference);

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
