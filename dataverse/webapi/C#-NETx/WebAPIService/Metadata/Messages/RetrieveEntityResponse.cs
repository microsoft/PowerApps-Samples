using Newtonsoft.Json.Linq;
using PowerApps.Samples.Metadata.Types;

namespace PowerApps.Samples.Metadata.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    // This is the RetrieveEntityResponse ComplexType
    // https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/retrieveentityresponse
    /// <summary>
    /// Contains the response from the RetrieveEntityRequest
    /// </summary>
    public sealed class RetrieveEntityResponse : HttpResponseMessage
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
        /// Metadata for the requested entity.
        /// </summary>
        public ComplexEntityMetadata EntityMetadata => _jObject[nameof(EntityMetadata)].ToObject<ComplexEntityMetadata>();
    }
}
