using System.Runtime.Serialization;

namespace PowerApps.Samples.Search.Types;

/// <summary>
/// The error detail returned as part of response.
/// </summary>
public sealed class ErrorDetail
{
    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    [DataMember(Name = "code")]
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    [DataMember(Name = "message")]
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets additional error information.
    /// </summary>
    [DataMember(Name = "propertybag")]
    public Dictionary<string, object> PropertyBag { get; set; }
}