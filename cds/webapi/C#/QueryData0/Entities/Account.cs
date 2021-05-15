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

        /// <summary>
        /// The Lookup property that can be retrieved without expanding the single-valued navigation property.
        /// </summary>
        [JsonIgnore]
        public Guid? _primarycontactid_value { get; init; }

        /// The formatted value for the lookup property that can be retrieved without expanding the single-valued navigation property.
        /// </summary>
        [JsonIgnore]
        [JsonPropertyName("_primarycontactid_value@OData.Community.Display.V1.FormattedValue")]
        public string _primarycontactid_display { get; init; }

        /// <summary>
        /// Contains the expanded value when retrieved.
        /// Can be set to a new Contact for Create with deep insert only.
        /// Setting this value for update does nothing!
        /// You must use Setprimarycontactid to set this value to an existing contact for create or update!
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Contact primarycontactid { get; set; }

        /// <summary>
        /// Sets a Primary Contact for the account.
        /// </summary>
        /// <param name="value">A reference to a Contact record</param>
        public void Setprimarycontactid(Contact value)
        {
            primarycontactid_ref = value.ToEntityReference().Path;
            primarycontactid = null;
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("primarycontactid@odata.bind")]
        public string primarycontactid_ref { get; set; }

        public List<TaskActivity> Account_Tasks { get; set; }

        public List<Contact> contact_customer_accounts { get; set; }


        /// <summary>
        /// The Lookup property that can be retrieved without expanding the single-valued navigation property.
        /// </summary>
        [JsonIgnore]
        public Guid? _createdby_value { get; init; }

        /// The formatted value for the lookup property that can be retrieved without expanding the single-valued navigation property.
        /// </summary>
        [JsonIgnore]
        [JsonPropertyName("_createdby_value@OData.Community.Display.V1.FormattedValue")]
        public string createdby_display { get; init; }

        /// <summary>
        /// Contains the expanded value when retrieved.
        /// Can be set to a new Contact for Create with deep insert only.
        /// Setting this value for update does nothing!
        /// You must use Setprimarycontactid to set this value to an existing contact for create or update!
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public SystemUser createdby { get; set; }



    }
}
