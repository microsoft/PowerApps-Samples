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
        public static Guid _accountId;
        public static Guid _teamId;
        public static Guid _roleId;
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
        /// Create a team, an account and a role.
        /// Add read account privileges to the role.
        /// Assign the role to the team so that they can read the account.
        /// Assign the account to the team.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // Instantiate an account entity record and set its property values.
            // See the Entity Metadata topic in the SDK documentation to determine 
            // which attributes must be set for each entity.
            Account setupAccount = new Account
            {
                Name = "Example Account"
            };

            // Create the account record.
            _accountId = service.Create(setupAccount);
            Console.WriteLine("Created {0}", setupAccount.Name);

            // Retrieve the default business unit needed to create the team and role.
            var queryDefaultBusinessUnit = new QueryExpression
            {
                EntityName = BusinessUnit.EntityLogicalName,
                ColumnSet = new ColumnSet("businessunitid"),
                Criteria = new FilterExpression()
            };

            queryDefaultBusinessUnit.Criteria.AddCondition("parentbusinessunitid",
                ConditionOperator.Null);

            var defaultBusinessUnit = (BusinessUnit)service.RetrieveMultiple(
                queryDefaultBusinessUnit).Entities[0];

            // Instantiate a team entity record and set its property values.
            // See the Entity Metadata topic in the SDK documentation to determine 
            // which attributes must be set for each entity.
            var setupTeam = new Team
            {
                Name = "Example Team",
                BusinessUnitId = new EntityReference(BusinessUnit.EntityLogicalName,
                    defaultBusinessUnit.Id)
            };

            // Create a team record.
            _teamId = service.Create(setupTeam);
            Console.WriteLine("Created {0}", setupTeam.Name);

            // Instantiate a role entity record and set its property values.
            // See the Entity Metadata topic in the SDK documentation to determine 
            // which attributes must be set for each entity.
            var setupRole = new Role
            {
                Name = "Example Role",
                BusinessUnitId = new EntityReference(BusinessUnit.EntityLogicalName,
                    defaultBusinessUnit.Id)
            };

            // Create a role record. Typically you would use an existing role that has the
            // the correct privileges. For this sample we need to be sure the role has
            // at least the privilege to read account records.
            _roleId = service.Create(setupRole);
            Console.WriteLine("Created {0}", setupRole.Name);

            // Create a query expression to find the prvReadAccountPrivilege.
            var queryReadAccountPrivilege = new QueryExpression
            {
                EntityName = Privilege.EntityLogicalName,
                ColumnSet = new ColumnSet("privilegeid", "name"),
                Criteria = new FilterExpression()
            };
            queryReadAccountPrivilege.Criteria.AddCondition("name",
                ConditionOperator.Equal, "prvReadAccount");

            // Retrieve the prvReadAccount privilege.
            Entity readAccountPrivilege = service.RetrieveMultiple(
                queryReadAccountPrivilege)[0];
            Console.WriteLine("Retrieved {0}", readAccountPrivilege.Attributes["name"]);

            // Add the prvReadAccount privilege to the example roles to assure the
            // team can read accounts.
            var addPrivilegesRequest = new AddPrivilegesRoleRequest
            {
                RoleId = _roleId,
                Privileges = new[]
                {
                    // Grant prvReadAccount privilege.
                    new RolePrivilege
                    {
                        PrivilegeId = readAccountPrivilege.Id
                    }
                }
            };
            service.Execute(addPrivilegesRequest);

            Console.WriteLine("Added privilege to role");

            // Add the role to the team.
            service.Associate(
                       Team.EntityLogicalName,
                       _teamId,
                       new Relationship("teamroles_association"),
                       new EntityReferenceCollection() { new EntityReference(Role.EntityLogicalName, _roleId) });

            Console.WriteLine("Assigned team to role");

            // It takes some time for the privileges to propagate to the team. Delay the
            // application until the privilege has been assigned.
            bool teamLacksPrivilege = true;
            while (teamLacksPrivilege)
            {
                var retrieveTeamPrivilegesRequest =
                    new RetrieveTeamPrivilegesRequest
                    {
                        TeamId = _teamId
                    };

                var retrieveTeamPrivilegesResponse =
                    (RetrieveTeamPrivilegesResponse)service.Execute(
                    retrieveTeamPrivilegesRequest);

                foreach (RolePrivilege rp in
                    retrieveTeamPrivilegesResponse.RolePrivileges)
                {
                    if (rp.PrivilegeId == readAccountPrivilege.Id)
                    {
                        teamLacksPrivilege = false;
                        break;
                    }
                    else
                    {
                        System.Threading.Thread.CurrentThread.Join(500);
                    }
                }
            }

            return;
        }

        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the records created in this sample.</param>
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
                service.Delete("account", _accountId);
                service.Delete("team", _teamId);
                service.Delete("role", _roleId);


                Console.WriteLine("Entity records have been deleted.");
            }
        }

    }
}
