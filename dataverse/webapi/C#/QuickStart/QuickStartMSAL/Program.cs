using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace PowerApps.Samples
{
    class Program
    {
        static void Main()
        {
            // TODO Specify the Dataverse environment name to connect with.
            string resource = "https://crmue.crm.dynamics.com";

            // Azure Active Directory app registration shared by all Power App samples.
            // For your custom apps, you will need to register them with Azure AD yourself.
            // See https://learn.microsoft.com/powerapps/developer/data-platform/walkthrough-register-app-azure-active-directory
            var clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
            var redirectUri = "app://58145B91-0C36-4500-8554-080854F2AC97";

            #region Authentication

            var authBuilder = PublicClientApplicationBuilder.Create(clientId)
                             .WithAuthority(AadAuthorityAudience.AzureAdMultipleOrgs)
                             .WithRedirectUri(redirectUri)
                             .Build();
            var scope = resource + "/user_impersonation";
            string[] scopes = { scope };

            AuthenticationResult token = 
                authBuilder.AcquireTokenInteractive(scopes).ExecuteAsync().Result;
            #endregion Authentication

            #region Client configuration

            var client = new HttpClient
            {
                // See https://learn.microsoft.com/powerapps/developer/data-platform/webapi/compose-http-requests-handle-errors#web-api-url-and-versions
                BaseAddress = new Uri(resource + "/api/data/v9.2/"),
                Timeout = new TimeSpan(0, 2, 0)    // Standard two minute timeout on web service calls.
            };

            // Default headers for each Web API call.
            // See https://learn.microsoft.com/powerapps/developer/data-platform/webapi/compose-http-requests-handle-errors#http-headers
            HttpRequestHeaders headers = client.DefaultRequestHeaders;
            headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            headers.Add("OData-MaxVersion", "4.0");
            headers.Add("OData-Version", "4.0");
            headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            #endregion Client configuration

            #region Web API call

            // Invoke the Web API 'WhoAmI' unbound function.
            // See https://learn.microsoft.com/powerapps/developer/data-platform/webapi/compose-http-requests-handle-errors
            // See https://learn.microsoft.com/powerapps/developer/data-platform/webapi/use-web-api-functions#unbound-functions
            var response = client.GetAsync("WhoAmI").Result;

            if (response.IsSuccessStatusCode)
            {
                // Parse the JSON formatted service response to obtain the user ID.  
                JObject body = JObject.Parse(
                    response.Content.ReadAsStringAsync().Result);
                Guid userId = (Guid)body["UserId"];

                Console.WriteLine("Your user ID is {0}", userId);
            }
            else
            {
                Console.WriteLine("Web API call failed");
                Console.WriteLine("Reason: " + response.ReasonPhrase);
            }
            #endregion Web API call

            // Pause program execution by waiting for a key press.
            Console.ReadKey();
        }
    }
}
