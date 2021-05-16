using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PowerApps.Samples
{
    public class Contact : IEntity
    {
        [JsonIgnore]
        public Guid? Id => contactid;

        [JsonIgnore]
        public static string SetName => "contacts";

        [JsonIgnore]
        public static string LogicalName => "contact";

        [JsonPropertyName("@odata.type")]
        public string ODataType => $"Microsoft.Dynamics.CRM.{LogicalName}";


        public Guid? contactid { get; init; }

        public EntityReference ToEntityReference()
        {
            return new EntityReference(contactid, SetName);
        }

        [JsonPropertyName("@odata.etag")]
        public string ETag { get; init; }

        public Guid? _parentcustomerid_value { get; init; }

        public string firstname { get; set; }
        public string lastname { get; set; }
        public string fullname { get; init; }

        [JsonPropertyName("annualincome@OData.Community.Display.V1.FormattedValue")]
        public string annualincome_display { get; init; }

        public decimal annualincome { get; set; }
        public string jobtitle { get; set; }
        public string description { get; set; }

        public List<TaskActivity> Contact_Tasks { get; set; }

        public List<Account> account_primary_contact { get; set; }

        /// <summary>
        /// The Lookup property that can be retrieved without expanding the single-valued navigation property.
        /// </summary>
        [JsonIgnore]
        public Guid? _parentcustomerid_account_value { get; init; }

        /// The formatted value for the lookup property that can be retrieved without expanding the single-valued navigation property.
        /// </summary>
        [JsonIgnore]
        [JsonPropertyName("_parentcustomerid_account_value@OData.Community.Display.V1.FormattedValue")]
        public string parentcustomerid_account_display { get; init; }

        /// <summary>
        /// Contains the expanded value when retrieved.
        /// Can be set to a new Contact for Create with deep insert only.
        /// Setting this value for update does nothing!
        /// You must use Setprimarycontactid to set this value to an existing contact for create or update!
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Account parentcustomerid_account { get; set; }

        /// <summary>
        /// Sets a Primary Contact for the account.
        /// </summary>
        /// <param name="value">A reference to a Contact record</param>
        public void Setregardingobjectid_contact_task(Account value)
        {
            parentcustomerid_account_ref = value.ToEntityReference().Path;
            parentcustomerid_account = null;
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("parentcustomerid_account@odata.bind")]
        public string parentcustomerid_account_ref { get; set; }
    }
}
