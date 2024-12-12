using System.Net.Http.Headers;

namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to retrieve the count of a collection up to 5000.
    /// </summary>
    public sealed class GetCollectionCountRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the GetCollectionCountRequest
        /// </summary>
        /// <param name="collectionPath">The path to the collection to count</param>
        public GetCollectionCountRequest(string collectionPath)
        {
            Method = HttpMethod.Get;
            RequestUri = new Uri(
                uriString: $"{collectionPath}/$count",
                uriKind: UriKind.Relative);
        }
    }
}
