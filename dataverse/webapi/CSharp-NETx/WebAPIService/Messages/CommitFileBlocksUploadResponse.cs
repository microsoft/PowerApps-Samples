using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the data from the CommitFileBlocksUploadRequest
    /// </summary>
    public sealed class CommitFileBlocksUploadResponse : HttpResponseMessage
    {

        // Cache the async content
        private string? _content;

        // Provides JObject for property getters
        private JObject content
        {
            get
            {
                _content ??= Content.ReadAsStringAsync().GetAwaiter().GetResult();

                return JObject.Parse(_content);
            }
        }

        /// <summary>
        /// The unique identifier of the stored File.
        /// </summary>
        public Guid FileId => (Guid)content.GetValue(nameof(FileId));

        /// <summary>
        /// The size of the stored File in bytes.
        /// </summary>
        public int FileSizeInBytes => (int)content.GetValue(nameof(FileSizeInBytes));

    }
}
