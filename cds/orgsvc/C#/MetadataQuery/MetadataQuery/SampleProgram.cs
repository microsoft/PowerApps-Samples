using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {

        [STAThread] // Added to support UX
        static void Main(string[] args)
        {
            CrmServiceClient service = null;

            try
            {
                service = SampleHelpers.Connect("Connect");
                if (service.IsReady)
                {
                    #region Sample Code
                    ////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate
                   
                    _userId = ((WhoAmIResponse)service.Execute(new WhoAmIRequest())).UserId;
                    _languageCode = RetrieveUserUILanguageCode(service, _userId);
              
                    // An array SchemaName values for non-intersect, user-owned entities that should not be returned.
                    String[] excludedEntities = {
"WorkflowLog",
"Template",
"CustomerOpportunityRole",
"Import",
"UserQueryVisualization",
"UserEntityInstanceData",
"ImportLog",
"RecurrenceRule",
"QuoteClose",
"UserForm",
"SharePointDocumentLocation",
"Queue",
"DuplicateRule",
"OpportunityClose",
"Workflow",
"RecurringAppointmentMaster",
"CustomerRelationship",
"Annotation",
"SharePointSite",
"ImportData",
"ImportFile",
"OrderClose",
"Contract",
"BulkOperation",
"CampaignResponse",
"Connection",
"Report",
"CampaignActivity",
"UserEntityUISettings",
"IncidentResolution",
"GoalRollupQuery",
"MailMergeTemplate",
"Campaign",
"PostFollow",
"ImportMap",
"Goal",
"AsyncOperation",
"ProcessSession",
"UserQuery",
"ActivityPointer",
"List",
"ServiceAppointment"};

                    //A filter expression to limit entities returned to non-intersect, user-owned entities not found in the list of excluded entities.
                    var EntityFilter = new MetadataFilterExpression(LogicalOperator.And);
                    EntityFilter.Conditions.Add(new MetadataConditionExpression("IsIntersect", MetadataConditionOperator.Equals, false));
                    EntityFilter.Conditions.Add(new MetadataConditionExpression("OwnershipType", MetadataConditionOperator.Equals, OwnershipTypes.UserOwned));
                    EntityFilter.Conditions.Add(new MetadataConditionExpression("SchemaName", MetadataConditionOperator.NotIn, excludedEntities));
                    var isVisibileInMobileTrue = new MetadataConditionExpression("IsVisibleInMobile", MetadataConditionOperator.Equals, true);
                    EntityFilter.Conditions.Add(isVisibileInMobileTrue);

                    //A properties expression to limit the properties to be included with entities
                    var EntityProperties = new MetadataPropertiesExpression()
                    {
                        AllProperties = false
                    };
                    EntityProperties.PropertyNames.AddRange(new string[] { "Attributes" });
                    
                    //A condition expresson to return optionset attributes
                    MetadataConditionExpression[] optionsetAttributeTypes = new MetadataConditionExpression[] {
     new MetadataConditionExpression("AttributeType", MetadataConditionOperator.Equals, AttributeTypeCode.Picklist),
     new MetadataConditionExpression("AttributeType", MetadataConditionOperator.Equals, AttributeTypeCode.State),
     new MetadataConditionExpression("AttributeType", MetadataConditionOperator.Equals, AttributeTypeCode.Status),
     new MetadataConditionExpression("AttributeType", MetadataConditionOperator.Equals, AttributeTypeCode.Boolean)
     };

                    //A filter expression to apply the optionsetAttributeTypes condition expression
                    var AttributeFilter = new MetadataFilterExpression(LogicalOperator.Or);
                    AttributeFilter.Conditions.AddRange(optionsetAttributeTypes);

                    //A Properties expression to limit the properties to be included with attributes
                    var AttributeProperties = new MetadataPropertiesExpression() { AllProperties = false };
                    AttributeProperties.PropertyNames.Add("OptionSet");
                    AttributeProperties.PropertyNames.Add("AttributeType");

                    //A label query expression to limit the labels returned to only those for the user's preferred language
                    var labelQuery = new LabelQueryExpression();
                    labelQuery.FilterLanguages.Add(_languageCode);

                    //An entity query expression to combine the filter expressions and property expressions for the query.
                    var entityQueryExpression = new EntityQueryExpression()
                    {

                        Criteria = EntityFilter,
                        Properties = EntityProperties,
                        AttributeQuery = new AttributeQueryExpression()
                        {
                            Criteria = AttributeFilter,
                            Properties = AttributeProperties
                        },
                        LabelQuery = labelQuery

                    };

                    //Retrieve the metadata for the query without a ClientVersionStamp
                    var initialRequest = getMetadataChanges(service, entityQueryExpression, null, DeletedMetadataFilters.OptionSet);
                   

                    //Add option labels to the cache and display the changes
                    addOptionLabelsToCache(initialRequest.EntityMetadata, false);
                    String ClientVersionStamp = initialRequest.ServerVersionStamp;
                    Console.WriteLine("{0} option labels for {1} entities added to the cache.",
                     _optionLabelList.Count,
                     initialRequest.EntityMetadata.Count);
                    Console.WriteLine("");
                    Console.WriteLine("ClientVersionStamp: {0}", ClientVersionStamp);
                    Console.WriteLine("");


                    //Add new custom entity with optionset
                    Console.WriteLine("Adding a custom entity named {0} with a custom optionset attribute named : {1}",
                     _customEntitySchemaName, _customAttributeSchemaName);
                    Console.WriteLine("");
                    addCustomEntityWithOptionSet(service);
                    //Publishing isn't necessary when adding a custom entity

                    //Add new option labels to the cache and display the results
                    ClientVersionStamp = updateOptionLabelList(service, entityQueryExpression, ClientVersionStamp);

                    Console.WriteLine("ClientVersionStamp: {0}", ClientVersionStamp);
                    Console.WriteLine("");

                    //Add a new option to the custom optionset in the custom entity and publish the custom entity
                    Console.WriteLine("Adding an additional option to the {0} attribute options.",
                     _customAttributeSchemaName);
                    Console.WriteLine("");
                    addOptionToCustomEntityOptionSet(service);
                    //It is necessary to publish updates to metadata. Create and Delete operations are published automatically.
                    publishUpdatedEntity(service);



                    //Add the new option label to the cache and display the results
                    ClientVersionStamp = updateOptionLabelList(service, entityQueryExpression, ClientVersionStamp);

                    Console.WriteLine("ClientVersionStamp: {0}", ClientVersionStamp);
                    Console.WriteLine("");

                    Console.WriteLine("");
                    Console.WriteLine("Current Options: {0}", _optionLabelList.Count.ToString());
                    Console.WriteLine("");

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up

                    //Retrieve metadata changes to remove option labels from deleted attributes and display the results
                    ClientVersionStamp = updateOptionLabelList(service, entityQueryExpression, ClientVersionStamp);

                    Console.WriteLine("ClientVersionStamp: {0}", ClientVersionStamp);
                    Console.WriteLine("");
                }
                #endregion Demonstrate
                #endregion Sample Code
                else
                {
                    const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Microsoft Dataverse";
                    if (service.LastCrmError.Equals(UNABLE_TO_LOGIN_ERROR))
                    {
                        Console.WriteLine("Check the connection string values in cds/App.config.");
                        throw new Exception(service.LastCrmError);
                    }
                    else
                    {
                        throw service.LastCrmException;
                    }
                }
            }

            catch (Exception ex)
            {
                SampleHelpers.HandleException(ex);
            }

            finally
            {
                if (service != null)
                    service.Dispose();

                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }
        }
    }
}
