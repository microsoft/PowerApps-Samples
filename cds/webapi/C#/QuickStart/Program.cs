using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using PowerApps.Samples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;


namespace QuickStart
{
    class Program
    {
        private static readonly IConfiguration appSettings = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        //Establishes the MSAL app to manage caching access tokens
        private static readonly IPublicClientApplication app = PublicClientApplicationBuilder.Create(appSettings["ClientId"])
            .WithRedirectUri(appSettings["RedirectUri"])
            .WithAuthority(appSettings["Authority"])
            .Build();


        static async Task Main(string[] args)
        {
            //Configures the service
            var config = new Config
            {
                Url = appSettings["Url"],
                GetAccessToken = GetToken, //Function defined in app (below) to manage getting OAuth token
                //Optional settings that have defaults if not specified:
                MaxRetries = byte.Parse(appSettings["MaxRetries"]), //Default: 2
                TimeoutInSeconds = ushort.Parse(appSettings["TimeoutInSeconds"]), //Default: 120
                Version = appSettings["Version"], //Default 9.1
                CallerObjectId = new Guid(appSettings["CallerObjectId"]) //Default empty Guid
            };

            var service = new Service(config);

            try
            {
                WhoAmIResponse whoAmi = await service.WhoAmI();
                Console.WriteLine($"Your UserId:{whoAmi.UserId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }



        //Passed in the Config to manage getting the token by the service
        internal static async Task<string> GetToken()
        {

            List<string> scopes = new List<string> { $"{appSettings["Url"]}/user_impersonation" };

            AuthenticationResult result = null;
            var accounts = await app.GetAccountsAsync();


            if (accounts.Any())
            {
                result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                                  .ExecuteAsync();
            }
            else
            {
                //https://docs.microsoft.com/en-us/azure/active-directory/develop/scenario-desktop-acquire-token?tabs=dotnet#username-and-password

                if (!string.IsNullOrEmpty(appSettings["Password"]) && !string.IsNullOrEmpty(appSettings["UserPrincipalName"]))
                {
                    try
                    {
                        SecureString password = new NetworkCredential("", appSettings["Password"]).SecurePassword;

                        result = await app.AcquireTokenByUsernamePassword(scopes.ToArray(), appSettings["UserPrincipalName"], password)
                            .ExecuteAsync();
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                else
                {
                    Console.WriteLine("Need password in appsettings.json.");
                }
            }

            return result.AccessToken;
        }

    }
}
