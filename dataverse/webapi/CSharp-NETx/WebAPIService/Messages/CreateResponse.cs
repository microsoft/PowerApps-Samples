namespace PowerApps.Samples.Messages
{

    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response from the CreateRequest
    /// </summary>
    public sealed class CreateResponse : HttpResponseMessage
    {
        /// <summary>
        /// A reference to the record created.
        /// </summary>
        public EntityReference? EntityReference
        {
            get
            {
                if (Headers != null &&
                    Headers.Contains("OData-EntityId") &&
                    Headers.GetValues("OData-EntityId") != null)
                {
                    return new EntityReference(Headers.GetValues("OData-EntityId").FirstOrDefault());
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
