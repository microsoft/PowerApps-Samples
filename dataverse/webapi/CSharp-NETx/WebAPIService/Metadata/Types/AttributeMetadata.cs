using Newtonsoft.Json;
using System;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class AttributeMetadata : MetadataBase
    {

        /// <summary>
        /// Gets the name of the attribute that this attribute extends.
        /// </summary>
        public string AttributeOf { get; }

        /// <summary>
        ///
        /// </summary>
        public string AutoNumberFormat { get; set; }

        /// <summary>
        /// Gets whether field-level security can be applied to prevent a user from adding data to this attribute.
        /// </summary>
        [JsonProperty]
        public bool? CanBeSecuredForCreate { get; private set; }

        /// <summary>
        /// Gets whether field-level security can be applied to prevent a user from viewing data from this attribute.
        /// </summary>
        [JsonProperty]
        public bool? CanBeSecuredForRead { get; private set; }

        /// <summary>
        /// Gets whether field-level security can be applied to prevent a user from updating data for this attribute.
        /// </summary>
        [JsonProperty]
        public bool? CanBeSecuredForUpdate { get; private set; }

        /// <summary>
        /// Gets or sets the property that determines whether any settings not controlled by managed properties can be changed.
        /// </summary>
        public BooleanManagedProperty CanModifyAdditionalSettings { get; set; }

        /// <summary>
        /// Gets an organization-specific ID for the attribute used for auditing.
        /// </summary>
        [JsonProperty]
        public int? ColumnNumber { get; private set; }

        /// <summary>
        /// Gets the version that the attribute was deprecated in.
        /// </summary>
        [JsonProperty]
        public string DeprecatedVersion { get; private set; }

        /// <summary>
        /// Gets or sets the description of the attribute.
        /// </summary>
        public Label Description { get; set; }

        /// <summary>
        /// Gets or sets the display name for the attribute.
        /// </summary>
        public Label DisplayName { get; set; }

        /// <summary>
        /// Gets the logical name of the entity that contains the attribute.
        /// </summary>
        [JsonProperty]
        public string EntityLogicalName { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public string ExternalName { get; set; }

        [JsonProperty]
        public string InheritsFrom { get; private set; }

        /// <summary>
        /// Gets a string identifying the solution version that the solution component was added in.
        /// </summary>
        [JsonProperty]
        public string IntroducedVersion { get; private set; }

        /// <summary>
        /// Gets or sets the property that determines whether the attribute is enabled for auditing.
        /// </summary>
        public BooleanManagedProperty IsAuditEnabled { get; set; }

        /// <summary>
        /// Gets whether the attribute is a custom attribute.
        /// </summary>
        [JsonProperty]
        public bool? IsCustomAttribute { get; private set; }

        /// <summary>
        /// Gets or sets the property that determines whether the attribute allows customization.
        /// </summary>
        public BooleanManagedProperty IsCustomizable { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool? IsDataSourceSecret { get; set; }

        [JsonProperty]
        public bool? IsFilterable { get; private set; }

        public BooleanManagedProperty IsGlobalFilterEnabled { get; set; }

        /// <summary>
        /// Gets whether the attribute is a logical attribute.
        /// </summary>
        [JsonProperty]
        public bool? IsLogical { get; private set; }

        /// <summary>
        /// Gets whether the attribute is part of a managed solution.
        /// </summary>
        [JsonProperty]
        public bool? IsManaged { get; private set; }

        /// <summary>
        /// Gets whether the attribute represents the unique identifier for the record.
        /// </summary>
        [JsonProperty]
        public bool? IsPrimaryId { get; private set; }

        /// <summary>
        /// Gets or sets whether the attribute represents the primary attribute for the entity.
        /// </summary>
        [JsonProperty]
        public bool? IsPrimaryName { get; set; }

        /// <summary>
        /// Gets or sets the property that determines whether the attribute display name can be changed.
        /// </summary>
        public BooleanManagedProperty IsRenameable { get; set; }

        /// <summary>
        /// Gets or sets whether the attribute must be included in a form in a model-driven app.
        /// </summary>
        public bool? IsRequiredForForm { get; set; }

        /// <summary>
        /// Gets whether the data in the attribute can be retrieved.
        /// </summary>
        [JsonProperty]
        public bool? IsRetrievable { get; private set; }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty]
        public bool? IsSearchable { get; private set; }

        /// <summary>
        /// Gets or sets whether the attribute is secured for field-level security.
        /// </summary>
        public bool? IsSecured { get; set; }

        /// <summary>
        ///
        /// </summary>
        public BooleanManagedProperty IsSortableEnabled { get; set; }

        /// <summary>
        /// Gets or sets the property that determines whether the attribute appears in Advanced Find.
        /// </summary>
        public BooleanManagedProperty IsValidForAdvancedFind { get; set; }

        /// <summary>
        /// Gets whether the value can be set when a record is created.
        /// </summary>
        public bool? IsValidForCreate { get; set; }

        /// <summary>
        /// Gets or sets whether the attribute can be added to a form in a model-driven app.
        /// </summary>
        public bool? IsValidForForm { get; set; }

        /// <summary>
        /// Gets or sets whether the attribute can be included in a view.
        /// </summary>
        public bool? IsValidForGrid { get; set; }

        /// <summary>
        /// Gets whether the value can be retrieved.
        /// </summary>
        [JsonProperty]
        public bool? IsValidForRead { get; private set; }

        /// <summary>
        /// Gets whether the value can be updated.
        /// </summary>
        public bool? IsValidForUpdate { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool? IsValidODataAttribute { get; private set; }

        /// <summary>
        /// Gets or sets an attribute that is linked between appointments and recurring appointments.
        /// </summary>
        public Guid? LinkedAttributeId { get; set; }

        /// <summary>
        /// Gets or sets the logical name for the attribute.
        /// </summary>
        public string LogicalName { get; set; }

        /// <summary>
        /// Gets or sets the property that determines the data entry requirement level enforced for the attribute.
        /// </summary>
        public AttributeRequiredLevelManagedProperty RequiredLevel { get; set; }

        /// <summary>
        /// Gets or sets the schema name for the attribute.
        /// </summary>
        public string SchemaName { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates the source type for a calculated or rollup attribute.
        /// </summary>
        public int? SourceType { get; set; }
    }
}