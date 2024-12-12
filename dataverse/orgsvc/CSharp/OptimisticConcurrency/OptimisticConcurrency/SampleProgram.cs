using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
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
                    ////////////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate
                    // Retrieve an account.
                    var account = service.Retrieve("account", accountId, new ColumnSet("name", "creditlimit"));
                    Console.WriteLine("\tThe row version of the created account is {0}", account.RowVersion);

                    if (account != null)
                    {
                        // Create an in-memory account object from the retrieved account.
                        var updatedAccount = new Entity()
                        {
                            LogicalName = account.LogicalName,
                            Id = account.Id,
                            RowVersion = account.RowVersion
                        };

                        // Update just the credit limit.
                        updatedAccount["creditlimit"] = new Money(1000000);

                        // Set the request's concurrency behavour to check for a row version match.
                        var accountUpdate = new UpdateRequest()
                        {
                            Target = updatedAccount,
                            ConcurrencyBehavior = ConcurrencyBehavior.IfRowVersionMatches
                        };

                        // Do the update.
                        UpdateResponse accountUpdateResponse = (UpdateResponse)service.Execute(accountUpdate);
                        Console.WriteLine("Account '{0}' updated with a credit limit of {1}.", account["name"],
                            ((Money)updatedAccount["creditlimit"]).Value);

                        account = service.Retrieve("account", updatedAccount.Id, new ColumnSet());
                        Console.WriteLine("\tThe row version of the updated account is {0}", account.RowVersion);
                        accountRowVersion = account.RowVersion;
                    }

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
