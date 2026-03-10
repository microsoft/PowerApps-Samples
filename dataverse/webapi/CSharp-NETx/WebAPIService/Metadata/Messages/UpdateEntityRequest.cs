using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerApps.Samples.Metadata.Types;
using System.Text;

namespace PowerApps.Samples.Metadata.Messages
{
    /// <summary>
    /// Contains the data to update a table definition.
    /// </summary>
    public sealed class UpdateEntityRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the UpdateEntityRequest
        /// </summary>
        /// <param name="entityLogicalName">The logical name of the table</param>
        /// <param name="entityMetadata">The table definition with changes.</param>
        /// <param name="solutionUniqueName">The solution name to associate the changes with.</param>
        /// <param name="doNotMergeLabels">Whether to merge any lables included in the data.</param>
        public UpdateEntityRequest(
            string entityLogicalName,
            EntityMetadata entityMetadata,
            string? solutionUniqueName,
            bool doNotMergeLabels = false)
        {
            Method = HttpMethod.Put;
            RequestUri = new Uri(
                uriString: $"EntityDefinitions(LogicalName='{entityLogicalName}')",
                uriKind: UriKind.Relative);

            if (!string.IsNullOrWhiteSpace(solutionUniqueName))
            {
                Headers.Add("MSCRM.SolutionUniqueName", solutionUniqueName);
            }
            if (!doNotMergeLabels)
            {
                Headers.Add("MSCRM.MergeLabels", "true");
            }
            Content = new StringContent(
                content: JsonConvert.SerializeObject(entityMetadata,Formatting.Indented),
                encoding: Encoding.UTF8,
                mediaType: "application/json");
        }
    }
}
