using PowerApps.Samples.Search.Types;

namespace PowerPlatform.Dataverse.CodeSamples.types
{
    class SearchAutoCompleteResults
    {
        public ErrorDetail? Error {  get; set; }

        public string? Value { get; set; }

        public QueryContext? QueryContext { get; set; }
    }
}
