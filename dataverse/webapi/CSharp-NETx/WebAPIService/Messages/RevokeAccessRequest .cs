using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to revoke the shared access granted to a security principal (user, team, or organization) for the specified record.
    /// </summary>
    public sealed class RevokeAccessRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the RevokeAccessRequest
        /// </summary>
        /// <param name="target">The record to revoke access for.</param>
        /// <param name="principalAccess">The security principal (user, team, or organization) to revoke access to the shared record.</param>
        public RevokeAccessRequest(JObject target, JObject principal)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "RevokeAccess",
                uriKind: UriKind.Relative);

            JObject _content = new() {
                { "Target", target},
                { "Revokee", principal }
            };

            Content = new StringContent(
                    content: _content.ToString(),
                    encoding: System.Text.Encoding.UTF8,
                    mediaType: "application/json");
        }
    }
}