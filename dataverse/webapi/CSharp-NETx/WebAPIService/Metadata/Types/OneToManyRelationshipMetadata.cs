using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class OneToManyRelationshipMetadata : RelationshipMetadataBase
    {
        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.OneToManyRelationshipMetadata";

        /// <summary>
        /// The associated menu configuration.
        /// </summary>
        public AssociatedMenuConfiguration AssociatedMenuConfiguration { get; set; }

        /// <summary>
        /// The cascading behaviors for the entity relationship.
        /// </summary>
        public CascadeConfiguration CascadeConfiguration { get; set; }

        /// <summary>
        /// Whether this relationship is the designated hierarchical self-referential relationship for this entity.
        /// </summary>
        public bool IsHierarchical { get; set; }

        /// <summary>
        /// The name of primary attribute for the referenced entity.
        /// </summary>
        public string ReferencedAttribute { get; set; }

        /// <summary>
        /// The name of the referenced entity.
        /// </summary>
        public string ReferencedEntity { get; set; }

        /// <summary>
        /// The collection-valued navigation property used by this relationship.
        /// </summary>
        public string ReferencedEntityNavigationPropertyName { get; set; }

        /// <summary>
        /// The name of the referencing attribute.
        /// </summary>
        public string ReferencingAttribute { get; set; }

        /// <summary>
        /// The name of the referencing entity.
        /// </summary>
        public string ReferencingEntity { get; set; }

        /// <summary>
        /// The single-valued navigation property used by this relationship.
        /// </summary>
        public string ReferencingEntityNavigationPropertyName { get; set; }

        /// <summary>
        /// Used to set the companion attribute when creating a One-To-Many relationship
        /// </summary>
        public LookupAttributeMetadata Lookup { get; set; }
    }
}