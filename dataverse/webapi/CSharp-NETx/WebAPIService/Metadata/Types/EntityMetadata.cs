using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class EntityMetadata : MetadataBase
    {       

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.EntityMetadata";

        /// <summary>
        /// Whether a custom activity should appear in the activity menus in the Web application.
        /// </summary>

        public int? ActivityTypeMask { get; set; }
        /// <summary>
        /// Indicates whether the entity is enabled for auto created access teams.
        /// </summary>

        public bool? AutoCreateAccessTeams { get; set; }
        /// <summary>
        /// Indicates whether to automatically move records to the owner’s default queue when a record of this type is created or assigned
        /// </summary>

        public bool? AutoRouteToOwnerQueue { get; set; }
        /// <summary>
        /// TODO
        /// </summary>
        public BooleanManagedProperty CanBeInCustomEntityAssociation { get; set; }

        /// <summary>
        /// Whether the entity can be in a Many-to-Many entity relationship.
        /// </summary>
        public BooleanManagedProperty CanBeInManyToMany { get; set; }

        /// <summary>
        /// Whether the entity can be the referenced entity in a One-to-Many entity relationship.
        /// </summary>
        public BooleanManagedProperty CanBePrimaryEntityInRelationship { get; set; }

        /// <summary>
        /// Whether the entity can be the referencing entity in a One-to-Many entity relationship.
        /// </summary>
        public BooleanManagedProperty CanBeRelatedEntityInRelationship { get; set; }
        /// <summary>
        /// Whether the hierarchical state of entity relationships included in your managed solutions can be changed.
        /// </summary>
        public BooleanManagedProperty CanChangeHierarchicalRelationship { get; set; }
        /// <summary>
        /// For internal use only.
        /// </summary>
        public BooleanManagedProperty CanChangeTrackingBeEnabled { get; set; }

        /// <summary>
        ///  Whether additional attributes can be added to the entity.
        /// </summary>
        public BooleanManagedProperty CanCreateAttributes { get; set; }
        /// <summary>
        /// Whether new charts can be created for the entity.
        /// </summary>
        public BooleanManagedProperty CanCreateCharts { get; set; }
        /// <summary>
        /// Whether new forms can be created for the entity.
        /// </summary>
        public BooleanManagedProperty CanCreateForms { get; set; }
        /// <summary>
        /// Whether new views can be created for the entity.
        /// </summary>
        public BooleanManagedProperty CanCreateViews { get; set; }
        /// <summary>
        /// For internal use only.
        /// </summary>
        public BooleanManagedProperty CanEnableSyncToExternalSearchIndex { get; set; }
        /// <summary>
        /// Whether any other entity properties not represented by a managed property can be changed.
        /// </summary>
        public BooleanManagedProperty CanModifyAdditionalSettings { get; set; }
        /// <summary>
        /// Whether the entity can trigger a workflow process.
        /// </summary>
        [JsonProperty]
        public bool? CanTriggerWorkflow { get; private set; }
        /// <summary>
        /// Whether change tracking is enabled for an entity.
        /// </summary>

        public bool? ChangeTrackingEnabled { get; set; }
        /// <summary>
        /// The collection schema name of the entity.
        /// </summary>
        [JsonProperty]
        public string CollectionSchemaName { get; private set; }
        /// <summary>
        /// TODO
        /// </summary>
        public DateTime? CreatedOn { get; private set; }
        /// <summary>
        /// TODO
        /// </summary>
        public Guid? DataProviderId { get; set; }
        /// <summary>
        /// TODO
        /// </summary>

        public Guid? DataSourceId { get; set; }
        /// <summary>
        /// TODO
        /// </summary>

        public int? DaysSinceRecordLastModified { get; set; }
        /// <summary>
        /// The label containing the description for the entity.
        /// </summary>

        public Label Description { get; set; }
        /// <summary>
        /// A label containing the plural display name for the entity.
        /// </summary>

        public Label DisplayCollectionName { get; set; }
        /// <summary>
        /// A label containing the display name for the entity.
        /// </summary>

        public Label DisplayName { get; set; }
        /// <summary>
        /// Whether the entity will enforce custom state transitions.
        /// </summary>
        [JsonProperty]
        public bool? EnforceStateTransitions { get; private set; }
        /// <summary>
        /// The hexadecimal code to represent the color to be used for this entity in the application.
        /// </summary>

        public string EntityColor { get; set; }
        /// <summary>
        /// The URL of the resource to display help content for this entity
        /// </summary>

        public string EntityHelpUrl { get; set; }
        /// <summary>
        /// Whether the entity supports custom help content.
        /// </summary>

        public bool? EntityHelpUrlEnabled { get; set; }
        /// <summary>
        /// The name of the Web API entity set for this entity.
        /// </summary>
        public string EntitySetName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ExternalCollectionName { get; set; }
        /// <summary>
        ///
        /// </summary>

        public string ExternalName { get; set; }
        /// <summary>
        /// Whether activities are associated with this entity.
        /// </summary>

        public bool? HasActivities { get; set; }
        /// <summary>
        /// Whether the entity will have a special relationship to the Feedback entity.
        /// </summary>

        public bool? HasFeedback { get; set; }
        /// <summary>
        /// Whether notes are associated with this entity.
        /// </summary>

        public bool? HasNotes { get; set; }
        /// <summary>
        /// The name of the image web resource for the large icon for the entity.
        /// </summary>

        public string IconLargeName { get; set; }
        /// <summary>
        /// The name of the image web resource for the medium icon for the entity.
        /// </summary>

        public string IconMediumName { get; set; }
        /// <summary>
        /// The name of the image web resource for the small icon for the entity.
        /// </summary>

        public string IconSmallName { get; set; }
        /// <summary>
        /// The name of the image web resource for the small icon for the entity.
        /// </summary>

        public string IconVectorName { get; set; }
        /// <summary>
        /// A string identifying the solution version that the solution component was added in.
        /// </summary>
        [JsonProperty]
        public string IntroducedVersion { get; private set; }
        /// <summary>
        /// Whether the entity is an activity.
        /// </summary>

        public bool? IsActivity { get; set; }
        /// <summary>
        /// Whether the email messages can be sent to an email address stored in a record of this type.
        /// </summary>

        public bool? IsActivityParty { get; set; }
        /// <summary>
        /// Whether the entity uses the updated user interface.
        /// </summary>
        [JsonProperty]
        public bool? IsAIRUpdated { get; private set; }
        /// <summary>
        /// Whether auditing has been enabled for the entity.
        /// </summary>
        public BooleanManagedProperty IsAuditEnabled { get; set; }
        /// <summary>
        /// Whether the entity is available offline.
        /// </summary>

        public bool? IsAvailableOffline { get; set; }
        /// <summary>
        ///
        /// </summary>

        public bool? IsBPFEntity { get; set; }
        /// <summary>
        /// Whether the entity is enabled for business process flows.
        /// </summary>

        public bool? IsBusinessProcessEnabled { get; set; }
        /// <summary>
        /// Whether the entity is a child entity.
        /// </summary>
        [JsonProperty]
        public bool? IsChildEntity { get; private set; }
        /// <summary>
        /// Whether connections are enabled for this entity.
        /// </summary>
        public BooleanManagedProperty IsConnectionsEnabled { get; set; }
        /// <summary>
        /// Whether the entity is a custom entity.
        /// </summary>
        [JsonProperty]
        public bool? IsCustomEntity { get; private set; }
        /// <summary>
        /// Whether the entity is customizable.
        /// </summary>
        public BooleanManagedProperty IsCustomizable { get; set; }
        /// <summary>
        /// Whether document management is enabled.
        /// </summary>

        public bool? IsDocumentManagementEnabled { get; set; }
        /// <summary>
        ///
        /// </summary>

        public bool? IsDocumentRecommendationsEnabled { get; set; }
        /// <summary>
        /// Whether duplicate detection is enabled.
        /// </summary>
        public BooleanManagedProperty IsDuplicateDetectionEnabled { get; set; }
        /// <summary>
        /// Whether charts are enabled.
        /// </summary>
        [JsonProperty]
        public bool? IsEnabledForCharts { get; private set; }
        /// <summary>
        /// Whether this entity is enabled for external channels
        /// </summary>

        public bool? IsEnabledForExternalChannels { get; set; }
        /// <summary>
        /// For internal use only.
        /// </summary>

        public bool? IsEnabledForTrace { get; set; }
        /// <summary>
        /// Whether the entity can be imported using the Import Wizard.
        /// </summary>

        public bool? IsImportable { get; set; }
        /// <summary>
        /// Whether the entity is enabled for interactive experience.
        /// </summary>

        public bool? IsInteractionCentricEnabled { get; set; }
        /// <summary>
        /// Whether the entity is an intersection table for two other entities.
        /// </summary>

        public bool? IsIntersect { get; set; }
        /// <summary
        /// Whether Parature knowledge management integration is enabled for the entity.
        /// </summary>

        public bool? IsKnowledgeManagementEnabled { get; set; }
        /// <summary>
        ///
        /// </summary>
        [JsonProperty]
        public bool? IsLogicalEntity { get; private set; }
        /// <summary>
        /// Whether mail merge is enabled for this entity.
        /// </summary>
        public BooleanManagedProperty IsMailMergeEnabled { get; set; }
        /// <summary>
        /// Whether the entity is part of a managed solution.
        /// </summary>
        [JsonProperty]
        public bool? IsManaged { get; private set; }
        /// <summary>
        /// Whether entity mapping is available for the entity.
        /// </summary>
        public BooleanManagedProperty IsMappable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? IsMSTeamsIntegrationEnabled { get; set; }
        /// <summary>
        /// Whether this entity is enabled for offline data with Dynamics 365 for tablets and Dynamics 365 for phones.
        /// </summary>
        public BooleanManagedProperty IsOfflineInMobileClient { get; set; }
        /// <summary>
        /// Whether OneNote integration is enabled for the entity.
        /// </summary>

        public bool? IsOneNoteIntegrationEnabled { get; set; }
        /// <summary>
        /// Whether optimistic concurrency is enabled for the entity
        /// </summary>
        [JsonProperty]
        public bool? IsOptimisticConcurrencyEnabled { get; private set; }
        /// <summary>
        /// For internal use only.
        /// </summary>
        [JsonProperty]
        public bool? IsPrivate { get; private set; }
        /// <summary>
        /// Whether the entity is enabled for quick create forms.
        /// </summary>

        public bool? IsQuickCreateEnabled { get; set; }
        /// <summary>
        /// For internal use only.
        /// </summary>

        public bool? IsReadingPaneEnabled { get; set; }
        /// <summary>
        /// Whether Microsoft Dynamics 365 for tablets users can update data for this entity.
        /// </summary>
        public BooleanManagedProperty IsReadOnlyInMobileClient { get; set; }
        /// <summary>
        /// Whether the entity DisplayName and DisplayCollectionName can be changed by editing the entity in the application.
        /// </summary>
        public BooleanManagedProperty IsRenameable { get; set; }
        /// <summary>
        ///
        /// </summary>
        public bool? IsSLAEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsSolutionAware { get; set; }

        /// <summary>
        /// Whether the entity supports setting custom state transitions.
        /// </summary>
        [JsonProperty]
        public bool? IsStateModelAware { get; private set; }
        /// <summary>
        /// Whether the entity is will be shown in Advanced Find.
        /// </summary>
        [JsonProperty]
        public bool? IsValidForAdvancedFind { get; private set; }
        /// <summary>
        /// Whether the entity is enabled for queues.
        /// </summary>
        public BooleanManagedProperty IsValidForQueue { get; set; }
        /// <summary>
        /// Whether Microsoft Dynamics 365 for phones users can see data for this entity.
        /// </summary>
        public BooleanManagedProperty IsVisibleInMobile { get; set; }
        /// <summary>
        /// Whether Microsoft Dynamics 365 for tablets users can see data for this entity.
        /// </summary>
        public BooleanManagedProperty IsVisibleInMobileClient { get; set; }
        /// <summary>
        /// The logical collection name.
        /// </summary>
        public string LogicalCollectionName { get; set; }
        /// <summary>
        /// The logical name for the entity.
        /// </summary>

        public string LogicalName { get; set; }
        /// <summary>
        ///
        /// </summary>

        public string MobileOfflineFilters { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? ModifiedOn { get; private set; }


        /// <summary>
        /// The entity type code.
        /// </summary>
        [JsonProperty]
        public int? ObjectTypeCode { get; private set; }

        /// <summary>
        /// The ownership type for the entity.
        /// </summary>
        public OwnershipTypes OwnershipType { get; set; }

        /// <summary>
        /// The name of the attribute that is the primary id for the entity.
        /// </summary>
        [JsonProperty]
        public string PrimaryIdAttribute { get; private set; }
        /// <summary>
        /// The name of the primary image attribute for an entity.
        /// </summary>
        [JsonProperty]
        public string PrimaryImageAttribute { get; private set; }
        /// <summary>
        /// The name of the primary attribute for an entity.
        /// </summary>
        [JsonProperty]
        public string PrimaryNameAttribute { get; set; }
        /// <summary>
        /// The privilege metadata for the entity.
        /// </summary>
        [JsonProperty]
        public List<SecurityPrivilegeMetadata>? Privileges { get; private set; } 
        /// <summary>
        /// The name of the entity that is recurring.
        /// </summary>
        [JsonProperty]
        public string RecurrenceBaseEntityLogicalName { get; private set; }
        /// <summary>
        /// The name of the report view for the entity.
        /// </summary>
        [JsonProperty]
        public string ReportViewName { get; private set; }
        /// <summary>
        /// The schema name for the entity.
        /// </summary>

        public string SchemaName { get; set; }
        /// <summary>
        ///
        /// </summary>

        public string SettingOf { get; set; }

        public List<EntitySetting>? Settings { get; set; }

        public bool? SyncToExternalSearchIndex { get; set; }

        public string? TableType { get; set; }

        public bool? UsesBusinessDataLabelTable { get; set; }

        [JsonProperty]
        public List<AttributeMetadata>? Attributes { get; set; }
        [JsonProperty]
        public List<EntityKeyMetadata>? Keys { get; set; } 
        [JsonProperty]
        public List<ManyToManyRelationshipMetadata>? ManyToManyRelationships { get; set; }
        [JsonProperty]
        public List<OneToManyRelationshipMetadata>? ManyToOneRelationships { get; set; }
        [JsonProperty]
        public List<OneToManyRelationshipMetadata>? OneToManyRelationships { get; set; }
    }
}