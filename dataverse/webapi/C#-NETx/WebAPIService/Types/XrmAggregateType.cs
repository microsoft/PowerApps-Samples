using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace PowerApps.Samples.Types
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum XrmAggregateType
    {
        None,
        Count,
        CountColumn,
        Sum,
        Avg,
        Min,
        Max
    }
}
