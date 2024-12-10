using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the data from the InitializeAnnotationBlocksDownloadRequest
    /// </summary>
    public sealed class InitializeAnnotationBlocksDownloadResponse : HttpResponseMessage
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
        /// A token that uniquely identifies a sequence of related data blocks.
        /// </summary>
        public string FileContinuationToken => (string)JObject.GetValue(nameof(FileContinuationToken));

        /// <summary>
        /// The size of the data file in bytes.
        /// </summary>
        public int FileSizeInBytes => (int)JObject.GetValue(nameof(FileSizeInBytes));
   

        /// <summary>
        /// The name of the stored file.
        /// </summary>
        public string FileName => (string)JObject.GetValue(nameof(FileName));

    }
}
