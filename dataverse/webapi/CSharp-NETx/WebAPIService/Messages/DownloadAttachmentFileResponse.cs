namespace PowerApps.Samples.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the data from the DownloadAttachmentFileRequest.
    /// </summary>
    public sealed class DownloadAttachmentFileResponse : HttpResponseMessage
    {
        /// <summary>
        /// The requested annotation file value.
        /// </summary>
        public byte[] File => Convert.FromBase64String(Content.ReadAsStringAsync().GetAwaiter().GetResult());
    }
}
