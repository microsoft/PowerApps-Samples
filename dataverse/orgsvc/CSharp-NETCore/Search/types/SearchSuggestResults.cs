using Newtonsoft.Json;
using PowerApps.Samples.Search.Types;

namespace PowerPlatform.Dataverse.CodeSamples.types
{
    /// <summary>
    /// Contains the data from the searchsuggest response
    /// </summary>
    class SearchSuggestResults
    {
        /// <summary>
        /// Provides error information from Azure Cognitive search.
        /// </summary>
        [JsonProperty(PropertyName = "Error")]
        public ErrorDetail? Error { get; set; }

        /// <summary>
        /// A collection of matching records.
        /// </summary>
        public List<SuggestResult>? Value { get; set; }

        /// <summary>
        /// The query context returned as part of response.
        /// </summary>
        public QueryContext? QueryContext { get; set; }
    }
}
