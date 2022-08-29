namespace PowerApps.Samples.Metadata.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response to the CreateGlobalOptionSetRequest
    /// </summary>
    public sealed class CreateGlobalOptionSetResponse : HttpResponseMessage
    {
        /// <summary>
        /// A reference to the Global OptionSet created.
        /// </summary>
        public EntityReference OptionSetReference => new EntityReference(uri: Headers.GetValues("OData-EntityId").FirstOrDefault());
    }
}
