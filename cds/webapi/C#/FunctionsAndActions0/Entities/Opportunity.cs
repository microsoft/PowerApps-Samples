using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PowerApps.Samples
{
    public class Opportunity : IEntity
    {
        [JsonIgnore]
        public Guid? Id => opportunityid;

        [JsonIgnore]
        public static string SetName => "opportunities";

        [JsonIgnore]
        public static string LogicalName => "opportunity";

        [JsonPropertyName("@odata.type")]
        public string ODataType => $"Microsoft.Dynamics.CRM.{LogicalName}";


        public Guid? opportunityid { get; init; }

        public EntityReference ToEntityReference()
        {
            return new EntityReference(opportunityid, SetName);
        }

        [JsonPropertyName("@odata.etag")]
        public string ETag { get; init; }

        public string name { get; set; }
        public string description { get; set; }        

    }
}