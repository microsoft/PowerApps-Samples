using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

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


        static async Task Main()
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
            var executionDataflowBlockOptions = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism
            };

            var count = 0;
            double secondsToComplete;

            //Will be populated with account records to import
            List<JObject> accountsToImport = new List<JObject>();

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

            using (var svc = new CDSWebApiService(serviceConfig))
            {
                secondsToComplete = await ProcessData(svc, accountsToImport, executionDataflowBlockOptions);
            }

            Console.WriteLine($"Created and deleted {accountsToImport.Count} accounts in  {Math.Round(secondsToComplete)} seconds.");

            Console.WriteLine("Sample completed. Press any key to exit.");
            Console.ReadLine();
        }

        static async Task<double> ProcessData(CDSWebApiService svc, List<JObject> accountsToImport,
            ExecutionDataflowBlockOptions executionDataflowBlockOptions)
        {


            var createAccounts = new TransformBlock<JObject, Uri>(
                async a =>
                {

                    return await svc.PostCreateAsync("accounts", a);

                },
                    executionDataflowBlockOptions
                );

            var deleteAccounts = new ActionBlock<Uri>(
                async u =>
                {
                    await svc.DeleteAsync(u.ToString());
                },
                executionDataflowBlockOptions
              );

            createAccounts.LinkTo(deleteAccounts, new DataflowLinkOptions { PropagateCompletion = true });

            var start = DateTime.Now;

            accountsToImport.ForEach(a => createAccounts.SendAsync(a));
            createAccounts.Complete();
            await deleteAccounts.Completion;

            //Calculate the duration to complete
            return (DateTime.Now - start).TotalSeconds;


        }
    }
}
