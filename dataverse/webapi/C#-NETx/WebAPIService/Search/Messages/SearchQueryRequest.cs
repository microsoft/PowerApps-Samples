using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerApps.Samples.Search.Types;

namespace PowerApps.Samples.Search.Messages
{
    /// <summary>
    /// Contains the data to send a searchquery request
    /// </summary>
    public sealed class SearchQueryRequest : HttpRequestMessage
    {
        private int? _top;
        private string? _search;
        private int? _skip;
        private string? _entities;
        private string? _orderby;
        private string? _filter;
        private bool _count;
        private string? _options;
        private string? _facets;

        public SearchQueryRequest(QueryParameters queryParameters)
        {
            _top = queryParameters.Top;
            _search = queryParameters.Search;
            _skip = queryParameters.Skip;
            _entities = JsonConvert.SerializeObject(queryParameters.Entities);
            _orderby = JsonConvert.SerializeObject(queryParameters.OrderBy);
            _filter = queryParameters.Filter;
            _count = queryParameters.Count;
            _options = JsonConvert.SerializeObject(queryParameters.Options);
            _facets = JsonConvert.SerializeObject(queryParameters.Facets);

            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "searchquery",
                uriKind: UriKind.Relative);

            SetContent();

        }
        /// <summary>
        /// The number of search results to retrieve. The default is 50, and the maximum value is 100.
        /// </summary>
        public int? Top { get { return _top; } set { _top = value; SetContent(); } }

        /// <summary>
        /// Search term.
        /// </summary>
        public string? Search { get { return _search; } set { _search = value; SetContent(); } }

        /// <summary>
        /// The number of search results to skip.
        /// </summary>
        public int? Skip { get { return _skip; } set { _skip = value; SetContent(); } }

        /// <summary>
        /// The default scope is searching across all search-configured entities and fields.
        /// </summary>
        public string? Entities { get { return _entities; } set { _entities = value; SetContent(); } }


        /// <summary>
        /// List of comma-separated clauses where each clause is an attribute name followed by 'asc' or 'desc'.
        /// </summary>
        public string? OrderBy { get { return _orderby; } set { _orderby = value; SetContent(); } }


        /// <summary>
        /// Filter criteria to reduce results returned.
        /// </summary>
        public string? Filter { get { return _filter; } set { _filter = value; SetContent(); } }

        /// <summary>
        /// Options are settings configured to search a search term. Eg. "{'querytype': 'lucene', 'searchmode': 'all', 'besteffortsearchenabled': 'true', 'grouprankingenabled': 'true'}".
        /// </summary>
        public string? Options { get { return _options; } set { _options = value; SetContent(); } }

        /// <summary>
        /// Facets support the ability to drill down into data results after they've been retrieved.
        /// </summary>
        public string? Facets { get { return _facets; } set { _facets = value; SetContent(); } }

        /// <summary>
        /// Sets the Content when values change
        /// </summary>
        private void SetContent()
        {
            JObject _content = new() {
                { "top", _top},
                { "search", _search},
                { "skip", _skip},
                { "entities", _entities},
                { "orderby", _orderby },
                { "filter", _filter },
                { "count", _count },
                { "options", _options },
                { "facets", _facets },
            };

            Content = new StringContent(
                    content: _content.ToString(),
                    encoding: System.Text.Encoding.UTF8,
                    mediaType: "application/json");

        }
    }
}