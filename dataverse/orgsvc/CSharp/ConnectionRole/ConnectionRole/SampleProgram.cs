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

                    // Create a Connection Role for account and contact
                    var newConnectionRole = new ConnectionRole
                    {
                        Name = "Example Connection Role",
                        Category = new OptionSetValue(Categories.Business)
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
                        "Created a related Connection Role Object Type Code record for Account.");

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
                        "Created a related Connection Role Object Type Code record for Contact.");

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
                    #endregion Demonstrate
                    #endregion Sample Code
                }
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
