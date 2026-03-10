using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
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
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Program app = new();

            int numberOfRecords = Settings.NumberOfRecords; //100 by default
            string tableSchemaName = "sample_Example";
            string tableLogicalName = tableSchemaName.ToLower(); //sample_example

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
                    isElastic: Settings.UseElastic,
                    createAlternateKey: Settings.CreateAlternateKey);

                bool isCreateMultipleAvailable = Utility.IsMessageAvailable(
                    service: serviceClient,
                    entityLogicalName: tableLogicalName,
                    messageName: "CreateMultiple");

                bool isUpdateMultipleAvailable = Utility.IsMessageAvailable(
                    service: serviceClient,
                    entityLogicalName: tableLogicalName,
                    messageName: "UpdateMultiple");

                // Confirm the table supports UpsertMultiple
                if (!isCreateMultipleAvailable || !isUpdateMultipleAvailable)
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

                    var createdRecordIds = Utility.CreateRecordsUtility(nCreate, tableLogicalName, serviceClient, out entities);

                    // Retrieve the records which were created in the CreateRecordsUtility.
                    var retrieveMultipleRequest = new RetrieveMultipleRequest();
                    var query = new QueryExpression
                    {
                        EntityName = tableLogicalName,
                        ColumnSet = new ColumnSet("sample_name"),
                    };

                    if (Settings.CreateAlternateKey)
                    {
                        if (!Settings.UseElastic)
                        {
#pragma warning disable CS0162 // Unreachable code detected
                            query.ColumnSet.AddColumn("sample_keyattribute");
#pragma warning restore CS0162 // Unreachable code detected
                        }
                        else
                        {
                            query.ColumnSet.AddColumn(tableLogicalName + "id");
                            query.ColumnSet.AddColumn(Settings.ElasticTablePartitionId);
                        }
                    }

                    retrieveMultipleRequest.Query = query;

                    var retrieveMultipleResponseRaw = (RetrieveMultipleResponse)serviceClient.Execute(retrieveMultipleRequest);
                    var recordsCreated = retrieveMultipleResponseRaw.EntityCollection.Entities.ToList();

                    // UpsertMultiple scenarios covered in this sample for x records
                    // 1. Create x/4 records letting the system set the primary key
                    // 2. Create x/4 records with a unique alternate key value
                    // 3. Update x/4 records by providing Primary Key (PK) which is entity.Id
                    // 4. Update x/4 records by providing Alternate Key (AK) which is entity.KeyAttribute

                    // 1. Prepare x/4 entity instances with no primary key set.
                    Console.WriteLine($"\tPreparing {numberOfRecords / 4} entity instances to create without alternate key value.");
                    for (int i = 0; i < nCreate / 2; i++)
                    {
                        entities.Entities.Add(new Entity(tableLogicalName)
                        {
                            Attributes = {
                            // Example: 'sample record 0000001'
                            { "sample_name", $"sample Upsert(Create by PK) record {i+1:0000000}" }
                        }
                        });
                    }

                    if (Settings.CreateAlternateKey)
                    {
                        // 2. Prepare x/4 entity instances with unique key attribute values.
                        Console.WriteLine($"\tPreparing {numberOfRecords / 4} entity instances to create with an alternate key value\n");
                        for (int i = 0; i < nCreate / 2; i++)
                        {
                            Entity entity = new(tableLogicalName);
                            entity.Attributes.Add("sample_name", $"sample Upsert(Create by AK) record {i + 1:0000000}");
                            if (!Settings.UseElastic)
                            {
                                entity.KeyAttributes.Add("sample_keyattribute", $"sample upsert record key {i + 1:0000000}");
                            }
                            else
                            {
                                entity.KeyAttributes.Add(tableLogicalName + "id", Guid.NewGuid());
                                entity.KeyAttributes.Add(Settings.ElasticTablePartitionId, $"sample upsert record key {i + 1:0000000}");
                            }
                            entities.Entities.Add(entity);
                        }
                    }

                    // 3. Prepare  x/4 entity instances with primary key values from previously created records.
                    Console.WriteLine($"\tPreparing {numberOfRecords / 4} entity instances to update using existing Primary Key value..");

                    // Assign Id values to created items to prepare to update them
                    for (int i = 0; i < recordsCreated.Count / 2; i++)
                    {
                        entities.Entities[i]["sample_name"] += " Updated using PK";
                        if (!Settings.UseElastic)
                        {
                            entities.Entities[i][tableLogicalName + "id"] = recordsCreated[i].Id;
                            entities.Entities[i].Attributes.Remove("sample_keyattribute");
                        }
                        else
                        {
                            entities.Entities[i].Id = recordsCreated[i].Id;
                            entities.Entities[i].KeyAttributes.Clear();
                        }
                    }

                    if (Settings.CreateAlternateKey)
                    {
                        // 4. Prepare x/4 entity instances with only alternate key values from previously created records
                        Console.WriteLine($"\tPreparing {numberOfRecords / 4} entity instances to update  with existing alternate key value..");
                        for (int i = recordsCreated.Count / 2; i < recordsCreated.Count; i++)
                        {
                            if (Settings.UseElastic)
                            {
                                entities.Entities[i]["sample_name"] += " Updated using AK";
                            }
                            else
                            {
                                entities.Entities[i].Attributes.Remove("sample_keyattribute");
                                entities.Entities[i].KeyAttributes.Add("sample_keyattribute", recordsCreated[i]["sample_keyattribute"]);
                                entities.Entities[i]["sample_name"] += " Updated using AK";
                            }
                        }
                    }


                    #region Verify Alternate Key is active

                    if (Settings.CreateAlternateKey && !Settings.UseElastic)
                    {

#pragma warning disable CS0162 // Unreachable code detected
                        Console.WriteLine("\nCheck if AlternateKey is available in the metadata for the table...");
#pragma warning restore CS0162 // Unreachable code detected
                        bool isAlternateKeyCreated = Utility.VerifyAlternateKeyIsActive(serviceClient, tableLogicalName);

                        if (!isAlternateKeyCreated)
                        {
                            Console.WriteLine("\tThere is a problem creating the index for the product code alternate key for the table.");
                            Console.WriteLine("\tThe sample cannot continue. Please try again.");
                        }

                        Console.WriteLine($"\tAlternate key {(isAlternateKeyCreated ? "is available" : "is not available")}\n");
                    }

                    #endregion

                    #region Send UpsertMultipleRequest
                    // Use UpsertMultipleRequest
                    UpsertMultipleRequest request = new()
                    {
                        Targets = entities
                    };
                    // Add Shared Variable with request to detect in a plug-in.
                    request["tag"] = "UpsertMultiple";

                    if (Settings.BypassCustomPluginExecution)
                    {
#pragma warning disable CS0162 // Unreachable code detected: Configurable by setting
                        request["BypassCustomPluginExecution"] = true;
#pragma warning restore CS0162 // Unreachable code detected: Configurable by setting
                    }

                    Console.WriteLine($"Sending UpsertMultipleRequest...");
                    Stopwatch updateStopwatch = Stopwatch.StartNew();
                    // Send the request
                    var response = (UpsertMultipleResponse)serviceClient.Execute(request);
                    updateStopwatch.Stop();
                    Console.WriteLine($"\tUpserted {entities.Entities.Count} records " +
                        $"in {Math.Round(updateStopwatch.Elapsed.TotalSeconds)} seconds.");

                    #endregion

                    var retrievemultiplerequest2 = new RetrieveMultipleRequest();
                    retrievemultiplerequest2.Query = query;

                    var retrieveResponse = (RetrieveMultipleResponse)serviceClient.Execute(retrieveMultipleRequest);
                    var recordsUpserted = retrieveMultipleResponseRaw.EntityCollection.Entities.ToList();

                    #region Validate UpsertMultiple Response

                    List<Guid> createdRecords = new();
                    List<Guid> updatedRecords = new();

                    if (response != null)
                    {
                        foreach (var upsertResponse in response.Results)
                        {
                            if (upsertResponse == null) continue;
                            if (upsertResponse.RecordCreated)
                            {
                                createdRecords.Add(upsertResponse.Target.Id);
                            }
                            else
                            {
                                updatedRecords.Add(upsertResponse.Target.Id);
                            }
                        }
                    }

                    Console.WriteLine($"Records Created: {createdRecords.Count}, " +
                        $"Records Updated: {updatedRecords.Count}");

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

                        createdRecords.AddRange(updatedRecords);
                        string deleteJobStatus = Utility.BulkDeleteRecordsByIds(
                            service: serviceClient,
                            tableLogicalName: tableLogicalName,
                            iDs: createdRecords.ToArray());

                        Console.WriteLine($"\tBulk Delete status: {deleteJobStatus}");
                    }


                }
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
            finally
            {
                // Delete sample_example table
                Utility.DeleteExampleTable(
                    service: serviceClient,
                    tableSchemaName: tableSchemaName);
            }
        }
    }
}