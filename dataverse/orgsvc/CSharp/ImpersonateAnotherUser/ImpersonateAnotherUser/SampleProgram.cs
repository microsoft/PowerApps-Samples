using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
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
                    // Retrieve the system user ID of the user to impersonate.
                    var orgContext = new OrganizationServiceContext(service);
                    _userId = (from user in orgContext.CreateQuery<SystemUser>()
                               where user.FullName == "Kevin Cook"
                               select user.SystemUserId.Value).FirstOrDefault();

                    // To impersonate another user, set the OrganizationServiceProxy.CallerId
                    // property to the ID of the other user.
                    service.CallerId = _userId;

                    // Instantiate an account object.
                    // See the Entity Metadata topic in the SDK documentation to determine 
                    // which attributes must be set for each entity.
                    Account account = new Account { Name = "Fourth Coffee" };

                    // Create an account record named Fourth Coffee.
                    _accountId = service.Create(account);
                    Console.Write("{0} {1} created, ", account.LogicalName, account.Name);
                    //</snippetImpersonateWithOnBehalfOfPrivilege2>

                    // Retrieve the account containing several of its attributes.
                    // CreatedBy should reference the impersonated SystemUser.
                    // CreatedOnBehalfBy should reference the running SystemUser.
                    ColumnSet cols = new ColumnSet(
                        "name",
                        "createdby",
                        "createdonbehalfby",
                        "address1_postalcode",
                        "lastusedincampaign");

                    Account retrievedAccount =
                        (Account)service.Retrieve(Account.EntityLogicalName,
                            _accountId, cols);
                    Console.Write("retrieved, ");

                    // Update the postal code attribute.
                    retrievedAccount.Address1_PostalCode = "98052";

                    // The address 2 postal code was set accidentally, so set it to null.
                    retrievedAccount.Address2_PostalCode = null;

                    // Shows use of a Money value.
                    retrievedAccount.Revenue = new Money(5000000);

                    // Shows use of a boolean value.
                    retrievedAccount.CreditOnHold = false;

                    // Update the account record.
                    service.Update(retrievedAccount);
                    Console.Write("updated, ");
                    #endregion Demonstrate

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
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
