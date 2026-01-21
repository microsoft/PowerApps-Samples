using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the data from the InitializeAttachmentBlocksUploadRequest
    /// </summary>
    public sealed class InitializeAttachmentBlocksUploadResponse : HttpResponseMessage
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
        /// A token that uniquely identifies a sequence of related data uploads.
        /// </summary>
        public string FileContinuationToken => (string)JObject.GetValue(nameof(FileContinuationToken));

    }
}
