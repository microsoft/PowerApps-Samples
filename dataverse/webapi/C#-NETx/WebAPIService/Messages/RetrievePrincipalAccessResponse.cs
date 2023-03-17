using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerApps.Samples.Types;

namespace PowerApps.Samples.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response from RetrievePrincipalAccessRequest
    /// </summary>
    public sealed class RetrievePrincipalAccessResponse : HttpResponseMessage
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

        public AccessRights AccessRights
        {
            get
            {
                // Needed to enable DeserializeObject to work.
                string accessRightsString = $"\"{_jObject[nameof(AccessRights)]}\"";

                AccessRights value = JsonConvert.DeserializeObject<AccessRights>(accessRightsString);

                return value;
            }
        }
    }
}
