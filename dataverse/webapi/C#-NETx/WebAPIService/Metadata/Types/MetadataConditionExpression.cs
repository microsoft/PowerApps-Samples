using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MetadataConditionExpression
    {
        public MetadataConditionExpression() { }

        public MetadataConditionExpression(MetadataConditionOperator conditionOperator, string propertyName, Object value)
        {
            ConditionOperator = conditionOperator;
            PropertyName = propertyName;
            Value = value;
        }

        public MetadataConditionOperator ConditionOperator { get; set; }
        public string PropertyName { get; set; }
        public Object Value { get; set; }
    }
}
