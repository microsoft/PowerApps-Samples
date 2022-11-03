using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Uploads a block of data to storage.
    /// </summary>
    public sealed class UploadBlockRequest : HttpRequestMessage
    {

        public UploadBlockRequest(string blockId, byte[] blockData,string fileContinuationToken)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "UploadBlock",
                uriKind: UriKind.Relative);

            JObject body = new()
            {
                { "BlockId", blockId},
                { "BlockData", blockData},
                { "FileContinuationToken", fileContinuationToken}
            };

            Content = new StringContent(
                    content: body.ToString(),
                    encoding: System.Text.Encoding.UTF8,
                    mediaType: "application/json");
        }
    }
}
