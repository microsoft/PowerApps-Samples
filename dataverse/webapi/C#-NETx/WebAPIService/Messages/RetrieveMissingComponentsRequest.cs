namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Retrieves a list of missing components in the target organization.
    /// </summary>
    public sealed class RetrieveMissingComponentsRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the RetrieveMissingComponentsRequest
        /// </summary>
        /// <param name="customizationFile">The contents of a solution file.</param>
        public RetrieveMissingComponentsRequest(byte[] customizationFile)
        {
            // TODO: Make this work
            Method = HttpMethod.Get;
            RequestUri = new Uri(
                uriString: $"RetrieveMissingComponents(CustomizationFile=@p1)" +
                $"?@p1={Uri.EscapeDataString(Convert.ToBase64String(customizationFile))}",
                uriKind: UriKind.Relative);
        }
    }
}