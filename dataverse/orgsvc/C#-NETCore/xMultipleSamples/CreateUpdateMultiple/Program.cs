using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System.Diagnostics;
using System.Net;
using System.ServiceModel;

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

            try
            {
                // Create sample_Example table for this sample
                Utility.CreateExampleTable(
                    serviceClient: serviceClient,
                    tableSchemaName: tableSchemaName, 
                    isElastic: Settings.UseElastic);

                // Confirm the table supports CreateMultiple
                if (!Utility.IsMessageAvailable(
                    service: serviceClient,
                    entityLogicalName: tableLogicalName,
                    messageName: "CreateMultiple"))
                {
                    Console.WriteLine($"The CreateMultiple message is not available " +
                        $"for the {tableSchemaName} table.");
                }
                else
                {
                    // Create a List of entity instances.
                    Console.WriteLine($"\nPreparing {numberOfRecords} records to create..");
                    List<Entity> entityList = new();
                    // Populate the list with the number of records to test.
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
                    // Create an EntityCollection populated with the list of entities.
                    EntityCollection entities = new(entityList)
                    {
                        EntityName = tableLogicalName
                    };


                    // Use CreateMultipleRequest
                    CreateMultipleRequest createMultipleRequest = new()
                    {
                        Targets = entities,
                    };
                    // Add Shared Variable with request to detect in a plug-in.
                    createMultipleRequest["tag"] = "CreateUpdateMultiple";

                    if (Settings.BypassCustomPluginExecution)
                    {
#pragma warning disable CS0162 // Unreachable code detected: Configurable by setting
                        createMultipleRequest["BypassCustomPluginExecution"] = true;
#pragma warning restore CS0162 // Unreachable code detected: Configurable by setting
                    }

                    Console.WriteLine($"Sending CreateMultipleRequest...");
                    Stopwatch createStopwatch = Stopwatch.StartNew();
                    // Send the request
                    CreateMultipleResponse createMultipleResponse =
                        (CreateMultipleResponse)serviceClient.Execute(createMultipleRequest);
                    createStopwatch.Stop();
                    Console.WriteLine($"\tCreated {entities.Entities.Count} records " +
                        $"in {Math.Round(createStopwatch.Elapsed.TotalSeconds)} seconds.");

                    Console.WriteLine($"\nPreparing {numberOfRecords} records to update..");

                    // Assign Id values to created items to prepare to update them
                    for (int i = 0; i < createMultipleResponse.Ids.Length; i++)
                    {
                        entities.Entities[i].Id = createMultipleResponse.Ids[i];
                    }

                    // Update the sample_name value:
                    foreach (Entity entity in entities.Entities)
                    {
                        entity["sample_name"] += " Updated";
                    }

                    // Use UpdateMultipleRequest
                    UpdateMultipleRequest updateMultipleRequest = new()
                    {
                        Targets = entities
                    };
                    // Add Shared Variable with request to detect in a plug-in.
                    updateMultipleRequest["tag"] = "CreateUpdateMultiple";

                    if (Settings.BypassCustomPluginExecution)
                    {
#pragma warning disable CS0162 // Unreachable code detected: Configurable by setting
                        updateMultipleRequest["BypassCustomPluginExecution"] = true;
#pragma warning restore CS0162 // Unreachable code detected: Configurable by setting
                    }

                    Console.WriteLine($"Sending UpdateMultipleRequest...");
                    Stopwatch updateStopwatch = Stopwatch.StartNew();
                    // Send the request
                    serviceClient.Execute(updateMultipleRequest);
                    updateStopwatch.Stop();
                    Console.WriteLine($"\tUpdated {entities.Entities.Count} records " +
                        $"in {Math.Round(updateStopwatch.Elapsed.TotalSeconds)} seconds.");

                    if (Settings.UseElastic) {

                        Console.WriteLine($"\nPreparing {numberOfRecords} records to delete..");
                        // Delete created rows with DeleteMultiple
                        EntityReferenceCollection targets = new();
                        foreach (Entity entity in entities.Entities)
                        {
                            targets.Add(entity.ToEntityReference());
                        }

                        OrganizationRequest deleteMultipleRequest = new("DeleteMultiple") { 
                            Parameters = {
                                {"Targets", targets }
                            }                            
                        };

                        Console.WriteLine($"Sending DeleteMultipleRequest...");
                        Stopwatch deleteStopwatch = Stopwatch.StartNew();
                        serviceClient.Execute(deleteMultipleRequest);
                        deleteStopwatch.Stop();

                        Console.WriteLine($"\tDeleted {entities.Entities.Count} records " +
                            $"in {Math.Round(deleteStopwatch.Elapsed.TotalSeconds)} seconds.");

                    }
                    else
                    {
                        // Delete created rows asynchronously
                        // When testing plug-ins, set break point here to observe data
                        Console.WriteLine($"\nStarting asynchronous bulk delete " +
                            $"of {createMultipleResponse.Ids.Length} created records...");

                        string deleteJobStatus = Utility.BulkDeleteRecordsByIds(
                            service: serviceClient,
                            tableLogicalName: tableLogicalName,
                            iDs: createMultipleResponse.Ids,
                            jobName: "Deleting records created by CreateUpdateMultiple Sample.");

                        Console.WriteLine($"\tBulk Delete status: {deleteJobStatus}");
                    }


                }

                // Delete sample_example table
                Utility.DeleteExampleTable(
                    service: serviceClient,
                    tableSchemaName: tableSchemaName);

            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Timestamp: {0}", ex.Detail.Timestamp);
                Console.WriteLine("Code: {0}", ex.Detail.ErrorCode);
                Console.WriteLine("Message: {0}", ex.Detail.Message);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Message: {0}", ex.Message);
                Console.WriteLine("Stack Trace: {0}", ex.StackTrace);
                Console.WriteLine("Inner Fault: {0}",
                ex.InnerException.Message ?? "No Inner Fault");
            }
            catch (Exception ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine(ex.Message);

                // Display the details of the inner exception.
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);

                    if (ex.InnerException is FaultException<OrganizationServiceFault> fe)
                    {
                        Console.WriteLine("Timestamp: {0}", fe.Detail.Timestamp);
                        Console.WriteLine("Code: {0}", fe.Detail.ErrorCode);
                        Console.WriteLine("Message: {0}", fe.Detail.Message);
                        Console.WriteLine("Trace: {0}", fe.Detail.TraceText);
                        Console.WriteLine("Inner Fault: {0}",
                            null == fe.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
                    }
                }
            }
        }
    }
}