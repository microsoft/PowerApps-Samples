using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace PowerApps.Samples
{
    class SampleProgram
    {
        static void Main(string[] args)
        {
            // The URL to the CDS environment you want to connect with
            // i.e. https://yourOrg.crm.dynamics.com
            string resource = "https://globaldisco.crm.dynamics.com/";

            // Azure Active Directory registered app clientid for Microsoft samples
            var clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
            // Azure Active Directory registered app Redirect URI for Microsoft samples
            var redirectUri = new Uri("app://58145B91-0C36-4500-8554-080854F2AC97");
            string authority = "https://login.microsoftonline.com/common";


            var context = new AuthenticationContext(authority, false);


            //Display prompt allowing user to select account to use.
            var platformParameters = new PlatformParameters(
              PromptBehavior.SelectAccount
              );

            //Get the token based on the credentials entered
            var token = context.AcquireTokenAsync(resource,
              clientId,
              redirectUri,
              platformParameters,
              UserIdentifier.AnyUser)
              .Result;

            try
            {
                List<Instance> instances = GetInstances(resource, token.AccessToken);
                if (instances.Count > 1)
                {
                    Console.WriteLine("Available Instances:");
                    instances.ForEach(x => {
                        Console.WriteLine("UniqueName:{0} ApiUrl:{1} Url:{2}", x.UniqueName, x.ApiUrl, x.Url);
                    });
                }
                else
                {
                    Console.WriteLine("No instances found.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                return;
            }
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }

        static List<Instance> GetInstances(string resource, string accessToken)
        {
            using (var client = new HttpClient())
            {
                //string GlobalDiscoUrl = "https://globaldisco.crm.dynamics.com/";
                string clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
                string secret = "5-=?11%-0H;%2S[eIce#Wn+)akI8";
                var cred = new ClientCredential(clientId, secret);

                client.BaseAddress = new Uri(resource);
                client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Bearer", accessToken);

                client.Timeout = new TimeSpan(0, 2, 0);
                client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                // Use the WhoAmI function
                var response = client.GetAsync("api/discovery/v1.0/Instances", HttpCompletionOption.ResponseHeadersRead).Result;

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
        }
    }

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
