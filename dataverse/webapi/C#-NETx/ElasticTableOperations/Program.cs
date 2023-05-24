using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerApps.Samples;
using PowerApps.Samples.Batch;
using PowerApps.Samples.Messages;
using PowerApps.Samples.Metadata.Messages;
using PowerApps.Samples.Metadata.Types;
using PowerApps.Samples.Methods;
using PowerApps.Samples.Types;
using System.Text;

namespace ElasticTableOperations
{
    internal class Program
    {
        const string TABLE_SCHEMA_NAME = "contoso_SensorData";
        const int RECORDS_TO_CREATE_FOR_QUERY = 100;

        static async Task Main()
        {
            Config config = App.InitializeApp();

            var service = new Service(config);

            Console.WriteLine($"Starting Elastic table operations sample.");

            #region Create Elastic table

            // Define the table and columns
            var sensorDataTable = new EntityMetadata
            {
                SchemaName = TABLE_SCHEMA_NAME,
                DisplayName = new Label("Sensor Data", 1033),
                DisplayCollectionName = new Label("Sensor Data", 1033),
                Description = new Label("Stores IoT data emitted from devices", 1033),
                OwnershipType = OwnershipTypes.UserOwned,
                TableType = "Elastic",
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

            var createEntityRequest = new CreateEntityRequest(sensorDataTable);
            var createEntityResponse = await service.SendAsync<CreateEntityResponse>(createEntityRequest);

            Console.WriteLine($"{TABLE_SCHEMA_NAME} table created.");

            #endregion Create Elastic table

            #region Create Record

            // Using deviceId as the partitionid in this sample.
            string deviceId = "Device-ABC-1234";

            JObject sensorDataObjCreate = new()
            {
                {"contoso_deviceid", deviceId },
                {"contoso_sensortype", "Humidity" },
                {"contoso_value", 40 },
                {"contoso_timestamp", DateTime.UtcNow },
                {"partitionid",deviceId },
                {"ttlinseconds", 86400 } // 86400  seconds in a day
            };

            // This entity reference referrs to the created record using regular style:
            // /contoso_sensordatas(7fae9aa4-12f8-ed11-8849-000d3a993550)
            EntityReference sensorDataRef = await service.Create(
                entitySetName: "contoso_sensordatas",
                record: sensorDataObjCreate);

            var keyAttributes = new Dictionary<string, string>()
            {
                { "contoso_sensordataid",sensorDataRef.Id.ToString() },
                { "partitionid",$"'{deviceId}'" }
            };

            // This entity reference referrs to the created record using alternate key style:
            // /contoso_sensordatas(contoso_sensordataid=7fae9aa4-12f8-ed11-8849-000d3a993550,partitionid='Device-ABC-1234')

            EntityReference sensorDataAltKeyRef = new(setName: "contoso_sensordatas", keyAttributes: keyAttributes);

            Console.WriteLine($"Created sensor data record with id:{sensorDataRef.Id}");


            #endregion Create Record

            #region Update Record

            JObject sensorDataObjUpdate1 = new()
            {
                {"contoso_value", 60 }
            };

            // Using partitionId parameter
            await service.Update(
                entityReference: sensorDataRef,
                record: sensorDataObjUpdate1,
                partitionId: deviceId);

            Console.WriteLine($"Updated sensor data record using partitionId parameter.");

            JObject sensorDataObjUpdate2 = new()
            {
                {"contoso_value", 80 }
            };

            // Using alternatekey parameter
            await service.Update(
                entityReference: sensorDataAltKeyRef,
                record: sensorDataObjUpdate2);

            Console.WriteLine($"Updated sensor data record using alternate key style.");

            #endregion Update Record

            #region Retrieve Record

            JObject retrievedSensorDataRecord = await service.Retrieve(
                entityReference: sensorDataRef,
                query: "?$select=contoso_value",
                partitionId: deviceId);

            Console.WriteLine($"Retrieved sensor data record using partitionId:\n");

            Console.WriteLine($"{retrievedSensorDataRecord}\n");

            retrievedSensorDataRecord = await service.Retrieve(
                entityReference: sensorDataAltKeyRef,
                query: "?$select=contoso_value");

            Console.WriteLine($"Retrieved sensor data record using alternate key style:\n");

            Console.WriteLine($"{retrievedSensorDataRecord}\n");

            #endregion Retrieve Record

            #region Upsert Record

            JObject sensorDataObjForUpsert = new()
            {
                {"contoso_deviceid", deviceId },
                {"contoso_sensortype", "Humidity" },
                {"contoso_value", 40 },
                {"contoso_timestamp", DateTime.UtcNow },
                {"partitionid",deviceId },
                {"ttlinseconds", 86400 } // 86400  seconds in a day
            };

            EntityReference testReference1 = await service.Upsert(
                entityReference: sensorDataRef,
                sensorDataObjForUpsert,
                upsertBehavior: UpsertBehavior.CreateOrUpdate);

            Console.WriteLine($"Upsert sensor data record:\n");
            Console.WriteLine($"Same ID values?:{testReference1.Id == sensorDataRef.Id}");

            // Using alternate key

            EntityReference testReference2 = await service.Upsert(
                entityReference: sensorDataAltKeyRef,
                sensorDataObjForUpsert,
                upsertBehavior: UpsertBehavior.CreateOrUpdate);

            Console.WriteLine($"Upsert sensor data record with alternate key:\n");
            Console.WriteLine($"Same ID values?:{testReference2.Id == sensorDataRef.Id}");

            #endregion Upsert Record

            #region Delete Record

            await service.Delete(entityReference: sensorDataRef, partitionId: deviceId);
            Console.WriteLine($"Deleted sensor data record with partitionId.\n");

            //You can also use the alternate key:

            //await service.Delete(entityReference: sensorDataAltKeyRef);
            //Console.WriteLine($"Deleted sensor data record with alternate key\n");

            #endregion Delete Record

            #region Demonstrate ExecuteCosmosSqlQuery

            // Create Multiple records to query

            Console.WriteLine($"Creating {RECORDS_TO_CREATE_FOR_QUERY} records to use for query example...");

            // Create requests to send in $batch
            List<HttpRequestMessage> requestList = new();

            for (int i = 0; i < RECORDS_TO_CREATE_FOR_QUERY; i++)
            {
                EnergyConsumption energyConsumption = new()
                {
                    Voltage = i,
                    VoltageUnit = "Volts",
                    Power = i + i,
                    PowerUnit = "Watts"
                };

                JObject sensordata = new()
                {
                    {"contoso_deviceid", deviceId },
                    {"contoso_sensortype", "Humidity" },
                    {"partitionid",deviceId },
                    {"contoso_energyconsumption",JsonConvert.SerializeObject(energyConsumption) },
                    {"ttlinseconds", 86400 } // 86400  seconds in a day
                };

                CreateRequest request = new(entitySetName: "contoso_sensordatas", record: sensordata);
                requestList.Add(request);

            }
            BatchRequest batchRequest = new(serviceBaseAddress: service.BaseAddress)
            {
                Requests = requestList
            };

            BatchResponse batchResponse = await service.SendAsync<BatchResponse>(batchRequest);

            Console.WriteLine($"{RECORDS_TO_CREATE_FOR_QUERY} records to use for query example created.");

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

            Console.WriteLine($"Output first page of 50 results:\n");

            queryResponse.Result.ForEach(result =>
            {
                Console.WriteLine($"\t{result["deviceId"]} {result["power"]}");
            });

            Console.WriteLine($"Output additional page of 50 results:\n");

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

                queryResponse.Result.ForEach(result =>
                {
                    Console.WriteLine($"\t{result["deviceId"]} {result["power"]}");
                });

            }


            #endregion Demonstrate ExecuteCosmosSqlQuery


            #region Delete Table

            Console.WriteLine($"\nDeleting the {TABLE_SCHEMA_NAME} table...");

            await service.Delete(createEntityResponse.TableReference);

            Console.WriteLine($"{TABLE_SCHEMA_NAME} table deleted.");

            #endregion Delete Table

            Console.WriteLine($"Elastic table operations sample completed.");

        }
    }
}