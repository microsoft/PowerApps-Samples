using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Rest;
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

                // Confirm the table supports UpsertMultiple
                if (!Utility.IsMessageAvailable(
                    service: serviceClient,
                    entityLogicalName: tableLogicalName,
                    messageName: "UpsertMultiple"))
                {
                    Console.WriteLine($"The UpsertMultiple message is not available " +
                        $"for the {tableSchemaName} table.");
                }
                else
                {
                    // For UpsertMultiple, we will try to set the batch to contain records
                    // where half of them will be created and half will be updated by UpsertMultiple
                    int nCreate = numberOfRecords / 2;
                    EntityCollection entities = new();

                    var createdRecordIds = CreateRecordsUtility(nCreate, tableLogicalName, serviceClient, out entities);

                    Console.WriteLine($"\nPreparing {nCreate} records to update..");

                    // Assign Id values to created items to prepare to update them
                    for (int i = 0; i < createdRecordIds.Length; i++)
                    {
                        entities.Entities[i].Id = createdRecordIds[i];
                    }

                    // Update the sample_name value:
                    foreach (Entity entity in entities.Entities)
                    {
                        entity["sample_name"] += " Updated";
                    }

                    Console.WriteLine($"\nPreparing {nCreate} records to create..");
                    for (int i = 0; i < nCreate; i++)
                    {
                        entities.Entities.Add(new Entity(tableLogicalName)
                        {
                            Attributes = {
                            // Example: 'sample record 0000001'
                            { "sample_name", $"sample record {i+1:0000000}" }
                        }
                        });
                    }

                    #region Send UpsertMultipleRequest
                    // Use UpsertMultipleRequest
                    UpsertMultipleRequest upsertMultipleRequest = new()
                    {
                        Targets = entities
                    };
                    // Add Shared Variable with request to detect in a plug-in.
                    upsertMultipleRequest["tag"] = "CreateUpdateMultiple";

                    if (Settings.BypassCustomPluginExecution)
                    {
#pragma warning disable CS0162 // Unreachable code detected: Configurable by setting
                        upsertMultipleRequest["BypassCustomPluginExecution"] = true;
#pragma warning restore CS0162 // Unreachable code detected: Configurable by setting
                    }

                    Console.WriteLine($"Sending UpsertMultipleRequest...");
                    Stopwatch updateStopwatch = Stopwatch.StartNew();
                    // Send the request
                    var upsertMultipleResponse = (UpsertMultipleResponse)serviceClient.Execute(upsertMultipleRequest);
                    updateStopwatch.Stop();
                    Console.WriteLine($"\tUpserted {entities.Entities.Count} records " +
                        $"in {Math.Round(updateStopwatch.Elapsed.TotalSeconds)} seconds.");

                    #endregion

                    if (Settings.UseElastic)
                    {

                        Console.WriteLine($"\nPreparing {numberOfRecords} records to delete..");
                        // Delete created rows with DeleteMultiple
                        EntityReferenceCollection targets = new();
                        foreach (Entity entity in entities.Entities)
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

                        Console.WriteLine($"\tDeleted {entities.Entities.Count} records " +
                            $"in {Math.Round(deleteStopwatch.Elapsed.TotalSeconds)} seconds.");

                    }
                    else
                    {
                        // Delete created rows asynchronously
                        // When testing plug-ins, set break point here to observe data
                        Console.WriteLine($"\nStarting asynchronous bulk delete " +
                            $"of {entities.Entities.Count} created records...");

                        // TODO : Need to come up with a way to get all the IDs to delete
                        string deleteJobStatus = Utility.BulkDeleteRecordsByIds(
                            service: serviceClient,
                            tableLogicalName: tableLogicalName,
                            iDs: createdRecordIds,
                            jobName: "Deleting records created by UpsertMultiple Sample.");

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

        public static Guid[] CreateRecordsUtility(int numberOfRecords, string tableLogicalName, ServiceClient serviceClient, out EntityCollection entities)
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
            entities = new(entityList)
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

            // Send the request
            CreateMultipleResponse createMultipleResponse =
                (CreateMultipleResponse)serviceClient.Execute(createMultipleRequest);

            return createMultipleResponse.Ids;
        }
    }
}