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

                    // Retrieve the sales people that will be added to the team.
                    salesPersons = SystemUserProvider.RetrieveSalespersons(service, ref ldapPath);

                    // Get the ID's of the current user and business unit.
                    var who = new WhoAmIRequest();
                    var whoResponse = (WhoAmIResponse)service.Execute(who);
                    _currentUserId = whoResponse.UserId;
                    businessUnitId = whoResponse.BusinessUnitId;

                    // Create a access team.
                    var team = new Team
                    {
                        AdministratorId = new EntityReference(
                            "systemuser", _currentUserId),
                        Name = "UserAccess Test Team",
                        BusinessUnitId = new EntityReference(
                            "businessunit", businessUnitId),
                        TeamType = new OptionSetValue((int)TeamTeamType.Access),
                    };

                    _teamId = service.Create(team);
                    Console.WriteLine("Created an access team named '{0}'.", team.Name);

                    // Add two sales people to the access team.
                    var addToTeamRequest = new AddMembersTeamRequest
                    {
                        TeamId = _teamId,
                        MemberIds = new[] { salesPersons[0], salesPersons[1] }
                    };
                    service.Execute(addToTeamRequest);
                    Console.WriteLine("Added two sales people to the team.");

                    // Grant the team read/write access to an account.
                    var accountReference = new EntityReference(Account.EntityLogicalName, _accountId);
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
                    service.Execute(grantAccessRequest);
                    Console.WriteLine("Granted read/write access on the account record to the team.");

                    // Retrieve and display access information for the account.
                    RetrieveAndDisplayEntityAccess(service, accountReference);

                    // Display the account access for the team and its members.
                    var currentUserReference = new EntityReference(
                        SystemUser.EntityLogicalName, _currentUserId);
                    RetrieveAndDisplayPrincipalAccess(service, accountReference, currentUserReference,
                        "Current User");
                    var firstSalesPersonReference = new EntityReference(
                        SystemUser.EntityLogicalName, salesPersons[0]);
                    RetrieveAndDisplayPrincipalAccess(service, accountReference, firstSalesPersonReference,
                        "Sales Person");
                    var secondSalesPersonReference = new EntityReference(
                        SystemUser.EntityLogicalName, salesPersons[1]);
                    RetrieveAndDisplayPrincipalAccess(service, accountReference, secondSalesPersonReference,
                        "Sales Person");

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
