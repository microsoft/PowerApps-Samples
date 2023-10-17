using System.Runtime.Serialization;

namespace PowerApps.Samples.Search.Types;

/// <summary>
/// Data contract for query request to Dataverse Search api. 
/// </summary>
public sealed class QueryParameters
{
    /// <summary>
    /// Gets or sets the search terms. These special characters must be escaped using '\' : + - & | ! ( ) { } [ ] ^ " ~ * ? : \ /
    /// </summary>
    [DataMember(Name = "search", IsRequired = true)]
    public string Search { get; set; }

    /// <summary>
    /// Gets or sets list of entities to scope the search query by listing the tables and their search columns, select columns and filters.
    /// </summary>
    [DataMember(Name = "entities")]
    public List<SearchEntity> Entities { get; set; }

    /// <summary>
    /// Gets or sets list of columns to facet by. 
    /// The string may contain parameters to customize the faceting, expressed as comma-separated name-value pairs.
    /// </summary>
    [DataMember(Name = "facets")]
    public List<string> Facets { get; set; }

    /// <summary>
    /// Gets or sets the common filters applied on the request.
    /// </summary>
    [DataMember(Name = "filter")]
    public string Filter { get; set; }

    /// <summary>
    /// Gets or sets list of search options.
    /// Valid values are:
    /// {'querytype': 'lucene', 'searchmode': 'all', 'besteffortsearchenabled': 'true', 'grouprankingenabled': 'true'} 
    /// "querytype": Values can be `simple` or `lucene`. Use Lucene Query parser for specialized query forms.
    /// "besteffortsearchenabled": Enable intelligent query workflow to return probable set of results if no good matches are found for the search request terms.
    /// "groupranking": Ranking of results in the response will be optimized for display in search results pages where results are grouped by table.
    /// "searchmode": Specifies whether all the search terms must be matched in order to consider the document as a match. Not specifying this flag will default to matching any word in the search term. 
    /// </summary>
    [DataMember(Name = "options")]
    public Dictionary<string, string> Options { get; set; }

    /// <summary>
    /// Gets or sets the list of comma-separated expressions to sort the results by. 
    /// Each expression can be followed by "asc" to indicate ascending, and "desc" to indicate descending.
    /// Only works with Lucene query syntax.
    /// </summary>
    [DataMember(Name = "orderby")]
    public List<string> OrderBy { get; set; }

    /// <summary>
    /// Gets or sets the Count field which specifies whether to fetch the total count of results. 
    /// This is the count of all documents that match the search, ignoring top and skip. 
    /// </summary>
    [DataMember(Name = "count")]
    public bool Count { get; set; }

    /// <summary>
    /// Gets or sets the number of search results to skip. 
    /// </summary>
    [DataMember(Name = "skip")]
    public int Skip { get; set; }

    /// <summary>
    /// Gets or sets the number of search results to retrieve. 
    /// </summary>
    [DataMember(Name = "top")]
    public int Top { get; set; }
}