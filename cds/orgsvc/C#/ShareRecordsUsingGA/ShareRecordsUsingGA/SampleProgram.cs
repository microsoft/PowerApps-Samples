using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
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
        private static Guid _currentUserId;
        private static List<Guid> _systemUserIds;
        private static Guid _teamId;
        private static Guid _leadId;
        private static Guid _taskId;
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
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate

                    // Retrieve and display the access that the calling user has to the
                    // created lead.
                    var leadReference = new EntityReference(Lead.EntityLogicalName, _leadId);
                    var currentUserReference = new EntityReference(
                        SystemUser.EntityLogicalName, _currentUserId);
                    RetrieveAndDisplayPrincipalAccess(service,leadReference, currentUserReference,
                        "Current User");

                    // Retrieve and display the access that the first user has to the
                    // created lead.
                    var systemUser1Ref = new EntityReference(SystemUser.EntityLogicalName,
                        _systemUserIds[0]);
                    RetrieveAndDisplayPrincipalAccess(service,leadReference, systemUser1Ref,
                        "System User 1");

                    // Grant the first user read access to the created lead.
                    var grantAccessRequest1 = new GrantAccessRequest
                    {
                        PrincipalAccess = new PrincipalAccess
                        {
                            AccessMask = AccessRights.ReadAccess,
                            Principal = systemUser1Ref
                        },
                        Target = leadReference
                    };

                    Console.WriteLine("Granting {0} to {1} ({2}) on the lead...\r\n",
                        AccessRights.ReadAccess, GetEntityReferenceString(systemUser1Ref), "System User 1");
                    service.Execute(grantAccessRequest1);


                    // Retrieve and display access information for the lead.
                    RetrieveAndDisplayPrincipalAccess(service,leadReference, systemUser1Ref,
                        "System User 1");
                    RetrieveAndDisplayLeadAccess(service,leadReference);

                    // Grant the team read/write access to the lead.
                    var teamReference = new EntityReference(Team.EntityLogicalName, _teamId);
                    var grantAccessRequest = new GrantAccessRequest
                    {
                        PrincipalAccess = new PrincipalAccess
                        {
                            AccessMask = AccessRights.ReadAccess | AccessRights.WriteAccess,
                            Principal = teamReference
                        },
                        Target = leadReference
                    };

                    Console.WriteLine("Granting {0} to {1} ({2}) on the lead...\r\n",
                        AccessRights.ReadAccess | AccessRights.WriteAccess, GetEntityReferenceString(teamReference), "Team");
                    service.Execute(grantAccessRequest);

                    var systemUser2Ref = new EntityReference(SystemUser.EntityLogicalName,
                        _systemUserIds[1]);


                    // Retrieve and display access information for the lead and system user 2.
                    RetrieveAndDisplayPrincipalAccess(service,leadReference, systemUser2Ref,
                        "System User 2");
                    RetrieveAndDisplayLeadAccess(service,leadReference);


                    // Grant the first user delete access to the lead.
                    var modifyUser1AccessReq = new ModifyAccessRequest
                    {
                        PrincipalAccess = new PrincipalAccess
                        {
                            AccessMask = AccessRights.DeleteAccess,
                            Principal = systemUser1Ref
                        },
                        Target = leadReference
                    };

                    Console.WriteLine("Granting delete access to {0} on the lead...\r\n",
                        GetEntityReferenceString(systemUser1Ref));
                    service.Execute(modifyUser1AccessReq);

                    // Retrieve and display access information for the lead.
                    RetrieveAndDisplayLeadAccess(service,leadReference);


                    // Revoke access to the lead for the second user.
                    var revokeUser2AccessReq = new RevokeAccessRequest
                    {
                        Revokee = systemUser2Ref,
                        Target = leadReference
                    };

                    Console.WriteLine("Revoking access to the lead for {0}...\r\n",
                        GetEntityReferenceString(systemUser2Ref));
                    service.Execute(revokeUser2AccessReq);
                
                    // Retrieve and display access information for the lead.
                    RetrieveAndDisplayPrincipalAccess(service,leadReference, systemUser2Ref,
                        "System User 2");

                    RetrieveAndDisplayLeadAccess(service,leadReference);

                    
                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up

                    //DeleteRequiredRecords(promptforDelete);


                }
                #endregion Demonstrate

                else
                {
                    const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Dynamics CRM";
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
