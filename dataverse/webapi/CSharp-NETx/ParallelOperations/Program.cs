using Newtonsoft.Json.Linq;
using PowerApps.Samples;
using PowerApps.Samples.Messages;
using System.Collections.Concurrent;

namespace ParallelOperations
{
    internal class Program
    {
        // How many records to create and delete with this sample.
        static readonly int numberOfRecords = 100;

        static async Task Main()
        {
            Config config = App.InitializeApp();

            var service = new Service(config);

            #region Optimize Connection

            // Change max connections from .NET to a remote service default: 2
            System.Net.ServicePointManager.DefaultConnectionLimit = 65000;
            // Bump up the min threads reserved for this app to ramp connections faster - minWorkerThreads defaults to 4, minIOCP defaults to 4 
            ThreadPool.SetMinThreads(100, 100);
            // Turn off the Expect 100 to continue message - 'true' will cause the caller to wait until it round-trip confirms a connection to the server 
            System.Net.ServicePointManager.Expect100Continue = false;
            // Can decrease overall transmission overhead but can cause delay in data packet arrival
            System.Net.ServicePointManager.UseNagleAlgorithm = false;

            #endregion Optimize Connection

            Console.WriteLine("--Starting Parallel Operations sample--");

            // Send a simple request to access the recommended degree of parallelism (DOP).
            HttpResponseMessage whoAmIResponse = await service.SendAsync(new WhoAmIRequest());
            int recommendedDegreeOfParallelism = int.Parse(whoAmIResponse.Headers.GetValues("x-ms-dop-hint").FirstOrDefault());

            Console.WriteLine($"Recommended degree of parallelism for this environment is {recommendedDegreeOfParallelism}.");

            var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = recommendedDegreeOfParallelism };

            var count = 0;

            // Will be populated with requests to create account records
            List<CreateRequest> accountsToImport = new();

            // ConcurrentBag is a thread-safe, unordered collection of objects.
            ConcurrentBag<DeleteRequest> accountsToDelete = new();

            Console.WriteLine($"Preparing to create {numberOfRecords} acccount records using Web API.");

            // Add account create requests to accountsToImport
            while (count < numberOfRecords)
            {
                var account = new JObject
                {
                    ["name"] = $"Account {count}"
                };
                accountsToImport.Add(new CreateRequest("accounts", account));
                count++;
            }

            try
            {
                Console.WriteLine($"Creating {accountsToImport.Count} accounts");
                var startCreate = DateTime.Now;

                // Send the requests in parallel
                await Parallel.ForEachAsync(accountsToImport, parallelOptions, async (account, token) =>
                  {
                      var createResponse = await service.SendAsync<CreateResponse>(account);

                      // Add the delete request to the ConcurrentBag to delete later
                      accountsToDelete.Add(new DeleteRequest(createResponse.EntityReference));
                  });

                // Calculate the duration to complete
                var secondsToCreate = (DateTime.Now - startCreate).TotalSeconds;

                Console.WriteLine($"Created {accountsToImport.Count} accounts in  {Math.Round(secondsToCreate)} seconds.");


                Console.WriteLine($"Deleting {accountsToDelete.Count} accounts");
                var startDelete = DateTime.Now;

                // Delete the accounts in parallel
                await Parallel.ForEachAsync(accountsToDelete, parallelOptions, async (deleteRequest, token) =>
                 {
                     await service.SendAsync(deleteRequest);
                 });

                // Calculate the duration to complete
                var secondsToDelete = (DateTime.Now - startDelete).TotalSeconds;

                Console.WriteLine($"Deleted {accountsToDelete.Count} accounts in {Math.Round(secondsToDelete)} seconds.");

            }
            catch (Exception)
            {
                throw;
            }
            Console.WriteLine("--Parallel Operations complete--");
        }
    }
}