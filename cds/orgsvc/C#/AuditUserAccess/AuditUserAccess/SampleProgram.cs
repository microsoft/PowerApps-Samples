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
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate

                    // Enable auditing on the organization and for user access by editing the
                    // organization's settings.
                    // First, get the organization's ID from the system user record.

                    var whoAmIReq = new WhoAmIRequest();
                    var whoAmIRes = (WhoAmIResponse)service.Execute(whoAmIReq);
                    Guid orgId = whoAmIRes.OrganizationId;
                    _systemUserId = whoAmIRes.UserId;

                    // Next, retrieve the organization's record.
                    var org = (Organization)service.Retrieve(
                        Organization.EntityLogicalName, orgId,
                        new ColumnSet("organizationid", "isauditenabled", "isuseraccessauditenabled", "useraccessauditinginterval"));

                    // Finally, enable auditing on the organization, including auditing for
                    // user access.
                    bool organizationAuditingFlag = org.IsAuditEnabled.Value;
                    bool userAccessAuditingFlag = org.IsUserAccessAuditEnabled.Value;
                    if (!organizationAuditingFlag || !userAccessAuditingFlag)
                    {
                        org.IsAuditEnabled = true;
                        org.IsUserAccessAuditEnabled = true;
                        service.Update(org);
                        Console.WriteLine("Enabled auditing for the organization and for user access.");
                        Console.WriteLine("Auditing interval is set to {0} hours.", org.UserAccessAuditingInterval);
                    }
                    else
                    {
                        Console.WriteLine("Auditing was enabled before the sample began, so no auditing settings were changed.");
                    }

                    // Enable auditing on the account entity, since no audits will be created
                    // when we create/update an account entity, otherwise.
                    var oldAccountAuditing = EnableEntityAuditing(service, Account.EntityLogicalName, true);

                    // Make an update request to the Account entity to be tracked by auditing.
                    var newAccount = new Account();
                    newAccount.AccountId = _newAccountId;
                    newAccount.AccountNumber = "1-A";
                    newAccount.AccountCategoryCode = new OptionSetValue(
                        (int)AccountAccountCategoryCode.PreferredCustomer);
                    newAccount.Telephone1 = "555-555-5555";

                    service.Update(newAccount);
                    Console.WriteLine("Created an account and made updates which should be captured by auditing.");

                    // Set the organization and account auditing flags back to the old values
                    if (!organizationAuditingFlag || !userAccessAuditingFlag)
                    {
                        // Only revert them if they were actually changed to begin with.
                        org.IsAuditEnabled = organizationAuditingFlag;
                        org.IsUserAccessAuditEnabled = userAccessAuditingFlag;
                        service.Update(org);
                        Console.WriteLine("Reverted organization and user access auditing to their previous values.");
                    }
                    else
                    {
                        Console.WriteLine("Auditing was enabled before the sample began, so no auditing settings were reverted.");
                    }

                    // Revert the account entity auditing.
                    EnableEntityAuditing(service, Account.EntityLogicalName, oldAccountAuditing);

                    #endregion Revert auditing

                    #region Show Audited Records

                    // Select all columns for convenience.
                    var query = new QueryExpression(Audit.EntityLogicalName)
                    {
                        ColumnSet = new ColumnSet(true),
                        Criteria = new FilterExpression(LogicalOperator.And)
                    };

                    // Only retrieve audit records that track user access.
                    query.Criteria.AddCondition("action", ConditionOperator.In,
                        (int)AuditAction.UserAccessAuditStarted,
                        (int)AuditAction.UserAccessAuditStopped,
                        (int)AuditAction.UserAccessviaWebServices,
                        (int)AuditAction.UserAccessviaWeb);

                    // Change this to false in order to retrieve audit records for all users
                    // when running the sample.
                    var filterAuditsRetrievedByUser = true;
                    if (filterAuditsRetrievedByUser)
                    {
                        // Only retrieve audit records for the current user or the "SYSTEM"
                        // user.
                        var userFilter = new FilterExpression(LogicalOperator.Or);
                        userFilter.AddCondition(
                            "userid", ConditionOperator.Equal, _systemUserId);
                        userFilter.AddCondition(
                            "useridname", ConditionOperator.Equal, "SYSTEM");
                    }
                    // Only retrieve records for this sample run, so that we don't get too
                    // many results if auditing was enabled previously.
                    query.Criteria.AddCondition(
                        "createdon", ConditionOperator.GreaterEqual, DateTime.UtcNow);

                    var results = service.RetrieveMultiple(query);
                    Console.WriteLine("Retrieved audit records:");
                    foreach (Audit audit in results.Entities)
                    {
                        Console.Write("\r\n  Action: {0},  User: {1},"
                            + "\r\n    Created On: {2}, Operation: {3}",
                            (AuditAction)audit.Action.Value,
                            audit.UserId.Name,
                            audit.CreatedOn.Value.ToLocalTime(),
                            (AuditOperation)audit.Operation.Value);

                        // Display the name of the related object (which will be the user
                        // for audit records with Action UserAccessviaWebServices.
                        if (!String.IsNullOrEmpty(audit.ObjectId.Name))
                        {
                            Console.WriteLine(
                                ",\r\n    Related Record: {0}", audit.ObjectId.Name);
                        }
                        else
                        {
                            Console.WriteLine();
                        }
                    }

                    #endregion Show Audited Records

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
    }
}
