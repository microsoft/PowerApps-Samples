using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ComplexEntityMetadata
    {
        public int? ActivityTypeMask { get; set; }

        public List<ComplexAttributeMetadata> Attributes { get; set; }

        public bool? AutoCreateAccessTeams { get; set; }

        public bool? AutoRouteToOwnerQueue { get; set; }

        public BooleanManagedProperty CanBeInCustomEntityAssociation { get; set; }

        public BooleanManagedProperty CanBeInManyToMany { get; set; }

        public BooleanManagedProperty CanBePrimaryEntityInRelationship { get; set; }

        public BooleanManagedProperty CanBeRelatedEntityInRelationship { get; set; }

        public BooleanManagedProperty CanChangeHierarchicalRelationship { get; set; }

        public BooleanManagedProperty CanChangeTrackingBeEnabled { get; set; }

        public BooleanManagedProperty CanCreateAttributes { get; set; }

        public BooleanManagedProperty CanCreateCharts { get; set; }

        public BooleanManagedProperty CanCreateForms { get; set; }

        public BooleanManagedProperty CanCreateViews { get; set; }

        public BooleanManagedProperty CanEnableSyncToExternalSearchIndex { get; set; }

        public BooleanManagedProperty CanModifyAdditionalSettings { get; set; }

        public bool? CanTriggerWorkflow { get; set; }

        public bool? ChangeTrackingEnabled { get; set; }

        public string CollectionSchemaName { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid? DataProviderId { get; set; }

        public Guid? DataSourceId { get; set; }

        public int? DaysSinceRecordLastModified { get; set; }

        public Label Description { get; set; }

        public Label DisplayCollectionName { get; set; }

        public Label DisplayName { get; set; }

        public bool? EnforceStateTransitions { get; set; }

        public string EntityColor { get; set; }

        public string EntityHelpUrl { get; set; }

        public bool? EntityHelpUrlEnabled { get; set; }

        public string EntitySetName { get; set; }

        public string ExternalCollectionName { get; set; }

        public string ExternalName { get; set; }

        public bool? HasActivities { get; set; }

        public bool? HasChanged { get; set; }

        public bool? HasFeedback { get; set; }

        public bool? HasNotes { get; set; }

        public string IconLargeName { get; set; }

        public string IconMediumName { get; set; }

        public string IconSmallName { get; set; }

        public string IconVectorName { get; set; }

        public string IntroducedVersion { get; set; }

        public bool? IsActivity { get; set; }

        public bool? IsActivityParty { get; set; }

        public bool? IsAIRUpdated { get; set; }

        public BooleanManagedProperty IsAuditEnabled { get; set; }

        public bool? IsAvailableOffline { get; set; }

        public bool? IsBPFEntity { get; set; }

        public bool? IsBusinessProcessEnabled { get; set; }

        public bool? IsChildEntity { get; set; }

        public BooleanManagedProperty IsConnectionsEnabled { get; set; }

        public bool? IsCustomEntity { get; set; }

        public BooleanManagedProperty IsCustomizable { get; set; }

        public bool? IsDocumentManagementEnabled { get; set; }

        public bool? IsDocumentRecommendationsEnabled { get; set; }

        public BooleanManagedProperty IsDuplicateDetectionEnabled { get; set; }

        public bool? IsEnabledForCharts { get; set; }

        public bool? IsEnabledForExternalChannels { get; set; }

        public bool? IsEnabledForTrace { get; set; }

        public bool? IsImportable { get; set; }

        public bool? IsInteractionCentricEnabled { get; set; }

        public bool? IsIntersect { get; set; }

        public bool? IsKnowledgeManagementEnabled { get; set; }

        public bool? IsLogicalEntity { get; set; }

        public BooleanManagedProperty IsMailMergeEnabled { get; set; }

        public bool? IsManaged { get; set; }

        public BooleanManagedProperty IsMappable { get; set; }

        public bool? IsMSTeamsIntegrationEnabled { get; set; }

        public BooleanManagedProperty IsOfflineInMobileClient { get; set; }

        public bool? IsOneNoteIntegrationEnabled { get; set; }

        public bool? IsOptimisticConcurrencyEnabled { get; set; }

        public bool? IsPrivate { get; set; }

        public bool? IsQuickCreateEnabled { get; set; }

        public bool? IsReadingPaneEnabled { get; set; }

        public BooleanManagedProperty IsReadOnlyInMobileClient { get; set; }

        public BooleanManagedProperty IsRenameable { get; set; }

        public bool? IsSLAEnabled { get; set; }

        public bool? IsSolutionAware { get; set; }

        public bool? IsStateModelAware { get; set; }

        public bool? IsValidForAdvancedFind { get; set; }

        public BooleanManagedProperty IsValidForQueue { get; set; }

        public BooleanManagedProperty IsVisibleInMobile { get; set; }

        public BooleanManagedProperty IsVisibleInMobileClient { get; set; }

        public List<ComplexEntityKeyMetadata> Keys { get; set; }

        public string LogicalCollectionName { get; set; }

        public string LogicalName { get; set; }

        public List<ComplexManyToManyRelationshipMetadata> ManyToManyRelationships { get; set; }

        public List<ComplexOneToManyRelationshipMetadata> ManyToOneRelationships { get; set; }

        public Guid? MetadataId { get; set; }

        public string MobileOfflineFilters { get; set; }

        public int? ObjectTypeCode { get; set; }

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.ComplexEntityMetadata";

        public List<ComplexOneToManyRelationshipMetadata> OneToManyRelationships { get; set; }

        public OwnershipTypes? OwnershipType { get; set; }

        public string PrimaryIdAttribute { get; set; }

        public string PrimaryImageAttribute { get; set; }

        public string PrimaryNameAttribute { get; set; }

        public List<SecurityPrivilegeMetadata> Privileges { get; set; }

        public string RecurrenceBaseEntityLogicalName { get; set; }

        public string ReportViewName { get; set; }

        public string SchemaName { get; set; }

        public string SettingOf { get; set; }

        public List<EntitySetting> Settings { get; set; }

        public bool? SyncToExternalSearchIndex { get; set; }

        public bool? UsesBusinessDataLabelTable { get; set; }
    }
}