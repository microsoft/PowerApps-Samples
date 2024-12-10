using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class LookupAttributeMetadata : AttributeMetadata
    {

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.LookupAttributeMetadata";

        public AttributeTypeCode AttributeType { get; } = AttributeTypeCode.Lookup;

        public AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.LookupType);

        /// <summary>
        /// The target entity types for the lookup.
        /// </summary>
        public List<string> Targets { get; set; }

        /// <summary>
        ///
        /// </summary>
        public LookupFormat Format { get; set; }
    }
}