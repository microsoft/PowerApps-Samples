using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SecurityTypes
    {
        None,
        Append,
        ParentChild,
        Pointer = 4,
        Inheritance = 8
    }
}