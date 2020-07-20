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
        private static BulkDeleteResponse _bulkDeleteResponse;
        private static Guid? _asyncOperationId;
        private static Guid? _bulkDeleteOperationId;
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

            //PerformBulkDeleteBackup(service);
        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, prompt);
        }

        // <summary>
        /// Performs the main operation of the sample - performs a bulk delete on inactive
        /// opportunities and activities to remove them from the system.
        /// </summary>
        private static void PerformBulkDeleteBackup(CrmServiceClient service)
        {
            try
            {
                // Query for a system user to send an email to after the bulk delete
                // operation completes.
                var userRequest = new WhoAmIRequest();
                var userResponse = (WhoAmIResponse)service.Execute(userRequest);
                Guid currentUserId = userResponse.UserId;

                // Create a condition for a bulk delete request.
                // NOTE: This sample uses very specific queries for deleting records
                // that have been manually exported in order to free space.
                QueryExpression opportunitiesQuery = BuildOpportunityQuery();

                // Create the bulk delete request.
                var bulkDeleteRequest = new BulkDeleteRequest();

                // Set the request properties.
                bulkDeleteRequest.JobName = "Backup Bulk Delete";

                // Querying activities
                bulkDeleteRequest.QuerySet = new QueryExpression[]
                {
                    opportunitiesQuery,
                    BuildActivityQuery(Task.EntityLogicalName),
                    BuildActivityQuery(Fax.EntityLogicalName),
                    BuildActivityQuery(PhoneCall.EntityLogicalName),
                    BuildActivityQuery(Email.EntityLogicalName),
                    BuildActivityQuery(Letter.EntityLogicalName),
                    BuildActivityQuery(Appointment.EntityLogicalName),
                    BuildActivityQuery(ServiceAppointment.EntityLogicalName),
                    BuildActivityQuery(CampaignResponse.EntityLogicalName),
                    BuildActivityQuery(RecurringAppointmentMaster.EntityLogicalName)
                };

                // Set the start time for the bulk delete.
                bulkDeleteRequest.StartDateTime = DateTime.Now;

                // Set the required recurrence pattern.
                bulkDeleteRequest.RecurrencePattern = String.Empty;

                // Set email activity properties.
                bulkDeleteRequest.SendEmailNotification = false;
                bulkDeleteRequest.ToRecipients = new Guid[] { currentUserId };
                bulkDeleteRequest.CCRecipients = new Guid[] { };

                // Submit the bulk delete job.
                // NOTE: Because this is an asynchronous operation, the response will be immediate.
                _bulkDeleteResponse =
                    (BulkDeleteResponse)service.Execute(bulkDeleteRequest);
                Console.WriteLine("The bulk delete operation has been requested.");

                CheckSuccess(service);
            }
            catch (System.Web.Services.Protocols.SoapException)
            {
                // Perform error handling here.
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// This method will query for the BulkDeleteOperation until it has been
        /// completed or until the designated time runs out.  It then checks to see if
        /// the operation was successful.
        /// </summary>
        private static void CheckSuccess(CrmServiceClient service)
        {
            // Query for bulk delete operation and check for status.
            var bulkQuery = new QueryByAttribute(
                BulkDeleteOperation.EntityLogicalName);
            bulkQuery.ColumnSet = new ColumnSet(true);

            // NOTE: When the bulk delete operation was submitted, the GUID that was
            // returned was the asyncoperationid, not the bulkdeleteoperationid.
            bulkQuery.Attributes.Add("asyncoperationid");
            _asyncOperationId = _bulkDeleteResponse.JobId;
            bulkQuery.Values.Add(_asyncOperationId);

            // With only the asyncoperationid at this point, a RetrieveMultiple is
            // required to get the
            // bulk delete operation created above.
            EntityCollection entityCollection = service.RetrieveMultiple(bulkQuery);
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
                    createdBulkDeleteOperation = (BulkDeleteOperation)entityCollection.Entities[0];

                    // Check the operation's state.
                    if (createdBulkDeleteOperation.StateCode.Value != BulkDeleteOperationState.Completed)
                    {
                        // The operation has not yet completed.  Wait a second for the
                        // status to change.
                        System.Threading.Thread.Sleep(1000);
                        secondsTicker--;

                        // Retrieve a fresh version of the bulk delete operation.
                        entityCollection = service.RetrieveMultiple(bulkQuery);
                    }
                    else
                    {
                        // Stop polling as the operation's state is now complete.
                        secondsTicker = 0;
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

            // Validate that the operation was completed.
            if (createdBulkDeleteOperation != null)
            {
                _bulkDeleteOperationId = createdBulkDeleteOperation.BulkDeleteOperationId;
                if (createdBulkDeleteOperation.StateCode.Value != BulkDeleteOperationState.Completed)
                {
                    Console.WriteLine(
                        "Polling for the BulkDeleteOperation took longer than allowed ({0} seconds).",
                        ARBITRARY_MAX_POLLING_TIME);
                }
                else
                {
                    Console.WriteLine("The BulkDeleteOperation succeeded.\r\n  Successes: {0}, Failures: {1}",
                        createdBulkDeleteOperation.SuccessCount,
                        createdBulkDeleteOperation.FailureCount);
                }
            }
            else
            {
                Console.WriteLine("The BulkDeleteOperation could not be retrieved.");
            }
        }

        /// <summary>
        /// Builds a query that matches all opportunities that are not in the open state.
        /// </summary>
        private static QueryExpression BuildOpportunityQuery()
        {
            // Create a query that will match all opportunities that do not have a state
            // of open.
            var closedCondition = new ConditionExpression(
                "statecode", ConditionOperator.NotEqual, (int)OpportunityState.Open);

            // Create a filter expression for a bulk delete request.
            var closedFilter = new FilterExpression();
            closedFilter.Conditions.Add(closedCondition);

            var queryExpression = new QueryExpression();

            queryExpression.EntityName = Opportunity.EntityLogicalName;
            queryExpression.Criteria = closedFilter;

            // Return all records
            queryExpression.Distinct = false;

            return queryExpression;
        }

        /// <summary>
        /// Builds a query which will match all activities that are in the canceled or
        /// completed state.
        /// </summary>
        private static QueryExpression BuildActivityQuery( String entityName)
        {
            var canceledCondition = new ConditionExpression(
                "statecode", ConditionOperator.Equal, (int)ActivityPointerState.Canceled);
            var completedCondition = new ConditionExpression(
                "statecode", ConditionOperator.Equal, (int)ActivityPointerState.Completed);

            var closedFilter = new FilterExpression(LogicalOperator.Or);
            closedFilter.Conditions.AddRange(canceledCondition, completedCondition);

            var queryExpression = new QueryExpression();

            queryExpression.EntityName = entityName;
            queryExpression.Criteria = closedFilter;

            queryExpression.Distinct = false;

            return queryExpression;
        }
        /// <summary>
        /// This method deletes the AsyncOperation and BulkDeleteOperation that were
        /// created in the database, if the user confirms that deleting these is
        /// desired.
        /// </summary>
        private static void DeleteRequiredRecords(CrmServiceClient service, bool promptToDelete)
        {
            var toBeDeleted = true;
            if (promptToDelete)
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
                if (_bulkDeleteOperationId.HasValue)
                {
                    service.Delete(
                        BulkDeleteOperation.EntityLogicalName,
                        _bulkDeleteOperationId.Value);
                }

                if (_asyncOperationId.HasValue)
                {
                    service.Delete(
                        AsyncOperation.EntityLogicalName, _asyncOperationId.Value);
                }

                Console.WriteLine("The AsyncOperation and BulkDeleteOperation have been deleted.");
            }
        }

    }
}
