using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace PowerApps.Samples.Metadata.Query
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MetadataFilterExpression
    {
        public List<MetadataConditionExpression> Conditions { get; set; }
        public LogicalOperator FilterOperator { get; set; }
        public List<MetadataFilterExpression> Filters { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum LogicalOperator
    {
        And, Or
    }
}