using Newtonsoft.Json;

namespace PowerApps.Samples.Types
{
    /// <summary>
    /// Specifies complex condition and logical filter expressions used for filtering the results of the query.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class FilterExpression
    {
        /// <summary>
        /// Condition expressions that include attributes, condition operators, and attribute values.
        /// </summary>
        public List<ConditionExpression> Conditions { get; set; } 


        public string? FilterHint { get; set; }

        /// <summary>
        /// The logical AND/OR filter operator.
        /// </summary>
        public LogicalOperator FilterOperator { get; set; }
        /// <summary>
        /// A collection of condition and logical filter expressions that filter the results of the query.
        /// </summary>
        public List<FilterExpression> Filters { get; set; } 

        /// <summary>
        /// Indicates whether the expression is part of a quick find query.
        /// </summary>
        public bool IsQuickFindFilter { get; set; }
    }
}
