using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Rest;
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
                    isElastic: Settings.UseElastic,
                    createAlternateKey: Settings.CreateAlternateKey);

                // Confirm the table supports UpsertMultiple
                if (Settings.UseElastic && !Utility.IsMessageAvailable(
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

                    // Retrieve the records which were created in the CreateRecordsUtility.
                    var retrieveMultipleRequest = new RetrieveMultipleRequest();
                    var query = new QueryExpression
                    {
                        EntityName = tableLogicalName,
                        ColumnSet = new ColumnSet("sample_name"),
                    };

                    if (Settings.CreateAlternateKey)
                    {
#pragma warning disable CS0162 // Unreachable code detected
                        query.ColumnSet.AddColumn("sample_keyattribute");
#pragma warning restore CS0162 // Unreachable code detected
                    }

                    retrieveMultipleRequest.Query = query;

                    var retrieveMultipleResponseRaw = (RetrieveMultipleResponse)serviceClient.Execute(retrieveMultipleRequest);
                    var recordsCreated = retrieveMultipleResponseRaw.EntityCollection.Entities.ToList();

                    // UpsertMultiple scenarios covered in this sample for x records
                    // 1. Create x/4 records by providing PK which is entity.Id
                    // 2. Create x/4 records by providing AK which is entity.KeyAttribute
                    // 3. Upsert x/4 records by providing PK which is entity.Id
                    // 4. Upsert x/4 records by providing AK which is entity.KeyAttribute

                    // 1. Create x/4 records by providing PK which is entity.Id
                    Console.WriteLine($"\nPreparing {numberOfRecords / 4} records to create using PK..");
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
                        // 2. Create x/4 records by providing AK which is entity.KeyAttribute
                        Console.WriteLine($"\nPreparing {numberOfRecords / 4} records to create using AK..");
                        for (int i = 0; i < nCreate / 2; i++)
                        {
                            Entity entity = new Entity(tableLogicalName);
                            entity.Attributes.Add("sample_name", $"sample Upsert(Create by AK) record {i + 1:0000000}");
                            entity.KeyAttributes.Add("sample_keyattribute", $"sample upsert record key {i + 1:0000000}");
                            entities.Entities.Add(entity);
                        }
                    }

                    // 3. Upsert x/4 records by providing PK which is entity.Id
                    Console.WriteLine($"\nPreparing {numberOfRecords / 4} records to update using PK..");

                    // Assign Id values to created items to prepare to update them
                    for (int i = 0; i < recordsCreated.Count / 2; i++)
                    {
                        entities.Entities[i].Id = recordsCreated[i].Id;
                        entities.Entities[i]["sample_name"] += " Updated using PK";
                    }

                    if (Settings.CreateAlternateKey)
                    {
                        // 4. Upsert x/4 records by providing AK which is entity.KeyAttribute
                        Console.WriteLine($"\nPreparing {numberOfRecords / 4} records to update using AK..");
                        for (int i = recordsCreated.Count / 2; i < recordsCreated.Count; i++)
                        {
                            entities.Entities[i].KeyAttributes.Add("sample_keyattribute", recordsCreated[i]["sample_keyattribute"]);
                            entities.Entities[i]["sample_name"] += " Updated using AK";
                        }
                    }


                    #region Verify Alternate Key is active

                    if (Settings.CreateAlternateKey)
                    {

#pragma warning disable CS0162 // Unreachable code detected
                        Console.WriteLine("Check if AlternateKey is available in the metadata for the entity.");
#pragma warning restore CS0162 // Unreachable code detected
                        bool isAlternateKeyCreated = Utility.VerifyAlternateKeyIsActive(serviceClient, tableLogicalName);

                        if (!isAlternateKeyCreated)
                        {
                            Console.WriteLine("There is a problem creating the index for the product code alternate key for the entity.");
                            Console.WriteLine("The sample cannot continue. Please try again.");
                        }

                        Console.WriteLine($"Is Alternate key available: {isAlternateKeyCreated}");
                    }

                    #endregion

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

                    #region Validate UpsertMultiple Response

                    List<Guid> createdRecords = new List<Guid>();
                    List<Guid> updatedRecords = new List<Guid>();

                    if (upsertMultipleResponse != null)
                    {
                        foreach (var response in upsertMultipleResponse.Results)
                        {
                            if(response == null) continue;
                            if(response.RecordCreated)
                            {
                                createdRecords.Add(response.Target.Id);
                            }
                            else
                            {
                                updatedRecords.Add(response.Target.Id);
                            }
                        }
                    }

                    Console.WriteLine($"Records Created through UpsertMultiple: {createdRecords.Count}, Records Updated through UpsertMultiple: {updatedRecords.Count}");

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

        public static Guid[] CreateRecordsUtility(int numberOfRecords, string tableLogicalName, IOrganizationService orgService, out EntityCollection entities)
        {
            // Create a List of entity instances.
            Console.WriteLine($"\nPreparing {numberOfRecords} records to create..");
            List<Entity> entityList = new();
            // Populate the list with the number of records to test.
            for (int i = 0; i < numberOfRecords; i++)
            {
                Entity entity = new Entity(tableLogicalName);

                // Example: 'sample record 0000001'
                // Example key: 'sample record key 0000001'
                entity["sample_name"] = $"sample record {i + 1:0000000}";

                if (Settings.CreateAlternateKey)
                {
#pragma warning disable CS0162 // Unreachable code detected
                    entity["sample_keyattribute"] = $"sample pre-upsert record key {i + 1:0000000}";
#pragma warning restore CS0162 // Unreachable code detected
                }

                entityList.Add(entity);
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
                (CreateMultipleResponse)orgService.Execute(createMultipleRequest);

            return createMultipleResponse.Ids;
        }
    }
}