using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        public static Guid _connectionRole1Id;
        public static Guid _connectionRole2Id;
        private static bool prompt = true;

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

                    // Create the Connection Role 1
                    ConnectionRole newConnectionRole1 = new ConnectionRole
                    {
                        Name = "Example Connection Role 1",
                        Category = new OptionSetValue((int)connectionrole_category.Business),
                    };

                    _connectionRole1Id = service.Create(newConnectionRole1);
                    Console.WriteLine("Created {0}.", newConnectionRole1.Name);

                    // Create a related Connection Role Object Type Code record for Account
                    ConnectionRoleObjectTypeCode newAccountConnectionRole1TypeCode
                        = new ConnectionRoleObjectTypeCode
                        {
                            ConnectionRoleId = new EntityReference(
                                ConnectionRole.EntityLogicalName, _connectionRole1Id),
                            AssociatedObjectTypeCode = Account.EntityLogicalName
                        };

                    service.Create(newAccountConnectionRole1TypeCode);
                    Console.WriteLine(
                        "Created a related Connection Role 1 Object Type Code record for Account."
                        );

                    // Create a related Connection Role Object Type Code record for Contact
                    ConnectionRoleObjectTypeCode newContactConnectionRole1TypeCode
                        = new ConnectionRoleObjectTypeCode
                        {
                            ConnectionRoleId = new EntityReference(
                                ConnectionRole.EntityLogicalName, _connectionRole1Id),
                            AssociatedObjectTypeCode = Contact.EntityLogicalName
                        };

                    service.Create(newContactConnectionRole1TypeCode);
                    Console.WriteLine(
                        "Created a related Connection Role 1 Object Type Code record for Contact."
                        );

                    // Create the Connection Role 2
                    ConnectionRole newConnectionRole2 = new ConnectionRole
                    {
                        Name = "Example Connection Role 2",
                        Category = new OptionSetValue((int)connectionrole_category.Business),
                    };

                    _connectionRole2Id = service.Create(newConnectionRole2);
                    Console.WriteLine("Created {0}.", newConnectionRole2.Name);

                    // Create a related Connection Role 2 Object Type Code record for Account
                    ConnectionRoleObjectTypeCode newAccountConnectionRole2TypeCode
                        = new ConnectionRoleObjectTypeCode
                        {
                            ConnectionRoleId = new EntityReference(
                                ConnectionRole.EntityLogicalName, _connectionRole2Id),
                            AssociatedObjectTypeCode = Account.EntityLogicalName
                        };

                    service.Create(newAccountConnectionRole2TypeCode);
                    Console.WriteLine(
                        "Created a related Connection Role 2 Object Type Code record for Account."
                        );

                    // Create a related Connection Role 2 Object Type Code record for Contact
                    ConnectionRoleObjectTypeCode newContactConnectionRole2TypeCode
                        = new ConnectionRoleObjectTypeCode
                        {
                            ConnectionRoleId = new EntityReference(
                                ConnectionRole.EntityLogicalName, _connectionRole2Id),
                            AssociatedObjectTypeCode = Contact.EntityLogicalName
                        };

                    service.Create(newContactConnectionRole2TypeCode);
                    Console.WriteLine(
                        "Created a related Connection Role 2 Object Type Code record for Contact."
                        );
                    // Associate the connection roles
                    AssociateRequest associateConnectionRoles = new AssociateRequest
                    {
                        Target = new EntityReference(ConnectionRole.EntityLogicalName,
                           _connectionRole1Id),
                        RelatedEntities = new EntityReferenceCollection()
                        {
                            new EntityReference(ConnectionRole.EntityLogicalName,
                                _connectionRole2Id)
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
                    Console.WriteLine("Associated the connection roles.");

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
                }
                #endregion Demonstrate
                #endregion Sample Code
                else
                {
                    const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Microsoft Dataverse";
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
