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
                    //////////////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate

                    String entityLogicalName = "email"; // Also try "incident"
                    // Retrieve status options for the Incident entity

                    //Retrieve just the incident entity and its attributes
                    MetadataFilterExpression entityFilter = new MetadataFilterExpression(LogicalOperator.And);
                    entityFilter.Conditions.Add(new MetadataConditionExpression("LogicalName", MetadataConditionOperator.Equals, entityLogicalName));
                    MetadataPropertiesExpression entityProperties = new MetadataPropertiesExpression(new string[] { "Attributes" });

                    //Retrieve just the status attribute and the OptionSet property
                    MetadataFilterExpression attributeFilter = new MetadataFilterExpression(LogicalOperator.And);
                    attributeFilter.Conditions.Add(new MetadataConditionExpression("AttributeType", MetadataConditionOperator.Equals, AttributeTypeCode.Status));
                    MetadataPropertiesExpression attributeProperties = new MetadataPropertiesExpression(new string[] { "OptionSet" });

                    //Instantiate the entity query
                    EntityQueryExpression query = new EntityQueryExpression()
                    {
                        Criteria = entityFilter,
                        Properties = entityProperties,
                        AttributeQuery = new AttributeQueryExpression() { Criteria = attributeFilter, Properties = attributeProperties }
                    };

                    //Retrieve the metadata
                    RetrieveMetadataChangesRequest request = new RetrieveMetadataChangesRequest() { Query = query };
                    RetrieveMetadataChangesResponse response = (RetrieveMetadataChangesResponse)service.Execute(request);


                    StatusAttributeMetadata statusAttribute = (StatusAttributeMetadata)response.EntityMetadata[0].Attributes[0];
                    OptionMetadataCollection statusOptions = statusAttribute.OptionSet.Options;
                    //Loop through each of the status options
                    foreach (StatusOptionMetadata option in statusOptions)
                    {
                        String StatusOptionLabel = GetOptionSetLabel(service, statusAttribute, option.Value.Value);
                        Console.WriteLine("[{0}] {1} records can transition to:", StatusOptionLabel, entityLogicalName);
                        List<StatusOption> validStatusOptions = GetValidStatusOptions(service, entityLogicalName, option.Value.Value);
                        //Loop through each valid transition for the option
                        foreach (StatusOption opt in validStatusOptions)
                        {
                            Console.WriteLine("{0,-3}{1,-10}{2,-5}{3,-10}", opt.StateValue, opt.StateLabel, opt.StatusValue, opt.StatusLabel);
                        }
                        Console.WriteLine("");
                    }
                }

                #endregion Demonstrate
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
            #endregion Sample Code
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
