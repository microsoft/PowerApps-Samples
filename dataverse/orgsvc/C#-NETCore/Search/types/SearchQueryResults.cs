using PowerApps.Samples.Search.Types;

namespace PowerPlatform.Dataverse.CodeSamples.types
{
    /// <summary>
    /// Used to deserialize JSON data from the searchqueryResponse.response string property.
    /// </summary>
    public sealed class SearchQueryResults
    {
        /// <summary>
        /// Provides error information from Azure Cognitive search.
        /// </summary>
        public ErrorDetail? Error { get; set; }

        /// <summary>
        /// A collection of matching records.
        /// </summary>
        public List<QueryResult>? Value { get; set; }

        /// <summary>
        /// If facets were requested in the query, a dictionary of facet values.
        /// </summary>
        public Dictionary<string, IList<FacetResult>>? Facets { get; set; }

        /// <summary>
        /// The query context returned as part of response.
        /// </summary>
        public QueryContext? QueryContext { get; set; }

        /// <summary>
        /// If `"Count": true` is included in the body of the request, the count of all documents that match the search, ignoring top and skip.
        /// </summary>
        public long Count { get; set; }
    }
}
