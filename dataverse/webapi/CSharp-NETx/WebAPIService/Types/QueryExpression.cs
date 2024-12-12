namespace PowerApps.Samples.Types
{
    public class QueryExpression
    {

        public QueryExpression(string entityName)
        {

            EntityName = entityName;
        }

        public ColumnSet ColumnSet { get; set; }
        public FilterExpression Criteria { get; set; }
        public string DataSource { get; set; }
        public bool Distinct { get; set; }
        public string EntityName { get; set; }
        public List<LinkEntity> LinkEntities { get; set; } = new List<LinkEntity>();
        public bool NoLock { get; set; }
        public List<OrderExpression> Orders { get; set; } = new List<OrderExpression>();
        public PagingInfo PageInfo { get; set; }
        public string QueryHints { get; set; }
        public QueryExpression SubQueryExpression { get; set; }
        public int TopCount { get; set; }
    }
}
