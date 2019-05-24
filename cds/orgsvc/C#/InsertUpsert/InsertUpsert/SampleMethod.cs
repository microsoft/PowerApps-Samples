using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
   public partial class SampleProgram
    {
        /// <summary>

        /// Function to set up the sample.

        /// </summary>

        /// <param name="service">Specifies the service to connect to.</param>

        private static void SetUpSample(CrmServiceClient service)

        {

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
            // Waits for the alternate key to be active.
            WaitForEntityAndKeysToBeActive(_serviceProxy, _customProductsEntityName.ToLower());

            // Processes the data in newsampleproduct.xml 
            // to represent new products. Creates 13 records in sample_product entity.
            // RecordCreated property returns true to indicate the records were created.

            ProcessUpsert(".\\newsampleproduct.xml");

            // Processes the data in updatedsampleproduct.xml 
            // to represent updates to products previously created. 
            // Updates 6 existing records in sample_product entity.
            // RecordCreated property returns false to indicate the existing records were updated.
            ProcessUpsert(".\\updatedsampleproduct.xml");
        }
        public void ProcessUpsert(String Filename)
        {
            Console.WriteLine("Executing upsert operation.....");
            XmlTextReader tr = new XmlTextReader(Filename);
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(tr);
            XmlNodeList xnlNodes = xdoc.DocumentElement.SelectNodes("/products/product");

            foreach (XmlNode xndNode in xnlNodes)
            {
                String productCode = xndNode.SelectSingleNode("Code").InnerText;
                String productName = xndNode.SelectSingleNode("Name").InnerText;
                String productCategory = xndNode.SelectSingleNode("Category").InnerText;
                String productMake = xndNode.SelectSingleNode("Make").InnerText;

                //use alternate key for product
                Entity productToCreate = new Entity("sample_product", "sample_productcode", productCode);

                productToCreate["sample_name"] = productName;
                productToCreate["sample_category"] = productCategory;
                productToCreate["sample_make"] = productMake;
                UpsertRequest request = new UpsertRequest()
                {
                    Target = productToCreate
                };

                try
                {
                    // Execute UpsertRequest and obtain UpsertResponse. 
                    UpsertResponse response = (UpsertResponse)_serviceProxy.Execute(request);
                    if (response.RecordCreated)
                        Console.WriteLine("New record {0} is created!", productName);
                    else
                        Console.WriteLine("Existing record {0} is updated!", productName);
                }

                // Catch any service fault exceptions that Microsoft Dynamics CRM throws.
                catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>)
                {
                    throw;
                }

            }
            // Prompts to view the sample_product entity records.
            // If you choose "y", IE will be launched to display the new or updated records.
            if (PromptForView())
            {
                ViewEntityListInBrowser();
            }

        }

        private static bool PromptForView()
        {
            Console.WriteLine("\nDo you want to view the sample product entity records? (y/n)");
            String answer = Console.ReadLine();
            if (answer.StartsWith("y") || answer.StartsWith("Y"))
            { return true; }
            else
            { return false; }
        }
        /// <summary>
        /// Alternate keys may not be active immediately after the UpsertSample 
        /// solution is installed.This method polls the metadata for the sample_product
        /// entity to delay execution of the rest of the sample until the alternate keys are ready.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        /// <param name="entityLogicalName">The entity logical name, i.e. sample_product.</param>
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

        // Displays the sample product entity records in the browser.
        public void ViewEntityListInBrowser()
        {

            try
            {
                // Get the view ID
                QueryByAttribute query = new QueryByAttribute("savedquery");
                query.AddAttributeValue("returnedtypecode", "sample_product");
                query.AddAttributeValue("name", "Active Sample Products");
                query.ColumnSet = new ColumnSet("savedqueryid", "name");
                query.AddOrder("name", OrderType.Ascending);
                RetrieveMultipleRequest req = new RetrieveMultipleRequest() { Query = query };
                RetrieveMultipleResponse resp = (RetrieveMultipleResponse)_serviceProxy.Execute(req);

                SavedQuery activeSampleProductsView = (SavedQuery)resp.EntityCollection[0];

                String webServiceURL = _serviceProxy.ServiceConfiguration.CurrentServiceEndpoint.Address.Uri.AbsoluteUri;
                String entityInDefaultSolutionUrl = webServiceURL.Replace("XRMServices/2011/Organization.svc",
                String.Format("main.aspx?etn={0}&pagetype=entitylist&viewid=%7b{1}%7d&viewtype=1039", "sample_product", activeSampleProductsView.SavedQueryId));

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
        private static void CleanUpSample(CrmServiceClient service)

        {

            //Delete the ChangeTrackingSample solution

            SampleHelpers.DeleteSolution(service, "ChangeTrackingSample");

        }


    }
    }
