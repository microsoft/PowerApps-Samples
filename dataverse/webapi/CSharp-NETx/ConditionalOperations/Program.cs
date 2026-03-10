using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerApps.Samples;
using PowerApps.Samples.Batch;
using PowerApps.Samples.Messages;
using PowerApps.Samples.Methods;

namespace ConditionalOperations
{
    internal class Program
    {
        static async Task Main()
        {
            Config config = App.InitializeApp();

            var service = new Service(config);

            List<EntityReference> recordsToDelete = new();
            bool deleteCreatedRecords = true;
            string queryOptions = "?$select=name,revenue,telephone1,description";

            Console.WriteLine("--Starting Conditional Operations sample--");

            #region Section 0: Create sample records

            Console.WriteLine("\n--Section 0 started--");

            // Create an account record
            var account1 = new JObject {
                        { "name", "Contoso Ltd" },
                        { "telephone1", "555-0000" }, //Phone number value increments with each update attempt
                        { "revenue", 5000000},
                        { "description", "Parent company of Contoso Pharmaceuticals, etc."} };

            // Create and retrieve the record
            JObject retrievedaccount1 = await service.CreateRetrieve(
                entitySetName: "accounts",
                record: account1,
                query: queryOptions);

            EntityReference account1Ref = new(
                    entitySetName: "accounts",
                    id: (Guid)retrievedaccount1["accountid"]);

            recordsToDelete.Add(account1Ref); //To delete later


            // Store the ETag value from the retrieved record
            string initialAcctETagVal = retrievedaccount1["@odata.etag"].ToString();

            Console.WriteLine("Created and retrieved the initial account, shown below:");
            Console.WriteLine(retrievedaccount1.ToString(Formatting.Indented));

            #endregion Section 0: Create sample records  

            #region Section 1: Conditional GET

            Console.WriteLine("\n--Section 1 started--");

            Console.WriteLine("\n** Conditional GET demonstration **");

            // Attempt to retrieve the account record using a conditional GET defined by a message header with
            // the current ETag value.
            try
            {
                //Using RetrieveRequest
                RetrieveRequest retrieveAccount1Request = new(
                    entityReference: account1Ref,
                    query: queryOptions,
                    eTag: initialAcctETagVal);

                // This should throw an exception:
                var retrieveAccount1Response =
                    await service.SendAsync<RetrieveResponse>(retrieveAccount1Request);


                // Not expected; the returned response contains content.
                Console.WriteLine("Instance retrieved using ETag: {0}", initialAcctETagVal);
                Console.WriteLine(retrievedaccount1.ToString(Formatting.Indented));
            }
            catch (ServiceException e)
            {
                if (e.HttpStatusCode == System.Net.HttpStatusCode.NotModified) // Expected result
                {
                    Console.WriteLine("Account record retrieved using ETag: {0}", initialAcctETagVal);
                    Console.WriteLine("Expected outcome: Entity was not modified so nothing was returned.");
                }
                else { throw e; }
            }

            // Modify the account instance by updating the telephone1 column value
            await service.SetColumnValue(
                entityReference: account1Ref,
                propertyName: "telephone1",
                value: "555-0001");

            // Re-attempt to retrieve using conditional GET defined by a message header with
            // the current ETag value.
            try
            {
                //Using Retrieve Method
                retrievedaccount1 = await service.Retrieve(
                    entityReference: account1Ref,
                    query: queryOptions,
                    includeAnnotations: false,
                    eTag: initialAcctETagVal);

                // Expected result because exception didn't occur:
                Console.WriteLine("Modified account record retrieved using ETag: {0}", initialAcctETagVal);
                Console.WriteLine("Notice the updated ETag value and telephone number");

            }
            catch (ServiceException e)
            {
                if (e.HttpStatusCode == System.Net.HttpStatusCode.NotModified) // Not expected
                {
                    Console.WriteLine("Unexpected outcome: Entity was modified so something should be returned.");
                }
                else { throw e; }
            }

            // Save the updated ETag value
            var updatedAcctETagVal = retrievedaccount1["@odata.etag"].ToString();

            // Show the updated record
            Console.WriteLine(retrievedaccount1.ToString(Formatting.Indented));

            #endregion Section 1: Conditional GET

            #region Section 2: Optimistic concurrency on delete and update

            Console.WriteLine("\n--Section 2 started--");

            Console.WriteLine("\n** Optimistic concurrency demonstration **");

            // Attempt to delete original account (if matches original ETag value).
            // If you replace "initialAcctETagVal" with "updatedAcctETagVal", the delete will
            // succeed. However, we want the delete to fail for now to demonstrate use of the ETag.
            Console.WriteLine("Attempting to delete the account using the original ETag value");

            try
            {
                //Using DeleteRequest
                DeleteRequest deleteAccount1Request = new(
                    entityReference: account1Ref,
                    eTag: initialAcctETagVal);

                // This should throw an exception:
                var deleteAccount1Response =
                    await service.SendAsync(deleteAccount1Request);

                // Not expected; this code should not execute.
                Console.WriteLine("Account deleted");
            }
            catch (ServiceException e)
            {
                if (e.HttpStatusCode == System.Net.HttpStatusCode.PreconditionFailed) // Expected result
                {
                    Console.WriteLine($"Expected Error: {e.Message}");
                    Console.WriteLine($"\tAccount not deleted using ETag '{initialAcctETagVal}', status code: '{e.HttpStatusCode}'.");
                }
                else { throw e; }
            }

            Console.WriteLine("Attempting to update the account using the original ETag value");

            JObject accountUpdate = new() {
                        { "telephone1", "555-0002" },
                        { "revenue", 6000000 }
                    };

            try
            {
                UpdateRequest updateAccount1Request = new(
                    entityReference: account1Ref,
                    record: accountUpdate,
                    eTag: initialAcctETagVal);

                // This should throw an exception:
                await service.SendAsync(updateAccount1Request);

                // Not expected; this code should not execute.
                Console.WriteLine("Account updated using original ETag {0}", initialAcctETagVal);
            }
            catch (ServiceException e)
            {
                if (e.HttpStatusCode == System.Net.HttpStatusCode.PreconditionFailed) // Expected error
                {
                    Console.WriteLine($"Expected Error: {e.Message}");
                    Console.WriteLine($"\tAccount not updated using ETag '{initialAcctETagVal}', " +
                        $"status code: '{e.HttpStatusCode}'.");
                }
                else { throw e; }
            }

            // Reattempt update if matches current ETag value.
            accountUpdate["telephone1"] = "555-0003";
            Console.WriteLine("Attempting to update the account using the current ETag value");

            try
            {
                await service.Update(
                    entityReference: account1Ref,
                    record: accountUpdate,
                    eTag: updatedAcctETagVal); //Using newer ETag this time

                // Expected program flow; this code should execute.
                Console.WriteLine($"\nAccount successfully updated using ETag: {updatedAcctETagVal}.");
            }
            catch (ServiceException e)
            {
                if (e.HttpStatusCode == System.Net.HttpStatusCode.PreconditionFailed) // Not expected
                {
                    Console.WriteLine($"Unexpected status code: '{e.HttpStatusCode}'");
                }
                else { throw e; }
            }

            // Retrieve and output current account state.
            retrievedaccount1 = await service.Retrieve(
                entityReference: account1Ref,
                query: queryOptions);

            Console.WriteLine("\nBelow is the final state of the account");
            Console.WriteLine(retrievedaccount1.ToString(Formatting.Indented));

            #endregion Section 2: Optimistic concurrency on delete and update

            #region Section 3: Delete sample records

            Console.WriteLine("\n--Section 3 started--");

            // Delete all the created sample records.  

            if (!deleteCreatedRecords)
            {
                Console.Write("\nDo you want these entity records deleted? (y/n) [y]: ");
                string answer = Console.ReadLine();
                answer = answer.Trim();
                if (!(answer.StartsWith("y") || answer.StartsWith("Y") || answer == string.Empty))
                { recordsToDelete.Clear(); }
                else
                {
                    Console.WriteLine("\nDeleting created records.");
                }
            }
            else
            {
                Console.WriteLine("\nDeleting created records.");
            }

            List<HttpRequestMessage> deleteRequests = new();

            foreach (EntityReference recordToDelete in recordsToDelete)
            {
                deleteRequests.Add(new DeleteRequest(recordToDelete));
            }

            BatchRequest batchRequest = new(service.BaseAddress)
            {
                Requests = deleteRequests
            };

            await service.SendAsync(batchRequest);


            #endregion Section 3: Delete sample records

            Console.WriteLine("--Conditional Operations sample complete--");
        }
    }
}