namespace PowerApps.Samples.Metadata.Messages
{
    /// <summary>
    /// Contains the data to delete a column.
    /// </summary>
    public sealed class DeleteAttributeRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the DeleteAttributeRequest
        /// </summary>
        /// <param name="entityLogicalName">The logical name of the table.</param>
        /// <param name="logicalName">The logical name of the column.</param>
        /// <param name="strongConsistency">Whether to apply strong consistency header to the request.</param>
        public DeleteAttributeRequest(string entityLogicalName, string logicalName, bool strongConsistency = false)
        {
            Method = HttpMethod.Delete;
            RequestUri = new Uri(
                uriString: $"EntityDefinitions(LogicalName='{entityLogicalName}')/Attributes(LogicalName='{logicalName}')", 
                uriKind: UriKind.Relative);
            if (strongConsistency)
            {
                Headers.Add("Consistency", "Strong");
            }
        }
    }
}
