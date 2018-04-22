using System;
using System.ServiceModel;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Configuration;

// These namespaces are found in the Microsoft.Xrm.Sdk.dll assembly
// found in the SDK\bin folder.
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;

// This namespace is found in Microsoft.Crm.Sdk.Proxy.dll assembly
// found in the SDK\bin folder.
using Microsoft.Xrm.Tooling.Connector;

namespace PowerApps.Samples
{

    partial class SampleProgram
    {

        private static OrganizationServiceProxy _service;
        private const String customBooksEntityName = "sample_book";
        System.String ManagedSolutionLocation = @"ChangeTrackingSample_1_0_0_0_managed.zip";

        public void Run(IOrganizationService service, bool promptForDelete)
        {
            try
            {


                // Check CRM version
                if (CheckCRMVersion(service))
                {
                    // Import the ChangeTrackingSample solution if it is not already installed.
                    ImportChangeTrackingSolution(service);

                    // Wait for entity and key index to be active.
                    WaitForEntityAndKeysToBeActive(service, customBooksEntityName.ToLower());

                    // Create 10 demo records.
                    CreateRequiredRecords(service);

                    //<snippetChangeTrackingSample1>
                    string token;

                    // Initialize page number.
                    int pageNumber = 1;
                    List<Entity> initialrecords = new List<Entity>();

                    // Retrieve records by using Change Tracking feature.
                    RetrieveEntityChangesRequest request = new RetrieveEntityChangesRequest();

                    request.EntityName = customBooksEntityName.ToLower();
                    request.Columns = new ColumnSet("sample_bookcode", "sample_name", "sample_author");
                    request.PageInfo = new PagingInfo() { Count = 5000, PageNumber = 1, ReturnTotalRecordCount = false };


                    // Initial Synchronization. Retrieves all records as well as token value.
                    Console.WriteLine("Initial synchronization....retrieving all records.");
                    while (true)
                    {
                        RetrieveEntityChangesResponse response = (RetrieveEntityChangesResponse)service.Execute(request);

                        initialrecords.AddRange(response.EntityChanges.Changes.Select(x => (x as NewOrUpdatedItem).NewOrUpdatedEntity).ToArray());
                        initialrecords.ForEach(x => Console.WriteLine("initial record id:{0}", x.Id));
                        if (!response.EntityChanges.MoreRecords)
                        {
                            // Store token for later query
                            token = response.EntityChanges.DataToken;
                            break;

                        }
                        // Increment the page number to retrieve the next page.
                        request.PageInfo.PageNumber++;
                        // Set the paging cookie to the paging cookie returned from current results.
                        request.PageInfo.PagingCookie = response.EntityChanges.PagingCookie;
                    }
                    //</snippetChangeTrackingSample1>

                    // Display the initial records.
                    // Do you like to view the records in browser? Add code.
                    if (PromptForView())
                    {
                        ViewEntityListInBrowser();
                    }

                    // Delay 10 seconds, then create/update/delete records
                    Console.WriteLine("waiting 10 seconds until next operation..");
                    Thread.Sleep(10000);


                    // Add another 10 records, 1 update, and 1 delete.
                    UpdateRecords(service);

                    // Second Synchronization. Basically do the same.
                    // Reset paging
                    pageNumber = 1;
                    request.PageInfo.PageNumber = pageNumber;
                    request.PageInfo.PagingCookie = null;
                    // Assign token
                    request.DataVersion = token;

                    // Instantiate cache.
                    List<Entity> updatedRecords = new List<Entity>();
                    List<EntityReference> deletedRecords = new List<EntityReference>();

                    while (true)
                    {

                        RetrieveEntityChangesResponse results = (RetrieveEntityChangesResponse)service.Execute(request);

                        updatedRecords.AddRange(results.EntityChanges.Changes.Where(x => x.Type == ChangeType.NewOrUpdated).Select(x => (x as NewOrUpdatedItem).NewOrUpdatedEntity).ToArray());
                        deletedRecords.AddRange(results.EntityChanges.Changes.Where(x => x.Type == ChangeType.RemoveOrDeleted).Select(x => (x as RemovedOrDeletedItem).RemovedItem).ToArray());


                        if (!results.EntityChanges.MoreRecords)
                            break;

                        // Increment the page number to retrieve the next page.
                        request.PageInfo.PageNumber++;
                        // Set the paging cookie to the paging cookie returned from current results.
                        request.PageInfo.PagingCookie = results.EntityChanges.PagingCookie;
                    }

                    // Do synchronizig work here.
                    Console.WriteLine("Retrieving changes since the last sync....");
                    updatedRecords.ForEach(x => Console.WriteLine("new or updated record id:{0}!", x.Id));
                    deletedRecords.ForEach(x => Console.WriteLine("record id:{0} deleted!", x.Id));

                    // Prompt to view the records in the browser.
                    if (PromptForView())
                    {
                        Console.WriteLine("Retrieving the changes for the sample_book entity.....");
                        ViewEntityListInBrowser();
                    }

                    // Prompts to delete the ChangeTrackingSample managed solution.
                    DeleteChangeTrackingSampleSolution(service, promptForDelete);
                }
            }
            // Catch any service fault exceptions that Microsoft Dynamics CRM throws.
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>)
            {
                // You can handle an exception here or pass it back to the calling method.
                throw;
            }
        }

        /// <summary>
        /// Checks whether the ChangeTrackingSample solution is already installed.
        /// If it is not, the ChangeTrackingSample_1_0_0_0_managed.zip file is imported to install
        /// this solution.
        /// </summary>
       

        static public void Main(string[] args)
        {
            try
            {
                // Obtain the target organization's Web address and client logon 
                // credentials from the user.
                CrmServiceClient csc = new CrmServiceClient(ConfigurationManager.ConnectionStrings["Connect"].ConnectionString);
                IOrganizationService service = csc.OrganizationServiceProxy;
                //_service = new OrganizationServiceProxy(ConfigurationManager.ConnectionStrings["Connect"].ConnectionString);
               // _serviceProxy = new OrganizationServiceProxy(serverConfig.OrganizationUri, serverConfig.HomeRealmUri, serverConfig.Credentials, serverConfig.DeviceCredentials);
                _service = csc.OrganizationServiceProxy;

                    SampleProgram app = new SampleProgram();
                    app.Run(csc, true);

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

            finally
            {

                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }

        } //#endregion Main
    } //Samleprogram
} //namespace