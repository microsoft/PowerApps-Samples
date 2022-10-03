using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
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
