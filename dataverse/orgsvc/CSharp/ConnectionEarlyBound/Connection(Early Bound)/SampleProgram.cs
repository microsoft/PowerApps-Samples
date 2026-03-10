using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        
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

                    // Create a connection between the account and the contact.
                    // Assign a connection role to a record.
                    var newConnection = new Connection
                    {
                        Record1Id = new EntityReference(Account.EntityLogicalName,
                            _accountId),
                        Record1RoleId = new EntityReference(ConnectionRole.EntityLogicalName,
                            _connectionRoleId),
                        Record2RoleId = new EntityReference(ConnectionRole.EntityLogicalName,
                            _connectionRoleId),
                        Record2Id = new EntityReference(Contact.EntityLogicalName,
                            _contactId)
                    };

                    _connectionId = service.Create(newConnection);

                    Console.WriteLine(
                        "Created a connection between the account and the contact.");

                    
                    #region CleanUp
                    CleanUpSample(service);
                    #endregion CleanUp
                }

                else
                {
                    throw service.LastCrmException;
                }
            }
            #endregion Demonstrate
            #endregion Sample Code
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

