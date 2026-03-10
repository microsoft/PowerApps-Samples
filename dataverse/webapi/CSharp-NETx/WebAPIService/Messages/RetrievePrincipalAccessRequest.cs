namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Retrieves the access rights of the specified security principal (team or user) to the specified record.
    /// </summary>
    public sealed class RetrievePrincipalAccessRequest : HttpRequestMessage
    {
        /// <summary>
        /// Instantiates a RetrievePrincipalAccessRequest
        /// </summary>
        /// <param name="principal">The principal to retrieve access.</param>
        /// <param name="target">The target record for which to retrieve access rights.</param>
        /// <exception cref="ArgumentException"></exception>
        public RetrievePrincipalAccessRequest(EntityReference principal, EntityReference target)
        {
            // RetrievePrincipalAccess is bound to these three entity sets
            string[] validEntitySets = { "systemusers", "teams", "organizations"};

            if (!validEntitySets.Contains(principal.SetName))
            {
                throw new ArgumentException($"RetrievePrincipalAccessRequest.principal " +
                    $"must be one of these entity sets: {string.Join(",",validEntitySets)}.");
            }

            string path = $"{principal.Path}/Microsoft.Dynamics.CRM.RetrievePrincipalAccess(Target=@p1)";
            string parameters = $"?@p1={target.AsODataId()}";

            Method = HttpMethod.Get;
            RequestUri = new Uri(
                uriString:path+parameters, 
                uriKind: UriKind.Relative);
        }     
    }
}
