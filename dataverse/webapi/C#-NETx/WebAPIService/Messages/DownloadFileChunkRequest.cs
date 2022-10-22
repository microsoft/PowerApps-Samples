namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to download a block of data.
    /// </summary>
    public sealed class DownloadFileChunkRequest : HttpRequestMessage
    {

        public DownloadFileChunkRequest(EntityReference entityReference,
            string fileColumnLogicalName,
            int offSet,
            int chunkSize
            )
        {
            Method = HttpMethod.Get;
            RequestUri = new Uri(
                uriString: $"{entityReference.Path}/{fileColumnLogicalName}/$value?size=full",
                uriKind: UriKind.Relative);
            Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(offSet, offSet + chunkSize - 1);

        }
    }
}
