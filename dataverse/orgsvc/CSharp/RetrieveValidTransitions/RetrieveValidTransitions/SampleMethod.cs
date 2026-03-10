using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PowerApps.Samples
{
   public partial class SampleProgram
    {
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

            
        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            
        }
        /// <summary>
        /// Returns valid status option transitions regardless of whether state transitions are enabled for the entity
        /// </summary>
        /// <param name="entityLogicalName">The logical name of the entity</param>
        /// <param name="currentStatusValue">The current status of the entity instance</param>
        /// <returns>A list of StatusOptions that represent the valid transitions</returns>
        public static List<StatusOption> GetValidStatusOptions(CrmServiceClient service, String entityLogicalName, int currentStatusValue)
        {

            List<StatusOption> validStatusOptions = new List<StatusOption>();

            //Check entity Metadata

            //Retrieve just one entity definition
            MetadataFilterExpression entityFilter = new MetadataFilterExpression(LogicalOperator.And);
            entityFilter.Conditions.Add(new MetadataConditionExpression("LogicalName", MetadataConditionOperator.Equals, entityLogicalName));
            //Return the attributes and the EnforceStateTransitions property
            MetadataPropertiesExpression entityProperties = new MetadataPropertiesExpression(new string[] { "Attributes", "EnforceStateTransitions" });

            //Retrieve only State or Status attributes
            MetadataFilterExpression attributeFilter = new MetadataFilterExpression(LogicalOperator.Or);
            attributeFilter.Conditions.Add(new MetadataConditionExpression("AttributeType", MetadataConditionOperator.Equals, AttributeTypeCode.Status));
            attributeFilter.Conditions.Add(new MetadataConditionExpression("AttributeType", MetadataConditionOperator.Equals, AttributeTypeCode.State));

            //Retrieve only the OptionSet property of the attributes
            MetadataPropertiesExpression attributeProperties = new MetadataPropertiesExpression(new string[] { "OptionSet" });

            //Set the query
            EntityQueryExpression query = new EntityQueryExpression()
            {
                Criteria = entityFilter,
                Properties = entityProperties,
                AttributeQuery = new AttributeQueryExpression() { Criteria = attributeFilter, Properties = attributeProperties }
            };

            //Retrieve the metadata
            RetrieveMetadataChangesRequest request = new RetrieveMetadataChangesRequest() { Query = query };
            RetrieveMetadataChangesResponse response = (RetrieveMetadataChangesResponse)service.Execute(request);

            //Check the value of EnforceStateTransitions
            Boolean? EnforceStateTransitions = response.EntityMetadata[0].EnforceStateTransitions;

            //Capture the state and status attributes
            StatusAttributeMetadata statusAttribute = new StatusAttributeMetadata();
            StateAttributeMetadata stateAttribute = new StateAttributeMetadata();

            foreach (AttributeMetadata attributeMetadata in response.EntityMetadata[0].Attributes)
            {
                switch (attributeMetadata.AttributeType)
                {
                    case AttributeTypeCode.Status:
                        statusAttribute = (StatusAttributeMetadata)attributeMetadata;
                        break;
                    case AttributeTypeCode.State:
                        stateAttribute = (StateAttributeMetadata)attributeMetadata;
                        break;
                }
            }


            if (EnforceStateTransitions.HasValue && EnforceStateTransitions.Value == true)
            {
                //When EnforceStateTransitions is true use the TransitionData to filter the valid options
                foreach (StatusOptionMetadata option in statusAttribute.OptionSet.Options)
                {
                    if (option.Value == currentStatusValue)
                    {
                        if (option.TransitionData != String.Empty)
                        {
                            XDocument transitionData = XDocument.Parse(option.TransitionData);

                            IEnumerable<XElement> elements = (((XElement)transitionData.FirstNode)).Descendants();

                            foreach (XElement e in elements)
                            {
                                int statusOptionValue = Convert.ToInt32(e.Attribute("tostatusid").Value);
                                String statusLabel = GetOptionSetLabel(service,statusAttribute, statusOptionValue);

                                string stateLabel = String.Empty;
                                int? stateValue = null;
                                foreach (StatusOptionMetadata statusOption in statusAttribute.OptionSet.Options)
                                {
                                    if (statusOption.Value.Value == statusOptionValue)
                                    {
                                        stateValue = statusOption.State.Value;
                                        stateLabel = GetOptionSetLabel(service, stateAttribute, stateValue.Value);
                                        validStatusOptions.Add(new StatusOption()
                                        {
                                            StateLabel = stateLabel,
                                            StateValue = stateValue.Value,
                                            StatusLabel = statusLabel,
                                            StatusValue = statusOptionValue
                                        });
                                    }
                                }
                            }
                        }
                    }
                }

            }
            else
            {
                ////When EnforceStateTransitions is false do not filter the available options

                foreach (StatusOptionMetadata option in statusAttribute.OptionSet.Options)
                {
                    if (option.Value != currentStatusValue)
                    {

                        String statusLabel = "";
                        try
                        {
                            statusLabel = option.Label.UserLocalizedLabel.Label;
                        }
                        catch (Exception)
                        {
                            statusLabel = option.Label.LocalizedLabels[0].Label;
                        };

                        String stateLabel = GetOptionSetLabel(service,stateAttribute, option.State.Value);

                        validStatusOptions.Add(new StatusOption()
                        {
                            StateLabel = stateLabel,
                            StateValue = option.State.Value,
                            StatusLabel = statusLabel,
                            StatusValue = option.Value.Value

                        });
                    }
                }
            }
            return validStatusOptions;

        }

        /// <summary>
        /// Returns a string representing the label of an option in an optionset
        /// </summary>
        /// <param name="attribute">The metadata for an an attribute with options</param>
        /// <param name="value">The value of the option</param>
        /// <returns>The label for the option</returns>
        public static String GetOptionSetLabel(CrmServiceClient service, EnumAttributeMetadata attribute, int value)
        {
            String label = "";
            foreach (OptionMetadata option in attribute.OptionSet.Options)
            {
                if (option.Value.Value == value)
                {
                    try
                    {
                        label = option.Label.UserLocalizedLabel.Label;
                    }
                    catch (Exception)
                    {
                        label = option.Label.LocalizedLabels[0].Label;
                    };
                }
            }
            return label;
        }

    }
    public class StatusOption
    {
        public int StatusValue { get; set; }
        public String StatusLabel { get; set; }
        public int StateValue { get; set; }
        public String StateLabel { get; set; }
    }
}

    

