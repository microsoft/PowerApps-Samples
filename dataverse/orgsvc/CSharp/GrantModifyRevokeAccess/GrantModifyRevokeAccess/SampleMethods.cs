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
        private static Guid _currentUserId;
        private static List<Guid> _systemUserIds;
        private static Guid _teamId;
        private static Guid _accountId;
        private static Guid _taskId;
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

        private static void RetrieveAndDisplayPrincipalAccess(CrmServiceClient service, EntityReference accountReference,
            EntityReference principal, String additionalIdentifier)
        {
            var principalAccessReq = new RetrievePrincipalAccessRequest
            {
                Principal = principal,
                Target = accountReference
            };
            var principalAccessRes = (RetrievePrincipalAccessResponse)
                service.Execute(principalAccessReq);
            Console.WriteLine("Access rights of {0} ({1}) on the account: {2}\r\n",
                GetEntityReferenceString(service,principal),
                additionalIdentifier,
                principalAccessRes.AccessRights);
        }

        private static void RetrieveAndDisplayAccountAccess(CrmServiceClient service, EntityReference accountReference)
        {
            var accessRequest = new RetrieveSharedPrincipalsAndAccessRequest
            {
                Target = accountReference
            };

            // The RetrieveSharedPrincipalsAndAccessResponse returns an entity reference
            // that has a LogicalName of "user" when returning access information for a
            // "team."
            var accessResponse = (RetrieveSharedPrincipalsAndAccessResponse)
                service.Execute(accessRequest);
            Console.WriteLine("The following have the specified granted access to the account.");
            foreach (var principalAccess in accessResponse.PrincipalAccesses)
            {
                Console.WriteLine("\t{0}:\r\n\t\t{1}",
                    GetEntityReferenceString(service,principalAccess.Principal),
                    principalAccess.AccessMask);
            }
            Console.WriteLine();

        }

        private static String GetEntityReferenceString(CrmServiceClient service, EntityReference entityReference)
        {
            return String.Format("{0} with GUID {1}", entityReference.LogicalName,
                entityReference.Id);
        }
        /// <summary>
        /// Creates any entity records that this sample requires.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // Create a unique identifier for this sample for preventing name conflicts.
            var sampleIdentifier = Guid.NewGuid();

            // Retrieve/create the system users to use for the sample.
            var ldapPath = String.Empty;
            _systemUserIds = SystemUserProvider.RetrieveDelegates(
                service, ref ldapPath);

            // Retrieve the root business unit to use for creating the team for the
            // sample.
            var businessUnitQuery = new QueryExpression
            {
                EntityName = BusinessUnit.EntityLogicalName,
                ColumnSet = new ColumnSet("businessunitid"),
                Criteria = new FilterExpression()
            };

            businessUnitQuery.Criteria.AddCondition("parentbusinessunitid",
                ConditionOperator.Null);
            var businessUnitResult = service.RetrieveMultiple(businessUnitQuery);
            var businessUnit = businessUnitResult.Entities[0].ToEntity<BusinessUnit>();

            // Get the GUID of the current user.
            var who = new WhoAmIRequest();
            var whoResponse = (WhoAmIResponse)service.Execute(who);
            _currentUserId = whoResponse.UserId;

            // Create a team for use in the sample.
            var team = new Team
            {
                AdministratorId = new EntityReference(
                    "systemuser", _currentUserId),
                Name = String.Format("User Access Sample Team {0}", sampleIdentifier),
                BusinessUnitId = businessUnit.ToEntityReference()
            };
            _teamId = service.Create(team);

            // Add the second user to the newly created team.
            var addToTeamRequest = new AddMembersTeamRequest
            {
                TeamId = _teamId,
                MemberIds = new[] { _systemUserIds[1] }
            };
            service.Execute(addToTeamRequest);

            // Create a account for use in the sample.
            var account = new Account
            {
                Name = "User Access Sample Company",
                
            };
            _accountId = service.Create(account);

            // Create a task to associate to the account.
            var accountReference = new EntityReference(Account.EntityLogicalName, _accountId);
            var task = new Task
            {
                Subject = "User Access Sample Task",
                RegardingObjectId = accountReference
            };
            _taskId = service.Create(task);

            // Create a letter to associate to the account.
            var letter = new Letter
            {
                Subject = "User Access Sample Letter",
                RegardingObjectId = accountReference
            };
            service.Create(letter);
        }

        private static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
            bool toBeDeleted = true;

            if (prompt)
            {
                // Ask the user if the created entities should be deleted.
                Console.Write("\nDo you want these entity records deleted? (y/n) [y]: ");
                String answer = Console.ReadLine();
                if (answer.StartsWith("y") ||
                    answer.StartsWith("Y") ||
                    answer == String.Empty)
                {
                    toBeDeleted = true;
                }
                else
                {
                    toBeDeleted = false;
                }
            }

            if (toBeDeleted)
            {
                // Delete records created in this sample.
                service.Delete(Team.EntityLogicalName, _teamId);
                // Deleting the account will delete its associated activities.
                service.Delete(Account.EntityLogicalName, _accountId);

                Console.WriteLine("Entity record(s) have been deleted.");
            }
        }
    }
}
