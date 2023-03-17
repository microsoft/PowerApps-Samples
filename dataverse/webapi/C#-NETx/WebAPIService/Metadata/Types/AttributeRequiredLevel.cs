using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata.Types
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