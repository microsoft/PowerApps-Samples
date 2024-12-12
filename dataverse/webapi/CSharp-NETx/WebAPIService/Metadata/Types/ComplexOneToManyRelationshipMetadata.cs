using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ComplexOneToManyRelationshipMetadata
    {
        public AssociatedMenuConfiguration AssociatedMenuConfiguration { get; set; }

        public CascadeConfiguration CascadeConfiguration { get; set; }

        public bool? HasChanged { get; set; }

        public string IntroducedVersion { get; set; }

        public BooleanManagedProperty IsCustomizable { get; set; }

        public bool? IsCustomRelationship { get; set; }

        public bool? IsHierarchical { get; set; }

        public bool? IsManaged { get; set; }

        public bool? IsValidForAdvancedFind { get; set; }

        public Guid? MetadataId { get; set; }

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.ComplexOneToManyRelationshipMetadata";
        public string ReferencedAttribute { get; set; }

        public string ReferencedEntity { get; set; }

        public string ReferencedEntityNavigationPropertyName { get; set; }
        public string ReferencingAttribute { get; set; }

        public string ReferencingEntity { get; set; }
        public string ReferencingEntityNavigationPropertyName { get; set; }

        public int RelationshipBehavior { get; set; }
        public RelationshipType RelationshipType { get; set; }

        public string SchemaName { get; set; }

        public SecurityTypes SecurityTypes { get; set; }
    }
}