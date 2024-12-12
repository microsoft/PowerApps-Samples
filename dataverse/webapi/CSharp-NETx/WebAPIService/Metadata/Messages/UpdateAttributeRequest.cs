using Newtonsoft.Json;
using PowerApps.Samples.Metadata.Types;
using System.Text;

namespace PowerApps.Samples.Metadata.Messages
{
    /// <summary>
    /// Contains the data to update a column definition.
    /// </summary>
    public sealed class UpdateAttributeRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the UpdateAttributeRequest
        /// </summary>
        /// <param name="entityLogicalName">The logical name of the table</param>
        /// <param name="attributeLogicalName">The logical name of the column.</param>
        /// <param name="attributeMetadata">The table definition with changes.</param>
        /// <param name="solutionUniqueName">The solution name to associate the changes with.</param>
        /// <param name="doNotMergeLabels">Whether to merge any lables included in the data.</param>
        public UpdateAttributeRequest(
            string entityLogicalName, 
            string attributeLogicalName, 
            AttributeMetadata attributeMetadata, 
            string? solutionUniqueName = null, 
            bool doNotMergeLabels = false)
        {
            Method = HttpMethod.Put;
            RequestUri = new Uri(
                uriString: $"EntityDefinitions(LogicalName='{entityLogicalName}')" +
                $"/Attributes(LogicalName='{attributeLogicalName}')",
                uriKind: UriKind.Relative);

            if (!string.IsNullOrWhiteSpace(solutionUniqueName))
            {
                Headers.Add("MSCRM.SolutionUniqueName", solutionUniqueName);
            }
            if (!doNotMergeLabels) {
                Headers.Add("MSCRM.MergeLabels", "true");
            }
            Content = new StringContent(
                content: JsonConvert.SerializeObject(attributeMetadata,Formatting.Indented),
                encoding: Encoding.UTF8,
                mediaType: "application/json");
        }
    }
}
