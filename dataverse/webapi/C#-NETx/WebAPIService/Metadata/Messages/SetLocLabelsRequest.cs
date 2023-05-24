using Newtonsoft.Json.Linq;
using PowerApps.Samples.Metadata.Types;
using System.Text;

namespace PowerApps.Samples.Metadata.Messages
{
    /// <summary>
    /// Contains the data that is needed to set localized labels for a limited set of entity attributes.
    /// </summary>
    public sealed class SetLocLabelsRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the SetLocLabelsRequest
        /// </summary>
        /// <param name="entityMoniker">Reference to the item</param>
        /// <param name="attributeName">Name of the property</param>
        /// <param name="labels">The labels to set.</param>
        /// <param name="solutionUniqueName">The name of the solution.</param>
        public SetLocLabelsRequest(JObject entityMoniker, string attributeName, LocalizedLabel[] labels, string? solutionUniqueName = null)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(uriString: "SetLocLabels", uriKind: UriKind.Relative);
            if (!string.IsNullOrWhiteSpace(solutionUniqueName))
            {
                Headers.Add("MSCRM.SolutionUniqueName", solutionUniqueName);
            }

            JObject content = new()
            {
                { "EntityMoniker", entityMoniker},
                { "AttributeName", attributeName},
                { "Labels", JToken.FromObject(labels) }
            };

            Content = new StringContent(
                content: content.ToString(),
                encoding: Encoding.UTF8,
                mediaType: "application/json");
        }
    }
}