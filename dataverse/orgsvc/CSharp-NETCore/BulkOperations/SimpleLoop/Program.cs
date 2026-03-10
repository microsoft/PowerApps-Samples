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

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Create a Dataverse service client using the default connection string.
            ServiceClient serviceClient =
                new(app.Configuration.GetConnectionString("default"))
                {
                    UseWebApi = false
                };

            // Create sample_Example table for this sample
            Utility.CreateExampleTable(
                serviceClient: serviceClient,
                tableSchemaName: tableSchemaName, 
                isElastic: Settings.UseElastic);


            // Create a List of entity instances
            Console.WriteLine($"\nPreparing {numberOfRecords} records to create..");
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

            Console.WriteLine($"Sending create requests one at a time...");
            Stopwatch createStopwatch = Stopwatch.StartNew();

            entityList.ForEach(entity =>
            {
                CreateRequest createRequest = new() { Target = entity };
                // Add Shared Variable with request to detect in a plug-in.
                createRequest["tag"] = "SimpleLoop";

                if (Settings.BypassCustomPluginExecution)
                {
#pragma warning disable CS0162 // Unreachable code detected: Configurable by setting
                    createRequest["BypassCustomPluginExecution"] = true;
#pragma warning restore CS0162 // Unreachable code detected: Configurable by setting
                }

                var createResponse = (CreateResponse)serviceClient.Execute(createRequest);

                entity.Id = createResponse.id;

            });

            createStopwatch.Stop();
            Console.WriteLine($"\tCreated {entityList.Count} records " +
                $"in {Math.Round(createStopwatch.Elapsed.TotalSeconds)} seconds.");

            Console.WriteLine($"\nPreparing {numberOfRecords} records to update..");


            // Update the sample_name value:
            foreach (Entity entity in entityList)
            {
                entity["sample_name"] += " Updated";
            }

            Stopwatch updateStopwatch = Stopwatch.StartNew();
            Console.WriteLine($"Sending update requests one at a time...");
            updateStopwatch.Start();
            // Send the request
            entityList.ForEach(entity =>
            {
                UpdateRequest updateRequest = new() { Target = entity };
                // Add Shared Variable with request to detect in a plug-in.
                updateRequest["tag"] = "SimpleLoop";

                if (Settings.BypassCustomPluginExecution)
                {
#pragma warning disable CS0162 // Unreachable code detected: Configurable by setting
                    updateRequest["BypassCustomPluginExecution"] = true;
#pragma warning restore CS0162 // Unreachable code detected: Configurable by setting
                }

                serviceClient.Execute(updateRequest);

            });
            updateStopwatch.Stop();
            Console.WriteLine($"\tUpdated {entityList.Count} records " +
                $"in {Math.Round(updateStopwatch.Elapsed.TotalSeconds)} seconds.");

            if (Settings.UseElastic)
            {
                Console.WriteLine($"\nPreparing {numberOfRecords} records to delete..");
                // Delete created rows with DeleteMultiple
                EntityReferenceCollection targets = new();
                foreach (Entity entity in entityList)
                {
                    targets.Add(entity.ToEntityReference());
                }

                OrganizationRequest deleteMultipleRequest = new("DeleteMultiple")
                {
                    Parameters = {
                                {"Targets", targets }
                            }
                };

                Console.WriteLine($"Sending DeleteMultipleRequest...");
                Stopwatch deleteStopwatch = Stopwatch.StartNew();
                serviceClient.Execute(deleteMultipleRequest);
                deleteStopwatch.Stop();

                Console.WriteLine($"\tDeleted {entityList.Count} records " +
                    $"in {Math.Round(deleteStopwatch.Elapsed.TotalSeconds)} seconds.");
            }
            else {
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
                    jobName: "Deleting records created by SimpleLoop Sample.");

                Console.WriteLine($"\tBulk Delete status: {deleteJobStatus}");
            }
            
        

            // Delete sample_example table
            Utility.DeleteExampleTable(
                service: serviceClient,
                tableSchemaName: tableSchemaName);
        }
    }
}