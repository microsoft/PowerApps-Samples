namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Retrieves all security principals (users, teams, or organizations) that have access to, and access rights for, the specified record because it was shared with them.
    /// </summary>
    public sealed class RetrieveSharedPrincipalsAndAccessRequest : HttpRequestMessage
    {
        /// <summary>
        /// Instantiates a RetrieveSharedPrincipalsAndAccessRequest
        /// </summary>
        /// <param name="target">The target record for which to retrieve access rights.</param>
        /// <exception cref="ArgumentException"></exception>
        public RetrieveSharedPrincipalsAndAccessRequest(EntityReference target)
        {
            string path = "RetrieveSharedPrincipalsAndAccess(Target=@p1)";
            string parameters = $"?@p1={target.AsODataId()}";

            Method = HttpMethod.Get;
            RequestUri = new Uri(
                uriString: path + parameters,
                uriKind: UriKind.Relative);
        }
    }
}
