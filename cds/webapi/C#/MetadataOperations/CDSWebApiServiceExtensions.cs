using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerApps.Samples.Metadata
{
    public static class CDSWebApiServiceExtensions
    {
        /// <summary>
        /// The strong consistency header.
        /// Strong consistency offers a linearizability guarantee. Linearizability refers to serving requests concurrently.
        /// The reads are guaranteed to return the most recent committed version of an item.
        /// Users are always guaranteed to read the latest committed write.
        /// Use of this header incurs a small overhead and is therefore not suited to high volume workloads.
        /// </summary>
        private static readonly Dictionary<string, List<string>> strongConsistencyHeader = new Dictionary<string, List<string>>()
        {
            {"Consistency", new List<string> { "Strong" } }
        };

        /// <summary>
        /// Adds a solution component to an unmanaged solution.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="componentId">ID of the solution component.</param>
        /// <param name="componentType">The solution component to add to the unmanaged solution.</param>
        /// <param name="solutionUniqueName">Unique name of the solution.</param>
        /// <param name="addRequiredComponents">Indicates whether other solution components that are required by the solution component should also be added to the unmanaged solution.</param>
        /// <param name="doNotIncludeSubComponents">Indicates whether the subcomponents should be included.</param>
        /// <param name="includedComponentsSettingsValues">Any settings to be included with the component.</param>
        /// <returns>The ID of the new solution component record.</returns>
        public static Guid AddSolutionComponent(this CDSWebApiService svc,
            Guid componentId,
            SolutionComponentType componentType,
            string solutionUniqueName,
            bool addRequiredComponents,
            bool? doNotIncludeSubComponents = null,
            string includedComponentsSettingsValues = null)
        {
            Guid solutionComponentId;

            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                throw new Exception("AddSolutionComponent requires the solutionUniqueName parameter value.");
            }

            //Add the attribute as part of a solution
            JObject addSolutionComponentParameters = new JObject
            {
                ["ComponentId"] = componentId,
                ["ComponentType"] = (int)componentType,
                ["SolutionUniqueName"] = solutionUniqueName,
                ["AddRequiredComponents"] = addRequiredComponents
            };
            if (doNotIncludeSubComponents != null)
            {
                addSolutionComponentParameters["DoNotIncludeSubcomponents"] = doNotIncludeSubComponents;
            }
            if (includedComponentsSettingsValues != null)
            {
                addSolutionComponentParameters["IncludedComponentSettingsValues"] = includedComponentsSettingsValues;
            }

            try
            {
                JObject addSolutionComponentResponse = new JObject();

                addSolutionComponentResponse = svc.Post("AddSolutionComponent", addSolutionComponentParameters, strongConsistencyHeader);

                solutionComponentId = new Guid(addSolutionComponentResponse.GetValue("id").ToString());
            }
            catch (Exception ex)
            {
                throw new Exception($"There was a problem adding the {componentType} as a component to the {solutionUniqueName} solution:{ex.Message}");
            }

            return solutionComponentId;
        }

        /// <summary>
        /// Checks whether the specified entity can be the primary entity (one) in a one-to-many relationship.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="entityName">The logical name of the entity to check.</param>
        /// <returns>Whether the specified entity can be the primary entity (one) in a one-to-many relationship.</returns>
        public static bool CanBeReferenced(this CDSWebApiService svc, string entityName)
        {
            try
            {
                JObject body = new JObject()
                {
                    ["EntityName"] = entityName
                };
                return (bool)svc.Post("CanBeReferenced", body)["CanBeReferenced"];
            }
            catch (ServiceException se)
            {
                throw new Exception($"Service Error:{se.Message}", se);
            }
            catch (Exception ex)
            {
                throw new Exception("There was a problem using the CanBeReferenced Action", ex);
            }
        }

        /// <summary>
        /// Checks whether an entity can be the referencing entity in a one-to-many relationship.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="entityName">The logical name of the entity to check.</param>
        /// <returns>Whether an entity can be the referencing entity in a one-to-many relationship.</returns>
        public static bool CanBeReferencing(this CDSWebApiService svc, string entityName)
        {
            try
            {
                JObject body = new JObject()
                {
                    ["EntityName"] = entityName
                };
                return (bool)svc.Post("CanBeReferencing ", body)["CanBeReferencing"];
            }
            catch (ServiceException se)
            {
                throw new Exception($"Service Error: {se.Message}", se);
            }
            catch (Exception ex)
            {
                throw new Exception("There was a problem using the CanBeReferencing Action", ex);
            }
        }

        /// <summary>
        /// Checks whether an entity can participate in a many-to-many relationship.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="entitylogicalname">The logical name of the entity to check.</param>
        /// <returns>Whether an entity can participate in a many-to-many relationship.</returns>
        public static bool CanManyToMany(this CDSWebApiService svc, string entitylogicalname)
        {
            try
            {
                JObject body = new JObject()
                {
                    ["EntityName"] = entitylogicalname
                };
                return (bool)svc.Post("CanManyToMany", body)["CanManyToMany"];
            }
            catch (ServiceException se)
            {
                throw new Exception($"Service Error: {se.Message}", se);
            }
            catch (Exception ex)
            {
                throw new Exception("There was a problem using the CanManyToMany Action", ex);
            }
        }

        /// <summary>
        /// Creates an attribute and adds it to an unmanaged solution
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="entityLogicalName">The logical name of the entity to add the attribute to.</param>
        /// <param name="attribute">The attribute to add.</param>
        /// <param name="solutionUniqueName">The unique name of the unmanaged solution to add the attribute to.</param>
        /// <returns>The Id of the attribute and solution component</returns>
        public static CreateAttributeResponse CreateAttribute(this CDSWebApiService svc,
            string entityLogicalName,
            AttributeMetadata attribute,
            string solutionUniqueName = null)
        {
            Uri uri;
            Guid attributeMetadataId;
            Guid solutionId;
            Guid solutionComponentId;

            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                //Verify that solution exists and is unmanaged.
                try
                {
                    solutionId = svc.CheckSolution(solutionUniqueName);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            //Create the attribute
            try
            {
                uri = svc.PostCreate($"EntityDefinitions(LogicalName='{entityLogicalName}')/Attributes", JObject.FromObject(attribute));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating the attribute:{ex.Message}");
            }

            //Get the id from the URI
            string id = uri.ToString().Substring(uri.ToString().IndexOf("Attributes(") + 11, 36);
            attributeMetadataId = new Guid(id);

            try
            {
                solutionComponentId = svc.AddSolutionComponent(attributeMetadataId, SolutionComponentType.Attribute, solutionUniqueName, true);
            }
            catch (Exception)
            {
                throw;
            }

            return new CreateAttributeResponse()
            {
                AttributeId = attributeMetadataId,
                SolutionComponentId = solutionComponentId
            };
        }

        /// <summary>
        /// Creates a customer relationship
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="lookup">The customer lookup to create.</param>
        /// <param name="oneToManyRelationships">A pair of relationships to create for the account and contact entities.</param>
        /// <param name="solutionUniqueName">The unique name of the unmanaged solution.</param>
        /// <returns>Id values of the created attribute and relationships</returns>
        public static CreateCustomerRelationshipsResponse CreateCustomerRelationships(this CDSWebApiService svc,
            ComplexLookupAttributeMetadata lookup,
            List<ComplexOneToManyRelationshipMetadata> oneToManyRelationships,
            string solutionUniqueName = null)
        {
            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                //Verify that solution exists and is unmanaged.
                try
                {
                    svc.CheckSolution(solutionUniqueName);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            JObject args = new JObject
            {
                ["Lookup"] = JObject.FromObject(lookup),
                ["OneToManyRelationships"] = JArray.FromObject(oneToManyRelationships)
            };
            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                args["SolutionUniqueName"] = solutionUniqueName;
            }

            CreateCustomerRelationshipsResponse response;
            try
            {
                response = JsonConvert.DeserializeObject<CreateCustomerRelationshipsResponse>(
                    svc.Post("CreateCustomerRelationships", args).ToString()
                    );
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in CreateCustomerRelationships: {ex.Message}");
            }

            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                try
                {
                    svc.AddSolutionComponent(response.AttributeId, SolutionComponentType.Attribute, solutionUniqueName, true);
                    svc.AddSolutionComponent(response.RelationshipIds.First(), SolutionComponentType.Relationship, solutionUniqueName, true);
                    svc.AddSolutionComponent(response.RelationshipIds.Skip(1).First(), SolutionComponentType.Relationship, solutionUniqueName, true);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error adding the items to a solution: {ex.Message}");
                }
            }

            return response;
        }

        /// <summary>
        /// Creates an entity
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="entity">The EntityMetadata</param>
        /// <param name="primaryAttribute">The Primary attribute of the entity.</param>
        /// <param name="solutionUniqueName">The unique name of the unmanaged solution to add the entity to.</param>
        /// <returns>The Ids of the entity and primary name attribute created.</returns>
        public static CreateEntityResponse CreateEntity(this CDSWebApiService svc,
            EntityMetadata entity,
            StringAttributeMetadata primaryAttribute,
            string solutionUniqueName = null)
        {
            Guid solutionId;
            Uri uri;
            Guid entityid;

            if (entity.DisplayName?.LocalizedLabels[0]?.Label == null)
            {
                throw new Exception("Entity.DisplayName is required.");
            }

            if (entity.DisplayCollectionName?.LocalizedLabels[0]?.Label == null)
            {
                throw new Exception("Entity.DisplayCollectionName is required.");
            }

            if (string.IsNullOrEmpty(entity.SchemaName))
            {
                throw new Exception("Entity.SchemaName is required.");
            }

            if (!entity.HasNotes.HasValue)
            {
                throw new Exception("Entity.HasValue is required.");
            }
            if (!entity.HasActivities.HasValue)
            {
                throw new Exception("Entity.HasActivities is required.");
            }

            if (string.IsNullOrEmpty(primaryAttribute.SchemaName))
            {
                throw new Exception("PrimaryAttribute.SchemaName is required.");
            }

            if (!primaryAttribute.MaxLength.HasValue)
            {
                throw new Exception("PrimaryAttribute.MaxLength is required.");
            }

            if (primaryAttribute.DisplayName?.LocalizedLabels[0]?.Label == null)
            {
                throw new Exception("PrimaryAttribute.DisplayName is required.");
            }

            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                //Verify that solution exists and is unmanaged.
                try
                {
                    solutionId = svc.CheckSolution(solutionUniqueName);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            if (!entity.Attributes.Contains(primaryAttribute))
            {
                entity.Attributes.Add(primaryAttribute);
            }
            //Required
            primaryAttribute.IsPrimaryName = true;
            entity.PrimaryNameAttribute = primaryAttribute.LogicalName;

            try
            {
                uri = svc.PostCreate("EntityDefinitions", JObject.FromObject(entity));
                string id = uri.ToString().Substring(uri.ToString().IndexOf("(") + 1, 36);
                entityid = new Guid(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating the entity: {ex.Message}");
            }

            AttributeMetadata attribute;
            try
            {
                attribute = svc.RetrieveAttribute<StringAttributeMetadata>(entity.LogicalName ?? entity.SchemaName.ToLower(), primaryAttribute.LogicalName ?? primaryAttribute.SchemaName.ToLower(), "MetadataId");
            }
            catch (ServiceException se)
            {
                throw new Exception($"Error retrieving the primary attribute created with the entity:{se.Message}");
            }

            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                try
                {
                    svc.AddSolutionComponent(entityid, SolutionComponentType.Entity, solutionUniqueName, true);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error adding the entity to a solution: {ex.Message}");
                }
            }

            return new CreateEntityResponse
            {
                AttributeId = (Guid)attribute.MetadataId,
                EntityId = entityid
            };
        }

        /// <summary>
        /// Creates a global option set.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="optionset">The OptionSet to create.</param>
        /// <param name="solutionUniqueName">The unique name of the unmanaged solution to add the global optionset to.</param>
        /// <returns>The Id of the Global OptionSet</returns>
        public static Guid CreateGlobalOptionSet(this CDSWebApiService svc,
            OptionSetMetadata optionset,
            string solutionUniqueName = null)
        {
            Uri uri;
            Guid optionSetId;
            Guid solutionId;

            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                //Verify that solution exists and is unmanaged.
                try
                {
                    solutionId = svc.CheckSolution(solutionUniqueName);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            try
            {
                uri = svc.PostCreate($"GlobalOptionSetDefinitions", JObject.FromObject(optionset));
                string id = uri.ToString().Substring(uri.ToString().IndexOf("(") + 1, 36);
                optionSetId = new Guid(id);
            }
            catch (ServiceException se)
            {
                throw new Exception($"Error creating Global Option Set:{se.Message}", se);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating Global Option Set:{ex.Message}", ex);
            }

            try
            {
                svc.AddSolutionComponent(optionSetId, SolutionComponentType.OptionSet, solutionUniqueName, true);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding the global optionset to the solution:{ex.Message}", ex);
            }

            return optionSetId;
        }

        /// <summary>
        /// Creates a ManyToMany entity relationship.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="manyToManyRelationship">The ManyToManyRelationship to create.</param>
        /// <param name="solutionUniqueName">The unique name of the unmanaged solution to add the attribute to.</param>
        /// <returns>The MetadataId of the created relationship.</returns>
        public static Guid CreateManyToMany(this CDSWebApiService svc,
        ManyToManyRelationshipMetadata manyToManyRelationship,
        string solutionUniqueName = null)
        {
            Uri uri;
            Guid relationshipId;
            Guid solutionId;

            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                //Verify that solution exists and is unmanaged.
                try
                {
                    solutionId = svc.CheckSolution(solutionUniqueName);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            //Create the relationship
            try
            {
                uri = svc.PostCreate($"RelationshipDefinitions", JObject.FromObject(manyToManyRelationship));
            }
            catch (Exception)
            {
                throw new Exception($"There was a problem creating the ManyToManyRelationship.");
            }
            //Get the ID of the relationship from the Uri;
            string id = uri.ToString().Substring(uri.ToString().IndexOf("(") + 1, 36);
            relationshipId = new Guid(id);

            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                try
                {
                    svc.AddSolutionComponent(relationshipId, SolutionComponentType.EntityRelationship, solutionUniqueName, true);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return relationshipId;
        }

        /// <summary>
        /// Creates a One-to-Many relationship
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="oneToManyRelationship">The One-to-many entity relationships to create.</param>
        /// <param name="lookup">The lookup attribute for the relationship</param>
        /// <param name="solutionUniqueName">The unique name of the unmanaged solution to add the attribute to.</param>
        /// <returns>The Id of the lookup attribute and the relationship created.</returns>
        public static CreateOneToManyResponse CreateOneToMany(this CDSWebApiService svc,
            OneToManyRelationshipMetadata oneToManyRelationship,
            LookupAttributeMetadata lookup,
            string solutionUniqueName)
        {
            Uri uri;
            Guid relationshipId;
            Guid solutionId;
            Guid attributeId;

            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                //Verify that solution exists and is unmanaged.
                try
                {
                    solutionId = svc.CheckSolution(solutionUniqueName);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            //Set the lookup to the relationship
            oneToManyRelationship.Lookup = lookup;

            try
            {
                uri = svc.PostCreate($"RelationshipDefinitions", JObject.FromObject(oneToManyRelationship));
                string id = uri.ToString().Substring(uri.ToString().IndexOf("(") + 1, 36);
                relationshipId = new Guid(id);
            }
            catch (Exception)
            {
                throw;
            }

            try
            {
                LookupAttributeMetadata attribute = null;

                var lookupLogicalName = lookup.LogicalName ?? lookup.SchemaName.ToLower();

                attribute = svc.RetrieveAttribute<LookupAttributeMetadata>(
                   oneToManyRelationship.ReferencingEntity,
                   lookupLogicalName,
                   "MetadataId");

                attributeId = (Guid)attribute.MetadataId;
            }
            catch (Exception)
            {
                throw;
            }

            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                try
                {
                    svc.AddSolutionComponent(relationshipId, SolutionComponentType.EntityRelationship, solutionUniqueName, true);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return new CreateOneToManyResponse
            {
                RelationshipId = relationshipId,
                AttributeId = attributeId
            };
        }

        /// <summary>
        /// Deletes an attribute
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="entityLogicalName">The logical name of the entity that contains the attribute.</param>
        /// <param name="attributeLogicalName">The logical name of the attribute to delete.</param>
        public static void DeleteAttribute(this CDSWebApiService svc, string entityLogicalName, string attributeLogicalName)
        {
            try
            {
                svc.Delete("EntityDefinitions" +
                    $"(LogicalName='{entityLogicalName}')/Attributes" +
                    $"(LogicalName='{attributeLogicalName}')", strongConsistencyHeader);
            }
            catch (Exception)
            {
                throw new Exception($"There was a problem deleting the {attributeLogicalName} attribute in the {entityLogicalName} entity.");
            }
        }

        /// <summary>
        /// Deletes an entity by Id
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="entityMetadataId">The MetadataId of the entity to delete.</param>
        public static void DeleteEntity(this CDSWebApiService svc,
            Guid entityMetadataId)
        {
            svc.DeleteEntity(entityMetadataId, null);
        }

        /// <summary>
        /// Deletes an entity by name
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="entityLogicalName">The logical Name of the entity to delete</param>
        public static void DeleteEntity(this CDSWebApiService svc,
            string entityLogicalName)
        {
            svc.DeleteEntity(null, entityLogicalName);
        }

        /// <summary>
        /// Deletes a global optionset by Id.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="optionSetId">The Id of the optionset.</param>
        public static void DeleteGlobalOptionSet(this CDSWebApiService svc,
            Guid optionSetId)
        {
            svc.DeleteGlobalOptionSet(optionSetId, null);
        }

        /// <summary>
        /// Deletes a global optionset by name.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="name">The name of the optionset.</param>
        public static void DeleteGlobalOptionSet(this CDSWebApiService svc,
            string name)
        {
            svc.DeleteGlobalOptionSet(null, name);
        }

        /// <summary>
        /// Deletes an option from a Global OptionSet
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="optionSetName">The name of the Global OptionSet</param>
        /// <param name="value">The value of the option to delete.</param>
        /// <param name="solutionUniqueName">The name of the solution</param>
        public static void DeleteGlobalOptionSetOptionValue(this CDSWebApiService svc,
            string optionSetName,
            int value,
            string solutionUniqueName = null)
        {
            Guid solutionId;
            Guid optionsetId;
            OptionSetMetadata optionset;

            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                //Verify that solution exists and is unmanaged.
                try
                {
                    solutionId = svc.CheckSolution(solutionUniqueName);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            //Retrieve the Global OptionSet
            try
            {
                optionset = svc.RetrieveGlobalOptionSet(null, optionSetName);
                optionsetId = (Guid)optionset.MetadataId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving the global optionset {optionSetName}", ex);
            }

            bool exists = optionset.Options.Any(x => x.Value == value);
            if (!exists)
            {
                throw new Exception($"The value {value} doesn't exist in the global optionset {optionSetName}.");
            }

            JObject args = new JObject
            {
                ["OptionSetName"] = optionSetName,
                ["Value"] = value
            };
            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                args["SolutionUniqueName"] = solutionUniqueName;
            }
            try
            {
                svc.Post("DeleteOptionValue", args);
            }
            catch (Exception ex)
            {
                throw new Exception("Error using DeleteGlobalOptionSetOptionValue", ex);
            }
        }

        /// <summary>
        /// Deletes a local optionset
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="entityLogicalName">The logical name of the entity containing the optionset</param>
        /// <param name="attributeLogicalName">The logical name of the attribute.</param>
        /// <param name="value">The value of the option to remove.</param>
        public static void DeleteLocalOptionSetOptionValue(this CDSWebApiService svc,
            string entityLogicalName,
            string attributeLogicalName,
            int value)
        {
            JObject args = new JObject
            {
                ["EntityLogicalName"] = entityLogicalName,
                ["AttributeLogicalName"] = attributeLogicalName,
                ["Value"] = value
            };

            try
            {
                svc.Post("DeleteOptionValue", args);
            }
            catch (Exception ex)
            {
                throw new Exception("Error using DeleteLocalOptionSetOptionValue", ex);
            }
        }

        /// <summary>
        /// Deletes an entity relationship by Id.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="relationshipId">The MetadataId of the relationship.</param>
        public static void DeleteRelationship(this CDSWebApiService svc,
            Guid relationshipId)
        {
            //Sometimes serveral attempts are required.
            DeleteRelationship(svc, relationshipId, null);
        }

        /// <summary>
        /// Deletes an entity relationship by name.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="relationshipSchemaName">The SchemaName of the relationship.</param>
        public static void DeleteRelationship(this CDSWebApiService svc,
            string relationshipSchemaName)
        {
            DeleteRelationship(svc, null, relationshipSchemaName);
        }

        /// <summary>
        /// Exports a solution.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="solutionName">The unique name of the solution.</param>
        /// <param name="managed">Indicates whether the solution should be exported as a managed solution.</param>
        /// <param name="targetVersion">The version that the exported solution will support.</param>
        /// <param name="exportAutoNumberingSettings">Indicates whether auto numbering settings should be included in the solution being exported.</param>
        /// <param name="exportCalendarSettings">Indicates whether calendar settings should be included in the solution being exported</param>
        /// <param name="exportCustomizationSettings">Indicates whether customization settings should be included in the solution being exported.</param>
        /// <param name="exportEmailTrackingSettings">Indicates whether email tracking settings should be included in the solution being exported.</param>
        /// <param name="exportGeneralSettings">Indicates whether general settings should be included in the solution being exported.</param>
        /// <param name="exportMarketingSettings">Indicates whether marketing settings should be included in the solution being exported.</param>
        /// <param name="exportOutlookSynchronizationSettings">Indicates whether outlook synchronization settings should be included in the solution being exported.</param>
        /// <param name="exportRelationshipRoles">Indicates whether relationship role settings should be included in the solution being exported.</param>
        /// <param name="exportIsvConfig">Indicates whether ISV.Config settings should be included in the solution being exported.</param>
        /// <param name="exportSales">	Indicates whether sales settings should be included in the solution being exported</param>
        /// <param name="exportExternalApplications"></param>
        /// <returns>A byte array containing the data for the file.</returns>
        public static byte[] ExportSolution(this CDSWebApiService svc,
            string solutionName,
            bool managed,
            string targetVersion = null,
            bool exportAutoNumberingSettings = false,
            bool exportCalendarSettings = false,
            bool exportCustomizationSettings = false,
            bool exportEmailTrackingSettings = false,
            bool exportGeneralSettings = false,
            bool exportMarketingSettings = false,
            bool exportOutlookSynchronizationSettings = false,
            bool exportRelationshipRoles = false,
            bool exportIsvConfig = false,
            bool exportSales = false,
            bool exportExternalApplications = false

            )
        {
            JObject args = new JObject
            {
                ["SolutionName"] = solutionName,
                ["Managed"] = managed,
                ["TargetVersion"] = targetVersion,
                ["ExportAutoNumberingSettings"] = exportAutoNumberingSettings,
                ["ExportCalendarSettings"] = exportCalendarSettings,
                ["ExportCustomizationSettings"] = exportCustomizationSettings,
                ["ExportEmailTrackingSettings"] = exportEmailTrackingSettings,
                ["ExportGeneralSettings"] = exportGeneralSettings,
                ["ExportMarketingSettings"] = exportMarketingSettings,
                ["ExportOutlookSynchronizationSettings"] = exportOutlookSynchronizationSettings,
                ["ExportRelationshipRoles"] = exportRelationshipRoles,
                ["ExportIsvConfig"] = exportIsvConfig,
                ["ExportSales"] = exportSales,
                ["ExportExternalApplications"] = exportExternalApplications
            };
            try
            {
                var response = svc.Post("ExportSolution", args);
                return Convert.FromBase64String(response["ExportSolutionFile"].ToString());
            }
            catch (ServiceException se)
            {
                throw new Exception($"Service Error in ExportSolution: {se.Message}", se);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in ExportSolution: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Retrieves a list of all the entities that can participate in a Many-to-Many entity relationship.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <returns>A list of all the entities that can participate in a Many-to-Many entity relationship.</returns>
        public static List<string> GetValidManyToMany(this CDSWebApiService svc)
        {
            try
            {
                var results = svc.Get($"GetValidManyToMany", strongConsistencyHeader)["EntityNames"].ToString();
                var entityNames = JsonConvert.DeserializeObject<List<string>>(results);
                return entityNames;
            }
            catch (Exception ex)
            {
                throw new Exception("Error using GetValidManyToMany", ex);
            }
        }

        /// <summary>
        /// Retrieves a list of entity logical names that are valid as the primary entity (one) from the specified entity in a one-to-many relationship.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="referencingEntityName">The logical name of the entity to get valid referenced entities.</param>
        /// <returns>A list of entity logical names that are valid as the primary entity (one) from the specified entity in a one-to-many relationship.</returns>
        public static List<string> GetValidReferencedEntities(this CDSWebApiService svc, string referencingEntityName)
        {
            try
            {
                var results = svc.Get($"GetValidReferencedEntities(ReferencingEntityName=@p1)" +
                    $"?@p1='{referencingEntityName}'", strongConsistencyHeader)["EntityNames"].ToString();
                var entityNames = JsonConvert.DeserializeObject<List<string>>(results);
                return entityNames;
            }
            catch (Exception ex)
            {
                throw new Exception("Error using GetValidReferencedEntities ", ex);
            }
        }

        /// <summary>
        /// Retrieves the set of entities that are valid as the related entity (many) to the specified entity in a one-to-many relationship.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="referencedEntityName">The name of the primary entity in the relationship</param>
        /// <returns>The set of entities that are valid as the related entity (many) to the specified entity in a one-to-many relationship.</returns>
        public static List<string> GetValidReferencingEntities(this CDSWebApiService svc, string referencedEntityName)
        {
            try
            {
                var results = svc.Get($"GetValidReferencingEntities(ReferencedEntityName=@p1)" +
                    $"?@p1='{referencedEntityName}'", strongConsistencyHeader)["EntityNames"].ToString();
                var entityNames = JsonConvert.DeserializeObject<List<string>>(results);
                return entityNames;
            }
            catch (Exception ex)
            {
                throw new Exception("Error using GetValidReferencingEntities.", ex);
            }
        }

        /// <summary>
        /// Imports a solution.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="overwriteUnmanagedCustomizations">Indicates whether any unmanaged customizations that have been applied over existing managed solution components should be overwritten</param>
        /// <param name="publishWorkflows">Indicates whether any processes (workflows) included in the solution should be activated after they are imported.</param>
        /// <param name="customizationFile">The compressed solutions file to import.</param>
        /// <param name="importJobId">The ID of the import job that will be created to perform the import.</param>
        /// <param name="convertToManaged">Converts any matching unmanaged customizations into your managed solution.</param>
        /// <param name="skipProductUpdateDependencies">Indicates whether enforcement of dependencies related to product updates should be skipped.</param>
        /// <param name="holdingSolution">No Description</param>
        /// <param name="skipQueueRibbonJob">No Description</param>
        /// <param name="layerDesiredOrder">No Description</param>
        /// <param name="asyncRibbonProcessing">No Description</param>
        /// <param name="componentParameters">No Description</param>
        public static void ImportSolution(this CDSWebApiService svc,
            byte[] customizationFile,
            Guid importJobId = new Guid(),
            bool overwriteUnmanagedCustomizations = false,
            bool publishWorkflows = false,
            bool convertToManaged = false,
            bool skipProductUpdateDependencies = false,
            bool holdingSolution = false,
            bool skipQueueRibbonJob = false,
            LayerDesiredOrder layerDesiredOrder = null,
            bool asyncRibbonProcessing = false,
            JObject componentParameters = null)
        {
            var args = new JObject
            {
                ["AsyncRibbonProcessing"] = asyncRibbonProcessing,
                ["ConvertToManaged"] = convertToManaged,
                ["CustomizationFile"] = customizationFile,
                ["HoldingSolution"] = holdingSolution,
                ["ImportJobId"] = importJobId,
                ["OverwriteUnmanagedCustomizations"] = skipQueueRibbonJob,
                ["PublishWorkflows"] = publishWorkflows,
                ["SkipProductUpdateDependencies"] = skipProductUpdateDependencies,
                ["SkipQueueRibbonJob"] = overwriteUnmanagedCustomizations,
                ["OverwriteUnmanagedCustomizations"] = overwriteUnmanagedCustomizations
            };

            if (layerDesiredOrder != null)
            {
                args["LayerDesiredOrder"] = JToken.FromObject(layerDesiredOrder);
            }
            if (componentParameters != null)
            {
                args["ComponentParameters"] = componentParameters;
            }

            try
            {
                svc.Post("ImportSolution", args);
            }
            catch (ServiceException se)
            {
                throw new Exception($"Servicew Error in ImportSolution: {se.Message}", se);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in ImportSolution: {ex.Message}", ex);
            }
        }
        /// <summary>
        /// Inserts an option into a Global OptionSet
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="optionSetName">The name of the global optionset</param>
        /// <param name="value">The new value. If no value is provided one will be generated.</param>
        /// <param name="label">The label of the option.</param>
        /// <param name="description">The description of the option</param>
        /// <param name="parentValues">The parent values of the option.</param>
        /// <param name="solutionUniqueName">The name of the solution.</param>
        /// <returns>The optionset value.</returns>
        public static int InsertGlobalOptionSetOptionValue(this CDSWebApiService svc,
                string optionSetName,
                Label label,
                int? value = null,
                Label description = null,
                int[] parentValues = null,
                string solutionUniqueName = null)
        {
            Guid solutionId;
            Guid optionsetId;
            OptionSetMetadata optionset;
            int NewOptionValue;

            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                //Verify that solution exists and is unmanaged.
                try
                {
                    solutionId = svc.CheckSolution(solutionUniqueName);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            //Retrieve the Global OptionSet
            try
            {
                optionset = svc.RetrieveGlobalOptionSet(null, optionSetName);
                optionsetId = (Guid)optionset.MetadataId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving the global optionset {optionSetName}", ex);
            }

            bool exists = optionset.Options.Any(x => x.Value == value);
            if (exists)
            {
                throw new Exception($"The value {value} already exists in the global optionset {optionSetName}.");
            }

            JObject args = new JObject
            {
                ["OptionSetName"] = optionSetName
            };
            if (value != null)
            {
                args["Value"] = value;
            }

            if (label != null)
            {
                args["Label"] = JObject.FromObject(label);
            }

            if (description != null)
            {
                args["Description"] = JObject.FromObject(description);
            }

            if (parentValues != null)
            {
                args["ParentValues"] = JObject.FromObject(parentValues);
            }

            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                args["SolutionUniqueName"] = solutionUniqueName;
            }

            try
            {
                NewOptionValue = (int)svc.Post("InsertOptionValue", args)["NewOptionValue"];
            }
            catch (Exception ex)
            {
                throw new Exception("Error using InsertGlobalOptionSetOptionValue", ex);
            }

            return NewOptionValue;
        }

        /// <summary>
        /// Inserts a local optionset
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="entityLogicalName">The logical name of the entity to contain the optionset</param>
        /// <param name="attributeLogicalName">The logical name of the attribute that contains the optionset</param>
        /// <param name="label">The label of the optionset to add.</param>
        /// <param name="value">The value of the optionset to add.</param>
        /// <param name="description">The description of the optionset to add.</param>
        /// <param name="parentValues">The parent values of the optionset to add.</param>
        /// <param name="solutionUniqueName">The unique name of the solution to add the optionset to</param>
        /// <returns>The optionset value.</returns>
        public static int InsertLocalOptionSetOptionValue(this CDSWebApiService svc,
            string entityLogicalName,
            string attributeLogicalName,
            Label label,
            int? value = null,
            Label description = null,
            int[] parentValues = null,
            string solutionUniqueName = null)
        {
            Guid solutionId;
            OptionSetMetadata optionset = null;
            int NewOptionValue;

            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                //Verify that solution exists and is unmanaged.
                try
                {
                    solutionId = svc.CheckSolution(solutionUniqueName);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            try
            {
                optionset = svc.RetrieveLocalOptionSet(entityLogicalName, attributeLogicalName);
            }
            catch (ServiceException se)
            {
                throw new Exception($"Service Error retrieving optionset:{se.Message}", se);
            }

            if (optionset.IsGlobal.HasValue & optionset.IsGlobal.Value == true)
            {
                throw new Exception($"The optionset is a global optionset. You must insert the value into the {optionset.Name} global optionset.");
            }

            if (value != null)
            {
                if (optionset.Options.Any(x => x.Value == value))
                {
                    throw new Exception($"The option value {value} is already being used by this optionset.");
                }
            }

            JObject args = new JObject
            {
                ["EntityLogicalName"] = entityLogicalName,
                ["AttributeLogicalName"] = attributeLogicalName,
                ["Label"] = JObject.FromObject(label)
            };
            if (description != null)
            {
                args["Description"] = JObject.FromObject(description);
            }
            if (parentValues != null)
            {
                args["ParentValues"] = JObject.FromObject(parentValues);
            }
            if (solutionUniqueName != null)
            {
                args["SolutionUniqueName"] = solutionUniqueName;
            }
            try
            {
                NewOptionValue = (int)svc.Post("InsertOptionValue", args)["NewOptionValue"];
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting local option:{ex.Message}");
            }
            return NewOptionValue;
        }

        /// <summary>
        /// Inserts a new option into a StatusAttributeMetadata attribute.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="entityLogicalName">The logical name of the entity containing the status attribute</param>
        /// <param name="label">The label of the value to add.</param>
        /// <param name="stateCode">The corresponding state code value.</param>
        /// <param name="value">The value  to add.</param>
        /// <param name="description">The description of the value to add.</param>
        /// <param name="solutionUniqueName">The unique name of the solution to add the optionset to</param>
        /// <returns>The optionset value.</returns>
        public static int InsertStatusValue(this CDSWebApiService svc,
            string entityLogicalName,
            Label label,
            int stateCode,
            int? value = null,
            Label description = null,
            string solutionUniqueName = null)
        {
            int NewOptionValue;

            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                //Verify that solution exists and is unmanaged.
                try
                {
                    svc.CheckSolution(solutionUniqueName);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            JObject args = new JObject
            {
                ["EntityLogicalName"] = entityLogicalName,
                ["AttributeLogicalName"] = "statuscode",
                ["Label"] = JObject.FromObject(label),
                ["StateCode"] = stateCode
            };
            if (value != null)
            {
                args["Value"] = value;
            }

            if (description != null)
            {
                args["Description"] = JObject.FromObject(description);
            }

            if (solutionUniqueName != null)
            {
                args["SolutionUniqueName"] = solutionUniqueName;
            }
            try
            {
                NewOptionValue = (int)svc.Post("InsertStatusValue", args)["NewOptionValue"];
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting local option:{ex.Message}");
            }
            return NewOptionValue;
        }

        /// <summary>
        /// Order the options of an optionset
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="values">The option values in the new order</param>
        /// <param name="optionSetName">The name of the global option set</param>
        /// <param name="entityLogicalName">The logical name of the entity containing the attribute</param>
        /// <param name="attributeLogicalName">The logical name of the attribute containing the local optionset.</param>
        /// <param name="solutionUniqueName">The unique name of the solution to add the optionset to</param>
        public static void OrderOption(this CDSWebApiService svc,
            int[] values,
            string optionSetName = null,
            string entityLogicalName = null,
            string attributeLogicalName = null,
            string solutionUniqueName = null)
        {
            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                //Verify that solution exists and is unmanaged.
                try
                {
                    svc.CheckSolution(solutionUniqueName);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            if (string.IsNullOrEmpty(optionSetName))
            {
                if (string.IsNullOrEmpty(entityLogicalName) || string.IsNullOrEmpty(attributeLogicalName))
                {
                    throw new Exception("The parameters entityLogicalName and attributeLogicalName are required when the parameter optionSetName is null.");
                }
            }

            JObject args = new JObject()
            {
                ["Values"] = JToken.FromObject(values)
            };
            if (!string.IsNullOrEmpty(optionSetName))
            {
                args["OptionSetName"] = optionSetName;
            }
            if (!string.IsNullOrEmpty(entityLogicalName))
            {
                args["EntityLogicalName"] = entityLogicalName;
            }
            if (!string.IsNullOrEmpty(attributeLogicalName))
            {
                args["AttributeLogicalName"] = attributeLogicalName;
            }
            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                args["SolutionUniqueName"] = solutionUniqueName;
            }

            try
            {
                svc.Post("OrderOption", args);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error using OrderOption:{ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all the global option sets
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <returns>A list of all the global option set definitions</returns>
        public static List<OptionSetMetadata> RetrieveAllGlobalOptionSets(this CDSWebApiService svc)
        {
            try
            {
                var results = svc.Get("GlobalOptionSetDefinitions", strongConsistencyHeader)["value"];
                return JsonConvert.DeserializeObject<List<OptionSetMetadata>>(results.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in RetrieveAllGlobalOptionSets:{ex.Message}", ex);
            }
        }

        /// <summary>
        /// Retrieves an attribute using id values
        /// </summary>
        /// <typeparam name="T">The type of the attribute derived from AttributeMetadata</typeparam>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="entityMetadataId">The MetadataId of the entity that contains the attribute.</param>
        /// <param name="metadataid">The MetadataId of the attribute.</param>
        /// <param name="properties">The Properties of the attribute to include.</param>
        /// <returns>The AttributeMetadata of the attribute</returns>
        public static T RetrieveAttribute<T>(this CDSWebApiService svc,
            Guid entityMetadataId,
            Guid metadataid,
            params string[] properties
            ) where T : AttributeMetadata
        {
            return RetrieveAttribute<T>(svc, null, entityMetadataId, null, metadataid, properties);
        }

        /// <summary>
        /// Retrieves an attribute using logical names
        /// </summary>
        /// <typeparam name="T">The type of the attribute derived from AttributeMetadata</typeparam>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="entityLogicalName">The logical name of the entity that contains the attribute.</param>
        /// <param name="logicalName">The logical name of the attribute.</param>
        /// <param name="properties">The Properties of the attribute to include.</param>
        /// <returns>The AttributeMetadata of the attribute</returns>
        public static T RetrieveAttribute<T>(this CDSWebApiService svc,
         string entityLogicalName,
         string logicalName,
         params string[] properties
         ) where T : AttributeMetadata
        {
            return RetrieveAttribute<T>(svc, entityLogicalName, null, logicalName, null, properties);
        }


        /// <summary>
        /// Retrieves data for a specific entity by Id
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="entityFilters">Controls which related data to return</param>
        /// <param name="metadataId">The Metadata Id of the entity to return</param>
        /// <param name="properties">The Properties of the entity to return</param>
        /// <returns>The data for an entity</returns>
        public static EntityMetadata RetrieveEntity(this CDSWebApiService svc,
            EntityFilters entityFilters,
            Guid metadataId,
            params string[] properties
            )
        {
            return RetrieveEntity(svc: svc,
                entityFilters: entityFilters,
                logicalName: null,
                metadataId: metadataId,
                properties: properties);
        }

        /// <summary>
        /// Retrieves data for a specific entity by name
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="entityFilters">Controls which related data to return</param>
        /// <param name="logicalName">The LogicalName of the entity to return</param>
        /// <param name="properties">The Properties of the entity to return</param>
        /// <returns>The data for an entity</returns>
        public static EntityMetadata RetrieveEntity(this CDSWebApiService svc,
            EntityFilters entityFilters,
            string logicalName,
            params string[] properties)
        {
            return RetrieveEntity(svc: svc,
                entityFilters: entityFilters,
                logicalName: logicalName,
                metadataId: null,
                properties: properties);
        }

        /// <summary>
        /// Retrieves a Global Optionset by Id
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="name">The name of the optionset to retrieve.</param>
        /// <returns>The data for a global optionset</returns>
        public static OptionSetMetadata RetrieveGlobalOptionSet(this CDSWebApiService svc,
            Guid optionSetId)
        {
            return RetrieveGlobalOptionSet(svc, optionSetId, null);
        }

        /// <summary>
        /// Retrieves a Global Optionset by name
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="name">The name of the optionset to retrieve.</param>
        /// <returns>The data for a global optionset</returns>
        public static OptionSetMetadata RetrieveGlobalOptionSet(this CDSWebApiService svc,
            string name)
        {
            return RetrieveGlobalOptionSet(svc, null, name);
        }

        /// <summary>
        /// Retrieves the OptionSet of an attribute.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="entityLogicalName">The logical name of the entity.</param>
        /// <param name="attributeLogicalName">The logical name of the attribute.</param>
        /// <returns>The OptionSet of an attribute.</returns>
        public static OptionSetMetadata RetrieveLocalOptionSet(this CDSWebApiService svc,
            string entityLogicalName,
            string attributeLogicalName)
        {
            JToken attribute = null;
            string className = string.Empty;
            JToken typedAttribute = null;

            try
            {
                attribute = svc.Get($"EntityDefinitions(LogicalName='{entityLogicalName}')" +
                                            $"/Attributes(LogicalName='{attributeLogicalName}')" +
                                            $"?$select=AttributeTypeName", strongConsistencyHeader);
            }
            catch (ServiceException se)
            {
                throw new Exception($"Service Error retrieving {entityLogicalName} {attributeLogicalName} attribute:{se.Message}", se);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving {entityLogicalName} {attributeLogicalName} attribute:{ex.Message}", ex);
            }

            string typeName = (string)attribute["AttributeTypeName"]["Value"];

            switch (typeName)
            {
                case "MultiSelectPicklistType":
                    className = "MultiSelectPicklistAttributeMetadata";
                    break;

                case "PicklistType":
                    className = "PicklistAttributeMetadata";
                    break;

                case "StateType":
                    className = "StateAttributeMetadata";
                    break;

                case "StatusType":
                    className = "StatusAttributeMetadata";
                    break;

                default:
                    throw new Exception($"The {entityLogicalName} {attributeLogicalName} attribute is a {typeName} attribute. It doesn't have an OptionSetMetadata optionset.");
            }

            try
            {
                typedAttribute = svc.Get($"EntityDefinitions(LogicalName='{entityLogicalName}')" +
                                            $"/Attributes(LogicalName='{attributeLogicalName}')" +
                                            $"/Microsoft.Dynamics.CRM.{className}" +
                                            $"?$select=MetadataId&$expand=OptionSet", strongConsistencyHeader);
            }
            catch (ServiceException se)
            {
                throw new Exception($"Service Error retrieving typed attribute:{se.Message}", se);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving typed attribute:{ex.Message}", ex);
            }

            var optionsetbase = typedAttribute["OptionSet"].ToString();

            var optionSet = JsonConvert.DeserializeObject<OptionSetMetadata>(optionsetbase);

            //Replace the ordinary OptionMetadata with special State and Status types
            if (typeName == "StateType")
            {
                var stateOptions = JsonConvert.DeserializeObject<List<StateOptionMetadata>>(typedAttribute["OptionSet"]["Options"].ToString());
                optionSet.Options.Clear();

                stateOptions.ForEach(x =>
                {
                    optionSet.Options.Add(x);
                });
            }
            if (typeName == "StatusType")
            {
                var statusOptions = JsonConvert.DeserializeObject<List<StatusOptionMetadata>>(typedAttribute["OptionSet"]["Options"].ToString());
                optionSet.Options.Clear();
                statusOptions.ForEach(x =>
                {
                    optionSet.Options.Add(x);
                });
            }

            return optionSet;
        }

        /// <summary>
        /// Retrieve entity relationship metadata by Id.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="metadataId">The MetadataId of the RelationshipMetadataBase to be retrieved. Required.</param>
        /// <returns>The entity relationship metadata.</returns>
        public static RelationshipMetadataBase RetrieveRelationship(this CDSWebApiService svc,
        Guid metadataId)
        {
            return RetrieveRelationship(svc: svc, metadataId: metadataId, schemaName: null);
        }

        /// <summary>
        /// Retrieve entity relationship metadata by Name
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="schemaName">The Schema name for the entity relationship to be retrieved. Required.</param>
        /// <returns>The entity relationship metadata.</returns>
        public static RelationshipMetadataBase RetrieveRelationship(this CDSWebApiService svc,
        string schemaName)
        {
            return RetrieveRelationship(svc: svc, schemaName: schemaName, metadataId: null);
        }

        /// <summary>
        /// Updates an attribute
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="entityLogicalName">The logical name of the entity that contains the attribute</param>
        /// <param name="attributeMetadata">The modified attribute to update.</param>
        /// <param name="mergeLabels">Whether to merge the new labels with any existing labels. Required.</param>
        /// <param name="solutionUniqueName">The unique name of the unmanaged solution to add the attribute change to.</param>
        public static void UpdateAttribute(this CDSWebApiService svc,
            string entityLogicalName,
            AttributeMetadata attributeMetadata,
            bool mergeLabels,
            string solutionUniqueName = null)
        {
            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                //Verify that solution exists and is unmanaged.
                try
                {
                    svc.CheckSolution(solutionUniqueName);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            try
            {
                svc.Put(path: $"EntityDefinitions(LogicalName='{entityLogicalName}')/Attributes({attributeMetadata.MetadataId})", JObject.FromObject(attributeMetadata), mergeLabels);

            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating attribute: {ex.Message}", ex);
            }

            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                try
                {
                    svc.AddSolutionComponent((Guid)attributeMetadata.MetadataId, SolutionComponentType.Attribute, solutionUniqueName, true);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error adding the attribute to a solution: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Updates an entity
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="entityMetadata">The entity to update.</param>
        /// <param name="mergeLabels">Whether to merge the new labels with any existing labels. Required.</param>
        /// <param name="solutionUniqueName">The unique name of the unmanaged solution to add the entity change to.</param>
        public static void UpdateEntity(this CDSWebApiService svc,
            EntityMetadata entityMetadata,
            bool mergeLabels,
            string solutionUniqueName = null)
        {
            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                //Verify that solution exists and is unmanaged.
                try
                {
                    svc.CheckSolution(solutionUniqueName);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            try
            {
                svc.Put(path: $"EntityDefinitions({entityMetadata.MetadataId})", JObject.FromObject(entityMetadata), mergeLabels);


            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating entity: {ex.Message}", ex);
            }

            if (!string.IsNullOrEmpty(solutionUniqueName))
            {
                try
                {
                    svc.AddSolutionComponent((Guid)entityMetadata.MetadataId, SolutionComponentType.Entity, solutionUniqueName, true);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error adding the entity to a solution: {ex.Message}");
                }
            }
        }


        /// <summary>
        /// Updates an option value in a global option set.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="optionSetName">The name of the global option set.</param>
        /// <param name="value">The value for the option.</param>
        /// <param name="mergeLabels">Indicates whether to keep text defined for languages not included in the Label.</param>
        /// <param name="label">The label for the option.</param>
        /// <param name="description">Description for the option.</param>
        /// <param name="parentValues">For internal use only.</param>
        /// <param name="solutionUniqueName">The name of the unmanaged solution that this global option set should be associated with.</param>
        public static void UpdateGlobalOptionValue(this CDSWebApiService svc,
            string optionSetName,
            int value,
            bool mergeLabels,
            Label label = null,
            Label description = null,
            int[] parentValues = null,
            string solutionUniqueName = null)
        {
            try
            {
                svc.UpdateOptionValue(
                optionSetName: optionSetName,
                value: value,
                mergeLabels: mergeLabels,
                label: label,
                description: description,
                parentValues: parentValues,
                solutionUniqueName: solutionUniqueName);
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Updates an option value in a local option set.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="entityLogicalName">The logical name of the entity when updating the local option set in a picklist attribute.</param>
        /// <param name="attributeLogicalName">The logical name of the attribute when updating a local option set in a picklist attribute.</param>
        /// <param name="value">The value for the option.</param>
        /// <param name="mergeLabels">Indicates whether to keep text defined for languages not included in the Label.</param>
        /// <param name="label">The label for the option.</param>
        /// <param name="description">Description for the option.</param>
        /// <param name="parentValues">For internal use only.</param>
        /// <param name="solutionUniqueName">The name of the unmanaged solution that this global option set should be associated with.</param>
        public static void UpdateLocalOptionValue(this CDSWebApiService svc,
            string entityLogicalName,
            string attributeLogicalName,
            int value,
            bool mergeLabels,
            Label label = null,
            Label description = null,
            int[] parentValues = null,
            string solutionUniqueName = null)
        {
            try
            {
                svc.UpdateOptionValue(
                value: value,
                mergeLabels: mergeLabels,
                entityLogicalName: entityLogicalName,
                attributeLogicalName: attributeLogicalName,
                label: label,
                description: description,
                parentValues: parentValues,
                solutionUniqueName: solutionUniqueName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Updates an entity relationship
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="relationship">The relationship metadata to be updated. Required.</param>
        /// <param name="mergeLabels">Whether to merge the new labels with any existing labels. Required.</param>
        public static void UpdateRelationship(this CDSWebApiService svc,
            RelationshipMetadataBase relationship,
            bool mergeLabels)
        {
            try
            {
                svc.Put(path: $"RelationshipDefinitions({relationship.MetadataId})", JObject.FromObject(relationship), mergeLabels);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating relationship: {ex.Message}", ex);
            }
        }
        /// <summary>
        /// Updates an option set value in for a StateAttributeMetadata attribute.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="value">The value for the option.</param>
        /// <param name="mergeLabels">Indicates whether to keep text defined for languages not included in the Label.</param>
        /// <param name="optionSetName">The name of the global option set.</param>
        /// <param name="entityLogicalName">The logical name of the entity when updating the local option set in a picklist attribute.</param>
        /// <param name="attributeLogicalName">The logical name of the attribute when updating a local option set in a picklist attribute.</param>
        /// <param name="label">The label for the option.</param>
        /// <param name="description">Description for the option.</param>
        /// <param name="defaultStatusCode">The default value for the statuscode (status reason) attribute when this statecode value is set.</param>
        public static void UpdateStateValue(this CDSWebApiService svc,
            int value,
            bool mergeLabels,
            string optionSetName = null,
            string entityLogicalName = null,
            string attributeLogicalName = null,
            Label label = null,
            Label description = null,
            int? defaultStatusCode = null)
        {
            JObject args = new JObject
            {
                ["Value"] = value,
                ["MergeLabels"] = mergeLabels
            };
            if (!string.IsNullOrEmpty("optionSetName"))
            {
                args["OptionSetName"] = optionSetName;
            }
            if (!string.IsNullOrEmpty("entityLogicalName"))
            {
                args["EntityLogicalName"] = entityLogicalName;
            }
            if (!string.IsNullOrEmpty("attributeLogicalName"))
            {
                args["AttributeLogicalName"] = attributeLogicalName;
            }
            if (label != null)
            {
                args["Label"] = JObject.FromObject(label);
            }
            if (description != null)
            {
                args["Description"] = JObject.FromObject(description);
            }
            if (defaultStatusCode.HasValue)
            {
                args["DefaultStatusCode"] = defaultStatusCode.Value;
            }

            try
            {
                svc.Post("UpdateStateValue", args);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in UpdateStateValue: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Checks whether a solution exists and is unmanaged;
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="solutionUniqueName">The unique name of the solution to check.</param>
        /// <returns>The solutionid</returns>
        private static Guid CheckSolution(this CDSWebApiService svc,
            string solutionUniqueName)
        {
            Guid solutionId;
            //Verify that solution exists and is unmanaged.
            try
            {
                JObject solutionQueryResults = (JObject)svc.Get($"solutions?$select=solutionid,ismanaged&$filter=uniquename eq '{solutionUniqueName}'", strongConsistencyHeader);

                JObject solution = (JObject)solutionQueryResults["value"][0];

                if (((bool)solution["ismanaged"]) == false)
                {
                    solutionId = new Guid(solution.GetValue("solutionid").ToString());
                }
                else
                {
                    throw new Exception($"The solution {solutionUniqueName} is a managed solution.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"A solution named '{solutionUniqueName}' could not be found. Error: {ex.Message}");
            }
            return solutionId;
        }

        /// <summary>
        /// Deletes an entity
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="entityMetadataId">The MetadataId of the entity to delete.</param>
        /// <param name="entityLogicalName">The logical Name of the entity to delete</param>
        private static void DeleteEntity(this CDSWebApiService svc,
            Guid? entityMetadataId = null,
            string entityLogicalName = null)
        {
            string entityKey;
            if (entityMetadataId == null && string.IsNullOrEmpty(entityLogicalName))
            {
                throw new Exception("DeleteEntity requires either entityMetadataId or entityLogicalName parameters.");
            }
            else
            {
                if (entityMetadataId != null)
                {
                    entityKey = entityMetadataId.ToString();
                }
                else
                {
                    entityKey = $"LogicalName='{entityLogicalName}'";
                }
            }

            try
            {
                svc.Delete($"EntityDefinitions({entityKey})", strongConsistencyHeader);
            }
            catch (ServiceException se)
            {
                throw new Exception($"Service Error deleteing the entity:{se.Message}", se);
            }
        }

        /// <summary>
        /// Deletes a global optionset.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="optionSetId">The Id of the optionset.</param>
        /// <param name="name">The name of the optionset.</param>
        private static void DeleteGlobalOptionSet(this CDSWebApiService svc,
            Guid? optionSetId = null, string name = null)
        {
            string key;
            if (optionSetId == null && string.IsNullOrEmpty(name))
            {
                throw new Exception("DeleteGlobalOptionSet requires either optionSetId or name parameters.");
            }
            else
            {
                if (optionSetId != null)
                {
                    key = optionSetId.ToString();
                }
                else
                {
                    key = $"Name='{name}'";
                }
            }

            try
            {
                svc.Delete($"GlobalOptionSetDefinitions({key})", strongConsistencyHeader);
            }
            catch (ServiceException se)
            {
                throw new Exception($"Service Error deleting the global option set:{se.Message}", se);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting the global option set:{ex.InnerException.Message}", ex);
            }
        }

        /// <summary>
        /// Deletes an entity relationship
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="relationshipId">The MetadataId of the relationship.</param>
        /// <param name="relationshipSchemaName">The SchemaName of the relationship.</param>
        private static void DeleteRelationship(this CDSWebApiService svc,
            Guid? relationshipId = null,
            string relationshipSchemaName = null)
        {
            if (relationshipId == null && string.IsNullOrEmpty(relationshipSchemaName))
            {
                throw new Exception("RetrieveEntity requires either relationshipSchemaName or relationshipId parameters.");
            }
            //Get the key to use
            string key;
            if (relationshipId != null)
            {
                key = relationshipId.ToString();
            }
            else
            {
                key = $"SchemaName='{relationshipSchemaName}'";
            }
            try
            {
                svc.Delete($"RelationshipDefinitions({key})", strongConsistencyHeader);
            }
            catch (Exception)
            {
                throw;
            }
        }



        /// <summary>
        /// Retrieves an attribute using either id or logical name values
        /// </summary>
        /// <typeparam name="T">The type of the attribute derived from AttributeMetadata</typeparam>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="entityLogicalName">The logical name of the entity that contains the attribute.</param>
        /// <param name="entityMetadataId">The MetadataId of the entity that contains the attribute.</param>
        /// <param name="logicalName">The logical name of the attribute.</param>
        /// <param name="metadataId">The MetadataId of the attribute.</param>
        /// <param name="properties">The Properties of the attribute to include.</param>
        /// <returns>The AttributeMetadata of the attribute</returns>
        private static T RetrieveAttribute<T>(this CDSWebApiService svc,
            string entityLogicalName = null,
            Guid? entityMetadataId = null,
            string logicalName = null,
            Guid? metadataId = null,
            params string[] properties
            ) where T : AttributeMetadata
        {
            string typeName = typeof(T).Name;
            //Get the entityKey to use
            string entityKey;
            if (entityMetadataId == null && string.IsNullOrEmpty(entityLogicalName))
            {
                throw new Exception("RetrieveAttribute requires either EntityLogicalName or EntityMetadataId parameters.");
            }
            else
            {
                if (entityMetadataId != null)
                {
                    entityKey = entityMetadataId.ToString();
                }
                else
                {
                    entityKey = $"LogicalName='{entityLogicalName}'";
                }
            }

            //Get the attributeKey to use
            string attributeKey;
            if (metadataId == null && string.IsNullOrEmpty(logicalName))
            {
                throw new Exception("RetrieveAttribute requires either LogicalName or MetadataId parameters.");
            }
            else
            {
                if (metadataId != null)
                {
                    attributeKey = metadataId.ToString();
                }
                else
                {
                    attributeKey = $"LogicalName='{logicalName}'";
                }
            }

            string path = $"EntityDefinitions({entityKey})/Attributes({attributeKey})/Microsoft.Dynamics.CRM.{typeName}";

            List<string> parameters = new List<string>();

            //The specific entity properties to return
            if (properties.Length > 0)
            {
                parameters.Add($"$select={string.Join(",", properties)}");
            }

            switch (typeName)
            {
                case "EntityNameAttributeMetadata":
                case "MultiSelectPicklistAttributeMetadata":
                case "PicklistAttributeMetadata":
                case "StateAttributeMetadata":
                case "StatusAttributeMetadata":
                case "BooleanAttributeMetadata":
                    parameters.Add("$expand=OptionSet");
                    break;

                default:
                    break;
            }

            string arguments = string.Empty;

            if (parameters.Count > 0)
            {
                arguments = $"?{string.Join("&", parameters.ToArray())}";
            }

            JToken json = svc.Get($"{path}{arguments}", strongConsistencyHeader);
            return JsonConvert.DeserializeObject<T>(json.ToString());
        }

        /// <summary>
        /// Retrieves data for a specific entity
        /// </summary>
        /// <param name="svc"></param>
        /// <param name="entityFilters">Controls which related data to return</param>
        /// <param name="logicalName">The LogicalName of the entity to return</param>
        /// <param name="metadataId">The Metadata Id of the entity to return</param>
        /// <param name="properties">The Properties of the entity to return</param>
        /// <returns>The entity metadata</returns>
        private static EntityMetadata RetrieveEntity(this CDSWebApiService svc,
            EntityFilters entityFilters,
            string logicalName = null,
            Guid? metadataId = null,
            params string[] properties)
        {
            //Get the key to use
            string key;
            if (metadataId == null && string.IsNullOrEmpty(logicalName))
            {
                throw new Exception("RetrieveEntity requires either LogicalName or MetadataId parameters.");
            }
            else
            {
                if (metadataId != null)
                {
                    key = metadataId.ToString();
                }
                else
                {
                    key = $"LogicalName='{logicalName}'";
                }
            }

            //Parameters that will be passed
            List<string> parameters = new List<string>();

            //Get related data
            //There is probably a smarter way to achieve this...but it works.
            if (entityFilters.HasFlag(EntityFilters.Keys) ||
                entityFilters.HasFlag(EntityFilters.Attributes) ||
                entityFilters.HasFlag(EntityFilters.ManyToManyRelationships) ||
                entityFilters.HasFlag(EntityFilters.ManyToOneRelationships) ||
                entityFilters.HasFlag(EntityFilters.OneToManyRelationships) ||
                entityFilters.HasFlag(EntityFilters.AllRelationships) ||
                entityFilters.HasFlag(EntityFilters.All))
            {
                List<string> expands = new List<string>();
                if (entityFilters.HasFlag(EntityFilters.Keys) &
                    !entityFilters.HasFlag(EntityFilters.All))
                {
                    expands.Add("Keys");
                }
                if (entityFilters.HasFlag(EntityFilters.Attributes) &
                    !entityFilters.HasFlag(EntityFilters.All))
                {
                    expands.Add("Attributes");
                }
                if (entityFilters.HasFlag(EntityFilters.ManyToManyRelationships) &
                    !entityFilters.HasFlag(EntityFilters.All) &
                    !entityFilters.HasFlag(EntityFilters.AllRelationships))
                {
                    expands.Add("ManyToManyRelationships");
                }
                if (entityFilters.HasFlag(EntityFilters.ManyToOneRelationships) &
                    !entityFilters.HasFlag(EntityFilters.All) &
                    !entityFilters.HasFlag(EntityFilters.AllRelationships))
                {
                    expands.Add("ManyToOneRelationships");
                }
                if (entityFilters.HasFlag(EntityFilters.OneToManyRelationships) &
                    !entityFilters.HasFlag(EntityFilters.All) &
                    !entityFilters.HasFlag(EntityFilters.AllRelationships))
                {
                    expands.Add("OneToManyRelationships");
                }
                if (entityFilters.HasFlag(EntityFilters.AllRelationships) &
                    !entityFilters.HasFlag(EntityFilters.All))
                {
                    expands.Add("ManyToManyRelationships");
                    expands.Add("ManyToOneRelationships");
                    expands.Add("OneToManyRelationships");
                }
                if (entityFilters.HasFlag(EntityFilters.All))
                {
                    expands.Add("Keys");
                    expands.Add("Attributes");
                    expands.Add("ManyToManyRelationships");
                    expands.Add("ManyToOneRelationships");
                    expands.Add("OneToManyRelationships");
                }

                parameters.Add($"$expand={string.Join(",", expands.ToArray())}");
            }

            //The specific entity properties to return
            if (properties.Length > 0)
            {
                parameters.Add($"$select={string.Join(",", properties)}");
            }

            string arguments = string.Empty;

            if (parameters.Count > 0)
            {
                arguments = $"?{string.Join("&", parameters.ToArray())}";
            }

            JToken results;
            try
            {
                results = svc.Get($"EntityDefinitions({key}){arguments}", strongConsistencyHeader);
            }
            catch (ServiceException se)
            {
                throw new Exception($"Service Error: {se.Message}", se);
            }
            catch (Exception)
            {
                throw;
            }

            return JsonConvert.DeserializeObject<EntityMetadata>(results.ToString());
        }

        /// <summary>
        /// Retrieves a Global Optionset
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="optionSetId">The Id of the optionset to retrieve.</param>
        /// <param name="name">The name of the optionset to retrieve.</param>
        /// <returns>The Global OptionSet data</returns>
        private static OptionSetMetadata RetrieveGlobalOptionSet(this CDSWebApiService svc,
            Guid? optionSetId = null, string name = null)
        {
            string key;
            if (optionSetId == null && string.IsNullOrEmpty(name))
            {
                throw new Exception("RetrieveGlobalOptionSet requires either optionSetId or name parameters.");
            }
            else
            {
                if (optionSetId != null)
                {
                    key = optionSetId.ToString();
                }
                else
                {
                    key = $"Name='{name}'";
                }
            }
            JToken results;
            try
            {
                results = svc.Get($"GlobalOptionSetDefinitions({key})", strongConsistencyHeader);
            }
            catch (ServiceException se)
            {
                throw new Exception($"Service Error: {se.Message}", se);
            }
            catch (Exception)
            {
                throw;
            }
            return JsonConvert.DeserializeObject<OptionSetMetadata>(results.ToString());
        }

        /// <summary>
        /// Retrieve entity relationship metadata.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="metadataId">The MetadataId of the RelationshipMetadataBase to be retrieved. Optional.</param>
        /// <param name="schemaName">The Schema name for the entity relationship to be retrieved. Optional.</param>
        /// <returns>The entity relationship metadata.</returns>
        private static RelationshipMetadataBase RetrieveRelationship(this CDSWebApiService svc,
        Guid? metadataId = null,
        string schemaName = null)
        {
            if (metadataId == null && string.IsNullOrEmpty(schemaName))
            {
                throw new Exception("RetrieveRelationship requires either metadataId or name parameters.");
            }

            try
            {
                //Get the key to use
                string key = (metadataId != null) ? metadataId.ToString() : $"SchemaName='{schemaName}'";

                var results = svc.Get($"RelationshipDefinitions({key})", strongConsistencyHeader);
                var type = (string)results["RelationshipType"];

                switch (type)
                {
                    case "ManyToManyRelationship":
                        return JsonConvert.DeserializeObject<ManyToManyRelationshipMetadata>(results.ToString());

                    default: //OneToManyRelationship
                        return JsonConvert.DeserializeObject<OneToManyRelationshipMetadata>(results.ToString());
                }
            }
            catch (ServiceException se)
            {
                throw new Exception($"Service Error: {se.Message}", se);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Updates an option value in a global or local option set.
        /// </summary>
        /// <param name="svc">An instance of the CDSWebApiService to extend.</param>
        /// <param name="value">The value for the option.</param>
        /// <param name="mergeLabels">Indicates whether to keep text defined for languages not included in the Label.</param>
        /// <param name="optionSetName">The name of the global option set.</param>
        /// <param name="entityLogicalName">The logical name of the entity when updating the local option set in a picklist attribute.</param>
        /// <param name="attributeLogicalName">The logical name of the attribute when updating a local option set in a picklist attribute.</param>
        /// <param name="label">The label for the option.</param>
        /// <param name="description">Description for the option.</param>
        /// <param name="parentValues">For internal use only.</param>
        /// <param name="solutionUniqueName">The name of the unmanaged solution that this global option set should be associated with.</param>
        private static void UpdateOptionValue(this CDSWebApiService svc,
            int value,
            bool mergeLabels,
            string optionSetName = null,
            string entityLogicalName = null,
            string attributeLogicalName = null,
            Label label = null,
            Label description = null,
            int[] parentValues = null,
            string solutionUniqueName = null)
        {
            JObject args = new JObject
            {
                ["Value"] = value,
                ["MergeLabels"] = mergeLabels
            };
            if (!string.IsNullOrEmpty("optionSetName"))
            {
                args["OptionSetName"] = optionSetName;
            }
            if (!string.IsNullOrEmpty("entityLogicalName"))
            {
                args["EntityLogicalName"] = entityLogicalName;
            }
            if (!string.IsNullOrEmpty("attributeLogicalName"))
            {
                args["AttributeLogicalName"] = attributeLogicalName;
            }
            if (label != null)
            {
                args["Label"] = JObject.FromObject(label);
            }
            if (description != null)
            {
                args["Description"] = JObject.FromObject(description);
            }
            if (parentValues != null)
            {
                args["ParentValues"] = JObject.FromObject(parentValues);
            }
            if (!string.IsNullOrEmpty("solutionUniqueName"))
            {
                args["SolutionUniqueName"] = solutionUniqueName;

                try
                {
                    svc.CheckSolution(solutionUniqueName);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            try
            {
                svc.Post("UpdateOptionValue", args);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in UpdateOptionValue: {ex.Message}", ex);
            }
        }
    }
}