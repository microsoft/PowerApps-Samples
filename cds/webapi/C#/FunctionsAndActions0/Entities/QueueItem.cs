using System;
using System.Text.Json.Serialization;

namespace PowerApps.Samples
{
    public class QueueItem : IEntity
    {
        [JsonIgnore]
        public Guid? Id => queueitemid;

        [JsonIgnore]
        public static string SetName => "queueitems";

        [JsonIgnore]
        public static string LogicalName => "queueitem";

        [JsonPropertyName("@odata.type")]
        public string ODataType => $"Microsoft.Dynamics.CRM.{LogicalName}";

        public EntityReference ToEntityReference()
        {
            return new EntityReference(queueitemid, SetName);
        }

        [JsonPropertyName("@odata.etag")]
        public string ETag { get; init; }

        public Guid? queueitemid { get; init; }

        public string name { get; set; }

    }
}
