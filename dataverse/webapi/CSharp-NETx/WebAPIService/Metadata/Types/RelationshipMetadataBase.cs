using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class RelationshipMetadataBase : MetadataBase
    {
        /// <summary>
        /// A string identifying the solution version that the solution component was added in.
        /// </summary>
        public string IntroducedVersion { get; set; }

        /// <summary>
        /// Whether the entity relationship is customizable.
        /// </summary>
        public BooleanManagedProperty IsCustomizable { get; set; }

        /// <summary>
        /// Whether the relationship is a custom relationship.
        /// </summary>
        public bool IsCustomRelationship { get; set; }

        /// <summary>
        /// Whether the entity relationship is part of a managed solution.
        /// </summary>
        public bool IsManaged { get; set; }

        /// <summary>
        /// Whether the entity relationship should be shown in Advanced Find.
        /// </summary>
        public bool IsValidForAdvancedFind { get; set; }

        /// <summary>
        /// The type of relationship.
        /// </summary>
        public RelationshipType RelationshipType { get; set; }

        /// <summary>
        /// The schema name for the entity relationship.
        /// </summary>
        public string SchemaName { get; set; }

        /// <summary>
        /// The security type for the relationship.
        /// </summary>
        public SecurityTypes SecurityTypes { get; set; }
    }
}