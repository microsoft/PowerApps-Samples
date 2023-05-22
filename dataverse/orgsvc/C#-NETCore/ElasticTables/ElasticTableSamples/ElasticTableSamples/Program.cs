using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System.Net;

namespace PowerPlatform.Dataverse.CodeSamples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string path = Environment.GetEnvironmentVariable("DATAVERSE_APPSETTINGS") ?? "appsettings.json";
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile(path, optional: false, reloadOnChange: true).Build();
            ServiceClient client = new ServiceClient(configuration.GetConnectionString("default"));

            try
            {
                Utility.CreateSensorDataEntity(client);

                // using deviceId as the partitionid for these samples.
                string deviceId = "Device-ABC-1234";
                string sessionToken = null;
                Guid contosoSensorDataId1 = CreateRecord(client, deviceId, ref sessionToken);
                UpdateRecord(client, contosoSensorDataId1, deviceId, ref sessionToken);
                UpdateRecordWithAlternateKey(client, contosoSensorDataId1, deviceId, ref sessionToken);

                RetrieveRecord(client, contosoSensorDataId1, deviceId, sessionToken);
                RetrieveRecordWithAlternateKey(client, contosoSensorDataId1, deviceId, sessionToken);

                UpsertRecord(client, contosoSensorDataId1, deviceId, sessionToken);
                DeleteRecord(client, contosoSensorDataId1, deviceId, sessionToken);

                // Create another record
                Guid contosoSensorDataId2 = CreateRecord(client, deviceId, ref sessionToken);
                DeleteRecordWithAlternateKey(client, contosoSensorDataId2, deviceId, sessionToken);

                List<Guid> createdRecordIds = CreateMultipleRecords(client, deviceId);
                ExecuteCosmosSqlQuery(client, deviceId);
                UpdateMultipleRecords(client, deviceId, createdRecordIds);

                // Delete all the data that is created.
                DeleteMultipleRecords(client, deviceId, createdRecordIds);
            }
            finally
            {
                Utility.DeleteSensorDataEntity(client);
            }
        }

        private static Guid CreateRecord(IOrganizationService service, string deviceId, ref string sessionToken)
        {
            Console.WriteLine($"Creating {"contoso_sensordata"} record..\n");

            Entity entity = new Entity("contoso_sensordata")
            {
                Attributes =
                {
                    { "contoso_deviceid", deviceId },
                    { "contoso_sensortype", "Humidity" },
                    { "contoso_value", 40 },
                    { "contoso_timestamp", DateTime.UtcNow},
                    { "partitionid", deviceId },
                    { "ttlinseconds", 86400  }  // 86400  seconds in a day
                }
            };

            CreateRequest request = new CreateRequest
            {
                Target = entity
            };

            CreateResponse response = (CreateResponse)service.Execute(request);

            Console.WriteLine($"{"contoso_sensordata"} record created.\n");

            // Capture the session token
            sessionToken = response.Results["x-ms-session-token"].ToString();

            return response.id;
        }

        private static void UpdateRecord(IOrganizationService service, Guid contosoSensorDataId, string deviceId, ref string sessionToken)
        {
            Console.WriteLine($"Updating {"contoso_sensordata"} record.. \n");

            Entity entity = new Entity("contoso_sensordata")
            {
                Attributes =
                {
                    { "contoso_sensordataid", contosoSensorDataId },
                    { "partitionid", deviceId },
                    { "contoso_value", 60 },
                    { "contoso_timestamp", DateTime.UtcNow },
                }
            };

            UpdateRequest request = new UpdateRequest
            {
                Target = entity,
                ["SessionToken"] = sessionToken
            };

            UpdateResponse response = (UpdateResponse)service.Execute(request);
            Console.WriteLine($"Updated {"contoso_sensordata"} record.\n");

            // Capture the session token
            sessionToken = response.Results["x-ms-session-token"].ToString();
        }

        private static void UpdateRecordWithAlternateKey(IOrganizationService service, Guid contosoSensorDataId, string deviceId, ref string sessionToken)
        {
            Console.WriteLine($"Updating {"contoso_sensordata"} record with alternate key.. \n");
            KeyAttributeCollection keys = new KeyAttributeCollection()
            {
                { "contoso_sensordataid", contosoSensorDataId },
                { "partitionid", deviceId }
            };

            Entity entity = new Entity("contoso_sensordata", keys)
            {
                Attributes =
                {
                    { "contoso_value", 60 },
                    { "contoso_timestamp", DateTime.UtcNow }
                }
            };

            UpdateRequest request = new UpdateRequest
            {
                Target = entity,
                ["SessionToken"] = sessionToken
            };

            UpdateResponse response = (UpdateResponse)service.Execute(request);
            Console.WriteLine($"Updated {"contoso_sensordata"} record with alternate key.\n");

            // Capture the session token
            sessionToken = response.Results["x-ms-session-token"].ToString();
        }

        private static void RetrieveRecord(IOrganizationService service, Guid contosoSensorDataId, string deviceId, string sessionToken)
        {
            Console.WriteLine($"Retrieving {"contoso_sensordata"} record.. \n");

            EntityReference entityReference = new EntityReference("contoso_sensordata", contosoSensorDataId);

            RetrieveRequest request = new RetrieveRequest
            {
                ColumnSet = new ColumnSet("contoso_value"),
                Target = entityReference,
                ["partitionId"] = deviceId,
                ["SessionToken"] = sessionToken
            };
            RetrieveResponse response = (RetrieveResponse)service.Execute(request);

            Console.WriteLine($"Retrieved {"contoso_sensordata"} record.\n");
            Console.WriteLine($"contoso_value: {response.Entity.GetAttributeValue<int>("contoso_value")}");
        }

        private static void RetrieveRecordWithAlternateKey(IOrganizationService service, Guid contosoSensorDataId, string deviceId, string sessionToken)
        {
            Console.WriteLine($"Retrieving {"contoso_sensordata"} record with alternate key.. \n");
            KeyAttributeCollection keys = new KeyAttributeCollection()
            {
                { "contoso_sensordataid", contosoSensorDataId },
                { "partitionid", deviceId }
            };

            EntityReference entityReference = new EntityReference("contoso_sensordata", keys);

            RetrieveRequest request = new RetrieveRequest
            {
                ColumnSet = new ColumnSet("contoso_value"),
                Target = entityReference,
                ["SessionToken"] = sessionToken
            };
            RetrieveResponse response = (RetrieveResponse)service.Execute(request);

            Console.WriteLine($"Retrieved {"contoso_sensordata"} record with alternate key.\n");
            Console.WriteLine($"contoso_value: {response.Entity.GetAttributeValue<int>("contoso_value")}");
        }

        private static bool UpsertRecord(IOrganizationService service, Guid contosoSensorDataId, string deviceId, string sessionToken)
        {
            Console.WriteLine($"Upserting {"contoso_sensordata"} record.. \n");
            Entity entity = new Entity("contoso_sensordata", contosoSensorDataId)
            {
                Attributes =
                {
                    { "contoso_deviceid", deviceId },
                    { "contoso_sensortype".ToLower(), "Humidity" },
                    { "contoso_value", 60 },
                    { "contoso_timestamp", DateTime.UtcNow },
                    { "partitionid", deviceId },
                    { "ttlinseconds", 86400 }
                }
            };

            UpsertRequest request = new UpsertRequest
            {
                Target = entity,
                ["SessionToken"] = sessionToken
            };

            UpsertResponse response = (UpsertResponse)service.Execute(request);

            Console.WriteLine($"Upserted {"contoso_sensordata"} record.\n");

            // Capture the session token
            sessionToken = response.Results["x-ms-session-token"].ToString();

            return response.RecordCreated;
        }

        private static void DeleteRecord(IOrganizationService service, Guid contosoSensorDataId, string deviceId, string sessionToken)
        {
            Console.WriteLine($"Deleting {"contoso_sensordata"} record.. \n");
            DeleteRequest request = new DeleteRequest
            {
                Target = new EntityReference("contoso_sensordata", contosoSensorDataId),
                ["partitionId"] = deviceId,
                ["SessionToken"] = sessionToken
            };

            service.Execute(request);
            Console.WriteLine($"Deleted {"contoso_sensordata"} record.\n");
        }

        private static void DeleteRecordWithAlternateKey(IOrganizationService service, Guid contosoSensorDataId, string deviceId, string sessionToken)
        {
            Console.WriteLine($"Deleting {"contoso_sensordata"} record with alternate key.. \n");
            KeyAttributeCollection keyAttributeCollection = new()
            {
                ["contoso_sensordataid"] = contosoSensorDataId,
                ["partitionid"] = deviceId
            };

            DeleteRequest request = new DeleteRequest
            {
                Target = new EntityReference("contoso_sensordata", keyAttributeCollection),
                ["SessionToken"] = sessionToken
            };

            service.Execute(request);
            Console.WriteLine($"Deleted {"contoso_sensordata"} record with alternate key.\n");
        }

        private static List<Guid> CreateMultipleRecords(IOrganizationService service, string deviceId)
        {
            int numberOfRecords = Settings.NumberOfRecords;
            List<Guid> createdRecordIds = new List<Guid>();

            // Create a List of entity instances.
            Console.WriteLine($"Preparing {numberOfRecords} records to create..\n");
            List<Entity> entityList = new();

            // Populate the list with the number of records to test.
            for (int i = 0; i < numberOfRecords; i++)
            {
                Entity entity = new Entity("contoso_sensordata")
                {
                    Attributes =
                    {
                        { "contoso_sensortype", $"Humidity" },
                        { "partitionid", deviceId }
                    }
                };

                EnergyConsumption energyConsumption = new()
                {
                    Voltage = i,
                    VoltageUnit = "Volts",
                    Power = i + 1,
                    PowerUnit = "Watts"
                };

                // contoso_energyconsumption is a string attribute with json format.
                entity["contoso_energyconsumption"] = JsonConvert.SerializeObject(energyConsumption);

                entityList.Add(entity);
            }

            int skipRecordCount = 0;

            // Executing CreateMultiple in batches.
            List<Entity> entitiesToProcess;
            while ((entitiesToProcess = entityList.Skip(skipRecordCount).Take(Settings.BatchSize).ToList()) != null
                && entitiesToProcess.Count > 0)
            {
                createdRecordIds.AddRange(CreateMultiple(service, entitiesToProcess));
                skipRecordCount += entitiesToProcess.Count;
                entitiesToProcess.Clear();
            }

            return createdRecordIds;
        }

        private static Guid[] CreateMultiple(IOrganizationService service, List<Entity> entities)
        {
            Console.WriteLine($"Creating {entities.Count} {"contoso_sensordata"} records.. \n");
            EntityCollection entityCollection = new(entities)
            {
                EntityName = "contoso_sensordata"
            };

            CreateMultipleRequest createMultipleRequest = new()
            {
                Targets = entityCollection,
            };
            CreateMultipleResponse createMultipleResponse = (CreateMultipleResponse)service.Execute(createMultipleRequest);

            Console.WriteLine($"Created {entities.Count()} {"contoso_sensordata"} records.\n");
            return createMultipleResponse.Ids;
        }

        private static List<EnergyConsumption> ExecuteCosmosSqlQuery(IOrganizationService service, string deviceId)
        {
            Console.WriteLine($"Requesting ExecuteCosmosSqlQuery.. \n");
            OrganizationRequest request = new OrganizationRequest("ExecuteCosmosSqlQuery");

            request.Parameters["EntityLogicalName"] = "contoso_sensordata";
            request.Parameters["QueryText"] = $"select c.props.contoso_deviceid as deviceId, c.props.contoso_timestamp as timestamp, c.props.contoso_energyconsumption.power as power " +
                $"from c where c.props.contoso_sensortype='Humidity' and c.props.contoso_energyconsumption.power > 50";
            request.Parameters["PartitionId"] = deviceId;

            OrganizationResponse response = service.Execute(request);

            // Deserialized query result into a class with expected schema.
            Entity result = (Entity)response.Results["Result"];
            List<EnergyConsumption> energyConsumptions = JsonConvert.DeserializeObject<List<EnergyConsumption>>(result["Result"].ToString());
            Console.WriteLine($"Returned {energyConsumptions.Count} records from ExecuteCosmosSqlQuery.\n");

            return energyConsumptions;
        }

        private static void UpdateMultipleRecords(IOrganizationService service, string deviceId, List<Guid> createdRecordIds)
        {
            Console.WriteLine($"Preparing {createdRecordIds.Count} records to update..\n");
            List<Entity> entityList = new();

            // Populate the list with the number of records to test.
            for (int i = 0; i < createdRecordIds.Count; i++)
            {
                Entity entity = new Entity("contoso_sensordata", createdRecordIds[i])
                {
                    Attributes =
                    {
                        { "partitionid", deviceId }
                    }
                };

                // updating this.
                EnergyConsumption energyConsumption = new()
                {
                    Voltage = i + 1,
                    VoltageUnit = "Volts",
                    Power = i + 2,
                    PowerUnit = "Watts"
                };

                entity["contoso_energyconsumption"] = JsonConvert.SerializeObject(energyConsumption);

                entityList.Add(entity);
            }

            int skipRecordCount = 0;

            // Executing UpdateMultiple in batches.
            List<Entity> entitiesToProcess;
            while ((entitiesToProcess = entityList.Skip(skipRecordCount).Take(Settings.BatchSize).ToList()) != null
                && entitiesToProcess.Count > 0)
            {
                UpdateMultiple(service, entitiesToProcess);
                skipRecordCount += entitiesToProcess.Count;
                entitiesToProcess.Clear();
            }
        }

        private static void UpdateMultiple(IOrganizationService service, List<Entity> entities)
        {
            Console.WriteLine($"Updating {entities.Count} {"contoso_sensordata"} records.. \n");

            // Create an EntityCollection populated with the list of entities.
            EntityCollection entityCollection = new(entities)
            {
                EntityName = "contoso_sensordata"
            };

            UpdateMultipleRequest updateMultipleRequest = new()
            {
                Targets = entityCollection,
            };
            service.Execute(updateMultipleRequest);

            Console.WriteLine($"Updated {entities.Count} {"contoso_sensordata"} records.\n");
        }

        private static void DeleteMultipleRecords(IOrganizationService service, string deviceId, List<Guid> recordIds)
        {
            Console.WriteLine($"Preparing {recordIds.Count} records to delete..\n");
            EntityReferenceCollection entityReferences = new();

            // Populate the list with the number of records to test.
            for (int i = 0; i < recordIds.Count; i++)
            {
                EntityReference entityReference = new EntityReference("contoso_sensordata")
                {
                    KeyAttributes =
                    {
                        { "partitionid", deviceId },
                        { "contoso_sensordataid", recordIds[i] }
                    }
                };

                entityReferences.Add(entityReference);
            }

            int skipRecordCount = 0;

            // Executing DeleteMultiple in batches.
            List<EntityReference> referencesToProcess;
            while ((referencesToProcess = entityReferences.Skip(skipRecordCount).Take(Settings.BatchSize).ToList()) != null
                && referencesToProcess.Count > 0)
            {
                DeleteMultiple(service, referencesToProcess);
                skipRecordCount += referencesToProcess.Count;
                referencesToProcess.Clear();
            }
        }

        private static void DeleteMultiple(IOrganizationService service, List<EntityReference> entityReferences)
        {
            Console.WriteLine($"Deleting {entityReferences.Count} {"contoso_sensordata"} records.. \n");

            OrganizationRequest deleteMultipleRequest = new()
            {
                RequestName = "DeleteMultiple",
                ["Targets"] = new EntityReferenceCollection(entityReferences)
            };
            service.Execute(deleteMultipleRequest);

            Console.WriteLine($"Deleted {entityReferences.Count} {"contoso_sensordata"} records.\n");
        }

        /// <summary>
        /// This class is used to populate the string attribute with json format (contoso_energyconsumption)
        /// </summary>
        private class EnergyConsumption
        {
            [JsonProperty("power")]
            public int Power { get; set; }

            [JsonProperty("powerUnit")]
            public string PowerUnit { get; set; }

            [JsonProperty("voltage")]
            public int Voltage { get; set; }
            
            [JsonProperty("voltageUnit")]
            public string VoltageUnit { get; set; }
        }
    }
}