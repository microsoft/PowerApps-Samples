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

                    // Create the 'From:' activity party for the email
                    ActivityParty fromParty = new ActivityParty
                    {
                        PartyId = new EntityReference(SystemUser.EntityLogicalName, _userId)
                    };

                    // Create the 'To:' activity party for the email
                    ActivityParty toParty = new ActivityParty
                    {
                        PartyId = new EntityReference(Contact.EntityLogicalName, _contactId)
                    };

                    Console.WriteLine("Created activity parties.");

                    // Create an e-mail message.
                    Email email = new Email
                    {
                        To = new ActivityParty[] { toParty },
                        From = new ActivityParty[] { fromParty },
                        Subject = "SDK Sample e-mail",
                        Description = "SDK Sample for SendEmailFromTemplate Message.",
                        DirectionCode = true
                    };

                    //Create a query expression to get one of Email Template of type "contact"

                    QueryExpression queryBuildInTemplates = new QueryExpression
                    {
                        EntityName = "template",
                        ColumnSet = new ColumnSet("templateid", "templatetypecode"),
                        Criteria = new FilterExpression()
                    };
                    queryBuildInTemplates.Criteria.AddCondition("templatetypecode",
                        ConditionOperator.Equal, "contact");
                    EntityCollection templateEntityCollection = service.RetrieveMultiple(queryBuildInTemplates);

                    if (templateEntityCollection.Entities.Count > 0)
                    {
                        _templateId = (Guid)templateEntityCollection.Entities[0].Attributes["templateid"];
                    }
                    else
                    {
                        throw new ArgumentException("Standard Email Templates are missing");
                    }

                    // Create the request
                    SendEmailFromTemplateRequest emailUsingTemplateReq = new SendEmailFromTemplateRequest
                    {
                        Target = email,

                        // Use a built-in Email Template of type "contact".
                        TemplateId = _templateId,

                        // The regarding Id is required, and must be of the same type as the Email Template.
                        RegardingId = _contactId,
                        RegardingType = Contact.EntityLogicalName
                    };

                    SendEmailFromTemplateResponse emailUsingTemplateResp = (SendEmailFromTemplateResponse)service.Execute(emailUsingTemplateReq);

                    // Verify that the e-mail has been created
                    _emailId = emailUsingTemplateResp.Id;
                    if (!_emailId.Equals(Guid.Empty))
                    {
                        Console.WriteLine("Successfully sent an e-mail message using the template.");
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
