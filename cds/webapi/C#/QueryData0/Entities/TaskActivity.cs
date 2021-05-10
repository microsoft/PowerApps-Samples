using System;
using System.Text.Json.Serialization;

namespace PowerApps.Samples
{
    public class TaskActivity : IEntity
    {

        [JsonIgnore]
        public Guid? Id => activityid;

        [JsonIgnore]
        public static string SetName => "tasks";

        [JsonIgnore]
        public static string LogicalName => "task";

        [JsonPropertyName("@odata.type")]
        public string ODataType => $"Microsoft.Dynamics.CRM.{LogicalName}";


        public Guid? activityid { get; init; }

        public EntityReference ToEntityReference()
        {
            return new EntityReference(activityid, SetName);
        }

        public string subject { get; set; }
        public string description { get; set; }
        public DateTimeOffset? scheduledstart { get; set; } 
        public DateTimeOffset? scheduledend { get; set; }
        public int actualdurationminutes { get; set; }

    }
}
