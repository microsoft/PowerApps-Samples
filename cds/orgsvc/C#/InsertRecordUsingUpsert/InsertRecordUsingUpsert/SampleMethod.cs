using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Xml;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        private static bool prompt = true;
        /// <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        /// 
        private static void SetUpSample(CrmServiceClient service)
        {
            // Check that the current version is greater than the minimum version
            if (!SampleHelpers.CheckVersion(service, new Version("7.1.0.0")))
            {
                //The environment version is lower than version 7.1.0.0
                return;
            }
            //Import the ChangeTrackingSample solution
            if (SampleHelpers.ImportSolution(service, "UpsertSample", "UpsertSample_1_0_0_0_managed.zip"))
            {
                //Wait a minute if the solution is being imported. This will give time for the new metadata to be cached.
                Thread.Sleep(TimeSpan.FromSeconds(60));
            }

            //Verify that the alternate key indexes are ready
            if (!VerifyBookCodeKeyIsActive(service))
            {
                Console.WriteLine("There is a problem creating the index for the product code alternate key for the sample_product entity.");
                Console.WriteLine("The sample cannot continue. Please try again.");

                //Delete the ChangeTrackingSample solution
                SampleHelpers.DeleteSolution(service, "UpsertSample");
                return;
            }
        }

        /// <summary>
        /// Alternate keys may not be active immediately after a solution defining them is installed.
        /// This method polls the metadata for a specific entity
        /// to delay execution of the rest of the sample until the alternate keys are ready.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        /// <param name="asyncJob">The system job that creates the index to support the alternate key</param>
        /// <param name="iteration">The number of times this method has been called.</param>
        /// 
        private static bool VerifyBookCodeKeyIsActive(CrmServiceClient service, EntityReference asyncJob = null, int iteration = 0)
        {
            if (iteration > 5)
            {
                //Give up
                return false;
            }


            if (iteration == 0) //only the first time
            {
                //Get whether the Entity Key index is active from the metadata
                var entityQuery = new EntityQueryExpression();
                entityQuery.Criteria = new MetadataFilterExpression(LogicalOperator.And)
                {
                    Conditions = { { new MetadataConditionExpression("LogicalName", MetadataConditionOperator.Equals, "sample_product") } }
                };

                entityQuery.Properties = new MetadataPropertiesExpression("Keys");

                var metadataRequest = new RetrieveMetadataChangesRequest() { Query = entityQuery };
                var metadataResponse = (RetrieveMetadataChangesResponse)service.Execute(metadataRequest);
                var bookcodekey = metadataResponse.EntityMetadata.FirstOrDefault().Keys.FirstOrDefault();

                if (bookcodekey.EntityKeyIndexStatus == EntityKeyIndexStatus.Active)
                {
                    return true;
                }
                else
                {

                    iteration++;
                    return VerifyBookCodeKeyIsActive(service, bookcodekey.AsyncJob, iteration);
                }
            }
            else
            {
                //Check the status of the system job that is should indicate that the alternate key index is active.
                AsyncOperation systemJob = (AsyncOperation)service.Retrieve(asyncJob.LogicalName, asyncJob.Id, new ColumnSet("statecode", "statuscode"));

                if (systemJob.StateCode == AsyncOperationState.Completed) //Completed
                {

                    if (!systemJob.StatusCode.Value.Equals(30)) //Not Succeeded
                    {

                        //Delete the system job and try to reactivate
                        service.Delete(asyncJob.LogicalName, asyncJob.Id);

                        ReactivateEntityKeyRequest reactivateRequest = new ReactivateEntityKeyRequest()
                        {
                            EntityLogicalName = "sample_product",
                            EntityKeyLogicalName = "sample_productcode"
                        };
                        ReactivateEntityKeyResponse reactivateResponse = (ReactivateEntityKeyResponse)service.Execute(reactivateRequest);

                        //Get the system job created by the reactivate request
                        QueryByAttribute systemJobQuery = new QueryByAttribute("asyncoperation");
                        systemJobQuery.AddAttributeValue("primaryentitytype", "sample_product");
                        systemJobQuery.AddOrder("createdon", OrderType.Descending);
                        systemJobQuery.TopCount = 1;
                        systemJobQuery.ColumnSet = new ColumnSet("asyncoperationid", "name");

                        EntityCollection systemJobs = service.RetrieveMultiple(systemJobQuery);
                        asyncJob = systemJobs.Entities.FirstOrDefault().ToEntityReference();

                        iteration++;
                        return VerifyBookCodeKeyIsActive(service, asyncJob, iteration);
                    }
                    else
                    {
                        //It succeeded
                        return true;
                    }
                }
                else
                {
                    //Give it more time to complete
                    Thread.Sleep(TimeSpan.FromSeconds(30));
                    iteration++;
                    return VerifyBookCodeKeyIsActive(service, asyncJob, iteration);
                }

            }

        }

        public static void ProcessUpsert(CrmServiceClient service, String Filename)
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
                var request = new UpsertRequest()
                {
                    Target = productToCreate
                };

                try
                {
                    // Execute UpsertRequest and obtain UpsertResponse. 
                    var response = (UpsertResponse)service.Execute(request);
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


        }
        private static void CleanUpSample(CrmServiceClient service)
        {
            SampleHelpers.DeleteSolution(service, "UpsertSample");
        }
    }
}
