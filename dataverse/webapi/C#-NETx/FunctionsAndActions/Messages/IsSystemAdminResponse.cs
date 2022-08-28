using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response from the IsSystemAdminRequest
    /// </summary>
    public sealed class IsSystemAdminResponse : HttpResponseMessage
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
        /// Whether the user has the System Administrator role
        /// </summary>
        public bool HasRole => (bool)_jObject.GetValue(nameof(HasRole));
    }
}
