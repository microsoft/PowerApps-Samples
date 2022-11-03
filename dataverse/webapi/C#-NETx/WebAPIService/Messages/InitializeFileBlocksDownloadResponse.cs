using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the data from the InitializeFileBlocksDownloadRequest
    /// </summary>
    public sealed class InitializeFileBlocksDownloadResponse : HttpResponseMessage
    {

        // Cache the async content
        private string? _content;

        // Provides JObject for property getters
        private JObject jObject
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
        public string FileContinuationToken => (string)jObject.GetValue(nameof(FileContinuationToken));

        /// <summary>
        /// The size of the data file in bytes.
        /// </summary>
        public long FileSizeInBytes => (long)jObject.GetValue(nameof(FileSizeInBytes));


        /// <summary>
        /// The name of the stored file.
        /// </summary>
        public string FileName => (string)jObject.GetValue(nameof(FileName));

    }
}
