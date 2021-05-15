using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PowerApps.Samples
{
    public class UserQuery : IEntity
    {
        [JsonIgnore]
        public Guid? Id => userqueryid;

        [JsonIgnore]
        public static string SetName => "userqueries";

        [JsonIgnore]
        public static string LogicalName => "userquery";

        [JsonPropertyName("@odata.type")]
        public string ODataType => $"Microsoft.Dynamics.CRM.{LogicalName}";

        public EntityReference ToEntityReference()
        {
            return new EntityReference(userqueryid, SetName);
        }

        [JsonPropertyName("@odata.etag")]
        public string ETag { get; init; }

        public Guid? userqueryid { get; init; }
        public string name { get; set; }
        public string description { get; set; }
        public int querytype { get; set; }
        public string returnedtypecode { get; set; }
        public string fetchxml { get; set; }


    }
}