using PowerApps.Samples.Search.Types;

namespace PowerPlatform.Dataverse.CodeSamples.types
{
    class SearchQueryResults
    {
        public ErrorDetail? Error { get; set; }

        public List<QueryResult>? Value { get; set; }

        public Dictionary<string, IList<FacetResult>>? Facets { get; set; }

        public QueryContext? QueryContext { get; set; }

        public long Count { get; set; }
    }
}
