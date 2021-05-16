using System;
using System.Text.Json.Serialization;

namespace PowerApps.Samples
{
    public class OpportunityCloseActivity : IEntity
    {

        [JsonIgnore]
        public Guid? Id => activityid;

        [JsonIgnore]
        public static string SetName => "opportunitycloses";

        [JsonIgnore]
        public static string LogicalName => "opportunityclose";

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
        public Guid? _opportunityid_value { get; init; }

        /// The formatted value for the lookup property that can be retrieved without expanding the single-valued navigation property.
        /// </summary>
        [JsonIgnore]
        [JsonPropertyName("_opportunityid_value@OData.Community.Display.V1.FormattedValue")]
        public string opportunityid_display { get; init; }

        /// <summary>
        /// Contains the expanded value when retrieved.
        /// Can be set to a new Opportunity for Create with deep insert only.
        /// Setting this value for update does nothing!
        /// You must use Setopportunityid to set this value to an existing opportunity for create or update!
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Opportunity opportunityid { get; set; }

        /// <summary>
        /// Sets an Opportunity for the opportunity close.
        /// </summary>
        /// <param name="value">A reference to a Contact record</param>
        public void Setopportunityid(Opportunity value)
        {
            opportunityid_ref = value.ToEntityReference().Path;
            opportunityid = null;
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("opportunityid@odata.bind")]
        public string opportunityid_ref { get; set; }

    }
}
