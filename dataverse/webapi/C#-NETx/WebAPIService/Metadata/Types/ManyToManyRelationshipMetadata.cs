using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ManyToManyRelationshipMetadata : RelationshipMetadataBase
    {
        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.ManyToManyRelationshipMetadata";

        /// <summary>
        /// The associated menu configuration for the first entity.
        /// </summary>
        public AssociatedMenuConfiguration Entity1AssociatedMenuConfiguration { get; set; }

        /// <summary>
        /// The attribute that defines the relationship in the first entity.
        /// </summary>
        public string Entity1IntersectAttribute { get; set; }

        /// <summary>
        /// The logical name of the first entity in the relationship.
        /// </summary>
        public string Entity1LogicalName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Entity1NavigationPropertyName { get; set; }

        /// <summary>
        /// The associated menu configuration for the second entity.
        /// </summary>
        public AssociatedMenuConfiguration Entity2AssociatedMenuConfiguration { get; set; }

        /// <summary>
        /// The attribute that defines the relationship in the second entity.
        /// </summary>
        public string Entity2IntersectAttribute { get; set; }

        /// <summary>
        /// The logical name of the second entity in the relationship.
        /// </summary>
        public string Entity2LogicalName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Entity2NavigationPropertyName { get; set; }

        /// <summary>
        /// The name of the intersect entity for the relationship.
        /// </summary>
        public string IntersectEntityName { get; set; }
    }
}