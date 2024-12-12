using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        private static List<OptionSetOption> _optionLabelList = new List<OptionSetOption>();
        private static String _customEntitySchemaName = "sample_SampleEntityForMetadataQuery";
        private static String _customAttributeSchemaName = "sample_ExampleOptionSet";
        private static Guid _userId;
        private static int _languageCode;
        private static bool prompt = true;
        /// <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        /// 
        private static void SetUpSample(CrmServiceClient service)
        {
            // Check that the current version is greater than the minimum version
            if (!SampleHelpers.CheckVersion(service, new Version("7.1.0.0")))
            {
                //The environment version is lower than version 7.1.0.0
                return;
            }

            //CreateRequiredRecords(service);
        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, prompt);
        }

        /// <summary>
        /// Creates any entity records that this sample requires.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            
            //Creates required records for the sample.
        }

        protected static RetrieveMetadataChangesResponse getMetadataChanges(CrmServiceClient service, EntityQueryExpression entityQueryExpression, String clientVersionStamp, DeletedMetadataFilters deletedMetadataFilter)
        {
            var retrieveMetadataChangesRequest = new RetrieveMetadataChangesRequest()
            {
                Query = entityQueryExpression,
                ClientVersionStamp = clientVersionStamp,
                DeletedMetadataFilters = deletedMetadataFilter
            };

            return (RetrieveMetadataChangesResponse)service.Execute(retrieveMetadataChangesRequest);

        }
  
        protected static String updateOptionLabelList(CrmServiceClient service, EntityQueryExpression entityQueryExpression, String clientVersionStamp)
        {
            //Retrieve metadata changes and add them to the cache
            RetrieveMetadataChangesResponse updateResponse;
            try
            {
                updateResponse = getMetadataChanges(service, entityQueryExpression, clientVersionStamp, DeletedMetadataFilters.OptionSet);
                addOptionLabelsToCache(updateResponse.EntityMetadata, true);
                removeOptionLabelsFromCache(updateResponse.DeletedMetadata, true);

            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                // Check for ErrorCodes.ExpiredVersionStamp (0x80044352)
                // Will occur when the timestamp exceeds the Organization.ExpireSubscriptionsInDays value, which is 90 by default.
                if (ex.Detail.ErrorCode == unchecked((int)0x80044352))
                {
                    //reinitialize cache
                    _optionLabelList.Clear();

                    updateResponse = getMetadataChanges(service, entityQueryExpression, null, DeletedMetadataFilters.OptionSet);
                    //Add them to the cache and display the changes
                    addOptionLabelsToCache(updateResponse.EntityMetadata, true);

                }
                else
                {
                    throw ex;
                }

            }
            return updateResponse.ServerVersionStamp;
        }

        protected static void addOptionLabelsToCache(EntityMetadataCollection entityMetadataCollection, Boolean showChanges)
        {

            List<OptionSetOption> changes = new List<OptionSetOption>();

            foreach (EntityMetadata em in entityMetadataCollection)
            {
                foreach (AttributeMetadata am in em.Attributes)
                {
                    switch (am.AttributeType)
                    {
                        case AttributeTypeCode.Boolean:
                            var booleanAttribute = (BooleanAttributeMetadata)am;
                            //Labels will not be included if they aren't new
                            if (booleanAttribute.OptionSet.FalseOption.Label.UserLocalizedLabel != null)
                            {
                                changes.Add(new OptionSetOption(
                                (Guid)booleanAttribute.OptionSet.MetadataId,
                                0,
                                booleanAttribute.OptionSet.FalseOption.Label.UserLocalizedLabel.Label)
                                );
                            }
                            //Labels will not be included if they aren't new
                            if (booleanAttribute.OptionSet.TrueOption.Label.UserLocalizedLabel != null)
                            {
                                changes.Add(new OptionSetOption(
                                (Guid)booleanAttribute.OptionSet.MetadataId,
                                1,
                                booleanAttribute.OptionSet.TrueOption.Label.UserLocalizedLabel.Label));
                            }
                            break;
                        default:
                            var optionsetAttribute = (EnumAttributeMetadata)am;
                            foreach (OptionMetadata option in optionsetAttribute.OptionSet.Options)
                            {
                                //Labels will not be included if they aren't new
                                if (option.Label.UserLocalizedLabel != null)
                                {
                                    changes.Add(new OptionSetOption(
                                     (Guid)optionsetAttribute.OptionSet.MetadataId,
                                    (int)option.Value,
                                    option.Label.UserLocalizedLabel.Label));
                                }
                            }
                            break;
                    }
                }
            }

            _optionLabelList.AddRange(changes);

            if (showChanges)
            {

                if (changes.Count > 0)
                {
                    Console.WriteLine("{0} option labels for {1} entities were added to the cache.", changes.Count, entityMetadataCollection.Count);
                    Console.WriteLine("{0} Option Labels cached", _optionLabelList.Count);
                }
                else
                { Console.WriteLine("No option labels were added to the cache."); }

            }
        }
        protected static void removeOptionLabelsFromCache(DeletedMetadataCollection DeletedMetadata, Boolean showChanges)
        {
            List<OptionSetOption> optionSetOptionsToRemove = new List<OptionSetOption>();

            if (DeletedMetadata.Keys.Contains(DeletedMetadataFilters.OptionSet))
            {
                DataCollection<Guid> optionsetmetadataids = (DataCollection<Guid>)DeletedMetadata[DeletedMetadataFilters.OptionSet];
                foreach (Guid metadataid in optionsetmetadataids)
                {
                    foreach (OptionSetOption oso in _optionLabelList)
                    {
                        if (metadataid == oso.optionsetId)
                        {
                            optionSetOptionsToRemove.Add(oso);
                        }
                    }
                }
            }
            foreach (OptionSetOption option in optionSetOptionsToRemove)
            {
                _optionLabelList.Remove(option);
            }
            if (showChanges)
            {
                if (optionSetOptionsToRemove.Count > 0)
                {
                    Console.WriteLine("{0} Option Labels removed", optionSetOptionsToRemove.Count);
                    Console.WriteLine("{0} Total Option Labels currently cached", _optionLabelList.Count);
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine("No Option Labels removed.");
                    Console.WriteLine("");
                }

            }
        }     

        protected static void addCustomEntityWithOptionSet(CrmServiceClient service)
        {
            String primaryAttributeSchemaName = "sample_SampleEntityForMetadataQueryName";

            var createEntityRequest = new CreateEntityRequest
            {

                //Define the entity
                Entity = new EntityMetadata
                {
                    SchemaName = _customEntitySchemaName,
                    LogicalName = _customEntitySchemaName.ToLower(),
                    DisplayName = new Label("Entity for MetadataQuery Sample", _languageCode),
                    DisplayCollectionName = new Label("Entity for MetadataQuery Sample", _languageCode),
                    Description = new Label("An entity created for the MetadataQuery Sample", _languageCode),
                    OwnershipType = OwnershipTypes.UserOwned,
                    IsVisibleInMobile = new BooleanManagedProperty(true),
                    IsActivity = false,

                },

                // Define the primary attribute for the entity
                PrimaryAttribute = new StringAttributeMetadata
                {
                    SchemaName = primaryAttributeSchemaName,
                    LogicalName = primaryAttributeSchemaName.ToLower(),
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                    MaxLength = 100,
                    Format = StringFormat.Text,
                    DisplayName = new Label("Entity for MetadataQuery Sample Name", _languageCode),
                    Description = new Label("The primary attribute for the Bank Account entity.", _languageCode)
                }

            };
            service.Execute(createEntityRequest);


            //PublishXmlRequest publishXmlRequest = new PublishXmlRequest { ParameterXml = String.Format("<importexportxml><entities><entity>{0}</entity></entities></importexportxml>", _customEntitySchemaName.ToLower()) };
            //_service.Execute(publishXmlRequest);

            //Add an optionset attribute

            var createAttributeRequest = new CreateAttributeRequest
            {
                EntityName = _customEntitySchemaName.ToLower(),
                Attribute = new PicklistAttributeMetadata
                {
                    SchemaName = _customAttributeSchemaName,
                    DisplayName = new Label("Example OptionSet for MetadataQuery Sample", _languageCode),
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),

                    OptionSet = new OptionSetMetadata
                    {
                        IsGlobal = false,
                        OptionSetType = OptionSetType.Picklist,
                        Options =  {
       new OptionMetadata(new Label("First Option",_languageCode),null),
       new OptionMetadata(new Label("Second Option",_languageCode),null),
       new OptionMetadata(new Label("Third Option",_languageCode),null),
       new OptionMetadata(new Label("Fourth Option",_languageCode),null)
      }
                    }
                }
            };

            service.Execute(createAttributeRequest);

        }

        protected static void addOptionToCustomEntityOptionSet(CrmServiceClient service)
        {

            var insertOptionValueRequest =new InsertOptionValueRequest
             {
                 AttributeLogicalName = _customAttributeSchemaName.ToLower(),
                 EntityLogicalName = _customEntitySchemaName.ToLower(),
                 Label = new Label("Fifth Option", _languageCode)
             };

            service.Execute(insertOptionValueRequest);


        }
      
        protected static int RetrieveUserUILanguageCode(CrmServiceClient service, Guid userId)
        {
            var userSettingsQuery = new QueryExpression("usersettings");
            userSettingsQuery.ColumnSet.AddColumns("uilanguageid", "systemuserid");
            userSettingsQuery.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, userId);
            EntityCollection userSettings = service.RetrieveMultiple(userSettingsQuery);
            if (userSettings.Entities.Count > 0)
            {
                return (int)userSettings.Entities[0]["uilanguageid"];
            }
            return 0;
        }
     

        protected static void publishUpdatedEntity(CrmServiceClient service)
        {
            var publishXmlRequest = new PublishXmlRequest
            {
                ParameterXml = "<importexportxml><entities><entity>" + _customEntitySchemaName.ToLower() + "</entity></entities></importexportxml>"
            };
            service.Execute(publishXmlRequest);
        }

        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the records created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
            bool toBeDeleted = true;

            if (prompt)
            {
                // Ask the user if the created entities should be deleted.
                Console.Write("\nDo you want these entity records deleted? (y/n) [y]: ");
                String answer = Console.ReadLine();
                if (answer.StartsWith("y") ||
                    answer.StartsWith("Y") ||
                    answer == String.Empty)
                {
                    toBeDeleted = true;
                }
                else
                {
                    toBeDeleted = false;
                }
            }

            if (toBeDeleted)
            {
                var request = new DeleteEntityRequest()
                {
                    LogicalName = _customEntitySchemaName.ToLower(),
                };
                service.Execute(request);
                Console.WriteLine("Entity record(s) have been deleted.");
            }
        }

    }

    public class OptionSetOption
    {
        public OptionSetOption(Guid OptionsetId, int OptionValue, String Label)
        {
            this._optionsetId = OptionsetId;
            this._optionValue = OptionValue;
            this._label = Label;
        }

        private Guid _optionsetId;
        private int _optionValue;
        private String _label;

        public Guid optionsetId { get { return this._optionsetId; } }
        public int optionValue { get { return this._optionValue; } }
        public String lable { get { return this._label; } }
    }
}
