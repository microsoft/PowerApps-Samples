namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to disassociate a record
    /// </summary>
    public sealed class DisassociateRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the DisassociateRequest
        /// </summary>
        /// <param name="entityWithCollection">A record with a collection.</param>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="entityToRemove">The record to remove.</param>
        public DisassociateRequest(
            EntityReference entityWithCollection,
            string collectionName,
            EntityReference entityToRemove)
        {
            Method = HttpMethod.Delete;
            RequestUri = new Uri(
                uriString: $"{entityWithCollection.Path}/{collectionName}({entityToRemove.Id})/$ref", 
                uriKind: UriKind.Relative);
        }
    }
}
