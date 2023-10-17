using System.Runtime.Serialization;

namespace PowerApps.Samples.Search.Types;

/// <summary>
/// Specifies the type of a facet query result.
/// </summary>
public enum FacetType
{
    /// <summary>
    /// The facet counts documents with a particular field value.
    /// </summary>
    [EnumMember(Value = "value")]
    Value = 0,

    /// <summary>
    /// The facet counts documents with a field value in a particular range.
    /// </summary>
    [EnumMember(Value = "range")]
    Range = 1,
}