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
        /// <param name="entityMetadata">The data that defines the table</param>
        /// <param name="solutionUniqueName">The name of the solution to add the table to.</param>
        public CreateEntityKeyRequest(EntityKeyMetadata entityKeyMetadata, Guid entityId, bool useStrongConsistency = false)
        {
            Method = HttpMethod.Post;
            //TODO: This doesn't need to use the metadataid, it can use the LogicalName
            // EntityDefinitions(LogicalName='{tableLogicalName}')
            RequestUri = new Uri(uriString: $"EntityDefinitions({entityId})/Keys", uriKind: UriKind.Relative);
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
