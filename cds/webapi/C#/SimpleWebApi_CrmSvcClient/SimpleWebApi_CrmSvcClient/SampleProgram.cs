using Microsoft.Xrm.Tooling.Connector;
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

            try
            {
                //NOTE: this sample required an OAuth or Certificate type for authentication,  Non OAUTH connect strings are not supported by CrmServiceClient currently. 
                var a = Console.ReadLine();
                //Get configuration data from App.config connectionStrings
                string connectionString = ConfigurationManager.ConnectionStrings["Connect"].ConnectionString;

                // Check to make sure this is an oAuth string. 
                string authType = SampleHelpers.GetParameterValueFromConnectionString(connectionString, "authtype");
                if (!(authType.Equals("oauth" , StringComparison.OrdinalIgnoreCase) || authType.Equals("certificate" , StringComparison.OrdinalIgnoreCase )))
                {
                    Console.WriteLine("Only oAuth or Certificate Auth is supported for this sample at this time,  Please reform your connection string to have an oAuth or Certificate based connection" );
                    Console.WriteLine("For your convenience we have configured the following Sample Client ID and Redirect URI's for your use with samples");
                    Console.WriteLine($"ClientId = {SampleHelpers.clientId}");
                    Console.WriteLine($"RedirectUri = {SampleHelpers.redirectUrl}");
                    return; 
                }

                // else try to setup a CrmSvcClient

                using (CrmServiceClient client = new CrmServiceClient(connectionString)) {
                    if (!client.IsReady)
                    {
                        Console.WriteLine("Failed to authenticate to CDS, Please see the log for this process for details.");
                        Console.WriteLine("Note: You may need to configure <add name=\"Microsoft.Xrm.Tooling.Connector.CrmServiceClient\" value=\"Verbose\" /> to see the full log of what occured");
                        return; 
                    }

                    
                    //Send the WhoAmI request to the Web API using a GET request. 
                    HttpResponseMessage response = client.ExecuteCrmWebRequest(HttpMethod.Get, "WhoAmI", string.Empty, null);
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

                    //WhoAmIResponse response = WhoAmI(client);
                    //Console.WriteLine("Your system user ID is: {0}", response.UserId);

                }
            }
            catch (Exception ex)
            {
                SampleHelpers.DisplayException(ex);
                throw;
            }
            finally {
                Console.WriteLine("Press <Enter> to exit the program.");
                Console.ReadLine();
            }            
        }
    }
}
