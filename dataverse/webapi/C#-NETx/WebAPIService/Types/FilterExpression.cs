namespace PowerApps.Samples.Types
{
    public class FilterExpression
    {
        public FilterExpression(LogicalOperator logicalOperator)
        {
            FilterOperator = logicalOperator;
        }
        public List<ConditionExpression> Conditions { get; set; } = new List<ConditionExpression>();
        public string FilterHint { get; set; }
        public LogicalOperator FilterOperator { get; set; }
        public List<FilterExpression> Filters { get; set; } = new List<FilterExpression>();
        public bool IsQuickFindFilter { get; set; }
    }
}
