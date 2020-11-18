using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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
                // Service implements IOrganizationService interface 
                if (service.IsReady)
                {

                    #region Sample Code
                    //////////////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate

                    /*
                        The Sample setup will import a ChangeTracking solution that contains 
                        a sample_book entity that has an alternate key named sample_bookcode.

                        10 initial sample_book entity records are created so that changes to those
                        entities can be tracked.
                    */

                    //To cache the RetrieveEntityChangesResponse.EntityChanges.DataToken value
                    string dataVersionToken;

                    //To cache information about the inital set of book records created.
                    List<Entity> initialRecords = new List<Entity>();

                    //Retrieve initial records with tracked changes
                    RetrieveEntityChangesRequest initialRequest = new RetrieveEntityChangesRequest()
                    {
                        EntityName = "sample_book",
                        Columns = new ColumnSet("sample_bookcode", "sample_name", "sample_author"),
                        PageInfo = new PagingInfo()
                        { Count = 5000, PageNumber = 1, ReturnTotalRecordCount = false }
                    };

                    // Initial Synchronization. Retrieves all records as well as token value.
                    Console.WriteLine("Initial synchronization....retrieving all records.");

                    RetrieveEntityChangesResponse initialResponse = (RetrieveEntityChangesResponse)service.Execute(initialRequest);

                    //Cache the initial records for comparison.
                    initialRecords.AddRange(initialResponse.EntityChanges.Changes.Select(x => (x as NewOrUpdatedItem).NewOrUpdatedEntity).ToArray());

                    // Store token for second query
                    dataVersionToken = initialResponse.EntityChanges.DataToken;

                    Console.WriteLine("Waiting 10 seconds until next operation..");
                    Thread.Sleep(10000);

                    //Add another 10 records, 1 update, and 1 delete
                    UpdateBookRecordsForSample(service);

                    // Instantiate cache for changed and deleted records
                    List<Entity> updatedRecords = new List<Entity>();
                    List<EntityReference> deletedRecords = new List<EntityReference>();

                    //Retrieve Changes since the initial records were created
                    //The request is identical except it now has the DataVersion value set to the DataToken of the previous request.
                    RetrieveEntityChangesRequest secondRequest = new RetrieveEntityChangesRequest()
                    {
                        EntityName = "sample_book",
                        Columns = new ColumnSet("sample_bookcode", "sample_name", "sample_author"),
                        PageInfo = new PagingInfo()
                        { Count = 5000, PageNumber = 1, ReturnTotalRecordCount = false },
                        DataVersion = dataVersionToken
                    };

                    //Get the results from the second request
                    RetrieveEntityChangesResponse results = (RetrieveEntityChangesResponse)service.Execute(secondRequest);

                    //Separate the results by type: NewOrUpdated or RemoveOrDeleted
                    updatedRecords.AddRange(results.EntityChanges.Changes
                        .Where(x => x.Type == ChangeType.NewOrUpdated)
                        .Select(x => (x as NewOrUpdatedItem).NewOrUpdatedEntity).ToArray());

                    deletedRecords.AddRange(results.EntityChanges.Changes
                        .Where(x => x.Type == ChangeType.RemoveOrDeleted)
                        .Select(x => (x as RemovedOrDeletedItem).RemovedItem).ToArray());

                    //Test results
                    Console.WriteLine("\nList of updated records:");
                    updatedRecords.ForEach(e =>
                    {
                        Console.WriteLine(" name: {0}", e["sample_name"]);

                            /*
                             Expected:
                               name: Demo Book 10
                               name: Demo Book 11
                               name: Demo Book 12
                               name: Demo Book 13
                               name: Demo Book 14
                               name: Demo Book 15
                               name: Demo Book 16
                               name: Demo Book 17
                               name: Demo Book 18
                               name: Demo Book 19
                               name: Demo Book 0 updated < Time record was updated >
                            */
                    }
                    );

                    Console.WriteLine("\nList of deleted record entity references:");
                    deletedRecords.ForEach(e =>
                    {
                        Console.WriteLine(" LogicalName: {0} Id:{1}", e.LogicalName, e.Id);

                            /*
                             Expected:
                               LogicalName: sample_book Id:< GUID of record that was deleted >
                            */

                    }
                    );
                    Console.WriteLine("\n");

                    #endregion Demonstrate
                    #region Clean up
                    // Provides option to delete the ChangeTracking solution
                    CleanUpSample(service);
                    #endregion Clean up
                    //////////////////////////////////////////////
                    #endregion Sample Code

                    Console.WriteLine("The sample completed successfully");
                    return;
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
