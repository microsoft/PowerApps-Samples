using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
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
<<<<<<< HEAD
                    //////////////////////////////////////////////
=======
                    /////////////////////////////////////////////
>>>>>>> e17f0bedd03f33aec18be3f488aee58b5c2fbc6a
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate

                    // Create Field Security Profile.
<<<<<<< HEAD
                    FieldSecurityProfile managersProfile = new FieldSecurityProfile();
=======
                    var managersProfile = new FieldSecurityProfile();
>>>>>>> e17f0bedd03f33aec18be3f488aee58b5c2fbc6a
                    managersProfile.Name = "Managers";
                    profileId = service.Create(managersProfile);
                    Console.Write("Created Profile, ");

                    // Add team to profile.
                    var teamToProfile = new AssociateRequest()
                    {
                        Target = new EntityReference(FieldSecurityProfile.EntityLogicalName,
                            profileId),
                        RelatedEntities = new EntityReferenceCollection()
<<<<<<< HEAD
                        {
                            new EntityReference(Team.EntityLogicalName, teamId)
                        },
=======
        {
            new EntityReference(Team.EntityLogicalName, teamId)
        },
>>>>>>> e17f0bedd03f33aec18be3f488aee58b5c2fbc6a
                        Relationship = new Relationship("teamprofiles_association")
                    };
                    service.Execute(teamToProfile);

                    // Add user to the profile.
                    var userToProfile = new AssociateRequest()
                    {
                        Target = new EntityReference(FieldSecurityProfile.EntityLogicalName,
                            profileId),
                        RelatedEntities = new EntityReferenceCollection()
<<<<<<< HEAD
                        {
                            new EntityReference(SystemUser.EntityLogicalName, userId)
                        },
=======
        {
            new EntityReference(SystemUser.EntityLogicalName, userId)
        },
>>>>>>> e17f0bedd03f33aec18be3f488aee58b5c2fbc6a
                        Relationship = new Relationship("systemuserprofiles_association")
                    };
                    service.Execute(userToProfile);

                    // Create custom activity entity.
                    var req = new CreateEntityRequest()
                    {
                        Entity = new EntityMetadata
                        {
                            LogicalName = "new_tweet",
                            DisplayName = new Label("Tweet", 1033),
                            DisplayCollectionName = new Label("Tweet", 1033),
                            OwnershipType = OwnershipTypes.UserOwned,
                            SchemaName = "New_Tweet",
                            IsActivity = true,
                            IsAvailableOffline = true,
                            IsAuditEnabled = new BooleanManagedProperty(true),
                            IsMailMergeEnabled = new BooleanManagedProperty(false),
                        },
                        HasActivities = false,
                        HasNotes = true,
                        PrimaryAttribute = new StringAttributeMetadata()
                        {
                            SchemaName = "Subject",
                            LogicalName = "subject",
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(
                                AttributeRequiredLevel.None),
                            MaxLength = 100,
                            DisplayName = new Label("Subject", 1033)
                        }
                    };
                    service.Execute(req);
                    Console.Write("Entity Created, ");

                    // Add privileges for the Tweet entity to the Marketing Role.
<<<<<<< HEAD
                    RolePrivilege[] privileges = new RolePrivilege[3];
=======
                    var privileges = new RolePrivilege[3];
>>>>>>> e17f0bedd03f33aec18be3f488aee58b5c2fbc6a

                    // SDK: prvCreateActivity
                    privileges[0] = new RolePrivilege();
                    privileges[0].PrivilegeId = new Guid("{091DF793-FE5E-44D4-B4CA-7E3F580C4664}");
                    privileges[0].Depth = PrivilegeDepth.Global;

                    // SDK: prvReadActivity
                    privileges[1] = new RolePrivilege();
                    privileges[1].PrivilegeId = new Guid("{650C14FE-3521-45FE-A000-84138688E45D}");
                    privileges[1].Depth = PrivilegeDepth.Global;

                    // SDK: prvWriteActivity
                    privileges[2] = new RolePrivilege();
                    privileges[2].PrivilegeId = new Guid("{0DC8F72C-57D5-4B4D-8892-FE6AAC0E4B81}");
                    privileges[2].Depth = PrivilegeDepth.Global;

                    // Create and execute the request.
                    var request = new AddPrivilegesRoleRequest()
                    {
                        RoleId = roleId,
                        Privileges = privileges
                    };
<<<<<<< HEAD
                    AddPrivilegesRoleResponse response =
=======
                    var response =
