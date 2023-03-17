using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ComplexEntityKeyMetadata
    {
        public Label DisplayName { get; set; }

        public EntityKeyIndexStatus EntityKeyIndexStatus { get; set; }

        public string EntityLogicalName { get; set; }

        public bool? HasChanged { get; set; }

        public string IntroducedVersion { get; set; }

        public BooleanManagedProperty IsCustomizable { get; set; }

        public bool? IsExportKey { get; set; }

        public bool? IsManaged { get; set; }

        public bool? IsSynchronous { get; set; }

        public List<string> KeyAttributes { get; set; } = new List<string>();

        public string LogicalName { get; set; }

        public Guid MetadataId { get; set; }

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.ComplexEntityKeyMetadata";
        public string SchemaName { get; set; }
    }
}