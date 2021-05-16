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

        [JsonPropertyName("@odata.etag")]
        public string ETag { get; init; }

        public string subject { get; set; }
        public string description { get; set; }
        public DateTimeOffset? scheduledstart { get; set; }
        public DateTimeOffset? scheduledend { get; set; }
        public int? actualdurationminutes { get; set; }
        public int? statecode { get; set; }
        public int? statuscode { get; set; }



        /// <summary>
        /// The Lookup property that can be retrieved without expanding the single-valued navigation property.
        /// </summary>
        [JsonIgnore]
        public Guid? _regardingobjectid_contact_task_value { get; init; }

        /// The formatted value for the lookup property that can be retrieved without expanding the single-valued navigation property.
        /// </summary>
        [JsonIgnore]
        [JsonPropertyName("_regardingobjectid_contact_task_value@OData.Community.Display.V1.FormattedValue")]
        public string regardingobjectid_contact_task_display { get; init; }

        /// <summary>
        /// Contains the expanded value when retrieved.
        /// Can be set to a new Contact for Create with deep insert only.
        /// Setting this value for update does nothing!
        /// You must use Setprimarycontactid to set this value to an existing contact for create or update!
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Contact regardingobjectid_contact_task { get; set; }

        /// <summary>
        /// Sets a Primary Contact for the account.
        /// </summary>
        /// <param name="value">A reference to a Contact record</param>
        public void Setregardingobjectid_contact_task(Contact value)
        {
            regardingobjectid_contact_task_ref = value.ToEntityReference().Path;
            regardingobjectid_contact_task = null;
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("regardingobjectid_contact_task@odata.bind")]
        public string regardingobjectid_contact_task_ref { get; set; }

    }
}
