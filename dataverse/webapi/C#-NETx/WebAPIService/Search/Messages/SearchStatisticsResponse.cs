using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Search.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response from the SearchstatisticsRequest
    /// </summary>
    public sealed class SearchStatisticsResponse : HttpResponseMessage
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

        /// <summary>
        /// The storage size in Bytes
        /// </summary>
        public long StorageSizeInBytes => (int)ResponseObj["value"]["storagesizeinbytes"];

        /// <summary>
        /// The storage size in Megabytes
        /// </summary>
        public long StorageSizeInMb => (int)ResponseObj["value"]["storagesizeinmb"];

        /// <summary>
        /// The document count
        /// </summary>
        public long DocumentCount => (int)ResponseObj["value"]["documentcount"];


    }
}
