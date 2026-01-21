using Newtonsoft.Json.Linq;
using System.Text;

namespace PowerApps.Samples.Metadata.Messages
{
    /// <summary>
    /// Contains the data to determine whether the specified entity can be the primary entity (one) in a one-to-many relationship.
    /// </summary>
    public sealed class CanBeReferencedRequest : HttpRequestMessage
    {
        /// <summary>
        /// Returns an HttpRequestMessage for the CanBeReferenced Action
        /// </summary>
        /// <param name="entityName">Logical entity name.</param>
        public CanBeReferencedRequest(string entityName)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "CanBeReferenced", 
                uriKind: UriKind.Relative);
            Content = new StringContent(
                content: new JObject() { { "EntityName", entityName } }.ToString(),
                encoding: Encoding.UTF8,
                mediaType: "application/json");
        }
    }
}
