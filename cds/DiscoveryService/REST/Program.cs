using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security;

namespace PowerApps.Samples
{
    class Program
    {

        //These sample application registration values are available for all online instances.
        public static string clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
        public static string redirectUrl = "http://localhost";
        static readonly string username = "yourUserName@yourOrgName.onmicrosoft.com";
        static readonly string password = "yourPassword";
        //Set the Cloud if you want to search other than Commercial.
        static readonly Cloud cloud = Cloud.Commercial;

        //Establishes the MSAL app to manage caching access tokens
        private static IPublicClientApplication app = PublicClientApplicationBuilder.Create(clientId)
            .WithRedirectUri(redirectUrl)
            .WithAuthority("https://login.microsoftonline.com/organizations")
            .Build();

        static async Task Main()
        {
           List<Instance> instances =  await GetInstances(username, password);

            if (instances.Count >= 1)
            {
                Console.WriteLine("Available Instances:");
                instances.ForEach(x =>
                {
                    Console.WriteLine($"UniqueName:{x.UniqueName} ApiUrl:{x.ApiUrl} Url:{x.Url}");
                });
            }
            else
            {
                Console.WriteLine("No instances found.");
            }
        }

        static async Task<List<Instance>> GetInstances(string username, string password)
        {

            string GlobalDiscoUrl = "https://globaldisco.crm.dynamics.com/";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetToken(username, password));
            client.Timeout = new TimeSpan(0, 2, 0);
            client.BaseAddress = new Uri(GlobalDiscoUrl);

            HttpResponseMessage response = client.GetAsync("api/discovery/v2.0/Instances", HttpCompletionOption.ResponseHeadersRead).Result;


            if (response.IsSuccessStatusCode)
            {
                //Get the response content and parse it.
                string result = response.Content.ReadAsStringAsync().Result;
                JObject body = JObject.Parse(result);
                JArray values = (JArray)body.GetValue("value");

                if (!values.HasValues)
                {
                    return new List<Instance>();
                }

                return JsonConvert.DeserializeObject<List<Instance>>(values.ToString());
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        internal static async Task<string> GetToken(string username, string password)
        {

            List<string> scopes = new() { "https://globaldisco.crm.dynamics.com//user_impersonation" };

            AuthenticationResult? result = null;
            var accounts = await app.GetAccountsAsync();


            if (accounts.Any())
            {
                result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                                  .ExecuteAsync();
            }
            else
            {
                //https://docs.microsoft.com/en-us/azure/active-directory/develop/scenario-desktop-acquire-token?tabs=dotnet#username-and-password

                try
                {
                    SecureString securePassword = new NetworkCredential("", password).SecurePassword;

                    result = await app.AcquireTokenByUsernamePassword(scopes.ToArray(), username, securePassword)
                        .ExecuteAsync();
                }
                catch (Exception)
                {
                    throw;
                }

            }

            if (result != null && !string.IsNullOrEmpty(result.AccessToken))
            {

                return result.AccessToken;
            }
            else
            {
                throw new Exception("Failed to get accesstoken.");
            }
        }
    }
}