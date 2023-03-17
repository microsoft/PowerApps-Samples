namespace PowerApps.Samples.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response from the UpsertRequest
    /// </summary>
    public sealed class UpsertResponse : HttpResponseMessage
    {
        /// <summary>
        /// A reference to the record.
        /// </summary>
        public EntityReference? EntityReference => new EntityReference(
                        uri: Headers.GetValues("OData-EntityId").FirstOrDefault());
    }
}
