using System.Runtime.Serialization;

namespace PowerApps.Samples.Search.Types;

/// <summary>
/// Data contract for suggest request to Dataverse Search api. 
/// </summary>
public sealed class SuggestParameters
{
    /// <summary>
    /// Gets or sets the search terms.
    /// </summary>
    [DataMember(Name = "search", IsRequired = true)]
    public string Search { get; set; }

    /// <summary>
    /// Gets or sets list of entities to scope the suggest query by listing the tables and their search columns, select columns and filters.
    /// </summary>
    [DataMember(Name = "entities")]
    public List<SearchEntity> Entities { get; set; }

    /// <summary>
    /// Gets or sets the common filters applied on the request.
    /// </summary>
    [DataMember(Name = "filter")]
    public string Filter { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use fuzzy matching for the suggest query.
    /// When set to true, the query will find terms even if there's a substituted or missing character in the search text.
    /// </summary>
    [DataMember(Name = "fuzzy")]
    public bool Fuzzy { get; set; }

    /// <summary>
    /// Gets or sets list of suggest options.
    /// Valid values are: 
    /// { 'advancedsuggestenabled': 'true' } if your suggest request needs to route through advanced algorithm.
    /// </summary>
    [DataMember(Name = "options")]
    public Dictionary<string, string> Options { get; set; }

    /// <summary>
    /// Gets or sets the list of comma-separated expressions to sort the results by. 
    /// Each expression can be followed by "asc" to indicate ascending, and "desc" to indicate descending.
    /// </summary>
    [DataMember(Name = "orderby")]
    public List<string> OrderBy { get; set; }

    /// <summary>
    /// Gets or sets the number of suggest results to be retrieved. 
    /// </summary>
    [DataMember(Name = "top")]
    public int Top { get; set; }
}