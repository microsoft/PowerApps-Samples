using Newtonsoft.Json.Linq;
using System.Text;

namespace PowerApps.Samples.Metadata.Messages
{
    /// <summary>
    /// Contains the data to determine whether whether an entity can participate in a many-to-many relationship.
    /// </summary>
    public sealed class CanManyToManyRequest : HttpRequestMessage
    {
        /// <summary>
        /// Returns an HttpRequestMessage for the CanManyToMany Action
        /// </summary>
        /// <param name="entityName">Logical entity name.</param>
        public CanManyToManyRequest(string entityName)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "CanManyToMany",
                uriKind: UriKind.Relative);
            Content = new StringContent(
                content: new JObject() { { "EntityName", entityName } }.ToString(),
                encoding: Encoding.UTF8,
                mediaType: "application/json");
        }
    }
}
