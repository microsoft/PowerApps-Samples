// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER  
//  EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF  
//  MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
// ====================================================================================== 
using Microsoft.Crm.Sdk.Samples.HelperCode;
using System;
using System.Net.Http;
using System.Threading.Tasks;


namespace Microsoft.Crm.Sdk.Samples
{
    /// <summary>
    /// This sample retrieves Customer Engagement instances
    /// in your Office 365 tenant.
    /// </summary>    

    class RetrieveInstances
    {
        private HttpClient httpClient;

        //TODO: Change this value if your Office 365 tenant is in a different region than North America
        private static string _serviceUrl = "https://admin.services.crm.dynamics.com";

        private void ConnectToAPI()
        {
            Console.WriteLine("Connecting to the Online Management API service...");
            
            // Discover authority for the Online Management API service
            var authority = Authentication.DiscoverAuthority(_serviceUrl);

            // Authenticate to the Online Management API service by 
            // passing in the discovered authority 
            Authentication auth = new Authentication(authority.Result.ToString());            

            // Use an HttpClient object to connect to Online Management API service.           
            httpClient = new HttpClient(auth.ClientHandler, true);

            // Define the API service base address and the max period of execute time 
            httpClient.BaseAddress = new Uri(_serviceUrl);
            httpClient.Timeout = new TimeSpan(0, 2, 0);            
        }

        public async Task RetrieveInstancesAsync()
        {
            HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/instances");
            HttpResponseMessage myResponse = await httpClient.SendAsync(myRequest);

            if (myResponse.IsSuccessStatusCode)
            {
                var result = myResponse.Content.ReadAsStringAsync().Result;
                Console.WriteLine("Your instances retrieved from Office 365 tenant: \n{0}", result);
            }
            else
            {
                Console.WriteLine("The request failed with a status of '{0}'",
                       myResponse.ReasonPhrase);
            }
        }

        static public void Main(string[] args)
        {
            RetrieveInstances app = new RetrieveInstances();
            try
            {
                // Connect to the Online Management API. 
                app.ConnectToAPI();

                // Run your request
                Task.WaitAll(Task.Run(async () => await app.RetrieveInstancesAsync()));
            }
            catch (System.Exception ex) { DisplayException(ex); }
            finally
            {
                if (app.httpClient != null)
                { app.httpClient.Dispose(); }
                Console.WriteLine("Press <Enter> to exit the program.");
                Console.ReadLine();
            }
        }

        /// <summary> Helper method to display exceptions </summary> 
        private static void DisplayException(Exception ex)
        {
            Console.WriteLine("The application terminated with an error.");
            Console.WriteLine(ex.Message);
            while (ex.InnerException != null)
            {
                Console.WriteLine("\t* {0}", ex.InnerException.Message);
                ex = ex.InnerException;
            }
        }
    }
}

