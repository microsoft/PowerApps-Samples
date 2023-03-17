using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        private static ServiceContext context;
        private static TransactionCurrency _currency;
        private static bool prompt = true;
        
        /// <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        /// 
        private static void SetUpSample(CrmServiceClient service)
        {
            // Check that the current version is greater than the minimum version
            if (!SampleHelpers.CheckVersion(service, new Version("7.1.0.0")))
            {
                //The environment version is lower than version 7.1.0.0
                return;
            }
            
            CreateRequiredRecords(service);
        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, prompt);
        }

        //private static string GetOrganizations(string DiscoverServiceURL, string UserName, string Password)
        //{
        //    ClientCredentials credentials = new ClientCredentials();
        //    credentials.UserName.UserName = UserName;
        //    credentials.UserName.Password = Password;

        //    using (var discoveryProxy = new DiscoveryServiceProxy(new Uri(DiscoverServiceURL), null, credentials, null))
        //    {
        //        discoveryProxy.Authenticate();

        //        // Get all Organizations using Discovery Service

        //        RetrieveOrganizationsRequest retrieveOrganizationsRequest = new RetrieveOrganizationsRequest()
        //        {
        //            AccessType = EndpointAccessType.Default,
        //            Release = OrganizationRelease.Current
        //        };

        //        RetrieveOrganizationsResponse retrieveOrganizationsResponse =
        //        (RetrieveOrganizationsResponse)discoveryProxy.Execute(retrieveOrganizationsRequest);

        //        if (retrieveOrganizationsResponse.Details.Count > 0)
        //        {
        //            //var orgs = new List<String>();
        //            foreach (OrganizationDetail orgInfo in retrieveOrganizationsResponse.Details)
        //                //orgs.Add(orgInfo.FriendlyName);
        //                return orgInfo.FriendlyName;

        //            //return orgs;
        //        }
        //        else
        //            return null;
        //    }
        //}
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            Console.WriteLine("  Creating currency 'Canadian Dollar'");
            // Create another currency
            _currency = new TransactionCurrency()
            {
                CurrencyName = "Canadian Dollar",
                CurrencyPrecision = 2,
                ExchangeRate = (decimal)0.9755,
                ISOCurrencyCode = "CAD", // Canadian Dollar currency code
                CurrencySymbol = "$"
            };
            _currency.Id = service.Create(_currency);
        }

        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
            bool toBeDeleted = true;

            if (prompt)
            {
                // Ask the user if the created entities should be deleted.
                Console.Write("\nDo you want these entity records deleted? (y/n) [y]: ");
                String answer = Console.ReadLine();
                if (answer.StartsWith("y") ||
                    answer.StartsWith("Y") ||
                    answer == String.Empty)
                {
                    toBeDeleted = true;
                }
                else
                {
                    toBeDeleted = false;
                }
            }

            if (toBeDeleted)
            {
                service.Delete(TransactionCurrency.EntityLogicalName,
                    _currency.Id);
                Console.WriteLine("Entity records have been deleted.");
            }
        }
    }
}
