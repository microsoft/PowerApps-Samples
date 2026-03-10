using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PowerApps.Samples.Search.Types;

/// <summary>
/// Indicates the status of the lockbox.
/// </summary>
[DataContract]
[JsonConverter(typeof(StringEnumConverter))]
public enum LockboxStatus
{
    /// <summary>
    /// Indicates dataverse search is not provisioned.
    /// </summary>
    [EnumMember]
    Unknown = 0,

    /// <summary>
    /// Indicates lockbox is disabled.
    /// </summary>
    [EnumMember]
    Disabled = 1,

    /// <summary>
    /// Indicates lockbox is enabled.
    /// </summary>
    [EnumMember]
    Enabled = 2,

    /// <summary>
    /// Indicates lockbox is disabling in progress.
    /// </summary>
    [EnumMember]
    DisablingInProgress = 3,

    /// <summary>
    /// Indicates lockbox is enabling in progress.
    /// </summary>
    [EnumMember]
    EnablingInProgress = 4,
}