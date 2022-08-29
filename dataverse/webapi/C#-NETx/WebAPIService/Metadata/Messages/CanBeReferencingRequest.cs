using Newtonsoft.Json.Linq;
using System.Text;

namespace PowerApps.Samples.Metadata.Messages
{
    /// <summary>
    /// Contains the data to determine whether an entity can be the referencing entity in a one-to-many relationship.
    /// </summary>
    public sealed class CanBeReferencingRequest : HttpRequestMessage
    {
        /// <summary>
        /// Returns an HttpRequestMessage for the CanBeReferencing Action
        /// </summary>
        /// <param name="entityName">Logical entity name.</param>
        public CanBeReferencingRequest(string entityName)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "CanBeReferencing",
                uriKind: UriKind.Relative);
            Content = new StringContent(
                content: new JObject() { { "EntityName", entityName } }.ToString(),
                encoding: Encoding.UTF8,
                mediaType: "application/json");
        }
    }
}
