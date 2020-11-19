using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OptionSetType
    {
        Picklist,
        State,
        Status,
        Boolean
    }
}