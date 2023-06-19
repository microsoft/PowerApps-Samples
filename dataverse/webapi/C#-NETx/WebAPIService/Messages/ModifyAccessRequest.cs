using Newtonsoft.Json.Linq;
using PowerApps.Samples.Types;

namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to modify the access granted to a security principal (user, team, or organization) for the specified record.
    /// </summary>
    public sealed class ModifyAccessRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the ModifyAccessRequest
        /// </summary>
        /// <param name="target">The record to modify access for.</param>
        /// <param name="principalAccess">The security principal (user, team, or organization) that is granted access to the specified record and the access rights to modify.</param>
        public ModifyAccessRequest(JObject target, PrincipalAccess principalAccess)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "ModifyAccess",
                uriKind: UriKind.Relative);

            JObject _content = new() {
                { "Target", target},
                { "PrincipalAccess", JObject.FromObject(principalAccess) }
            };

            Content = new StringContent(
                    content: _content.ToString(),
                    encoding: System.Text.Encoding.UTF8,
                    mediaType: "application/json");
        }
    }
}