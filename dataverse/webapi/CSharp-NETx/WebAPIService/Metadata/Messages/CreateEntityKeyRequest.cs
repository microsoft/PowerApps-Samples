using Newtonsoft.Json.Linq;
using PowerApps.Samples.Metadata.Types;
using System.Text;

namespace PowerApps.Samples.Metadata.Messages
{
    /// <summary>
    /// Contains the data to create a table
    /// </summary>
    public sealed class CreateEntityKeyRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the CreateEntityKeyRequest
        /// </summary>
        /// <param name="entityKeyMetadata">The data that defines the entity key</param>
        /// <param name="tableLogicalName">The LogicalName of the table.</param>
        public CreateEntityKeyRequest(EntityKeyMetadata entityKeyMetadata, string tableLogicalName, bool useStrongConsistency = false)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(uriString: $"EntityDefinitions(LogicalName='{tableLogicalName}')/Keys", uriKind: UriKind.Relative);
            if (useStrongConsistency)
            {
                Headers.Add("Consistency", "Strong");
            }
            Content = new StringContent(
                content: JObject.FromObject(entityKeyMetadata).ToString(),
                encoding: Encoding.UTF8,
                mediaType: "application/json");
        }
    }
}
