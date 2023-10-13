using Newtonsoft.Json;
using PowerApps.Samples.Search.Types;

namespace PowerPlatform.Dataverse.CodeSamples.types
{
    class SearchSuggestResults
    {
        [JsonProperty(PropertyName = "Error")]
        public ErrorDetail? Error { get; set; }

        public List<SuggestResult>? Value { get; set; }

        public QueryContext? QueryContext { get; set; }
    }
}
