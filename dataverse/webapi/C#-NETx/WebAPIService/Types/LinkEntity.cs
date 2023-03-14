using Newtonsoft.Json;
using System.Xml.Linq;

namespace PowerApps.Samples.Types
{
    /// <summary>
    /// Specifies the links between multiple entity types used in creating complex queries.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class LinkEntity
    {
        /// <summary>
        /// The column set.
        /// </summary>
        public ColumnSet Columns { get; set; }
        /// <summary>
        /// The alias for the entity.
        /// </summary>
        public string? EntityAlias { get; set; }
        /// <summary>
        /// The join operator.
        /// </summary>
        public JoinOperator JoinOperator { get; set; }
        /// <summary>
        /// The complex condition and logical filter expressions that filter the results of the query.
        /// </summary>
        public FilterExpression LinkCriteria { get; set; }
        /// <summary>
        /// The links between multiple entity types.
        /// </summary>
        public List<LinkEntity> LinkEntities { get; set; } = new List<LinkEntity>();
        /// <summary>
        /// The logical name of the attribute of the entity that you are linking from.
        /// </summary>
        public string? LinkFromAttributeName { get; set; }
        /// <summary>
        /// The logical name of the attribute of the entity that you are linking from.
        /// </summary>
        public string? LinkFromEntityName { get; set; }
        /// <summary>
        /// The logical name of the attribute of the entity that you are linking to.
        /// </summary>
        public string? LinkToAttributeName { get;set; }
        /// <summary>
        /// The logical name of the attribute of the entity that you are linking to.
        /// </summary>
        public string? LinkToEntityName { get; set;}
        /// <summary>
        /// The order expressions to apply to the query.
        /// </summary>
        public List<OrderExpression> Orders { get; set; } = new List<OrderExpression>();
    }
}
