using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Net;
using System.Net.Http.Headers;
using System.Security;

namespace PowerApps.Samples
{
    class Program
    {
        //These sample application registration values are available for all online instances.
        static readonly string clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
        static readonly string redirectUrl = "http://localhost";

        //Establishes the MSAL app to manage caching access tokens
        private static IPublicClientApplication app = PublicClientApplicationBuilder.Create(clientId)
            .WithRedirectUri(redirectUrl)
            .WithAuthority("https://login.microsoftonline.com/organizations")
            .Build();

        static async Task Main()
        {
            string username = "yourUserName@yourOrgName.onmicrosoft.com";
            string password = "yourPassword";

            //Set the Cloud if you want to search other than Commercial.
            Cloud cloud = Cloud.Commercial;

            List<Instance> instances = await GetInstances(username, password, cloud);

            if (instances.Count.Equals(0))
            {
                Console.WriteLine("No valid environments returned for these credentials.");
                return;
            }

            Console.WriteLine("Type the number of the environments you want to use and press Enter.");

            int number = 0;

            //Display instances so they can be selected
            foreach (Instance instance in instances)
            {
                number++;

                //Get the Organization Service URL
                string apiUrl = instance.ApiUrl;
                string friendlyName = instance.FriendlyName;

                Console.WriteLine($"{number} Name: {instance.FriendlyName} URL: {apiUrl}");
            }

            string typedValue = string.Empty;

            try
            {
                //Capture the user's choice
                typedValue = Console.ReadLine();

                int selected = int.Parse(typedValue);

                if (selected <= number)
                {

                    Instance selectedInstance = instances[selected - 1];
                    Console.WriteLine($"You selected '{selectedInstance.FriendlyName}'");

                    //Use the selected instance to get the UserId
                    await ShowUserId(selectedInstance, username, password);

                }
                else
                {
                    throw new ArgumentOutOfRangeException("The selected value is not valid.");
                }
            }
            catch (ArgumentOutOfRangeException aex)
            {
                Console.WriteLine(aex.Message);
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to process value: {0}", typedValue);
            }
        }


        /// <summary>
        /// Gets the instance data for the specified user and cloud.
        /// </summary>
        /// <param name="username">The user's username</param>
        /// <param name="password">The user's password</param>
        /// <param name="cloud">The Cloud enum value that corresponds to the region.</param>
        /// <returns>List of all the instances</returns>
        /// <exception cref="Exception"></exception>
        static async Task<List<Instance>> GetInstances(string username, string password, Cloud cloud)
        {
            try
            {
                //Get the Cloud URL from the Description Attribute applied for the Cloud enum member
                //i.e. Commercial is "https://globaldisco.crm.dynamics.com"
                var type = typeof(Cloud);
                var memInfo = type.GetMember(cloud.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                string baseUrl = ((DescriptionAttribute)attributes[0]).Description;

                HttpClient client = new();
                string token = await GetToken(baseUrl, username, password);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(scheme: "Bearer", parameter: token);
                client.Timeout = new TimeSpan(0, 2, 0);
                client.BaseAddress = new Uri(baseUrl);

                HttpResponseMessage response = await client
                    .GetAsync("/api/discovery/v2.0/Instances", HttpCompletionOption.ResponseHeadersRead);

                if (response.IsSuccessStatusCode)
                {
                    //Get the response content and parse it.
                    string result = await response.Content.ReadAsStringAsync();
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
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Gets an access token using MSAL app.
        /// </summary>
        /// <param name="baseUrl">The Resource to authenticate to</param>
        /// <param name="username">The user's username</param>
        /// <param name="password">The user's password</param>
        /// <returns>An AccessToken</returns>
        /// <exception cref="Exception"></exception>
        internal static async Task<string> GetToken(string baseUrl, string username, string password)
        {
            try
            {
                List<string> scopes = new() { $"{baseUrl}//user_impersonation" };
                var accounts = await app.GetAccountsAsync();

                AuthenticationResult? result;
                if (accounts.Any())
                {
                    result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                                      .ExecuteAsync();
                }
                else
                {
                    try
                    {
                        SecureString securePassword = new NetworkCredential("", password).SecurePassword;

                        // Flow not recommended for production
                        result = await app.AcquireTokenByUsernamePassword(scopes.ToArray(), username, securePassword)
                            .ExecuteAsync();
                    }
                    catch (MsalUiRequiredException)
                    {

                        // When MFA is required
                        result = await app.AcquireTokenInteractive(scopes)
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
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Shows the user's UserId for selected instance.
        /// </summary>
        /// <param name="instance">A selected instance</param>
        /// <param name="username">The user's username</param>
        /// <param name="password">The user's password</param>
        /// <returns></returns>
        private static async Task ShowUserId(Instance instance, string username, string password)
        {
            try
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetToken(instance.ApiUrl, username, password));
                client.Timeout = new TimeSpan(0, 2, 0);
                client.BaseAddress = new Uri(instance.ApiUrl);

                HttpResponseMessage response = await client.GetAsync("/api/data/v9.2/WhoAmI", HttpCompletionOption.ResponseHeadersRead);

                if (response.IsSuccessStatusCode)
                {
                    JObject content = JObject.Parse(await response.Content.ReadAsStringAsync());
                    string userId = content["UserId"].ToString();

                    Console.WriteLine($"Your UserId for {instance.FriendlyName} is: {userId}");
                }
                else
                {
                    Console.WriteLine($"Error calling WhoAmI: StatusCode {response.StatusCode} Reason: {response.ReasonPhrase}");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}