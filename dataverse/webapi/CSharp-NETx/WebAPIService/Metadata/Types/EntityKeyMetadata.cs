using Newtonsoft.Json;
using System;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class EntityKeyMetadata : MetadataBase
    {
        /// <summary>
        /// The asynchronous job.
        /// </summary>
        public Guid? AsyncJob { get; set; }

        /// <summary>
        /// A label containing the display name for the key.
        /// </summary>
        public Label DisplayName { get; set; }

        /// <summary>
        /// The entity key index status.
        /// </summary>
        public EntityKeyIndexStatus EntityKeyIndexStatus { get; set; }

        /// <summary>
        /// The entity logical name.
        /// </summary>
        public string EntityLogicalName { get; set; }

        /// <summary>
        /// A string identifying the solution version that the solution component was added in.
        /// </summary>
        public string IntroducedVersion { get; set; }

        /// <summary>
        /// Whether the entity key metadata is customizable.
        /// </summary>
        public BooleanManagedProperty IsCustomizable { get; set; }

        /// <summary>
        /// Whether entity key metadata is managed or not.
        /// </summary>
        public bool IsManaged { get; set; }

        /// <summary>
        /// The key attributes.
        /// </summary>
        public string[] KeyAttributes { get; set; }

        /// <summary>
        /// The logical name.
        /// </summary>
        public string LogicalName { get; set; }

        /// <summary>
        /// The schema name.
        /// </summary>
        public string SchemaName { get; set; }
    }
}