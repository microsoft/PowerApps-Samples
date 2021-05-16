using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PowerApps.Samples
{
    public class SystemUser : IEntity
    {
        [JsonIgnore]
        public Guid? Id => systemuserid;

        [JsonIgnore]
        public static string SetName => "systemusers";

        [JsonIgnore]
        public static string LogicalName => "systemuser";

        [JsonPropertyName("@odata.type")]
        public string ODataType => $"Microsoft.Dynamics.CRM.{LogicalName}";


        public Guid? systemuserid { get; init; }

        public EntityReference ToEntityReference()
        {
            return new EntityReference(systemuserid, SetName);
        }

        [JsonPropertyName("@odata.etag")]
        public string ETag { get; init; }

        public string fullname { get; set; }


        /// <summary>
        /// The Lookup property that can be retrieved without expanding the single-valued navigation property.
        /// </summary>
        [JsonIgnore]
        public Guid? _queueid_value { get; init; }

        /// The formatted value for the lookup property that can be retrieved without expanding the single-valued navigation property.
        /// </summary>
        [JsonIgnore]
        [JsonPropertyName("_queueid_value@OData.Community.Display.V1.FormattedValue")]
        public string _queueid_display { get; init; }

        /// <summary>
        /// Contains the expanded value when retrieved.
        /// Can be set to a new Queue for Create with deep insert only.
        /// Setting this value for update does nothing!
        /// You must use Setqueueid to set this value to an existing contact for create or update!
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Queue queueid { get; set; }

        /// <summary>
        /// Sets a Primary Contact for the account.
        /// </summary>
        /// <param name="value">A reference to a Contact record</param>
        public void Setqueueid(Queue value)
        {
            queueid_ref = value.ToEntityReference().Path;
            queueid = null;
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("queueid@odata.bind")]
        public string queueid_ref { get; set; }

    }
}
