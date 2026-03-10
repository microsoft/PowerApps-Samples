using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to commits the uploaded data blocks to the File store.
    /// </summary>
    public sealed class CommitFileBlocksUploadRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the CommitFileBlocksUploadRequest
        /// </summary>
        /// <param name="fileName">A filename to associate with the binary data file.</param>
        /// <param name="mimeType">The MIME type of the uploaded file.</param>
        /// <param name="blockList">The IDs of the uploaded data blocks, in the correct sequence, that will result in the final File when the data blocks are combined.</param>
        /// <param name="fileContinuationToken">A token that uniquely identifies a sequence of related data uploads.</param>
        public CommitFileBlocksUploadRequest(
            string fileName, 
            string mimeType, 
            List<string> blockList, 
            string fileContinuationToken)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "CommitFileBlocksUpload",
                uriKind: UriKind.Relative);

            JObject body = new()
            {
                { "FileName", fileName },
                { "MimeType", mimeType },
                { "BlockList", JToken.FromObject(blockList.ToArray()) },
                { "FileContinuationToken", fileContinuationToken}
            };

            Content = new StringContent(
                    content: body.ToString(),
                    encoding: System.Text.Encoding.UTF8,
                    mediaType: "application/json");
        }
    }
}
