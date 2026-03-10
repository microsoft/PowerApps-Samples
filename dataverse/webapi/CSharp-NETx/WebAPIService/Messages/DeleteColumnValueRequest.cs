namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to delete a column value
    /// </summary>
    public sealed class DeleteColumnValueRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes a DeleteColumnValueRequest
        /// </summary>
        /// <param name="entityReference">A reference to a record that has the property</param>
        /// <param name="propertyName">The name of the property with the value to delete.</param>
        public DeleteColumnValueRequest(EntityReference entityReference, string propertyName)
        {
            Method = HttpMethod.Delete;
            RequestUri = new Uri(
                uriString: $"{entityReference.Path}/{propertyName}",
                uriKind: UriKind.Relative);
        }
    }
}
