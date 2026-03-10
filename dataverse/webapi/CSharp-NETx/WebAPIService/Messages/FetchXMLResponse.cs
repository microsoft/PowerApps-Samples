using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response from the FetchXmlRequest
    /// </summary>
    public sealed class FetchXmlResponse : HttpResponseMessage
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
        /// How many records returned.
        /// </summary>
        public int? Count => (int?)JObject.GetValue("@odata.count");

        /// <summary>
        /// A paging cookie value to be used for subsequent requests. Only populated if request.IncludeAnnotations is true.
        /// </summary>
        public string? FetchxmlPagingCookie => (string?)JObject.GetValue("@Microsoft.Dynamics.CRM.fetchxmlpagingcookie");

        /// <summary>
        /// The total number of records matching the filter criteria, up to 5000, irrespective of the page size. Only populated if request.IncludeAnnotations is true.
        /// </summary>
        public int? TotalRecordCount => (int?)JObject.GetValue("@Microsoft.Dynamics.CRM.totalrecordcount");

        /// <summary>
        /// Whether the number of records exceeds the total record count
        /// </summary>
        public bool TotalRecordCountExceeded => JObject.GetValue("@Microsoft.Dynamics.CRM.totalrecordcountlimitexceeded")?.ToString() == "True";

        /// <summary>
        /// Whether more records match the query filter.
        /// </summary>
        public bool MoreRecords => JObject.GetValue("@Microsoft.Dynamics.CRM.morerecords")?.ToString() == "True";

    }
}
