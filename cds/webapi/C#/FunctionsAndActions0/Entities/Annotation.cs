using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PowerApps.Samples
{
    public class Annotation : IEntity
    {
        [JsonIgnore]
        public Guid? Id => annotationid;

        [JsonIgnore]
        public static string SetName => "annotations";

        [JsonIgnore]
        public static string LogicalName => "annotation";

        [JsonPropertyName("@odata.type")]
        public string ODataType => $"Microsoft.Dynamics.CRM.{LogicalName}";

        public EntityReference ToEntityReference()
        {
            return new EntityReference(annotationid, SetName);
        }

        [JsonPropertyName("@odata.etag")]
        public string ETag { get; init; }

        public Guid? annotationid { get; init; }
        public string subject { get; set; }
        public string documentbody { get; set; }
        public string notetext { get; set; }

        public Contact objectid_contact { get; set; }
    }
}
