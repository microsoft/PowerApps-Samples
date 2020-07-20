using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System.Configuration;

namespace PowerApps.Samples
{
    class Program
    {

        //These sample application registration values are available for all online instances.
        public static string clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
        public static string redirectUrl = "app://58145B91-0C36-4500-8554-080854F2AC97";

        // version of ADAL that is linked to deal with different version of ADAL. 
        private static Version _ADALAsmVersion;

        static void Main(string[] args)
        {
            try
            {
                List<Instance> instances = null;
                var connecStringContainer = ConfigurationManager.ConnectionStrings["Connect"];
                if (connecStringContainer != null)
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["Connect"].ConnectionString;
                    //This sample does not use the Url set in the connection string, just the credentials.
                    string username = GetParameterValueFromConnectionString(connectionString, "Username");
                    string password = GetParameterValueFromConnectionString(connectionString, "Password");
                    instances = GetInstances(username, password);
                }
                else
                    instances = GetInstances(string.Empty, string.Empty);


                if (instances.Count >= 1)
                {
                    Console.WriteLine("Available Instances:");
                    instances.ForEach(x =>
                    {
                        Console.WriteLine("UniqueName:{0} ApiUrl:{1} Url:{2}", x.UniqueName, x.ApiUrl, x.Url);
                    });
                }
                else
                {
                    Console.WriteLine("No instances found.");
                }

            }
            catch (TypeInitializationException tiex)
            {
                StringBuilder errorMessageBuilder = new StringBuilder();

                errorMessageBuilder.Append("Make sure the ADALListener ");
                errorMessageBuilder.Append("shared listener in App.Config system.diagnostics > ");
                errorMessageBuilder.Append("sharedlisteners is commented out before running this sample.");
                Console.WriteLine(errorMessageBuilder.ToString());
                DisplayException(tiex);
            }
            catch (Exception ex)
            {
                DisplayException(ex);
                throw;
            }
            finally
            {
                Console.WriteLine("Press <Enter> to exit the program.");
                Console.ReadLine();
            }


        }
        /// <summary>
        /// Uses the global web api discovery service to return instances
        /// </summary>
        /// <param name="clientId">The Azure AD client (app) registration</param>
        /// <param name="username">The user name</param>
        /// <param name="password">The password</param>
        /// <returns>A List of Instances</returns>
        static List<Instance> GetInstances(string username, string password)
        {

            string GlobalDiscoUrl = "https://globaldisco.crm.dynamics.com/";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetAccessToken(username, password, new Uri("https://disco.crm.dynamics.com/api/discovery/"))); // Getting Authority from Commercial 
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

        #region Utilities 

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

        #region AuthSupport
        /// <summary>
        /// Forming version tagged UriBuilder
        /// </summary>
        /// <param name="discoveryServiceUri"></param>
        /// <returns></returns>
        private static UriBuilder GetUriBuilderWithVersion(Uri discoveryServiceUri)
        {
            UriBuilder webUrlBuilder = new UriBuilder(discoveryServiceUri);
            string webPath = "web";

            if (!discoveryServiceUri.AbsolutePath.EndsWith(webPath))
            {
                if (discoveryServiceUri.AbsolutePath.EndsWith("/"))
                    webUrlBuilder.Path = string.Concat(webUrlBuilder.Path, webPath);
                else
                    webUrlBuilder.Path = string.Concat(webUrlBuilder.Path, "/", webPath);
            }

            UriBuilder versionTaggedUriBuilder = new UriBuilder(webUrlBuilder.Uri);
            return versionTaggedUriBuilder;
        }

