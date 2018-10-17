using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {

        private static JObject account = new JObject();            // Sample CRM entity instance
        private static string accountUri;          // Sample instance absolute URI
        private static string queryOptions;        // Select clause to filter the record retrievals
        private static string initialAcctETagVal;  // The initial ETag value of the account created   
        private static string updatedAcctETagVal;  // The ETag value of the account after it is updated
        private static string entityUri;

        /// <summary> 
        /// Returns the current state of the specified entity, using the specified query criteria. 
        /// </summary>
        /// <param name="entityUri">Relative URI of the entity instance to retrieve</param>
        /// <param name="selectCriteria">Query selection, filtering, expansion</param>
        /// <returns>JObject containing entity's specified properties; otherwise null if not exists.
        /// </returns>
        private static JObject GetCurrentRecord(HttpClient httpClient, string entityUri, string queryOptions)
        {
            JObject entity = null;
            if (String.IsNullOrEmpty(entityUri))
            { throw new ArgumentNullException(); }
            HttpResponseMessage response1 = httpClient.GetAsync(entityUri + queryOptions).Result;
            if (response1.IsSuccessStatusCode) //200
            {
                string body = response1.Content.ReadAsStringAsync().Result;
                entity = JObject.Parse(body);
            }
            else if (response1.StatusCode == HttpStatusCode.NotFound) //404
            { return null; }
            else
            { throw new Exception(string.Format("Failed to return current state of the entity", response1.Content)); }
            return entity;
        }
    }
}
