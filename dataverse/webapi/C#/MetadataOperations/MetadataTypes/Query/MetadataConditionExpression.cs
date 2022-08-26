using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata.Query
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MetadataConditionExpression
    {
        public MetadataConditionOperator ConditionOperator { get; set; }
        public string PropertyName { get; set; }
        public Object Value { get; set; }
    }
}