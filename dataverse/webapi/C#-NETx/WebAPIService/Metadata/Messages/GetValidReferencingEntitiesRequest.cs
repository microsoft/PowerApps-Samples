namespace PowerApps.Samples.Metadata.Messages
{
    /// <summary>
    /// Contains the data to retrieve the set of entities that are valid as the related entity (many) to the specified entity in a one-to-many relationship.
    /// </summary>
    public sealed class GetValidReferencingEntitiesRequest : HttpRequestMessage
    {
        /// <summary>
        /// Returns an HttpRequestMessage for the GetValidReferencingEntities Function
        /// </summary>
        /// <param name="referencedEntityName">The name of the primary entity in the relationship</param>
        public GetValidReferencingEntitiesRequest(
            string referencedEntityName)
        {
            Method = HttpMethod.Get;
            RequestUri = new Uri(
                uriString: $"GetValidReferencingEntities(ReferencedEntityName='{referencedEntityName}')",
                uriKind: UriKind.Relative);
        }
    }
}

