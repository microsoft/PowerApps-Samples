using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerApps.Samples.Search.Types;

namespace PowerApps.Samples.Search.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response from the SearchStatusRequest
    /// </summary>
    public sealed class SearchStatusResponse : HttpResponseMessage
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

        public SearchStatus Status => Enum.Parse<SearchStatus>(ResponseObj["value"]?["status"]?.ToString() ?? SearchStatus.NotProvisioned.ToString(), ignoreCase: true);

        public LockboxStatus LockboxStatus => Enum.Parse<LockboxStatus>(ResponseObj["value"]?["lockboxstatus"]?.ToString() ?? LockboxStatus.Unknown.ToString(), ignoreCase: true);

        public CMKStatus CMKStatus => Enum.Parse<CMKStatus>(ResponseObj["value"]?["cmkstatus"]?.ToString() ?? CMKStatus.Unknown.ToString(), ignoreCase: true);

        public List<EntityStatusInfo>? EntityStatusInfo => JsonConvert.DeserializeObject<List<EntityStatusInfo>>(ResponseObj["value"]?["entitystatusresults"]?.ToString() ?? string.Empty);

        public List<ManyToManyRelationshipSyncStatus>? ManyToManyRelationshipSyncStatus => JsonConvert.DeserializeObject<List<ManyToManyRelationshipSyncStatus>>(ResponseObj["value"]?["manytomanyrelationshipsyncstatus"]?.ToString() ?? string.Empty);
    }
}
