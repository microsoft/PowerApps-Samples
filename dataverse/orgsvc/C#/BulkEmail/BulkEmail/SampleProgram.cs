using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        private static List<Guid> _contactsIds = new List<Guid>();
        private const int ARBITRARY_MAX_POLLING_TIME = 60;
        private static bool prompt = true;

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
                    #region Demonstrate

                    // Get a system user to use as the sender.
                    var emailSenderRequest = new WhoAmIRequest();
                    var emailSenderResponse =
                        service.Execute(emailSenderRequest) as WhoAmIResponse;

                    // Set trackingId for bulk mail request.
                    Guid trackingId = Guid.NewGuid();

                    var bulkMailRequest = new SendBulkMailRequest()
                    {
                        // Create a query expression for the bulk operation to use to retrieve 
                        // the contacts in the email list.
                        Query = new QueryExpression()
                        {
                            EntityName = Contact.EntityLogicalName,
                            ColumnSet = new ColumnSet(new String[] { "contactid" }),
                            Criteria = new FilterExpression()
                            {
                                Conditions =
                            {
                                new ConditionExpression("contactid",ConditionOperator.In, _contactsIds)
                            }
                            }
                        },
                        // Set the Sender.
                        Sender = new EntityReference(SystemUser.EntityLogicalName, emailSenderResponse.UserId),
                        // Set the RegardingId - this field is required.
                        //RegardingId = Guid.Empty,
                        //RegardingType = SystemUser.EntityLogicalName,

                        // Use a built-in Microsoft Dynamics CRM email template.
                        // NOTE: The email template's "template type" must match the type of 
                        // customers in the email list.  Our list contains contacts, so our 
                        // template must be for contacts.
                        TemplateId = new Guid("07B94C1D-C85F-492F-B120-F0A743C540E6"),
                        RequestId = trackingId
                    };

                    // Execute the async bulk email request
                    var resp = (SendBulkMailResponse)
                        service.Execute(bulkMailRequest);

                    Console.WriteLine("  Sent Bulk Email.");
                    #endregion

                    #region Monitoring SendBulkEmail

                    Console.WriteLine();
                    Console.WriteLine("Starting monitoring process..");

                    // Now that we've executed the bulk operation, we need to retrieve it 
                    // using our tracking Id.

                    var bulkQuery = new QueryByAttribute()
                    {
                        EntityName = AsyncOperation.EntityLogicalName,
                        ColumnSet = new ColumnSet(new string[] { "requestid"}),
                        Attributes = { "requestid" },
                        Values = { trackingId }
                    };

                    
                    // Retrieve the bulk email async operation.
                    EntityCollection aResponse = service.RetrieveMultiple(bulkQuery);
                    

                    Console.WriteLine("  Retrieved Bulk Email Async Operation.");

                    // Monitor the async operation via polling.
                    int secondsTicker = ARBITRARY_MAX_POLLING_TIME;

                    AsyncOperation createdBulkMailOperation = null;

                    Console.WriteLine("  Checking operation's state for " + ARBITRARY_MAX_POLLING_TIME + " seconds.");
                    Console.WriteLine();

                    while (secondsTicker > 0)
                    {
                        // Make sure the async operation was retrieved.
                        if (aResponse.Entities.Count > 0)
                        {
                            // Grab the one bulk operation that has been created.
                            createdBulkMailOperation = (AsyncOperation)aResponse.Entities[0];

                            // Check the operation's state.
                            if (createdBulkMailOperation.StateCode.Value !=
                                AsyncOperationState.Completed)
                            {
                                // The operation has not yet completed. 
                                // Wait a second for the status to change.
                                System.Threading.Thread.Sleep(1000);
                                secondsTicker--;

                                // Retrieve a fresh version the bulk delete operation.
                                aResponse = service.RetrieveMultiple(bulkQuery);
                            }
                            else
                            {
                                // Stop polling because the operation's state is now complete.
                                secondsTicker = 0;
                            }
                        }
                        else
                        {
                            // Wait a second for the async operation to activate.
                            System.Threading.Thread.Sleep(1000);
                            secondsTicker--;

                            // Retrieve the entity again
                            aResponse = service.RetrieveMultiple(bulkQuery);
                        }
                    }

                    // When the bulk email operation has completed, all sent emails will 
                    // have a status of "Pending Send" and will be picked up by your email 
                    // router.  Alternatively, you can then use BackgroundSendEmail to download
                    // all the emails created with the SendBulkEmail message. 
                    // See the BackgroundSendEmail sample for an example.
                    #endregion

                    #region Check success

                    // Validate async operation succeeded
                    if (createdBulkMailOperation.StateCode.Value == AsyncOperationState.Completed)
                    {
                        Console.WriteLine("Operation Completed.");
                    }
                    else
                    {
                        Console.WriteLine("Operation not completed yet.");
                    }

                  
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
