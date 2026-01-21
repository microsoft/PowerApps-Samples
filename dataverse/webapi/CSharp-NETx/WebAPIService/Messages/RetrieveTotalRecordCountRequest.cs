using Newtonsoft.Json;

namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to returns data on the total number of records for specific entities. The data retrieved will be from a snapshot within last 24 hours.
    /// </summary>
    public sealed class RetrieveTotalRecordCountRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the RetrieveTotalRecordCountRequest
        /// </summary>
        /// <param name="entityNames">The logical names of the entities to include in the query.</param>
        public RetrieveTotalRecordCountRequest(string[] entityNames)
        {
            Method = HttpMethod.Get;
            RequestUri = new Uri(
                uriString: $"RetrieveTotalRecordCount(EntityNames=@p1)" +
                $"?@p1={JsonConvert.SerializeObject(entityNames)}",
                uriKind: UriKind.Relative);
        }
    }
}