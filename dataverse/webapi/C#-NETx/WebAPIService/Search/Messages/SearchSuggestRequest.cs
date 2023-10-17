using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerApps.Samples.Search.Types;

namespace PowerApps.Samples.Search.Messages
{
    /// <summary>
    /// Contains the data to send a SearchSuggest request
    /// </summary>
    public sealed class SearchSuggestRequest : HttpRequestMessage
    {
        private string? _orderby;
        private string? _options;
        private bool _fuzzy;
        private string? _search;
        private string? _filter;
        private string? _entities;
        private int? _top;

        public SearchSuggestRequest(SuggestParameters suggestParameters)
        {
            _top = suggestParameters.Top;
            _fuzzy = suggestParameters.Fuzzy;
            _search = suggestParameters.Search;
            _entities = JsonConvert.SerializeObject(suggestParameters.Entities);
            _orderby = JsonConvert.SerializeObject(suggestParameters.OrderBy);
            _filter = suggestParameters.Filter;
            _options = JsonConvert.SerializeObject(suggestParameters.Options);


            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "searchsuggest",
                uriKind: UriKind.Relative);

            SetContent();

        }

        /// <summary>
        /// List of comma-separated clauses where each clause is an attribute name followed by 'asc' or 'desc'.
        /// </summary>
        public string? OrderBy { get { return _orderby; } set { _orderby = value; SetContent(); } }

        /// <summary>
        /// Options are settings configured to search a search term. Eg. "{ 'advancedsuggestenabled': 'true' }".
        /// </summary>
        public string? Options { get { return _options; } set { _options = value; SetContent(); } }

        /// <summary>
        /// Fuzzy search to aid with misspellings. The default is false.
        /// </summary>
        public bool Fuzzy { get { return _fuzzy; } set { _fuzzy = value; SetContent(); } }

        /// <summary>
        /// Search term.
        /// </summary>
        public string? Search { get { return _search; } set { _search = value; SetContent(); } }

        /// <summary>
        /// Filter criteria to reduce results returned.
        /// </summary>
        public string? Filter { get { return _filter; } set { _filter = value; SetContent(); } }

        /// <summary>
        /// The default scope is searching across all search-configured entities and fields.
        /// </summary>
        public string? Entities { get { return _entities; } set { _entities = value; SetContent(); } }

        /// <summary>
        /// Number of suggestions to retrieve. The default is 5.
        /// </summary>
        public int? Top { get { return _top; } set { _top = value; SetContent(); } }

        private void SetContent()
        {
            JObject _content = new() {
                { "orderby", _orderby},
                { "options", _options},
                { "fuzzy", _fuzzy},
                { "search", _search},
                { "filter", _filter },
                { "entities", _entities },
                { "top", _top },
            };

            Content = new StringContent(
                    content: _content.ToString(),
                    encoding: System.Text.Encoding.UTF8,
                    mediaType: "application/json");

        }
    }
}