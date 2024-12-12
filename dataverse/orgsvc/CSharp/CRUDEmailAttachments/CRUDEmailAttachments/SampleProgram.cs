using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Text;

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

                    // Create three e-mail attachments
                    for (int i = 0; i < 3; i++)
                    {
                        ActivityMimeAttachment _sampleAttachment = new ActivityMimeAttachment
                        {
                            ObjectId = new EntityReference(Email.EntityLogicalName, emailId),
                            ObjectTypeCode = Email.EntityLogicalName,
                            Subject = String.Format("Sample Attachment {0}", i),
                            Body = System.Convert.ToBase64String(
                                    new ASCIIEncoding().GetBytes("Example Attachment")),
                            FileName = String.Format("ExampleAttachment{0}.txt", i)
                        };

                        emailAttachmentId[i] = service.Create(_sampleAttachment);
                    }

                    Console.WriteLine("Created three e-mail attachments for the e-mail activity.");

                    // Retrieve an attachment including its id, subject, filename and body.
                    ActivityMimeAttachment _singleAttachment =
                        (ActivityMimeAttachment)service.Retrieve(
                                                    ActivityMimeAttachment.EntityLogicalName,
                                                    emailAttachmentId[0],
                                                    new ColumnSet("activitymimeattachmentid",
                                                        "subject",
                                                        "filename",
                                                        "body"));

                    Console.WriteLine("Retrieved an email attachment, {0}.", _singleAttachment.FileName);

                    // Update attachment
                    _singleAttachment.FileName = "ExampleAttachmentUpdated.txt";
                    service.Update(_singleAttachment);

                    Console.WriteLine("Updated the retrieved e-mail attachment to {0}.", _singleAttachment.FileName);

                    // Retrieve all attachments associated with the email activity.
                    QueryExpression _attachmentQuery = new QueryExpression
                    {
                        EntityName = ActivityMimeAttachment.EntityLogicalName,
                        ColumnSet = new ColumnSet("activitymimeattachmentid"),
                        Criteria = new FilterExpression
                        {
                            Conditions =
                        {
                            new ConditionExpression
                            {
                                AttributeName = "objectid",
                                Operator = ConditionOperator.Equal,
                                Values = {emailId}
                            },
                            new ConditionExpression
                            {
                                AttributeName = "objecttypecode",
                                Operator = ConditionOperator.Equal,
                                Values = {Email.EntityLogicalName}
                            }
                        }
                        }
                    };

                    EntityCollection results = service.RetrieveMultiple(
                        _attachmentQuery);

                    Console.WriteLine("Retrieved all the e-mail attachments.");
                   
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
            catch(Exception ex)
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
