using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
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
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region  Demonstrate

                    // The custom prefix would typically be passed in as an argument or
                    // determined by the publisher of the custom solution.
                    String prefix = "new_";

                    String customEntityName = prefix + "sampleentity";

                    // Create the custom activity entity.
                    CreateEntityRequest request = new CreateEntityRequest
                    {
                        HasNotes = true,
                        HasActivities = false,
                        PrimaryAttribute = new StringAttributeMetadata
                        {
                            SchemaName = "Subject",
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                            MaxLength = 100,
                            DisplayName = new Microsoft.Xrm.Sdk.Label("Subject", 1033)
                        },
                        Entity = new EntityMetadata
                        {
                            IsActivity = true,
                            SchemaName = customEntityName,
                            DisplayName = new Microsoft.Xrm.Sdk.Label("Sample Entity", 1033),
                            DisplayCollectionName = new Microsoft.Xrm.Sdk.Label("Sample Entity", 1033),
                            OwnershipType = OwnershipTypes.UserOwned,
                            IsAvailableOffline = true,

                        }
                    };

                    service.Execute(request);

                    //Entity must be published

                    // Add few attributes to the custom activity entity.
                    CreateAttributeRequest fontFamilyAttributeRequest =
                        new CreateAttributeRequest
                        {
                            EntityName = customEntityName,
                            Attribute = new StringAttributeMetadata
                            {
                                SchemaName = prefix + "fontfamily",
                                DisplayName = new Microsoft.Xrm.Sdk.Label("Font Family", 1033),
                                MaxLength = 100
                            }
                        };
                    CreateAttributeResponse fontFamilyAttributeResponse =
                        (CreateAttributeResponse)service.Execute(
                        fontFamilyAttributeRequest);

                    CreateAttributeRequest fontColorAttributeRequest =
                        new CreateAttributeRequest
                        {
                            EntityName = customEntityName,
                            Attribute = new StringAttributeMetadata
                            {
                                SchemaName = prefix + "fontcolor",
                                DisplayName = new Microsoft.Xrm.Sdk.Label("Font Color", 1033),
                                MaxLength = 50
                            }
                        };
                    CreateAttributeResponse fontColorAttributeResponse =
                        (CreateAttributeResponse)service.Execute(
                        fontColorAttributeRequest);

                    CreateAttributeRequest fontSizeAttributeRequest =
                        new CreateAttributeRequest
                        {
                            EntityName = customEntityName,
                            Attribute = new IntegerAttributeMetadata
                            {
                                SchemaName = prefix + "fontSize",
                                DisplayName = new Microsoft.Xrm.Sdk.Label("Font Size", 1033)
                            }
                        };
                    CreateAttributeResponse fontSizeAttributeResponse =
                        (CreateAttributeResponse)service.Execute(
                        fontSizeAttributeRequest);

                    Console.WriteLine("The custom activity has been created.");

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
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
