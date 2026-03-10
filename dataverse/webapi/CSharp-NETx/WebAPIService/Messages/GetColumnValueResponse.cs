using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace PowerApps.Samples.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the data from the GetColumnValueRequest.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class GetColumnValueResponse<T> : HttpResponseMessage
    {
        string _content;

        private JsonDocument _jsonDocument
        {
            get
            {
                _content ??= Content.ReadAsStringAsync().GetAwaiter().GetResult();

                return JsonDocument.Parse(_content);
            }
        }

        /// <summary>
        /// The requested typed column  value.
        /// </summary>
        public T Value => (T)Convert.ChangeType(_jsonDocument.RootElement.GetProperty("value").GetString(), typeof(T));
    }
}
