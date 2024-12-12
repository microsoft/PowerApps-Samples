namespace PowerApps.Samples.Search.Messages
{
    /// <summary>
    /// Contains the data to perform the searchstatistics function
    /// </summary>
    public sealed class SearchStatisticsRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the searchstatisticsRequest
        /// </summary>
        public SearchStatisticsRequest()
        {
            Method = HttpMethod.Get;
            RequestUri = new Uri(
                uriString: "searchstatistics",
                uriKind: UriKind.Relative);
        }
    }
}
