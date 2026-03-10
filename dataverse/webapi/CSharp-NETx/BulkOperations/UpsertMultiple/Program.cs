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
                // UpsertMultiple scenarios covered in this sample for x records
                // 1. Create x/4 records letting the system set the primary key
                // 2. Create x/4 records with a unique alternate key value
                // 3. Update x/4 records by providing Primary Key (PK) which is entity.Id
                // 4. Update x/4 records by providing Alternate Key (AK) which is entity.KeyAttribute

                // Create Example table
                await Utility.CreateExampleTable(service: service,
                    tableSchemaName: tableSchemaName,
                    isElastic: Settings.UseElastic);

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
                    if (Settings.UseElastic)
                    {
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
                            { "partitionid", partitionValue},
                            // Each record MUST have the @odata.type specified.
                            { "@odata.type",$"Microsoft.Dynamics.CRM.{tableLogicalName}" }
                        });
                        }

                        // Use CreateMultiple
                        var createMultipleResponse = await SendCreateMultipleRequest(service, entityList);

                        // Prepare above created entity records to be updated by UpsertMultiple

                        Console.WriteLine($"\nPreparing {numRecCreate} records to Upsert(update)..");

                        var numRecUpdatePK = Math.Ceiling((double)(numRecCreate / 2));

                        if (!Settings.CreateAlternateKey)
                        {
#pragma warning disable CS0162 // Unreachable code detected
                            numRecUpdatePK = numRecCreate;
#pragma warning restore CS0162 // Unreachable code detected
                        }

                        // Update the sample_name value. We want to Update num/2 records using Primary Key and num/2 records using Alternate Keys
                        var j = 0;
                        foreach (JObject entity in entityList)
                        {
                            if (j >= numRecUpdatePK && Settings.CreateAlternateKey)
                            {
                                // 3. Upsert(Update) records using AK
                                entity[tablePrimaryNameColumnName] += " UpdatedByAK";
                                entity.Add(new JProperty("@odata.id", $"{tableLogicalName}s({tablePrimaryKeyName}={createMultipleResponse.Ids[j]},partitionid='{partitionValue}')"));
                                entity.Remove("partitionid");
                            }
                            else
                            {
                                // 4. Upsert(Update) records using PK
                                entity[tablePrimaryNameColumnName] += " UpdatedByPK";
                                entity.Add(tablePrimaryKeyName, createMultipleResponse.Ids[j]);
                                entity.Remove("@odata.id");
                            }

                            j++;

                        }

                        var numRecUpsertCreate = numberOfRecords - numRecCreate;

                        var num = Math.Ceiling((double)(numRecUpsertCreate / 2));

                        if (!Settings.CreateAlternateKey)
                        {
                            num = numRecUpsertCreate;
                        }

                        // 1. Create numberOfRecords/4 records to be created using Primary Key
                        Console.WriteLine($"\nPreparing {num} records to Upsert(create) using Primary Key..");
                        for (int i = (int)numRecCreate; i < (num + numRecCreate); i++)
                        {
                            entityList.Add(new JObject() {
                            { "sample_name", $"sample record {i+1:0000000}"},
                            { tablePrimaryKeyName, Guid.NewGuid() },
                            { "partitionid", partitionValue },
                            // Each record MUST have the @odata.type specified.
                            { "@odata.type",$"Microsoft.Dynamics.CRM.{tableLogicalName}" }
                        });
                        }

                        // 2. Create numberOfRecords/4 records to be created using Alternate Key
                        if (Settings.CreateAlternateKey)
                        {
                            Console.WriteLine($"\nPreparing {num} records to Upsert(create) using Alternate Key..");
                            for (int i = (int)(num + numRecCreate); i < numberOfRecords; i++)
                            {
                                entityList.Add(new JObject() {
                            { "sample_name", $"sample record {i+1:0000000}"},
                            { "partitionid", partitionValue },
                            { "@odata.id", $"{tableLogicalName}s({tablePrimaryKeyName}={Guid.NewGuid()},partitionid='{partitionValue}')" },
                            // Each record MUST have the @odata.type specified.
                            { "@odata.type",$"Microsoft.Dynamics.CRM.{tableLogicalName}" }
                        });

                            }
                        }

                        // Use UpsertMultipleRequest

                        await SendUpsertMultipleRequest(service, entityList, true);

                        var retrieveMultipleQuery = $"{tableLogicalName}s?$select={tablePrimaryKeyName},{tablePrimaryNameColumnName}";

                        if (Settings.CreateAlternateKey)
                        {
                            retrieveMultipleQuery += $",partitionid";
                        }

                        await BulkDeleteCreatedRecords(service, "partitionid", retrieveMultipleQuery);
                    }
                    else
                    {
                        // Create a List of entity instances to be updated during UpsertMultiple call.

                        // To handle cases where number of records is an odd number
                        var numRecCreate = Math.Ceiling((double)(numberOfRecords / 2));

                        Console.WriteLine($"\nPreparing {numRecCreate} records to create..");

                        List<JObject> entityList = new();

                        // Populate the list with the number of records to test.
                        for (int i = 0; i < numRecCreate; i++)
                        {
                            entityList.Add(new JObject() {
                            { "sample_name", $"sample record {i+1:0000000}"},
                            // Each record MUST have the @odata.type specified.
                            { "@odata.type",$"Microsoft.Dynamics.CRM.{tableLogicalName}" }
                        });
                            if(Settings.CreateAlternateKey)
                            {
                                entityList[i].Add(new JProperty(tableAlternateKey, $"{i + 1:0000000}"));
                            }
                        }

                        // CreateMultiple
                        var createMultipleResponse = await SendCreateMultipleRequest(service, entityList);

                        // Prepare above created entity records to be updated by UpsertMultiple

                        Console.WriteLine($"\nPreparing {numRecCreate} records to Upsert(update)..");

                        var numRecUpdatePK = Math.Ceiling((double)(numRecCreate / 2));

                        if (!Settings.CreateAlternateKey)
                        {
#pragma warning disable CS0162 // Unreachable code detected
                            numRecUpdatePK = numRecCreate;
#pragma warning restore CS0162 // Unreachable code detected
                        }

                        // Update the sample_name value. We want to Update num/2 records using Primary Key and num/2 records using Alternate Keys
                        var j = 0;
                        foreach (JObject entity in entityList)
                        {
                            if (j >= numRecUpdatePK && Settings.CreateAlternateKey)
                            {
                                // 3. Upsert(Update) records using AK
                                entity[tablePrimaryNameColumnName] += " UpdatedByAK";
                                entity.Add(new JProperty("@odata.id", $"{tableLogicalName}s({tableAlternateKey}='{j + 1:0000000}')"));
                                entity.Remove(tableAlternateKey);
                            }
                            else
                            {
                                // 4. Upsert(Update) records using PK
                                entity[tablePrimaryNameColumnName] += " UpdatedByPK";
                                entity.Add(tablePrimaryKeyName, createMultipleResponse?.Ids[j]);
                                entity.Remove(tableAlternateKey);
                            }

                            j++;

                        }

                        var numRecUpsertCreate = numberOfRecords - numRecCreate;

                        var num = Math.Ceiling((double)(numRecUpsertCreate / 2));

                        if (!Settings.CreateAlternateKey)
                        {
                            num = numRecUpsertCreate;
                        }

                        // 1. Create numberOfRecords/4 records to be created using Primary Key
                        Console.WriteLine($"\nPreparing {num} records to Upsert(create) using Primary Key..");
                        for (int i = (int)numRecCreate; i < (num + numRecCreate); i++)
                        {
                            entityList.Add(new JObject() {
                            {"sample_name", $"sample record {i+1:0000000}"},
                            {tablePrimaryKeyName, Guid.NewGuid() },
                            // Each record MUST have the @odata.type specified.
                            {"@odata.type",$"Microsoft.Dynamics.CRM.{tableLogicalName}" }
                        });
                        }


                        // 2. Create numberOfRecords/4 records to be created using Alternate Key
                        if (Settings.CreateAlternateKey)
                        {
                            Console.WriteLine($"\nPreparing {num} records to Upsert(create) using Alternate Key..");
                            for (int i = (int)(num + numRecCreate); i < numberOfRecords; i++)
                            {
                                entityList.Add(new JObject() {
                            {"sample_name", $"sample record {i+1:0000000}"},
                            { "@odata.id", $"{tableLogicalName}s({tableAlternateKey}='{i+1:0000000}')" },
                            // Each record MUST have the @odata.type specified.
                            {"@odata.type",$"Microsoft.Dynamics.CRM.{tableLogicalName}" }
                        });
                            }
                        }

                        // Use UpsertMultipleRequest

                        await SendUpsertMultipleRequest(service, entityList, true);

                        var retrieveMultipleQuery = $"{tableLogicalName}s?$select={tablePrimaryKeyName},{tablePrimaryNameColumnName}";

                        if (Settings.CreateAlternateKey)
                        {
                            retrieveMultipleQuery += $",{tableAlternateKey}";
                        }
                        await BulkDeleteCreatedRecords(service, tableAlternateKey, retrieveMultipleQuery);
                    }
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

        /// <summary>
        /// Method used to create and send the UpsertMultiple request
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entityList"></param>
        /// <param name="measureDuration"></param>
        private static async Task SendUpsertMultipleRequest(Service service, List<JObject> entityList, bool measureDuration = false)
        {
            // Use UpsertMultiple
            UpsertMultipleRequest upsertMultipleRequest = new(
                entitySetName: tableSetName,
                targets: entityList);

            upsertMultipleRequest.RequestUri = new Uri(
                upsertMultipleRequest.RequestUri.ToString(),
                uriKind: UriKind.Relative);

            Console.WriteLine($"Sending POST request to /{tableSetName}/Microsoft.Dynamics.CRM.UpsertMultiple");

            if (measureDuration)
            {
                Stopwatch upsertStopwatch = Stopwatch.StartNew();
                // Send the request
                await service.SendAsync(upsertMultipleRequest);
                upsertStopwatch.Stop();
                Console.WriteLine($"\tUpserted {entityList.Count} records " +
                    $"in {Math.Round(upsertStopwatch.Elapsed.TotalSeconds)} seconds.");
            }
            else
            {
                await service.SendAsync(upsertMultipleRequest);
            }
        }

        /// <summary>
        /// Bulk Delete all the created records
        /// </summary>
        /// <param name="service"></param>
        /// <param name="alternateKey"></param>
        private static async Task BulkDeleteCreatedRecords(Service service, string alternateKey, string retrieveQuery)
        {
            // Delete created rows asynchronously
            // When testing plug-ins, set break point here to observe data
            Console.WriteLine($"\nStarting asynchronous bulk delete " +
                $"of {numberOfRecords} upserted records...");

            // Retrieve the record ids for records to be updated using PK
            var retrieveMultipleRequest2 = new RetrieveMultipleRequest(
                queryUri: retrieveQuery);


            var retrieveMultipleResponse2 = await service.SendAsync<RetrieveMultipleResponse>(retrieveMultipleRequest2);

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

        /// <summary>
        /// Method used to create and send the CreateMultiple request
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entityList"></param>
        /// <param name="measureDuration"></param>
        private static async Task<CreateMultipleResponse> SendCreateMultipleRequest(Service service, List<JObject> entityList, bool measureDuration = false)
        {
            // Use createMultiple
            CreateMultipleRequest createMultipleRequest = new(
                entitySetName: tableSetName,
                targets: entityList);

            createMultipleRequest.RequestUri = new Uri(
                createMultipleRequest.RequestUri.ToString(),
                uriKind: UriKind.Relative);

            Console.WriteLine($"Sending POST request to /{tableSetName}/Microsoft.Dynamics.CRM.CreateMultiple");

            CreateMultipleResponse? response;
            if (measureDuration)
            {
                Stopwatch createStopwatch = Stopwatch.StartNew();
                // Send the request
                response = await service.SendAsync<CreateMultipleResponse>(createMultipleRequest);
                createStopwatch.Stop();
                Console.WriteLine($"\tcreateed {entityList.Count} records " +
                    $"in {Math.Round(createStopwatch.Elapsed.TotalSeconds)} seconds.");
            }
            else
            {
                response = await service.SendAsync<CreateMultipleResponse>(createMultipleRequest);
            }

            return response;
        }
    }
}