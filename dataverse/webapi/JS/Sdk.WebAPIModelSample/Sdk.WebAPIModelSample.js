// =====================================================================
//  This file is part of the Microsoft Dynamics CRM SDK code samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or on-line documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
// =====================================================================

"use strict";
var Sdk = window.Sdk || { __namespace: true };
Sdk.WebAPIModelSample = Sdk.WebAPIModelSample || { __namespace: true };
(function () {

    // Entities
    this.MetadataBase = function () {

        var metadataid,hasChanged;
        Object.defineProperties(this,
            {
                "MetadataId": {
                    get: function () { return metadataid; },
                    set: function (value) {
                        if (isString(value)) {
                            metadataid = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.MetadataBase.MetadataId is a string value.")
                        }
                    },
                    enumerable: true
                },
                "HasChanged": {
                    get: function () { return hasChanged; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            hasChanged = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.MetadataBase.HasChanged is a boolean value.")
                        }
                    },
                    enumerable: true
                }
            });

    }
    this.OptionSetMetadataBase = function () {
        Sdk.WebAPIModelSample.MetadataBase.call(this);
        var description,displayName,introducedVersion,isCustomizable,isCustomOptionSet,isGlobal,isManaged,name,optionSetType;
        Object.defineProperties(this,
                    {
                        "Description": {
                            get: function () { return description; },
                            set: function (value) {
                                if (isLabel(value)) {
                                    description = value;
                                }
                                else {
                                    throw new Error("Sdk.WebAPIModelSample.OptionSetMetadataBase.Description is a Sdk.WebAPIModelSample.Label value.")
                                }
                            },
                            enumerable: true
                        },
                        "DisplayName": {
                            get: function () { return displayName; },
                            set: function (value) {
                                if (isLabel(value)) {
                                    displayName = value;
                                }
                                else {
                                    throw new Error("Sdk.WebAPIModelSample.OptionSetMetadataBase.DisplayName is a Sdk.WebAPIModelSample.Label value.")
                                }
                            },
                            enumerable: true
                        },
                        "IntroducedVersion": {
                            get: function () { return introducedVersion; },
                            set: function (value) {
                                if (isString(value)) {
                                    introducedVersion = value;
                                }
                                else {
                                    throw new Error("Sdk.WebAPIModelSample.OptionSetMetadataBase.IntroducedVersion is a string value.")
                                }
                            },
                            enumerable: true
                        },
                        "IsCustomizable": {
                            get: function () { return isCustomizable; },
                            set: function (value) {
                                if (isBooleanManagedProperty(value)) {
                                    isCustomizable = value;
                                }
                                else {
                                    throw new Error("Sdk.WebAPIModelSample.OptionSetMetadataBase.IsCustomizable is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                                }
                            },
                            enumerable: true
                        },
                        "IsCustomOptionSet": {
                            get: function () { return isCustomOptionSet; },
                            set: function (value) {
                                if (isBoolean(value)) {
                                    isCustomOptionSet = value;
                                }
                                else {
                                    throw new Error("Sdk.WebAPIModelSample.OptionSetMetadataBase.IsCustomOptionSet is a boolean value.")
                                }
                            },
                            enumerable: true
                        },
                        "IsGlobal": {
                            get: function () { return isManaged; },
                            set: function (value) {
                                if (isBoolean(value)) {
                                    isManaged = value;
                                }
                                else {
                                    throw new Error("Sdk.WebAPIModelSample.OptionSetMetadataBase.IsGlobal is a boolean value.")
                                }
                            },
                            enumerable: true
                        },
                        "IsManaged": {
                            get: function () { return isGlobal; },
                            set: function (value) {
                                if (isBoolean(value)) {
                                    isGlobal = value;
                                }
                                else {
                                    throw new Error("Sdk.WebAPIModelSample.OptionSetMetadataBase.IsManaged is a boolean value.")
                                }
                            },
                            enumerable: true
                        },
                        "Name": {
                            get: function () { return name; },
                            set: function (value) {
                                if (isString(value)) {
                                    name = value;
                                }
                                else {
                                    throw new Error("Sdk.WebAPIModelSample.OptionSetMetadataBase.Name is a string value.")
                                }
                            },
                            enumerable: true
                        },
                        "OptionSetType": {
                            get: function () { return optionSetType; },
                            set: function (value) {
                                if (isOptionSetType(value)) {
                                    optionSetType = value;
                                }
                                else {
                                    throw new Error("Sdk.WebAPIModelSample.OptionSetMetadataBase.OptionSetType is a Sdk.WebAPIModelSample.OptionSetType value.")
                                }
                            },
                            enumerable: true
                        }
                    });
    }
    this.OptionSetMetadata = function () {
        Sdk.WebAPIModelSample.OptionSetMetadataBase.call(this);
        var options;
        Object.defineProperties(this,
            {
                "Options": {
                    get: function () { return metadataid; },
                    set: function (value) {
                        if (isArrayOfOptionMetadata(value)) {
                            metadataid = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.OptionSetMetadata.Options is an array of Sdk.WebAPIModelSample.OptionMetadata values.")
                        }
                    },
                    enumerable: true
                }
            });
    }
    this.BooleanOptionSetMetadata = function () {
        Sdk.WebAPIModelSample.OptionSetMetadataBase.call(this);
        var falseOption,trueOption;
        Object.defineProperties(this,
    {
        "FalseOption": {
            get: function () { return falseOption; },
            set: function (value) {
                if (isOptionMetadata(value)) {
                    falseOption = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.BooleanOptionSetMetadata.FalseOption is a Sdk.WebAPIModelSample.OptionMetadata value.")
                }
            },
            enumerable: true
        },
        "TrueOption": {
            get: function () { return trueOption; },
            set: function (value) {
                if (isOptionMetadata(value)) {
                    trueOption = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.BooleanOptionSetMetadata.TrueOption is a Sdk.WebAPIModelSample.OptionMetadata value.")
                }
            },
            enumerable: true
        }
    });

    }
    this.EntityKeyMetadata = function () {
        Sdk.WebAPIModelSample.MetadataBase.call(this);
        var asyncJob,displayName,entityKeyIndexStatus,entityLogicalName,introducedVersion,isCustomizable,isManaged,keyAttributes,logicalName,schemaName;
        Object.defineProperties(this,
            {
                "AsyncJob": {
                    get: function () { return asyncJob; },
                    set: function (value) {
                        if (isEntityReference(value)) {
                            asyncJob = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityKeyMetadata.AsyncJob is a Sdk.WebAPIModelSample.EntityReference value.")
                        }
                    },
                    enumerable: true
                },
                "DisplayName": {
                    get: function () { return displayName; },
                    set: function (value) {
                        if (isLabel(value)) {
                            displayName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityKeyMetadata.DisplayName is a Sdk.WebAPIModelSample.Label value.")
                        }
                    },
                    enumerable: true
                },
                "EntityKeyIndexStatus": {
                    get: function () { return entityKeyIndexStatus; },
                    set: function (value) {
                        if (isEntityKeyIndexStatus(value)) {
                            entityKeyIndexStatus = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityKeyMetadata.EntityKeyIndexStatus is a Sdk.WebAPIModelSample.EntityKeyIndexStatus value.")
                        }
                    },
                    enumerable: true
                },
                "EntityLogicalName": {
                    get: function () { return entityLogicalName; },
                    set: function (value) {
                        if (isString(value)) {
                            entityLogicalName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityKeyMetadata.EntityLogicalName is a string value.")
                        }
                    },
                    enumerable: true
                },
                "IntroducedVersion": {
                    get: function () { return introducedVersion; },
                    set: function (value) {
                        if (isString(value)) {
                            introducedVersion = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityKeyMetadata.IntroducedVersion is a string value.")
                        }
                    },
                    enumerable: true
                },
                "IsCustomizable": {
                    get: function () { return isCustomizable; },
                    set: function (value) {
                        if (isBooleanManagedProperty(value)) {
                            isCustomizable = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityKeyMetadata.IsCustomizable is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                        }
                    },
                    enumerable: true
                },
                "IsManaged": {
                    get: function () { return isManaged; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isManaged = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityKeyMetadata.IsManaged is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "KeyAttributes": {
                    get: function () { return keyAttributes; },
                    set: function (value) {
                        if (isArrayOfString(value)) {
                            keyAttributes = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityKeyMetadata.KeyAttributes is an array of string values.")
                        }
                    },
                    enumerable: true
                },
                "LogicalName": {
                    get: function () { return logicalName; },
                    set: function (value) {
                        if (isString(value)) {
                            logicalName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityKeyMetadata.LogicalName is a string value.")
                        }
                    },
                    enumerable: true
                },
                "SchemaName": {
                    get: function () { return schemaName; },
                    set: function (value) {
                        if (isString(value)) {
                            schemaName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityKeyMetadata.SchemaName is a string value.")
                        }
                    },
                    enumerable: true
                }
            });
    }
    this.RelationshipMetadataBase = function () {
        Sdk.WebAPIModelSample.MetadataBase.call(this);
        var introducedVersion,isCustomizable,isCustomRelationship,isManaged,isValidForAdvancedFind,relationshipType,schemaName,securityTypes;
        Object.defineProperties(this,
    {
        "IntroducedVersion": {
            get: function () { return introducedVersion; },
            set: function (value) {
                if (isString(value)) {
                    introducedVersion = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.RelationshipMetadataBase.introducedVersion is a string value.");
                }
            },
            enumerable: true
        },
        "IsCustomizable": {
            get: function () { return isCustomizable; },
            set: function (value) {
                if (isBooleanManagedProperty(value)) {
                    value.ManagedPropertyLogicalName = "iscustomizable";
                    isCustomizable = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.RelationshipMetadataBase.IsCustomizable is a Sdk.WebAPIModelSample.BooleanManagedProperty value.");
                }
            },
            enumerable: true
        },
        "IsCustomRelationship": {
            get: function () { return isCustomRelationship; },
            set: function (value) {
                if (isBoolean(value)) {
                    isCustomRelationship = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.RelationshipMetadataBase.IsCustomRelationship is a boolean value.");
                }
            },
            enumerable: true
        },
        "IsManaged": {
            get: function () { return isManaged; },
            set: function (value) {
                if (isBoolean(value)) {
                    isManaged = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.RelationshipMetadataBase.IsManaged is a boolean value.");
                }
            },
            enumerable: true
        },
        "IsValidForAdvancedFind": {
            get: function () { return isValidForAdvancedFind; },
            set: function (value) {
                if (isBoolean(value)) {
                    isValidForAdvancedFind = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.RelationshipMetadataBase.IsValidForAdvancedFind is a boolean value.");
                }
            },
            enumerable: true
        },
        "RelationshipType": {
            get: function () { return relationshipType; },
            set: function (value) {
                if (isRelationshipType(value)) {
                    relationshipType = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.RelationshipMetadataBase.RelationshipType is a Sdk.WebAPIModelSample.RelationshipType value.");
                }
            },
            enumerable: true
        },
        "SchemaName": {
            get: function () { return schemaName; },
            set: function (value) {
                if (isString(value)) {
                    schemaName = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.RelationshipMetadataBase.SchemaName is a string value.");
                }
            },
            enumerable: true
        },
        "SecurityTypes": {
            get: function () { return securityTypes; },
            set: function (value) {
                if (isSecurityTypes(value)) {
                    securityTypes = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.RelationshipMetadataBase.SecurityTypes is a Sdk.WebAPIModelSample.SecurityTypes value.");
                }
            },
            enumerable: true
        }
    });

    }
    this.OneToManyRelationshipMetadata = function () {
        Sdk.WebAPIModelSample.RelationshipMetadataBase.call(this);
        var lookup,associatedMenuConfiguration,cascadeConfiguration,isHierarchical,referencedAttribute,referencedEntity,referencedEntityNavigationPropertyName,referencingAttribute,referencingEntity,referencingEntityNavigationPropertyName;

        Object.defineProperties(this,
            {
                "@odata.type": {
                    get: function () { return "Microsoft.Dynamics.CRM.OneToManyRelationshipMetadata"; },
                    enumerable: true
                },
                "AssociatedMenuConfiguration": {
                    get: function () { return associatedMenuConfiguration; },
                    set: function (value) {
                        if (isAssociatedMenuConfiguration(value)) {
                            associatedMenuConfiguration = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.OneToManyRelationshipMetadata.AssociatedMenuConfiguration is a Sdk.WebAPIModelSample.AssociatedMenuConfiguration value.");
                        }
                    },
                    enumerable: true
                },
                "CascadeConfiguration": {
                    get: function () { return cascadeConfiguration; },
                    set: function (value) {
                        if (isCascadeConfiguration(value)) {
                            cascadeConfiguration = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.OneToManyRelationshipMetadata.CascadeConfiguration is a Sdk.WebAPIModelSample.CascadeConfiguration value.");
                        }
                    },
                    enumerable: true
                },
                "IsHierarchical": {
                    get: function () { return isHierarchical; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isHierarchical = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.OneToManyRelationshipMetadata.IsHierarchical is a boolean value.");
                        }
                    },
                    enumerable: true
                },
                "ReferencedAttribute": {
                    get: function () { return referencedAttribute; },
                    set: function (value) {
                        if (isString(value)) {
                            referencedAttribute = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.OneToManyRelationshipMetadata.ReferencedAttribute is a string value.");
                        }
                    },
                    enumerable: true
                },
                "ReferencedEntity": {
                    get: function () { return referencedEntity; },
                    set: function (value) {
                        if (isString(value)) {
                            referencedEntity = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.OneToManyRelationshipMetadata.ReferencedEntity is a string value.");
                        }
                    },
                    enumerable: true
                },
                "ReferencedEntityNavigationPropertyName": {
                    get: function () { return referencedEntityNavigationPropertyName; },
                    set: function (value) {
                        if (isString(value)) {
                            referencedEntityNavigationPropertyName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.OneToManyRelationshipMetadata.ReferencedEntityNavigationPropertyName is a string value.");
                        }
                    },
                    enumerable: true
                },
                "ReferencingAttribute": {
                    get: function () { return referencingAttribute; },
                    set: function (value) {
                        if (isString(value)) {
                            referencingAttribute = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.OneToManyRelationshipMetadata.ReferencingAttribute is a string value.");
                        }
                    },
                    enumerable: true
                },
                "ReferencingEntity": {
                    get: function () { return referencingEntity; },
                    set: function (value) {
                        if (isString(value)) {
                            referencingEntity = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.OneToManyRelationshipMetadata.ReferencingEntity is a string value.");
                        }
                    },
                    enumerable: true
                },
                "ReferencingEntityNavigationPropertyName": {
                    get: function () { return referencingEntityNavigationPropertyName; },
                    set: function (value) {
                        if (isString(value)) {
                            referencingEntityNavigationPropertyName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.OneToManyRelationshipMetadata.ReferencingEntityNavigationPropertyName is a string value.");
                        }
                    },
                    enumerable: true
                },
                "Lookup": {                   
                    get: function () { return lookup; },
                    set: function (value) {
                        if (isLookupAttributeMetadata(value)) {
                            lookup = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.OneToManyRelationshipMetadata.Lookup is a Sdk.WebAPIModelSample.LookupAttributeMetadata value.");
                        }
                    },
                    enumerable: true
                }
            });
    }
    this.ManyToManyRelationshipMetadata = function () {
        Sdk.WebAPIModelSample.RelationshipMetadataBase.call(this);
        var entity1AssociatedMenuConfiguration,entity1IntersectAttribute,entity1LogicalName,entity1NavigationPropertyName,entity2AssociatedMenuConfiguration,entity2IntersectAttribute,entity2LogicalName,entity2NavigationPropertyName,intersectEntityName;
        Object.defineProperties(this,
            {
                "@odata.type": {
                    get: function () { return "Microsoft.Dynamics.CRM.ManyToManyRelationshipMetadata"; },
                    enumerable: true
                },
                "Entity1AssociatedMenuConfiguration": {
                    get: function () { return entity1AssociatedMenuConfiguration; },
                    set: function (value) {
                        if (isAssociatedMenuConfiguration(value)) {
                            entity1AssociatedMenuConfiguration = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.ManyToManyRelationshipMetadata.Entity1AssociatedMenuConfiguration is a Sdk.WebAPIModelSample.AssociatedMenuConfiguration value.")
                        }
                    },
                    enumerable: true
                },
                "Entity1IntersectAttribute": {
                    get: function () { return entity1IntersectAttribute; },
                    set: function (value) {
                        if (isString(value)) {
                            entity1IntersectAttribute = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.ManyToManyRelationshipMetadata.Entity1IntersectAttribute is a string value.")
                        }
                    },
                    enumerable: true
                },
                "Entity1LogicalName": {
                    get: function () { return entity1LogicalName; },
                    set: function (value) {
                        if (isString(value)) {
                            entity1LogicalName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.ManyToManyRelationshipMetadata.Entity1LogicalName is a string value.")
                        }
                    },
                    enumerable: true
                },
                "Entity1NavigationPropertyName": {
                    get: function () { return entity1NavigationPropertyName; },
                    set: function (value) {
                        if (isString(value)) {
                            entity1NavigationPropertyName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.ManyToManyRelationshipMetadata.Entity1NavigationPropertyName is a string value.")
                        }
                    },
                    enumerable: true
                },
                "Entity2AssociatedMenuConfiguration": {
                    get: function () { return entity2AssociatedMenuConfiguration; },
                    set: function (value) {
                        if (isAssociatedMenuConfiguration(value)) {
                            entity2AssociatedMenuConfiguration = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.ManyToManyRelationshipMetadata.Entity2AssociatedMenuConfiguration is a Sdk.WebAPIModelSample.AssociatedMenuConfiguration value.")
                        }
                    },
                    enumerable: true
                },
                "Entity2IntersectAttribute": {
                    get: function () { return entity2IntersectAttribute; },
                    set: function (value) {
                        if (isString(value)) {
                            entity2IntersectAttribute = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.ManyToManyRelationshipMetadata.Entity2IntersectAttribute is a string value.")
                        }
                    },
                    enumerable: true
                },
                "Entity2LogicalName": {
                    get: function () { return entity2LogicalName; },
                    set: function (value) {
                        if (isString(value)) {
                            entity2LogicalName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.ManyToManyRelationshipMetadata.Entity2LogicalName is a string value.")
                        }
                    },
                    enumerable: true
                },
                "Entity2NavigationPropertyName": {
                    get: function () { return entity2NavigationPropertyName; },
                    set: function (value) {
                        if (isString(value)) {
                            entity2NavigationPropertyName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.ManyToManyRelationshipMetadata.Entity2NavigationPropertyName is a string value.")
                        }
                    },
                    enumerable: true
                },
                "IntersectEntityName": {
                    get: function () { return intersectEntityName; },
                    set: function (value) {
                        if (isString(value)) {
                            intersectEntityName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.ManyToManyRelationshipMetadata.IntersectEntityName is a string value.")
                        }
                    },
                    enumerable: true
                }
            });
    }
    this.EntityMetadata = function () {
        Sdk.WebAPIModelSample.MetadataBase.call(this);
        var attributes;
        var manyToManyRelationships;
        var manyToOneRelationships;
        var oneToManyRelationships;
        var activityTypeMask;
        var autoCreateAccessTeams;
        var autoRouteToOwnerQueue;
        var canBeInManyToMany;
        var canBePrimaryEntityInRelationship;
        var canBeRelatedEntityInRelationship;
        var canChangeHierarchicalRelationship;
        var canCreateAttributes;
        var canCreateCharts;
        var canCreateForms;
        var canCreateViews;
        var canEnableSyncToExternalSearchIndex;
        var canModifyAdditionalSettings;
        var canTriggerWorkflow;
        var changeTrackingEnabled;
        var collectionSchemaName;
        var daysSinceRecordLastModified;
        var description;
        var displayCollectionName;
        var displayName;
        var enforceStateTransitions;
        var entityColor;
        var entityHelpUrl;
        var entityHelpUrlEnabled;
        var entitySetName;
        var hasActivities;
        var hasNotes;
        var iconLargeName;
        var iconMediumName;
        var iconSmallName;
        var introducedVersion;
        var isActivity;
        var isActivityParty;
        var isAIRUpdated;
        var isAuditEnabled;
        var isAvailableOffline;
        var isBusinessProcessEnabled;
        var isChildEntity;
        var isConnectionsEnabled;
        var isCustomEntity;
        var isCustomizable;
        var isDocumentManagementEnabled;
        var isDuplicateDetectionEnabled;
        var isEnabledForCharts;
        var isEnabledForExternalChannels;
        var isEnabledForTrace;
        var isImportable;
        var isInteractionCentricEnabled;
        var isIntersect;
        var isKnowledgeManagementEnabled;
        var isMailMergeEnabled;
        var isManaged;
        var isMappable;
        var isOfflineInMobileClient;
        var isOneNoteIntegrationEnabled;
        var isOptimisticConcurrencyEnabled;
        var isPrivate;
        var isQuickCreateEnabled;
        var isReadingPaneEnabled;
        var isReadOnlyInMobileClient;
        var isRenameable;
        var isStateModelAware;
        var isValidForAdvancedFind;
        var isValidForQueue;
        var isVisibleInMobile;
        var isVisibleInMobileClient;
        var logicalCollectionName;
        var logicalName;
        var objectTypeCode;
        var ownershipType;
        var primaryIdAttribute;
        var primaryImageAttribute;
        var primaryNameAttribute;
        var privileges;
        var recurrenceBaseEntityLogicalName;
        var reportViewName;
        var schemaName;
        var syncToExternalSearchIndex;

        Object.defineProperties(this,
            {

                "@odata.type": {
                    get: function () { return "Microsoft.Dynamics.CRM.EntityMetadata"; },
                    enumerable: true
                },
                "Attributes": {
                    get: function () { return attributes; },
                    set: function (value) {
                        if (isArrayOfAttributeMetadata(value)) {
                            attributes = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.Attributes is an array of Sdk.WebAPIModelSample.AttributeMetadata values.")
                        }
                    },
                    enumerable: true
                },
                //TODO: Keys
                "ManyToManyRelationships": {
                    get: function () { return manyToManyRelationships; },
                    set: function (value) {
                        if (isArrayOfManyToManyRelationshipMetadata(value)) {
                            manyToManyRelationships = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.ManyToManyRelationships is an array of Sdk.WebAPIModelSample.ManyToManyRelationshipMetadata values.")
                        }
                    },
                    enumerable: true
                },
                "ManyToOneRelationships": {
                    get: function () { return manyToOneRelationships; },
                    set: function (value) {
                        if (isArrayOfOneToManyRelationshipMetadata(value)) {
                            manyToOneRelationships = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.Attributes is an array of Sdk.WebAPIModelSample.OneToManyRelationshipMetadata values.")
                        }
                    },
                    enumerable: true
                },
                "OneToManyRelationships": {
                    get: function () { return oneToManyRelationships; },
                    set: function (value) {
                        if (isArrayOfOneToManyRelationshipMetadata(value)) {
                            oneToManyRelationships = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.Attributes is an array of Sdk.WebAPIModelSample.OneToManyRelationshipMetadata values.")
                        }
                    },
                    enumerable: true
                },
                "ActivityTypeMask": {
                    get: function () { return activityTypeMask; },
                    set: function (value) {
                        if (isNumber(value)) {
                            activityTypeMask = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.ActivityTypeMask is a number value.")
                        }
                    },
                    enumerable: true
                },
                "AutoCreateAccessTeams": {
                    get: function () { return autoCreateAccessTeams; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            autoCreateAccessTeams = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.AutoCreateAccessTeams is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "AutoRouteToOwnerQueue": {
                    get: function () { return autoRouteToOwnerQueue; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            autoRouteToOwnerQueue = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.AutoRouteToOwnerQueue is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "CanBeInManyToMany": {
                    get: function () { return canBeInManyToMany; },
                    set: function (value) {
                        if (isBooleanManagedProperty(value)) {
                            value.ManagedPropertyLogicalName = "canbeinmanytomany";
                            canBeInManyToMany = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.CanBeInManyToMany is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                        }
                    },
                    enumerable: true
                },
                "CanBePrimaryEntityInRelationship": {
                    get: function () { return canBePrimaryEntityInRelationship; },
                    set: function (value) {
                        if (isBooleanManagedProperty(value)) {
                            value.ManagedPropertyLogicalName = "canbeprimaryentityinrelationship";
                            canBePrimaryEntityInRelationship = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.CanBePrimaryEntityInRelationship is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                        }
                    },
                    enumerable: true
                },
                "CanBeRelatedEntityInRelationship": {
                    get: function () { return canBeRelatedEntityInRelationship; },
                    set: function (value) {
                        if (isBooleanManagedProperty(value)) {
                            value.ManagedPropertyLogicalName = "canberelatedentityinrelationship";
                            canBeRelatedEntityInRelationship = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.CanBeRelatedEntityInRelationship is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                        }
                    },
                    enumerable: true
                },
                "CanChangeHierarchicalRelationship": {
                    get: function () { return canChangeHierarchicalRelationship; },
                    set: function (value) {
                        if (isBooleanManagedProperty(value)) {
                            value.ManagedPropertyLogicalName = "canchangehierarchicalrelationship";
                            canChangeHierarchicalRelationship = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.CanChangeHierarchicalRelationship is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                        }
                    },
                    enumerable: true
                },
                "CanCreateAttributes": {
                    get: function () { return canCreateAttributes; },
                    set: function (value) {
                        if (isBooleanManagedProperty(value)) {
                            value.ManagedPropertyLogicalName = "cancreateattributes";
                            canCreateAttributes = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.CanCreateAttributes is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                        }
                    },
                    enumerable: true
                },
                "CanCreateCharts": {
                    get: function () { return canCreateCharts; },
                    set: function (value) {
                        if (isBooleanManagedProperty(value)) {
                            value.ManagedPropertyLogicalName = "cancreatecharts";
                            canCreateCharts = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.CanCreateCharts is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                        }
                    },
                    enumerable: true
                },
                "CanCreateForms": {
                    get: function () { return canCreateForms; },
                    set: function (value) {
                        if (isBooleanManagedProperty(value)) {
                            value.ManagedPropertyLogicalName = "cancreateforms";
                            canCreateForms = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.CanCreateForms is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                        }
                    },
                    enumerable: true
                },
                "CanCreateViews": {
                    get: function () { return canCreateViews; },
                    set: function (value) {
                        if (isBooleanManagedProperty(value)) {
                            value.ManagedPropertyLogicalName = "cancreateviews";
                            canCreateViews = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.CanCreateViews is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                        }
                    },
                    enumerable: true
                },
                "CanEnableSyncToExternalSearchIndex": {
                    get: function () { return canEnableSyncToExternalSearchIndex; },
                    set: function (value) {
                        if (isBooleanManagedProperty(value)) {
                            value.ManagedPropertyLogicalName = "canenablesynctoexternalsearchindex";
                            canEnableSyncToExternalSearchIndex = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.CanEnableSyncToExternalSearchIndex is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                        }
                    },
                    enumerable: true
                },
                "CanModifyAdditionalSettings": {
                    get: function () { return canModifyAdditionalSettings; },
                    set: function (value) {
                        if (isBooleanManagedProperty(value)) {
                            value.ManagedPropertyLogicalName = "canmodifyadditionalsettings";
                            canModifyAdditionalSettings = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.CanModifyAdditionalSettings is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                        }
                    },
                    enumerable: true
                },
                "CanTriggerWorkflow": {
                    get: function () { return canTriggerWorkflow; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            canTriggerWorkflow = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.CanTriggerWorkflow is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "ChangeTrackingEnabled": {
                    get: function () { return changeTrackingEnabled; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            changeTrackingEnabled = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.ChangeTrackingEnabled is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "CollectionSchemaName": {
                    get: function () { return collectionSchemaName; },
                    set: function (value) {
                        if (isString(value)) {
                            collectionSchemaName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.CollectionSchemaName is a string value.")
                        }
                    },
                    enumerable: true
                },
                "DaysSinceRecordLastModified": {
                    get: function () { return daysSinceRecordLastModified; },
                    set: function (value) {
                        if (isNumber(value)) {
                            daysSinceRecordLastModified = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.DaysSinceRecordLastModified is a number value.")
                        }
                    },
                    enumerable: true
                },
                "Description": {
                    get: function () { return description; },
                    set: function (value) {
                        if (isLabel(value)) {
                            description = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.Description is a Sdk.WebAPIModelSample.Label value.")
                        }
                    },
                    enumerable: true
                },
                "DisplayCollectionName": {
                    get: function () { return displayCollectionName; },
                    set: function (value) {
                        if (isLabel(value)) {
                            displayCollectionName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.DisplayCollectionName is a Sdk.WebAPIModelSample.Label value.")
                        }
                    },
                    enumerable: true
                },
                "DisplayName": {
                    get: function () { return displayName; },
                    set: function (value) {
                        if (isLabel(value)) {
                            displayName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.DisplayName is a Sdk.WebAPIModelSample.Label value.")
                        }
                    },
                    enumerable: true
                },
                "EnforceStateTransitions": {
                    get: function () { return enforceStateTransitions; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            enforceStateTransitions = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.EnforceStateTransitions is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "EntityColor": {
                    get: function () { return entityColor; },
                    set: function (value) {
                        if (isString(value)) {
                            entityColor = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.EntityColor is a string value.")
                        }
                    },
                    enumerable: true
                },
                "EntityHelpUrl": {
                    get: function () { return entityHelpUrl; },
                    set: function (value) {
                        if (isString(value)) {
                            entityHelpUrl = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.EntityHelpUrl is a string value.")
                        }
                    },
                    enumerable: true
                },
                "EntityHelpUrlEnabled": {
                    get: function () { return entityHelpUrlEnabled; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            entityHelpUrlEnabled = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.EntityHelpUrlEnabled is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "EntitySetName": {
                    get: function () { return entitySetName; },
                    set: function (value) {
                        if (isString(value)) {
                            entitySetName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.EntitySetName is a string value.")
                        }
                    },
                    enumerable: true
                },
                "HasActivities": {
                    get: function () { return hasActivities; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            hasActivities = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.HasActivities is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "HasNotes": {
                    get: function () { return hasNotes; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            hasNotes = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.HasNotes is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IconLargeName": {
                    get: function () { return iconLargeName; },
                    set: function (value) {
                        if (isString(value)) {
                            iconLargeName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IconLargeName is a string value.")
                        }
                    },
                    enumerable: true
                },
                "IconMediumName": {
                    get: function () { return iconMediumName; },
                    set: function (value) {
                        if (isString(value)) {
                            iconMediumName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IconMediumName is a string value.")
                        }
                    },
                    enumerable: true
                },
                "IconSmallName": {
                    get: function () { return iconSmallName; },
                    set: function (value) {
                        if (isString(value)) {
                            iconSmallName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IconSmallName is a string value.")
                        }
                    },
                    enumerable: true
                },
                "IntroducedVersion": {
                    get: function () { return introducedVersion; },
                    set: function (value) {
                        if (isString(value)) {
                            introducedVersion = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IntroducedVersion is a string value.")
                        }
                    },
                    enumerable: true
                },
                "IsActivity": {
                    get: function () { return isActivity; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isActivity = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsActivity is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IsActivityParty": {
                    get: function () { return isActivityParty; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isActivityParty = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsActivityParty is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IsAIRUpdated": {
                    get: function () { return isAIRUpdated; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isAIRUpdated = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsAIRUpdated is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IsAuditEnabled": {
                    get: function () { return isAuditEnabled; },
                    set: function (value) {
                        if (isBooleanManagedProperty(value)) {
                            value.ManagedPropertyLogicalName = "canmodifyauditsettings";
                            isAuditEnabled = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsAuditEnabled is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                        }
                    },
                    enumerable: true
                },
                "IsAvailableOffline": {
                    get: function () { return isAvailableOffline; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isAvailableOffline = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsAvailableOffline is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IsBusinessProcessEnabled": {
                    get: function () { return isBusinessProcessEnabled; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isBusinessProcessEnabled = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsBusinessProcessEnabled is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IsChildEntity": {
                    get: function () { return isChildEntity; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isChildEntity = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsChildEntity is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IsConnectionsEnabled": {
                    get: function () { return isConnectionsEnabled; },
                    set: function (value) {
                        if (isBooleanManagedProperty(value)) {
                            value.ManagedPropertyLogicalName = "canmodifyconnectionsettings";
                            isConnectionsEnabled = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsConnectionsEnabled is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                        }
                    },
                    enumerable: true
                },
                "IsCustomEntity": {
                    get: function () { return isCustomEntity; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isCustomEntity = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsCustomEntity is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IsCustomizable": {
                    get: function () { return isCustomizable; },
                    set: function (value) {
                        if (isBooleanManagedProperty(value)) {
                            value.ManagedPropertyLogicalName = "iscustomizable";
                            isCustomizable = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsCustomizable is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                        }
                    },
                    enumerable: true
                },
                "IsDocumentManagementEnabled": {
                    get: function () { return isDocumentManagementEnabled; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isDocumentManagementEnabled = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsDocumentManagementEnabled is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IsDuplicateDetectionEnabled": {
                    get: function () { return isDuplicateDetectionEnabled; },
                    set: function (value) {
                        if (isBooleanManagedProperty(value)) {
                            value.ManagedPropertyLogicalName = "canmodifyduplicatedetectionsettings";
                            isDuplicateDetectionEnabled = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsDuplicateDetectionEnabled is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                        }
                    },
                    enumerable: true
                },
                "IsEnabledForCharts": {
                    get: function () { return isEnabledForCharts; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isEnabledForCharts = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsEnabledForCharts is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IsEnabledForExternalChannels": {
                    get: function () { return isEnabledForExternalChannels; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isEnabledForExternalChannels = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsEnabledForExternalChannels is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IsEnabledForTrace": {
                    get: function () { return isEnabledForTrace; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isEnabledForTrace = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsEnabledForTrace is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IsImportable": {
                    get: function () { return isImportable; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isImportable = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsImportable is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IsInteractionCentricEnabled": {
                    get: function () { return isInteractionCentricEnabled; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isInteractionCentricEnabled = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsInteractionCentricEnabled is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IsIntersect": {
                    get: function () { return isIntersect; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isIntersect = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsIntersect is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IsKnowledgeManagementEnabled": {
                    get: function () { return isKnowledgeManagementEnabled; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isKnowledgeManagementEnabled = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsKnowledgeManagementEnabled is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IsMailMergeEnabled": {
                    get: function () { return isMailMergeEnabled; },
                    set: function (value) {
                        if (isBooleanManagedProperty(value)) {
                            value.ManagedPropertyLogicalName = "canmodifymailmergesettings";
                            isMailMergeEnabled = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsMailMergeEnabled is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                        }
                    },
                    enumerable: true
                },
                "IsManaged": {
                    get: function () { return isManaged; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isManaged = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsManaged is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IsMappable": {
                    get: function () { return isMappable; },
                    set: function (value) {
                        if (isBooleanManagedProperty(value)) {
                            value.ManagedPropertyLogicalName = "ismappable";
                            isMappable = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsMappable is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                        }
                    },
                    enumerable: true
                },
                "IsOfflineInMobileClient": {
                    get: function () { return isOfflineInMobileClient; },
                    set: function (value) {
                        if (isBooleanManagedProperty(value)) {
                            value.ManagedPropertyLogicalName = "canmodifymobileclientoffline";
                            isOfflineInMobileClient = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsOfflineInMobileClient is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                        }
                    },
                    enumerable: true
                },
                "IsOneNoteIntegrationEnabled": {
                    get: function () { return isOneNoteIntegrationEnabled; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isOneNoteIntegrationEnabled = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsOneNoteIntegrationEnabled is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IsOptimisticConcurrencyEnabled": {
                    get: function () { return isOptimisticConcurrencyEnabled; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isOptimisticConcurrencyEnabled = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsOptimisticConcurrencyEnabled is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IsPrivate": {
                    get: function () { return isPrivate; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isPrivate = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsPrivate is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IsQuickCreateEnabled": {
                    get: function () { return isQuickCreateEnabled; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isQuickCreateEnabled = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsQuickCreateEnabled is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IsReadingPaneEnabled": {
                    get: function () { return isReadingPaneEnabled; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isReadingPaneEnabled = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsReadingPaneEnabled is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IsReadOnlyInMobileClient": {
                    get: function () { return isReadOnlyInMobileClient; },
                    set: function (value) {
                        if (isBooleanManagedProperty(value)) {
                            value.ManagedPropertyLogicalName = "canmodifymobileclientreadonly";
                            isReadOnlyInMobileClient = value;
                        }
                        else
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsReadOnlyInMobileClient is a Sdk.WebAPIModelSample.BooleanManagedProperty value.");
                    },
                    enumerable: true
                },
                "IsRenameable": {
                    get: function () { return isRenameable; },
                    set: function (value) {
                        if (isBooleanManagedProperty(value)) {
                            value.ManagedPropertyLogicalName = "isrenameable";
                            isRenameable = value;
                        }
                        else
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsRenameable is a Sdk.WebAPIModelSample.BooleanManagedProperty value.");
                    },
                    enumerable: true
                },
                "IsStateModelAware": {
                    get: function () { return isStateModelAware; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isStateModelAware = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsStateModelAware is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IsValidForAdvancedFind": {
                    get: function () { return isValidForAdvancedFind; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isValidForAdvancedFind = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsValidForAdvancedFind is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "IsValidForQueue": {
                    get: function () { return isValidForQueue; },
                    set: function (value) {
                        if (isBooleanManagedProperty(value)) {
                            value.ManagedPropertyLogicalName = "canmodifyqueuesettings";
                            isValidForQueue = value;
                        }
                        else
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsValidForQueue is a Sdk.WebAPIModelSample.BooleanManagedProperty value.");
                    },
                    enumerable: true
                },
                "IsVisibleInMobile": {
                    get: function () { return isVisibleInMobile; },
                    set: function (value) {
                        if (isBooleanManagedProperty(value)) {
                            value.ManagedPropertyLogicalName = "canmodifymobilevisibility";
                            isVisibleInMobile = value;
                        }
                        else
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsVisibleInMobile is a Sdk.WebAPIModelSample.BooleanManagedProperty value.");
                    },
                    enumerable: true
                },
                "IsVisibleInMobileClient": {
                    get: function () { return isVisibleInMobileClient; },
                    set: function (value) {
                        if (isBooleanManagedProperty(value)) {
                            value.ManagedPropertyLogicalName = "canmodifymobileclientvisibility";
                            isVisibleInMobileClient = value;
                        }
                        else
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.IsVisibleInMobileClient is a Sdk.WebAPIModelSample.BooleanManagedProperty value.");
                    },
                    enumerable: true
                },
                "LogicalCollectionName": {
                    get: function () { return logicalCollectionName; },
                    set: function (value) {
                        if (isString(value)) {
                            logicalCollectionName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.LogicalCollectionName is a string value.")
                        }
                    },
                    enumerable: true
                },
                "LogicalName": {
                    get: function () { return logicalName; },
                    set: function (value) {
                        if (isString(value)) {
                            logicalName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.LogicalName is a string value.")
                        }
                    },
                    enumerable: true
                },
                "ObjectTypeCode": {
                    get: function () { return objectTypeCode; },
                    set: function (value) {
                        if (isNumber(value)) {
                            objectTypeCode = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.ObjectTypeCode is a number value.")
                        }
                    },
                    enumerable: true
                },
                "OwnershipType": {
                    get: function () { return ownershipType; },
                    set: function (value) {
                        if (isOwnershipType(value)) {
                            ownershipType = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.OwnershipType is a Sdk.WebAPIModelSample.OwnershipTypes value.")
                        }
                    },
                    enumerable: true
                },
                "PrimaryIdAttribute": {
                    get: function () { return primaryIdAttribute; },
                    set: function (value) {
                        if (isString(value)) {
                            primaryIdAttribute = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.PrimaryIdAttribute is a string value.")
                        }
                    },
                    enumerable: true
                },
                "PrimaryImageAttribute": {
                    get: function () { return primaryImageAttribute; },
                    set: function (value) {
                        if (isString(value)) {
                            primaryImageAttribute = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.PrimaryImageAttribute is a string value.")
                        }
                    },
                    enumerable: true
                },
                "PrimaryNameAttribute": {
                    get: function () { return primaryNameAttribute; },
                    set: function (value) {
                        if (isString(value)) {
                            primaryNameAttribute = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.PrimaryNameAttribute is a string value.")
                        }
                    },
                    enumerable: true
                },
                "Privileges": {
                    get: function () { return privileges; },
                    set: function (value) {
                        if (isArrayOfSecurityPrivilegeMetadata(value)) {
                            privileges = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.Privileges is an array of Sdk.WebAPIModelSample.SecurityPrivilegeMetadata values.")
                        }
                    },
                    enumerable: true
                },
                "RecurrenceBaseEntityLogicalName": {
                    get: function () { return recurrenceBaseEntityLogicalName; },
                    set: function (value) {
                        if (isString(value)) {
                            recurrenceBaseEntityLogicalName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.RecurrenceBaseEntityLogicalName is a string value.")
                        }
                    },
                    enumerable: true
                },
                "ReportViewName": {
                    get: function () { return reportViewName; },
                    set: function (value) {
                        if (isString(value)) {
                            reportViewName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.ReportViewName is a string value.")
                        }
                    },
                    enumerable: true
                },
                "SchemaName": {
                    get: function () { return schemaName; },
                    set: function (value) {
                        if (isString(value)) {
                            schemaName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.SchemaName is a string value.")
                        }
                    },
                    enumerable: true
                },
                "SyncToExternalSearchIndex": {
                    get: function () { return syncToExternalSearchIndex; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            syncToExternalSearchIndex = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.EntityMetadata.SyncToExternalSearchIndex is a boolean value.")
                        }
                    },
                    enumerable: true
                }

            });
    }
    this.AttributeMetadata = function () {
        //TODO: generate these descriptions from resource files
        /// <summary>Contains all the metadata for an entity attribute.</summary>
        /// <field name='AttributeOf' type='String'>The name of the attribute that this attribute extends.</field>

        Sdk.WebAPIModelSample.MetadataBase.call(this);
        var attributeOf;
        var attributeType;
        var attributeTypeName;
        var canBeSecuredForCreate;
        var canBeSecuredForRead;
        var canBeSecuredForUpdate;
        var canModifyAdditionalSettings;
        var columnNumber;
        var deprecatedVersion;
        var description;
        var displayName;
        var entityLogicalName;
        var inheritsFrom;
        var introducedVersion;
        var isAuditEnabled;
        var isCustomAttribute;
        var isCustomizable;
        var isFilterable;
        var isGlobalFilterEnabled;
        var isLogical;
        var isManaged;
        var isPrimaryId;
        var isPrimaryName;
        var isRenameable;
        var isRetrievable;
        var isSearchable;
        var isSecured;
        var isSortableEnabled;
        var isValidForAdvancedFind;
        var isValidForCreate;
        var isValidForRead;
        var isValidForUpdate;
        var linkedAttributeId;
        var logicalName;
        var requiredLevel;
        var schemaName;
        var sourceType;

        Object.defineProperties(this,
    {
        "AttributeOf": {
            get: function () { return attributeOf; },
            set: function (value) {
                if (isString(value)) {
                    attributeOf = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.AttributeOf is a string value.")
                }
            },
            enumerable: true
        },
        "AttributeType": {
            get: function () { return attributeType; },
            set: function (value) {
                if (isAttributeTypeCode(value)) {
                    attributeType = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.AttributeType is an Sdk.WebAPIModelSample.AttributeTypeCode value.")
                }
            },
            enumerable: true
        },
        "AttributeTypeName": {
            get: function () { return attributeTypeName; },
            set: function (value) {
                if (isAttributeTypeDisplayName(value)) {
                    attributeTypeName = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.AttributeTypeName is Sdk.WebAPIModelSample.AttributeTypeDisplayName value.")
                }
            },
            enumerable: true
        },
        "CanBeSecuredForCreate": {
            get: function () {
                return canBeSecuredForCreate;
            },
            set: function (value) {
                if (isBoolean(value)) {
                    canBeSecuredForCreate = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.CanBeSecuredForCreate is a boolean value.")
                }
            },
            enumerable: true
        },
        "CanBeSecuredForRead": {
            get: function () { return canBeSecuredForRead; },
            set: function (value) {
                if (isBoolean(value)) {
                    canBeSecuredForRead = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.CanBeSecuredForRead is a boolean value.")
                }
            },
            enumerable: true
        },
        "CanBeSecuredForUpdate": {
            get: function () { return canBeSecuredForUpdate; },
            set: function (value) {
                if (isBoolean(value)) {
                    canBeSecuredForUpdate = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.CanBeSecuredForUpdate is a boolean value.")
                }
            },
            enumerable: true
        },
        "CanModifyAdditionalSettings": {
            get: function () { return canModifyAdditionalSettings; },
            set: function (value) {
                if (isBooleanManagedProperty(value)) {
                    value.ManagedPropertyLogicalName = "canmodifyadditionalsettings";
                    canModifyAdditionalSettings = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.CanModifyAdditionalSettings is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                }
            },
            enumerable: true
        },
        "ColumnNumber": {
            get: function () { return columnNumber; },
            set: function (value) {
                if (isNumber(value)) {
                    columnNumber = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.ColumnNumber is a number value.")
                }
            },
            enumerable: true
        },
        "DeprecatedVersion": {
            get: function () { return deprecatedVersion; },
            set: function (value) {
                if (isString(value)) {
                    deprecatedVersion = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.DeprecatedVersion is a string value.")
                }
            },
            enumerable: true
        },
        "Description": {
            get: function () { return description; },
            set: function (value) {
                if (isLabel(value)) {
                    description = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.Description is a Sdk.WebAPIModelSample.Label value.")
                }
            },
            enumerable: true
        },
        "DisplayName": {
            get: function () { return displayName; },
            set: function (value) {
                if (isLabel(value)) {
                    displayName = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.DisplayName is a Sdk.WebAPIModelSample.Label  value.")
                }
            },
            enumerable: true
        },
        "EntityLogicalName": {
            get: function () { return entityLogicalName; },
            set: function (value) {
                if (isString(value)) {
                    entityLogicalName = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.EntityLogicalName is a string value.")
                }
            },
            enumerable: true
        },
        "InheritsFrom": {
            get: function () { return inheritsFrom; },
            set: function (value) {
                if (isString(value)) {
                    inheritsFrom = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.InheritsFrom is a string value.")
                }
            },
            enumerable: true
        },
        "IntroducedVersion": {
            get: function () { return introducedVersion; },
            set: function (value) {
                if (isString(value)) {
                    introducedVersion = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.IntroducedVersion is a string value.")
                }
            },
            enumerable: true
        },
        "IsAuditEnabled": {
            get: function () { return isAuditEnabled; },
            set: function (value) {
                if (isBooleanManagedProperty(value)) {
                    value.ManagedPropertyLogicalName = "canmodifyauditsettings";
                    isAuditEnabled = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.IsAuditEnabled is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                }
            },
            enumerable: true
        },
        "IsCustomAttribute": {
            get: function () { return isCustomAttribute; },
            set: function (value) {
                if (isBoolean(value)) {
                    isCustomAttribute = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.IsCustomAttribute is a boolean value.")
                }
            },
            enumerable: true
        },
        "IsCustomizable": {
            get: function () { return isCustomizable; },
            set: function (value) {
                if (isBooleanManagedProperty(value)) {
                    value.ManagedPropertyLogicalName = "iscustomizable";
                    isCustomizable = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.IsCustomizable is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                }
            },
            enumerable: true
        },
        "IsFilterable": {
            get: function () { return isFilterable; },
            set: function (value) {
                if (isBoolean(value)) {
                    isFilterable = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.IsFilterable is a boolean value.")
                }
            },
            enumerable: true
        },
        "IsGlobalFilterEnabled": {
            get: function () { return isGlobalFilterEnabled; },
            set: function (value) {
                if (isBooleanManagedProperty(value)) {
                    value.ManagedPropertyLogicalName = "canmodifyglobalfiltersettings"
                    isGlobalFilterEnabled = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.IsGlobalFilterEnabled is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                }
            },
            enumerable: true
        },
        "IsLogical": {
            get: function () { return isLogical; },
            set: function (value) {
                if (isBoolean(value)) {
                    isLogical = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.IsLogical is a boolean value.")
                }
            },
            enumerable: true
        },
        "IsManaged": {
            get: function () { return isManaged; },
            set: function (value) {
                if (isBoolean(value)) {
                    isManaged = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.IsManaged is a boolean value.")
                }
            },
            enumerable: true
        },
        "IsPrimaryId": {
            get: function () { return isPrimaryId; },
            set: function (value) {
                if (isBoolean(value)) {
                    isPrimaryId = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.IsPrimaryId is a boolean value.")
                }
            },
            enumerable: true
        },
        "IsPrimaryName": {
            get: function () { return isPrimaryName; },
            set: function (value) {
                if (isBoolean(value)) {
                    isPrimaryName = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.IsPrimaryName is a boolean value.")
                }
            },
            enumerable: true
        },
        "IsRenameable": {
            get: function () { return isRenameable; },
            set: function (value) {
                if (isBooleanManagedProperty(value)) {
                    value.ManagedPropertyLogicalName = "isrenameable"
                    isRenameable = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.IsRenameable is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                }
            },
            enumerable: true
        },
        "IsRetrievable": {
            get: function () { return isRetrievable; },
            set: function (value) {
                if (isBoolean(value)) {
                    isRetrievable = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.IsRetrievable is a boolean value.")
                }
            },
            enumerable: true
        },
        "IsSearchable": {
            get: function () { return isSearchable; },
            set: function (value) {
                if (isBoolean(value)) {
                    isSearchable = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.IsSearchable is a boolean value.")
                }
            },
            enumerable: true
        },
        "IsSortableEnabled": {
            get: function () { return isSortableEnabled; },
            set: function (value) {
                if (isBooleanManagedProperty(value)) {
                    value.ManagedPropertyLogicalName = "canmodifyissortablesettings"
                    isSortableEnabled = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.IsSortableEnabled is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                }
            },
            enumerable: true
        },
        "IsValidForAdvancedFind": {
            get: function () { return isValidForAdvancedFind; },
            set: function (value) {
                if (isBooleanManagedProperty(value)) {
                    value.ManagedPropertyLogicalName = "canmodifysearchsettings"
                    isValidForAdvancedFind = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.IsValidForAdvancedFind is a Sdk.WebAPIModelSample.BooleanManagedProperty value.")
                }
            },
            enumerable: true
        },
        "IsValidForCreate": {
            get: function () { return isValidForCreate; },
            set: function (value) {
                if (isBoolean(value)) {
                    isValidForCreate = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.IsValidForCreate is a boolean value.")
                }
            },
            enumerable: true
        },
        "IsValidForRead": {
            get: function () { return isValidForRead; },
            set: function (value) {
                if (isBoolean(value)) {
                    isValidForRead = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.IsValidForRead is a boolean value.")
                }
            },
            enumerable: true
        },
        "IsValidForUpdate": {
            get: function () { return isValidForUpdate; },
            set: function (value) {
                if (isBoolean(value)) {
                    isValidForUpdate = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.IsValidForUpdate is a boolean value.")
                }
            },
            enumerable: true
        },
        "LinkedAttributeId": {
            get: function () { return linkedAttributeId; },
            set: function (value) {
                if (isString(value)) {
                    linkedAttributeId = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.LinkedAttributeId is a string value.")
                }
            },
            enumerable: true
        },
        "LogicalName": {
            get: function () { return logicalName; },
            set: function (value) {
                if (isString(value)) {
                    logicalName = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.LogicalName is a boolean value.")
                }
            },
            enumerable: true
        },
        "RequiredLevel": {
            get: function () { return requiredLevel; },
            set: function (value) {
                if (isAttributeRequiredLevelManagedProperty(value)) {
                    value.ManagedPropertyLogicalName = "canmodifyrequirementlevelsettings";
                    requiredLevel = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.RequiredLevel is a Sdk.WebAPIModelSample.AttributeRequiredLevelManagedProperty value.")
                }
            },
            enumerable: true
        },
        "SchemaName": {
            get: function () { return schemaName; },
            set: function (value) {
                if (isString(value)) {
                    schemaName = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.SchemaName is a string value.")
                }
            },
            enumerable: true
        },
        "SourceType": {
            get: function () { return sourceType; },
            set: function (value) {
                if (isNumber(value)) {
                    sourceType = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.AttributeMetadata.SourceType is a number value.")
                }
            },
            enumerable: true
        }
    });

    }
    //Not doing ManagedPropertyMetadata right now: Internal use Only
    this.BigIntAttributeMetadata = function () {
        Sdk.WebAPIModelSample.AttributeMetadata.call(this);
        this.AttributeType = Sdk.WebAPIModelSample.AttributeTypeCode.BigInt;
        this.AttributeTypeName = Sdk.WebAPIModelSample.AttributeTypeDisplayName.BigIntType;
        var maxValue;
        var minValue;
        Object.defineProperties(this,
    {
        "@odata.type": {
            get: function () { return "Microsoft.Dynamics.CRM.BigIntAttributeMetadata"; },
            enumerable: true
        },
        "MaxValue": {
            get: function () { return maxValue; },
            set: function (value) {
                if (isNumber(value)) {
                    maxValue = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.BigIntAttributeMetadata.MaxValue is a number value.")
                }
            },
            enumerable: true
        },
        "MinValue": {
            get: function () { return minValue; },
            set: function (value) {
                if (isNumber(value)) {
                    minValue = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.BigIntAttributeMetadata.MinValue is a number  value.")
                }
            },
            enumerable: true
        }
    });

    }
    this.BooleanAttributeMetadata = function () {
        Sdk.WebAPIModelSample.AttributeMetadata.call(this);
        this.AttributeType = Sdk.WebAPIModelSample.AttributeTypeCode.Boolean;
        this.AttributeTypeName = Sdk.WebAPIModelSample.AttributeTypeDisplayName.BooleanType;
        var defaultvalue;
        var formulaDefinition;
        var sourceTypeMask;
        var localOptionSet;
        var optionset;

        Object.defineProperties(this,
    {
        "@odata.type": {
            get: function () { return "Microsoft.Dynamics.CRM.BooleanAttributeMetadata"; },
            enumerable: true
        },
        "DefaultValue": {
            get: function () { return defaultvalue; },
            set: function (value) {
                if (isBoolean(value)) {
                    defaultvalue = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.BooleanAttributeMetadata.DefaultValue is a boolean  value.")
                }
            },
            enumerable: true
        },
        "FormulaDefinition": {
            get: function () { return formulaDefinition; },
            set: function (value) {
                if (isString(value)) {
                    formulaDefinition = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.BooleanAttributeMetadata.FormulaDefinition is a string  value.")
                }
            },
            enumerable: true
        },
        "SourceTypeMask": {
            get: function () { return sourceTypeMask; },
            set: function (value) {
                if (isNumber(value)) {
                    sourceTypeMask = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.BooleanAttributeMetadata.SourceTypeMask is a number  value.")
                }
            },
            enumerable: true
        },
        "LocalOptionSet": {
            get: function () { return localOptionSet; },
            set: function (value) {
                if (isBooleanOptionSetMetadata(value)) {
                    localOptionSet = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.BooleanAttributeMetadata.LocalOptionSet is a Sdk.WebAPIModelSample.BooleanOptionSetMetadata value.")
                }
            },
            enumerable: true
        },
        "OptionSet": {
            get: function () { return optionset; },
            set: function (value) {
                if (isBooleanOptionSetMetadata(value)) {
                    optionset = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.BooleanAttributeMetadata.OptionSet is a Sdk.WebAPIModelSample.BooleanOptionSetMetadata value.")
                }
            },
            enumerable: true
        }
    });
    }
    this.DateTimeAttributeMetadata = function () {
        Sdk.WebAPIModelSample.AttributeMetadata.call(this);
        this.AttributeType = Sdk.WebAPIModelSample.AttributeTypeCode.DateTime;
        this.AttributeTypeName = Sdk.WebAPIModelSample.AttributeTypeDisplayName.DateTimeType;

        var minsupportedValue;
        var maxSupportedValue;
        var format;
        var imeMode;
        var sourceTypeMask;
        var formulaDefinition;
        var dateTimeBehavior;
        var canChangeDateTimeBehavior;

        Object.defineProperties(this,
    {
        "@odata.type": {
            get: function () { return "Microsoft.Dynamics.CRM.DateTimeAttributeMetadata"; },
            enumerable: true
        },
        "MinSupportedValue": {
            get: function () { return minsupportedValue; },
            set: function (value) {
                if (isDate(value)) {
                    minsupportedValue = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.DateTimeAttributeMetadata.MinSupportedValue is a Date  value.");
                }
            },
            enumerable: true
        },
        "MaxSupportedValue": {
            get: function () {
                return maxSupportedValue;
            },
            set: function (value) {
                if (isDate(value)) {
                    maxSupportedValue = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.DateTimeAttributeMetadata.MaxSupportedValue is a Date  value.");
                }
            },
            enumerable: true
        },
        "Format": {
            get: function () { return format; },
            set: function (value) {
                if (isDateTimeFormat(value)) {
                    format = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.DateTimeAttributeMetadata.Format is a Sdk.WebAPIModelSample.DateTimeFormat value.");
                }
            },
            enumerable: true
        },
        "ImeMode": {
            get: function () { return imeMode; },
            set: function (value) {
                if (isImeMode(value)) {
                    imeMode = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.DateTimeAttributeMetadata.ImeMode is a Sdk.WebAPIModelSample.ImeMode  value.");
                }
            },
            enumerable: true
        },
        "SourceTypeMask": {
            get: function () { return sourceTypeMask; },
            set: function (value) {
                if (isNumber(value)) {
                    sourceTypeMask = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.DateTimeAttributeMetadata.SourceTypeMask is a number  value.");
                }
            },
            enumerable: true
        },
        "FormulaDefinition": {
            get: function () { return formulaDefinition; },
            set: function (value) {
                if (isString(value)) {
                    formulaDefinition = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.DateTimeAttributeMetadata.FormulaDefinition is a string  value.");
                }
            },
            enumerable: true
        },
        "DateTimeBehavior": {
            get: function () { return dateTimeBehavior; },
            set: function (value) {
                if (isDateTimeBehavior(value)) {
                    dateTimeBehavior = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.DateTimeAttributeMetadata.DateTimeBehavior is a Sdk.WebAPIModelSample.DateTimeBehavior value.");
                }
            },
            enumerable: true
        },
        "CanChangeDateTimeBehavior": {
            get: function () { return canChangeDateTimeBehavior; },
            set: function (value) {
                if (isBooleanManagedProperty(value)) {
                    //TODO: add name
                    canChangeDateTimeBehavior = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.DateTimeAttributeMetadata.CanChangeDateTimeBehavior is a Sdk.WebAPIModelSample.BooleanManagedProperty  value.");
                }
            },
            enumerable: true
        }
    });
    }
    this.DecimalAttributeMetadata = function () {
        Sdk.WebAPIModelSample.AttributeMetadata.call(this);
        this.AttributeType = Sdk.WebAPIModelSample.AttributeTypeCode.Decimal;
        this.AttributeTypeName = Sdk.WebAPIModelSample.AttributeTypeDisplayName.DecimalType;
        var maxValue;
        var minValue;
        var precision;
        var imeMode;
        var formulaDefinition;
        var sourceTypeMask;

        Object.defineProperties(this,
    {
        "@odata.type": {
            get: function () { return "Microsoft.Dynamics.CRM.DecimalAttributeMetadata"; },
            enumerable: true
        },
        "MaxValue": {
            get: function () { return maxValue; },
            set: function (value) {
                if (isNumber(value)) {
                    maxValue = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.DecimalAttributeMetadata.MaxValue is a MaxValue  value.")
                }
            },
            enumerable: true
        },
        "MinValue": {
            get: function () { return minValue; },
            set: function (value) {
                if (isNumber(value)) {
                    minValue = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.DecimalAttributeMetadata.MinValue is a number  value.")
                }
            },
            enumerable: true
        },
        "Precision": {
            get: function () { return precision; },
            set: function (value) {
                if (isNumber(value)) {
                    precision = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.DecimalAttributeMetadata.MinValue is a number  value.")
                }
            },
            enumerable: true
        },
        "ImeMode": {
            get: function () { return imeMode; },
            set: function (value) {
                if (isImeMode(value)) {
                    imeMode = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.DecimalAttributeMetadata.ImeMode is a Sdk.WebAPIModelSample.ImeMode  value.");
                }
            },
            enumerable: true
        },
        "SourceTypeMask": {
            get: function () { return sourceTypeMask; },
            set: function (value) {
                if (isNumber(value)) {
                    sourceTypeMask = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.DecimalAttributeMetadata.SourceTypeMask is a number  value.");
                }
            },
            enumerable: true
        },
        "FormulaDefinition": {
            get: function () { return formulaDefinition; },
            set: function (value) {
                if (isString(value)) {
                    formulaDefinition = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.DecimalAttributeMetadata.FormulaDefinition is a string  value.");
                }
            },
            enumerable: true
        }
    });
    }
    this.DoubleAttributeMetadata = function () {
        Sdk.WebAPIModelSample.AttributeMetadata.call(this);
        this.AttributeType = Sdk.WebAPIModelSample.AttributeTypeCode.Double;
        this.AttributeTypeName = Sdk.WebAPIModelSample.AttributeTypeDisplayName.DoubleType;
        var maxValue;
        var minValue;
        var precision;
        var imeMode;
        Object.defineProperties(this,
    {
        "@odata.type": {
            get: function () { return "Microsoft.Dynamics.CRM.DoubleAttributeMetadata"; },
            enumerable: true
        },
        "MaxValue": {
            get: function () { return maxValue; },
            set: function (value) {
                if (isNumber(value)) {
                    maxValue = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.DoubleAttributeMetadata.MaxValue is a number  value.")
                }
            },
            enumerable: true
        },
        "MinValue": {
            get: function () { return minValue; },
            set: function (value) {
                if (isNumber(value)) {
                    minValue = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.DoubleAttributeMetadata.MinValue is a number  value.")
                }
            },
            enumerable: true
        },
        "Precision": {
            get: function () { return precision; },
            set: function (value) {
                if (isNumber(value)) {
                    precision = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.DoubleAttributeMetadata.Precision is a number  value.")
                }
            },
            enumerable: true
        },
        "ImeMode": {
            get: function () { return imeMode; },
            set: function (value) {
                if (isImeMode(value)) {
                    imeMode = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.DoubleAttributeMetadata.ImeMode is a Sdk.WebAPIModelSample.ImeMode  value.");
                }
            },
            enumerable: true
        }
    });
    }
    this.ImageAttributeMetadata = function () {
        Sdk.WebAPIModelSample.AttributeMetadata.call(this);
        this.AttributeType = Sdk.WebAPIModelSample.AttributeTypeCode.Virtual;
        this.AttributeTypeName = Sdk.WebAPIModelSample.AttributeTypeDisplayName.ImageType;
        var isPrimaryImage;
        var maxHeight;
        var maxWidth;
        Object.defineProperties(this,
    {
        "@odata.type": {
            get: function () { return "Microsoft.Dynamics.CRM.ImageAttributeMetadata"; },
            enumerable: true
        },
        "IsPrimaryImage": {
            get: function () { return isPrimaryImage; },
            set: function (value) {
                if (isBoolean(value)) {
                    isPrimaryImage = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.ImageAttributeMetadata.IsPrimaryImage is a boolean  value.")
                }
            },
            enumerable: true
        },
        "MaxHeight": {
            get: function () { return maxHeight; },
            set: function (value) {
                if (isNumber(value)) {
                    maxHeight = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.ImageAttributeMetadata.MaxHeight is a number  value.")
                }
            },
            enumerable: true
        },
        "MaxWidth": {
            get: function () { return maxWidth; },
            set: function (value) {
                if (isNumber(value)) {
                    maxWidth = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.ImageAttributeMetadata.MaxWidth is a number  value.")
                }
            },
            enumerable: true
        }
    });
    }
    this.IntegerAttributeMetadata = function () {
        Sdk.WebAPIModelSample.AttributeMetadata.call(this);
        this.AttributeType = Sdk.WebAPIModelSample.AttributeTypeCode.Integer;
        this.AttributeTypeName = Sdk.WebAPIModelSample.AttributeTypeDisplayName.IntegerType;
        var format;
        var maxValue;
        var minValue;
        var formulaDefinition;
        var sourceTypeMask;

        Object.defineProperties(this,
    {
        "@odata.type": {
            get: function () { return "Microsoft.Dynamics.CRM.IntegerAttributeMetadata"; },
            enumerable: true
        },
        "Format": {
            get: function () { return format; },
            set: function (value) {
                if (isIntegerFormat(value)) {
                    format = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.IntegerAttributeMetadata.Format is a Sdk.WebAPIModelSample.IntegerFormat value.")
                }
            },
            enumerable: true
        },
        "MaxValue": {
            get: function () { return maxValue; },
            set: function (value) {
                if (isNumber(value)) {
                    maxValue = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.IntegerAttributeMetadata.MaxValue is a MaxValue  value.")
                }
            },
            enumerable: true
        },
        "MinValue": {
            get: function () { return minValue; },
            set: function (value) {
                if (isNumber(value)) {
                    minValue = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.IntegerAttributeMetadata.MinValue is a number  value.")
                }
            },
            enumerable: true
        },
        "SourceTypeMask": {
            get: function () { return sourceTypeMask; },
            set: function (value) {
                if (isNumber(value)) {
                    sourceTypeMask = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.IntegerAttributeMetadata.SourceTypeMask is a number  value.");
                }
            },
            enumerable: true
        },
        "FormulaDefinition": {
            get: function () { return formulaDefinition; },
            set: function (value) {
                if (isString(value)) {
                    formulaDefinition = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.IntegerAttributeMetadata.FormulaDefinition is a string  value.");
                }
            },
            enumerable: true
        }
    });
    }
    this.LookupAttributeMetadata = function () {
        Sdk.WebAPIModelSample.AttributeMetadata.call(this);
        this.AttributeType = Sdk.WebAPIModelSample.AttributeTypeCode.Lookup;
        this.AttributeTypeName = Sdk.WebAPIModelSample.AttributeTypeDisplayName.LookupType;
        var targets;
        Object.defineProperties(this,
    {
        "@odata.type": {
            get: function () { return "Microsoft.Dynamics.CRM.LookupAttributeMetadata"; },
            enumerable: true
        },
        "Targets": {
            get: function () { return targets; },
            set: function (value) {
                if (isArrayOfString(value)) {
                    targets = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.LookupAttributeMetadata.Targets is an array of string values.")
                }
            },
            enumerable: true
        },
    });
    }
    //NOt doing ManagedPropertyAttributeMetadata
    this.MemoAttributeMetadata = function () {
        Sdk.WebAPIModelSample.AttributeMetadata.call(this);
        this.AttributeType = Sdk.WebAPIModelSample.AttributeTypeCode.Memo;
        this.AttributeTypeName = Sdk.WebAPIModelSample.AttributeTypeDisplayName.MemoType;
        var format, imeMode, maxLength, isLocalizable;

        Object.defineProperties(this,
    {
        "@odata.type": {
            get: function () { return "Microsoft.Dynamics.CRM.MemoAttributeMetadata"; },
            enumerable: true
        },
        "Format": {
            get: function () { return format; },
            set: function (value) {
                if (isStringFormat(value)) {
                    format = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.MemoAttributeMetadata.Format is a Sdk.WebAPIModelSample.StringFormat value.")
                }
            },
            enumerable: true
        },
        "ImeMode": {
            get: function () { return imeMode; },
            set: function (value) {
                if (isImeMode(value)) {
                    imeMode = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.MemoAttributeMetadata.ImeMode is a Sdk.WebAPIModelSample.ImeMode  value.");
                }
            },
            enumerable: true
        },
        "MaxLength": {
            get: function () { return maxLength; },
            set: function (value) {
                if (isNumber(value)) {
                    maxLength = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.MemoAttributeMetadata.MaxLength is a number value.");
                }
            },
            enumerable: true
        },
        "IsLocalizable": {
            get: function () { return isLocalizable; },
            set: function (value) {
                if (isBoolean(value)) {
                    isLocalizable = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.MemoAttributeMetadata.IsLocalizable is a boolean value.");
                }
            },
            enumerable: true
        }
    });
    }
    this.MoneyAttributeMetadata = function () {
        Sdk.WebAPIModelSample.AttributeMetadata.call(this);
        this.AttributeType = Sdk.WebAPIModelSample.AttributeTypeCode.Money;
        this.AttributeTypeName = Sdk.WebAPIModelSample.AttributeTypeDisplayName.MoneyType;
        var imeMode, maxValue, minValue, precision, precisionSource, calculationOf, formulaDefinition, sourceTypeMask, isBaseCurrency;
        Object.defineProperties(this,
    {
        "@odata.type": {
            get: function () { return "Microsoft.Dynamics.CRM.MoneyAttributeMetadata"; },
            enumerable: true
        },
        "ImeMode": {
            get: function () { return imeMode; },
            set: function (value) {
                if (isImeMode(value)) {
                    imeMode = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.MoneyAttributeMetadata.ImeMode is a Sdk.WebAPIModelSample.ImeMode value.");
                }
            },
            enumerable: true
        },
        "MaxValue": {
            get: function () { return maxValue; },
            set: function (value) {
                if (isNumber(value)) {
                    maxValue = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.MoneyAttributeMetadata.MaxValue is a number value.");
                }
            },
            enumerable: true
        },
        "MinValue": {
            get: function () { return minValue; },
            set: function (value) {
                if (isNumber(value)) {
                    minValue = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.MoneyAttributeMetadata.MinValue is a number value.");
                }
            },
            enumerable: true
        },
        "Precision": {
            get: function () { return precision; },
            set: function (value) {
                if (isNumber(value)) {
                    precision = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.MoneyAttributeMetadata.Precision is a number  value.");
                }
            },
            enumerable: true
        },
        "PrecisionSource": {
            get: function () { return precisionSource; },
            set: function (value) {
                if (isNumber(value)) {
                    precisionSource = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.MoneyAttributeMetadata.PrecisionSource is a number  value.");
                }
            },
            enumerable: true
        },
        "CalculationOf": {
            get: function () { return calculationOf; },
            set: function (value) {
                if (isString(value)) {
                    calculationOf = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.MoneyAttributeMetadata.CalculationOf is a string  value.");
                }
            },
            enumerable: true
        },
        "FormulaDefinition": {
            get: function () { return formulaDefinition; },
            set: function (value) {
                if (isString(value)) {
                    formulaDefinition = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.MoneyAttributeMetadata.FormulaDefinition is a string  value.");
                }
            },
            enumerable: true
        },
        "SourceTypeMask": {
            get: function () { return sourceTypeMask; },
            set: function (value) {
                if (isNumber(value)) {
                    sourceTypeMask = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.MoneyAttributeMetadata.SourceTypeMask is a number  value.");
                }
            },
            enumerable: true
        },
        "IsBaseCurrency": {
            get: function () { return isBaseCurrency; },
            set: function (value) {
                if (isBoolean(value)) {
                    isBaseCurrency = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.MoneyAttributeMetadata.IsBaseCurrency is a boolean  value.");
                }
            },
            enumerable: true
        }
    });
    }
    this.StringAttributeMetadata = function () {
        Sdk.WebAPIModelSample.AttributeMetadata.call(this);
        var format, formatName, formulaDefinition, imeMode, maxLength, sourceTypeMask, yomiOf;
        this.AttributeType = Sdk.WebAPIModelSample.AttributeTypeCode.String;
        this.AttributeTypeName = Sdk.WebAPIModelSample.AttributeTypeDisplayName.StringType;
        Object.defineProperties(this,
            {
                "@odata.type": {
                    get: function () { return "Microsoft.Dynamics.CRM.StringAttributeMetadata"; },
                    enumerable: true
                },
                "Format": {
                    get: function () { return format; },
                    set: function (value) {
                        if (isStringFormat(value)) {
                            format = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StringAttributeMetadata.Format is a Sdk.WebAPIModelSample.StringFormat  value.")
                        }
                    },
                    enumerable: true
                },
                "FormatName": {
                    get: function () { return formatName; },
                    set: function (value) {
                        if (isStringFormatName(value)) {
                            formatName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StringAttributeMetadata.FormatName is a Sdk.WebAPIModelSample.StringFormatName value.")
                        }
                    },
                    enumerable: true
                },
                "FormulaDefinition": {
                    get: function () { return formulaDefinition; },
                    set: function (value) {
                        if (isString(value)) {
                            formulaDefinition = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StringAttributeMetadata.FormulaDefinition is a string value.")
                        }
                    },
                    enumerable: true
                },
                "ImeMode": {
                    get: function () { return imeMode; },
                    set: function (value) {
                        if (isImeMode(value)) {
                            imeMode = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StringAttributeMetadata.ImeMode is a Sdk.WebAPIModelSample.ImeMode value.")
                        }
                    },
                    enumerable: true
                },
                "MaxLength": {
                    get: function () { return maxLength; },
                    set: function (value) {
                        if (isNumber(value)) {
                            maxLength = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StringAttributeMetadata.MaxLength is a number value.")
                        }
                    },
                    enumerable: true
                },
                "SourceTypeMask": {
                    get: function () { return sourceTypeMask; },
                    set: function (value) {
                        if (isNumber(value)) {
                            sourceTypeMask = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StringAttributeMetadata.SourceTypeMask is a number value.")
                        }
                    },
                    enumerable: true
                },
                "YomiOf": {
                    get: function () { return yomiOf; },
                    set: function (value) {
                        if (isString(value)) {
                            yomiOf = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StringAttributeMetadata.YomiOf is a string value.")
                        }
                    },
                    enumerable: true
                }
            });

        
    }
    this.UniqueIdentifierAttributeMetadata = function () {
        Sdk.WebAPIModelSample.AttributeMetadata.call(this);
        this.AttributeType = Sdk.WebAPIModelSample.AttributeTypeCode.Uniqueidentifier;
        this.AttributeTypeName = Sdk.WebAPIModelSample.AttributeTypeDisplayName.UniqueidentifierType;

        Object.defineProperties(this,
    {
        "@odata.type": {
            get: function () { return "Microsoft.Dynamics.CRM.UniqueIdentifierAttributeMetadata"; },
            enumerable: true
        }
    });
    }
    this.EnumAttributeMetadata = function () {
        Sdk.WebAPIModelSample.AttributeMetadata.call(this);
        var defaultFormValue, localOptionSet, optionset;

        Object.defineProperties(this,
    {
        "@odata.type": {
            get: function () { return "Microsoft.Dynamics.CRM.EnumAttributeMetadata"; },
            enumerable: true
        },
        "DefaultFormValue": {
            get: function () { return defaultFormValue; },
            set: function (value) {
                if (isNumber(value)) {
                    defaultFormValue = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.EnumAttributeMetadata.DefaultFormValue is a number value.")
                }
            },
            enumerable: true
        },
        "LocalOptionSet": {
            get: function () {
                return localOptionSet;
            },
            set: function (value) {
                if (isOptionSetMetadata(value)) {
                    localOptionSet = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.EnumAttributeMetadata.LocalOptionSet is a Sdk.WebAPIModelSample.OptionSetMetadata  value.")
                }
            },
            enumerable: true
        },
        "OptionSet": {
            get: function () {
                return optionset;
            },
            set: function (value) {
                if (isOptionSetMetadata(value)) {
                    optionset = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.EnumAttributeMetadata.OptionSet is a Sdk.WebAPIModelSample.OptionSetMetadata  value.")
                }
            },
            enumerable: true
        }
    });
    }
    this.EntityNameAttributeMetadata = function () {
        Sdk.WebAPIModelSample.EnumAttributeMetadata.call(this);
        this.AttributeType = Sdk.WebAPIModelSample.AttributeTypeCode.EntityName;
        this.AttributeTypeName = Sdk.WebAPIModelSample.AttributeTypeDisplayName.EntityNameType;

        Object.defineProperties(this,
    {
        "@odata.type": {
            get: function () { return "Microsoft.Dynamics.CRM.EntityNameAttributeMetadata"; },
            enumerable: true
        }
    });
    }
    this.PicklistAttributeMetadata = function () {
        Sdk.WebAPIModelSample.EnumAttributeMetadata.call(this);
        this.AttributeType = Sdk.WebAPIModelSample.AttributeTypeCode.Picklist;
        this.AttributeTypeName = Sdk.WebAPIModelSample.AttributeTypeDisplayName.PicklistType;
        var formulaDefinition, sourceTypeMask;
        Object.defineProperties(this,
    {
        "@odata.type": {
            get: function () { return "Microsoft.Dynamics.CRM.PicklistAttributeMetadata"; },
            enumerable: true
        },
        "FormulaDefinition": {
            get: function () { return formulaDefinition; },
            set: function (value) {
                if (isString(value)) {
                    formulaDefinition = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.PicklistAttributeMetadata.FormulaDefinition is a string value.")
                }
            },
            enumerable: true
        },
        "SourceTypeMask": {
            get: function () { return sourceTypeMask; },
            set: function (value) {
                if (isNumber(value)) {
                    sourceTypeMask = value;
                }
                else {
                    throw new Error("Sdk.WebAPIModelSample.PicklistAttributeMetadata.SourceTypeMask is a number value.")
                }
            },
            enumerable: true
        },
    });
    }
    this.StateAttributeMetadata = function () {
        Sdk.WebAPIModelSample.EnumAttributeMetadata.call(this);
        this.AttributeType = Sdk.WebAPIModelSample.AttributeTypeCode.State;
        this.AttributeTypeName = Sdk.WebAPIModelSample.AttributeTypeDisplayName.StateType;

        Object.defineProperties(this,
    {
        "@odata.type": {
            get: function () { return "Microsoft.Dynamics.CRM.StateAttributeMetadata"; },
            enumerable: true
        }
    });
    }
    this.StatusAttributeMetadata = function () {
        Sdk.WebAPIModelSample.EnumAttributeMetadata.call(this);
        this.AttributeType = Sdk.WebAPIModelSample.AttributeTypeCode.Status;
        this.AttributeTypeName = Sdk.WebAPIModelSample.AttributeTypeDisplayName.StatusType;

        Object.defineProperties(this,
    {
        "@odata.type": {
            get: function () { return "Microsoft.Dynamics.CRM.StatusAttributeMetadata"; },
            enumerable: true
        }
    });
    }

    //Complex types
    this.LocalizedLabel = function (label, languageCode) {
        /// <summary>A Localized Label</summary>
        /// <param name="label" type="String">The text of the label</param>
        /// <param name="languageCode" type="Number">The LCID of the Language</param>    

        var label;
        var languageCode;
        var isManaged;
        var metadataId;
        var hasChanged;

        Object.defineProperties(this, {
            "@odata.type": {
                get: function () { return "Microsoft.Dynamics.CRM.LocalizedLabel"; },
                enumerable: true
            }, "Label": {
                get: function () { return label; },
                set: function (value) {
                    if (isString(value))
                    { label = value; }
                    else {
                        throw new Error("Sdk.WebAPIModelSample.LocalizedLabel.Label is a string value.")
                    }
                },
                enumerable: true
            },
            "LanguageCode": {
                get: function () { return languageCode; },
                set: function (value) {
                    if (isNumber(value))
                    { languageCode = value; }
                    else {
                        throw new Error("Sdk.WebAPIModelSample.LocalizedLabel.LanguageCode is a number value.")
                    }
                },
                enumerable: true
            },
            "IsManaged": {
                get: function () { return isManaged; },
                set: function (value) {
                    if (isBoolean(value))
                    { isManaged = value; }
                    else {
                        throw new Error("Sdk.WebAPIModelSample.LocalizedLabel.IsManaged is a boolean value.")
                    }
                },
                enumerable: true
            },
            "MetadataId": {
                get: function () { return metadataId; },
                set: function (value) {
                    if (isString(value))
                    { metadataId = value; }
                    else {
                        throw new Error("Sdk.WebAPIModelSample.LocalizedLabel.MetadataId is a string value.")
                    }
                },
                enumerable: true
            },
            "HasChanged": {
                get: function () { return hasChanged; },
                set: function (value) {
                    if (isBoolean(value))
                    { hasChanged = value; }
                    else {
                        throw new Error("Sdk.WebAPIModelSample.LocalizedLabel.HasChanged is a boolean value.")
                    }
                },
                enumerable: true
            }
        })
        if (label) {
            this.Label = label;
        }
        if (languageCode) {
            this.LanguageCode = languageCode;
        }
    }
    this.Label = function (localizedLabel) {
        /// <summary>A Label for a metadata object</summary>
        /// <param name="localizedLabel" type="Sdk.WebAPIModelSample.LocalizedLabel">The localized label</param>

        var localizedLabels;
        var userlocalizedLabel;

        Object.defineProperties(this, {
            "@odata.type": {
                get: function () { return "Microsoft.Dynamics.CRM.Label"; },
                enumerable: true
            }, "LocalizedLabels": {
                get: function () { return localizedLabels; },
                set: function (value) {
                    if (isArrayOfLocalizedLabel(value))
                    { localizedLabels = value; }
                    else {
                        throw new Error("Sdk.WebAPIModelSample.Label.UserLocalizedLabel is an array of Sdk.WebAPIModelSample.LocalizedLabel values.");
                    }
                },
                enumerable: true
            },
            "UserLocalizedLabel": {
                get: function () { return userlocalizedLabel; },
                set: function (value) {
                    if (isLocalizedLabel(value))
                    { userlocalizedLabel = value; }
                    else {
                        throw new Error("Sdk.WebAPIModelSample.Label.UserLocalizedLabel is a Sdk.WebAPIModelSample.LocalizedLabel value.");
                    }
                },
                enumerable: true
            }
        });
        if (localizedLabel) {
            this.UserLocalizedLabel = localizedLabel;
            this.LocalizedLabels = [localizedLabel];
        }

    }

    this.BooleanManagedProperty = function (value, canBeChanged) {
        var _value;
        var canBeChangedValue;
        var managedPropertyLogicalName;
        Object.defineProperties(this, {
            "Value": {
                get: function () { return _value; },
                set: function (value) {
                    if (isBoolean(value))
                    { _value = value; }
                    else {
                        throw new Error("Sdk.WebAPIModelSample.BooleanManagedProperty.Value is a boolean value.")
                    }
                },
                enumerable: true
            },
            "CanBeChanged": {
                get: function () { return canBeChangedValue; },
                set: function (value) {
                    if (isBoolean(value))
                    { canBeChangedValue = value; }
                    else {
                        throw new Error("Sdk.WebAPIModelSample.BooleanManagedProperty.CanBeChanged is a boolean value.")
                    }
                },
                enumerable: true
            },
            "ManagedPropertyLogicalName": {
                get: function () { return managedPropertyLogicalName; },
                set: function (value) {
                    if (isString(value))
                    { managedPropertyLogicalName = value; }
                    else {
                        throw new Error("Sdk.WebAPIModelSample.BooleanManagedProperty.ManagedPropertyLogicalName is a string value.")
                    }
                },
                enumerable: true
            }
        });
        if (value) {
            this.Value = value;
        }
        if (canBeChanged) {
            this.CanBeChanged = canBeChanged;
        }

    }
    this.AttributeRequiredLevelManagedProperty = function (value, canBeChanged) {
        var _value;
        var canBeChangedValue = true;
        var managedPropertyLogicalName;
        Object.defineProperties(this, {
            "Value": {
                get: function () { return _value; },
                set: function (value) {
                    if (isAttributeRequiredLevel(value))
                    { _value = value; }
                    else {
                        throw new Error("Sdk.WebAPIModelSample.AttributeRequiredLevelManagedProperty.Value is a Sdk.WebAPIModelSample.AttributeRequiredLevel  value.")
                    }
                },
                enumerable: true
            },
            "CanBeChanged": {
                get: function () { return canBeChangedValue; },
                set: function (value) {
                    if (isBoolean(value))
                    { canBeChangedValue = value; }
                    else {
                        throw new Error("Sdk.WebAPIModelSample.AttributeRequiredLevelManagedProperty.CanBeChanged is a boolean value.")
                    }
                },
                enumerable: true
            },
            "ManagedPropertyLogicalName": {
                get: function () { return managedPropertyLogicalName; },
                set: function (value) {
                    if (isString(value))
                    { managedPropertyLogicalName = value; }
                    else {
                        throw new Error("Sdk.WebAPIModelSample.AttributeRequiredLevelManagedProperty.ManagedPropertyLogicalName is a string value.")
                    }
                },
                enumerable: true
            }
        });
        if (value) {
            this.Value = value;
        }
        if (canBeChanged) {
            this.CanBeChanged = canBeChanged;
        }

    }

    this.SecurityPrivilegeMetadata = function () {
        var canBeBasic;
        var canBeDeep;
        var canBeGlobal;
        var canBeLocal;
        var canBeEntityReference;
        var canBeParentEntityReference;
        var name;
        var privilegeId;
        var privilegeType;
        Object.defineProperties(this, {
            "CanBeBasic": {
                get: function () { return canBeBasic; },
                set: function (value) {
                    if (isBoolean(value))
                    { canBeBasic = value; }
                    else {
                        throw new Error("Sdk.WebAPIModelSample.SecurityPrivilegeMetadata.CanBeBasic is a boolean value.")
                    }
                },
                enumerable: true
            },
            "CanBeDeep": {
                get: function () { return canBeDeep; },
                set: function (value) {
                    if (isBoolean(value))
                    { canBeDeep = value; }
                    else {
                        throw new Error("Sdk.WebAPIModelSample.SecurityPrivilegeMetadata.CanBeDeep is a boolean value.")
                    }
                },
                enumerable: true
            },
            "CanBeGlobal": {
                get: function () { return canBeGlobal; },
                set: function (value) {
                    if (isBoolean(value))
                    { canBeGlobal = value; }
                    else {
                        throw new Error("Sdk.WebAPIModelSample.SecurityPrivilegeMetadata.CanBeGlobal is a boolean value.")
                    }
                },
                enumerable: true
            },
            "CanBeLocal": {
                get: function () { return canBeLocal; },
                set: function (value) {
                    if (isBoolean(value))
                    { canBeLocal = value; }
                    else {
                        throw new Error("Sdk.WebAPIModelSample.SecurityPrivilegeMetadata.CanBeLocal is a boolean value.")
                    }
                },
                enumerable: true
            },
            "CanBeEntityReference": {
                get: function () { return canBeEntityReference; },
                set: function (value) {
                    if (isBoolean(value))
                    { canBeEntityReference = value; }
                    else {
                        throw new Error("Sdk.WebAPIModelSample.SecurityPrivilegeMetadata.CanBeEntityReference is a boolean value.")
                    }
                },
                enumerable: true
            },
            "CanBeParentEntityReference": {
                get: function () { return canBeParentEntityReference; },
                set: function (value) {
                    if (isBoolean(value))
                    { canBeParentEntityReference = value; }
                    else {
                        throw new Error("Sdk.WebAPIModelSample.SecurityPrivilegeMetadata.CanBeParentEntityReference is a boolean value.")
                    }
                },
                enumerable: true
            },
            "Name": {
                get: function () { return name; },
                set: function (value) {
                    if (isString(value))
                    { name = value; }
                    else {
                        throw new Error("Sdk.WebAPIModelSample.SecurityPrivilegeMetadata.Name is a string value.")
                    }
                },
                enumerable: true
            },
            "PrivilegeId": {
                get: function () { return privilegeId; },
                set: function (value) {
                    if (isString(value))
                    { privilegeId = value; }
                    else {
                        throw new Error("Sdk.WebAPIModelSample.SecurityPrivilegeMetadata.PrivilegeId is a string value.")
                    }
                },
                enumerable: true
            },
            "PrivilegeType": {
                get: function () { return privilegeType; },
                set: function (value) {
                    if (isPrivilegeType(value))
                    { privilegeType = value; }
                    else {
                        throw new Error("Sdk.WebAPIModelSample.SecurityPrivilegeMetadata.PrivilegeType is a Sdk.WebAPIModelSample.PrivilegeType value.")
                    }
                },
                enumerable: true
            }
        });

    }
    this.AssociatedMenuConfiguration = function () {
        var behavior;
        var group;
        var label;
        var order;
        Object.defineProperties(this,
            {
                "Behavior": {
                    get: function () { return behavior; },
                    set: function (value) {
                        if (isAssociatedMenuBehavior(value)) {
                            behavior = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.AssociatedMenuConfiguration.Behavior is a Sdk.WebAPIModelSample.AssociatedMenuBehavior value.")
                        }
                    },
                    enumerable: true
                },
                "Group": {
                    get: function () { return group; },
                    set: function (value) {
                        if (isAssociatedMenuGroup(value)) {
                            group = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.AssociatedMenuConfiguration.Group is a Sdk.WebAPIModelSample.AssociatedMenuGroup value.")
                        }
                    },
                    enumerable: true
                },
                "Label": {
                    get: function () { return label; },
                    set: function (value) {
                        if (isLabel(value)) {
                            label = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.AssociatedMenuConfiguration.Label is a Sdk.WebAPIModelSample.Label value.")
                        }
                    },
                    enumerable: true
                },
                "Order": {
                    get: function () { return order; },
                    set: function (value) {
                        if (isNumber(value)) {
                            order = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.AssociatedMenuConfiguration.Order is a number value.")
                        }
                    },
                    enumerable: true
                }
            });
    }
    this.CascadeConfiguration = function () {
        var assign;
        var del;
        var merge;
        var reparent;
        var share;
        var unshare;
        Object.defineProperties(this,
           {
               "Assign": {
                   get: function () { return assign; },
                   set: function (value) {
                       if (isCascadeType(value)) {
                           assign = value;
                       }
                       else {
                           throw new Error("Sdk.WebAPIModelSample.CascadeConfiguration.Assign is a Sdk.WebAPIModelSample.CascadeType value.")
                       }
                   },
                   enumerable: true
               },
               "Delete": {
                   get: function () { return del; },
                   set: function (value) {
                       if (isCascadeType(value)) {
                           del = value;
                       }
                       else {
                           throw new Error("Sdk.WebAPIModelSample.CascadeConfiguration.Delete is a Sdk.WebAPIModelSample.CascadeType value.")
                       }
                   },
                   enumerable: true
               },
               "Merge": {
                   get: function () { return merge; },
                   set: function (value) {
                       if (isCascadeType(value)) {
                           merge = value;
                       }
                       else {
                           throw new Error("Sdk.WebAPIModelSample.CascadeConfiguration.Merge is a Sdk.WebAPIModelSample.CascadeType value.")
                       }
                   },
                   enumerable: true
               },
               "Reparent": {
                   get: function () { return reparent; },
                   set: function (value) {
                       if (isCascadeType(value)) {
                           reparent = value;
                       }
                       else {
                           throw new Error("Sdk.WebAPIModelSample.CascadeConfiguration.Reparent is a Sdk.WebAPIModelSample.CascadeType value.")
                       }
                   },
                   enumerable: true
               },
               "Share": {
                   get: function () { return share; },
                   set: function (value) {
                       if (isCascadeType(value)) {
                           share = value;
                       }
                       else {
                           throw new Error("Sdk.WebAPIModelSample.CascadeConfiguration.Share is a Sdk.WebAPIModelSample.CascadeType value.")
                       }
                   },
                   enumerable: true
               },
               "Unshare": {
                   get: function () { return unshare; },
                   set: function (value) {
                       if (isCascadeType(value)) {
                           unshare = value;
                       }
                       else {
                           throw new Error("Sdk.WebAPIModelSample.CascadeConfiguration.Unshare is a Sdk.WebAPIModelSample.CascadeType value.")
                       }
                   },
                   enumerable: true
               }
           });
    }
    this.EntityReference = function () {
        var id;
        var logicalName;
        var name;
        var rowVersion;
        Object.defineProperties(this,
           {
               "Id": {
                   get: function () { return id; },
                   set: function (value) {
                       if (isString(value)) {
                           id = value;
                       }
                       else {
                           throw new Error("Sdk.WebAPIModelSample.EntityReference.Id is a string value.")
                       }
                   },
                   enumerable: true
               },
               "LogicalName": {
                   get: function () { return logicalName; },
                   set: function (value) {
                       if (isString(value)) {
                           logicalName = value;
                       }
                       else {
                           throw new Error("Sdk.WebAPIModelSample.EntityReference.LogicalName is a string value.")
                       }
                   },
                   enumerable: true
               },
               "Name": {
                   get: function () { return name; },
                   set: function (value) {
                       if (isString(value)) {
                           name = value;
                       }
                       else {
                           throw new Error("Sdk.WebAPIModelSample.EntityReference.Name is a string value.")
                       }
                   },
                   enumerable: true
               },
               "RowVersion": {
                   get: function () { return rowVersion; },
                   set: function (value) {
                       if (isString(value)) {
                           rowVersion = value;
                       }
                       else {
                           throw new Error("Sdk.WebAPIModelSample.EntityReference.RowVersion is a string value.")
                       }
                   },
                   enumerable: true
               }
           });
    }
    this.OptionMetadata = function () {
        var _value;
        var label;
        var description;
        var color;
        var isManaged;
        var metadataid;
        var hasChanged;

        Object.defineProperties(this,
            {
                "Value": {
                    get: function () { return _value; },
                    set: function (value) {
                        if (isNumber(value)) {
                            _value = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.OptionMetadata.Value is a number value.")
                        }
                    },
                    enumerable: true
                },
                "Label": {
                    get: function () { return label; },
                    set: function (value) {
                        if (isLabel(value)) {
                            label = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.OptionMetadata.Label is a Sdk.WebAPIModelSample.Label value.")
                        }
                    },
                    enumerable: true
                },
                "Description": {
                    get: function () { return description; },
                    set: function (value) {
                        if (isLabel(value)) {
                            description = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.OptionMetadata.Description is a Sdk.WebAPIModelSample.Label value.")
                        }
                    },
                    enumerable: true
                },
                "Color": {
                    get: function () { return color; },
                    set: function (value) {
                        if (isString(value)) {
                            color = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.OptionMetadata.Color is a string value.")
                        }
                    },
                    enumerable: true
                },
                "isManaged": {
                    get: function () { return isManaged; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isManaged = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.OptionMetadata.isManaged is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "MetadataId": {
                    get: function () { return metadataid; },
                    set: function (value) {
                        if (isString(value)) {
                            metadataid = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.OptionMetadata.MetadataId is a string value.")
                        }
                    },
                    enumerable: true
                },
                "HasChanged": {
                    get: function () { return hasChanged; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            hasChanged = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.OptionMetadata.HasChanged is a boolean value.")
                        }
                    },
                    enumerable: true
                }
            });
    }
    this.StateOptionMetadata = function () {
        var defaultStatus;
        var invariantName;
        var _value;
        var label;
        var description;
        var color;
        var isManaged;
        var metadataid;
        var hasChanged;

        Object.defineProperties(this,
            {
                "DefaultStatus": {
                    get: function () { return defaultStatus; },
                    set: function (value) {
                        if (isNumber(value)) {
                            defaultStatus = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StateOptionMetadata.DefaultStatus is a number value.")
                        }
                    },
                    enumerable: true
                },
                "InvariantName": {
                    get: function () { return invariantName; },
                    set: function (value) {
                        if (isString(value)) {
                            invariantName = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StateOptionMetadata.InvariantName is a string value.")
                        }
                    },
                    enumerable: true
                },
                "Value": {
                    get: function () { return _value; },
                    set: function (value) {
                        if (isNumber(value)) {
                            _value = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StateOptionMetadata.Value is a number value.")
                        }
                    },
                    enumerable: true
                },
                "Label": {
                    get: function () { return label; },
                    set: function (value) {
                        if (isLabel(value)) {
                            label = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StateOptionMetadata.Label is a Sdk.WebAPIModelSample.Label value.")
                        }
                    },
                    enumerable: true
                },
                "Description": {
                    get: function () { return description; },
                    set: function (value) {
                        if (isLabel(value)) {
                            description = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StateOptionMetadata.Description is a Sdk.WebAPIModelSample.Label value.")
                        }
                    },
                    enumerable: true
                },
                "Color": {
                    get: function () { return color; },
                    set: function (value) {
                        if (isString(value)) {
                            color = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StateOptionMetadata.Color is a string value.")
                        }
                    },
                    enumerable: true
                },
                "isManaged": {
                    get: function () { return isManaged; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isManaged = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StateOptionMetadata.isManaged is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "MetadataId": {
                    get: function () { return metadataid; },
                    set: function (value) {
                        if (isString(value)) {
                            metadataid = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StateOptionMetadata.MetadataId is a string value.")
                        }
                    },
                    enumerable: true
                },
                "HasChanged": {
                    get: function () { return hasChanged; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            hasChanged = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StateOptionMetadata.HasChanged is a boolean value.")
                        }
                    },
                    enumerable: true
                }
            });
    }
    this.StatusOptionMetadata = function () {
        var state;
        var transitionData;
        var _value;
        var label;
        var description;
        var color;
        var isManaged;
        var metadataid;
        var hasChanged;

        Object.defineProperties(this,
            {
                "State": {
                    get: function () { return state; },
                    set: function (value) {
                        if (isNumber(value)) {
                            state = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StatusOptionMetadata.State is a number value.")
                        }
                    },
                    enumerable: true
                },
                "TransitionData": {
                    get: function () { return transitionData; },
                    set: function (value) {
                        if (isNumber(value)) {
                            transitionData = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StatusOptionMetadata.TransitionData is a string value.")
                        }
                    },
                    enumerable: true
                },
                "Value": {
                    get: function () { return _value; },
                    set: function (value) {
                        if (isNumber(value)) {
                            _value = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StatusOptionMetadata.Value is a number value.")
                        }
                    },
                    enumerable: true
                },
                "Label": {
                    get: function () { return label; },
                    set: function (value) {
                        if (isLabel(value)) {
                            label = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StatusOptionMetadata.Label is a Sdk.WebAPIModelSample.Label value.")
                        }
                    },
                    enumerable: true
                },
                "Description": {
                    get: function () { return description; },
                    set: function (value) {
                        if (isLabel(value)) {
                            description = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StatusOptionMetadata.Description is a Sdk.WebAPIModelSample.Label value.")
                        }
                    },
                    enumerable: true
                },
                "Color": {
                    get: function () { return color; },
                    set: function (value) {
                        if (isString(value)) {
                            color = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StatusOptionMetadata.Color is a string value.")
                        }
                    },
                    enumerable: true
                },
                "isManaged": {
                    get: function () { return isManaged; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            isManaged = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StatusOptionMetadata.isManaged is a boolean value.")
                        }
                    },
                    enumerable: true
                },
                "MetadataId": {
                    get: function () { return metadataid; },
                    set: function (value) {
                        if (isString(value)) {
                            metadataid = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StatusOptionMetadata.MetadataId is a string value.")
                        }
                    },
                    enumerable: true
                },
                "HasChanged": {
                    get: function () { return hasChanged; },
                    set: function (value) {
                        if (isBoolean(value)) {
                            hasChanged = value;
                        }
                        else {
                            throw new Error("Sdk.WebAPIModelSample.StatusOptionMetadata.HasChanged is a boolean value.")
                        }
                    },
                    enumerable: true
                }
            });
    }



    //Enums: Details defined below
    this.AttributeTypeCode = {};
    this.AttributeTypeDisplayName = {};
    this.StringFormat = {};
    this.StringFormatName = {};
    this.ImeMode = {};
    this.OwnershipTypes = {};
    this.PrivilegeType = {};
    this.AttributeRequiredLevel = {};
    this.RelationshipType = {};
    this.SecurityTypes = {};
    this.AssociatedMenuBehavior = {};
    this.AssociatedMenuGroup = {};
    this.CascadeType = {};
    this.EntityKeyIndexStatus = {};
    this.OptionSetType = {};
    this.DateTimeFormat = {};
    this.DateTimeBehavior = {};
    this.IntegerFormat = {};

    //Internal Validation functions
    //Simple data types
    function isBoolean(value) {
        return (typeof (value) === "boolean");
    }
    //Validating GUID values as strings. Should include additional Validation?
    function isString(value) {
        return (typeof (value) === "string");
    }
    function isNumber(value) {
        return (typeof (value) === "number");
    }
    function isDate(value) {
        //In case the code needs to cross frame boundries, not using instanceof Date;
        return Object.prototype.toString.call(value) === '[object Date]';
    }
    //Enum Value
    function isAttributeTypeCode(value) {
        for (var i in Sdk.WebAPIModelSample.AttributeTypeCode) {
            if (value === i) {
                return true;
            }
        }
        return false;
    }
    function isAttributeTypeDisplayNames(value) {
        for (var i in Sdk.WebAPIModelSample.AttributeTypeDisplayNames) {
            if (i === value)
            { return true; }
        }
        return false;
    }
    function isAttributeRequiredLevel(value) {
        for (var i in Sdk.WebAPIModelSample.AttributeRequiredLevel) {
            if (i === value)
                return true;
        }
        return false;
    }
    function isStringFormat(value) {
        for (var i in Sdk.WebAPIModelSample.StringFormat) {
            if (i === value)
                return true;
        }
        return false;
    }
    function isImeMode(value) {
        for (var i in Sdk.WebAPIModelSample.ImeMode) {
            if (i === value)
                return true;
        }
        return false;
    }
    function isOwnershipType(value) {
        for (var i in Sdk.WebAPIModelSample.OwnershipTypes) {
            if (i === value)
                return true;
        }
        return false;
    }
    function isPrivilegeType(value) {
        for (var i in Sdk.WebAPIModelSample.PrivilegeType) {
            if (i === value)
                return true;
        }
        return false;
    }
    function isRelationshipType(value) {
        for (var i in Sdk.WebAPIModelSample.RelationshipType) {
            if (i === value)
                return true;
        }
        return false;
    }
    function isSecurityTypes(value) {
        for (var i in Sdk.WebAPIModelSample.SecurityTypes) {
            if (i === value)
                return true;
        }
        return false;
    }
    function isAssociatedMenuBehavior(value) {
        for (var i in Sdk.WebAPIModelSample.AssociatedMenuBehavior) {
            if (i === value)
                return true;
        }
        return false;
    }
    function isAssociatedMenuGroup(value) {
        for (var i in Sdk.WebAPIModelSample.AssociatedMenuGroup) {
            if (i === value)
                return true;
        }
        return false;
    }
    function isCascadeType(value) {
        for (var i in Sdk.WebAPIModelSample.CascadeType) {
            if (i === value)
                return true;
        }
        return false;
    }
    function isEntityKeyIndexStatus(value) {
        for (var i in Sdk.WebAPIModelSample.EntityKeyIndexStatus) {
            if (i === value)
                return true;
        }
        return false;
    }
    function isOptionSetType(value) {
        for (var i in Sdk.WebAPIModelSample.OptionSetType) {
            if (i === value)
                return true;
        }
        return false;
    }
    function isDateTimeFormat(value) {
        for (var i in Sdk.WebAPIModelSample.DateTimeFormat) {
            if (i === value)
                return true;
        }
        return false;
    }
    function isDateTimeBehavior(value) {
        for (var i in Sdk.WebAPIModelSample.DateTimeBehavior) {
            if (i === value)
                return true;
        }
        return false;
    }
    function isIntegerFormat(value) {
        for (var i in Sdk.WebAPIModelSample.IntegerFormat) {
            if (JSON.stringify(Sdk.WebAPIModelSample.IntegerFormat[i]) === JSON.stringify(value)) // { Value:"Locale" }
                return true;
        }
        return false;
    }
    function isStringFormatName(value) {
        for (var i in Sdk.WebAPIModelSample.StringFormatName) {
            if (JSON.stringify(Sdk.WebAPIModelSample.StringFormatName[i]) === JSON.stringify(value)) 
                return true;
        }
        return false;
    }
    function isAttributeTypeDisplayName(value) {
        for (var i in Sdk.WebAPIModelSample.AttributeTypeDisplayName) {
            if (JSON.stringify(Sdk.WebAPIModelSample.AttributeTypeDisplayName[i]) === JSON.stringify(value)) 
                return true;
        }
        return false;
    }

    //Entity Type
    function isAttributeMetadata(value) {
        return value instanceof Sdk.WebAPIModelSample.AttributeMetadata;
    }
    function isOneToManyRelationshipMetadata(value) {
        return value instanceof Sdk.WebAPIModelSample.OneToManyRelationshipMetadata;
    }
    function isManyToManyRelationshipMetadata(value) {
        return value instanceof Sdk.WebAPIModelSample.ManyToManyRelationshipMetadata;
    }
    function isOptionSetMetadataBase(value) {
        return value instanceof Sdk.WebAPIModelSample.OptionSetMetadataBase;
    }
    function isOptionSetMetadata(value) {
        return value instanceof Sdk.WebAPIModelSample.OptionSetMetadata;
    }
    function isBooleanManagedProperty(value) {
        return value instanceof Sdk.WebAPIModelSample.BooleanManagedProperty;
    }
    function isLabel(value) {
        return value instanceof Sdk.WebAPIModelSample.Label;
    }
    function isAttributeRequiredLevelManagedProperty(value) {
        return value instanceof Sdk.WebAPIModelSample.AttributeRequiredLevelManagedProperty;
    }
    function isLocalizedLabel(value) {
        return value instanceof Sdk.WebAPIModelSample.LocalizedLabel;
    }
    function isSecurityPrivilegeMetadata(value) {
        return value instanceof Sdk.WebAPIModelSample.SecurityPrivilegeMetadata;
    }
    function isAssociatedMenuConfiguration(value) {
        return value instanceof Sdk.WebAPIModelSample.AssociatedMenuConfiguration;
    }
    function isCascadeConfiguration(value) {
        return value instanceof Sdk.WebAPIModelSample.CascadeConfiguration;
    }
    function isEntityReference(value) {
        return value instanceof Sdk.WebAPIModelSample.EntityReference;
    }
    function isOptionMetadata(value) {
        return value instanceof Sdk.WebAPIModelSample.OptionMetadata;
    }
    function isStateOptionMetadata(value) {
        return value instanceof Sdk.WebAPIModelSample.StateOptionMetadata;
    }
    function isStatusOptionMetadata(value) {
        return value instanceof Sdk.WebAPIModelSample.StatusOptionMetadata;
    }
    function isBooleanOptionSetMetadata(value) {
        return value instanceof Sdk.WebAPIModelSample.BooleanOptionSetMetadata;
    }
    function isLookupAttributeMetadata(value) {
        return value instanceof Sdk.WebAPIModelSample.LookupAttributeMetadata;
    }

    //Array of type
    function isArrayOfSecurityPrivilegeMetadata(value) {
        if (Array.isArray(value)) {
            for (var i = 0; i < value.length; i++) {
                if (!isSecurityPrivilegeMetadata(value[i]))
                    return false;
            }
            return true;
        }
        return false;
    }
    function isArrayOfAttributeMetadata(value) {
        if (Array.isArray(value)) {
            for (var i = 0; i < value.length; i++) {
                if (!isAttributeMetadata(value[i]))
                    return false;
            }
            return true;
        }
        return false;
    }
    function isArrayOfOneToManyRelationshipMetadata(value) {
        if (Array.isArray(value)) {
            for (var i = 0; i < value.length; i++) {
                if (!isOneToManyRelationshipMetadata(value[i]))
                    return false;
            }
            return true;
        }
        return false;
    }
    function isArrayOfManyToManyRelationshipMetadata(value) {
        if (Array.isArray(value)) {
            for (var i = 0; i < value.length; i++) {
                if (!isManyToManyRelationshipMetadata(value[i]))
                    return false;
            }
            return true;
        }
        return false;
    }
    function isArrayOfLocalizedLabel(value) {
        if (Array.isArray(value)) {
            for (var i = 0; i < value.length; i++) {
                if (!isLocalizedLabel(value[i]))
                    return false;
            }
            return true;
        }
        return false;
    }
    function isArrayOfString(value) {
        if (Array.isArray(value)) {
            for (var i = 0; i < value.length; i++) {
                if (!isString(value[i]))
                    return false;
            }
            return true;
        }
        return false;
    }
    function isArrayOfOptionMetadata(value) {
        if (Array.isArray(value)) {
            for (var i = 0; i < value.length; i++) {
                if (!isOptionMetadata(value[i]))
                    return false;
            }
            return true;
        }
        return false;
    }



}).call(Sdk.WebAPIModelSample);

//Enum members defined
Object.defineProperties(Sdk.WebAPIModelSample.AttributeTypeCode, {
    "Boolean": {
        value: "Boolean",
        enumerable: true
    },
    "Customer": {
        value: "Customer",
        enumerable: true
    },
    "DateTime": {
        value: "DateTime",
        enumerable: true
    },
    "Decimal": {
        value: "Decimal",
        enumerable: true
    },
    "Double": {
        value: "Double",
        enumerable: true
    },
    "Integer": {
        value: "Integer",
        enumerable: true
    },
    "Lookup": {
        value: "Lookup",
        enumerable: true
    },
    "Memo": {
        value: "Memo",
        enumerable: true
    },
    "Money": {
        value: "Money",
        enumerable: true
    },
    "Owner": {
        value: "Owner",
        enumerable: true
    },
    "PartyList": {
        value: "PartyList",
        enumerable: true
    },
    "Picklist": {
        value: "Picklist",
        enumerable: true
    },
    "State": {
        value: "State",
        enumerable: true
    },
    "Status": {
        value: "Status",
        enumerable: true
    },
    "String": {
        value: "String",
        enumerable: true
    },
    "Uniqueidentifier": {
        value: "Uniqueidentifier",
        enumerable: true
    },
    "CalendarRules": {
        value: "CalendarRules",
        enumerable: true
    },
    "Virtual": {
        value: "Virtual",
        enumerable: true
    },
    "BigInt": {
        value: "BigInt",
        enumerable: true
    },
    "ManagedProperty": {
        value: "ManagedProperty",
        enumerable: true
    },
    "EntityName": {
        value: "EntityName",
        enumerable: true
    }
});
Object.freeze(Sdk.WebAPIModelSample.AttributeTypeCode);

Object.defineProperties(Sdk.WebAPIModelSample.AttributeTypeDisplayName, {
    "BigIntType": {
        value: { Value: "BigIntType" },
        enumerable: true
    },
    "BooleanType": {
        value: { Value: "BooleanType" },
        enumerable: true
    },
    "CalendarRulesType": {
        value: { Value: "CalendarRulesType" },
        enumerable: true
    },
    "CustomerType": {
        value: { Value: "CustomerType" },
        enumerable: true
    },
    "DateTimeType": {
        value: { Value: "DateTimeType" },
        enumerable: true
    },
    "DecimalType": {
        value: { Value: "DecimalType" },
        enumerable: true
    },
    "DoubleType": {
        value: { Value: "DoubleType" },
        enumerable: true
    },

    "EntityNameType": {
        value: { Value: "EntityNameType" },
        enumerable: true
    },
    "ImageType": {
        value: { Value: "ImageType" },
        enumerable: true
    },
    "IntegerType": {
        value: { Value: "IntegerType" },
        enumerable: true
    },
    "LookupType": {
        value: { Value: "LookupType" },
        enumerable: true
    },
    "ManagedPropertyType": {
        value: { Value: "ManagedPropertyType" },
        enumerable: true
    },
    "MemoType": {
        value: { Value: "MemoType" },
        enumerable: true
    },
    "MoneyType": {
        value: { Value: "MoneyType" },
        enumerable: true
    },
    "OwnerType": {
        value: { Value: "OwnerType" },
        enumerable: true
    },
    "PartyListType": {
        value: { Value: "PartyListType" },
        enumerable: true
    },
    "PicklistType": {
        value: { Value: "PicklistType" },
        enumerable: true
    },
    "StateType": {
        value: { Value: "StateType" },
        enumerable: true
    },
    "StatusType": {
        value: { Value: "StatusType" },
        enumerable: true
    },
    "StringType": {
        value: { Value: "StringType" },
        enumerable: true
    },
    "UniqueidentifierType": {
        value: { Value: "UniqueidentifierType" },
        enumerable: true
    },
    "VirtualType": {
        value: { Value: "VirtualType" },
        enumerable: true
    }
});
Object.freeze(Sdk.WebAPIModelSample.AttributeTypeDisplayName);

Object.defineProperties(Sdk.WebAPIModelSample.AttributeRequiredLevel, {
    "None": {
        value: "None",
        enumerable: true
    },
    "SystemRequired": {
        value: "SystemRequired",
        enumerable: true
    },
    "ApplicationRequired": {
        value: "ApplicationRequired",
        enumerable: true
    },
    "Recommended": {
        value: "Recommended",
        enumerable: true
    }
});
Object.freeze(Sdk.WebAPIModelSample.AttributeRequiredLevel);

Object.defineProperties(Sdk.WebAPIModelSample.StringFormat, {
    "Email": {
        value: "Email",
        enumerable: true
    },
    "Phone": {
        value: "Phone",
        enumerable: true
    },
    "PhoneticGuide": {
        value: "PhoneticGuide",
        enumerable: true
    },
    "Text": {
        value: "Text",
        enumerable: true
    },
    "TextArea": {
        value: "TextArea",
        enumerable: true
    },
    "TickerSymbol": {
        value: "TickerSymbol",
        enumerable: true
    },
    "Url": {
        value: "Url",
        enumerable: true
    },
    "VersionNumber": {
        value: "VersionNumber",
        enumerable: true
    }

});
Object.freeze(Sdk.WebAPIModelSample.StringFormat);

Object.defineProperties(Sdk.WebAPIModelSample.StringFormatName, {
    "Email": {
        value: { Value: "Email" },
        enumerable: true
    },
    "Phone": {
        value: { Value: "Phone" },
        enumerable: true
    },
    "PhoneticGuide": {
        value: { Value: "PhoneticGuide" },
        enumerable: true
    },
    "Text": {
        value: { Value: "Text" },
        enumerable: true
    },
    "TextArea": {
        value: { Value: "TextArea" },
        enumerable: true
    },
    "TickerSymbol": {
        value: { Value: "TickerSymbol" },
        enumerable: true
    },
    "Url": {
        value: { Value: "Url" },
        enumerable: true
    },
    "VersionNumber": {
        value: { Value: "VersionNumber" },
        enumerable: true
    }

});
Object.freeze(Sdk.WebAPIModelSample.StringFormatName);

Object.defineProperties(Sdk.WebAPIModelSample.ImeMode, {
    "Auto": {
        value: "Auto",
        enumerable: true
    },
    "Inactive": {
        value: "Inactive",
        enumerable: true
    },
    "Active": {
        value: "Active",
        enumerable: true
    },
    "Disabled": {
        value: "Disabled",
        enumerable: true
    }

});
Object.freeze(Sdk.WebAPIModelSample.ImeMode);

Object.defineProperties(Sdk.WebAPIModelSample.OwnershipTypes, {
    "None": {
        value: "None",
        enumerable: true
    },
    "UserOwned": {
        value: "UserOwned",
        enumerable: true
    },
    "TeamOwned": {
        value: "TeamOwned",
        enumerable: true
    },
    "BusinessOwned": {
        value: "BusinessOwned",
        enumerable: true
    },
    "OrganizationOwned": {
        value: "OrganizationOwned",
        enumerable: true
    },
    "BusinessParented": {
        value: "BusinessParented",
        enumerable: true
    }

});
Object.freeze(Sdk.WebAPIModelSample.OwnershipTypes);

Object.defineProperties(Sdk.WebAPIModelSample.PrivilegeType, {
    "None": {
        value: "None",
        enumerable: true
    },
    "Create": {
        value: "Create",
        enumerable: true
    },
    "Read": {
        value: "Read",
        enumerable: true
    },
    "Write": {
        value: "Write",
        enumerable: true
    },
    "Delete": {
        value: "Delete",
        enumerable: true
    },
    "Assign": {
        value: "Assign",
        enumerable: true
    },
    "Share": {
        value: "Share",
        enumerable: true
    },
    "Append": {
        value: "Append",
        enumerable: true
    },
    "AppendTo": {
        value: "AppendTo",
        enumerable: true
    }

});
Object.freeze(Sdk.WebAPIModelSample.PrivilegeType);

Object.defineProperties(Sdk.WebAPIModelSample.RelationshipType, {
    "OneToManyRelationship": {
        value: "OneToManyRelationship",
        enumerable: true
    },
    "ManyToManyRelationship": {
        value: "ManyToManyRelationship",
        enumerable: true
    }
});
Object.freeze(Sdk.WebAPIModelSample.RelationshipType);

Object.defineProperties(Sdk.WebAPIModelSample.SecurityTypes, {
    "None": {
        value: "None",
        enumerable: true
    },
    "Append": {
        value: "Append",
        enumerable: true
    },
    "ParentChild": {
        value: "ParentChild",
        enumerable: true
    },
    "Pointer": {
        value: "Pointer",
        enumerable: true
    },
    "Inheritance": {
        value: "Inheritance",
        enumerable: true
    }
});
Object.freeze(Sdk.WebAPIModelSample.SecurityTypes);

Object.defineProperties(Sdk.WebAPIModelSample.AssociatedMenuBehavior, {
    "UseCollectionName": {
        value: "UseCollectionName",
        enumerable: true
    },
    "UseLabel": {
        value: "UseLabel",
        enumerable: true
    },
    "DoNotDisplay": {
        value: "DoNotDisplay",
        enumerable: true
    }
});
Object.freeze(Sdk.WebAPIModelSample.AssociatedMenuBehavior);

Object.defineProperties(Sdk.WebAPIModelSample.AssociatedMenuGroup, {
    "Details": {
        value: "Details",
        enumerable: true
    },
    "Sales": {
        value: "Sales",
        enumerable: true
    },
    "Service": {
        value: "Service",
        enumerable: true
    },
    "Marketing": {
        value: "Marketing",
        enumerable: true
    }
});
Object.freeze(Sdk.WebAPIModelSample.AssociatedMenuGroup);

Object.defineProperties(Sdk.WebAPIModelSample.CascadeType, {
    "NoCascade": {
        value: "NoCascade",
        enumerable: true
    },
    "Cascade": {
        value: "Cascade",
        enumerable: true
    },
    "Active": {
        value: "Active",
        enumerable: true
    },
    "UserOwned": {
        value: "UserOwned",
        enumerable: true
    },
    "RemoveLink": {
        value: "RemoveLink",
        enumerable: true
    },
    "Restrict": {
        value: "Restrict",
        enumerable: true
    }
});
Object.freeze(Sdk.WebAPIModelSample.CascadeType);

Object.defineProperties(Sdk.WebAPIModelSample.EntityKeyIndexStatus, {
    "Pending": {
        value: "Pending",
        enumerable: true
    },
    "InProgress": {
        value: "InProgress",
        enumerable: true
    },
    "Active": {
        value: "Active",
        enumerable: true
    },
    "Failed": {
        value: "Failed",
        enumerable: true
    }
});
Object.freeze(Sdk.WebAPIModelSample.EntityKeyIndexStatus);

Object.defineProperties(Sdk.WebAPIModelSample.OptionSetType, {
    "Picklist": {
        value: "Picklist",
        enumerable: true
    },
    "State": {
        value: "State",
        enumerable: true
    },
    "Status": {
        value: "Status",
        enumerable: true
    },
    "Boolean": {
        value: "Boolean",
        enumerable: true
    }
});
Object.freeze(Sdk.WebAPIModelSample.OptionSetType);

Object.defineProperties(Sdk.WebAPIModelSample.DateTimeFormat, {
    "DateOnly": {
        value: "DateOnly",
        enumerable: true
    },
    "DateAndTime": {
        value: "DateAndTime",
        enumerable: true
    }
});
Object.freeze(Sdk.WebAPIModelSample.DateTimeFormat);

Object.defineProperties(Sdk.WebAPIModelSample.DateTimeBehavior, {

    "UserLocal": {
        value: { Value: "UserLocal" },
        enumerable: true
    },
    "DateOnly": {
        value: { Value: "DateOnly" },
        enumerable: true
    },
    "TimeZoneIndependent": {
        value: { Value: "TimeZoneIndependent" },
        enumerable: true
    }
});
Object.freeze(Sdk.WebAPIModelSample.DateTimeBehavior);

Object.defineProperties(Sdk.WebAPIModelSample.IntegerFormat, {
    "None": {
        value: { Value: "None" },
        enumerable: true
    },
    "Duration": {
        value: { Value: "Duration" },
        enumerable: true
    },
    "TimeZone": {
        value: { Value: "TimeZone" },
        enumerable: true
    },
    "Language": {
        value: { Value: "Language" },
        enumerable: true
    },
    "Locale": {
        value: { Value: "Locale" },
        enumerable: true
    }
});
Object.freeze(Sdk.WebAPIModelSample.IntegerFormat);


//Establishing inheritance
Sdk.WebAPIModelSample.AttributeMetadata.prototype = Object.create(Sdk.WebAPIModelSample.MetadataBase.prototype);
Sdk.WebAPIModelSample.RelationshipMetadataBase.prototype = Object.create(Sdk.WebAPIModelSample.MetadataBase.prototype);
Sdk.WebAPIModelSample.OneToManyRelationshipMetadata.prototype = Object.create(Sdk.WebAPIModelSample.RelationshipMetadataBase.prototype);
Sdk.WebAPIModelSample.ManyToManyRelationshipMetadata.prototype = Object.create(Sdk.WebAPIModelSample.RelationshipMetadataBase.prototype);
Sdk.WebAPIModelSample.EntityKeyMetadata.prototype = Object.create(Sdk.WebAPIModelSample.MetadataBase.prototype);
Sdk.WebAPIModelSample.OptionSetMetadataBase.prototype = Object.create(Sdk.WebAPIModelSample.MetadataBase.prototype);
Sdk.WebAPIModelSample.OptionSetMetadata.prototype = Object.create(Sdk.WebAPIModelSample.OptionSetMetadataBase.prototype);
Sdk.WebAPIModelSample.BooleanOptionSetMetadata.prototype = Object.create(Sdk.WebAPIModelSample.OptionSetMetadataBase.prototype);
//Attributes
Sdk.WebAPIModelSample.BigIntAttributeMetadata.prototype = Object.create(Sdk.WebAPIModelSample.AttributeMetadata.prototype);
Sdk.WebAPIModelSample.BooleanAttributeMetadata.prototype = Object.create(Sdk.WebAPIModelSample.AttributeMetadata.prototype);
Sdk.WebAPIModelSample.DateTimeAttributeMetadata.prototype = Object.create(Sdk.WebAPIModelSample.AttributeMetadata.prototype);
Sdk.WebAPIModelSample.DecimalAttributeMetadata.prototype = Object.create(Sdk.WebAPIModelSample.AttributeMetadata.prototype);
Sdk.WebAPIModelSample.DoubleAttributeMetadata.prototype = Object.create(Sdk.WebAPIModelSample.AttributeMetadata.prototype);
Sdk.WebAPIModelSample.EnumAttributeMetadata.prototype = Object.create(Sdk.WebAPIModelSample.AttributeMetadata.prototype);
Sdk.WebAPIModelSample.ImageAttributeMetadata.prototype = Object.create(Sdk.WebAPIModelSample.AttributeMetadata.prototype);
Sdk.WebAPIModelSample.IntegerAttributeMetadata.prototype = Object.create(Sdk.WebAPIModelSample.AttributeMetadata.prototype);
Sdk.WebAPIModelSample.LookupAttributeMetadata.prototype = Object.create(Sdk.WebAPIModelSample.AttributeMetadata.prototype);
Sdk.WebAPIModelSample.MemoAttributeMetadata.prototype = Object.create(Sdk.WebAPIModelSample.AttributeMetadata.prototype);
Sdk.WebAPIModelSample.MoneyAttributeMetadata.prototype = Object.create(Sdk.WebAPIModelSample.AttributeMetadata.prototype);
Sdk.WebAPIModelSample.StringAttributeMetadata.prototype = Object.create(Sdk.WebAPIModelSample.AttributeMetadata.prototype);
Sdk.WebAPIModelSample.UniqueIdentifierAttributeMetadata.prototype = Object.create(Sdk.WebAPIModelSample.AttributeMetadata.prototype);
Sdk.WebAPIModelSample.EntityNameAttributeMetadata.prototype = Object.create(Sdk.WebAPIModelSample.EnumAttributeMetadata.prototype);
Sdk.WebAPIModelSample.PicklistAttributeMetadata.prototype = Object.create(Sdk.WebAPIModelSample.EnumAttributeMetadata.prototype);
Sdk.WebAPIModelSample.StateAttributeMetadata.prototype = Object.create(Sdk.WebAPIModelSample.EnumAttributeMetadata.prototype);
Sdk.WebAPIModelSample.StatusAttributeMetadata.prototype = Object.create(Sdk.WebAPIModelSample.EnumAttributeMetadata.prototype);




