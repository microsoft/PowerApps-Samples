using Microsoft.Identity.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EnhancedQuickStart
{
    /// <summary>
    /// Custom HTTP message handler that uses OAuth authentication through 
    /// Microsoft Authentication Library (MSAL).
    /// </summary>
    class OAuthMessageHandler : DelegatingHandler
    {
        private AuthenticationHeaderValue authHeader;

        public OAuthMessageHandler(string serviceUrl, string clientId, string redirectUrl, string username, string password,
                HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            string apiVersion = "9.2";
            string webApiUrl = $"{serviceUrl}/api/data/v{apiVersion}/";

            var authBuilder = PublicClientApplicationBuilder.Create(clientId)
                            .WithAuthority(AadAuthorityAudience.AzureAdMultipleOrgs)
                            .WithRedirectUri(redirectUrl)
                            .Build();
            var scope = serviceUrl + "/user_impersonation";
            string[] scopes = { scope };

            // First try to get an authentication token from the cache using a hint.
            AuthenticationResult authBuilderResult=null;
            try
            {
                authBuilderResult = authBuilder.AcquireTokenSilent(scopes, username)
                   .ExecuteAsync().Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Error acquiring auth token from cache:{System.Environment.NewLine}{ex}");

                // Token cache request failed, so request a new token.
                try
                {
                    if (username != string.Empty && password != string.Empty)
                    {
                        // Request a token based on username/password credentials.
                        authBuilderResult = authBuilder.AcquireTokenByUsernamePassword(scopes, username, password)
                                    .ExecuteAsync().Result;
                    }
                    else
                    {
                        // Prompt the user for credentials and get the token.
                        authBuilderResult = authBuilder.AcquireTokenInteractive(scopes)
                                    .ExecuteAsync().Result;
                    }
                }
                catch (Exception msalex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"Error acquiring auth token with user credentials:{System.Environment.NewLine}{msalex}");
                    throw;
                }
            }

            //Note that an Azure AD access token has finite lifetime, default expiration is 60 minutes.
            authHeader = new AuthenticationHeaderValue("Bearer", authBuilderResult.AccessToken);
        }

        protected override Task<HttpResponseMessage> SendAsync(
                  HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            request.Headers.Authorization = authHeader;
            return base.SendAsync(request, cancellationToken);
        }
    }
}
