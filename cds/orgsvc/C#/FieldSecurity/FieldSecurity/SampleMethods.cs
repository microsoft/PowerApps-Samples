using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
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
        private static Guid _roleId;
        private static Guid _teamId;
        private static Guid _userId;
        private static Guid _profileId;
        private static Guid _identityId;
        private static Guid _messageId;
        private static Guid _identityPermissionId;
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
        /// Creates any entity records that this sample requires.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // Get the user from the Helper.
           _userId = SystemUserProvider.RetrieveMarketingManager(service);
            Console.Write("User retrieved, ");
            //_userId = ((WhoAmIResponse)service.Execute(new WhoAmIRequest())).OrganizationId;
            //Console.Write("User retrieved, ");

            // Retrieve the security role needed to assign to the user.
            QueryExpression roleQuery = new QueryExpression
            {
                EntityName = Role.EntityLogicalName,
                ColumnSet = new ColumnSet("roleid"),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression("name", ConditionOperator.Equal,
                            "Marketing Manager")
                    }
                }
            };

            Role role = (Role)service.RetrieveMultiple(roleQuery).Entities[0];
            _roleId = role.Id;

            // Retrieve the default business unit needed to create the team.
            QueryExpression queryDefaultBusinessUnit = new QueryExpression
            {
                EntityName = BusinessUnit.EntityLogicalName,
                ColumnSet = new ColumnSet("businessunitid"),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression("parentbusinessunitid", ConditionOperator.Null)
                    }
                }
            };

            // Execute the query.
            BusinessUnit defaultBusinessUnit = (BusinessUnit)service.RetrieveMultiple(
                queryDefaultBusinessUnit).Entities[0];

            // Instantiate a team entity record and set its property values.
            // See the Entity Metadata topic in the SDK documentation to determine
            // which attributes must be set for each entity.
            Team setupTeam = new Team
            {
                Name = "CDS Management Team",
                BusinessUnitId = new EntityReference(BusinessUnit.EntityLogicalName,
                    defaultBusinessUnit.Id)
            };

            // Create a team record.
            _teamId = service.Create(setupTeam);
            Console.Write("Created Team, ");
        }

        /// <summary>
        /// Deletes any entity records and files that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the records created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service,bool prompt)
        {
            bool deleteRecords = true;

            if (prompt)
            {
                Console.WriteLine("\nDo you want these entity records deleted? (y/n) [y]: ");
                String answer = Console.ReadLine();

                deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y") || answer == String.Empty);
            }

            if (deleteRecords)
            {
                DeleteEntityRequest del = new DeleteEntityRequest()
                {
                    LogicalName = "new_message",
                };
                service.Execute(del);
                service.Delete(FieldSecurityProfile.EntityLogicalName, _profileId);
                service.Delete(Team.EntityLogicalName, _teamId);
                Console.WriteLine("Entity records have been deleted.");
            }
        }
    }

}
