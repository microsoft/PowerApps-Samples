namespace PowerApps.Samples.Search.Types
{
    /// <summary>
    /// Contains data about a matching record found from the query API.
    /// </summary>
    public sealed class QueryResult
    {
        /// <summary>
        /// Gets or sets the identifier of the record
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the logical name of the table
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// Gets or sets the object type code
        /// </summary>
        public int ObjectTypeCode { get; set; }

        /// <summary>
        /// Gets or sets the record attributes
        /// </summary>
        public Dictionary<string, object> Attributes { get; set; }

        /// <summary>
        /// Gets or sets the highlights
        /// </summary>
        public Dictionary<string, string[]> Highlights { get; set; }

        // Gets or sets the document score
        public double Score { get; set; }
    }
}
