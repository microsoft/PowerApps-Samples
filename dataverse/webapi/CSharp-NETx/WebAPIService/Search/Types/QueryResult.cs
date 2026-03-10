namespace PowerApps.Samples.Search.Types
{
    /// <summary>
    /// Contains data about a matching record found from the query API
    /// </summary>
    public class QueryResult
    {
        // Gets or sets the identifier of the record
        public string Id { get; set; }

        // Gets or sets the logical name of the table
        public string EntityName { get; set; }

        // Gets or sets the object type code
        public int ObjectTypeCode { get; set; }

        // Gets or sets the record attributes
        public Dictionary<string, object> Attributes { get; set; }

        // Gets or sets the highlights
        public Dictionary<string, string[]> Highlights { get; set; }

        // Gets or sets the document score
        public double Score { get; set; }
    }
}
