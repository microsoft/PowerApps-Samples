using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Diagnostics;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Threading;
using System.Configuration;

// These namespaces are found in the Microsoft.Xrm.Sdk.dll assembly
// found in the SDK\bin folder.
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;

// This namespace is found in Microsoft.Crm.Sdk.Proxy.dll assembly
// found in the SDK\bin folder.
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System.Net;

namespace PowerAppsSamples
{

    partial class SampleProgram
    {

        private static OrganizationServiceProxy _service;
        private const String customBooksEntityName = "sample_book";
        System.String ManagedSolutionLocation = @".\\Solution\ChangeTrackingSample_1_0_0_0_managed.zip";

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
        public void ImportChangeTrackingSolution(IOrganizationService service)
        {
            try
            {

                Console.WriteLine("Checking whether the ChangeTrackingSample solution already exists.....");

                QueryByAttribute queryCheckForSampleSolution = new QueryByAttribute();
                queryCheckForSampleSolution.AddAttributeValue("uniquename", "ChangeTrackingSample");
                queryCheckForSampleSolution.EntityName = Solution.EntityLogicalName;

                EntityCollection querySampleSolutionResults = service.RetrieveMultiple(queryCheckForSampleSolution);
                Solution SampleSolutionResults = null;
                if (querySampleSolutionResults.Entities.Count > 0)
                {
                    Console.WriteLine("The {0} solution already exists....", "ChangeTrackingSample");
                    SampleSolutionResults = (Solution)querySampleSolutionResults.Entities[0];

                }
                else
                {
                    Console.WriteLine("The ChangeTrackingSample solution does not exist. Importing the solution....");
                    byte[] fileBytes = File.ReadAllBytes(ManagedSolutionLocation);
                    ImportSolutionRequest impSolReq = new ImportSolutionRequest()
                    {
                        CustomizationFile = fileBytes
                    };

                    service.Execute(impSolReq);

                    Console.WriteLine("Imported Solution from {0}", ManagedSolutionLocation);
                    Console.WriteLine("Waiting for the alternate key index to be created.......");
                    Thread.Sleep(50000);

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
        /// Checks the current CRM version.
        /// If it is anything lower than 7.1.0.0, prompt to upgrade.
        /// </summary>
        private bool CheckCRMVersion(IOrganizationService service)
        {

            RetrieveVersionRequest crmVersionReq = new RetrieveVersionRequest();

            RetrieveVersionResponse crmVersionResp = (RetrieveVersionResponse)service.Execute(crmVersionReq);

            string version = crmVersionResp.Version;

            if (String.CompareOrdinal("7.1.0.0", crmVersionResp.Version) < 0)
            {
                return true;
            }
            else
            {
                Console.WriteLine("This sample cannot be run against the current version of CRM.");
                Console.WriteLine("Upgrade your CRM instance to the latest version to run this sample.");
                return false;
            }
        }

        /// <summary>
        /// Alternate keys may not be active immediately after the ChangeTrackingSample 
        /// solution is installed.This method polls the metadata for the sample_book
        /// entity to delay execution of the rest of the sample until the alternate keys are ready.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        /// <param name="entityLogicalName">The entity logical name, i.e. sample_product.</param>
        /// 
        private static void WaitForEntityAndKeysToBeActive(IOrganizationService service, string entityLogicalName)
        {
            EntityQueryExpression entityQuery = new EntityQueryExpression();
            entityQuery.Criteria = new MetadataFilterExpression(LogicalOperator.And)
            {
                Conditions = { { new MetadataConditionExpression("LogicalName", MetadataConditionOperator.Equals, entityLogicalName) } }
            };

            entityQuery.Properties = new MetadataPropertiesExpression("Keys");

            RetrieveMetadataChangesRequest metadataRequest = new RetrieveMetadataChangesRequest() { Query = entityQuery };

            bool allKeysReady = false;
            do
            {
                System.Threading.Thread.Sleep(5000);

                Console.WriteLine("Check for Entity...");
                RetrieveMetadataChangesResponse metadataResponse = (RetrieveMetadataChangesResponse)service.Execute(metadataRequest);

                if (metadataResponse.EntityMetadata.Count > 0)
                {
                    EntityKeyMetadata[] keys = metadataResponse.EntityMetadata[0].Keys;

                    allKeysReady = true;
                    if (keys.Length == 0)
                    {
                        Console.WriteLine("No Keys Found!!!");
                        allKeysReady = false;
                    }
                    else
                    {
                        foreach (var key in keys)
                        {
                            Console.WriteLine("  Key {0} status {1}", key.SchemaName, key.EntityKeyIndexStatus);
                            allKeysReady = allKeysReady && (key.EntityKeyIndexStatus == EntityKeyIndexStatus.Active);
                        }
                    }

                }
            } while (!allKeysReady);

            Console.WriteLine("Waiting 30 seconds for metadata caches to all synchronize...");
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(30));
        }

        // Prompt to view the entity.
        private static bool PromptForView()
        {
            Console.WriteLine("\nDo you want to view the sample product entity records? (y/n)");
            String answer = Console.ReadLine();
            if (answer.StartsWith("y") || answer.StartsWith("Y"))
            { return true; }
            else
            { return false; }
        }

        //Displays the sample product entity records in the browser.
        public void ViewEntityListInBrowser()
        {

            try
            {
                //Get the view ID
                QueryByAttribute query = new QueryByAttribute("savedquery");
                query.AddAttributeValue("returnedtypecode", "sample_book");
                query.AddAttributeValue("name", "Active Sample Books");
                query.ColumnSet = new ColumnSet("savedqueryid", "name");
                query.AddOrder("name", OrderType.Ascending);
                RetrieveMultipleRequest req = new RetrieveMultipleRequest() { Query = query };
                RetrieveMultipleResponse resp = (RetrieveMultipleResponse)_service.Execute(req);

                SavedQuery activeSampleBooksView = (SavedQuery)resp.EntityCollection[0];

                String webServiceURL = _service.ServiceConfiguration.CurrentServiceEndpoint.Address.Uri.AbsoluteUri;
                String entityInDefaultSolutionUrl = webServiceURL.Replace("XRMServices/2011/Organization.svc",
                 String.Format("main.aspx?etn={0}&pagetype=entitylist&viewid=%7b{1}%7d&viewtype=1039", "sample_book", activeSampleBooksView.SavedQueryId));

                // View in IE
                ProcessStartInfo browserProcess = new ProcessStartInfo("iexplore.exe");
                browserProcess.Arguments = entityInDefaultSolutionUrl;
                Process.Start(browserProcess);

            }
            catch (SystemException)
            {
                Console.WriteLine("\nThere was a problem opening Internet Explorer.");
            }


        }

        Guid bookIdtoDelete;
        /// <summary>
        /// Creates any entity records that this sample requires.
        /// </summary>
        public void CreateRequiredRecords(IOrganizationService service)
        {
            Console.WriteLine("Creating required records......");
            // Create 10 book records for demo.
            for (int i = 0; i < 10; i++)
            {
                Entity book = new Entity("sample_book");
                book["sample_name"] = "Demo Book " + i.ToString();
                book["sample_bookcode"] = "BookCode" + i.ToString();
                book["sample_author"] = "Author" + i.ToString();

                bookIdtoDelete = service.Create(book);
            }
            Console.WriteLine("10 records created...");
        }
        /// <summary>
        /// Update and delete records that this sample requires.
        /// </summary>
        public void UpdateRecords(IOrganizationService service)
        {
            Console.WriteLine("Adding ten more records....");
            // Create another 10 Account records for demo.
            for (int i = 10; i < 20; i++)
            {
                Entity book = new Entity("sample_book");
                book["sample_name"] = "Demo Book " + i.ToString();
                book["sample_bookcode"] = "BookCode" + i.ToString();
                book["sample_author"] = "Author" + i.ToString();
                service.Create(book);
            }

            // Update a record.
            Console.WriteLine("Updating an existing record");
            Entity updatebook = new Entity(customBooksEntityName.ToLower(), "sample_bookcode", "BookCode0");
            updatebook["sample_name"] = "Demo Book 0 updated";

            service.Update(updatebook);

            // Delete a record.
            Console.WriteLine("Deleting the {0} record....", bookIdtoDelete);
            service.Delete(customBooksEntityName.ToLower(), bookIdtoDelete);
        }
        /// <summary>
        /// Deletes the managed solution that was created for this sample.
        /// <param name="prompt"> Indicates whether to prompt the user to delete 
        /// the solution created in this sample.</param>
        /// If you choose "y", the managed solution will be deleted including the 
        /// sample_book entity and all the data in the entity. 
        /// If you choose "n", you must delete the solution manually to return 
        /// your organization to the original state.
        /// </summary>
        public void DeleteChangeTrackingSampleSolution(IOrganizationService service, bool prompt)
        {
            bool deleteSolution = true;
            if (prompt)
            {
                Console.WriteLine("\nDo you want to delete the ChangeTackingSample solution? (y/n)");
                String answer = Console.ReadLine();

                deleteSolution = (answer.StartsWith("y") || answer.StartsWith("Y"));
            }
            if (deleteSolution)
            {
                Console.WriteLine("Deleting the {0} solution....", "ChangeTrackingSample");
                QueryExpression queryImportedSolution = new QueryExpression
                {
                    EntityName = Solution.EntityLogicalName,
                    ColumnSet = new ColumnSet(new string[] { "solutionid", "friendlyname" }),
                    Criteria = new FilterExpression()
                };
                queryImportedSolution.Criteria.AddCondition("uniquename", ConditionOperator.Equal, "ChangeTrackingSample");

                Solution ImportedSolution = (Solution)service.RetrieveMultiple(queryImportedSolution).Entities[0];

                service.Delete(Solution.EntityLogicalName, (Guid)ImportedSolution.SolutionId);

                Console.WriteLine("Deleted the {0} solution.", ImportedSolution.FriendlyName);
            }
        }

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
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Timestamp: {0}", ex.Detail.Timestamp);
                Console.WriteLine("Code: {0}", ex.Detail.ErrorCode);
                Console.WriteLine("Message: {0}", ex.Detail.Message);
                Console.WriteLine("Plugin Trace: {0}", ex.Detail.TraceText);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
            }
            catch (System.TimeoutException ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Message: {0}", ex.Message);
                Console.WriteLine("Stack Trace: {0}", ex.StackTrace);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.InnerException.Message ? "No Inner Fault" : ex.InnerException.Message);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine(ex.Message);

                // Display the details of the inner exception.
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);

                    FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> fe = ex.InnerException
                        as FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>;
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