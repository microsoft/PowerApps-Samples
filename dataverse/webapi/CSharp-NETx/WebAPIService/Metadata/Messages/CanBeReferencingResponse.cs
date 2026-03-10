using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Metadata.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response from the CanBeReferencing action.
    /// </summary>
    public sealed class CanBeReferencingResponse : HttpResponseMessage
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
        /// Whether the entity can be the referencing entity (many) in a one-to-many relationship.
        /// </summary>
        public bool CanBeReferencing => (bool)_jObject["CanBeReferencing"];
    }
}
