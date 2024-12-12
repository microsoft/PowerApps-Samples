using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        private static Guid _bulkDeleteOperationId;
        private static Guid _asyncOperationId;
        private static bool prompt = true;
        private static ServiceContext _context;
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

            CreateRequiredRecords(service);
        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRecords(service, prompt);
        }

        /// <summary>
        /// Create an account that will be deleted in the main portion of the sample.
        /// </summary>
        private static void CreateRequiredRecords(CrmServiceClient service)
        {
            var account = new Account
            {
                Name = "Fourth Coffee",
                WebSiteURL = "http://www.fourthcoffee.com/"
            };
            service.Create(account);
        }

        /// <summary>
        /// Perform the main action of the sample - issuing a BulkDeleteRequest.
        /// </summary>
        /// <param name="useRecurrence">
        /// whether or not to create a recurring BulkDeleteRequest.
        /// </param>
        private static void PerformBulkDelete(CrmServiceClient service, bool useRecurrence, bool promptToDelete)
        {


            Console.WriteLine("Performing Bulk Delete Operation");

            // Query for a system user to send an email to after the bulk delete
            // operation completes.
            var userRequest = new WhoAmIRequest();
            var userResponse = (WhoAmIResponse)service.Execute(userRequest);
            Guid currentUserId = userResponse.UserId;

            Console.WriteLine("  Requesting user retrieved.");

            // Create a condition for a bulk delete request.
            // NOTE: If no records are found that match this condition, the bulk delete
            // will not fail.  It will succeed with 0 successes and 0 failures.
            var deleteCondition = new ConditionExpression(
                "name", ConditionOperator.Equal, "Fourth Coffee");

            // Create a fiter expression for the bulk delete request.
            var deleteFilter = new FilterExpression();
            deleteFilter.Conditions.Add(deleteCondition);

            // Create the bulk delete query set.
            var bulkDeleteQuery = new QueryExpression
            {
                EntityName = Account.EntityLogicalName,
                Distinct = false,
                Criteria = deleteFilter
            };

            // Create the bulk delete request.
            var bulkDeleteRequest = new BulkDeleteRequest
            {
                JobName = "Sample Bulk Delete",
                QuerySet = new[] { bulkDeleteQuery },
                StartDateTime = DateTime.Now,
                ToRecipients = new[] { currentUserId },
                CCRecipients = new Guid[] { },
                SendEmailNotification = true,
                RecurrencePattern = String.Empty

            };

            // Create a recurring BulkDeleteOperation.
            if (useRecurrence)
            {
                bulkDeleteRequest.RecurrencePattern = "FREQ=DAILY;INTERVAL=1;";
            }

            // Submit the bulk delete job.
            // NOTE: Because this is an asynchronous operation, the response will be
            // immediate.
            var bulkDeleteResponse =
                (BulkDeleteResponse)service.Execute(bulkDeleteRequest);
            _asyncOperationId = bulkDeleteResponse.JobId;

            Console.WriteLine("  The Bulk Delete Request was made and the Bulk\n" +
                              "    Delete Operation should be created.");

            // To monitor the asynchronous operation, retrieve the
            // bulkdeleteoperation object.
            // NOTE: There will be a period of time from when the async operation
            // request was submitted to the time when a successful query for that
            // async operation can be made.  When using plug-ins, events can be
            // subscribed to that will fire when the async operation status changes.
            var bulkQuery = new QueryByAttribute();
            bulkQuery.ColumnSet = new ColumnSet(true);
            bulkQuery.EntityName = BulkDeleteOperation.EntityLogicalName;

            // NOTE: When the bulk delete operation was submitted, the GUID that was
            // returned was the asyncoperationid, not the bulkdeleteoperationid.
            bulkQuery.Attributes.Add("asyncoperationid");
            bulkQuery.Values.Add(bulkDeleteResponse.JobId);

            // With only the asyncoperationid at this point, a RetrieveMultiple is
            // required to get the bulk delete operation created above.
            var entityCollection =
                service.RetrieveMultiple(bulkQuery);
            BulkDeleteOperation createdBulkDeleteOperation = null;

            // When creating a recurring BulkDeleteOperation, the BulkDeleteOperation
            // will be in suspended status after the current instance has completed.
            // When creating a non-recurring BulkDeleteOperation, it will be in
            // Completed status when it is finished.
            var bulkOperationEnded = useRecurrence
                ? BulkDeleteOperationState.Suspended
                : BulkDeleteOperationState.Completed;

            createdBulkDeleteOperation = RetrieveBulkDeleteOperation(service,
                bulkQuery, entityCollection, bulkOperationEnded);
            _bulkDeleteOperationId = createdBulkDeleteOperation.Id;

            if (createdBulkDeleteOperation != null)
            {
                // If the BulkDeleteOperation is recurring, the status will be
                // "Waiting" after the operation completes this instance.  If it is
                // non-recurring, the status will be "Succeeded".
                var bulkOperationSuccess = useRecurrence
                    ? bulkdeleteoperation_statuscode.Waiting
                    : bulkdeleteoperation_statuscode.Succeeded;

                InspectBulkDeleteOperation(service,createdBulkDeleteOperation,
                    bulkOperationEnded, bulkOperationSuccess, useRecurrence);

                DeleteRecords(service, promptToDelete);
            }
            else
            {
                Console.WriteLine("  The Bulk Delete Operation could not be retrieved.");
            }

        }

        /// <summary>
        /// Inspect and display information about the created BulkDeleteOperation.
        /// </summary>
        /// <param name="createdBulkDeleteOperation">
        /// the BulkDeleteOperation to inspect.
        /// </param>
        /// <param name="bulkOperationEnded">
        /// the statecode that will tell us if the BulkDeleteOperation is ended.
        /// </param>
        /// <param name="bulkOperationSuccess">
        /// the statuscode that will tell us if the BulkDeleteOperation was successful.
        /// </param>
        /// <param name="useRecurrence">
        /// whether or not the BulkDeleteOperation is a recurring operation.
        /// </param>
        private static void InspectBulkDeleteOperation(CrmServiceClient service,
            BulkDeleteOperation createdBulkDeleteOperation,
            BulkDeleteOperationState bulkOperationEnded,
            bulkdeleteoperation_statuscode bulkOperationSuccess,
            bool useRecurrence)
        {
            // Validate that the operation was completed.
            if (createdBulkDeleteOperation.StateCode != bulkOperationEnded)
            {
                // This will happen if it took longer than the polling time allowed 
                // for this operation to complete.
                Console.WriteLine("  Completion of the Bulk Delete took longer\n" +
                                  "    than the polling time allotted.");
            }
            else if (createdBulkDeleteOperation.StatusCode.Value
                     != (int)bulkOperationSuccess)
            {
                Console.WriteLine("  The Bulk Delete operation failed.");
            }
            else if (!useRecurrence)
            {
                // Check for the number of successful deletes.
                var successfulDeletes = createdBulkDeleteOperation.SuccessCount ?? 0;
                Console.WriteLine("  {0} records were successfully deleted",
                                  successfulDeletes);

                // Check for any failures that may have occurred during the bulk
                // delete operation.
                if (createdBulkDeleteOperation.FailureCount > 0)
                {
                    Console.WriteLine("  {0} records failed to be deleted:",
                                      createdBulkDeleteOperation.FailureCount);

                    // Query for all the failures.
                    var failureQuery = new QueryByAttribute();
                    failureQuery.ColumnSet = new ColumnSet(true);
                    failureQuery.EntityName = BulkDeleteFailure.EntityLogicalName;
                    failureQuery.Attributes.Add("bulkdeleteoperationid");
                    var bulkDeleteOperationId =
                        createdBulkDeleteOperation.BulkDeleteOperationId ?? Guid.Empty;
                    failureQuery.Values.Add(bulkDeleteOperationId);

                    // Retrieve the bulkdeletefailure objects.
                    EntityCollection entityCollection = service.RetrieveMultiple(
                        failureQuery);

                    // Examine each failure for information regarding the failure.
                    foreach (BulkDeleteFailure failureOperation in
                        entityCollection.Entities)
                    {
                        // Process failure information.
                        Console.WriteLine(String.Format(
                            "    {0}, {1}",
                            failureOperation.RegardingObjectId.Name,
                            failureOperation.RegardingObjectId.Id));
                    }
                }
            }
            else
            {
                // NOTE: If recurrence is used, we cannot reliably retrieve data
                // about the records that were deleted, since a sub-BulkDeleteOperation
                // is created by Microsoft Dynamics CRM that does not have any fields tying it back to the
                // Asynchronous operation or the BulkDeleteOperation. This makes it
                // unreliable to know which subprocess to retrieve.
                Console.WriteLine("  The recurring Bulk Delete Operation was created successfully.");
            }
        }

        /// <summary>
        /// Retrieves the BulkDeleteOperation, but it's not necessarily created
        /// immediately, so this method uses polling.
        /// </summary>
        /// <param name="bulkQuery">the query to find the BulkDeleteOperation.</param>
        /// <param name="entityCollection">the initial results of the query.</param>
        /// <param name="operationEndedStatus">
        /// the statecode that will indicate that the operation has ended.
        /// </param>
        private static BulkDeleteOperation RetrieveBulkDeleteOperation(CrmServiceClient service,
            QueryByAttribute bulkQuery, EntityCollection entityCollection,
            BulkDeleteOperationState operationEndedStatus)
        {
            BulkDeleteOperation createdBulkDeleteOperation = null;
            // Monitor the async operation via polling until it is complete or max
            // polling time expires.
            const int ARBITRARY_MAX_POLLING_TIME = 60;
            int secondsTicker = ARBITRARY_MAX_POLLING_TIME;
            while (secondsTicker > 0)
            {
                // Make sure the async operation was retrieved.
                if (entityCollection.Entities.Count > 0)
                {
                    // Grab the one bulk operation that has been created.
                    createdBulkDeleteOperation =
                        (BulkDeleteOperation)entityCollection.Entities[0];

                    // Check the operation's state.
                    // NOTE: If a recurrence for the BulkDeleteOperation was
                    // specified, the state of the operation will be Suspended,
                    // not Completed, since the operation will run again in the
                    // future.
                    if (createdBulkDeleteOperation.StateCode !=
                        operationEndedStatus)
                    {
                        // The operation has not yet completed.  Wait a second for
                        // the status to change.
                        System.Threading.Thread.Sleep(1000);
                        secondsTicker--;

                        // Retrieve a fresh version of the bulk delete operation.
                        entityCollection = service.RetrieveMultiple(bulkQuery);
                    }
                    else
                    {
                        // Stop polling as the operation's state is now complete.
                        secondsTicker = 0;
                        Console.WriteLine(
                            "  The BulkDeleteOperation record has been retrieved.");
                    }
                }
                else
                {
                    // Wait a second for async operation to activate.
                    System.Threading.Thread.Sleep(1000);
                    secondsTicker--;

                    // Retrieve the entity again.
                    entityCollection = service.RetrieveMultiple(bulkQuery);
                }
            }
            return createdBulkDeleteOperation;
        }

        /// <summary>
        /// Deletes records that were created in the sample.
        /// </summary>
        /// <param name="prompt">whether or not to prompt the user for deletion.</param>
        private static void DeleteRecords(CrmServiceClient service,bool prompt)
        {
            var toBeDeleted = true;
            if (prompt)
            {
                // Ask the user if the created entities should be deleted.
                Console.Write("\nDo you want these entity records deleted? (y/n) [y]: ");
                String answer = Console.ReadLine();
                if (answer.StartsWith("y") ||
                    answer.StartsWith("Y") ||
                    answer == String.Empty)
                {
                    toBeDeleted = true;
                }
                else
                {
                    toBeDeleted = false;
                }
            }

            if (toBeDeleted)
            {
                // Delete the bulk delete operation so that it won't clutter the
                // database.
                service.Delete(
                    BulkDeleteOperation.EntityLogicalName, _bulkDeleteOperationId);

                var asyncOperationEntity = service.Retrieve(
                    AsyncOperation.EntityLogicalName,
                    _asyncOperationId,
                    new ColumnSet("statecode", "asyncoperationid"));
                var asyncOperation = asyncOperationEntity.ToEntity<AsyncOperation>();

                if (asyncOperation.StateCode != AsyncOperationState.Completed)
                {
                    // We have to update the AsyncOperation to be in a Completed state
                    // before we can delete it.
                    asyncOperation.StateCode = AsyncOperationState.Completed;
                    service.Update(asyncOperation);
                }

                service.Delete(
                    AsyncOperation.EntityLogicalName, _asyncOperationId);

                Console.WriteLine("  The AsyncOperation and BulkDeleteOperation have been deleted.");
            }
        }

    }
}
            