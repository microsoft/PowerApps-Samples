using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AttributeRequiredLevel
    {
        None = 0,
        SystemRequired,
        ApplicationRequired,
        Recommended
    }
}