using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Net.Http;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        static void Main(string[] args)
        {
            //These sample application registration values are available for all online instances.
            string clientId = "e5cf0024-a66a-4f16-85ce-99ba97a24bb2";
            string redirectUrl = "http://localhost/SdkSample";
            try
            {
                //Get configuration data from ../../../../cds/App.config
                string connectionString = ConfigurationManager.ConnectionStrings["Connect"].ConnectionString;

                //Required for all:
                string url = getParameterValueFromConnectionString(connectionString, "Url");
                string authType = getParameterValueFromConnectionString(connectionString, "authtype");

                //Required only for on-premises Active Directory authentication
                string username = getParameterValueFromConnectionString(connectionString, "Username");
                string password = getParameterValueFromConnectionString(connectionString, "Password");
                string domain = getParameterValueFromConnectionString(connectionString, "Domain");               

                using (HttpClient client = getHttpClient(url, username, domain, password, clientId, redirectUrl, authType)) {

                    //Send the WhoAmI request to the Web API using a GET request. 
                    var response = client.GetAsync("api/data/v9.0/WhoAmI",
                            HttpCompletionOption.ResponseHeadersRead).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        //Get the response content and parse it.
                        JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        Guid userId = (Guid)body["UserId"];
                        Console.WriteLine("Your system user ID is: {0}", userId);
                    }
                    else
                    {
                        Console.WriteLine("The request failed with a status of '{0}'",
                               response.ReasonPhrase);
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayException(ex);
                throw;
            }
            finally {
                Console.WriteLine("Press <Enter> to exit the program.");
                Console.ReadLine();
            }            
        }
    }
}
