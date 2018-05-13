using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        [STAThread] // Added to support UX
        static void Main(string[] args)
        {

            try
            {

                using (HttpClient client = SampleHelpers.Connect("Connect", "v9.0")) {

                    ////Send the WhoAmI request to the Web API using a GET request. 
                    HttpResponseMessage response = client.GetAsync("WhoAmI",
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
