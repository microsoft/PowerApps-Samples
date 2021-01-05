using Microsoft.Crm.Sdk.Messages;
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

                    // Retrieve the current user information.
                    WhoAmIRequest whoAmIRequest = new WhoAmIRequest();
                    WhoAmIResponse whoAmIResponse = (WhoAmIResponse)service.Execute(
                        whoAmIRequest);

                    ColumnSet columnSet = new ColumnSet("fullname");
                    SystemUser currentUser = (SystemUser)service.Retrieve(
                        SystemUser.EntityLogicalName,
                        whoAmIResponse.UserId, columnSet);
                    String currentUserName = currentUser.FullName;
                    _userId = currentUser.Id;

                    // Create an instance of an existing queueitem in order to specify 
                    // the user that will be working on it using PickFromQueueRequest.

                    PickFromQueueRequest pickFromQueueRequest = new PickFromQueueRequest
                    {
                        QueueItemId = _queueItemId,
                        WorkerId = _userId
                    };

                    service.Execute(pickFromQueueRequest);

                    Console.WriteLine("The letter queue item is queued for new owner {0}.",
                        currentUserName);

                    
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
