namespace PowerApps.Samples.Metadata.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response from the CreateAttributeRequest
    /// </summary>
    public sealed class CreateAttributeResponse : HttpResponseMessage
    {
        /// <summary>
        /// The Id of the column created.
        /// </summary>
        public Guid AttributeId
        {
            get
            {
                string uri = Headers.GetValues("OData-EntityId").FirstOrDefault();

                int firstParen = uri.LastIndexOf('(');
                int lastParen = uri.LastIndexOf(')');

                firstParen++;

                if (Guid.TryParse(uri[firstParen..lastParen], out Guid id))
                {
                   return id;
                }
                else
                {
                    throw new Exception("Unable to extract attributeid value");
                }
            }
        }
    }
}
