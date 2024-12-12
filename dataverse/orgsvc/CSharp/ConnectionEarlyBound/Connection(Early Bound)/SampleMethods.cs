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
        public static Guid _connectionRoleId;
        public static Guid _connectionId;
        public static Guid _accountId;
        public static Guid _contactId;
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
        /// Create a new connectionrole instance. 
        /// Create related Connection Role Object Type Code records
        /// for the account and the contact entities.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // Create a Connection Role for account and contact
            var newConnectionRole = new ConnectionRole
            {
                Name = "Example Connection Role",
                Category = new OptionSetValue((int)connectionrole_category.Business)
            };

            _connectionRoleId = service.Create(newConnectionRole);
            Console.WriteLine("Created {0}.", newConnectionRole.Name);

            // Create a related Connection Role Object Type Code record for Account
            var newAccountConnectionRoleTypeCode
                = new ConnectionRoleObjectTypeCode
                {
                    ConnectionRoleId = new EntityReference(
                        ConnectionRole.EntityLogicalName, _connectionRoleId),
                    AssociatedObjectTypeCode = Account.EntityLogicalName
                };

            service.Create(newAccountConnectionRoleTypeCode);
            Console.WriteLine(
                "Created a related Connection Role Object Type Code record for Account."
                );

            // Create a related Connection Role Object Type Code record for Contact
            var newContactConnectionRoleTypeCode
                = new ConnectionRoleObjectTypeCode
                {
                    ConnectionRoleId = new EntityReference(
                        ConnectionRole.EntityLogicalName, _connectionRoleId),
                    AssociatedObjectTypeCode = Contact.EntityLogicalName
                };

            service.Create(newContactConnectionRoleTypeCode);
            Console.WriteLine(
                "Created a related Connection Role Object Type Code record for Contact."
                );

            // Associate the connection role with itself.
            var associateConnectionRoles = new AssociateRequest
            {
                Target = new EntityReference(ConnectionRole.EntityLogicalName,
                    _connectionRoleId),
                RelatedEntities = new EntityReferenceCollection()
                        {
                            new EntityReference(ConnectionRole.EntityLogicalName,
                                _connectionRoleId)
                        },
                // The name of the relationship connection role association 
                // relationship in MS CRM.
                Relationship = new Relationship()
                {
                    PrimaryEntityRole = EntityRole.Referencing, // Referencing or Referenced based on N:1 or 1:N reflexive relationship.
                    SchemaName = "connectionroleassociation_association"
                }
            };

            service.Execute(associateConnectionRoles);
            Console.WriteLine("Associated the connection role with itself.");

            // Create an Account
            var setupAccount = new Account { Name = "Example Account" };
            _accountId = service.Create(setupAccount);
            Console.WriteLine("Created {0}.", setupAccount.Name);

            // Create a Contact
            var setupContact = new Contact { LastName = "Example Contact" };
            _contactId = service.Create(setupContact);
            Console.WriteLine("Created {0}.", setupContact.LastName);

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
                service.Delete(Connection.EntityLogicalName, _connectionId);
                service.Delete(Account.EntityLogicalName, _accountId);
                service.Delete(Contact.EntityLogicalName, _contactId);
               service.Delete(ConnectionRole.EntityLogicalName, _connectionRoleId);

                Console.WriteLine("Entity records have been deleted.");
            }
        }
    }
}
