using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;

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
                    // Create the target for the request.
                    var target = new EntityReference();

                    // Id is the GUID of the account that is being merged into.
                    // LogicalName is the type of the entity being merged to, as a string
                    target.Id = _account1Id;
                    target.LogicalName = Account.EntityLogicalName;

                    // Create the request.
                    var merge = new MergeRequest();
                    // SubordinateId is the GUID of the account merging.
                    merge.SubordinateId = _account2Id;
                    merge.Target = target;
                    merge.PerformParentingChecks = false;

                    Console.WriteLine("\nMerging account2 into account1 and adding " +
                        "\"test\" as Address 1 Line 1");

                    // Create another account to hold new data to merge into the entity.
                    // If you use the subordinate account object, its data will be merged.
                    var updateContent = new Account();
                    updateContent.Address1_Line1 = "test";

                    // Set the content you want updated on the merged account
                    merge.UpdateContent = updateContent;

                    // Execute the request.
                    var merged = (MergeResponse)service.Execute(merge);

                    var mergeeAccount =
                        (Account)service.Retrieve(Account.EntityLogicalName,
                        _account2Id, new ColumnSet(allColumns: true));

                    if (mergeeAccount.Merged == true)
                    {
                        var mergedAccount =
                            (Account)service.Retrieve(Account.EntityLogicalName,
                            _account1Id, new ColumnSet(allColumns: true));

                        Console.WriteLine("\nAccounts merged successfully into account1");
                        Console.WriteLine("  Name: {0}", mergedAccount.Name);
                        Console.WriteLine("  Description: {0}", mergedAccount.Description);
                        Console.WriteLine("  Number of Employees: {0}",
                            mergedAccount.NumberOfEmployees);
                        Console.WriteLine("  Address 1 Line 1: {0}",
                            mergedAccount.Address1_Line1);
                    }
                    #endregion Demonstrate

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
                }
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
