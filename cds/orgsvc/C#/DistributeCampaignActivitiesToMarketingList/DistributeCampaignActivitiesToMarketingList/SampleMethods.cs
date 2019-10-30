using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
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
        private static Guid _accountId;
        private static readonly List<Guid> _contactIdList = new List<Guid>();
        private static Guid _marketingListId;
        private static Guid _copiedMarketingListId;
        private static Guid _campaignId;
        private static Guid _campaignActivityId;
        private static Guid _originalCampaignId;
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

            CreateRequiredRecords(service);
        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, prompt);
        }

        private static class MarketingListType
        {
            internal static bool Static = false;
            internal static bool Dynamic = true;
        }

        // This static class contains values for overriding or removing when making a qualify
        // member list request.  (Overriding means replacing all members of the list with
        // the specified members in the request.)
        private static class OverrideOrRemove
        {
            internal static bool Override = true;
            internal static bool Remove = false;
        }
        private static void DistributeCampaign(CrmServiceClient service)
        {
            Console.WriteLine("=== Creating and Distributing the Campaign ===");
            // Create the campaign.
            var campaign = new Campaign
            {
                Name = "Sample Campaign"
            };

            _originalCampaignId = service.Create(campaign);

            NotifyEntityCreated(Campaign.EntityLogicalName, _originalCampaignId);

            // Copy the campaign.
            var campaignCopyRequest = new CopyCampaignRequest
            {
                BaseCampaign = _originalCampaignId
            };

            var copyCampaignResponse =
                (CopyCampaignResponse)service.Execute(campaignCopyRequest);
            _campaignId = copyCampaignResponse.CampaignCopyId;

            Console.WriteLine("  Copied the campaign to new campaign with GUID \r\n\t{{{0}}}",
                _campaignId);

            var activity = new CampaignActivity
            {
                Subject = "Sample phone call",
                ChannelTypeCode = new OptionSetValue((int)CampaignActivityChannelTypeCode.Phone),
                RegardingObjectId = new EntityReference(
                    Campaign.EntityLogicalName, _campaignId)
            };

            _campaignActivityId = service.Create(activity);

            NotifyEntityCreated(CampaignActivity.EntityLogicalName, _campaignActivityId);

            // Find the current user to determine who the owner of the activity should be.
            var whoAmI = new WhoAmIRequest();
            var currentUser = (WhoAmIResponse)service.Execute(whoAmI);

            // Add the marketing list created earlier to the campaign.
            var addListToCampaignRequest = new AddItemCampaignRequest
            {
                CampaignId = _campaignId,
                EntityId = _copiedMarketingListId,
                EntityName = List.EntityLogicalName,
            };

            service.Execute(addListToCampaignRequest);

            Console.WriteLine("  Added the marketing list to the campaign.");

            // Add the marketing list created earlier to the campaign activity.
            var addListToActivityRequest = new AddItemCampaignActivityRequest
            {
                CampaignActivityId = _campaignActivityId,
                ItemId = _copiedMarketingListId,
                EntityName = List.EntityLogicalName
            };

            service.Execute(addListToActivityRequest);

            Console.WriteLine("  Added the marketing list to the campaign activity.");

            // Create the phone call to use for distribution.
            var phonecall = new PhoneCall
            {
                Subject = "Sample Phone Call"
            };

            // Distribute and execute the campaign activity.
            // PostWorkflowEvent signals Microsoft Dynamics CRM to actually create the phone call activities.
            // Propagate also signals to Microsoft Dynamics CRM to create the phone call activities.
            // OwnershipOptions indicates whom the created activities should be assigned
            // to.
            var distributeRequest = new DistributeCampaignActivityRequest
            {
                Activity = phonecall,
                CampaignActivityId = _campaignActivityId,
                Owner = new EntityReference(
                    SystemUser.EntityLogicalName, currentUser.UserId),
                OwnershipOptions = PropagationOwnershipOptions.Caller,
                PostWorkflowEvent = true,
                Propagate = true,
                SendEmail = false,
            };

            var distributeResponse =
                (DistributeCampaignActivityResponse)service.Execute(distributeRequest);

            Console.WriteLine("  Distributed and executed the campaign activity to the marketing list.");

            // Retrieve the members that were distributed to.
            var retrieveMembersRequest = new RetrieveMembersBulkOperationRequest
            {
                BulkOperationId = distributeResponse.BulkOperationId,
                BulkOperationSource = (int)BulkOperationSource.CampaignActivity,
                EntitySource = (int)EntitySource.Contact,
                Query = new QueryExpression(Contact.EntityLogicalName)
            };

            var retrieveMembersResponse = (RetrieveMembersBulkOperationResponse)
                service.Execute(retrieveMembersRequest);

            Console.WriteLine("  Contacts with the following GUIDs were distributed to:");
            foreach (var member in retrieveMembersResponse.EntityCollection.Entities)
            {
                Console.WriteLine("\t{{{0}}}", member.Id);
            }
        }

        private static void CreateMarketingList(CrmServiceClient service)
        {
            Console.WriteLine("=== Creating the Marketing List ===");
            // Create the marketing list.  Make it static because members are going to be
            // added to the list.
            var list = new List
            {
                CreatedFromCode = new OptionSetValue((int)ListCreatedFromCode.Contact),
                ListName = "Sample Contact Marketing List",
                Type = MarketingListType.Static
            };

            _marketingListId = service.Create(list);

            NotifyEntityCreated(List.EntityLogicalName, _marketingListId);

            // Add a list of contacts to the marketing list.
            var addMemberListReq = new AddListMembersListRequest
            {
                MemberIds = new[] { _contactIdList[0], _contactIdList[2] },
                ListId = _marketingListId
            };

            service.Execute(addMemberListReq);

            Console.WriteLine("  Contacts with GUIDs \r\n\t{{{0}}}\r\n\tand {{{1}}}\r\n  were added to the list.",
                _contactIdList[0], _contactIdList[1]);

            // Copy the marketing list.  First create a new one, and then copy over the
            // members.
            list.ListName = list.ListName + " Copy";
            _copiedMarketingListId = service.Create(list);
            var copyRequest = new CopyMembersListRequest
            {
                SourceListId = _marketingListId,
                TargetListId = _copiedMarketingListId
            };

            service.Execute(copyRequest);

            // Add a single contact to the copied marketing list.
            var addMemberReq = new AddMemberListRequest
            {
                EntityId = _contactIdList[1],
                ListId = _copiedMarketingListId
            };

            service.Execute(addMemberReq);

            Console.WriteLine("  Contact with GUID\r\n\t{{{0}}}\r\n  was added to the list.",
                _contactIdList[1]);

            // Qualify the marketing list.
            var qualifyRequest = new QualifyMemberListRequest
            {
                OverrideorRemove = OverrideOrRemove.Override,
                MembersId = new[] { _contactIdList[0], _contactIdList[1] },
                ListId = _copiedMarketingListId
            };

            service.Execute(qualifyRequest);

            Console.WriteLine("  Qualified the copied marketing list so that it only\r\n    includes the first two members.");
        }

        private static void NotifyEntityCreated(string entityName, Guid entityId)
        {
            Console.WriteLine("  {0} created with GUID {{{1}}}",
                entityName, entityId);
        }

        private static void CreateRequiredRecords(CrmServiceClient service)
        {
            // Create an account.
            var account = new Account
            {
                Name = "Litware, Inc.",
                Address1_StateOrProvince = "Colorado"
            };
            _accountId = service.Create(account);

            // Create the contacts.
            var contact = new Contact
            {
                FirstName = "Ben",
                LastName = "Andrews",
                EMailAddress1 = "sample@example.com",
                Address1_City = "Redmond",
                Address1_StateOrProvince = "WA",
                Address1_Telephone1 = "(206)555-5555",
                ParentCustomerId = new EntityReference
                {
                    Id = _accountId,
                    LogicalName = account.LogicalName
                }
            };
            _contactIdList.Add(service.Create(contact));

            contact = new Contact
            {
                FirstName = "Colin",
                LastName = "Wilcox",
                EMailAddress1 = "sample@example.com",
                Address1_City = "Bellevue",
                Address1_StateOrProvince = "WA",
                Address1_Telephone1 = "(425)555-5555",
                ParentCustomerId = new EntityReference
                {
                    Id = _accountId,
                    LogicalName = account.LogicalName
                }
            };
            _contactIdList.Add(service.Create(contact));

            contact = new Contact
            {
                FirstName = "Lisa",
                LastName = "Andrews",
                EMailAddress1 = "sample@example.com",
                Address1_City = "Redmond",
                Address1_StateOrProvince = "WA",
                Address1_Telephone1 = "(206)555-5556",
                ParentCustomerId = new EntityReference
                {
                    Id = _accountId,
                    LogicalName = account.LogicalName
                }
            };
            _contactIdList.Add(service.Create(contact));

        }

        private static void RemoveRelationships(CrmServiceClient service)
        {
            // Remove the marketing list from the campaign activity.
            var removeFromCampaignActivity = new RemoveItemCampaignActivityRequest
            {
                CampaignActivityId = _campaignActivityId,
                ItemId = _copiedMarketingListId
            };

            service.Execute(removeFromCampaignActivity);
            Console.WriteLine("  Removed the marketing list from the campaign activity.");

            // Remove the marketing list from the campaign.
            var removeFromCampaign = new RemoveItemCampaignRequest
            {
                CampaignId = _campaignId,
                EntityId = _copiedMarketingListId
            };

            service.Execute(removeFromCampaign);
            Console.WriteLine("  Removed the marketing list from the campaign.");
        }

        private static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
            var toBeDeleted = true;
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
                RemoveRelationships(service);

                // Delete the marketing lists.
                service.Delete(List.EntityLogicalName, _marketingListId);
                service.Delete(List.EntityLogicalName, _copiedMarketingListId);

                // Delete the contacts.
                foreach (var contactId in _contactIdList)
                {
                    service.Delete(Contact.EntityLogicalName, contactId);
                }

                // Delete the account.
                service.Delete(Account.EntityLogicalName, _accountId);

                // Just delete the campaign, no need to delete the campaign activity.
                service.Delete(Campaign.EntityLogicalName, _campaignId);
                service.Delete(Campaign.EntityLogicalName, _originalCampaignId);
            }
        }
    }
}
