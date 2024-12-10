using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace PowerApps.Samples.Search.Types;

/// <summary>
/// Data contract for response parameters for index status api.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class FieldStatusInfo
{
    /// <summary>
    /// Gets or sets index field name.
    /// </summary>
    [JsonProperty(PropertyName = "indexfieldname")]
    public string IndexFieldName { get; set; }
}