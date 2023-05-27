

using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using System.Text;

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
            if (path == null) path = "appsettings.json";

            // Load the app's configuration settings from the JSON file.
            Configuration = new ConfigurationBuilder()
                .AddJsonFile(path, optional: false, reloadOnChange: true)
                .Build();
        }

        static void Main()
        {
            Program app = new();

            // Create a Dataverse service client using the default connection string.
            ServiceClient serviceClient =
                new(app.Configuration.GetConnectionString("default"));

            try
            {
                // Utility.CreateSensorDataEntity(serviceClient);

                #region No partitionid
                Console.WriteLine("Examples without partitionid\n");

                // Elastic table records work like other records
                // when no strategy is used for setting partitionids.

                string sessionToken;
                Guid sensordataId;

                CreateRequest createRequest = new()
                {
                    Target = new Entity("contoso_sensordata")
                    {
                        Attributes =
                        {
                            { "contoso_deviceid", "Device-ABC-1234"},
                            { "contoso_sensortype", "Humidity" },
                            { "contoso_value", 10 },
                            { "contoso_timestamp", DateTime.UtcNow},
                            { "ttlinseconds", 86400  }  // 86400  seconds in a day
                        }
                    }
                };

                var createResponse = (CreateResponse)serviceClient.Execute(createRequest);
                sensordataId = createResponse.id;
                Console.WriteLine($"Created record with id: {sensordataId}");
                // Capture session token on write operation
                sessionToken = createResponse.Results["x-ms-session-token"].ToString();

                // Update the record

                // Json Data to store in contoso_energyconsumption
                EnergyConsumption energyConsumption = new()
                {
                    Voltage = 3,
                    VoltageUnit = "Volts",
                    Power = 4,
                    PowerUnit = "Watts"
                };

                UpdateRequest updateRequest = new()
                {
                    Target = new Entity("contoso_sensordata", sensordataId)
                    {
                        Attributes =
                        {
                            { "contoso_value", 20 },
                            { "contoso_energyconsumption", JsonConvert.SerializeObject(energyConsumption) }
                        }
                    }
                };

                var updateResponse = (UpdateResponse)serviceClient.Execute(updateRequest);
                Console.WriteLine($"Updated the record with id: {sensordataId}");
                // Capture session token on write operation
                sessionToken = updateResponse.Results["x-ms-session-token"].ToString();

                //Upsert the record
                UpsertRequest upsertRequest = new()
                {
                    Target = new Entity("contoso_sensordata", sensordataId)
                    {
                        Attributes =
                         {
                            // IMPORTANT: Upsert must include all writable record data
                            // Data for an existing record will be overwritten
                            { "contoso_deviceid", "Device-ABC-1234"},
                            { "contoso_sensortype", "Humidity" },
                            { "contoso_value", 30 },
                            { "contoso_timestamp", DateTime.UtcNow},
                            { "ttlinseconds", 86400  },  // 86400  seconds in a day
                            { "contoso_energyconsumption", JsonConvert.SerializeObject(energyConsumption) }
                         }
                    }
                };
                var upsertResponse = (UpsertResponse)serviceClient.Execute(upsertRequest);
                Console.WriteLine($"Upserted the record with id: {sensordataId}");
                Console.WriteLine($"Record created with upsert?:{upsertResponse.RecordCreated}");

                // Retrieve the record
                RetrieveRequest retrieveRequest = new()
                {
                    ColumnSet = new ColumnSet(
                         "contoso_deviceid",
                         "contoso_sensortype",
                         "contoso_value",
                         "contoso_timestamp",
                         "ttlinseconds",
                         "contoso_energyconsumption"),
                    Target = new EntityReference("contoso_sensordata", sensordataId),
                    ["SessionToken"] = sessionToken //Set session token on read operation
                };

                var retrieveResponse = (RetrieveResponse)serviceClient.Execute(retrieveRequest);
                Entity retrievedRecord = retrieveResponse.Entity;
                // Write out the properties retrieved
                Console.WriteLine($"Retrieved the record with id: {sensordataId}");
                Console.WriteLine($"\tcontoso_sensordataid: {retrievedRecord["contoso_sensordataid"]}");
                Console.WriteLine($"\tcontoso_deviceid: {retrievedRecord.GetAttributeValue<string>("contoso_deviceid")}");
                Console.WriteLine($"\tcontoso_sensortype: {retrievedRecord.GetAttributeValue<string>("contoso_sensortype")}");
                Console.WriteLine($"\tcontoso_value: {retrievedRecord.GetAttributeValue<int>("contoso_value")}");
                Console.WriteLine($"\tcontoso_timestamp: {retrievedRecord.GetAttributeValue<DateTime>("contoso_timestamp")}");
                Console.WriteLine($"\tttlinseconds: {retrievedRecord.GetAttributeValue<int>("ttlinseconds")}");
                Console.WriteLine($"\tcontoso_energyconsumption {retrievedRecord.GetAttributeValue<string>("contoso_energyconsumption")}");


                // Delete sensordata record
                DeleteRequest deleteRequest = new()
                {
                    Target = new EntityReference("contoso_sensordata", sensordataId)
                };
                serviceClient.Execute(deleteRequest);
                Console.WriteLine($"Deleted the record with id: {sensordataId}");

                #endregion No partitionid

                #region With partitionid
                Console.WriteLine("Examples with partitionid\n");

                // When a partitionid is used the partitionid is required
                // To uniquely identify each record.

                // using deviceId as the partitionid for these samples.
                string deviceId = "Device-ABC-1234";
                Guid record1Id; //The first record created
                Guid record2Id; //The second record created

                Console.WriteLine("Creating record...");
                record1Id = CreateRecord(serviceClient, deviceId, ref sessionToken);
                Console.WriteLine("\tRecord created.\n");

                Console.WriteLine("Updating record...");
                UpdateRecord(serviceClient, record1Id, deviceId, ref sessionToken);
                Console.WriteLine("\tRecord updated.\n");

                Console.WriteLine("Updating record with alternate key...");
                UpdateRecordWithAlternateKey(serviceClient, record1Id, deviceId, ref sessionToken);
                Console.WriteLine("\tRecord updated with alternate key.\n");

                Console.WriteLine("Retrieving record...");
                Entity record = RetrieveRecord(serviceClient, record1Id, deviceId, sessionToken);
                Console.WriteLine($"\tRecord retrieved.");
                Console.WriteLine($"\tcontoso_value: {record.GetAttributeValue<int>("contoso_value")}\n");

                Console.WriteLine($"Retrieving record with alternate key...");
                record = RetrieveRecordWithAlternateKey(serviceClient, record1Id, deviceId, sessionToken);
                Console.WriteLine($"\tRetrieved record with alternate key.");
                Console.WriteLine($"\tcontoso_value: {record.GetAttributeValue<int>("contoso_value")}\n");

                Console.WriteLine($"Upserting  record...");
                bool created = UpsertRecord(serviceClient, record1Id, deviceId, sessionToken);
                Console.WriteLine($"\tUpserted record.\n");

                Console.WriteLine($"Deleting record...");
                DeleteRecord(serviceClient, record1Id, deviceId, sessionToken);
                Console.WriteLine($"\tRecord deleted.\n");

                // Create another record to demonstrate delete with alternate key
                record2Id = CreateRecord(serviceClient, deviceId, ref sessionToken);

                Console.WriteLine($"Deleting record with alternate key...");
                DeleteRecordWithAlternateKey(serviceClient, record2Id, deviceId, sessionToken);
                Console.WriteLine($"\tRecord deleted with alternate key.\n");

                #endregion With partitionid

                #region CreateMultiple and UpdateMultiple


                Console.WriteLine($"Creating {Settings.NumberOfRecords} records using CreateMultiple");
                Console.WriteLine($"In batches of {Settings.BatchSize}");
                List<Guid> createdRecordIds = CreateMultipleRecords(
                    serviceClient, deviceId,
                    Settings.NumberOfRecords,
                    Settings.BatchSize);
                Console.WriteLine($"\tCreated {createdRecordIds.Count} records.\n");

                Console.WriteLine($"Updating {Settings.NumberOfRecords} records using UpdateMultiple");
                Console.WriteLine($"In batches of {Settings.BatchSize}");
                UpdateMultipleRecords(serviceClient, deviceId, createdRecordIds, Settings.BatchSize);
                Console.WriteLine($"Updated {Settings.NumberOfRecords} records.\n");

                #endregion CreateMultiple and UpdateMultiple

                #region ExecuteCosmosSqlQuery

                Console.WriteLine($"Requesting ExecuteCosmosSqlQuery.. \n");
                ExecuteCosmosSqlQueryResponse response = ExecuteCosmosSqlQuery(serviceClient, deviceId);
                Console.WriteLine($"PagingCookie: {response.PagingCookie}");
                Console.WriteLine($"HasMore: {response.HasMore}");

                List<EnergyConsumption> energyConsumptions = JsonConvert.DeserializeObject<List<EnergyConsumption>>(response.Result.ToString());
                Console.WriteLine($"Returned {energyConsumptions.Count} records from ExecuteCosmosSqlQuery.\n");

                if (response.HasMore)
                {
                    response = ExecuteCosmosSqlQuery(serviceClient, deviceId,50,response.PagingCookie);
                    energyConsumptions = JsonConvert.DeserializeObject<List<EnergyConsumption>>(response.Result.ToString());
                    Console.WriteLine($"Returned {energyConsumptions.Count} more records from ExecuteCosmosSqlQuery.\n");
                }

                #endregion ExecuteCosmosSqlQuery

                #region DeleteMultiple

                Console.WriteLine($"Deleteing {Settings.NumberOfRecords} records using DeleteMultiple");
                DeleteMultipleRecords(serviceClient, deviceId, createdRecordIds, Settings.BatchSize);
                Console.WriteLine($"Deleted {Settings.NumberOfRecords} records.\n");

                #endregion DeleteMultiple

            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Timestamp: {0}", ex.Detail.Timestamp);
                Console.WriteLine("Code: {0}", ex.Detail.ErrorCode);
                Console.WriteLine("Message: {0}", ex.Detail.Message);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
                if (ex.Detail.InnerFault != null)
                {
                    Console.WriteLine($"{ex.Detail.InnerFault.Message}");
                }
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Message: {0}", ex.Message);
                Console.WriteLine("Stack Trace: {0}", ex.StackTrace);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.InnerException.Message ? "No Inner Fault" : ex.InnerException.Message);
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

                // Utility.DeleteSensorDataEntity(serviceClient);
            }

            
        }

        private static Guid CreateRecord(
            IOrganizationService service,
            string deviceId,
            ref string sessionToken)
        {
            Entity entity = new("contoso_sensordata")
            {
                Attributes =
                {
                    { "contoso_deviceid", deviceId },
                    { "contoso_sensortype", "Humidity" },
                    { "contoso_value", 40 },
                    { "contoso_timestamp", DateTime.UtcNow},
                    { "partitionid", deviceId }, // deviceId is the chosen partitionid value
                    { "ttlinseconds", 86400  }  // 86400  seconds in a day
                }
            };

            CreateRequest request = new()
            {
                Target = entity
            };

            CreateResponse response = (CreateResponse)service.Execute(request);

            // Capture the session token so that retrieve operations will have strong consistency
            sessionToken = response.Results["x-ms-session-token"].ToString();

            return response.id;
        }

        private static void UpdateRecord(
            IOrganizationService service,
            Guid contosoSensorDataId,
            string deviceId,
            ref string sessionToken)
        {


            Entity entity = new("contoso_sensordata", contosoSensorDataId)
            {
                Attributes =
                {
                    { "partitionid", deviceId },
                    { "contoso_value", 60 },
                    { "contoso_timestamp", DateTime.UtcNow },
                }
            };

            UpdateRequest request = new()
            {
                Target = entity,
                ["SessionToken"] = sessionToken
            };

            UpdateResponse response = (UpdateResponse)service.Execute(request);


            /// Capture the session token so that retrieve operations will have strong consistency
            sessionToken = response.Results["x-ms-session-token"].ToString();
        }

        private static void UpdateRecordWithAlternateKey(
            IOrganizationService service,
            Guid contosoSensorDataId,
            string deviceId,
            ref string sessionToken)
        {
            KeyAttributeCollection keys = new()
            {
                { "contoso_sensordataid", contosoSensorDataId },
                { "partitionid", deviceId }
            };

            Entity entity = new("contoso_sensordata", keys)
            {
                Attributes =
                {
                    { "contoso_value", 60 },
                    { "contoso_timestamp", DateTime.UtcNow }
                }
            };

            UpdateRequest request = new()
            {
                Target = entity,
                ["SessionToken"] = sessionToken
            };

            var response = (UpdateResponse)service.Execute(request);


            // Capture the session token so that retrieve operations will have strong consistency
            sessionToken = response.Results["x-ms-session-token"].ToString();
        }

        private static Entity RetrieveRecord(
            IOrganizationService service,
            Guid contosoSensorDataId,
            string deviceId,
            string sessionToken)
        {
            EntityReference entityReference = new("contoso_sensordata", contosoSensorDataId);

            RetrieveRequest request = new()
            {
                ColumnSet = new ColumnSet("contoso_value"),
                Target = entityReference,
                ["partitionId"] = deviceId,
                ["SessionToken"] = sessionToken //Pass the session token for strong consistency
            };
            var response = (RetrieveResponse)service.Execute(request);
            return response.Entity;

        }

        private static Entity RetrieveRecordWithAlternateKey(
            IOrganizationService service,
            Guid contosoSensorDataId,
            string deviceId,
            string sessionToken)
        {

            KeyAttributeCollection keys = new()
            {
                { "contoso_sensordataid", contosoSensorDataId },
                { "partitionid", deviceId }
            };

            EntityReference entityReference = new("contoso_sensordata", keys);

            RetrieveRequest request = new()
            {
                ColumnSet = new ColumnSet("contoso_value"),
                Target = entityReference,
                ["SessionToken"] = sessionToken
            };
            var response = (RetrieveResponse)service.Execute(request);
            return response.Entity;

        }

        private static bool UpsertRecord(
            IOrganizationService service,
            Guid contosoSensorDataId,
            string deviceId,
            string sessionToken)
        {

            Entity entity = new("contoso_sensordata", contosoSensorDataId)
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

            UpsertRequest request = new()
            {
                Target = entity,
                ["SessionToken"] = sessionToken
            };

            var response = (UpsertResponse)service.Execute(request);

            // Capture the session token so that retrieve operations will have strong consistency
            sessionToken = response.Results["x-ms-session-token"].ToString();

            return response.RecordCreated;
        }

        private static void DeleteRecord(
            IOrganizationService service,
            Guid contosoSensorDataId,
            string deviceId,
            string sessionToken)
        {
            DeleteRequest request = new()
            {
                Target = new EntityReference("contoso_sensordata", contosoSensorDataId),
                ["partitionId"] = deviceId,
                ["SessionToken"] = sessionToken
            };

            service.Execute(request);
        }

        private static void DeleteRecordWithAlternateKey(
            IOrganizationService service,
            Guid contosoSensorDataId,
            string deviceId,
            string sessionToken)
        {
            KeyAttributeCollection keyAttributeCollection = new()
            {
                ["contoso_sensordataid"] = contosoSensorDataId,
                ["partitionid"] = deviceId
            };

            DeleteRequest request = new()
            {
                Target = new EntityReference("contoso_sensordata", keyAttributeCollection),
                ["SessionToken"] = sessionToken
            };

            service.Execute(request);
        }

        private static List<Guid> CreateMultipleRecords(
            IOrganizationService service,
            string deviceId,
            int numberOfRecords,
            int batchSize)
        {
            List<Guid> createdRecordIds = new();

            List<Entity> entityList = new();

            // Populate the list with the number of records to test.
            for (int i = 0; i < numberOfRecords; i++)
            {
                Entity entity = new("contoso_sensordata")
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
            while ((entitiesToProcess = entityList
                .Skip(skipRecordCount)
                .Take(batchSize)
                .ToList()) != null
                && entitiesToProcess.Count > 0)
            {
                createdRecordIds.AddRange(CreateMultiple(service, entitiesToProcess));
                skipRecordCount += entitiesToProcess.Count;
                entitiesToProcess.Clear();
            }

            return createdRecordIds;
        }

        private static Guid[] CreateMultiple(
            IOrganizationService service,
            List<Entity> entities)
        {
            EntityCollection entityCollection = new(entities)
            {
                EntityName = "contoso_sensordata"
            };

            CreateMultipleRequest request = new()
            {
                Targets = entityCollection,
            };
            var response = (CreateMultipleResponse)service.Execute(request);


            return response.Ids;
        }

        private static ExecuteCosmosSqlQueryResponse ExecuteCosmosSqlQuery(
            IOrganizationService service,
            string deviceId,
            long pageSize = 50,
            string? pagingCookie = null)
        {
            StringBuilder query = new();
            query.Append("select c.props.contoso_deviceid as deviceId, ");
            query.Append("c.props.contoso_timestamp as timestamp, ");
            query.Append("c.props.contoso_energyconsumption.power as power ");
            query.Append("from c where c.props.contoso_sensortype='Humidity' ");
            query.Append("and c.props.contoso_energyconsumption.power > 50");

            // Using OrganizationRequest because SDK doesn't yet have ExecuteCosmosSqlQueryRequest class
            OrganizationRequest request = new("ExecuteCosmosSqlQuery");

            request.Parameters["QueryText"] = query.ToString();
            request.Parameters["EntityLogicalName"] = "contoso_sensordata";                        
            request.Parameters["PageSize"] = pageSize;
            request.Parameters["PagingCookie"] = pagingCookie;
            request.Parameters["PartitionId"] = deviceId;

            OrganizationResponse response = service.Execute(request);
         
            Entity resultEntity = (Entity)response.Results["Result"];

            resultEntity.TryGetAttributeValue("PagingCookie", out pagingCookie);
            resultEntity.TryGetAttributeValue("HasMore", out bool hasMore);
            resultEntity.TryGetAttributeValue("Result", out string result);

            return new ExecuteCosmosSqlQueryResponse
            {
                PagingCookie = pagingCookie,
                HasMore = hasMore,
                Result = result
            };

        }

        private static void UpdateMultipleRecords(
            IOrganizationService service,
            string deviceId,
            List<Guid> createdRecordIds,
            int batchSize)
        {
            List<Entity> entityList = new();

            // Populate the list with the number of records to test.
            for (int i = 0; i < createdRecordIds.Count; i++)
            {
                var keys = new KeyAttributeCollection() {
                    { "contoso_sensordataid", createdRecordIds[i] },
                    { "partitionid", deviceId }
                };

                // Updating this JSON column value
                EnergyConsumption energyConsumption = new()
                {
                    Voltage = i + 1,
                    VoltageUnit = "Volts",
                    Power = i + 2,
                    PowerUnit = "Watts"
                };

                Entity entity = new("contoso_sensordata", keys)
                {
                    Attributes =
                    {
                        { "contoso_energyconsumption", JsonConvert.SerializeObject(energyConsumption) }
                    }
                };

                entityList.Add(entity);
            }

            int skipRecordCount = 0;

            // Executing UpdateMultiple in batches.
            List<Entity> entitiesToProcess;
            while ((entitiesToProcess = entityList
                .Skip(skipRecordCount)
                .Take(batchSize)
                .ToList()) != null
                && entitiesToProcess.Count > 0)
            {
                UpdateMultiple(service, entitiesToProcess);
                skipRecordCount += entitiesToProcess.Count;
                entitiesToProcess.Clear();
            }
        }

        private static void UpdateMultiple(
            IOrganizationService service,
            List<Entity> entities)
        {
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
        }

        private static void DeleteMultipleRecords(
            IOrganizationService service,
            string deviceId,
            List<Guid> recordIds,
            int batchSize)
        {
            EntityReferenceCollection entityReferences = new();

            // Populate the list with the number of records to test.
            for (int i = 0; i < recordIds.Count; i++)
            {
                EntityReference entityReference = new("contoso_sensordata")
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
            while ((referencesToProcess = entityReferences
                .Skip(skipRecordCount)
                .Take(batchSize)
                .ToList()) != null
                && referencesToProcess.Count > 0)
            {
                DeleteMultiple(service, referencesToProcess);
                skipRecordCount += referencesToProcess.Count;
                referencesToProcess.Clear();
            }
        }

        private static void DeleteMultiple(
            IOrganizationService service,
            List<EntityReference> entityReferences)
        {
            Console.WriteLine($"Deleting {entityReferences.Count} records...");

            OrganizationRequest deleteMultipleRequest = new()
            {
                RequestName = "DeleteMultiple",
                ["Targets"] = new EntityReferenceCollection(entityReferences)
            };
            service.Execute(deleteMultipleRequest);

            Console.WriteLine($"Deleted {entityReferences.Count} records.\n");
        }

        /// <summary>
        /// This class is used to populate the string attribute with json format (contoso_energyconsumption)
        /// </summary>
        public class EnergyConsumption
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

        /// <summary>
        /// Contains the results from a ExecuteCosmosSqlQueryRequest
        /// </summary>
        public class ExecuteCosmosSqlQueryResponse : Entity
        {
            [AttributeLogicalName("PagingCookie")]
            public string? PagingCookie
            {
                get => GetAttributeValue<string>("PagingCookie");
                set => SetAttributeValue("PagingCookie", value);
            }

            [AttributeLogicalName("HasMore")]
            public bool HasMore
            {
                get => GetAttributeValue<bool>("HasMore");
                set => SetAttributeValue("HasMore", value);
            }

            [AttributeLogicalName("Result")]
            public string? Result
            {
                get => GetAttributeValue<string>("Result");
                set => SetAttributeValue("Result", value);
            }
        }
    }
}