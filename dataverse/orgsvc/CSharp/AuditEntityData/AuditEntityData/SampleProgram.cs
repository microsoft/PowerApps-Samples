using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        [STAThread] // Added to support UX
        static void Main(string[] args)
        {
            CrmServiceClient service = null;
            bool organizationAuditingFlag;
            bool accountAuditingFlag;
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

                    Console.WriteLine("Enabling auditing on the organization and account entities.");

                    // Enable auditing on the organization.
                    // First, get the organization's ID from the system user record.
                    Guid orgId = ((WhoAmIResponse)service.Execute(new WhoAmIRequest())).OrganizationId;

                    // Next, retrieve the organization's record.
                    var retrivedOrg = service.Retrieve(Organization.EntityLogicalName, orgId,
                        new ColumnSet(new string[] {"isauditenabled" })) as Organization;

                    // Cache the value to set it back later
                    organizationAuditingFlag = retrivedOrg.IsAuditEnabled.Value;

                    // Enable auditing on the organization.
                    Organization orgToUpdate = new Organization {
                        Id = orgId,
                        IsAuditEnabled = true
                    };
                    service.Update(orgToUpdate);

                    // Enable auditing on account entities.
                    accountAuditingFlag = EnableEntityAuditing(service, Account.EntityLogicalName, true);

                    #endregion Enable Auditing for an Account
                    #region Retrieve the Record Change History
                    Console.WriteLine("Retrieving the account change history.\n");

                    // Retrieve the audit history for the account and display it.
                    var changeRequest = new RetrieveRecordChangeHistoryRequest { 
                        Target = new EntityReference(Account.EntityLogicalName, _newAccountId)
                    };                    

                    var changeResponse =
                        (RetrieveRecordChangeHistoryResponse)service.Execute(changeRequest);

                    AuditDetailCollection details = changeResponse.AuditDetailCollection;

                    foreach (AuditDetail detail in details.AuditDetails)
                    {
                        // Display some of the detail information in each audit record. 
                        DisplayAuditDetails(service,detail);
                    }
                    #endregion Retrieve the Record Change History

                    #region Retrieve the Attribute Change History

                    // Update the Telephone1 attribute in the Account entity record.
                    var accountToUpdate = new Account {
                        AccountId = _newAccountId,
                        Telephone1 = "123-555-5555"
                    };

                    service.Update(accountToUpdate);
                    Console.WriteLine("Updated the Telephone1 field in the Account entity.");

                    // Retrieve the attribute change history.
                    Console.WriteLine("Retrieving the attribute change history for Telephone1.");
                    var attributeChangeHistoryRequest = new RetrieveAttributeChangeHistoryRequest
                    {
                        Target = new EntityReference(
                        Account.EntityLogicalName, _newAccountId),
                        AttributeLogicalName = "telephone1"
                    };

                    var attributeChangeHistoryResponse =
                        (RetrieveAttributeChangeHistoryResponse)service.Execute(attributeChangeHistoryRequest);
                
                    // Display the attribute change history.
                    details = attributeChangeHistoryResponse.AuditDetailCollection;

                    foreach (var detail in details.AuditDetails)
                    {
                        DisplayAuditDetails(service,detail);
                    }

                    // Save an Audit record ID for later use.
                    Guid auditSampleId = details.AuditDetails.First().AuditRecord.Id;
                    #endregion Retrieve the Attribute Change History

                    #region Retrieve the Audit Details
                    Console.WriteLine("Retrieving audit details for an audit record.");

                    // Retrieve the audit details and display them.
                    var auditDetailsRequest = new RetrieveAuditDetailsRequest
                    {
                        AuditId = auditSampleId
                    };

                    var auditDetailsResponse =
                        (RetrieveAuditDetailsResponse)service.Execute(auditDetailsRequest);

                    DisplayAuditDetails(service,auditDetailsResponse.AuditDetail);

                    #endregion Retrieve the Audit Details

                    #region Revert Auditing
                    // Set the organization and account auditing flags back to the old values

                    orgToUpdate.IsAuditEnabled = organizationAuditingFlag;
                    service.Update(orgToUpdate);

                    EnableEntityAuditing(service,Account.EntityLogicalName, accountAuditingFlag);

                    #endregion Revert Auditing

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up

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

        private static bool EnableEntityAuditing(CrmServiceClient service,object entityLogicalName, bool v)
        {
            throw new NotImplementedException();
        }
    }
}