        /// <summary>
        /// Get the Authority and Support data from the requesting system using a sync call. 
        /// </summary>
        /// <param name="targetServiceUrl">Resource URL</param>
        /// <param name="logSink">Log tracer</param>
        /// <returns>Populated AuthenticationParameters or null</returns>
        private static AuthenticationParameters GetAuthorityFromTargetService(Uri targetServiceUrl)
        {
            try
            {
                // if using ADAL > 4.x  return.. // else remove oauth2/authorize from the authority
                if (_ADALAsmVersion == null)
                {
                    // initial setup to get the ADAL version 
                    var AdalAsm = System.Reflection.Assembly.GetAssembly(typeof(IPlatformParameters));
                    if (AdalAsm != null)
                        _ADALAsmVersion = AdalAsm.GetName().Version;
                }

                AuthenticationParameters foundAuthority;
                if (_ADALAsmVersion != null && _ADALAsmVersion >= Version.Parse("5.0.0.0"))
                {
                    foundAuthority = CreateFromUrlAsync(targetServiceUrl);
                }
                else
                {
                    foundAuthority = CreateFromResourceUrlAsync(targetServiceUrl);
                }

                if (_ADALAsmVersion != null && _ADALAsmVersion > Version.Parse("4.0.0.0"))
                {
                    foundAuthority.Authority = foundAuthority.Authority.Replace("oauth2/authorize", "");
                }

                return foundAuthority;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        /// <summary>
        /// Creates authentication parameters from the address of the resource.
        /// </summary>
        /// <param name="targetServiceUrl">Resource URL</param>
        /// <returns>AuthenticationParameters object containing authentication parameters</returns>
        private static AuthenticationParameters CreateFromResourceUrlAsync(Uri targetServiceUrl)
        {
            var result = (Task<AuthenticationParameters>)typeof(AuthenticationParameters)
                .GetMethod("CreateFromResourceUrlAsync").Invoke(null, new[] { targetServiceUrl });
            return result.Result;
        }

        /// <summary>
        /// Creates authentication parameters from the address of the resource.
        /// Invoked for ADAL 5+ which changed the method used to retrieve authentication parameters.
        /// </summary>
        /// <param name="targetServiceUrl">Resource URL</param>
        /// <returns>AuthenticationParameters object containing authentication parameters</returns>
        private static AuthenticationParameters CreateFromUrlAsync(Uri targetServiceUrl)
        {
            var result = (Task<AuthenticationParameters>)typeof(AuthenticationParameters)
                .GetMethod("CreateFromUrlAsync").Invoke(null, new[] { targetServiceUrl });

            return result.Result;
        }


        public static string GetAccessToken(string userName, string password, Uri serviceRoot)
        {
            var targetServiceUrl = GetUriBuilderWithVersion(serviceRoot);
            // Obtain the Azure Active Directory Authentication Library (ADAL) authentication context.
            AuthenticationParameters ap = GetAuthorityFromTargetService(targetServiceUrl.Uri);
            AuthenticationContext authContext = new AuthenticationContext(ap.Authority, false);
            //Note that an Azure AD access token has finite lifetime, default expiration is 60 minutes.
            AuthenticationResult authResult;

            if (userName != string.Empty && password != string.Empty)
            {

                UserPasswordCredential cred = new UserPasswordCredential(userName, password);
                authResult = authContext.AcquireTokenAsync(ap.Resource, clientId, cred).Result;
            }
            else
            {
                // Note that PromptBehavior.Always is why the UserID is aways prompted when this path is executed
                // Lookup PromptBehavior's to understand what other options exist. 
                PlatformParameters platformParameters = new PlatformParameters(PromptBehavior.Always);
                authResult = authContext.AcquireTokenAsync(ap.Resource, clientId, new Uri(redirectUrl), platformParameters).Result;
            }

            return authResult.AccessToken;
        }
        #endregion
        #endregion
    }

    /// <summary>
    /// Object returned by the discovery service
    /// </summary>

    class Instance
    {
        public string Id { get; set; }
        public string UniqueName { get; set; }
        public string UrlName { get; set; }
        public string FriendlyName { get; set; }
        public int State { get; set; }
        public string Version { get; set; }
        public string Url { get; set; }
        public string ApiUrl { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
