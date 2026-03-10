using Microsoft.Xrm.Sdk;
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
                    //////////////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate

                    // Instaniate an account object.
                    var account = new Entity("account");

                    // Set the required attributes. For account, only the name is required. 
                    // See the Entity Metadata topic in the SDK documentatio to determine 
                    // which attributes must be set for each entity.
                    account["name"] = "Fourth Coffee";

                    // Create an account record named Fourth Coffee.
                    accountId = service.Create(account);

                    Console.Write("{0} {1} created, ", account.LogicalName, account.Attributes["name"]);

                    // Create a column set to define which attributes should be retrieved.
                    var attributes = new ColumnSet(new string[] { "name", "ownerid" });

                    // Retrieve the account and its name and ownerid attributes.
                    account = service.Retrieve(account.LogicalName, accountId, attributes);
                    Console.Write("retrieved, ");

                    // Update the postal code attribute.
                    account["address1_postalcode"] = "98052";

                    // The address 2 postal code was set accidentally, so set it to null.
                    account["address2_postalcode"] = null;

                    // Shows use of Money.
                    account["revenue"] = new Money(5000000);

                    // Shows use of boolean.
                    account["creditonhold"] = false;

                    // Update the account.
                    service.Update(account);
                    Console.WriteLine("and updated.");

                    // Delete the account.
                    bool deleteRecords = true;

                    if (prompt)
                    {
                        Console.WriteLine("\nDo you want these entity records deleted? (y/n) [y]: ");
                        String answer = Console.ReadLine();

                        deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y") || answer == String.Empty);
                    }

                    if (deleteRecords)
                    {
                        service.Delete("account", accountId);

                        Console.WriteLine("Entity record(s) have been deleted.");
                    }

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
