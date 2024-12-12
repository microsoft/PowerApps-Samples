using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MetaViz
{
    // if ADAL is selected this client will be used to download metadata
    // if browser control is used this client will NOT be used. Download is done by the browser control on frmMain.
    // source https://github.com/microsoft/PowerApps-Samples/tree/master/cds/webapi/CSharp/ADALV3WhoAmI

    internal class HttpDownloadClient
    {
        // The URL to the CDS environment you want to connect with
        // i.e. https://yourOrg.crm.dynamics.com/
        private string baseUrl;

        // Azure Active Directory registered app clientid for Microsoft samples
        private const string clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
        private const string redirectUri = "app://58145B91-0C36-4500-8554-080854F2AC97";

        private HttpClient client;

        internal HttpDownloadClient(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        internal Guid Connect(string whoamiUrl)
        {
            const string authority = "https://login.microsoftonline.com/common";
            var context = new AuthenticationContext(authority, false);

            var platformParameters = new PlatformParameters(PromptBehavior.SelectAccount);
            string accessToken = context.AcquireTokenAsync(baseUrl, clientId, new Uri(redirectUri), platformParameters, UserIdentifier.AnyUser).Result.AccessToken;

            client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.Timeout = new TimeSpan(0, 2, 0);
            client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            client.DefaultRequestHeaders.Add("OData-Version", "4.0");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Use the WhoAmI function
            var response = client.GetAsync(whoamiUrl).Result;

            //Get the response content and parse it.  
            response.EnsureSuccessStatusCode();
            JObject body = JObject
                .Parse(response.Content.ReadAsStringAsync().Result);
            Guid userId = (Guid)body["UserId"];
            return userId;
        }

        internal string Fetch(string url)
        {
            try
            {
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                throw new HttpDownloadClientException($"Failed to download {url}. Please make sure you can open this URL.", ex);
            }
        }
    }

    internal class HttpDownloadClientException : Exception
    {
        internal HttpDownloadClientException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
