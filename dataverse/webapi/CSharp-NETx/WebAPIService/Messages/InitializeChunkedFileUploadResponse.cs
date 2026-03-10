using System.Web;

namespace PowerApps.Samples.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the data from the InitializeChunkedFileUploadRequest
    /// </summary>
    public sealed class InitializeChunkedFileUploadResponse : HttpResponseMessage
    {
        public Uri Url => Headers.Location;
        public int ChunkSize => int.Parse(Headers.GetValues("x-ms-chunk-size").First());

    }
}
