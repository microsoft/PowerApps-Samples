using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PrivilegeType
    {
        None, Create, Read, Write, Delete, Assign, Share, Append, AppendTo
    }
}