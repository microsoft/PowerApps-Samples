namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to download a block of data.
    /// </summary>
    public sealed class DownloadFileChunkRequest : HttpRequestMessage
    {
        /// <summary>
        /// Instantiates the DownloadFileChunkRequest
        /// </summary>
        /// <param name="entityReference">A reference to the record that has the file column to download.</param>
        /// <param name="fileColumnLogicalName">The logical name of the file column.</param>
        /// <param name="offSet">The size of the offset in bytes.</param>
        /// <param name="chunkSize">The number of bytes being sent.</param>
        /// <param name="returnFullSizedImage">When downloading image file, whether to return the full-sized image. Otherwise the thumbnail-sized image will be returned.</param>
        public DownloadFileChunkRequest(EntityReference entityReference,
            string fileColumnLogicalName,
            int offSet,
            int chunkSize,
            bool returnFullSizedImage = false
            )
        {
            string uriString = $"{entityReference.Path}/{fileColumnLogicalName}/$value";

            if (returnFullSizedImage)
                uriString += "?size=full";

            Method = HttpMethod.Get;
            RequestUri = new Uri(
                uriString: uriString,
                uriKind: UriKind.Relative);
            Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(offSet, offSet + chunkSize - 1);

        }
    }
}
