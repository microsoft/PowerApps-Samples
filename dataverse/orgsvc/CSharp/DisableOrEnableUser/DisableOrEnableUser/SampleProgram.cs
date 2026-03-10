using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        static Guid _userId;

        [STAThread] // Required to support the interactive login experience
        static void Main(string[] args)
        {
            CrmServiceClient service = null;
            try
            {
                service = SampleHelpers.Connect("Connect");
                if (service.IsReady)
                {
                    // Create any entity records that the demonstration code requires
                    SetUpSample(service);

                    #region Demonstrate
                   // Retrieve a user.
                    SystemUser user = service.Retrieve(SystemUser.EntityLogicalName,
                        _userId, new ColumnSet(new String [] {"systemuserid", "firstname", "lastname"})).ToEntity<SystemUser>();

                    if (user != null)
                    {
                        Console.WriteLine("'{1} {0}' account record was retrieved.", user.LastName, user.FirstName);
                        SetStateRequest request = new SetStateRequest()
                        {
                            EntityMoniker = user.ToEntityReference(),

                            // Sets the user to disabled.
                            State = new OptionSetValue(1),
                            // Required by request but always valued at -1 in this context.
                            Status = new OptionSetValue(-1)

                            // See DeleteRequiredRecords() to find out how to enable a user.
                        };

                        service.Execute(request);

                        Console.WriteLine("The specified system user account is now disabled.");
                    }
                    #endregion Demonstrate

                    DeleteRequiredRecords(service, true);
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
