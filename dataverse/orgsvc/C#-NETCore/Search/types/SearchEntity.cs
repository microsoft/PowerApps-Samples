using System.Runtime.Serialization;

namespace PowerApps.Samples.Search.Types;

/// <summary>
/// The entity schema to scope the search request.
/// </summary>
public sealed class SearchEntity
{
    /// <summary>
    /// Gets or sets the logical name of the table. Specifies scope of the query.
    /// </summary>
    [DataMember(Name = "name", IsRequired = true)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the list of columns that needs to be projected when table documents are returned in response. 
    /// If empty, only PrimaryName will be returned.
    /// </summary>
    [DataMember(Name = "selectcolumns")]
    public List<string> SelectColumns { get; set; }

    /// <summary>
    /// Gets or sets the list of columns to scope the query on.
    /// If empty, only PrimaryName will be searched on. 
    /// </summary>
    [DataMember(Name = "searchcolumns")]
    public List<string> SearchColumns { get; set; }

    /// <summary>
    /// Gets or sets the filters applied on the entity.
    /// </summary>
    [DataMember(Name = "filter")]
    public string Filter { get; set; }
}