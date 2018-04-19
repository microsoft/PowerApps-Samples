using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
     partial class SampleProgram
    {

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


    }

}
