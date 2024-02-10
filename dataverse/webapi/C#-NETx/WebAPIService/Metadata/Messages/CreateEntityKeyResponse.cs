using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Metadata.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response to the CreateEntityRequest
    /// </summary>
    public sealed class CreateEntityKeyResponse : HttpResponseMessage
    {
        //Provides JObject for property getters
        private JObject JObject
        {
            get
            {
                return JObject.Parse(Content.ReadAsStringAsync().GetAwaiter().GetResult());
            }
        }

        /// <summary>
        /// The ID values of the created entity key
        /// </summary>
        public Guid EntityKeyId => JObject[nameof(EntityKeyId)].ToObject<Guid>();
    }
}
