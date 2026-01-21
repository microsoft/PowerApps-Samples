using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response from the RetrieveMultipleRequest
    /// </summary>
    public sealed class RetrieveMultipleResponse : HttpResponseMessage
    {        
        //cache the async content
        private string? _content;

        //Provides JObject for property getters
        private JObject JObject
        {
            get
            {
                _content ??= Content.ReadAsStringAsync().GetAwaiter().GetResult();

                return JObject.Parse(_content);
            }
        }

        /// <summary>
        /// The records returned.
        /// </summary>
        public JArray? Records => (JArray)JObject.GetValue("value");

        /// <summary>
        /// How many records returned. Only populated if '$count=true' is included in the request.queryUri
        /// </summary>
        public int? Count => (int?)JObject.GetValue("@odata.count");

        /// <summary>
        /// The total number of records matching the filter criteria, up to 5000, irrespective of the page size. Only populated if request.IncludeAnnotations is true.
        /// </summary>
        public int? TotalRecordCount => (int?)JObject.GetValue("@Microsoft.Dynamics.CRM.totalrecordcount");

        /// <summary>
        /// Whether the total number of records matching the filter criteria exceeds the TotalRecordCount. Only populated if '$count=true' is included in the request.queryUri
        /// </summary>
        public bool TotalRecordCountExceeded => JObject.GetValue("@Microsoft.Dynamics.CRM.totalrecordcountlimitexceeded")?.ToString() == "True";

        /// <summary>
        /// A link to the next page of records, if any.
        /// </summary>
        public string NextLink => JObject.GetValue("@odata.nextLink")?.ToString();


    }
}
