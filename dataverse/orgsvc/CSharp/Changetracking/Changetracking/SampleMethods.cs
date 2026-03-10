using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;

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
        private static bool VerifyBookCodeKeyIsActive(IOrganizationService service, EntityReference asyncJob = null, int iteration = 0)
        {
            if (iteration > 5)
            {
                //Give up
                return false;
            }


            if (iteration == 0) //only the first time
            {
                //Get whether the Entity Key index is active from the metadata
                EntityQueryExpression entityQuery = new EntityQueryExpression();
                entityQuery.Criteria = new MetadataFilterExpression(LogicalOperator.And)
                {
                    Conditions = { { new MetadataConditionExpression("LogicalName", MetadataConditionOperator.Equals, "sample_book") } }
                };

                entityQuery.Properties = new MetadataPropertiesExpression("Keys");

                RetrieveMetadataChangesRequest metadataRequest = new RetrieveMetadataChangesRequest() { Query = entityQuery };
                RetrieveMetadataChangesResponse metadataResponse = (RetrieveMetadataChangesResponse)service.Execute(metadataRequest);
                EntityKeyMetadata bookcodekey = metadataResponse.EntityMetadata.FirstOrDefault().Keys.FirstOrDefault();

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
                            EntityLogicalName = "sample_book",
                            EntityKeyLogicalName = "sample_bookcode"
                        };
                        ReactivateEntityKeyResponse reactivateResponse = (ReactivateEntityKeyResponse)service.Execute(reactivateRequest);

                        //Get the system job created by the reactivate request
                        QueryByAttribute systemJobQuery = new QueryByAttribute("asyncoperation");
                        systemJobQuery.AddAttributeValue("primaryentitytype", "sample_book");
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

        /// <summary>
        /// Creates the inital set of book records in the sample
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        private static void CreateInitialBookRecordsForSample(IOrganizationService service)
        {
            int recordsCreated = 0;

            Console.WriteLine("Creating required records......");
            // Create 10 book records for demo.
            for (int i = 0; i < 10; i++)
            {
                Entity book = new Entity("sample_book");
                book["sample_name"] = "Demo Book " + i.ToString();
                book["sample_bookcode"] = "BookCode" + i.ToString();
                book["sample_author"] = "Author" + i.ToString();

                try
                {
                    service.Create(book);
                    recordsCreated++;
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    //Record with sample_bookcode alternate key value already exists.
                    //So it is fine that we don't re-create it.
                    if (!(ex.Detail.ErrorCode == -2147088238))
                    {
                        throw;
                    }
                }
            }
            Console.WriteLine("{0} records created...", recordsCreated);

        }
        /// <summary>
        /// Updates the set of records used in the sample
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        private static void UpdateBookRecordsForSample(IOrganizationService service)
        {
            int recordsCreated = 0;

            Console.WriteLine("Adding 10 more records");
            // Create 10 book records for demo.
            for (int i = 10; i < 20; i++)
            {
                Entity book = new Entity("sample_book");
                book["sample_name"] = "Demo Book " + i.ToString();
                book["sample_bookcode"] = "BookCode" + i.ToString();
                book["sample_author"] = "Author" + i.ToString();

                try
                {
                    service.Create(book);
                    recordsCreated++;
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    //Record with sample_bookcode alternate key value already exists.
                    //So it is fine that we don't re-create it.
                    if (!(ex.Detail.ErrorCode == -2147088238))
                    {
                        throw;
                    }
                }
            }
            Console.WriteLine("{0} records created...", recordsCreated);

            // Update a record.
            Console.WriteLine("Updating one of the initial records.");
            //Use the alternate key to reference the entity;
            Entity demoBookZero = new Entity("sample_book", "sample_bookcode", "BookCode0");
            demoBookZero["sample_name"] = string.Format("Demo Book 0 updated {0}", DateTime.Now.ToShortTimeString());

            service.Update(demoBookZero);

            //Delete a record
            Console.WriteLine("Deleting one of the initial records.");

            //Use a KeyAttributeCollection to set the alternate key for the record.
            KeyAttributeCollection demoBookOneKeys = new KeyAttributeCollection();
            demoBookOneKeys.Add(new KeyValuePair<string, object>("sample_bookcode", "BookCode1"));

            EntityReference bookOne = new EntityReference()
            {
                LogicalName = "sample_book",
                KeyAttributes = demoBookOneKeys
            };
            DeleteRequest deleteReq = new DeleteRequest() { Target = bookOne };
            service.Execute(deleteReq);
        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            //Delete the ChangeTrackingSample solution
            SampleHelpers.DeleteSolution(service, "ChangeTrackingSample");
        }

    }

}
