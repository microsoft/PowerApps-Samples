using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LayerDesiredOrderType
    {
        Below,Above,Base
    }
}
