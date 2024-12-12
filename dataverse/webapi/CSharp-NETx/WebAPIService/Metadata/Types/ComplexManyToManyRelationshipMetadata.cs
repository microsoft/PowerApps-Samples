using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ComplexManyToManyRelationshipMetadata
    {
        public AssociatedMenuConfiguration Entity1AssociatedMenuConfiguration { get; set; }

        public string Entity1IntersectAttribute { get; set; }

        public string Entity1LogicalName { get; set; }

        public string Entity1NavigationPropertyName { get; set; }

        public AssociatedMenuConfiguration Entity2AssociatedMenuConfiguration { get; set; }

        public string Entity2IntersectAttribute { get; set; }

        public string Entity2LogicalName { get; set; }

        public string Entity2NavigationPropertyName { get; set; }

        public bool HasChanged { get; set; }

        public string IntersectEntityName { get; set; }

        public BooleanManagedProperty IsCustomizable { get; set; }

        public bool? IsCustomRelationship { get; set; }

        public bool? IsManaged { get; set; }

        public bool? IsValidForAdvancedFind { get; set; }

        public Guid? MetadataId { get; set; }

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.ComplexManyToManyRelationshipMetadata";

        public RelationshipType RelationshipType { get; set; }

        public string SchemaName { get; set; }

        public SecurityTypes SecurityTypes { get; set; }
    }
}