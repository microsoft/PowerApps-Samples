using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System.Net;
using System.Security;

namespace PowerApps.Samples
{
    /// <summary>
    /// Manages authentication and initializing samples using WebAPIService
    /// </summary>
    public class App
    {
        private static readonly IConfiguration appSettings = new ConfigurationBuilder()
       //appsettings.json file 'Copy To Output Directory' property must be 'Copy if Newer'
       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
       .Build();

        //Establishes the MSAL app to manage caching access tokens
        private static IPublicClientApplication app = PublicClientApplicationBuilder.Create(appSettings["ClientId"])
            .WithRedirectUri(appSettings["RedirectUri"])
            .WithAuthority(appSettings["Authority"])
            .Build();

        /// <summary>
        /// Returns a Config to pass to the Service constructor.
        /// </summary>
        /// <returns></returns>
        public static Config InitializeApp()
        {
            //Used to configure the service
            Config config = new()
            {
                Url = appSettings["Url"],
                GetAccessToken = GetToken, //Function defined below to manage getting OAuth token

                //Optional settings that have defaults if not specified:
                MaxRetries = byte.Parse(appSettings["MaxRetries"]), //Default: 2
                TimeoutInSeconds = ushort.Parse(appSettings["TimeoutInSeconds"]), //Default: 120
                Version = appSettings["Version"], //Default 9.2
                CallerObjectId = new Guid(appSettings["CallerObjectId"]), //Default empty Guid
                DisableCookies = false
            };
            return config;



        }

        /// <summary>
        /// Returns an Access token for the app based on username and password from appsettings.json
        /// </summary>
        /// <returns>An Access token</returns>
        /// <exception cref="Exception"></exception>
        internal static async Task<string> GetToken()
        {
            List<string> scopes = new() { $"{appSettings["Url"]}/user_impersonation" };

            var accounts = await app.GetAccountsAsync();

            AuthenticationResult? result;
            if (accounts.Any())
            {
                result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                                  .ExecuteAsync();
            }
            else
            {
                // These samples use username/password for simplicity, but it is not a recommended pattern.
                // More information: 
                //https://learn.microsoft.com/azure/active-directory/develop/scenario-desktop-acquire-token?tabs=dotnet#username-and-password

                if (!string.IsNullOrWhiteSpace(appSettings["Password"]) && !string.IsNullOrWhiteSpace(appSettings["UserPrincipalName"]))
                {
                    try
                    {
                        SecureString password = new NetworkCredential("", appSettings["Password"]).SecurePassword;

                        result = await app.AcquireTokenByUsernamePassword(scopes.ToArray(), appSettings["UserPrincipalName"], password)
                            .ExecuteAsync();
                    }
                    catch (MsalUiRequiredException)
                    {
                        // You will get the following error if your UserPrincipalName or Password in appsettings.config is incorrect:
                        /*
                          Microsoft.Identity.Client.MsalClientException
                          HResult=0x80131500
                          Message=Only loopback redirect uri is supported, but app://58145b91-0c36-4500-8554-080854f2ac97/ was found. Configure http://localhost or http://localhost:port both during app registration and when you create the PublicClientApplication object. See https://aka.ms/msal-net-os-browser for details
                          Source=Microsoft.Identity.Client
                        */

                        //Open browser to enter credentials when MFA required
                        result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();

                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    throw new Exception("Need password in appsettings.json.");
                }
            }

            if (result != null && !string.IsNullOrWhiteSpace(result.AccessToken))
            {

                return result.AccessToken;
            }
            else
            {
                throw new Exception("Failed to get access token.");
            }
        }

    }
}

