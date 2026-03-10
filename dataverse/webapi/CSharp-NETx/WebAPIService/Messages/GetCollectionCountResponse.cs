namespace PowerApps.Samples.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response from the GetCollectionCountRequest
    /// </summary>
    public sealed class GetCollectionCountResponse : HttpResponseMessage
    {
        /// <summary>
        /// Gets the number of records in the collection
        /// </summary>
        public int Count
        {
            get
            {
                return int.Parse(Content.ReadAsStringAsync().GetAwaiter().GetResult());
            }
        }
    }
}
