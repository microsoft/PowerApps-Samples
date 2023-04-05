using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    /// <summary>
    /// Provides the D365 web service and Azure app registration information read
    /// from the App.config file in this project.
    /// </summary>
    /// <remarks>You must provide your own values for the app settings in the App.config
    /// file before running this sample.</remarks>
    class WebApiConfiguration
    {
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string TenantId { get; set; }
        public string ResourceUri { get; set; }
        public string ServiceRoot { get; set; }

        public WebApiConfiguration()
        {
            var appSettings = ConfigurationManager.AppSettings;

            ClientId = appSettings["Client-ID"];
            Secret = appSettings["Client-Secret"];
            TenantId = appSettings["Tenant-ID"];
            ResourceUri = appSettings["Resource-URL"];
            ServiceRoot = appSettings["Service-Root"];
        }
    }

    /// <summary>
    /// Single tenant service-to-service (S2S) sample. This sample makes use of an
    /// app registration in Azure to access a D365 server using Web API calls without 
    /// requiring a user's logon credentials.
    /// </summary>
    class SingleTenantS2S
    {
        static void Main(string[] args)
        {
            // Obtain the app registration and service configuration values from the App.config file.
            var webConfig = new WebApiConfiguration();

            // Send a Web API message request for the top 3 account names.
            var response = SendMessageAsync(webConfig, HttpMethod.Get,
                webConfig.ServiceRoot + "accounts?$select=name&$top=3").Result;

            // Format and then output the JSON response to the console.
            if (response.IsSuccessStatusCode)
            {
                JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                Console.WriteLine(body.ToString());
            }
            else
            {
                Console.WriteLine("The request failed with a status of '{0}'",
                       response.ReasonPhrase);
            }
            Console.ReadKey(); // Pause console until a key is pressed
        }

        /// <summary>
        /// Send a message via HTTP.
        /// </summary>
        /// <param name="webConfig">A Web API configuration.</param>
        /// <param name="httpMethod">The HTTP method to use with the message.</param>
        /// <param name="messageUri">The URI of the Web API endpoint plus ODATA parameters.</param>
        /// <param name="body">The message body; otherwise, null.</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> SendMessageAsync(WebApiConfiguration webConfig,
            HttpMethod httpMethod, string messageUri, string body = null)
        {
            // Get the access token that is required for authentication.
            var accessToken = await GetAccessToken(webConfig);

            // Create an HTTP message with the required Web API headers populated.
            var client = new HttpClient();
            var message = new HttpRequestMessage(httpMethod, messageUri);

            message.Headers.Add("OData-MaxVersion", "4.0");
            message.Headers.Add("OData-Version", "4.0");
            message.Headers.Add("Prefer", "odata.include-annotations=*");
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Add any body content specified in the passed parameter.   
            if (body != null)
                message.Content = new StringContent(body, UnicodeEncoding.UTF8, "application/json");

            // Send the message to the Web API. 
            return await client.SendAsync(message);
        }

        /// <summary>
        /// Get the authentication access token.
        /// </summary>
        /// <param name="webConfig">The Web API configuration.</param>
        /// <returns>Authentication access token</returns>
        public static async Task<string> GetAccessToken(WebApiConfiguration webConfig)
        {
            var authBuilder = ConfidentialClientApplicationBuilder.Create(webConfig.ClientId)
                .WithTenantId(webConfig.TenantId)
                .WithClientSecret(webConfig.Secret)
                .Build();

            string[] scopes = { webConfig.ResourceUri + "/.default" };

            AuthenticationResult tokenResult =
                 await authBuilder.AcquireTokenForClient(scopes).ExecuteAsync();

            return tokenResult.AccessToken;
        }
    }
}
