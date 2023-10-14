using Newtonsoft.Json;
using PowerApps.Samples.Search.Types;

namespace PowerPlatform.Dataverse.CodeSamples.types
{
    /// <summary>
    /// Contains the data from the searchsuggest response
    /// </summary>
    class SearchSuggestResults
    {
        [JsonProperty(PropertyName = "Error")]
        public ErrorDetail? Error { get; set; }

        public List<SuggestResult>? Value { get; set; }

        public QueryContext? QueryContext { get; set; }
    }
}
