using Newtonsoft.Json.Linq;
using PowerApps.Samples;
using PowerApps.Samples.Messages;
using System.Diagnostics;

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
        static readonly string tableAlternateKey = "sample_keyattribute";
        static readonly string partitionValue = "PARTITION_VALUE";

        static async Task Main()
        {
            Config config = App.InitializeApp();

            var service = new Service(config);

            try
            {
                // Create Example table
                await Utility.CreateExampleTable(service: service, 
                    tableSchemaName: tableSchemaName, 
                    isElastic: Settings.UseElastic);

                // Create alternate key for standard table and validate that the key is created
                // before sending Bulk Operation resquests
                if(Settings.CreateAlternateKey && !Settings.UseElastic)
                {
                    Utility.CreateAlternateKeyToEntity(
                service: service,
                entityLogicalName: tableLogicalName,
                schemaName: "sample_TestKey", 
                displayName: "Sample Test Key",
                keyAttributes: new List<string> { "sample_keyattribute" });

                    var keyStatus = await Utility.ValidateAlternateKeyIsCreated(service, tableLogicalName, "sample_TestKey");
                }

                bool isCreateMultipleAvailable = await Utility.IsMessageAvailable(
                    service: service,
                    entityLogicalName: tableLogicalName,
                    messageName: "CreateMultiple");

                bool isUpdateMultipleAvailable = await Utility.IsMessageAvailable(
                    service: service,
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
                    // UpsertMultiple scenarios covered in this sample for x records
                    // 1. Create x/4 records letting the system set the primary key
                    // 2. Create x/4 records with a unique alternate key value
                    // 3. Update x/4 records by providing Primary Key (PK) which is entity.Id
                    // 4. Update x/4 records by providing Alternate Key (AK) which is entity.KeyAttribute

                    // Create a List of entity instances to be updated during UpsertMultiple call.

                    // To handle cases where number of records is an odd number
                    var numRecCreate = Math.Ceiling((double)(numberOfRecords / 2)); 
                    
                    Console.WriteLine($"\nPreparing {numRecCreate} records to create..");

                    List<JObject> entityList = new();

                    // Populate the list with the number of records to test.
                    for (int i = 0; i < numRecCreate; i++)
                    {
                        entityList.Add(new JObject() {
                            {"sample_name", $"sample record {i+1:0000000}"},
                            // Each record MUST have the @odata.type specified.
                            {"@odata.type",$"Microsoft.Dynamics.CRM.{tableLogicalName}" }
                        });
                    }

                    if(Settings.CreateAlternateKey)
                    {
                        for(int i = 0;i < numRecCreate; i++)
                        {
                            var entity = entityList[i];

                            if (Settings.UseElastic)
                            {
                                
                                entity.Add("@odata.id", 
                                    $"{tableLogicalName}s({tablePrimaryKeyName}={Guid.NewGuid()}," +
                                    $"partitionid='{partitionValue}')");
                                entity.Add("partitionid", partitionValue);
                            }
                            else
                            {
                                // Add the alternate key in the payload in @odata.id tag
                                entity.Add(new JProperty("@odata.id", 
                                    $"{tableLogicalName}s({tableAlternateKey}='{i + 1:0000000}')"));
                            }
                        }
                    }

                    // Use UpsertMultiple
                    UpsertMultipleRequest upsertMultipleRequest = new(
                        entitySetName: tableSetName,
                        targets: entityList);

                    // Add a tag optional parameter to set a shared variable to be available to a plug-in.
                    // See https://learn.microsoft.com/power-apps/developer/data-platform/optional-parameters?tabs=webapi#add-a-shared-variable-to-the-plugin-execution-context

                    upsertMultipleRequest.RequestUri = new Uri(
                        upsertMultipleRequest.RequestUri.ToString() + "?tag=UpsertMultiple",
                        uriKind: UriKind.Relative);

                    if (Settings.BypassCustomPluginExecution)
                    {
#pragma warning disable CS0162 // Unreachable code detected: Configurable by setting
                        upsertMultipleRequest.Headers.Add("MSCRM.BypassCustomPluginExecution", "true");
#pragma warning restore CS0162 // Unreachable code detected: Configurable by setting
                    }


                    Console.WriteLine($"Sending POST request to /{tableSetName}/Microsoft.Dynamics.CRM.UpsertMultiple");

                    await service.SendAsync(upsertMultipleRequest);

                    // Prepare above created entity records to be updated by UpsertMultiple

                    Console.WriteLine($"\nPreparing {numRecCreate} records to Upsert(update)..");

                    var retrieveMultipleQuery = $"{tableLogicalName}s?$select={tablePrimaryKeyName}";

                    if(Settings.CreateAlternateKey)
                    {
                        if (Settings.UseElastic)
                        {
                            retrieveMultipleQuery += $",partitionid";
                        }
                        else
                        {
                            retrieveMultipleQuery += $",{tableAlternateKey}";
                        }
                    }

                    // Retrieve the record ids for records to be updated using PK
                    var retrieveMultipleRequest = new RetrieveMultipleRequest(
                        queryUri: retrieveMultipleQuery);

                    
                    var retrieveMultipleResponse = await service.SendAsync<RetrieveMultipleResponse>(retrieveMultipleRequest);

                     var numRecUpdatePK = Math.Ceiling((double)(numRecCreate / 2));
                    
                    if(!Settings.CreateAlternateKey)
                    {
#pragma warning disable CS0162 // Unreachable code detected
                        numRecUpdatePK = numRecCreate;
#pragma warning restore CS0162 // Unreachable code detected
                    }

                    // 3. Assign ID values to created items to prepare to upsert them using PK.
                    for (int i = 0; i < numRecUpdatePK; i++)
                    {
                        entityList[i].Add(tablePrimaryKeyName, retrieveMultipleResponse.Records?[i]?[tablePrimaryKeyName]);
                        entityList[i].Remove("@odata.id");
                    }

                    // Update the sample_name value. We want to Update num/2 records using Primary Key and num/2 records using Alternate Keys
                    var j = 0;
                    foreach (JObject entity in entityList)
                    {
                        if (j >= numRecUpdatePK  && Settings.CreateAlternateKey)
                        {
                            // 3. Upsert(Update) records using AK
                            entity[tablePrimaryNameColumnName] += " UpdatedByAK";
                            if(Settings.UseElastic)
                            {
                                entity.Remove("partitionid");
                            }
                        }
                        else
                        {
                            // 4. Upsert(Update) records using PK
                            entity[tablePrimaryNameColumnName] += " UpdatedByPK";
                        }

                        j++;

                    }

                    var numRecUpsertCreate = numberOfRecords - numRecCreate;

                    var num = Math.Ceiling((double)(numRecUpsertCreate / 2));

                    if(!Settings.CreateAlternateKey)
                    {
                        num = numRecUpsertCreate;
                    }

                    // 1. Create numberOfRecords/4 records to be created using Primary Key
                    Console.WriteLine($"\nPreparing {num} records to Upsert(create) using Primary Key..");

                    if (!Settings.UseElastic)
                    {
                        for (int i = 0; i < num; i++)
                        {
                            entityList.Add(new JObject() {
                            {"sample_name", $"sample record {i+1:0000000}"},
                            {tablePrimaryKeyName, Guid.NewGuid() },
                            // Each record MUST have the @odata.type specified.
                            {"@odata.type",$"Microsoft.Dynamics.CRM.{tableLogicalName}" }
                        });
                        }
                    }
                    else
                    {
                        for (int i = 0; i < num; i++)
                        {
                            entityList.Add(new JObject() {
                            {"sample_name", $"sample record {i+1:0000000}"},
                            {tablePrimaryKeyName, Guid.NewGuid() },
                            {"partitionid", partitionValue },
                            // Each record MUST have the @odata.type specified.
                            {"@odata.type",$"Microsoft.Dynamics.CRM.{tableLogicalName}" }
                        });
                        }
                    }

                    // 2. Create numberOfRecords/4 records to be created using Alternate Key
                    if (Settings.CreateAlternateKey)
                    {
                        Console.WriteLine($"\nPreparing {num} records to Upsert(create) using Alternate Key..");
                        for (int i = (int)num; i < numRecUpsertCreate; i++)
                        {
                            if (Settings.UseElastic)
                            {
                                entityList.Add(new JObject() {
                            { "sample_name", $"sample record {i+1:0000000}"},
                            { "@odata.id", $"{tableLogicalName}s({tablePrimaryKeyName}={Guid.NewGuid()},partitionid='{partitionValue}')" },
                            // Each record MUST have the @odata.type specified.
                            {"@odata.type",$"Microsoft.Dynamics.CRM.{tableLogicalName}" }
                        });
                            }
                            else
                            {
                                entityList.Add(new JObject() {
                            {"sample_name", $"sample record {i+1:0000000}"},
                            { "@odata.id", $"{tableLogicalName}s({tableAlternateKey}='sample record upsert record key {i+1:0000000}')" },
                            // Each record MUST have the @odata.type specified.
                            {"@odata.type",$"Microsoft.Dynamics.CRM.{tableLogicalName}" }
                        });
                            }
                        }
                    }

                    // Use UpsertMultipleRequest
                    
                    UpsertMultipleRequest upsertMultipleRequest2 = new(
                        entitySetName: tableSetName,
                        targets: entityList);

                    // Add a tag optional parameter to set a shared variable to be available to a plug-in.
                    // See https://learn.microsoft.com/power-apps/developer/data-platform/optional-parameters?tabs=webapi#add-a-shared-variable-to-the-plugin-execution-context
                    upsertMultipleRequest2.RequestUri = new Uri(
                        uriString: upsertMultipleRequest2.RequestUri.ToString() + "?tag=UpsertMultiple", 
                        uriKind: UriKind.Relative);

                    if (Settings.BypassCustomPluginExecution)
                    {
#pragma warning disable CS0162 // Unreachable code detected: Configurable by setting
                        upsertMultipleRequest2.Headers.Add("MSCRM.BypassCustomPluginExecution", "true");
#pragma warning restore CS0162 // Unreachable code detected: Configurable by setting
                    }

                    Console.WriteLine($"Sending POST request to /{tableSetName}/Microsoft.Dynamics.CRM.UpsertMultiple");

                    Stopwatch upsertStopwatch = Stopwatch.StartNew();
                    // Send the request
                    await service.SendAsync(upsertMultipleRequest2);
                    upsertStopwatch.Stop();
                    Console.WriteLine($"\tUpserted {entityList.Count} records " +
                        $"in {Math.Round(upsertStopwatch.Elapsed.TotalSeconds)} seconds.");

                    // Delete created rows asynchronously
                    // When testing plug-ins, set break point here to observe data
                    Console.WriteLine($"\nStarting asynchronous bulk delete " +
                        $"of {entityList.Count} upserted records...");

                    var retrieveRequest = new RetrieveMultipleRequest(queryUri: retrieveMultipleQuery);

                    var retrieveMultipleResponse2 = await service.SendAsync<RetrieveMultipleResponse>(retrieveRequest);

                    var idsToDelete = new List<Guid>();
                    for (int i = 0; i < numberOfRecords; i++)
                    {
                        idsToDelete.Add((Guid)retrieveMultipleResponse2.Records?[i]?[tablePrimaryKeyName]);
                    }

                    string deleteJobStatus = await Utility.BulkDeleteRecordsByIds(
                        service: service,
                        tableLogicalName: tableLogicalName,
                        iDs: idsToDelete.ToArray(),
                        jobName: "Deleting records created by UpsertMultiple Sample.");

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