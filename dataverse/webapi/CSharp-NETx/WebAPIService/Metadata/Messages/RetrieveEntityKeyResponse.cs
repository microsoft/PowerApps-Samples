using Newtonsoft.Json;
using PowerApps.Samples.Metadata.Types;

namespace PowerApps.Samples.Metadata.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response from the RetrieveEntityKeyRequest
    /// </summary>
    /// <typeparam name="T">The type of response.</typeparam>
    public sealed class RetrieveEntityKeyResponse : HttpResponseMessage
    {
        /// <summary>
        /// Metadata for the requested entity key.
        /// </summary>
        public EntityKeyMetadata EntityKeyMetadata => JsonConvert.DeserializeObject<EntityKeyMetadata>(Content.ReadAsStringAsync().GetAwaiter().GetResult());
    }
}
