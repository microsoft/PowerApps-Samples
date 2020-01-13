using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {

        // Define the IDs needed for this sample.
        private static Guid _queueId;
        private static Guid _teamId;
        private static Guid _roleId;
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
            var QueueViewType = new
            {
                Public = 0,
                Private = 1
            };

            // Create a queue instance and set its property values.
            var newQueue = new Queue
            {
                Name = "Example Queue",
                Description = "This is an example queue.",
                QueueViewType = new OptionSetValue(QueueViewType.Private)
            };

            // Create a new queue and store its returned GUID in a variable for later use.
            _queueId = service.Create(newQueue);
            Console.WriteLine("Created {0}", newQueue.Name);

            // Retrieve the default business unit for the creation of the team and role. 
            var queryDefaultBusinessUnit = new QueryExpression
            {
                EntityName = BusinessUnit.EntityLogicalName,
                ColumnSet = new ColumnSet("businessunitid"),
                Criteria = new FilterExpression
                {
                    Conditions =
                        {
                            new ConditionExpression
                            {
                                AttributeName = "parentbusinessunitid",
                                Operator = ConditionOperator.Null
                            }
                        }
                }
            };

            var defaultBusinessUnit =(BusinessUnit)service.RetrieveMultiple(
                queryDefaultBusinessUnit).Entities[0];

            // Create a new example team.
            var setupTeam = new Team
            {
                Name = "Example Team",
                BusinessUnitId = new EntityReference(BusinessUnit.EntityLogicalName, defaultBusinessUnit.BusinessUnitId.Value)
            };

            _teamId = service.Create(setupTeam);
            Console.WriteLine("Created {0}", setupTeam.Name);

            // Create a new example role.
            var setupRole = new Role
            {
                Name = "Example Role",
                BusinessUnitId = new EntityReference(BusinessUnit.EntityLogicalName, defaultBusinessUnit.BusinessUnitId.Value)
            };

            _roleId = service.Create(setupRole);
            Console.WriteLine("Created {0}", setupRole.Name);

            // Retrieve the prvReadQueue and prvAppendToQueue privileges.
            var queryQueuePrivileges = new QueryExpression
            {
                EntityName = Privilege.EntityLogicalName,
                ColumnSet = new ColumnSet("privilegeid", "name"),
                Criteria = new FilterExpression
                {
                    Conditions =
                        {
                            new ConditionExpression
                            {
                                AttributeName = "name",
                                Operator = ConditionOperator.In,
                                Values = { "prvReadQueue", "prvAppendToQueue" }
                            }
                        }
                }
            };

            DataCollection<Entity> retrievedQueuePrivileges =
                service.RetrieveMultiple(queryQueuePrivileges).Entities;

            Console.WriteLine("Retrieved prvReadQueue and prvAppendToQueue privileges.");

            // Define a list to hold the RolePrivileges we'll need to add
            List<RolePrivilege> rolePrivileges = new List<RolePrivilege>();

            foreach (Privilege privilege in retrievedQueuePrivileges)
            {
                RolePrivilege rolePrivilege = new RolePrivilege(
                    (int)PrivilegeDepth.Local, privilege.PrivilegeId.Value);
                rolePrivileges.Add(rolePrivilege);
            }

            // Add the prvReadQueue and prvAppendToQueue privileges to the example role.
            var addPrivilegesRequest = new AddPrivilegesRoleRequest
            {
                RoleId = _roleId,
                Privileges = rolePrivileges.ToArray()
            };
            service.Execute(addPrivilegesRequest);
            Console.WriteLine("Retrieved privileges are added to {0}.", setupRole.Name);


            // Add the example role to the example team.
            service.Associate(
                       Team.EntityLogicalName,
                       _teamId,
                       new Relationship("teamroles_association"),
                       new EntityReferenceCollection() { new EntityReference(Role.EntityLogicalName, _roleId) });

            // It takes some time for the privileges to propogate to the team.  
            // Verify this is complete before continuing.

            bool teamLacksPrivilege = true;
            while (teamLacksPrivilege)
            {
                RetrieveTeamPrivilegesRequest retrieveTeamPrivilegesRequest =
                    new RetrieveTeamPrivilegesRequest
                    {
                        TeamId = _teamId
                    };

                RetrieveTeamPrivilegesResponse retrieveTeamPrivilegesResponse =
                    (RetrieveTeamPrivilegesResponse)service.Execute(
                    retrieveTeamPrivilegesRequest);

                if (retrieveTeamPrivilegesResponse.RolePrivileges.Any(
                    rp => rp.PrivilegeId == rolePrivileges[0].PrivilegeId) &&
                    retrieveTeamPrivilegesResponse.RolePrivileges.Any(
                    rp => rp.PrivilegeId == rolePrivileges[1].PrivilegeId))
                {
                    teamLacksPrivilege = false;
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }

            Console.WriteLine("{0} has been added to {1}",
               setupRole.Name, setupTeam.Name);
            return;
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

                service.Delete(Queue.EntityLogicalName, _queueId);
                service.Delete(Team.EntityLogicalName, _teamId);
                service.Delete(Role.EntityLogicalName, _roleId);
                Console.WriteLine("Entity records have been deleted.");
            }
        }

    }
}
