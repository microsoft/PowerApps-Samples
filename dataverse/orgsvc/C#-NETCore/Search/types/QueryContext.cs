using System.Runtime.Serialization;

namespace PowerApps.Samples.Search.Types;

/// <summary>
/// The query context returned as part of response. This request is used for backend search, this is included for future feature releases, it is not currently used.
/// </summary>
public sealed class QueryContext
{
    /// <summary>
    /// Gets or sets the query string as specified in the request.
    /// </summary>
    [DataMember(Name = "originalquery")]
    public string OriginalQuery { get; set; }

    /// <summary>
    /// Gets or sets the query string that Dataverse search used to perform the query. 
    /// Dataverse search uses the altered query string if the original query string contained spelling mistakes or did not yield optimal results.
    /// </summary>
    [DataMember(Name = "alteredquery")]
    public string AlteredQuery { get; set; }

    /// <summary>
    /// Gets or sets the reason behind query alter decision by Dataverse search.
    /// </summary>
    [DataMember(Name = "reason")]
    public List<string> Reason { get; set; }

    /// <summary>
    /// Gets or sets the spell suggestion that are the likely words that represent user’s intent. 
    /// This will be populated only when the query was altered by Dataverse search due to spell check.
    /// </summary>
    [DataMember(Name = "spellsuggestions")]
    public List<string> SpellSuggestions { get; set; }
}