namespace PowerApps.Samples.Metadata.Messages
{
    /// <summary>
    /// Contains the data to retrieves a list of all the entities that can participate in a Many-to-Many entity relationship.
    /// </summary>
    public sealed class GetValidManyToManyRequest : HttpRequestMessage
    {
        /// <summary>
        /// Returns an HttpRequestMessage for the GetValidManyToMany Function
        /// </summary>
        public GetValidManyToManyRequest()
        {
            Method = HttpMethod.Get;
            RequestUri = new Uri(
                uriString: "GetValidManyToMany",
                uriKind: UriKind.Relative);
        }
    }
}
