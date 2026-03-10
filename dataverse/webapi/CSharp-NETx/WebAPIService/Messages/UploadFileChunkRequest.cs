using System.Net.Http.Headers;
namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Uploads a block of data to storage.
    /// </summary>
    public sealed class UploadFileChunkRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the UploadFileChunkRequest
        /// </summary>
        /// <param name="url">The InitializeChunkedFileUploadResponse.Url value</param>
        /// <param name="uploadFileName">The name of the file to upload</param>
        /// <param name="chunkSize">The InitializeChunkedFileUploadResponse.ChunkSize value</param>
        /// <param name="fileBytes">The bytes for the chunk.</param>
        /// <param name="offSet">The offset for the chunk.</param>
        public UploadFileChunkRequest(
            Uri url,
            string uploadFileName,
            int chunkSize, 
            byte[] fileBytes, 
            int offSet)
        {
            Method = HttpMethod.Patch;
            RequestUri = url;

            var count = (offSet + chunkSize) > fileBytes.Length ? fileBytes.Length % chunkSize : chunkSize;

            Content = new ByteArrayContent(fileBytes, offSet, count);

            Content.Headers.Add("Content-Type", "application/octet-stream");
            Content.Headers.ContentRange = new ContentRangeHeaderValue(offSet, offSet + (count - 1), fileBytes.Length);

            Headers.Add("x-ms-file-name", uploadFileName);

        }
    }
}
