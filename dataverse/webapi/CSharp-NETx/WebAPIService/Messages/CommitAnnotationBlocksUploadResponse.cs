using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the data from the CommitAnnotationBlocksUploadRequest
    /// </summary>
    public sealed class CommitAnnotationBlocksUploadResponse : HttpResponseMessage
    {

        // Cache the async content
        private string? _content;

        // Provides JObject for property getters
        private JObject JObject
        {
            get
            {
                _content ??= Content.ReadAsStringAsync().GetAwaiter().GetResult();

                return JObject.Parse(_content);
            }
        }

        /// <summary>
        /// The unique identifier of the stored annotation.
        /// </summary>
        public Guid AnnotationId => (Guid)JObject.GetValue(nameof(AnnotationId));

        /// <summary>
        /// The size of the stored annotation in bytes.
        /// </summary>
        public int FileSizeInBytes => (int)JObject.GetValue(nameof(FileSizeInBytes));

    }
}
