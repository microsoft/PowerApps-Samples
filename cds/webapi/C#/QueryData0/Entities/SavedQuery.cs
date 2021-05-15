using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PowerApps.Samples
{
    public class SavedQuery : IEntity
    {
        [JsonIgnore]
        public Guid? Id => savedqueryid;

        [JsonIgnore]
        public static string SetName => "savedqueries";

        [JsonIgnore]
        public static string LogicalName => "savedquery";

        [JsonPropertyName("@odata.type")]
        public string ODataType => $"Microsoft.Dynamics.CRM.{LogicalName}";

        public EntityReference ToEntityReference()
        {
            return new EntityReference(savedqueryid, SetName);
        }

        [JsonPropertyName("@odata.etag")]
        public string ETag { get; init; }

        public Guid? savedqueryid { get; init; }
        public string name { get; set; }
        public string layoutxml { get; set; }



    }
}