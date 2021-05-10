using System;
using System.Text.Json.Serialization;

namespace PowerApps.Samples
{
    public class ActivityPointer : IEntity
    {
        [JsonIgnore]
        public Guid? Id => activityid;

        [JsonIgnore]
        public static string SetName => "activitypointers";

        [JsonIgnore]
        public static string LogicalName => "activitypointer";

        [JsonPropertyName("@odata.type")]
        public string ODataType => $"Microsoft.Dynamics.CRM.{LogicalName}";


        public Guid? activityid { get; init; }

        public EntityReference ToEntityReference()
        {
            return new EntityReference(activityid, SetName);
        }
        
        public string subject { get; set; }
        public string description { get; set; }
        public DateTimeOffset scheduledend { get; set; }


    }
}
