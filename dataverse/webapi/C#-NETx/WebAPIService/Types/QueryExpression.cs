using Newtonsoft.Json;

namespace PowerApps.Samples.Types
{
    /// <summary>
    /// A complex query expressed in a hierarchy of expressions.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class QueryExpression
    {
        /// <summary>
        /// The columns to include.
        /// </summary>
        public ColumnSet? ColumnSet { get; set; }
        /// <summary>
        /// The complex condition and logical filter expressions that filter the results of the query.
        /// </summary>
        public FilterExpression? Criteria { get; set; }
      
        public string? DataSource { get; set; }
        /// <summary>
        /// Whether the results of the query contain duplicate entity instances.
        /// </summary>
        public bool Distinct { get; set; }
        /// <summary>
        /// The logical name of the entity.
        /// </summary>
        public string? EntityName { get; set; }
        /// <summary>
        /// A collection of the links between multiple entity types.
        /// </summary>
        public List<LinkEntity> LinkEntities { get; set; }
        /// <summary>
        /// Indicates that no shared locks are issued against the data that would prohibit other transactions from modifying the data in the records returned from the query.
        /// </summary>
        public bool NoLock { get; set; }
        /// <summary>
        /// The order in which the entity instances are returned from the query.
        /// </summary>
        public List<OrderExpression> Orders { get; set; }
        /// <summary>
        /// The number of pages and the number of entity instances per page returned from the query.
        /// </summary>
        public PagingInfo? PageInfo { get; set; }
        /// <summary>
        /// A hint for generated SQL text which affects the query's execution.
        /// </summary>
        public string? QueryHints { get; set; }

        public QueryExpression? SubQueryExpression { get; set; }
        /// <summary>
        /// The number of rows to be returned.
        /// </summary>
        public int TopCount { get; set; }

    }
}
