using Newtonsoft.Json;

namespace PowerApps.Samples.Types
{
    /// <summary>
    /// Specifies the attributes for which non-null values are returned from a query.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ColumnSet
    {
        public ColumnSet(params string[] columns)
        {
            if (columns != null || columns.Length > 0)
            {
                Columns = new List<string>(columns);
            }
        }
        /// <summary>
        /// Whether to retrieve all the attributes of a record.
        /// </summary>
        public bool AllColumns { get; set; }

        public List<XrmAttributeExpression> AttributeExpressions { get; set; } 

        /// <summary>
        /// The collection of strings containing the names of the columns to be retrieved from a query.
        /// </summary>
        public List<string> Columns { get; set; } 
    }
}
