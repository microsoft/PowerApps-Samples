using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace PowerApps.Samples.Metadata.Types
{
    public class ComplexBooleanOptionSetMetadata
    {
        public Label Description { get; set; }

        public Label DisplayName { get; set; }

        public string ExternalTypeName { get; set; }

        public OptionMetadata FalseOption { get; set; }

        public bool? HasChanged { get; set; }

        public string IntroducedVersion { get; set; }

        public BooleanManagedProperty IsCustomizable { get; set; }

        public bool? IsCustomOptionSet { get; set; }

        public bool? IsGlobal { get; set; }

        public bool? IsManaged { get; set; }

        public Guid? MetadataId { get; set; }

        public string Name { get; set; }

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.ComplexBooleanOptionSetMetadata";

        public OptionSetType OptionSetType { get; } = OptionSetType.Boolean;

        public OptionMetadata TrueOption { get; set; }
    }
}