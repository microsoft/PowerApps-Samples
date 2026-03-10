using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace PowerApps.Samples.Search.Types;

/// <summary>
/// Indicates the status of an entity type and/or the index as a whole.
/// </summary>
[DataContract]
[JsonConverter(typeof(StringEnumConverter))]
public enum SearchStatus
{
    /// <summary>
    /// Organization is not provisioned for search.
    /// </summary>
    [EnumMember(Value = "notprovisioned")]
    NotProvisioned = 0,

    /// <summary>
    /// Organization provisioning in progress.
    /// </summary>
    [EnumMember(Value = "provisioninginprogress")]
    ProvisioningInProgress = 1,

    /// <summary>
    /// Organization provisioned for search.
    /// </summary>
    [EnumMember(Value = "provisioned")]
    Provisioned = 2,
}