namespace PowerApps.Samples.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the data from the DownloadFileChunkRequest
    /// </summary>
    public sealed class DownloadFileChunkResponse : HttpResponseMessage
    {
        /// <summary>
        /// The portion of the file requested
        /// </summary>
        public byte[] Data
        {
            get
            {
                return Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            }
        }

        public int FileSize
        {
            get
            {
                // 'x-ms-file-size' Header not included for images unless '?size=full' included in URL
                if (Headers.Contains("x-ms-file-size"))
                {
                    return int.Parse(Headers.GetValues("x-ms-file-size").First());
                }
                else
                {
                    return Data.Length;
                }                
            }
        }
    }
}
