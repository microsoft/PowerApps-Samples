using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public class SampleHelpers
    {
        //These sample application registration values are available for all online instances.
        public static string clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
        public static string redirectUrl = "app://58145B91-0C36-4500-8554-080854F2AC97";
       

        public static string GetParameterValueFromConnectionString(string connectionString, string parameter)
        {
            try
            {
                return connectionString.Split(';').Where(s => s.Trim().StartsWith(parameter)).FirstOrDefault().Split('=')[1];
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        public static HttpClient GetHttpClient(string connectionString, string clientId, string redirectUrl, string version)
        {
            string url = GetParameterValueFromConnectionString(connectionString, "Url");
            string username = GetParameterValueFromConnectionString(connectionString, "Username");
            string domain = GetParameterValueFromConnectionString(connectionString, "Domain");
            string password = GetParameterValueFromConnectionString(connectionString, "Password");
            string authType = GetParameterValueFromConnectionString(connectionString, "AuthType");
            try
            {
                HttpMessageHandler messageHandler;

                switch (authType)
                {
                    case "Office365":
                    case "IFD":
                    case "OAuth":

                        messageHandler = new OAuthMessageHandler(url, clientId, redirectUrl, username, password,
                                 new HttpClientHandler());
                        break;
                    case "AD":
                        NetworkCredential credentials = new NetworkCredential(username, password, domain);
                        messageHandler = new HttpClientHandler() { Credentials = credentials };
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Valid authType values are 'Office365', 'IFD', or 'AD'.");

                }

                HttpClient httpClient = new HttpClient(messageHandler)
                {
                    BaseAddress = new Uri(string.Format("{0}/api/data/{1}/", url, version)),

                    Timeout = new TimeSpan(0, 2, 0)  //2 minutes
                };

                return httpClient;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary> Displays exception information to the console. </summary>
        /// <param name="ex">The exception to output</param>
        public static void DisplayException(Exception ex)
        {
            Console.WriteLine("The application terminated with an error.");
            Console.WriteLine(ex.Message);
            while (ex.InnerException != null)
            {
                Console.WriteLine("\t* {0}", ex.InnerException.Message);
                ex = ex.InnerException;
            }
        }
    }

    /// <summary>
    ///Custom HTTP message handler that uses OAuth authentication thru ADAL.
    /// </summary>
    class OAuthMessageHandler : DelegatingHandler
    {
        private AuthenticationHeaderValue authHeader;

        public OAuthMessageHandler(string serviceUrl, string clientId, string redirectUrl, string username, string password,
                HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            // Obtain the Azure Active Directory Authentication Library (ADAL) authentication context.
            AuthenticationParameters ap = AuthenticationParameters.CreateFromResourceUrlAsync(
                    new Uri(serviceUrl + "/api/data/")).Result;
            AuthenticationContext authContext = new AuthenticationContext(ap.Authority, false);
            //Note that an Azure AD access token has finite lifetime, default expiration is 60 minutes.
            AuthenticationResult authResult;
            if (username != string.Empty && password != string.Empty)
            {

                UserCredential cred = new UserCredential(username, password);
                authResult = authContext.AcquireToken(serviceUrl, clientId, cred);
            }
            else
            {
                authResult = authContext.AcquireToken(serviceUrl, clientId, new Uri(redirectUrl), PromptBehavior.Auto);
            }

            authHeader = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
        }

        protected override Task<HttpResponseMessage> SendAsync(
                 HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            request.Headers.Authorization = authHeader;
            return base.SendAsync(request, cancellationToken);
        }
    }
}
