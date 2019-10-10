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
        [STAThread]// Added to support UX
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

                    // Retrieve a role from CRM.
                    var query = new QueryExpression
                    {
                        EntityName = Role.EntityLogicalName,
                        ColumnSet = new ColumnSet("roleid"),
                        Criteria = new FilterExpression
                        {
                            Conditions =
                        {
                            // You would replace the condition below with an actual role
                            // name, or skip this query if you had a role id.
                            new ConditionExpression
                            {
                                AttributeName = "name",
                                Operator = ConditionOperator.Equal,
                                Values = {_roleName}
                            }
                        }
                        }
                    };

                    var role = service.RetrieveMultiple(query).Entities.
                        Cast<Role>().First();


                    // Add the role to the team.
                    service.Associate(
                           Team.EntityLogicalName,
                           _teamId,
                           new Relationship("teamroles_association"),
                           new EntityReferenceCollection() { new EntityReference(Role.EntityLogicalName, _roleId) });

                    Console.WriteLine("Assigned role to team");
                    //</snippetAssignSecurityRoleToTeam1>

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up

                }
                #endregion Demonstrate
                #endregion Sample code
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
        }
    }
}
