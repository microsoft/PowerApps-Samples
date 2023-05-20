using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System.Diagnostics;
using System.Net;

namespace PowerPlatform.Dataverse.CodeSamples
{
    class Program
    {
        /// <summary>
        /// Contains the application's configuration settings. 
        /// </summary>
        IConfiguration Configuration { get; }


        /// <summary>
        /// Constructor. Loads the application configuration settings from a JSON file.
        /// </summary>
        Program()
        {

            // Get the path to the appsettings file. If the environment variable is set,
            // use that file path. Otherwise, use the runtime folder's settings file.
            string? path = Environment.GetEnvironmentVariable("DATAVERSE_APPSETTINGS");
            path ??= "appsettings.json";

            // Load the app's configuration settings from the JSON file.
            Configuration = new ConfigurationBuilder()
                .AddJsonFile(path, optional: false, reloadOnChange: true)
                .Build();
        }
        static void Main()
        {
            Program app = new();

            int numberOfRecords = Settings.NumberOfRecords; //100 by default
            string tableSchemaName = "sample_Example";
            string tableLogicalName = tableSchemaName.ToLower(); //sample_example
            int chunkSize = Settings.BatchSize; // The maximum number of requests for ExecuteMultiple is 1000.

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Create a Dataverse service client using the default connection string.
            ServiceClient serviceClient =
                new(app.Configuration.GetConnectionString("default"));

            // Create sample_Example table for this sample
            Utility.CreateExampleTable(
                service: serviceClient, 
                tableSchemaName: tableSchemaName);


            // Create a List of entity instances
            Console.WriteLine($"Preparing {numberOfRecords} records to create...");
            List<Entity> entityList = new();
            // Populate the list with the number of records to test
            for (int i = 0; i < numberOfRecords; i++)
            {
                entityList.Add(new Entity(tableLogicalName)
                {
                    Attributes = {
                            // Example: 'sample record 0000001'
                            { "sample_name", $"sample record {i+1:0000000}" }
                        }
                });
            }


            // Preparing the ExecuteMultipleRequest parameters
            List<CreateRequest> createRequestsList = new();

            foreach (Entity enity in entityList)
            {
                CreateRequest createRequest = new() { Target = enity };
                // Add Shared Variable with request to detect in a plug-in.
                createRequest["tag"] = "ExecuteMultiple";

                if (Settings.BypassCustomPluginExecution)
                {
#pragma warning disable CS0162 // Unreachable code detected: Configurable by setting
                    createRequest["BypassCustomPluginExecution"] = true;
#pragma warning restore CS0162 // Unreachable code detected: Configurable by setting
                }

                createRequestsList.Add(createRequest);
            }


            // The limit per ExecuteMultipleRequest is 1000, so creating multiple 
            // ExecuteMultipleRequests when the total is greater than the configured chunkSize.
            List<ExecuteMultipleRequest> executeMultipleCreateRequests = new();

            foreach (IEnumerable<CreateRequest> createRequests in createRequestsList.Chunk(chunkSize))
            {
                OrganizationRequestCollection requestCollection = new();
                foreach (CreateRequest item in createRequests)
                {
                    requestCollection.Add(item);
                }

                executeMultipleCreateRequests.Add(new ExecuteMultipleRequest
                {
                    Requests = requestCollection,
                    Settings = new ExecuteMultipleSettings
                    {
                        ContinueOnError = true,
                        ReturnResponses = true // Returning Responses
                    }
                });
            }
                       
            List<ExecuteMultipleResponse> executeMultipleResponses = new();

            int createCount = 1;
            Stopwatch createStopwatch = Stopwatch.StartNew();

            Console.WriteLine($"Sending {executeMultipleCreateRequests.Count} " +
                $"ExecuteMultipleRequest{(executeMultipleCreateRequests.Count > 1? "s" : string.Empty)} to create...");

            // For each set of 1000 or less.
            executeMultipleCreateRequests.ForEach(emr =>
            {
                Console.WriteLine($" Sending ExecuteMultipleRequest {createCount}...");
                executeMultipleResponses.Add((ExecuteMultipleResponse)serviceClient.Execute(emr));
                createCount++;
            });

            createStopwatch.Stop();
            Console.WriteLine($"\tCreated {entityList.Count} records " +
                $"in {Math.Round(createStopwatch.Elapsed.TotalSeconds)} seconds.");

            Console.WriteLine($"Preparing {numberOfRecords} records to update...");

            List<CreateResponse> createResponses = new();

            // For each set of 1000 or less.
            executeMultipleResponses.ForEach(emr =>
            {
                foreach (ExecuteMultipleResponseItem item in emr.Responses)
                {
                    createResponses.Add((CreateResponse)item.Response);
                }
            });

            for (int i = 0; i < entityList.Count; i++)
            {
                entityList[i]["sample_name"] += " Updated";
                entityList[i].Id = createResponses[i].id;
            }

            List<UpdateRequest> updateRequestsList = new();

            foreach (Entity entity in entityList)
            {
                UpdateRequest updateRequest = new() { Target = entity };
                // Add Shared Variable with request to detect in a plug-in.
                updateRequest["tag"] = "ExecuteMultiple";

                if (Settings.BypassCustomPluginExecution)
                {
#pragma warning disable CS0162 // Unreachable code detected: Configurable by setting
                    updateRequest["BypassCustomPluginExecution"] = true;
#pragma warning restore CS0162 // Unreachable code detected: Configurable by setting
                }

                updateRequestsList.Add(updateRequest);
            }

            // The limit per ExecuteMultipleRequest is 1000, so creating multiple 
            // ExecuteMultipleRequests when the total is greater than the configured chunkSize.
            List<ExecuteMultipleRequest> executeMultipleUpdateRequests = new();

            foreach (IEnumerable<UpdateRequest> updateRequests in updateRequestsList.Chunk(chunkSize))
            {
                OrganizationRequestCollection requestCollection = new();
                foreach (UpdateRequest item in updateRequests)
                {
                    requestCollection.Add(item);
                }

                executeMultipleUpdateRequests.Add(new ExecuteMultipleRequest
                {
                    Requests = requestCollection,
                    Settings = new ExecuteMultipleSettings
                    {
                        ContinueOnError = true,
                        ReturnResponses = false //Not returning responses
                    }
                });
            }


            Stopwatch updateStopwatch = Stopwatch.StartNew();
            int updateCount = 1;            
            Console.WriteLine($"Sending {executeMultipleUpdateRequests.Count} " +
                $"ExecuteMultipleRequest{(executeMultipleUpdateRequests.Count > 1 ? "s" : string.Empty)} to update...");

            // For each set of 1000 or less.
            executeMultipleUpdateRequests.ForEach(emr =>
            {
                Console.WriteLine($" Sending ExecuteMultipleRequest {updateCount}...");
                serviceClient.Execute(emr);
                updateCount++;
            });

            updateStopwatch.Stop();
            Console.WriteLine($"\tUpdated {entityList.Count} records " +
                $"in {Math.Round(updateStopwatch.Elapsed.TotalSeconds)} seconds.");

            // Delete created rows asynchronously
            Console.WriteLine($"\nStarting asynchronous bulk delete " +
                $"of {entityList.Count} created records...");

            Guid[] iDs = new Guid[entityList.Count];

            for (int i = 0; i < entityList.Count; i++)
            {
                iDs[i] = entityList[i].Id;
            }

            string deleteJobStatus = Utility.BulkDeleteRecordsByIds(
                service: serviceClient,
                tableLogicalName: tableLogicalName,
                iDs: iDs,
                jobName: "Deleting records created by ExecuteMultiple Sample.");

            Console.WriteLine($"\tBulk Delete status: {deleteJobStatus}");

            // Delete sample_example table
            Utility.DeleteExampleTable(
                service: serviceClient, 
                tableSchemaName: tableSchemaName);
        }
    }
}