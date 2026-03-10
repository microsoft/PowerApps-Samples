namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to send the IsSystemAdminRequest
    /// This is a Function defined by a Custom API
    /// See https://learn.microsoft.com/power-apps/developer/data-platform/org-service/samples/issystemadmin-customapi-sample-plugin
    /// </summary>
    public sealed class IsSystemAdminRequest : HttpRequestMessage
    {
        public IsSystemAdminRequest(Guid systemUserId)
        {
            Method = HttpMethod.Get;
            RequestUri = new Uri(
                uriString: $"systemusers({systemUserId})/Microsoft.Dynamics.CRM.sample_IsSystemAdmin", 
                uriKind: UriKind.Relative);
        }
    }
}
