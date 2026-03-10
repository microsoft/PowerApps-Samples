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

                    // Create the BulkDetectDuplicatesRequest object
                    Console.WriteLine("  Creating the BulkDetectDuplicatesRequest object");
                    var request = new BulkDetectDuplicatesRequest()
                    {
                        JobName = "Detect Duplicate Accounts",
                        Query = new QueryExpression()
                        {
                            EntityName = Account.EntityLogicalName,
                            ColumnSet = new ColumnSet(true)
                        },
                        RecurrencePattern = String.Empty,
                        RecurrenceStartTime = DateTime.Now,
                        ToRecipients = new Guid[0],
                        CCRecipients = new Guid[0]
                    };

                    // Execute the request
                    Console.WriteLine("  Executing BulkDetectDuplicatesRequest");
                    response = (BulkDetectDuplicatesResponse)service
                        .Execute(request);

                    #region check success

                    Console.WriteLine("  Waiting for job to complete...");
                    WaitForAsyncJobToFinish(service,response.JobId, 240);

                    var query = new QueryByAttribute()
                    {
                        ColumnSet = new ColumnSet(true),
                        EntityName = "duplicaterecord"
                    };
                    query.Attributes.Add("asyncoperationid");
                    query.Values.Add(response.JobId);
                    EntityCollection results = service.RetrieveMultiple(query);

                    // check to make sure each id is found in the collection
                    var duplicateIds = results.Entities.Select((entity) =>
                        ((DuplicateRecord)entity).BaseRecordId.Id);
                    foreach (var id in duplicateAccounts.Select((account) => account.Id))
                    {
                        if (!duplicateIds.Contains(id))
                        {
                            throw new Exception(String.Format(
                                "Account with ID {0} was not detected as a duplicate",
                                id));
                        }
                    }
                    Console.WriteLine("  All accounts detected as duplicates successfully");

                    #endregion

                    Console.WriteLine();
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
