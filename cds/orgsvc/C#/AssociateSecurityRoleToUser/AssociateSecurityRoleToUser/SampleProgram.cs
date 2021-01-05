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

                    // Find the role.
                    QueryExpression query = new QueryExpression
                    {
                        EntityName = Role.EntityLogicalName,
                        ColumnSet = new ColumnSet("roleid"),
                        Criteria = new FilterExpression
                        {
                            Conditions =
                                {

                                    new ConditionExpression
                                    {
                                        AttributeName = "name",
                                        Operator = ConditionOperator.Equal,
                                        Values = {_givenRole}
                                    }
                                }
                        }
                    };

                    // Get the role.
                    EntityCollection roles = service.RetrieveMultiple(query);
                    if (roles.Entities.Count > 0)
                    {
                        Role salesRole = service.RetrieveMultiple(query).Entities[0].ToEntity<Role>();

                        Console.WriteLine("Role {0} is retrieved for the role assignment.", _givenRole);

                        _roleId = salesRole.Id;

                        // Associate the user with the role.
                        if (_roleId != Guid.Empty && _userId != Guid.Empty)
                        {
                            service.Associate(
                                        "systemuser",
                                        _userId,
                                        new Relationship("systemuserroles_association"),
                                        new EntityReferenceCollection() { new EntityReference(Role.EntityLogicalName, _roleId) });

                            Console.WriteLine("Role is associated with the user.");
                        }
                    }
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
