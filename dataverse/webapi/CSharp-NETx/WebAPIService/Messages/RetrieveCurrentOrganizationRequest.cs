using PowerApps.Samples.Types;

namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to Retrieve information about the current organization.
    /// </summary>
    public sealed class RetrieveCurrentOrganizationRequest : HttpRequestMessage
    {

        /// <summary>
        /// Initializes the RetrieveCurrentOrganizationRequest
        /// </summary>
        /// <param name="AccessType">The access type of the organization’s service endpoint.</param>
        public RetrieveCurrentOrganizationRequest(EndpointAccessType accessType)
        {
            Method = HttpMethod.Get;
            RequestUri = new Uri(
                uriString: $"RetrieveCurrentOrganization(AccessType=@p1)" +
                $"?@p1=Microsoft.Dynamics.CRM.EndpointAccessType'{accessType}'", 
                uriKind: UriKind.Relative);
        }
    }
}