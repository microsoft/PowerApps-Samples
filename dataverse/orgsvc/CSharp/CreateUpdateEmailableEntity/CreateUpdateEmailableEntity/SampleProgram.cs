using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        private const String _customEntityName = "new_agent";
        [STAThread] // Required to support the interactive login experience
        static void Main(string[] args)
        {
            CrmServiceClient service = null;
            try
            {
                service = SampleHelpers.Connect("Connect");
                if (service.IsReady)
                {
                    // Create any entity records that the demonstration code requires
                    SetUpSample(service);

                    #region Demonstrate

                    // Create the custom entity.
                    var createrequest = new CreateEntityRequest
                    {
                        // Define an entity to enable for emailing. In order to do so,
                        // IsActivityParty must be set.
                        Entity = new EntityMetadata
                        {
                            SchemaName = _customEntityName,
                            DisplayName = new Label("Agent", 1033),
                            DisplayCollectionName = new Label("Agents", 1033),
                            Description = new Label("Insurance Agents", 1033),
                            OwnershipType = OwnershipTypes.UserOwned,
                            IsActivity = false,

                            // Unless this flag is set, this entity cannot be party to an
                            // activity.
                            IsActivityParty = true
                        },

                        // As with built-in emailable entities, the Primary Attribute will
                        // be used in the activity party screens. Be sure to choose descriptive
                        // attributes.
                        PrimaryAttribute = new StringAttributeMetadata
                        {
                            SchemaName = "new_fullname",
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                            MaxLength = 100,
                            FormatName = StringFormatName.Text,
                            DisplayName = new Label("Agent Name", 1033),
                            Description = new Label("Agent Name", 1033)
                        }
                    };

                    service.Execute(createrequest);
                    Console.WriteLine("The emailable entity has been created.");

                    // The entity will not be selectable as an activity party until its customizations
                    // have been published. Otherwise, the e-mail activity dialog cannot find
                    // a correct default view.
                    PublishAllXmlRequest publishRequest = new PublishAllXmlRequest();
                    service.Execute(publishRequest);

                    // Before any emails can be created for this entity, an Email attribute
                    // must be defined.
                    var createFirstEmailAttributeRequest = new CreateAttributeRequest
                    {
                        EntityName = _customEntityName,
                        Attribute = new StringAttributeMetadata
                        {
                            SchemaName = "new_emailaddress",
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                            MaxLength = 100,
                            FormatName = StringFormatName.Email,
                            DisplayName = new Label("Email Address", 1033),
                            Description = new Label("Email Address", 1033)
                        }
                    };

                    service.Execute(createFirstEmailAttributeRequest);
                    Console.WriteLine("An email attribute has been added to the emailable entity.");

                    // Create a second, alternate email address. Since there is already one 
                    // email attribute on the entity, this will never be used for emailing
                    // even if the first one is not populated.
                    var createSecondEmailAttributeRequest = new CreateAttributeRequest
                    {
                        EntityName = _customEntityName,
                        Attribute = new StringAttributeMetadata
                        {
                            SchemaName = "new_secondaryaddress",
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                            MaxLength = 100,
                            FormatName = StringFormatName.Email,
                            DisplayName = new Label("Secondary Email Address", 1033),
                            Description = new Label("Secondary Email Address", 1033)
                        }
                    };

                    service.Execute(createSecondEmailAttributeRequest);

                    Console.WriteLine("A second email attribute has been added to the emailable entity.");
                    #endregion Demonstrate
                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
                }
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
