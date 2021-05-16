using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PowerApps.Samples
{
    public class Incident : IEntity
    {
        [JsonIgnore]
        public Guid? Id => incidentid;

        [JsonIgnore]
        public static string SetName => "incidents";

        [JsonIgnore]
        public static string LogicalName => "incident";

        [JsonPropertyName("@odata.type")]
        public string ODataType => $"Microsoft.Dynamics.CRM.{LogicalName}";

        public EntityReference ToEntityReference()
        {
            return new EntityReference(incidentid, SetName);
        }

        [JsonPropertyName("@odata.etag")]
        public string ETag { get; init; }

        public Guid? incidentid { get; init; }      
        public string title { get; set; }

        /// <summary>
        /// The Lookup property that can be retrieved without expanding the single-valued navigation property.
        /// </summary>
        [JsonIgnore]
        public Guid? _customerid_account_value { get; init; }

        /// The formatted value for the lookup property that can be retrieved without expanding the single-valued navigation property.
        /// </summary>
        [JsonIgnore]
        [JsonPropertyName("_customerid_account_value@OData.Community.Display.V1.FormattedValue")]
        public string _customerid_account_display { get; init; }

        /// <summary>
        /// Contains the expanded value when retrieved.
        /// Can be set to a new Account for Create with deep insert only.
        /// Setting this value for update does nothing!
        /// You must use Setcustomerid_account to set this value to an existing account for create or update!
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Account customerid_account { get; set; }

        /// <summary>
        /// Sets an Account for the incident
        /// </summary>
        /// <param name="value">A reference to an Account record</param>
        public void Setcustomerid_account(Account value)
        {
            customerid_account_ref = value.ToEntityReference().Path;
            customerid_account = null;
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("customerid_account@odata.bind")]
        public string customerid_account_ref { get; set; }

        public List<TaskActivity> Incident_Tasks { get; set; }


    }
}
