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
                    Guid CurrentUserId = ((WhoAmIResponse)service.Execute(new WhoAmIRequest())).UserId;

                    // Get known private queues for the user 
                    // by using RetrieveUserQueuesRequest message.
                    var retrieveUserQueuesRequest = new RetrieveUserQueuesRequest
                    {
                        UserId = CurrentUserId,
                        IncludePublic = true
                    };
                    var retrieveUserQueuesResponse =
                        (RetrieveUserQueuesResponse)service.Execute(retrieveUserQueuesRequest);
                    EntityCollection queues = (EntityCollection)retrieveUserQueuesResponse.EntityCollection;

                    Guid sourceQueueId = new Guid();
                    Guid destinationQueueId = new Guid();

                    foreach (Entity entity in queues.Entities)
                    {
                        var queue = (Queue)entity;
                        switch (queue.Name)
                        {
                            case "Source Queue":
                                sourceQueueId = queue.Id;
                                break;
                            case "Destination Queue":
                                destinationQueueId = queue.Id;
                                break;
                        }
                    }

                    // Move a record from a source queue to a destination queue
                    // by using the AddToQueue request message.
                    var routeRequest = new AddToQueueRequest
                    {
                        SourceQueueId = sourceQueueId,
                        Target = new EntityReference(Letter.EntityLogicalName, _letterId),
                        DestinationQueueId = destinationQueueId
                    };

                    // Execute the Request
                    service.Execute(routeRequest);

                    Console.WriteLine(@"The letter record has been moved to a new queue.");
                    #endregion Demonstrate

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
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
