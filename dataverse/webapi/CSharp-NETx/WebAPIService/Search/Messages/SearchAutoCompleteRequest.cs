using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerApps.Samples.Search.Types;

namespace PowerApps.Samples.Search.Messages
{
    /// <summary>
    /// Contains the data to send an autocomplete request
    /// </summary>
    public sealed class SearchAutoCompleteRequest : HttpRequestMessage
    {
        private string? _entities;
        private string? _search;
        private bool _fuzzy;
        private string? _filter;

        public SearchAutoCompleteRequest(AutocompleteParameters autocompleteParameters)
        {
            _search = autocompleteParameters.Search;
            _fuzzy = autocompleteParameters.Fuzzy;
            _entities = JsonConvert.SerializeObject(autocompleteParameters.Entities);
            _filter = autocompleteParameters.Filter;

            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "searchautocomplete",
                uriKind: UriKind.Relative);

            SetContent();

        }

        /// <summary>
        /// The default scope is searching across all search-configured entities and fields.
        /// </summary>
        public string? Entities { get { return _entities; } set { _entities = value; SetContent(); } }

        /// <summary>
        /// Search term.
        /// </summary>
        public string? Search { get { return _search; } set { _search = value; SetContent(); } }

        /// <summary>
        /// Fuzzy search to aid with misspellings. The default is false.
        /// </summary>
        public bool Fuzzy { get { return _fuzzy; } set { _fuzzy = value; SetContent(); } }

        /// <summary>
        /// Filter criteria to reduce results returned.
        /// </summary>
        public string? Filter { get { return _filter; } set { _filter = value; SetContent(); } }

        private void SetContent() {

            JObject _content = new() {
                { "entities", _entities },
                { "search", _search},
                { "fuzzy", _fuzzy},
                { "filter", _filter }
            };

            Content = new StringContent(
                    content: _content.ToString(),
                    encoding: System.Text.Encoding.UTF8,
                    mediaType: "application/json");

        }

    }
}