using Newtonsoft.Json;

namespace PowerApps.Samples.Search.Types
{
    /// <summary>
    /// Result object for suggest results.
    /// </summary>
    public sealed class SuggestResult
    {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets document.
        /// </summary>
        [JsonProperty(PropertyName = "document")]
        public Dictionary<string, object> Document { get; set; }
    }
}
