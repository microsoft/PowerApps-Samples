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
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up

                    #region Demonstrate

                    // Retrieve a user.
                    SystemUser user = service.Retrieve(SystemUser.EntityLogicalName,
                        _userId, new ColumnSet(new String[] { "systemuserid", "firstname", "lastname" })).ToEntity<SystemUser>();

                    if (user != null)
                    {
                        Console.WriteLine("{1} {0} user account is retrieved.", user.LastName, user.FirstName);

                        // Find a role.
                        var query = new QueryExpression
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
                        EntityCollection givenRoles = service.RetrieveMultiple(query);

                        if (givenRoles.Entities.Count > 0)
                        {
                            Role givenRole = givenRoles.Entities[0].ToEntity<Role>();

                            Console.WriteLine("Role {0} is retrieved.", _givenRole);

                            Console.WriteLine("Checking association between user and role.");

                            // Establish a SystemUser link for a query.
                            var systemUserLink = new LinkEntity()
                            {
                                LinkFromEntityName = SystemUserRoles.EntityLogicalName,
                                LinkFromAttributeName = "systemuserid",
                                LinkToEntityName = SystemUser.EntityLogicalName,
                                LinkToAttributeName = "systemuserid",
                                LinkCriteria =
                                {
                                    Conditions =
                                    {
                                        new ConditionExpression(
                                            "systemuserid", ConditionOperator.Equal, user.Id)
                                    }
                                }
                            };

                            // Build the query.
                            QueryExpression linkQuery = new QueryExpression()
                            {
                                EntityName = Role.EntityLogicalName,
                                ColumnSet = new ColumnSet("roleid"),
                                LinkEntities =
                                {
                                    new LinkEntity()
                                    {
                                        LinkFromEntityName = Role.EntityLogicalName,
                                        LinkFromAttributeName = "roleid",
                                        LinkToEntityName = SystemUserRoles.EntityLogicalName,
                                        LinkToAttributeName = "roleid",
                                        LinkEntities = {systemUserLink}
                                    }
                                },
                                Criteria =
                                {
                                    Conditions =
                                    {
                                        new ConditionExpression("roleid", ConditionOperator.Equal, givenRole.Id)
                                    }
                                }
                            };

                            // Retrieve matching roles.
                            EntityCollection matchEntities = service.RetrieveMultiple(linkQuery);

                            // if an entity is returned then the user is a member
                            // of the role
                            Boolean isUserInRole = (matchEntities.Entities.Count > 0);

                            if (isUserInRole)
                                Console.WriteLine("User do not belong to the role.");
                            else
                                Console.WriteLine("User belong to this role.");

                        }
                        // Disassociate the role.
                        if (givenRoles.Entities.Count > 0)
                        {
                            Role salesRole = service.RetrieveMultiple(query).Entities[0].ToEntity<Role>();

                            Console.WriteLine("Role {0} is retrieved.", _givenRole);

                            service.Disassociate(
                                        "systemuser",
                                        user.Id,
                                        new Relationship("systemuserroles_association"),
                                        new EntityReferenceCollection() { new EntityReference("role", salesRole.Id) });
                            Console.WriteLine("Role {0} is disassociated from user {1} {2}.", _givenRole, user.FirstName, user.LastName);
                        }
                    }

                    
                    #endregion Demonstrate
                }
#endregion Sample Code
                else
                {
                    const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Common Data Service";
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


