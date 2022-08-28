using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ComplexOptionSetMetadata
    {
        public Label Description { get; set; }

        public Label DisplayName { get; set; }

        public string ExternalTypeName { get; set; }

        public bool? HasChanged { get; set; }

        public string IntroducedVersion { get; set; }

        public BooleanManagedProperty IsCustomizable { get; set; }

        public bool? IsCustomOptionSet { get; set; }

        public bool? IsGlobal { get; set; }

        public bool? IsManaged { get; set; }

        public Guid? MetadataId { get; set; }

        public string Name { get; set; }

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.ComplexOptionSetMetadata";

        public List<OptionMetadata> Options { get; set; } = new List<OptionMetadata>();
        public OptionSetType OptionSetType { get; set; }

        public string ParentOptionSetName { get; set; }
    }
}