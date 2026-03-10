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

            // Create a Connection Role
            ConnectionRole setupConnectionRole = new ConnectionRole
            {
                Name = "Example Connection Role",
                Description = "This is an example one sided connection role.",
                Category = new OptionSetValue(Categories.Business),
            };

            _connectionRoleId = service.Create(setupConnectionRole);
            Console.WriteLine("Created {0}.", setupConnectionRole.Name);

            // Create a related Connection Role Object Type Code record for Account
            ConnectionRoleObjectTypeCode newAccountConnectionRoleTypeCode
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
            ConnectionRoleObjectTypeCode newContactConnectionRoleTypeCode
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

            //Create a few account records for use in the connections.
            Account setupAccount1 = new Account { Name = "Example Account 1" };
            _account1Id = service.Create(setupAccount1);
            Console.WriteLine("Created {0}.", setupAccount1.Name);

            Account setupAccount2 = new Account { Name = "Example Account 2" };
            _account2Id = service.Create(setupAccount2);
            Console.WriteLine("Created {0}.", setupAccount2.Name);

            //Creates a contact used in the connection.
            Contact setupContact = new Contact { LastName = "Example Contact" };
            _contactId = service.Create(setupContact);
            Console.WriteLine("Created {0}.", setupContact.LastName);

            // Create a new connection between Account 1 and the contact record.
            Connection newConnection1 = new Connection
            {
                Record1Id = new EntityReference(Account.EntityLogicalName,
                    _account1Id),
                Record1RoleId = new EntityReference(ConnectionRole.EntityLogicalName,
                    _connectionRoleId),
                Record2Id = new EntityReference(Contact.EntityLogicalName,
                    _contactId)
            };

            _connection1Id = service.Create(newConnection1);

            Console.WriteLine(
                    "Created a connection between the account 1 and the contact.");

            // Create a new connection between the contact and Account 2 record
            Connection newConnection2 = new Connection
            {
                Record1Id = new EntityReference(Contact.EntityLogicalName,
                    _contactId),
                Record1RoleId = new EntityReference(ConnectionRole.EntityLogicalName,
                    _connectionRoleId),
                Record2Id = new EntityReference(Account.EntityLogicalName,
                    _account2Id)
            };

            _connection2Id = service.Create(newConnection2);

            Console.WriteLine(
                    "Created a connection between the contact and the account 2.");

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
                service.Delete(Connection.EntityLogicalName, _connection1Id);
                service.Delete(Connection.EntityLogicalName, _connection2Id);
                service.Delete(Account.EntityLogicalName, _account1Id);
                service.Delete(Account.EntityLogicalName, _account2Id);
                service.Delete(Contact.EntityLogicalName, _contactId);
                service.Delete(ConnectionRole.EntityLogicalName, _connectionRoleId);

                Console.WriteLine("Entity records have been deleted.");
            }
        }
    }
}
