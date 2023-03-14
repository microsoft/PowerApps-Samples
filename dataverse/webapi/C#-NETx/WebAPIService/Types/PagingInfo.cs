using Newtonsoft.Json;

namespace PowerApps.Samples.Types
{
    /// <summary>
    /// Specifies a number of pages and a number of entity instances per page to return from the query.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class PagingInfo
    {
        /// <summary>
        /// The number of entity instances returned per page.
        /// </summary>
        public int Count { get;set; }
        /// <summary>
        /// The number of pages returned from the query.
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// The info used to page large result sets.
        /// </summary>
        public string? PagingCookie { get; set; }
        /// <summary>
        /// The total number of records should be returned from the query.
        /// </summary>
        public bool ReturnTotalRecordCount { get; set; }
    }
}
