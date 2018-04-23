using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using System.Threading;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        static void Main(string[] args)
        {

            try
            {
                //You must specify connection information in common-data-service/App.config to run this sample.
                using (CrmServiceClient csc = new CrmServiceClient(GetConnectionStringFromAppConfig("Connect")))
                {
                    if (csc.IsReady)
                    {
                        IOrganizationService service = csc.OrganizationServiceProxy;

                        #region Sample Code
                        //////////////////////////////////////////////
                        #region Set up
                        // Check that the current version is greater than the minimum version
                        if (!SampleHelpers.CheckVersion(service, new Version("7.1.0.0")))
                        {
                            //The environment version is lower than version 7.1.0.0
                            return;
                        }
                        //Import the ChangeTrackingSample solution
                        if (SampleHelpers.ImportSolution(service, "ChangeTrackingSample", "ChangeTrackingSample_1_0_0_0_managed.zip"))
                        {
                            //Wait a minute if the solution is being imported. This will give time for the new metadata to be cached.
                            Thread.Sleep(TimeSpan.FromSeconds(60));
                        }

                        //Verify that the alternate key indexes are ready
                        if (!VerifyBookCodeKeyIsActive(service))
                        {
                            Console.WriteLine("There is a problem creating the index for the book code alternate key for the sample_book entity.");
                            Console.WriteLine("The sample cannot continue. Please try again.");

                            //Delete the ChangeTrackingSample solution
                            SampleHelpers.DeleteSolution(service, "ChangeTrackingSample");
                            return;
                        }

                        // Create 10 sample book records.
                        CreateInitialBookRecordsForSample(service);
                        #endregion Set up
                        #region Demonstrate

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
                        }
                        );

                        Console.WriteLine("\nList of deleted record entity references:");
                        deletedRecords.ForEach(e =>
                        {                            
                            Console.WriteLine(" LogicalName: {0} Id:{1}", e.LogicalName, e.Id);
                        }
                        );
                        Console.WriteLine("\n");

                        #endregion Demonstrate
                        #region Clean up
                        //Delete the ChangeTrackingSample solution
                        SampleHelpers.DeleteSolution(service, "ChangeTrackingSample");
                        #endregion Clean up
                        //////////////////////////////////////////////
                        #endregion Sample Code

                        Console.WriteLine("The sample completed successfully");
                        return;
                    }
                    else
                    {
                        const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Dynamics CRM";
                        if (csc.LastCrmError.Equals(UNABLE_TO_LOGIN_ERROR))
                        {
                            Console.WriteLine("Check the connection string values in common-data-service/App.config.");
                            throw new Exception(csc.LastCrmError);
                        }
                        else
                        {
                            throw csc.LastCrmException;
                        }
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Timestamp: {0}", ex.Detail.Timestamp);
                Console.WriteLine("Code: {0}", ex.Detail.ErrorCode);
                Console.WriteLine("Message: {0}", ex.Detail.Message);
                Console.WriteLine("Plugin Trace: {0}", ex.Detail.TraceText);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Message: {0}", ex.Message);
                Console.WriteLine("Stack Trace: {0}", ex.StackTrace);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.InnerException.Message ? "No Inner Fault" : ex.InnerException.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine(ex.Message);

                // Display the details of the inner exception.
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);

                    FaultException<OrganizationServiceFault> fe = ex.InnerException
                        as FaultException<OrganizationServiceFault>;
                    if (fe != null)
                    {
                        Console.WriteLine("Timestamp: {0}", fe.Detail.Timestamp);
                        Console.WriteLine("Code: {0}", fe.Detail.ErrorCode);
                        Console.WriteLine("Message: {0}", fe.Detail.Message);
                        Console.WriteLine("Plugin Trace: {0}", fe.Detail.TraceText);
                        Console.WriteLine("Inner Fault: {0}",
                            null == fe.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
                    }
                }
            }
            // Additional exceptions to catch: SecurityTokenValidationException, ExpiredSecurityTokenException,
            // SecurityAccessDeniedException, MessageSecurityException, and SecurityNegotiationException.

            finally
            {

                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }

        }
        /// <summary>
        /// Gets a named connection string from App.config
        /// </summary>
        /// <param name="name">The name of the connection string to return</param>
        /// <returns>The named connection string</returns>
        static string GetConnectionStringFromAppConfig(string name)
        {
            //Verify common-data-service/App.config contains a valid connection string with the name.
            try
            {
                return ConfigurationManager.ConnectionStrings[name].ConnectionString;
            }
            catch (Exception)
            {
                Console.WriteLine("You must set connection data in cds/App.config before running this sample.");
                return string.Empty;
            }
        }
    }
}
