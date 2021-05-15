using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PowerApps.Samples
{
    public class SystemUser : IEntity
    {
        [JsonIgnore]
        public Guid? Id => systemuserid;

        [JsonIgnore]
        public static string SetName => "systemusers";

        [JsonIgnore]
        public static string LogicalName => "systemuser";

        [JsonPropertyName("@odata.type")]
        public string ODataType => $"Microsoft.Dynamics.CRM.{LogicalName}";


        public Guid? systemuserid { get; init; }

        public EntityReference ToEntityReference()
        {
            return new EntityReference(systemuserid, SetName);
        }

        [JsonPropertyName("@odata.etag")]
        public string ETag { get; init; }

        public string fullname { get; set; }

    }
}
