using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        public static WhoAmIResponse WhoAmI(HttpClient client) {
            WhoAmIResponse returnValue = new WhoAmIResponse();
            //Send the WhoAmI request to the Web API using a GET request. 
            HttpResponseMessage response = client.GetAsync("WhoAmI",
                    HttpCompletionOption.ResponseHeadersRead).Result;
            if (response.IsSuccessStatusCode)
            {
                //Get the response content and parse it.
                JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                returnValue.BusinessUnitId = (Guid)body["BusinessUnitId"];
                returnValue.UserId = (Guid)body["UserId"];
                returnValue.OrganizationId = (Guid)body["OrganizationId"];
            }
            else
            {
                throw new Exception(string.Format("The WhoAmI request failed with a status of '{0}'",
                       response.ReasonPhrase));
            }
            return returnValue;
        }
    }

    public class WhoAmIResponse
    {
        public Guid BusinessUnitId { get; set; } 
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
    }

}
