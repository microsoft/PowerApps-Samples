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


        static async Task Main()
        {
            Config config = App.InitializeApp();

            var service = new Service(config);

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

                    // Use CreateMultipleRequest
                    CreateMultipleRequest createMultipleRequest = new(
                        entitySetName: tableSetName,
                        targets: entityList);

                    // Add a tag optional parameter to set a shared variable to be available to a plug-in.
                    // See https://learn.microsoft.com/power-apps/developer/data-platform/optional-parameters?tabs=webapi#add-a-shared-variable-to-the-plugin-execution-context
                    createMultipleRequest.RequestUri = new Uri(
                        createMultipleRequest.RequestUri.ToString() + "?tag=CreateUpdateMultiple",
                        uriKind: UriKind.Relative);

                    if (Settings.BypassCustomPluginExecution)
                    {
#pragma warning disable CS0162 // Unreachable code detected: Configurable by setting
                        createMultipleRequest.Headers.Add("MSCRM.BypassCustomPluginExecution", "true");
#pragma warning restore CS0162 // Unreachable code detected: Configurable by setting
                    }


                    Console.WriteLine($"Sending POST request to /{tableSetName}/Microsoft.Dynamics.CRM.CreateMultiple");
                    Stopwatch createStopwatch = Stopwatch.StartNew();
                    // Send the request
                    var createMultipleResponse = await service.SendAsync<CreateMultipleResponse>(createMultipleRequest);
                    Console.WriteLine($"\tCreated {entityList.Count} records " +
                        $"in {Math.Round(createStopwatch.Elapsed.TotalSeconds)} seconds.");

                    Console.WriteLine($"\nPreparing {numberOfRecords} records to update..");

                    // Assign ID values to created items to prepare to update them.
                    for (int i = 0; i < createMultipleResponse.Ids.Length; i++)
                    {
                        entityList[i].Add(tablePrimaryKeyName, createMultipleResponse.Ids[i]);

                    }

                    // Update the sample_name value:
                    foreach (JObject entity in entityList)
                    {
                        entity[tablePrimaryNameColumnName] += " Updated";
                    }

                    // Use UpdateMultipleRequest
                    UpdateMultipleRequest updateMultipleRequest = new(
                        entitySetName: tableSetName,
                        targets: entityList);

                    // Add a tag optional parameter to set a shared variable to be available to a plug-in.
                    // See https://learn.microsoft.com/power-apps/developer/data-platform/optional-parameters?tabs=webapi#add-a-shared-variable-to-the-plugin-execution-context
                    updateMultipleRequest.RequestUri = new Uri(updateMultipleRequest.RequestUri.ToString() + "?tag=CreateUpdateMultiple", uriKind: UriKind.Relative);

                    if (Settings.BypassCustomPluginExecution)
                    {
#pragma warning disable CS0162 // Unreachable code detected: Configurable by setting
                        updateMultipleRequest.Headers.Add("MSCRM.BypassCustomPluginExecution", "true");
#pragma warning restore CS0162 // Unreachable code detected: Configurable by setting
                    }

                    Console.WriteLine($"Sending POST request to /{tableSetName}/Microsoft.Dynamics.CRM.UpdateMultiple");
                    Stopwatch updateStopwatch = Stopwatch.StartNew();
                    // Send the request
                    await service.SendAsync(updateMultipleRequest);
                    updateStopwatch.Stop();
                    Console.WriteLine($"\tUpdated {entityList.Count} records " +
                        $"in {Math.Round(updateStopwatch.Elapsed.TotalSeconds)} seconds.");

                    // Delete created rows asynchronously
                    // When testing plug-ins, set break point here to observe data
                    Console.WriteLine($"\nStarting asynchronous bulk delete " +
                        $"of {createMultipleResponse.Ids.Length} created records...");

                    string deleteJobStatus = await Utility.BulkDeleteRecordsByIds(
                        service: service,
                        tableLogicalName: tableLogicalName,
                        iDs: createMultipleResponse.Ids,
                        jobName: "Deleting records created by CreateUpdateMultiple Sample.");

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