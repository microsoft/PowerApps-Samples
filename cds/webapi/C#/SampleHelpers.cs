using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Tooling.Connector;
using PowerApps.Samples.LoginUX;
using System;
using System.Configuration;
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
        public static string clientId = "e5cf0024-a66a-4f16-85ce-99ba97a24bb2";
        public static string redirectUrl = "http://localhost/SdkSample";

        /// <summary>
        /// Gets an HttpClient
        /// </summary>
        /// <param name="name">The name of the connection string to use.</param>
        /// <returns></returns>
        public static HttpClient Connect(string name, string version)
        {
            HttpClient client = null;
            string connectionString = GetConnectionStringFromAppConfig(name);
            HttpMessageHandler messageHandler;

            //You can specify connection information in cds/App.config to run this sample without the login dialog
            if (string.IsNullOrEmpty(connectionString))
            {
                // Failed to find a connection string... Show login Dialog. 
                ExampleLoginForm loginFrm = new ExampleLoginForm();
                // Login process is Async, thus we need to detect when login is completed and close the form. 
                loginFrm.ConnectionToCrmCompleted += LoginFrm_ConnectionToCrmCompleted;
                // Show the dialog here. 
                loginFrm.ShowDialog();

                // If the login process completed
                if (loginFrm.CrmConnectionMgr != null && loginFrm.CrmConnectionMgr.CrmSvc != null && loginFrm.CrmConnectionMgr.CrmSvc.IsReady)
                {
                    AuthenticationType authType = loginFrm.CrmConnectionMgr.CrmSvc.ActiveAuthenticationType;
                    switch (authType) {
                        case AuthenticationType.Office365:
                        case AuthenticationType.IFD:
                        case AuthenticationType.OAuth:

                            
                            string host = loginFrm.CrmConnectionMgr.CrmSvc.CrmConnectOrgUriActual.Host;
                            string scheme = loginFrm.CrmConnectionMgr.CrmSvc.CrmConnectOrgUriActual.Scheme;
                            string url = string.Format("{0}://{1}", scheme, host);
                            string username = loginFrm.CrmConnectionMgr.CrmSvc.OAuthUserId;

                          // Blocked: Can't get the username or password needed to set the message handler
                            //messageHandler = new OAuthMessageHandler(url, clientId, redirectUrl, username, password,
                            //         new HttpClientHandler());
                            break;
                        case AuthenticationType.AD:

                          // Blocked: Can't get the username or password needed to set the credentials
                            //NetworkCredential credentials = new NetworkCredential(username, password, domain);
                            //messageHandler = new HttpClientHandler() { Credentials = credentials };
                            break;                        
                        default:
                            throw new ArgumentOutOfRangeException("Valid authType values are 'Office365', 'IFD', or 'AD'.");
                            break;
                    }
                }


            }
            else
            {

                string url = GetParameterValueFromConnectionString(connectionString, "Url");
                string username = GetParameterValueFromConnectionString(connectionString, "Username");
                string domain = GetParameterValueFromConnectionString(connectionString, "Domain");
                string password = GetParameterValueFromConnectionString(connectionString, "Password");
                string authType = GetParameterValueFromConnectionString(connectionString, "authtype");
                try
                {
                    

                    switch (authType)
                    {
                        case "Office365":
                        case "IFD":

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

                     client = new HttpClient(messageHandler)
                    {
                        BaseAddress = new Uri(string.Format("{0}/api/data/{1}/", url, version)),

                        Timeout = new TimeSpan(0, 2, 0)  //2 minutes
                    };                   
                }
                catch (Exception)
                {
                    throw;
                }
            }


            return client;
        }

        /// <summary>
        /// Gets a named connection string from App.config
        /// </summary>
        /// <param name="name">The name of the connection string to return</param>
        /// <returns>The named connection string</returns>
        private static string GetConnectionStringFromAppConfig(string name)
        {
            //Verify cds/App.config contains a valid connection string with the name.
            try
            {
                return ConfigurationManager.ConnectionStrings[name].ConnectionString;
            }
            catch (Exception)
            {
                Console.WriteLine("You can set connection data in cds/App.config before running this sample. - Switching to Interactive Mode");
                return string.Empty;
            }
        }

        /// <summary>
        /// Handle closing the dialog when completed. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void LoginFrm_ConnectionToCrmCompleted(object sender, EventArgs e)
        {
            if (sender is ExampleLoginForm)
            {
                ((ExampleLoginForm)sender).Close();
            }
        }



        static string GetParameterValueFromConnectionString(string connectionString, string parameter)
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
            string authType = GetParameterValueFromConnectionString(connectionString, "authtype");
            try
            {
                HttpMessageHandler messageHandler;

                switch (authType)
                {
                    case "Office365":
                    case "IFD":

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
