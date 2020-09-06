using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    class Program
    {
        //Get configuration data from App.config connectionStrings
        static readonly string connectionString = ConfigurationManager.ConnectionStrings["Connect"].ConnectionString;
        static readonly ServiceConfig serviceConfig = new ServiceConfig(connectionString);
        //Controls the max degree of parallelism
        static readonly int maxDegreeOfParallelism = 10;
        //How many records to create with this sample.
        static readonly int numberOfRecords = 100;

        static void Main()
        {
            #region Optimize Connection

            //Change max connections from .NET to a remote service default: 2
            System.Net.ServicePointManager.DefaultConnectionLimit = 65000;
            //Bump up the min threads reserved for this app to ramp connections faster - minWorkerThreads defaults to 4, minIOCP defaults to 4 
            System.Threading.ThreadPool.SetMinThreads(100, 100);
            //Turn off the Expect 100 to continue message - 'true' will cause the caller to wait until it round-trip confirms a connection to the server 
            System.Net.ServicePointManager.Expect100Continue = false;
            //Can decreas overall transmission overhead but can cause delay in data packet arrival
            System.Net.ServicePointManager.UseNagleAlgorithm = false;

            #endregion Optimize Connection

            var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = maxDegreeOfParallelism };

            var count = 0;

            //Will be populated with account records to import
            List<JObject> accountsToImport = new List<JObject>();
            //ConcurrentBag is a thread-safe, unordered collection of objects.
            ConcurrentBag<Uri> accountsToDelete = new ConcurrentBag<Uri>();
            Console.WriteLine($"Preparing to create {numberOfRecords} acccount records using Web API.");

            //Add account records to the list to import
            while (count < numberOfRecords)
            {
                var account = new JObject
                {
                    ["name"] = $"Account {count}"
                };
                accountsToImport.Add(account);
                count++;
            }


            try
            {
               
                using (CDSWebApiService svc = new CDSWebApiService(serviceConfig))
                {
                    Console.WriteLine($"Creating {accountsToImport.Count} accounts");
                    var startCreate = DateTime.Now;

                    //Create the accounts in parallel
                    Parallel.ForEach(accountsToImport, parallelOptions, (account) =>

                    {
                        //Add the Uri returned to the ConcurrentBag to delete later
                        accountsToDelete.Add(svc.PostCreate("accounts", account));

                    });

                    //Calculate the duration to complete
                    var secondsToCreate = (DateTime.Now - startCreate).TotalSeconds;

                    Console.WriteLine($"Created {accountsToImport.Count} accounts in  {Math.Round(secondsToCreate)} seconds.");


                    Console.WriteLine($"Deleting {accountsToDelete.Count} accounts");
                    var startDelete = DateTime.Now;

                    //Delete the accounts in parallel
                    Parallel.ForEach(accountsToDelete, parallelOptions, (uri) =>
                    {
                        svc.Delete(uri.ToString());
                    });

                    //Calculate the duration to complete
                    var secondsToDelete = (DateTime.Now - startDelete).TotalSeconds;

                    Console.WriteLine($"Deleted {accountsToDelete.Count} accounts in {Math.Round(secondsToDelete)} seconds.");
                    Console.WriteLine("Sample completed. Press any key to exit.");
                    Console.ReadLine();
                }
             }
			catch (Exception)
			{

				throw;
			}
        }
    }
}
