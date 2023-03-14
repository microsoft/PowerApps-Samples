using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Types
{
    /// <summary>
    /// An expression used to filter the results of the query.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ConditionExpression
    {
        /// <summary>
        /// The logical name of the attribute in the condition expression.
        /// </summary>
        public string? AttributeName { get; set; }
        /// <summary>
        /// Whether to compare column values.
        /// </summary>
        public bool CompareColumns { get; set; }
        /// <summary>
        /// The entity name for the condition.
        /// </summary>
        public string? EntityName { get; set; }
        /// <summary>
        /// The condition operator.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))] 
        public ConditionOperator Operator { get; set; }
        /// <summary>
        /// The values for the attribute.
        /// </summary>
        public List<Object> Values { get; set; } = new List<Object>();

    }
}
