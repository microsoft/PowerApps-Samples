namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to delete a record
    /// </summary>
    public sealed class DeleteRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the DeleteRequest
        /// </summary>
        /// <param name="entityReference">A reference to the record to delete.</param>
        /// <param name="partitionId">The partition key to use.</param>
        /// <param name="strongConsistency">Whether strong consistency should be applied.</param>
        /// <param name="eTag">The current ETag value to compare.</param>
        public DeleteRequest(EntityReference entityReference, string? partitionId = null, bool strongConsistency = false, string? eTag = null)
        {
            string path;
            if (partitionId != null)
            {
                path = $"{entityReference.Path}?partitionId={partitionId}";
            }
            else
            {
                path = entityReference.Path;
            }

            Method = HttpMethod.Delete;
            RequestUri = new Uri(
                uriString: path, 
                uriKind: UriKind.Relative);
            if (strongConsistency)
            { 
                Headers.Add("Consistency", "Strong");
            }
            if (eTag != null)
            {
                //Prevent delete if record has changed on the server
                Headers.Add("If-Match", eTag);
            }
        }
    }
}
