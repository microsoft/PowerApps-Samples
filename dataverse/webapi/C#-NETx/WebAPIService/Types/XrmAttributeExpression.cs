namespace PowerApps.Samples.Types
{
    /// <summary>
    /// Contains data to specify aggregation.
    /// </summary>
    public class XrmAttributeExpression
    {
        /// <summary>
        /// The type of aggregation.
        /// </summary>
        public XrmAggregateType AggregateType { get; set; }
        /// <summary>
        /// The alias to use.
        /// </summary>
        public string? Alias { get; set; }
        /// <summary>
        /// The name of the attribute.
        /// </summary>
        public string? AttributeName { get; set; }
        /// <summary>
        /// The datetime grouping to use.
        /// </summary>
        public XrmDateTimeGrouping DateTimeGrouping { get; set; }
        /// <summary>
        /// Whether the aggregation has a group by.
        /// </summary>
        public bool? HasGroupBy { get; set; }
    }
}
