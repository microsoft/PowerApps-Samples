using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
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
        public static Guid _primaryConnectionRoleId;
        public static Guid _reciprocalConnectionRoleId;
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
        /// Create a primary connection role instance. 
        /// Create a reciprocal connection role instance.
        /// Associate the connection roles.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // Define some anonymous types to define the range 
            // of possible connection property values.
            var Categories = new
            {
                Business = 1,
                Family = 2,
                Social = 3,
                Sales = 4,
                Other = 5
            };

            // Create the Connection Roles. 
            // Create the primary connection Role instance.
            ConnectionRole setupPrimaryConnectionRole = new ConnectionRole
            {
                Name = "Example Primary Connection Role",
                Category = new OptionSetValue(Categories.Business),
            };

            _primaryConnectionRoleId = service.Create(setupPrimaryConnectionRole);
            Console.WriteLine("Created {0}.", setupPrimaryConnectionRole.Name);

            // Create a related Connection Role Object Type Code record for Account
            // on the primary role.
            ConnectionRoleObjectTypeCode accountPrimaryConnectionRoleTypeCode
                = new ConnectionRoleObjectTypeCode
                {
                    ConnectionRoleId = new EntityReference(
                        ConnectionRole.EntityLogicalName, _primaryConnectionRoleId),
                    AssociatedObjectTypeCode = Account.EntityLogicalName
                };

            service.Create(accountPrimaryConnectionRoleTypeCode);
            Console.WriteLine(@"Created a related Connection Role Object Type Code 
                                record for Account on the primary role.");

            // Create another Connection Role.
            ConnectionRole setupReciprocalConnectionRole = new ConnectionRole
            {
                Name = "Example Reciprocal Connection Role",
                Category = new OptionSetValue(Categories.Business),
            };

            _reciprocalConnectionRoleId = service.Create(setupReciprocalConnectionRole);
            Console.WriteLine("Created {0}.", setupReciprocalConnectionRole.Name);

            // Create a related Connection Role Object Type Code record for Account
            // on the related role.
            ConnectionRoleObjectTypeCode accountReciprocalConnectionRoleTypeCode
                = new ConnectionRoleObjectTypeCode
                {
                    ConnectionRoleId = new EntityReference(
                        ConnectionRole.EntityLogicalName, _reciprocalConnectionRoleId),
                    AssociatedObjectTypeCode = Account.EntityLogicalName
                };

            service.Create(accountReciprocalConnectionRoleTypeCode);
            Console.WriteLine(@"Created a related Connection Role Object Type Code 
                                record for Account on the related role.");

            // Associate the connection roles.
            AssociateRequest associateConnectionRoles =
                new AssociateRequest
                {
                    Target = new EntityReference(ConnectionRole.EntityLogicalName,
                        _primaryConnectionRoleId),
                    RelatedEntities = new EntityReferenceCollection()
                    {
                        new EntityReference(ConnectionRole.EntityLogicalName,
                        _reciprocalConnectionRoleId)
                    },
                    // The name of the relationship connection role association 
                    // relationship in MS CRM
                    Relationship = new Relationship()
                    {
                        PrimaryEntityRole = EntityRole.Referencing, // Referencing or Referenced based on N:1 or 1:N reflexive relationship.
                        SchemaName = "connectionroleassociation_association"
                    }
                };

            service.Execute(associateConnectionRoles);

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
                service.Delete(ConnectionRole.EntityLogicalName, _primaryConnectionRoleId);
                service.Delete(ConnectionRole.EntityLogicalName, _reciprocalConnectionRoleId);

                Console.WriteLine("Entity records have been deleted.");
            }
        }
    }
}
