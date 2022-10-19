namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to update file column
    /// </summary>
    public sealed class UploadFileRequest : HttpRequestMessage
    {
        public UploadFileRequest(
            EntityReference entityReference,
            string columnName,
            Stream fileContent,
            string fileName)
        {
            Method = HttpMethod.Patch;
            RequestUri = new Uri(
                uriString: $"{entityReference.Path}/{columnName}",
                uriKind: UriKind.Relative);
            Content = new StreamContent(fileContent);
            Content.Headers.Add("Content-Type", "application/octet-stream");
            Content.Headers.Add("x-ms-file-name", fileName);
        }
    }
}
