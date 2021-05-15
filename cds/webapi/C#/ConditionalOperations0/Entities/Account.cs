using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PowerApps.Samples
{
   public class Account : IEntity
    {
        [JsonIgnore]
        public Guid? Id => accountid;

        [JsonIgnore]
        public static string SetName => "accounts";

        [JsonIgnore]
        public static string LogicalName => "account";

        [JsonPropertyName("@odata.type")]
        public string ODataType => $"Microsoft.Dynamics.CRM.{LogicalName}";


        public Guid? accountid { get; init; }

        public EntityReference ToEntityReference()
        {
            return new EntityReference(accountid, SetName);
        }

        [JsonPropertyName("@odata.etag")]
        public string ETag { get; init; }

        public string name { get; set; }
        public string telephone1 { get; set; }
        public decimal revenue { get; set; }
        public string description { get; set; }


    }
}
