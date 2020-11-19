using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata.Query
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MetadataConditionOperator
    {
        Equals,
        NotEquals,
        In,
        NotIn,
        GreaterThan,
        LessThan
    }
}