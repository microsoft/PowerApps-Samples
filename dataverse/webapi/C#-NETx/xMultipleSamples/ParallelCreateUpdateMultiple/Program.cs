using Newtonsoft.Json.Linq;
using PowerApps.Samples;
using PowerApps.Samples.Messages;
using System.Diagnostics;
using System.Net;

namespace PowerPlatform.Dataverse.CodeSamples
{
    internal class Program
    {
        static readonly int numberOfRecords = Settings.NumberOfRecords; //100 by default
        static readonly string tableSchemaName = "sample_Example";
        static readonly string tableSetName = "sample_examples";
        static readonly string tableLogicalName = tableSchemaName.ToLower(); //sample_example
        static readonly string tablePrimaryKeyName = "sample_exampleid";
        static readonly string tablePrimaryNameColumnName = "sample_name";
        static readonly int chunkSize = Settings.UseElastic ? Settings.ElasticBatchSize : Settings.StandardBatchSize; // Configurable batch size

        static async Task Main()
        {
            #region Optimize Connection settings

            //Change max connections from .NET to a remote service default: 2
            ServicePointManager.DefaultConnectionLimit = 65000;
            //Bump up the min threads reserved for this app to ramp connections faster - minWorkerThreads defaults to 4, minIOCP defaults to 4
            ThreadPool.SetMinThreads(100, 100);
            //Turn off the Expect 100 to continue message - 'true' will cause the caller to wait until it round-trip confirms a connection to the server
            ServicePointManager.Expect100Continue = false;
            //Can decrease overall transmission overhead but can cause delay in data packet arrival
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            #endregion Optimize Connection settings


            Config config = App.InitializeApp();

            // Disable affinity cookie for these operations. 
            config.DisableCookies = true;

            var service = new Service(config);

            Console.WriteLine($"RecommendedDegreesOfParallelism:{service.RecommendedDegreeOfParallelism}\n");

            try
            {
                await Utility.CreateExampleTable(service: service, tableSchemaName: tableSchemaName, isElastic: Settings.UseElastic);

                if (!await Utility.IsMessageAvailable(service: service, entityLogicalName: tableLogicalName, messageName: "CreateMultiple"))
                {
                    Console.WriteLine("The CreateMultiple message is not available " +
                    $"for the {tableSchemaName} table.");
                }
                else
                {

                    // Create a List of entity instances.
                    Console.WriteLine($"\nPreparing {numberOfRecords} records to create..");

                    List<JObject> entityList = new();
                    // Populate the list with the number of records to test.
                    for (int i = 0; i < numberOfRecords; i++)
                    {
                        entityList.Add(new JObject() {
                            {"sample_name", $"sample record {i+1:0000000}"},
                            // Each record MUST have the @odata.type specified.
                            {"@odata.type",$"Microsoft.Dynamics.CRM.{tableLogicalName}" }
                        });
                    }

                    ParallelOptions parallelOptions = new()
                    {
                        MaxDegreeOfParallelism = service.RecommendedDegreeOfParallelism
                    };


                    Console.WriteLine($"Sending POST requests to /{tableSetName}/Microsoft.Dynamics.CRM.CreateMultiple in parallel...");
                    Stopwatch createStopwatch = Stopwatch.StartNew();

                    await Parallel.ForEachAsync(
                            source: entityList.Chunk(chunkSize),
                            parallelOptions: parallelOptions,
                            async (entities, token) =>
                            {

                                CreateMultipleRequest createMultipleRequest = new(entitySetName: tableSetName, targets: entities.ToList());

                                // Add a tag optional parameter to set a shared variable to be available to a plug-in.
                                // See https://learn.microsoft.com/power-apps/developer/data-platform/optional-parameters?tabs=webapi#add-a-shared-variable-to-the-plugin-execution-context
                                createMultipleRequest.RequestUri = new Uri(
                                    createMultipleRequest.RequestUri.ToString() + "?tag=ParallelCreateUpdateMultiple",
                                    uriKind: UriKind.Relative);

                                if (Settings.BypassCustomPluginExecution)
                                {
#pragma warning disable CS0162 // Unreachable code detected: Configurable by setting
                                    createMultipleRequest.Headers.Add("MSCRM.BypassCustomPluginExecution", "true");
#pragma warning restore CS0162 // Unreachable code detected: Configurable by setting
                                }

                                var createMultipleResponse = await service.SendAsync<CreateMultipleResponse>(createMultipleRequest);

                                // Set the id values for the entities
                                for (int i = 0; i < entities.Length; i++)
                                {
                                    entities[i].Add(tablePrimaryKeyName, createMultipleResponse.Ids[i]);
                                }


                            });
                    createStopwatch.Stop();

                    Console.WriteLine($"\tCreated {entityList.Count} records " +
                        $"in {Math.Round(createStopwatch.Elapsed.TotalSeconds)} seconds.");

                    Console.WriteLine($"\nPreparing {numberOfRecords} records to update..");

                    // Update the sample_name value:
                    foreach (JObject entity in entityList)
                    {
                        entity[tablePrimaryNameColumnName] += " Updated";
                    }

                    Console.WriteLine($"Sending POST requests to /{tableSetName}/Microsoft.Dynamics.CRM.UpdateMultiple in parallel...");
                    Stopwatch updateStopwatch = Stopwatch.StartNew();

                    await Parallel.ForEachAsync(
                            source: entityList.Chunk(chunkSize),
                            parallelOptions: parallelOptions,
                            async (entities, token) => {

                                UpdateMultipleRequest updateMultipleRequest = new(entitySetName: tableSetName, targets: entities.ToList());

                                // Add a tag optional parameter to set a shared variable to be available to a plug-in.
                                // See https://learn.microsoft.com/power-apps/developer/data-platform/optional-parameters?tabs=webapi#add-a-shared-variable-to-the-plugin-execution-context
                                updateMultipleRequest.RequestUri = new Uri(
                                    updateMultipleRequest.RequestUri.ToString() + "?tag=ParallelCreateUpdateMultiple",
                                    uriKind: UriKind.Relative);

                                if (Settings.BypassCustomPluginExecution)
                                {
#pragma warning disable CS0162 // Unreachable code detected: Configurable by setting
                                    updateMultipleRequest.Headers.Add("MSCRM.BypassCustomPluginExecution", "true");
#pragma warning restore CS0162 // Unreachable code detected: Configurable by setting
                                }

                                await service.SendAsync(updateMultipleRequest);
                            }
                        );

                    updateStopwatch.Stop();
                    Console.WriteLine($"\tUpdated {numberOfRecords} records " +
                        $"in {Math.Round(updateStopwatch.Elapsed.TotalSeconds)} seconds.");

                    // Delete created rows asynchronously
                    // When testing plug-ins, set break point here to observe data
                    Console.WriteLine($"\nStarting asynchronous bulk delete " +
                        $"of {entityList.Count} created records...");

                    Guid[] iDs = new Guid[entityList.Count];

                    for (int i = 0; i < entityList.Count; i++)
                    {
                        iDs[i] = (Guid)entityList[i][tablePrimaryKeyName];
                    }

                    string deleteJobStatus = await Utility.BulkDeleteRecordsByIds(
                        service: service,
                        tableLogicalName: tableLogicalName,
                        iDs: iDs,
                        jobName: "Deleting records created by ParallelCreateUpdateMultiple Sample.");

                    Console.WriteLine($"\tBulk Delete status: {deleteJobStatus}\n");

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await Utility.DeleteExampleTable(service: service, tableSchemaName: tableSchemaName);
            }
        }
    }
}