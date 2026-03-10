using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to download a block of data.
    /// </summary>
    public sealed class DownloadBlockRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the DownloadBlockRequest
        /// </summary>
        /// <param name="offset">The offset (in bytes) from the beginning of the block to the first byte of data in the block.</param>
        /// <param name="blockLength">The size of the block in bytes.</param>
        /// <param name="fileContinuationToken">A token that uniquely identifies a sequence of related data blocks.</param>
        public DownloadBlockRequest(long offset, long blockLength, string fileContinuationToken)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "DownloadBlock",
                uriKind: UriKind.Relative);

            JObject content = new() {
                { "Offset", offset },
                { "BlockLength", blockLength },
                { "FileContinuationToken", fileContinuationToken }
            };

            Content = new StringContent(
                    content: content.ToString(),
                    encoding: System.Text.Encoding.UTF8,
                    mediaType: "application/json");
        }
    }
}
