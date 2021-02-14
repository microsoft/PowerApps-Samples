using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Client;
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
                    //String un = service.
                    String currentOrganizatoinUniqueName = service.ConnectedOrgFriendlyName;

                        RetrieveExchangeRateRequest request = new RetrieveExchangeRateRequest()
                    {
                        TransactionCurrencyId = _currency.Id
                    };
                    RetrieveExchangeRateResponse response =
                        (RetrieveExchangeRateResponse)service.Execute(request);
                    Console.WriteLine("  Retrieved exchange rate for created currency");

                    context = new ServiceContext(service);
                    // get the base currency for the current org
                    var baseCurrencyName =
                        (from currency in context.TransactionCurrencySet
                         join org in context.OrganizationSet
                         on currency.Id equals org.BaseCurrencyId.Id
                         where org.Name == currentOrganizatoinUniqueName
                         select currency.CurrencyName).FirstOrDefault();
                    Console.WriteLine("  This organization's base currency is {0}",
                        baseCurrencyName);

                    Console.WriteLine(
                        "  The conversion from {0} -> {1} is {2}",
                        _currency.CurrencyName,
                        baseCurrencyName,
                        response.ExchangeRate);

                   
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
