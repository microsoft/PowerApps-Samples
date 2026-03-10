using Newtonsoft.Json.Linq;
using PowerApps.Samples.Metadata.Types;
using System.Text;

namespace PowerApps.Samples.Metadata.Messages
{
    /// <summary>
    /// Contains the data to create a table
    /// </summary>
    public sealed class CreateEntityRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the CreateEntityRequest
        /// </summary>
        /// <param name="entityMetadata">The data that defines the table</param>
        /// <param name="solutionUniqueName">The name of the solution to add the table to.</param>
        public CreateEntityRequest(EntityMetadata entityMetadata, string? solutionUniqueName = null, bool useStrongConsistency = false)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(uriString: "EntityDefinitions", uriKind: UriKind.Relative);
            if (!string.IsNullOrWhiteSpace(solutionUniqueName))
            {
                Headers.Add("MSCRM.SolutionUniqueName", solutionUniqueName);
            }
            if (useStrongConsistency)
            {
                Headers.Add("Consistency", "Strong");
            }
            Content = new StringContent(
                content: JObject.FromObject(entityMetadata).ToString(),
                encoding: Encoding.UTF8,
                mediaType: "application/json");
        }
    }
}
