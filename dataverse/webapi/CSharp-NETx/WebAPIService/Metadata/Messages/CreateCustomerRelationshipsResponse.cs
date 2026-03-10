using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Metadata.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response to the CreateCustomerRelationshipsRequest
    /// </summary>
    public sealed class CreateCustomerRelationshipsResponse : HttpResponseMessage
    {
        // Cache the async content
        private string? _content;

        //Provides JObject for property getters
        private JObject _jObject
        {
            get
            {
                _content ??= Content.ReadAsStringAsync().GetAwaiter().GetResult();

                return JObject.Parse(_content);
            }
        }


        /// <summary>
        /// The MetadataId of the LookupAttributeMetadata that is created.
        /// </summary>
        public Guid AttributeId => (Guid)_jObject.GetValue(nameof(AttributeId));

        /// <summary>
        /// An array of relationship IDs created for the attribute to Account and Contact entities.
        /// </summary>
        public Guid[] RelationshipIds => JsonConvert.DeserializeObject<Guid[]>(_jObject.GetValue(nameof(RelationshipIds)).ToString());
    }
}
