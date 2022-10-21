namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to retrieve a file column value
    /// 
    public sealed class DownloadFileRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the DownloadFileRequest
        /// </summary>
        /// <param name="entityReference">A reference to the record to get the column data from.</param>
        /// <param name="property">The name of the column.</param>
        public DownloadFileRequest(EntityReference entityReference, string property)
        {
            Method = HttpMethod.Get;
            RequestUri = new Uri(
                uriString: $"{entityReference.Path}/{property}/$value",
                uriKind: UriKind.Relative);
        }
    }
}
