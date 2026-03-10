using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        private static string _customEntityName = "new_bankaccount";
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

                    //Create a custom entity
                    var createrequest = new CreateEntityRequest
                    {

                        //Define the entity
                        Entity = new EntityMetadata
                        {
                            SchemaName = _customEntityName,
                            DisplayName = new Label("Bank Account", 1033),
                            DisplayCollectionName = new Label("Bank Accounts", 1033),
                            Description = new Label("An entity to store information about customer bank accounts", 1033),
                            OwnershipType = OwnershipTypes.UserOwned,
                            IsActivity = false,

                        },

                        // Define the primary attribute for the entity
                        PrimaryAttribute = new StringAttributeMetadata
                        {
                            SchemaName = "new_accountname",
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                            MaxLength = 100,
                            FormatName = StringFormatName.Text,
                            DisplayName = new Label("Account Name", 1033),
                            Description = new Label("The primary attribute for the Bank Account entity.", 1033)
                        }

                    };
                    service.Execute(createrequest);
                    Console.WriteLine("The bank account entity has been created.");

                    //Add a String attribute to the custom entity
                    CreateAttributeRequest createBankNameAttributeRequest = new CreateAttributeRequest
                    {
                        EntityName = _customEntityName,
                        Attribute = new StringAttributeMetadata
                        {
                            SchemaName = "new_bankname",
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                            MaxLength = 100,
                            FormatName = StringFormatName.Text,
                            DisplayName = new Label("Bank Name", 1033),
                            Description = new Label("The name of the bank.", 1033)
                        }
                    };

                    service.Execute(createBankNameAttributeRequest);

                    //Add a Money attribute to the custom entity
                    CreateAttributeRequest createBalanceAttributeRequest = new CreateAttributeRequest
                    {
                        EntityName = _customEntityName,
                        Attribute = new MoneyAttributeMetadata
                        {
                            SchemaName = "new_balance",
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                            PrecisionSource = 2,
                            DisplayName = new Label("Balance", 1033),
                            Description = new Label("Account Balance at the last known date", 1033),

                        }
                    };

                    service.Execute(createBalanceAttributeRequest);

                    //Add a DateTime attribute to the custom entity
                    CreateAttributeRequest createCheckedDateRequest = new CreateAttributeRequest
                    {
                        EntityName = _customEntityName,
                        Attribute = new DateTimeAttributeMetadata
                        {
                            SchemaName = "new_checkeddate",
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                            Format = DateTimeFormat.DateOnly,
                            DisplayName = new Label("Date", 1033),
                            Description = new Label("The date the account balance was last confirmed", 1033)

                        }
                    };

                    service.Execute(createCheckedDateRequest);
                    Console.WriteLine("An date attribute has been added to the bank account entity.");

                    //Adding lookup attribute to the custom entity
                    CreateOneToManyRequest req = new CreateOneToManyRequest()
                    {
                        Lookup = new LookupAttributeMetadata()
                        {
                            Description = new Label("The referral (lead) from the bank account owner", 1033),
                            DisplayName = new Label("Referral", 1033),
                            LogicalName = "new_parent_leadid",
                            SchemaName = "New_Parent_leadId",
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.Recommended)
                        },
                        OneToManyRelationship = new OneToManyRelationshipMetadata()
                        {
                            AssociatedMenuConfiguration = new AssociatedMenuConfiguration()
                            {
                                Behavior = AssociatedMenuBehavior.UseCollectionName,
                                Group = AssociatedMenuGroup.Details,
                                Label = new Label("Bank Accounts", 1033),
                                Order = 10000
                            },
                            CascadeConfiguration = new CascadeConfiguration()
                            {
                                Assign = CascadeType.Cascade,
                                Delete = CascadeType.Cascade,
                                Merge = CascadeType.Cascade,
                                Reparent = CascadeType.Cascade,
                                Share = CascadeType.Cascade,
                                Unshare = CascadeType.Cascade
                            },
                            ReferencedEntity = "lead",
                            ReferencedAttribute = "leadid",
                            ReferencingEntity = _customEntityName,
                            SchemaName = "new_lead_new_bankaccount"
                        }
                    };
                    service.Execute(req);
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
