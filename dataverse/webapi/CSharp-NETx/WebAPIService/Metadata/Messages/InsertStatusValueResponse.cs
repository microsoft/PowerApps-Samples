using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Metadata.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response to the CreateEntityRequest
    /// </summary>
    public sealed class InsertStatusValueResponse : HttpResponseMessage
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
        /// The option value for the new option.
        /// </summary>
        public int NewOptionValue
        {
            get
            {
                return (int)_jObject["NewOptionValue"];
            }
        }
    }
}
