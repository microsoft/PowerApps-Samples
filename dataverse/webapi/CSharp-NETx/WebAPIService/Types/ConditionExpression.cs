namespace PowerApps.Samples.Types
{
    public class ConditionExpression
    {
        public ConditionExpression(string entityName, string attributeName, ConditionOperator conditionOperator, List<Object> values)
        {
            EntityName = entityName;
            AttributeName = attributeName;
            Operator = conditionOperator;
            Values = values;
        }

        public string AttributeName { get; set; }
        public bool CompareColumns { get; set; }
        public string EntityName { get; set; }
        public ConditionOperator Operator { get; set; }
        public List<Object> Values { get; set; }
    }
}