>>>>>>> e17f0bedd03f33aec18be3f488aee58b5c2fbc6a
                        (AddPrivilegesRoleResponse)service.Execute(request);

                    // Create custom identity attribute.
                    var attrReq = new CreateAttributeRequest()
                    {
                        Attribute = new StringAttributeMetadata()
                        {
                            LogicalName = "new_identity",
                            DisplayName = new Label("Identity", 1033),
                            SchemaName = "New_Identity",
                            MaxLength = 500,
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(
                                AttributeRequiredLevel.Recommended),
                            IsSecured = true
                        },
                        EntityName = "new_tweet"
                    };
<<<<<<< HEAD
                    CreateAttributeResponse identityAttributeResponse =
=======
                    var identityAttributeResponse =
>>>>>>> e17f0bedd03f33aec18be3f488aee58b5c2fbc6a
                        (CreateAttributeResponse)service.Execute(attrReq);
                    identityId = identityAttributeResponse.AttributeId;
                    Console.Write("Identity Created, ");

                    // Create custom message attribute.
                    attrReq = new CreateAttributeRequest()
                    {
                        Attribute = new StringAttributeMetadata()
                        {
                            LogicalName = "new_message",
                            DisplayName = new Label("Message", 1033),
                            SchemaName = "New_Message",
                            MaxLength = 140,
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(
                                AttributeRequiredLevel.Recommended),
                            IsSecured = true
                        },
                        EntityName = "new_tweet"
                    };
                    var messageAttributeResponse =
                        (CreateAttributeResponse)service.Execute(attrReq);
                    messageId = messageAttributeResponse.AttributeId;
                    Console.Write("Message Created, ");

                    // Create field permission object for Identity.
                    var identityPermission = new FieldPermission();
                    identityPermission.AttributeLogicalName = "new_identity";
                    identityPermission.EntityName = "new_tweet";
                    identityPermission.CanRead = new OptionSetValue(FieldPermissionType.Allowed);
                    identityPermission.FieldSecurityProfileId = new EntityReference(
                        FieldSecurityProfile.EntityLogicalName, profileId);
                    identityPermissionId = service.Create(identityPermission);
                    Console.Write("Permission Created, ");

                    // Create list for storing retrieved profiles.
                    List<Guid> profileIds = new List<Guid>();

                    // Build query to obtain the field security profiles.
                    var qe = new QueryExpression()
                    {
                        EntityName = FieldSecurityProfile.EntityLogicalName,
                        ColumnSet = new ColumnSet("fieldsecurityprofileid"),
                        LinkEntities =
<<<<<<< HEAD
                        {
                            new LinkEntity
                            {
                                LinkFromEntityName = FieldSecurityProfile.EntityLogicalName,
                                LinkToEntityName = SystemUser.EntityLogicalName,
                                LinkCriteria =
                                {
                                    Conditions =
                                    {
                                        new ConditionExpression("systemuserid", ConditionOperator.Equal, userId)
                                    }
                                }
                            }
                        }
=======
        {
            new LinkEntity
            {
                LinkFromEntityName = FieldSecurityProfile.EntityLogicalName,
                LinkToEntityName = SystemUser.EntityLogicalName,
                LinkCriteria =
                {
                    Conditions =
                    {
                        new ConditionExpression("systemuserid", ConditionOperator.Equal, userId)
                    }
                }
            }
        }
>>>>>>> e17f0bedd03f33aec18be3f488aee58b5c2fbc6a
                    };

                    // Execute the query and obtain the results.
                    var rmRequest = new RetrieveMultipleRequest()
                    {
                        Query = qe
                    };

                    EntityCollection bec = ((RetrieveMultipleResponse)service.Execute(
                        rmRequest)).EntityCollection;

                    // Extract profiles from query result.
                    foreach (FieldSecurityProfile profileEnt in bec.Entities)
                    {
                        profileIds.Add(profileEnt.FieldSecurityProfileId.Value);
                    }
                    Console.Write("Profiles Retrieved, ");

                    // Retrieve attribute permissions of a FieldSecurityProfile.
                    DataCollection<Entity> dc;

                    // Retrieve the attributes.
<<<<<<< HEAD
                    QueryByAttribute qba = new QueryByAttribute(FieldPermission.EntityLogicalName);
=======
                    var qba = new QueryByAttribute(FieldPermission.EntityLogicalName);
>>>>>>> e17f0bedd03f33aec18be3f488aee58b5c2fbc6a
                    qba.AddAttributeValue("fieldsecurityprofileid", profileId);
                    qba.ColumnSet = new ColumnSet("attributelogicalname");

                    dc = service.RetrieveMultiple(qba).Entities;
                    Console.Write("Attributes Retrieved. ");
<<<<<<< HEAD
                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
=======

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up

>>>>>>> e17f0bedd03f33aec18be3f488aee58b5c2fbc6a
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
