using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to delete a stored binary file, attachment, or annotation.
    /// </summary>
    public sealed class DeleteFileRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the DeleteFileRequest
        /// </summary>
        /// <param name="fileId">The identifier of the stored binary file, attachment, or annotation.</param>
        public DeleteFileRequest(Guid fileId)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "DeleteFile",
                uriKind: UriKind.Relative);

            JObject jObject = new() {
                {"FileId",fileId }
            };

            Content = new StringContent(
                    content: jObject.ToString(),
                    encoding: System.Text.Encoding.UTF8,
                    mediaType: "application/json");
        }
    }
}
