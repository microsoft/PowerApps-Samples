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

                    // Instantiate an account object.
                    // See the Entity Metadata topic in the SDK documentation to determine 
                    // which attributes must be set for each entity.
                    var account = new Account { Name = "Fourth Coffee" };

                    // Create an account record named Fourth Coffee.
                    accountId = service.Create(account);
                    Console.Write("{0} {1} created, ", account.LogicalName, account.Name);

                    // Retrieve the account containing several of its attributes.
                    var cols = new ColumnSet(
                        new String[] { "name", "address1_postalcode", "lastusedincampaign", "versionnumber" });

                    Account retrievedAccount = (Account)service.Retrieve("account", accountId, cols);
                    Console.Write("retrieved ");

                    // Retrieve version number of the account. Shows BigInt attribute usage.
                    long? versionNumber = retrievedAccount.VersionNumber;

                    if (versionNumber != null)
                        Console.WriteLine("version # {0}, ", versionNumber);

                    // Update the postal code attribute.
                    retrievedAccount.Address1_PostalCode = "98052";

                    // The address 2 postal code was set accidentally, so set it to null.
                    retrievedAccount.Address2_PostalCode = null;

                    // Shows usage of option set (picklist) enumerations defined in OptionSets.cs.
                    retrievedAccount.Address1_AddressTypeCode = new OptionSetValue((int)AccountAddress1_AddressTypeCode.Primary);
                    retrievedAccount.Address1_ShippingMethodCode = new OptionSetValue((int)AccountAddress1_ShippingMethodCode.DHL);
                    retrievedAccount.IndustryCode = new OptionSetValue((int)AccountIndustryCode.AgricultureandNonpetrolNaturalResourceExtraction);

                    // Shows use of a Money value.
                    retrievedAccount.Revenue = new Money(5000000);

                    // Shows use of a Boolean value.
                    retrievedAccount.CreditOnHold = false;

                    // Shows use of EntityReference.
                    retrievedAccount.ParentAccountId = new EntityReference(Account.EntityLogicalName, parentAccountId);

                    // Shows use of Memo attribute.
                    retrievedAccount.Description = "Account for Fourth Coffee.";

                    // Update the account record.
                    service.Update(retrievedAccount);
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
