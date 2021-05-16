using System;
using System.Text.Json.Serialization;

namespace PowerApps.Samples
{
    public class LetterActivity : IEntity
    {

        [JsonIgnore]
        public Guid? Id => activityid;

        [JsonIgnore]
        public static string SetName => "letters";

        [JsonIgnore]
        public static string LogicalName => "letter";

        [JsonPropertyName("@odata.type")]
        public string ODataType => $"Microsoft.Dynamics.CRM.{LogicalName}";


        public Guid? activityid { get; init; }

        public EntityReference ToEntityReference()
        {
            return new EntityReference(activityid, SetName);
        }

        [JsonPropertyName("@odata.etag")]
        public string ETag { get; init; }

        public string subject { get; set; }
        public string description { get; set; }
        public DateTimeOffset? scheduledstart { get; set; }
        public DateTimeOffset? scheduledend { get; set; }
        public int? actualdurationminutes { get; set; }
        public int? statecode { get; set; }
        public int? statuscode { get; set; }


    }
}
