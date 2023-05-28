using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
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

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Create a Dataverse service client using the default connection string.
            ServiceClient serviceClient =
                new(app.Configuration.GetConnectionString("default"));

            Console.WriteLine($"Starting Elastic table operations sample.\n");

            try
            {
                Console.WriteLine("=== Start Region 0: Creating contoso_SensorData table === \n");
                Utility.CreateSensorDataEntity(serviceClient);

                #region No partitionid
                Console.WriteLine("=== Start Region 1: Examples without partitionid === \n");

                // Elastic table records work like other records
                // when no strategy is used for setting partitionids
                // But you must still manage the sessionToken

                string sessionToken;  // Place to keep the current session token

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
                Console.WriteLine("Data:");
                Console.WriteLine($"\tcontoso_sensordataid: {retrievedRecord["contoso_sensordataid"]}");
                Console.WriteLine($"\tcontoso_deviceid: {retrievedRecord.GetAttributeValue<string>("contoso_deviceid")}");
                Console.WriteLine($"\tcontoso_sensortype: {retrievedRecord.GetAttributeValue<string>("contoso_sensortype")}");
                Console.WriteLine($"\tcontoso_value: {retrievedRecord.GetAttributeValue<int>("contoso_value")}");
                Console.WriteLine($"\tcontoso_timestamp: {retrievedRecord.GetAttributeValue<DateTime>("contoso_timestamp")}");
                Console.WriteLine($"\tttlinseconds: {retrievedRecord.GetAttributeValue<int>("ttlinseconds")}");
                Console.WriteLine($"\tcontoso_energyconsumption {retrievedRecord.GetAttributeValue<string>("contoso_energyconsumption")}\n");


                // Delete sensordata record
                DeleteRequest deleteRequest = new()
                {
                    Target = new EntityReference("contoso_sensordata", sensordataId)
                };
                serviceClient.Execute(deleteRequest);
                Console.WriteLine($"Deleted the record with id: {sensordataId}\n");

                #endregion No partitionid

                #region With partitionid
                Console.WriteLine("=== Start Region 2: Examples with partitionid === \n");

                // When a partitionid is used the partitionid is required
                // To uniquely identify each record.

                // using deviceId as the partitionid for these samples.
                string deviceId = "Device-ABC-1234";
                Guid record1Id; //The first record created
                Guid record2Id; //The second record created

                Console.WriteLine("Creating record...");
                record1Id = CreateRecord(serviceClient, deviceId, ref sessionToken);
                Console.WriteLine("\tRecord created.");

                Console.WriteLine("Updating record...");
                UpdateRecord(serviceClient, record1Id, deviceId, ref sessionToken);
                Console.WriteLine("\tRecord updated.");

                Console.WriteLine("Updating record with alternate key...");
                UpdateRecordWithAlternateKey(serviceClient, record1Id, deviceId, ref sessionToken);
                Console.WriteLine("\tRecord updated with alternate key.");

                Console.WriteLine("Retrieving record...");
                Entity record = RetrieveRecord(serviceClient, record1Id, deviceId, sessionToken);
                Console.WriteLine($"\tRecord retrieved.");
                Console.WriteLine($"\tcontoso_value: {record.GetAttributeValue<int>("contoso_value")}");

                Console.WriteLine($"Retrieving record with alternate key...");
                record = RetrieveRecordWithAlternateKey(serviceClient, record1Id, deviceId, sessionToken);
                Console.WriteLine($"\tRetrieved record with alternate key.");
                Console.WriteLine($"\tcontoso_value: {record.GetAttributeValue<int>("contoso_value")}");

                Console.WriteLine($"Upserting  record...");
                bool created = UpsertRecord(serviceClient, record1Id, deviceId, ref sessionToken);
                Console.WriteLine($"\tUpserted record.");

                Console.WriteLine($"Deleting record...");
                DeleteRecord(serviceClient, record1Id, deviceId, ref sessionToken);
                Console.WriteLine($"\tRecord deleted.");

                // Create another record to demonstrate delete with alternate key
                record2Id = CreateRecord(serviceClient, deviceId, ref sessionToken);

                Console.WriteLine($"Deleting record with alternate key...");
                DeleteRecordWithAlternateKey(serviceClient, record2Id, deviceId, ref sessionToken);
                Console.WriteLine($"\tRecord deleted with alternate key.\n");

                #endregion With partitionid

                #region CreateMultiple and UpdateMultiple
                Console.WriteLine("=== Start Region 3: CreateMultiple and UpdateMultiple Examples === \n");

                Console.WriteLine($"Creating {Settings.NumberOfRecords} records using CreateMultiple");
                Console.WriteLine($"In batches of {Settings.BatchSize}");

                List<Guid> createdRecordIds = CreateMultipleRecords(
                    service: serviceClient, 
                    deviceId: deviceId,
                    numberOfRecords: Settings.NumberOfRecords,
                    batchSize: Settings.BatchSize);

                Console.WriteLine($"\tCreated {createdRecordIds.Count} records.\n");

                Console.WriteLine($"Updating {Settings.NumberOfRecords} records using UpdateMultiple");
                Console.WriteLine($"In batches of {Settings.BatchSize}");

                UpdateMultipleRecords(
                    service: serviceClient, 
                    deviceId: deviceId,
                    recordIds: createdRecordIds, 
                    batchSize: Settings.BatchSize);

                Console.WriteLine($"\tUpdated {Settings.NumberOfRecords} records.\n");

                #endregion CreateMultiple and UpdateMultiple

                #region ExecuteCosmosSqlQuery
                Console.WriteLine("=== Start Region 4: ExecuteCosmosSqlQuery Examples === \n");

                StringBuilder query = new();
                query.Append("select c.props.contoso_deviceid as deviceId, ");
                query.Append("c.props.contoso_timestamp as timestamp, ");
                query.Append("c.props.contoso_energyconsumption.power as power ");
                query.Append("from c where c.props.contoso_sensortype='Humidity' ");
                query.Append("and c.props.contoso_energyconsumption.power > 50");

                int pageSize = 100; //The number of results to return per page

                Console.WriteLine($"Requesting ExecuteCosmosSqlQuery.. \n");

                ExecuteCosmosSqlQueryResponse response = ExecuteCosmosSqlQuery(
                    service:serviceClient,
                    query: query.ToString(), 
                    deviceId:deviceId, 
                    pageSize: pageSize);

                Console.WriteLine($"ExecuteCosmosSqlQueryResponse.PagingCookie: {response.PagingCookie}\n");
                Console.WriteLine($"ExecuteCosmosSqlQueryResponse.HasMore: {response.HasMore}\n");

                Console.WriteLine($"Output first page of {pageSize} results:\n");

                // All the results will be added to this
                JArray results = JArray.Parse(json: response.Result);

                Console.WriteLine($"Returned initial {results.Count} results from ExecuteCosmosSqlQuery:");

                results.ToList().ForEach(result =>
                {
                    Console.WriteLine($"\t{result["deviceId"]} {result["power"]}");
                });

                while (response.HasMore)
                {
                    response = response = ExecuteCosmosSqlQuery(
                    service: serviceClient,
                    query: query.ToString(), //Same query each time
                    deviceId: deviceId,
                    pageSize: pageSize, 
                    pagingCookie: response.PagingCookie); //Include paging cookie

                    JArray moreResults = JArray.Parse(json: response.Result);

                    Console.WriteLine($"\nReturned {moreResults.Count} more results from ExecuteCosmosSqlQuery.");

                    moreResults.ToList().ForEach(result =>
                    {
                        Console.WriteLine($"\t{result["deviceId"]} {result["power"]}");
                    });

                    results.Merge(moreResults);                    
                }

                Console.WriteLine($"\nReturned total of {results.Count} results using ExecuteCosmosSqlQuery\n");


                #endregion ExecuteCosmosSqlQuery

                #region DeleteMultiple
                Console.WriteLine("=== Start Region 5: DeleteMultiple Example === \n");

                Console.WriteLine($"Deleteing {Settings.NumberOfRecords} records using DeleteMultiple");
                Console.WriteLine($"In batches of {Settings.BatchSize}");
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
                Console.WriteLine("=== Start Region 6: Delete contoso_SensorData table === \n");
                Utility.DeleteSensorDataEntity(serviceClient);

                Console.WriteLine("\n=== SDK ElasticTableOperations Sample Completed === \n");
            }


        }

        /// <summary>
        /// Creates a contoso_SensorData record setting the partitionid.
        /// </summary>
        /// <param name="service">An authenticated client that implemnents the IOrganizationService interface.</param>
        /// <param name="deviceId">The value used for the partitionid.</param>
        /// <param name="sessionToken">The current session token.</param>
        /// <returns>The ID of the record created.</returns>
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

        /// <summary>
        /// Updates a record without using alternate key
        /// </summary>
        /// <param name="service">An authenticated client that implemnents the IOrganizationService interface.</param>
        /// <param name="contosoSensorDataId">The unique identifier of the record to update.</param>
        /// <param name="deviceId">The partitionid value</param>
        /// <param name="sessionToken">The current session token</param>
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
                    // Setting partitionid attribute
                    { "partitionid", deviceId }, // This cannot change the value
                    { "contoso_value", 60 },
                    { "contoso_timestamp", DateTime.UtcNow },
                }
            };

            UpdateRequest request = new()
            {
                Target = entity
                //,["partitionId"] = deviceId, //To identify the record
                // Currently not possible to set partitionId on Update or Upsert requests
                // https://learn.microsoft.com/power-apps/developer/data-platform/elastic-tables#partitionid-optional-parameter-not-available-for-all-messages
            };

            UpdateResponse response = (UpdateResponse)service.Execute(request);


            /// Capture the session token so that retrieve operations will have strong consistency
            sessionToken = response.Results["x-ms-session-token"].ToString();
        }

        /// <summary>
        /// Updates record using alternate key
        /// </summary>
        /// <param name="service">An authenticated client that implemnents the IOrganizationService interface.</param>
        /// <param name="contosoSensorDataId">The unique identifier of the record to update.</param>
        /// <param name="deviceId">The partitionid value</param>
        /// <param name="sessionToken">The current session token</param>
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
                    { "contoso_value", 80 },
                    { "contoso_timestamp", DateTime.UtcNow }
                }
            };

            UpdateRequest request = new()
            {
                Target = entity
            };

            var response = (UpdateResponse)service.Execute(request);


            // Capture the session token so that retrieve operations will have strong consistency
            sessionToken = response.Results["x-ms-session-token"].ToString();
        }

        /// <summary>
        /// Retrieves a record without using alternate key
        /// </summary>
        /// <param name="service">An authenticated client that implemnents the IOrganizationService interface.</param>
        /// <param name="contosoSensorDataId">The unique identifier of the record to update.</param>
        /// <param name="deviceId">The partitionid value</param>
        /// <param name="sessionToken">The current session token</param>
        /// <returns>The specified contoso_SensorData record</returns>
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
                ["partitionId"] = deviceId, //To identify the record
                ["SessionToken"] = sessionToken //Pass the session token for strong consistency
            };
            var response = (RetrieveResponse)service.Execute(request);
            return response.Entity;

        }

        /// <summary>
        /// Retrieves a record using alternate key
        /// </summary>
        /// <param name="service">An authenticated client that implemnents the IOrganizationService interface.</param>
        /// <param name="contosoSensorDataId">The unique identifier of the record to update.</param>
        /// <param name="deviceId">The partitionid value</param>
        /// <param name="sessionToken">The current session token</param>
        /// <returns>The specified contoso_SensorData record</returns>
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
                ["SessionToken"] = sessionToken //Pass the session token for strong consistency
            };
            var response = (RetrieveResponse)service.Execute(request);
            return response.Entity;

        }

        /// <summary>
        /// Upserts a record without alternate key
        /// </summary>
        /// <param name="service">An authenticated client that implemnents the IOrganizationService interface.</param>
        /// <param name="contosoSensorDataId">The unique identifier of the record to update.</param>
        /// <param name="deviceId">The partitionid value</param>
        /// <param name="sessionToken">The current session token</param>
        /// <returns>Whether a new record was created or not.</returns>
        private static bool UpsertRecord(
            IOrganizationService service,
            Guid contosoSensorDataId,
            string deviceId,
            ref string sessionToken)
        {
            Entity entity = new("contoso_sensordata", contosoSensorDataId)
            {
                // For Upsert it is required to set all the attribute values
                // If matching record is found, all data is replaced.
                Attributes =
                {
                    { "contoso_deviceid", deviceId },
                    { "contoso_sensortype".ToLower(), "Humidity" },
                    { "contoso_value", 60 },
                    { "contoso_timestamp", DateTime.UtcNow },
                    { "partitionid", deviceId }, // This cannot change the value
                    { "ttlinseconds", 86400 }
                }
            };

            UpsertRequest request = new()
            {
                Target = entity
                //,["partitionId"] = deviceId, //To identify the record
                // Currently not possible to set partitionId on Update or Upsert requests
                // https://learn.microsoft.com/power-apps/developer/data-platform/elastic-tables#partitionid-optional-parameter-not-available-for-all-messages
            };

            var response = (UpsertResponse)service.Execute(request);

            // Capture the session token so that retrieve operations will have strong consistency
            sessionToken = response.Results["x-ms-session-token"].ToString();

            return response.RecordCreated;
        }

        /// <summary>
        /// Deletes a record using partitionId
        /// </summary>
        /// <param name="service">An authenticated client that implemnents the IOrganizationService interface.</param>
        /// <param name="contosoSensorDataId">The unique identifier of the record to update.</param>
        /// <param name="deviceId">The partitionid value</param>
        /// <param name="sessionToken">The current session token</param>
        private static void DeleteRecord(
            IOrganizationService service,
            Guid contosoSensorDataId,
            string deviceId,
            ref string sessionToken)
        {
            DeleteRequest request = new()
            {
                Target = new EntityReference("contoso_sensordata", contosoSensorDataId),
                ["partitionId"] = deviceId
            };

            service.Execute(request);

            // Capture the session token so that retrieve operations will have strong consistency
            // Known issue: x-ms-session-token not available in Delete
            //sessionToken = response.Results["x-ms-session-token"].ToString();
        }

        /// <summary>
        /// Deletes a record using alternate key.
        /// </summary>
        /// <param name="service">An authenticated client that implemnents the IOrganizationService interface.</param>
        /// <param name="contosoSensorDataId">The unique identifier of the record to update.</param>
        /// <param name="deviceId">The partitionid value</param>
        /// <param name="sessionToken">The current session token</param>
        private static void DeleteRecordWithAlternateKey(
            IOrganizationService service,
            Guid contosoSensorDataId,
            string deviceId,
            ref string sessionToken)
        {
            KeyAttributeCollection keyAttributeCollection = new()
            {
                ["contoso_sensordataid"] = contosoSensorDataId,
                ["partitionid"] = deviceId
            };

            DeleteRequest request = new()
            {
                Target = new EntityReference("contoso_sensordata", keyAttributeCollection)
            };

            service.Execute(request);


            // Capture the session token so that retrieve operations will have strong consistency
            // Known issue: x-ms-session-token not available in Delete
            //sessionToken = response.Results["x-ms-session-token"].ToString();
        }


        /// <summary>
        /// Creates multiple contoso_SensorData records in batches.
        /// </summary>
        /// <param name="service">An authenticated client that implemnents the IOrganizationService interface.</param>
        /// <param name="deviceId">The partitionid value</param>
        /// <param name="numberOfRecords">The total number of records to create.</param>
        /// <param name="batchSize">The number of records to create per request.</param>
        /// <returns></returns>
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
                // contoso_energyconsumption stores this JSON data
                EnergyConsumption energyConsumption = new()
                {
                    Voltage = i,
                    VoltageUnit = "Volts",
                    Power = i + 1,
                    PowerUnit = "Watts"
                };

                Entity entity = new("contoso_sensordata")
                {
                    Attributes =
                    {
                        {"contoso_deviceid", deviceId },
                        {"contoso_sensortype", "Humidity" },
                        {"partitionid", deviceId },
                        {"contoso_energyconsumption",JsonConvert.SerializeObject(energyConsumption) },
                        {"ttlinseconds", 86400 } // 86400  seconds in a day
                    }
                };

                entityList.Add(entity);
            }

            int skipRecordCount = 0;

            // Executing CreateMultiple in batches.
            List<Entity> entitiesToProcess;
            while ((entitiesToProcess = entityList
                .Skip(skipRecordCount)
                .Take(batchSize) // Recommended batch size is 100
                .ToList()) != null
                && entitiesToProcess.Count > 0)
            {
                // Creating a batch of records
                createdRecordIds.AddRange(CreateMultiple(service, entitiesToProcess));

                skipRecordCount += entitiesToProcess.Count;
                entitiesToProcess.Clear();
            }

            return createdRecordIds;
        }

        /// <summary>
        /// Creates multiple records
        /// </summary>
        /// <param name="service">An authenticated client that implemnents the IOrganizationService interface.</param>
        /// <param name="entities">A List of entities to create.</param>
        /// <returns>The ID values of the created records.</returns>
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

        /// <summary>
        /// Updates multiple contoso_SensorData records in batches.
        /// </summary>
        /// <param name="service">An authenticated client that implemnents the IOrganizationService interface.</param>
        /// <param name="deviceId">The value used for the partitionid.</param>
        /// <param name="recordIds">The IDs of the records to update</param>
        /// <param name="batchSize">The size of the batch to use.</param>
        private static void UpdateMultipleRecords(
            IOrganizationService service,
            string deviceId,
            List<Guid> recordIds,
            int batchSize)
        {
            List<Entity> entityList = new();

            // Populate the list with the number of records to test.
            for (int i = 0; i < recordIds.Count; i++)
            {
                var keys = new KeyAttributeCollection() {
                    { "contoso_sensordataid", recordIds[i] },
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
                        { 
                            "contoso_energyconsumption", 
                            JsonConvert.SerializeObject(energyConsumption) 
                        }
                    }
                };

                entityList.Add(entity);
            }

            int skipRecordCount = 0;

            // Executing UpdateMultiple in batches.
            List<Entity> entitiesToProcess;
            while ((entitiesToProcess = entityList
                .Skip(skipRecordCount)
                .Take(batchSize)  // Recommended batch size is 100
                .ToList()) != null
                && entitiesToProcess.Count > 0)
            {
                // Updating a batch of records
                UpdateMultiple(service, entitiesToProcess);

                skipRecordCount += entitiesToProcess.Count;
                entitiesToProcess.Clear();
            }
        }

        /// <summary>
        /// Updates a group of records
        /// </summary>
        /// <param name="service">An authenticated client that implemnents the IOrganizationService interface.</param>
        /// <param name="entities">A List of contoso_sensordata Entities to update.</param>
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

        /// <summary>
        /// Executes a CosmosDB SQL query
        /// </summary>
        /// <param name="service">An authenticated client that implemnents the IOrganizationService interface.</param>
        /// <param name="query">The CosmosDB SQL query to run</param>
        /// <param name="deviceId">The partitionid value</param>
        /// <param name="pageSize">The pages size for the number of results to return.</param>
        /// <param name="pagingCookie">A paging cookied to retrieve multiple pages of results.</param>
        /// <returns>ExecuteCosmosSqlQueryResponse</returns>
        private static ExecuteCosmosSqlQueryResponse ExecuteCosmosSqlQuery(
            IOrganizationService service,
            string query,
            string deviceId,
            long pageSize = 100,
            string? pagingCookie = null)
        {
            // Using OrganizationRequest because SDK doesn't yet have ExecuteCosmosSqlQueryRequest class
            OrganizationRequest request = new("ExecuteCosmosSqlQuery")
            {
                Parameters = {
                    {"QueryText", query },
                    {"EntityLogicalName", "contoso_sensordata" },
                    {"PageSize", pageSize },
                    {"PagingCookie", pagingCookie },
                    {"PartitionId", deviceId }
                }

            };

            OrganizationResponse response = service.Execute(request);

            // ExecuteCosmosSqlQuery returns an Entity
            Entity resultEntity = (Entity)response.Results["Result"];

            // Get the known properties of the entity returned
            resultEntity.TryGetAttributeValue("PagingCookie", out pagingCookie);
            resultEntity.TryGetAttributeValue("HasMore", out bool hasMore);
            resultEntity.TryGetAttributeValue("Result", out string result);

            // Return the response with a custom ExecuteCosmosSqlQueryResponse class
            // ExecuteCosmosSqlQueryResponse is not in the SDK.
            return new ExecuteCosmosSqlQueryResponse
            {
                PagingCookie = pagingCookie,
                HasMore = hasMore,
                Result = result
            };

        }


        /// <summary>
        /// Deletes multiple records in batches
        /// </summary>
        /// <param name="service">An authenticated client that implemnents the IOrganizationService interface.</param>
        /// <param name="deviceId">The partitionid value</param>
        /// <param name="recordIds">The IDs of the records to delete.</param>
        /// <param name="batchSize">The number of records to delete in each batch.</param>
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
                .Take(batchSize) // Recommended batch size is 100
                .ToList()) != null
                && referencesToProcess.Count > 0)
            {
                DeleteMultiple(service, referencesToProcess);
                skipRecordCount += referencesToProcess.Count;
                referencesToProcess.Clear();
            }
        }

        /// <summary>
        /// Deletes a group of records
        /// </summary>
        /// <param name="service">An authenticated client that implemnents the IOrganizationService interface.</param>
        /// <param name="entityReferences">A list of EntityReferences for records to delte.</param>
        private static void DeleteMultiple(
            IOrganizationService service,
            List<EntityReference> entityReferences)
        {
            // Using OrganizationRequest because SDK doesn't have DeleteMultipleRequest class yet.
            OrganizationRequest deleteMultipleRequest = new()
            {
                RequestName = "DeleteMultiple",
                ["Targets"] = new EntityReferenceCollection(entityReferences)
            };
            service.Execute(deleteMultipleRequest);
        }
        
    }
}