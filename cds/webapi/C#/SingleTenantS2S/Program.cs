using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;


namespace Microsoft.Dynamics365.CustomerEngagement.Samples
{
    /// <summary>
    /// Contains the D365 web service and Azure app registration information read
    /// from the app.config file in this project.
    /// </summary>
    /// <remarks>You must provide your own values for the app settings in the app.config
    /// file before running this sample.</remarks>
    class WebApiConfiguration
    {
        public string clientId;
        public string secret;
        public string tenantId;
        public string resourceUri;
        public string serviceRoot;

        public WebApiConfiguration()
        {
            var appSettings = ConfigurationManager.AppSettings;

            clientId    = appSettings["Client-ID"];
            secret      = appSettings["Client-Secret"];
            tenantId    = appSettings["Tenant-ID"];
            resourceUri = appSettings["Resource-URL"];
            serviceRoot = appSettings["Service-Root"];
        }
    }

    /// <summary>
    /// Single tenant service-to-service (S2S) sample. This sample makes use of an
    /// app registration in Azure to access a D365 server using WebAPI calls without 
    /// requiring the user's logon credentials.
    /// </summary>
    class SingleTenantS2S
    {
        static void Main(string[] args)
        {
            // Obtain the app registration and service configuration values from the app.config file.
            var webConfig = new WebApiConfiguration();

            // Send a WebAPI message request for the top 3 account names.
            var response = SendMessage(webConfig, HttpMethod.Get,
                webConfig.serviceRoot + "accounts?$select=name&$top=3").Result.Content.ReadAsStringAsync();

            Console.WriteLine(response.Result);
        }

        /// <summary>
        /// Send a message via HTTP.
        /// </summary>
        /// <param name="webConfig">A WebAPI configuration.</param>
        /// <param name="httpMethod">The HTTP method to use with the message.</param>
        /// <param name="messageUri">The URI of the WebAPI endpoint plus ODATA parameters.</param>
        /// <param name="body">The message body; otherwise, null.</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> SendMessage(WebApiConfiguration webConfig,
            HttpMethod httpMethod, string messageUri, string body = null)
        {
            // Get the access token that is required for authentication. Note that this sample does
            // not refresh the token. However, in real code you should check the token for expiration
            // and refresh when needed.
            var accessToken = await GetAccessToken(webConfig);

            // Create an HTTP message with the required WebAPI headers populated.
            var client = new HttpClient();
            var message = new HttpRequestMessage(httpMethod, messageUri);

            message.Headers.Add("OData-MaxVersion", "4.0");
            message.Headers.Add("OData-Version", "4.0");
            message.Headers.Add("Prefer", "odata.include-annotations=\"*\"");
            message.Headers.Add("Authorization", $"Bearer {accessToken}");

            // Add any body content specified in the passed parameter.   
            if (body != null)
                message.Content = new StringContent(body, UnicodeEncoding.UTF8, "application/json; odata.metadata=minimal");

            // Send the message to the WebAPI. 
            return await client.SendAsync(message);
        }

        /// <summary>
        /// Get the authentication access token.
        /// </summary>
        /// <param name="webConfig">The WebAPI configuration.</param>
        /// <returns></returns>
        public static async Task<string> GetAccessToken(WebApiConfiguration webConfig)
        {
            var credentials = new ClientCredential(webConfig.clientId, webConfig.secret);
            var authContext = new AuthenticationContext(
                "https://login.microsoftonline.com/" + webConfig.tenantId);
            var result = await authContext.AcquireTokenAsync(webConfig.resourceUri, credentials);

            return result.AccessToken;
        }
    }
}
