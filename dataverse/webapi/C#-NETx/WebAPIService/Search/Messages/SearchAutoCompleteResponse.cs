using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerApps.Samples.Search.Types;

namespace PowerApps.Samples.Search.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response from the SSearchAutoCompleteRequest
    /// </summary>
    public sealed class SearchAutoCompleteResponse : HttpResponseMessage
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


        private string ResponseString => _jObject.GetValue(propertyName: "response").ToString();

        private JObject ResponseObj => (JObject)JsonConvert.DeserializeObject(ResponseString);

        public ErrorDetail? Error => JsonConvert.DeserializeObject<ErrorDetail>(ResponseObj[nameof(Error)]?.ToString() ?? string.Empty);

        public string? Value => (string?)ResponseObj[nameof(Value)];

        public QueryContext? QueryContext => JsonConvert.DeserializeObject<QueryContext>(ResponseObj[nameof(QueryContext)]?.ToString() ?? string.Empty);


    }
}
