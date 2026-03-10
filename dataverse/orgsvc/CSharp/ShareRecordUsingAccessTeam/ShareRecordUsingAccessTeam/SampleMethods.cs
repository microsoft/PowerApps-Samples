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
        // Define the IDs needed for this sample.
        private static Guid _accountId;
        private static Guid _teamId;
        private static Guid _currentUserId;
       static  String ldapPath = String.Empty;
       static Guid businessUnitId;


        // System users in the team.
        public static List<Guid> salesPersons;
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

        /// <summary>
        /// Retrieve and display entity access information.
        /// </summary>
        /// <param name="entityReference"></param>
        private static void RetrieveAndDisplayEntityAccess(CrmServiceClient service, EntityReference entityReference)
        {
            var accessRequest = new RetrieveSharedPrincipalsAndAccessRequest
            {
                Target = entityReference
            };

            // The RetrieveSharedPrincipalsAndAccessResponse returns an entity reference
            // that has a LogicalName of "user" when returning access information for a
            // team.
            var accessResponse = (RetrieveSharedPrincipalsAndAccessResponse)
                service.Execute(accessRequest);

            Console.WriteLine("The following have the specified granted access to the entity.");

            foreach (var principalAccess in accessResponse.PrincipalAccesses)
            {
                Console.WriteLine("\t{0}:\r\n\t\t{1}",
                    String.Format("{0} with GUID {1}", principalAccess.Principal.LogicalName,
                principalAccess.Principal.Id), principalAccess.AccessMask);
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Retrives and display principal access information.
        /// </summary>
        /// <param name="entityReference"></param>
        /// <param name="principal"></param>
        /// <param name="additionalIdentifier"></param>
        private static void RetrieveAndDisplayPrincipalAccess(CrmServiceClient service, EntityReference entityReference,
            EntityReference principal, String additionalIdentifier)
        {
            var principalAccessReq = new RetrievePrincipalAccessRequest
            {
                Principal = principal,
                Target = entityReference
            };
            var principalAccessRes = (RetrievePrincipalAccessResponse)
                service.Execute(principalAccessReq);

            Console.WriteLine("Access rights of {0} ({1}) on the entity: {2}\r\n",
                String.Format("{0} with GUID {1}", principal.LogicalName,
                principal.Id), additionalIdentifier,
                principalAccessRes.AccessRights);
        }
        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, prompt);
        }

        /// <summary>
        /// Create any entity records that this sample requires.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // Create an account named Fourth Coffee.
            Account testAccount = new Account
            {
                Name = "Fourth Coffee"
            };

            _accountId = service.Create(testAccount);

            Console.WriteLine("Created an account named '{0}'.", testAccount.Name);
            return;
        }

        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the records created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
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
                // Remove all the users from the team before deleting the team.
                var deleteFromTeamRequest = new RemoveMembersTeamRequest
                {
                    TeamId = _teamId,
                    MemberIds = new[] { _currentUserId, salesPersons[0], salesPersons[1] }
                };
                service.Execute(deleteFromTeamRequest);

                // Delete records created in this sample.
                service.Delete(Account.EntityLogicalName, _accountId);
                service.Delete(Team.EntityLogicalName, _teamId);

                Console.WriteLine("Entity record(s) have been deleted.");
            }
        }

    }
}
