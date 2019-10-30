using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
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
                    // Create FetchXml for marketing list's query which locates accounts
                    // in Seattle.
                    String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                        <entity name='account'>
                                        <attribute name='name' />
                                        <attribute name='address1_city' />
                                        <attribute name='primarycontactid' />
                                        <attribute name='telephone1' />
                                        <attribute name='accountid' />
                                        <order attribute='name' descending='false' />
                                        <filter type='and'>
                                        <condition attribute='address1_city' operator='eq' value='seattle' />
                                        </filter>
                                        </entity>
                                        </fetch>";
                    // Create dynamic list. Set the type to true to declare a dynamic
                    // list.
                    var dynamicList = new List()
                    {
                        Type = true,
                        ListName = "Dynamic List",
                        CreatedFromCode = new OptionSetValue((int)ListCreatedFromCode.Account),
                        Query = fetchXml
                    };
                    _dynamicListId = service.Create(dynamicList);
                    dynamicList.Id = _dynamicListId;

                    Console.WriteLine("Created dynamic list.");

                    #endregion

                    #region Associate dynamic list to campaign

                    // Create a campaign.
                    var campaign = new Campaign()
                    {
                        Name = "Sample Campaign"
                    };
                    _campaignId = service.Create(campaign);
                    campaign.Id = _campaignId;

                    // Add the dynamic list to the campaign.
                    var addListToCampaignRequest =
                        new AddItemCampaignRequest()
                        {
                            CampaignId = _campaignId,
                            EntityId = _dynamicListId,
                            EntityName = List.EntityLogicalName,
                        };
                    service.Execute(addListToCampaignRequest);

                    Console.WriteLine("Added dynamic list to the campaign.");

                    // Create a campaign activity to distribute fax to the list members.
                    var campaignActivity = new CampaignActivity()
                    {
                        Subject = "Sample Campaign Activity",
                        ChannelTypeCode = new OptionSetValue((int)CampaignActivityChannelTypeCode.Fax),
                        RegardingObjectId = campaign.ToEntityReference()
                    };
                    _campaignActivityId = service.Create(campaignActivity);

                    // Add dynamic list to campaign activity.
                    var addListToCampaignActivityRequest =
                        new AddItemCampaignActivityRequest()
                        {
                            CampaignActivityId = _campaignActivityId,
                            ItemId = _dynamicListId,
                            EntityName = List.EntityLogicalName
                        };
                    service.Execute(addListToCampaignActivityRequest);

                    Console.WriteLine("Added dynamic list to the campaign activity.");

                    #endregion

                    #region Associate static list to campaign

                    // Copy the dynamic list to a static list.
                    var copyRequest =
                        new CopyDynamicListToStaticRequest()
                        {
                            ListId = _dynamicListId
                        };
                    CopyDynamicListToStaticResponse copyResponse =
                        (CopyDynamicListToStaticResponse)service.Execute(copyRequest);
                    _staticListId = copyResponse.StaticListId;

                    Console.WriteLine("Copied dynamic list to a static list.");

                    // Add the static list to the campaign.
                    var addStaticListToCampaignRequest =
                        new AddItemCampaignRequest()
                        {
                            CampaignId = _campaignId,
                            EntityId = _staticListId,
                            EntityName = List.EntityLogicalName
                        };
                    service.Execute(addStaticListToCampaignRequest);

                    Console.WriteLine("Added static list to the campaign.");

                    // Add the static list to the campaign activity.
                    var addStaticListToCampaignActivityRequest =
                        new AddItemCampaignActivityRequest()
                        {
                            CampaignActivityId = _campaignActivityId,
                            ItemId = _staticListId,
                            EntityName = List.EntityLogicalName
                        };
                    service.Execute(addStaticListToCampaignActivityRequest);

                    Console.WriteLine("Added static list to the campaign's activity.");

                    #endregion

                    #region Create fax for campaign's activity
                    // Create a fax.
                    var fax = new Fax()
                    {
                        Subject = "Example Fax"
                    };

                    Console.WriteLine("Created fax for campaign's activity.");
                    #endregion Create fax for campaign's activity

                    #region Distribute fax to the marketing list
                    // Distribute the campaign activity to the marketing lists.
                    var distributeRequest =
                        new DistributeCampaignActivityRequest()
                        {
                            CampaignActivityId = _campaignActivityId,
                            Activity = fax,
                            Owner = new EntityReference("systemuser", _salesManagerId),
                            Propagate = true,
                            SendEmail = false,
                            PostWorkflowEvent = true
                        };
                    service.Execute(distributeRequest);

                    Console.WriteLine("Distributed fax to the marketing lists.");
                    #endregion Distribute fax to the marketing list

                    #region Retrieve collection of entities from marketing list
                    // Retrieve a collection of entities that correspond 
                    // to all of the members in a marketing list
                    // This approach of retrieving list members allows you to dynamically
                    // retrieve the members of a list programmatically without requiring 
                    // knowledge of the member entity type.
                    var orgContext =
                        new OrganizationServiceContext(service);

                    var member = (from mb in orgContext.CreateQuery<List>()
                                  where mb.Id == _dynamicListId
                                  select mb).FirstOrDefault();

                    string fetchQuery = member.Query;

                    var memberRequest = new RetrieveMultipleRequest();
                    var fetch = new FetchExpression(fetchQuery);
                    memberRequest.Query = fetch;
                    var memberResponse =
                        (RetrieveMultipleResponse)service.Execute(memberRequest);

                    Console.WriteLine("Retrieved collection of entities from a marketing list.");
                    #endregion Retrieve collection of entities from marketing list
                }
                else
                {
                    const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Common Data Service";
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
