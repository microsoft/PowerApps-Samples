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
    }
}
