using Newtonsoft.Json;

namespace PowerApps.Samples.Types
{
    /// <summary>
    /// The values of the specified column should be sorted in descending order, from highest to lowest.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class OrderExpression
    {

        public string? Alias { get; set; }
        /// <summary>
        /// The name of the column in the order expression.
        /// </summary>
        public string? AttributeName { get; set;}
        /// <summary>
        /// The name of the table in the order expression.
        /// </summary>
        public string? EntityName { get; set; }
        /// <summary>
        /// The order, ascending or descending.
        /// </summary>
        public OrderType OrderType { get; set; } 
    }
}
