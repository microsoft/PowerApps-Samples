using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ComplexAttributeMetadata
    {
        public string AttributeOf { get; set; }
        public AttributeTypeCode AttributeType { get; set; }

        public AttributeTypeDisplayName AttributeTypeName { get; set; }

        public string AutoNumberFormat { get; set; }
        public bool? CanBeSecuredForCreate { get; set; }
        public bool? CanBeSecuredForRead { get; set; }
        public bool? CanBeSecuredForUpdate { get; set; }
        public BooleanManagedProperty CanModifyAdditionalSettings { get; set; }
        public int ColumnNumber { get; set; }

        public string DeprecatedVersion { get; set; }
        public Label Description { get; set; }

        public Label DisplayName { get; set; }
        public string EntityLogicalName { get; set; }
        public string ExternalName { get; set; }
        public bool? HasChanged { get; set; }
        public string InheritsFrom { get; set; }
        public string IntroducedVersion { get; set; }
        public BooleanManagedProperty IsAuditEnabled { get; set; }

        public bool? IsCustomAttribute { get; set; }

        public BooleanManagedProperty IsCustomizable { get; set; }
        public bool? IsDataSourceSecret { get; set; }
        public bool? IsFilterable { get; set; }
        public BooleanManagedProperty IsGlobalFilterEnabled { get; set; }
        public bool? IsLogical { get; set; }
        public bool? IsManaged { get; set; }
        public bool? IsPrimaryId { get; set; }

        public bool? IsPrimaryName { get; set; }

        public BooleanManagedProperty IsRenameable { get; set; }
        public bool? IsRequiredForForm { get; set; }
        public bool? IsRetrievable { get; set; }
        public bool? IsSearchable { get; set; }
        public bool? IsSecured { get; set; }
        public BooleanManagedProperty IsSortableEnabled { get; set; }
        public BooleanManagedProperty IsValidForAdvancedFind { get; set; }
        public bool? IsValidForCreate { get; set; }

        public bool? IsValidForForm { get; set; }
        public bool? IsValidForGrid { get; set; }
        public bool? IsValidForRead { get; set; }

        public bool? IsValidForUpdate { get; set; }
        public bool? IsValidODataAttribute { get; set; }
        public Guid? LinkedAttributeId { get; set; }

        public string LogicalName { get; set; }
        public Guid? MetadataId { get; set; }
        public AttributeRequiredLevelManagedProperty RequiredLevel { get; set; }
        public string SchemaName { get; set; }
        public int SourceType { get; set; }
    }
}