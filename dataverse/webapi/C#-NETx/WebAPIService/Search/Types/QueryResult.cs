namespace PowerApps.Samples.Search.Types
{
    /// <summary>
    /// Contains data about a matching record found from the query API
    /// </summary>
    public class QueryResult
    {
        public string Id { get; set; }
        public string EntityName { get; set; }

        public int ObjectTypeCode { get; set; }

        public Dictionary<string, object> Attributes { get; set; }

        public Dictionary<string, string[]> Highlights { get; set; }

        public double Score { get; set; }
    }
}
