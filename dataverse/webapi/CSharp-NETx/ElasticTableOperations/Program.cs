using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerApps.Samples;
using PowerApps.Samples.Messages;
using PowerApps.Samples.Metadata.Messages;
using PowerApps.Samples.Metadata.Types;
using PowerApps.Samples.Methods;
using PowerApps.Samples.Types;
using System.Text;

namespace PowerPlatform.Dataverse.CodeSamples
{
    internal class Program
    {
        const string TABLE_SCHEMA_NAME = "contoso_SensorData";
        const string TABLE_SET_NAME = "contoso_sensordatas";

        static async Task Main()
        {
            Config config = App.InitializeApp();

            var service = new Service(config);

            #region Optimize Connection

            // Change max connections from .NET to a remote service default: 2
            System.Net.ServicePointManager.DefaultConnectionLimit = 65000;
            // Bump up the min threads reserved for this app to ramp connections faster - minWorkerThreads defaults to 4, minIOCP defaults to 4 
            ThreadPool.SetMinThreads(100, 100);
            // Turn off the Expect 100 to continue message - 'true' will cause the caller to wait until it round-trip confirms a connection to the server 
            System.Net.ServicePointManager.Expect100Continue = false;
            // Can decrease overall transmission overhead but can cause delay in data packet arrival
            System.Net.ServicePointManager.UseNagleAlgorithm = false;

            #endregion Optimize Connection

            Console.WriteLine($"Starting Elastic table operations sample.\n");

            try
            {
                #region Create Elastic table
                Console.WriteLine("=== Start Region 0: Creating contoso_SensorData table === \n");
                // Define the table and columns
                var sensorDataTable = new EntityMetadata
                {
                    SchemaName = TABLE_SCHEMA_NAME,
                    EntitySetName = TABLE_SET_NAME,
                    DisplayName = new Label("Sensor Data", 1033),
                    DisplayCollectionName = new Label("Sensor Data", 1033),
                    Description = new Label("Stores IoT data emitted from devices", 1033),
                    OwnershipType = OwnershipTypes.UserOwned,
                    TableType = "Elastic", //This makes it an elastic table.
                    IsActivity = false,
                    CanCreateCharts = new BooleanManagedProperty(false),
                    HasActivities = false,
                    HasNotes = false,
                    Attributes = new List<AttributeMetadata> {
                    {
                        new StringAttributeMetadata{
                            SchemaName = "contoso_SensorType",
                            DisplayName = new Label("Sensor Type", 1033),
                            IsPrimaryName = true,
                            MaxLength = 100
                        }
                    },
                    {
                        new StringAttributeMetadata{
                            SchemaName = "contoso_DeviceId",
                            DisplayName = new Label("Device Id", 1033),
                            MaxLength = 1000
                        }
                    },
                    {
                        new IntegerAttributeMetadata
                        {
                            SchemaName = "contoso_Value",
                            DisplayName = new Label("Value", 1033),
                            MinValue = int.MinValue,
                            MaxValue = int.MaxValue
                        }
                    },
                    {
                        new DateTimeAttributeMetadata{
                            SchemaName = "contoso_TimeStamp",
                            DisplayName = new Label("Time Stamp", 1033)
                        }
                    },
                    {
                        new StringAttributeMetadata{
                            SchemaName = "contoso_EnergyConsumption",
                            DisplayName = new Label("Energy Consumption", 1033),
                            Description = new Label("Stores unstructured energy consumption data as reported by device", 1033),
                            FormatName = new StringFormatName(StringFormatNameValues.Json),
                            MaxLength = 1000
                        }
                    },
                }
                };

                Console.WriteLine($"Creating the {TABLE_SCHEMA_NAME} table...");

                var createEntityRequest = new CreateEntityRequest(
                    entityMetadata: sensorDataTable, 
                    useStrongConsistency: true);

                var createEntityResponse = await service.SendAsync<CreateEntityResponse>(createEntityRequest);

                Console.WriteLine($"{TABLE_SCHEMA_NAME} table created.");

                #endregion Create Elastic table

                /*
                
                Unlike the ElasticTableOperations sample for the SDK for .NET, this sample only includes examples that set
                the partitionid. If you are not using an partioning strategy, code using WebAPIServiceClient
                is the same as with standard tables. This is because WebAPIServiceClient manages the sessionToken for you,
                demonstrating how this can be managed in your .NET code.

                In PowerApps-Samples\dataverse\webapi\CSharp-NETx\WebAPIService\Service.cs

                Within the WebAPIServiceClient.SendAsync method:

                This code includes the MSCRM.SessionToken header with all GET operations before the request is sent

                    // Session token used by elastic tables to enable strong consistency
                    // See https://learn.microsoft.com/power-apps/developer/data-platform/use-elastic-tables?tabs=webapi#sending-the-session-token
                    if (!string.IsNullOrWhiteSpace(_sessionToken) && request.Method == HttpMethod.Get) {
                        request.Headers.Add("MSCRM.SessionToken", _sessionToken);
                    }
                
                This code captures the current session token, if it is included, after every response is recieved.

                    // Capture the current session token value
                    // See https://learn.microsoft.com/power-apps/developer/data-platform/use-elastic-tables?tabs=webapi#getting-the-session-token
                    if (response.Headers.Contains("x-ms-session-token"))
                    {
                        _sessionToken = response.Headers.GetValues("x-ms-session-token")?.FirstOrDefault()?.ToString();
                    }

                */


                #region Create Record
                Console.WriteLine("\n=== Start Region 1: Create Record === \n");

                // Using deviceId as the partitionid in this sample.
                string deviceId = "Device-ABC-1234";

                JObject sensorDataObjCreate = new()
                        {
                            {"contoso_deviceid", deviceId },
                            {"contoso_sensortype", "Humidity" },
                            {"contoso_value", 40 },
                            {"contoso_timestamp", DateTime.UtcNow },
                            {"partitionid",deviceId }, //Setting the partitionid with deviceid value
                            {"ttlinseconds", 86400 } // 86400  seconds in a day
                        };

                // This sensorDataRef entity reference refers to the created record using using alternate key style:
                // /contoso_sensordatas(contoso_sensordataid=7fae9aa4-12f8-ed11-8849-000d3a993550,partitionid='Device-ABC-1234')
                EntityReference sensorDataRef = await service.Create(
                    entitySetName: "contoso_sensordatas",
                    record: sensorDataObjCreate);


                Console.WriteLine($"Created sensor data record at:{sensorDataRef.Path}");


                #endregion Create Record

                #region Update Record
                Console.WriteLine("\n=== Start Region 2: Update Record === \n");

                JObject sensorDataObjUpdate1 = new()
                        {
                            {"contoso_value", 60 }
                        };

                // Using partitionId parameter
                await service.Update(
                    entityReference: sensorDataRef,
                    record: sensorDataObjUpdate1,
                    partitionId: deviceId); //Including the partiionid for update

                Console.WriteLine($"Updated sensor data record using partitionId parameter.");

                JObject sensorDataObjUpdate2 = new()
                        {
                            {"contoso_value", 80 }
                        };

                // Without partitionId parameter
                await service.Update(
                    entityReference: sensorDataRef, //Alternate key only
                    record: sensorDataObjUpdate2);

                Console.WriteLine($"Updated sensor data record using alternate key style.");

                #endregion Update Record

                #region Retrieve Record
                Console.WriteLine("\n=== Start Region 3: Retrieve Record === \n");


                JObject retrievedSensorDataRecord = await service.Retrieve(
                    entityReference: sensorDataRef,
                    query: "?$select=contoso_value",
                    partitionId: deviceId); //With partitionid

                Console.WriteLine($"Retrieved sensor data record using partitionId:\n");

                Console.WriteLine($"{retrievedSensorDataRecord}\n");

                retrievedSensorDataRecord = await service.Retrieve(
                    entityReference: sensorDataRef, //With alternate key only
                    query: "?$select=contoso_value");

                Console.WriteLine($"Retrieved sensor data record using alternate key style:\n");

                Console.WriteLine($"{retrievedSensorDataRecord}");

                #endregion Retrieve Record

                #region Upsert Record
                Console.WriteLine("\n=== Start Region 4: Upsert Record === \n");

                JObject sensorDataObjForUpsert = new()
                        {

                // For Upsert it is required to set all the attribute values
                // If matching record is found, all data is replaced.

                            {"contoso_deviceid", deviceId },
                            {"contoso_sensortype", "Humidity" },
                            {"contoso_value", 60 },
                            {"contoso_timestamp", DateTime.UtcNow },
                            {"partitionid",deviceId },
                            {"ttlinseconds", 86400 } // 86400  seconds in a day
                        };

                // It isn't possible to set the partitionId parameter for upsert.
                // The value must be included in the body.
                EntityReference upsertReference = await service.Upsert(
                    entityReference: sensorDataRef,
                    record: sensorDataObjForUpsert,
                    upsertBehavior: UpsertBehavior.CreateOrUpdate);

                Console.WriteLine($"Upserted sensor data record at {upsertReference.Path}");
  
                #endregion Upsert Record

                #region Delete Record
                Console.WriteLine("\n=== Start Region 5: Delete Record === \n");

                await service.Delete(entityReference: sensorDataRef, partitionId: deviceId);
                Console.WriteLine($"Deleted sensor data record with partitionId.\n");
             
                #endregion Delete Record

                #region CreateMultiple
                Console.WriteLine("\n=== Start Region 6: Demonstrate CreateMultiple === \n");


                Console.WriteLine($"Creating {Settings.NumberOfRecords} records to use for query example...");

                List<JObject> recordsToCreate = new();

                for (int i = 0; i < Settings.NumberOfRecords; i++)
                {
                    JObject sensordata = new()
                    {
                        // CreateMultiple payload must specify the @odata.type
                        {"@odata.type", $"Microsoft.Dynamics.CRM.{TABLE_SCHEMA_NAME.ToLower()}" },
                        {"contoso_deviceid", deviceId },
                        {"contoso_sensortype", "Humidity" },
                        {"partitionid", deviceId },
                        {"ttlinseconds", 86400 } // 86400  seconds in a day
                    };

                    recordsToCreate.Add(sensordata);

                }

                var parallelOptions = new ParallelOptions()
                {
                    MaxDegreeOfParallelism = service.RecommendedDegreeOfParallelism
                };

                // ID values to update and delete records later
                List<Guid> createdRecordIds = new();

                // Sending CreateMultiple requests in parallel:
                await Parallel.ForEachAsync(
                        source: recordsToCreate.Chunk(Settings.BatchSize),
                        parallelOptions: parallelOptions,
                        async (targets, cancellationToken) =>
                        {

                            CreateMultipleRequest request = new(entitySetName: "contoso_sensordatas", targets: targets.ToList());

                            var response = await service.SendAsync<CreateMultipleResponse>(request);

                            // So these records can be updated and deleted later
                            createdRecordIds.AddRange(response.Ids.ToList());

                            Console.WriteLine($"\t{response.Ids.Length} records created using CreateMultiple");

                        });

                Console.WriteLine($"{Settings.NumberOfRecords} records to use for query example created.");

                #endregion CreateMultiple

                #region UpdateMultiple

                Console.WriteLine("\n=== Start Region 7: Demonstrate UpdateMultiple === \n");

                List<JObject> recordsToUpdate = new();

                int count = 1;
                createdRecordIds.ForEach(id =>
                {

                    EnergyConsumption energyConsumption = new()
                    {
                        Voltage = count,
                        VoltageUnit = "Volts",
                        Power = count + count,
                        PowerUnit = "Watts"
                    };

                    JObject sensordata = new()
                    {
                        // UpdateMultiple payload must specify the @odata.type
                        {"@odata.type", $"Microsoft.Dynamics.CRM.{TABLE_SCHEMA_NAME.ToLower()}" },
                        {$"{TABLE_SCHEMA_NAME.ToLower()}id", id },
                        {"partitionid", deviceId },
                        {"contoso_energyconsumption",JsonConvert.SerializeObject(energyConsumption) },
                    };

                    recordsToUpdate.Add(sensordata);
                    count++;

                });

                // Sending UpdateMultiple requests in parallel:
                await Parallel.ForEachAsync(
                        source: recordsToUpdate.Chunk(Settings.BatchSize),
                        parallelOptions: parallelOptions,
                        async (targets, cancellationToken) =>
                        {

                            UpdateMultipleRequest request = new(entitySetName: "contoso_sensordatas", targets: targets.ToList());

                            await service.SendAsync(request);

                            Console.WriteLine($"\t{targets.Length} records updated using UpdateMultiple");

                        });

                Console.WriteLine($"{recordsToUpdate.Count} records updated using UpdateMultiple.");
                #endregion UpdateMultiple

                #region Demonstrate ExecuteCosmosSqlQuery
                Console.WriteLine("\n=== Start Region 8: Demonstrate ExecuteCosmosSqlQuery === \n");

                StringBuilder sb = new();
                sb.Append("select c.props.contoso_deviceid as deviceId, ");
                sb.Append("c.props.contoso_timestamp as timestamp, ");
                sb.Append("c.props.contoso_energyconsumption.power as power ");
                sb.Append("from c where c.props.contoso_sensortype=@sensortype and ");
                sb.Append("c.props.contoso_energyconsumption.power > @power");


                ExecuteCosmosSqlQueryRequest queryRequest = new(
                    queryText: sb.ToString(),
                    entityLogicalName: "contoso_sensordata")
                {
                    QueryParameters = new ParameterCollection()
                    {
                        Keys = new List<string> { "@sensortype", "@power" },
                        Values = new List<PowerApps.Samples.Types.Object> {
                            { new PowerApps.Samples.Types.Object(ObjectType.String,"Humidity") },
                            { new PowerApps.Samples.Types.Object(ObjectType.Int,"5") }
                        }
                    },
                    PageSize = 50,
                    PartitionId = deviceId
                };

                var queryResponse = await service.SendAsync<ExecuteCosmosSqlQueryResponse>(queryRequest);

                Console.WriteLine($"ExecuteCosmosSqlQueryResponse.PagingCookie: {queryResponse.PagingCookie}\n");
                Console.WriteLine($"ExecuteCosmosSqlQueryResponse.HasMore: {queryResponse.HasMore}\n");

                Console.WriteLine($"Output first page of 50 results:\n");

                queryResponse.Result.ForEach(result =>
                {
                    Console.WriteLine($"\t{result["deviceId"]} {result["power"]}");
                });

                while (queryResponse.HasMore)
                {
                    ExecuteCosmosSqlQueryRequest pagedQueryRequest = new(
                        queryText: sb.ToString(),
                        entityLogicalName: "contoso_sensordata")
                    {
                        QueryParameters = new ParameterCollection()
                        {
                            Keys = new List<string> { "@sensortype", "@power" },
                            Values = new List<PowerApps.Samples.Types.Object> {
                                    { new PowerApps.Samples.Types.Object(ObjectType.String,"Humidity") },
                                    { new PowerApps.Samples.Types.Object(ObjectType.Int,"5") }
                                }
                        },
                        PageSize = 50,
                        PartitionId = deviceId,
                        PagingCookie = queryResponse.PagingCookie
                    };

                    queryResponse = await service.SendAsync<ExecuteCosmosSqlQueryResponse>(pagedQueryRequest);

                    Console.WriteLine($"\nOutput additional page of 50 results:\n");

                    queryResponse.Result.ForEach(result =>
                    {
                        Console.WriteLine($"\t{result["deviceId"]} {result["power"]}");
                    });
                }


                #endregion Demonstrate ExecuteCosmosSqlQuery

                #region Demonstrate DeleteMultiple

                Console.WriteLine("\n=== Start Region 9: Demonstrate DeleteMultiple === \n");

                List<JObject> recordsToDelete = new();

                createdRecordIds.ForEach(id =>
                {

                    JObject sensordata = new()
                    {
                        // DeleteMultiple payload must specify the @odata.type
                        {"@odata.type", $"Microsoft.Dynamics.CRM.{TABLE_SCHEMA_NAME.ToLower()}" },
                        {$"{TABLE_SCHEMA_NAME.ToLower()}id", id },
                        {"partitionid", deviceId }
                    };

                    recordsToDelete.Add(sensordata);

                });

                // Sending DeleteMultiple requests in parallel:
                await Parallel.ForEachAsync(
                        source: recordsToDelete.Chunk(Settings.BatchSize),
                        parallelOptions: parallelOptions,
                        async (targets, cancellationToken) =>
                        {

                            DeleteMultipleRequest request = new(entitySetName: "contoso_sensordatas", targets: targets.ToList());

                            await service.SendAsync(request);

                            Console.WriteLine($"\t{targets.Length} records deleted using DeleteMultiple");

                        });

                Console.WriteLine($"{recordsToDelete.Count} records deleted using DeleteMultiple.");

                #endregion Demonstrate DeleteMultiple

            }
            catch (Exception ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine(ex.Message);
                throw ex;
            }
            finally
            {

                #region Delete Table
                Console.WriteLine("\n=== Start Region 10: Delete Table === \n");
                Console.WriteLine($"\nDeleting the {TABLE_SCHEMA_NAME} table...");

                Dictionary<string, string> keys = new() {
                    {"LogicalName",$"'{TABLE_SCHEMA_NAME.ToLower()}'"}
                };

                var tableReference = new EntityReference(setName: "EntityDefinitions", keyAttributes: keys);

                await service.Delete(tableReference);

                Console.WriteLine($"{TABLE_SCHEMA_NAME} table deleted.");

                #endregion Delete Table

                Console.WriteLine("\n=== Web API ElasticTableOperations Sample Completed === \n");
            }

        }
    }
}