namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to initialize a chunked file upload
    /// </summary>
    public sealed class InitializeChunkedFileUploadRequest : HttpRequestMessage
    {
        public InitializeChunkedFileUploadRequest(EntityReference entityReference,
            string fileColumnLogicalName,
            string uploadFileName)
        {
            Method = HttpMethod.Patch;
            RequestUri = new Uri(
                uriString: $"{entityReference.Path}/{fileColumnLogicalName}?x-ms-file-name={uploadFileName}",
                uriKind: UriKind.Relative);

            Headers.Add("x-ms-transfer-mode", "chunked");
        }
    }
}
