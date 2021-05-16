using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PowerApps.Samples
{
    public class Queue : IEntity
    {
        [JsonIgnore]
        public Guid? Id => queueid;

        [JsonIgnore]
        public static string SetName => "queues";

        [JsonIgnore]
        public static string LogicalName => "queue";

        [JsonPropertyName("@odata.type")]
        public string ODataType => $"Microsoft.Dynamics.CRM.{LogicalName}";


        public Guid? queueid { get; init; }

        public EntityReference ToEntityReference()
        {
            return new EntityReference(queueid, SetName);
        }

        [JsonPropertyName("@odata.etag")]
        public string ETag { get; init; }

        public string fullname { get; set; }

    }
}
