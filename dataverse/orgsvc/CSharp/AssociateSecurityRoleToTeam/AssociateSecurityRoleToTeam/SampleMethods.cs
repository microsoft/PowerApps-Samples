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
        // Define the IDs needed for this sample.
        private static Guid _teamId;
        private static Guid _roleId;
        public const String _roleName = "An Example Role";
        private static bool prompt = true;
        /// <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        /// 
        private static void SetUpSample(CrmServiceClient service)
        {
            // Check that the current version is greater than the minimum version
            if (!SampleHelpers.CheckVersion(service, new Version("7.1.0.0")))
            {
                //The environment version is lower than version 7.1.0.0
                return;
            }

            CreateRequiredRecords(service);
        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, prompt);
        }

        /// <summary>
        /// This method creates any entity records that this sample requires.
        /// Creates the email activity.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // Retrieve the default business unit needed to create the team and role.
            var query = new QueryExpression
            {
                EntityName = BusinessUnit.EntityLogicalName,
                ColumnSet = new ColumnSet("businessunitid"),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression("parentbusinessunitid",
                            ConditionOperator.Null)
                    }
                }
            };

            var defaultBusinessUnit = service.RetrieveMultiple(query)
                .Entities.Cast<BusinessUnit>().FirstOrDefault();

            // Instantiate a team entity record and set its property values.
            // See the Entity Metadata topic in the SDK documentation  
            // to determine which attributes must be set for each entity.
            var setupTeam = new Team
            {
                Name = "An Example Team",
                BusinessUnitId = new EntityReference(BusinessUnit.EntityLogicalName,
                    defaultBusinessUnit.Id)
            };

            // Create a team record.
            _teamId = service.Create(setupTeam);
            Console.WriteLine("Created team '{0}'", setupTeam.Name);

            // Instantiate a role entity record and set its property values.
            // See the Entity Metadata topic in the SDK documentation  
            // to determine which attributes must be set for each entity.
            var setupRole = new Role
            {
                Name = _roleName,
                BusinessUnitId = new EntityReference(BusinessUnit.EntityLogicalName,
                    defaultBusinessUnit.Id)
            };

            // Create the role record. 
            _roleId = service.Create(setupRole);
            Console.WriteLine("Created role '{0}'", setupRole.Name);

            // Define an array listing the privileges that we want to add to the role
            String[] privilegesToAdd = new string[] { "prvReadContact",
                "prvCreateContact", "prvReadAccount", "prvCreateAccount" };

            // Query for the privileges we want to add to the role
            var queryPrivileges = new QueryExpression
            {
                EntityName = Privilege.EntityLogicalName,
                ColumnSet = new ColumnSet("privilegeid", "name"),
                Criteria = new FilterExpression()
            };
            queryPrivileges.Criteria.AddCondition("name", ConditionOperator.In,
                privilegesToAdd);

            DataCollection<Entity> returnedPrivileges = service.RetrieveMultiple(
                queryPrivileges).Entities;
            Console.WriteLine("Retrieved privileges to add to role");

            // Define a list to hold the RolePrivileges we'll need to add
            List<RolePrivilege> rolePrivileges = new List<RolePrivilege>();

            foreach (Privilege privilege in returnedPrivileges)
            {
                var rolePrivilege = new RolePrivilege(
                    (int)PrivilegeDepth.Local, privilege.PrivilegeId.Value);
                rolePrivileges.Add(rolePrivilege);
            }

            // Add the retrieved privileges to the example role.
            var addPrivilegesRequest = new AddPrivilegesRoleRequest
            {
                RoleId = _roleId,
                Privileges = rolePrivileges.ToArray()
            };
            service.Execute(addPrivilegesRequest);
            Console.WriteLine("Added privileges to role");
        }


        /// <summary>
        /// Deletes the custom entity record that was created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the entity created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
            bool deleteRecords = true;

            if (prompt)
            {
                Console.WriteLine("\nDo you want these entity records deleted? (y/n)");
                String answer = Console.ReadLine();

                deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y"));
            }

            if (deleteRecords)
            {

                service.Delete(Team.EntityLogicalName, _teamId);
                service.Delete(Role.EntityLogicalName, _roleId);
                Console.WriteLine("Entity records have been deleted.");
            }
        }

    }
}
