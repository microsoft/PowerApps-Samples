using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PowerApps.Samples.Search.Types;

/// <summary>
/// Indicates the status of the customer managed key.
/// </summary>
[DataContract]
[JsonConverter(typeof(StringEnumConverter))]
public enum CMKStatus
{
    /// <summary>
    /// Indicates dataverse search is not provisioned.
    /// </summary>
    [EnumMember]
    Unknown = 0,

    /// <summary>
    /// Indicates customer managed key is disabled.
    /// </summary>
    [EnumMember]
    Disabled = 1,

    /// <summary>
    /// Indicates customer managed key is enabled.
    /// </summary>
    [EnumMember]
    Enabled = 2,

    /// <summary>
    /// Indicates customer managed key is disabling in progress.
    /// </summary>
    [EnumMember]
    DisablingInProgress = 3,

    /// <summary>
    /// Indicates customer managed key is enabling in progress.
    /// </summary>
    [EnumMember]
    EnablingInProgress = 4,
}