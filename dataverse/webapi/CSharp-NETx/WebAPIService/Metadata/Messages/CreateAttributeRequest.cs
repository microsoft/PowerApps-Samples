using Newtonsoft.Json.Linq;
using PowerApps.Samples.Metadata.Types;
using System.Text;

namespace PowerApps.Samples.Metadata.Messages
{
    /// <summary>
    /// Contains the data to update a column definitions
    /// </summary>
    public sealed class CreateAttributeRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the CreateAttributeRequest
        /// </summary>
        /// <param name="entityLogicalName">The logical name of the table that will contain the column</param>
        /// <param name="attributeMetadata">The data defining the column</param>
        /// <param name="solutionUniqueName">The name of the solution to add the column to.</param>
        public CreateAttributeRequest(
            string entityLogicalName, 
            AttributeMetadata attributeMetadata, 
            string? solutionUniqueName = null)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: $"EntityDefinitions(LogicalName='{entityLogicalName}')/Attributes", 
                uriKind: UriKind.Relative);

            if (!string.IsNullOrWhiteSpace(solutionUniqueName))
            {
                Headers.Add("MSCRM.SolutionUniqueName", solutionUniqueName);
            }
            Content = new StringContent(
                content: JObject.FromObject(attributeMetadata).ToString(),
                encoding: Encoding.UTF8,
                mediaType: "application/json");
        }
    }
}
