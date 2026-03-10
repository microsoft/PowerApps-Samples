using Newtonsoft.Json;
using PowerApps.Samples.Types;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MetadataConditionExpression
    {
        public MetadataConditionExpression() { }

        public MetadataConditionExpression(MetadataConditionOperator conditionOperator, string propertyName, Samples.Types.Object value)
        {
            ConditionOperator = conditionOperator;
            PropertyName = propertyName;
            Value = value;
        }

        public MetadataConditionOperator ConditionOperator { get; set; }
        public string PropertyName { get; set; }
        public Samples.Types.Object Value { get; set; }
    }
}
