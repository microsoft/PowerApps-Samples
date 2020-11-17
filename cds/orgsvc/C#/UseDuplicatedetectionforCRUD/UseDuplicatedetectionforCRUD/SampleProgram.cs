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
                    //////////////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate

                    // Create and account record with the named Proseware, Inc. and already existing Account Number.
                    var account = new Account
                    {
                        Name = "Proseware, Inc.",
                        AccountNumber = "ACC005"
                    };

                    // Create operation by suppressing duplicate detection
                    var reqCreate = new CreateRequest();
                    reqCreate.Target = account;
                    reqCreate.Parameters.Add("SuppressDuplicateDetection", true); // Change to false to activate the duplicate detection.
                    CreateResponse createResponse = (CreateResponse)service.Execute(reqCreate);
                    dupAccountId = createResponse.id;
                    Console.Write("Account: {0} {1} created with SuppressDuplicateDetection to true, ",
                        account.Name, account.AccountNumber);

                    // Retrieve the account containing with its few attributes.
                    ColumnSet cols = new ColumnSet(
                        new String[] { "name", "accountnumber" });

                    var retrievedAccount = (Account)service.Retrieve("account", dupAccountId, cols);
                    Console.Write("retrieved, ");

                    // Update the existing account with new account number.
                    retrievedAccount.AccountNumber = "ACC006";

                    // Update operation – update record, if a duplicate is not found.
                    UpdateRequest reqUpdate = new UpdateRequest();
                    reqUpdate.Target = retrievedAccount;
                    reqUpdate["SuppressDuplicateDetection"] = false; // Duplicate detection is activated.

                    // Update the account record.
                    var updateResponse = (UpdateResponse)service.Execute(reqUpdate);
                    Console.WriteLine("and updated.");
                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
                }
                #endregion Demonstrate
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
