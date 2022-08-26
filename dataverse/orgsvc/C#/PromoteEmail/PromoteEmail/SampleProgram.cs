using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial  class SampleProgram
    {
        // Define the IDs needed for this sample.
        private static Guid _contactId;
        private static Guid _emailId;
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
                    #region sample Code
                    ///////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Setup
                    #region Demonstrate
                    // Create a contact to send an email to (To: field)
                    Contact emailContact = new Contact()
                    {
                        FirstName = "Lisa",
                        LastName = "Andrews",
                        EMailAddress1 = "lisa@contoso.com"
                    };
                    _contactId = service.Create(emailContact);
                    Console.WriteLine("Created a sample contact.");

                    // Get a system user to send the email (From: field)
                    WhoAmIRequest systemUserRequest = new WhoAmIRequest();
                    WhoAmIResponse systemUserResponse = (WhoAmIResponse)service.Execute(systemUserRequest);

                    ColumnSet cols = new ColumnSet("internalemailaddress");
                    SystemUser emailSender = (SystemUser)service.Retrieve(SystemUser.EntityLogicalName, systemUserResponse.UserId, cols);


                    // Create the request.
                    DeliverPromoteEmailRequest deliverEmailRequest = new DeliverPromoteEmailRequest
                    {
                        Subject = "SDK Sample Email",
                        To = emailContact.EMailAddress1,
                        From = emailSender.InternalEMailAddress,
                        Bcc = String.Empty,
                        Cc = String.Empty,
                        Importance = "high",
                        Body = "This message will create an email activity.",
                        //MessageId = Guid.NewGuid().ToString(),
                        SubmittedBy = "",
                        ReceivedOn = DateTime.Now
                    };

                    // We won't attach a file to the email, but the Attachments property is required.
                    deliverEmailRequest.Attachments = new EntityCollection(new ActivityMimeAttachment[0]);
                    deliverEmailRequest.Attachments.EntityName = ActivityMimeAttachment.EntityLogicalName;

                    // Execute the request.
                    DeliverPromoteEmailResponse deliverEmailResponse = (DeliverPromoteEmailResponse)service.Execute(deliverEmailRequest);

                    // Verify the success.

                    // Define an anonymous type to define the possible values for
                    // email status
                    var EmailStatus = new
                    {
                        Draft = 1,
                        Completed = 2,
                        Sent = 3,
                        Received = 3,
                        Canceled = 5,
                        PendingSend = 6,
                        Sending = 7,
                        Failed = 8,
                    };

                    // Query for the delivered email, and verify the status code is "Sent".
                    ColumnSet deliveredMailColumns = new ColumnSet("statuscode");
                    Email deliveredEmail = (Email)service.Retrieve(Email.EntityLogicalName, deliverEmailResponse.EmailId, deliveredMailColumns);

                    _emailId = deliveredEmail.ActivityId.Value;

                    if (deliveredEmail.StatusCode.Value == EmailStatus.Sent)
                    {
                        Console.WriteLine("Successfully created and delivered the e-mail message.");
                    }


                    DeleteRequiredRecords(service,prompt);
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
