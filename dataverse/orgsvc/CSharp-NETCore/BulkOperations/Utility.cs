using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace PowerPlatform.Dataverse.CodeSamples
{
    public static class Utility
    {
        /// <summary>
        /// Detects whether a table with the specified schema name exists.
        /// </summary>
        /// <param name="service">The IOrganizationService</param>
        /// <param name="tableSchemaName">The schema name of the table to check.</param>
        /// <returns></returns>
        private static bool TableExists(IOrganizationService service, string tableSchemaName)
        {
            EntityQueryExpression entityQueryExpression = new()
            {
                Properties = new MetadataPropertiesExpression("MetadataId"),
                Criteria = new MetadataFilterExpression(LogicalOperator.And)
                {
                    Conditions = {
                        {
                            new MetadataConditionExpression(
                                // Actually testing LogicalName to avoid case sensitivity issues
                                propertyName:"LogicalName",
                                conditionOperator:MetadataConditionOperator.Equals,
                                tableSchemaName.ToLower())
                        }
                    }
                }
            };
            RetrieveMetadataChangesRequest request = new()
            {
                Query = entityQueryExpression
            };

            var response = (RetrieveMetadataChangesResponse)service.Execute(request);

            // There can be only one or none.
            if (response.EntityMetadata.Count == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Asynchronously deletes a group of records for a specified table by id.
        /// </summary>
        /// <param name="service">The IOrganizationService</param>
        /// <param name="tableLogicalName">The logical name of the table.</param>
        /// <param name="iDs">The id values of the records to create.</param>
        /// <param name="jobName">A name for the system job to delete the records.</param>
        /// <returns></returns>
        public static string BulkDeleteRecordsByIds(
            IOrganizationService service,
            string tableLogicalName,
            Guid[] iDs,
            string jobName = "Records Deleted with BulkDeleteRecordsByIds sample method.")
        {
            // Assumption that every primary key name follows this format:
            string primarykeyName = $"{tableLogicalName}id";

            object[] valuesParameter = new object[iDs.Length];
            iDs.CopyTo(valuesParameter, 0);

            QueryExpression query = new(tableLogicalName)
            {
                ColumnSet = new ColumnSet(primarykeyName),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions = {
                        new ConditionExpression(
                            attributeName:primarykeyName,
                            conditionOperator: ConditionOperator.In,
                            values: iDs)
                    }
                }
            };


            BulkDeleteRequest bulkDeleteRequest = new()
            {
                QuerySet = new QueryExpression[] { query },
                JobName = jobName,                          // Required
                ToRecipients = new List<Guid>().ToArray(),  // Required
                CCRecipients = new List<Guid>().ToArray(),  // Required
                SendEmailNotification = false,              // Required
                RecurrencePattern = string.Empty            // Required
            };

            Stopwatch deleteAsync = Stopwatch.StartNew();
            BulkDeleteResponse bulkDeleteResponse = (BulkDeleteResponse)service.Execute(bulkDeleteRequest);

            int testLimit = 1000; // ~ 16 minutes
            int count = 0;
            string message = "\tAsynchronous job to delete {0} records completed in {1} seconds.";

            // Poll the system job every second to determine if it has finished.
            while (count < testLimit)
            {
                Task.Delay(1000).Wait(); // Wait a second

                Entity job = service.Retrieve(
                    entityName: "asyncoperation",
                    id: bulkDeleteResponse.JobId,
                    columnSet: new ColumnSet("statecode", "statuscode"));

                // When it is completed
                if (job.GetAttributeValue<OptionSetValue>("statecode").Value == 3)
                {

                    deleteAsync.Stop();
                    Console.WriteLine(string.Format(message, iDs.Length, Math.Round(deleteAsync.Elapsed.TotalSeconds)));

                    // Determine the status
                    return job.GetAttributeValue<OptionSetValue>("statuscode").Value switch
                    {
                        30 => "Succeeded",
                        31 => "Failed",
                        32 => "Canceled",
                        _ => "Error",
                    };
                }

                count++;
            }

            // If the test limit is exceeded
            deleteAsync.Stop();
            Console.WriteLine(string.Format(message, iDs.Length, Math.Round(deleteAsync.Elapsed.TotalSeconds)));
            return "TestLimitExceeded";
        }


        /// <summary>
        /// Deletes all the records for the sample_example table.
        /// </summary>
        /// <param name="service">The IOrganizationService</param>
        /// <returns>Status message</returns>
        public static string BulkDeleteAllSampleExampleRecords(
            IOrganizationService service)
        {

            QueryExpression query = new("sample_example")
            {
                ColumnSet = new ColumnSet("sample_exampleid"),
                Criteria = new FilterExpression(LogicalOperator.And) // No conditions
            };


            BulkDeleteRequest bulkDeleteRequest = new()
            {
                QuerySet = new QueryExpression[] { query },
                JobName = "Records Deleted with BulkDeleteAllSampleExampleRecords sample method.",
                ToRecipients = new List<Guid>().ToArray(),  // Required
                CCRecipients = new List<Guid>().ToArray(),  // Required
                SendEmailNotification = false,              // Required
                RecurrencePattern = string.Empty            // Required
            };

            Stopwatch deleteAsync = Stopwatch.StartNew();
            BulkDeleteResponse bulkDeleteResponse = (BulkDeleteResponse)service.Execute(bulkDeleteRequest);

            int testLimit = 1000; // ~ 16 minutes
            int count = 0;
            string message = "\tAsynchronous job to delete all records completed in {0} seconds.";

            // Poll the system job every second to determine if it has finished.
            while (count < testLimit)
            {
                Task.Delay(1000).Wait(); // Wait a second

                Entity job = service.Retrieve(
                    entityName: "asyncoperation",
                    id: bulkDeleteResponse.JobId,
                    columnSet: new ColumnSet("statecode", "statuscode"));

                // When it is completed
                if (job.GetAttributeValue<OptionSetValue>("statecode").Value == 3)
                {

                    deleteAsync.Stop();
                    Console.WriteLine(string.Format(message, Math.Round(deleteAsync.Elapsed.TotalSeconds)));

                    // Determine the status
                    return job.GetAttributeValue<OptionSetValue>("statuscode").Value switch
                    {
                        30 => "Succeeded",
                        31 => "Failed",
                        32 => "Canceled",
                        _ => "Error",
                    };
                }

                count++;
            }

            // If the test limit is exceeded
            deleteAsync.Stop();
            Console.WriteLine(string.Format(message, Math.Round(deleteAsync.Elapsed.TotalSeconds)));
            return "TestLimitExceeded";
        }

        /// <summary>
        /// Deletes all the records for the specified table.
        /// </summary>
        /// <param name="service">The IOrganizationService</param>
        /// <returns>Status message</returns>
        public static string BulkDeleteRecordsByEntityName(
            IOrganizationService service, string tableLogicalName)
        {
            var primaryKeyName = $"{tableLogicalName}id";

            QueryExpression query = new(tableLogicalName)
            {
                ColumnSet = new ColumnSet(primaryKeyName),
                Criteria = new FilterExpression(LogicalOperator.And) // No conditions
            };


            BulkDeleteRequest bulkDeleteRequest = new()
            {
                QuerySet = new QueryExpression[] { query },
                JobName = "Records Deleted with BulkDeleteRecordsByEntityName sample method.",
                ToRecipients = new List<Guid>().ToArray(),  // Required
                CCRecipients = new List<Guid>().ToArray(),  // Required
                SendEmailNotification = false,              // Required
                RecurrencePattern = string.Empty            // Required
            };

            Stopwatch deleteAsync = Stopwatch.StartNew();
            BulkDeleteResponse bulkDeleteResponse = (BulkDeleteResponse)service.Execute(bulkDeleteRequest);

            int testLimit = 1000; // ~ 16 minutes
            int count = 0;
            string message = "\tAsynchronous job to delete all records completed in {0} seconds.";

            // Poll the system job every second to determine if it has finished.
            while (count < testLimit)
            {
                Task.Delay(1000).Wait(); // Wait a second

                Entity job = service.Retrieve(
                    entityName: "asyncoperation",
                    id: bulkDeleteResponse.JobId,
                    columnSet: new ColumnSet("statecode", "statuscode"));

                // When it is completed
                if (job.GetAttributeValue<OptionSetValue>("statecode").Value == 3)
                {

                    deleteAsync.Stop();
                    Console.WriteLine(string.Format(message, Math.Round(deleteAsync.Elapsed.TotalSeconds)));

                    // Determine the status
                    return job.GetAttributeValue<OptionSetValue>("statuscode").Value switch
                    {
                        30 => "Succeeded",
                        31 => "Failed",
                        32 => "Canceled",
                        _ => "Error",
                    };
                }

                count++;
            }

            // If the test limit is exceeded
            deleteAsync.Stop();
            Console.WriteLine(string.Format(message, Math.Round(deleteAsync.Elapsed.TotalSeconds)));
            return "TestLimitExceeded";
        }

        /// <summary>
        /// Detect whether a specified message is supported for the specified table.
        /// </summary>
        /// <param name="service">The IOrganizationService instance.</param>
        /// <param name="entityLogicalName">The logical name of the table.</param>
        /// <param name="messageName">The name of the message.</param>
        /// <returns></returns>
        public static bool IsMessageAvailable(
            IOrganizationService service,
            string entityLogicalName,
            string messageName)
        {
            QueryExpression query = new("sdkmessagefilter")
            {
                ColumnSet = new ColumnSet("sdkmessagefilterid"),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions = {
            new ConditionExpression(
                attributeName:"primaryobjecttypecode",
                conditionOperator: ConditionOperator.Equal,
                value: entityLogicalName)
            }
                },
                LinkEntities = {
            new LinkEntity(
                linkFromEntityName:"sdkmessagefilter",
                linkToEntityName:"sdkmessage",
                linkFromAttributeName:"sdkmessageid",
                linkToAttributeName:"sdkmessageid",
                joinOperator: JoinOperator.Inner)
            {
                    LinkCriteria = new FilterExpression(LogicalOperator.And){
                    Conditions = {
                        new ConditionExpression(
                            attributeName:"name",
                            conditionOperator: ConditionOperator.Equal,
                            value: messageName)
                        }
                    }
            }
        }
            };

            EntityCollection entityCollection = service.RetrieveMultiple(query);

            return entityCollection.Entities.Count.Equals(1);
        }

        /// <summary>
        /// Creates the table used by projects in this solution.
        /// </summary>
        /// <param name="serviceClient">The ServiceClient instance.</param>
        /// <param name="tableSchemaName">The SchemaName of the table to create.</param>
        public static async void CreateExampleTable(ServiceClient serviceClient, 
            string tableSchemaName, 
            bool isElastic = false,
            bool createAlternateKey = false)
        {
            // Forces metadata cache to be updated to prevent error
            // Creating attributes immediately after
            serviceClient.ForceServerMetadataCacheConsistency = true;

            // Don't create the table if it already exists
            if (TableExists(serviceClient, tableSchemaName.ToLower()))
            {
                Console.WriteLine($"{tableSchemaName} table already exists.");
                return;
            }

            Console.WriteLine($"Creating {tableSchemaName} {(isElastic ? "Elastic" : "Standard")} table...");

            if (isElastic)
            {
                JObject entityMetadataObject = new()
            {
                { "@odata.type", "Microsoft.Dynamics.CRM.EntityMetadata"},
                { "SchemaName", tableSchemaName },
                { "DisplayName", new JObject()
                    {
                        { "@odata.type", "Microsoft.Dynamics.CRM.Label" },
                        { "LocalizedLabels", new JArray()
                            {
                                new JObject()
                                {
                                    { "@odata.type", "Microsoft.Dynamics.CRM.LocalizedLabel" },
                                    { "Label", $"Example {(isElastic? "(Elastic)": "(Standard)")}" },
                                    { "LanguageCode", 1033}
                                }
                            }
                        }
                    }
                },
                { "DisplayCollectionName", new JObject()
                    {
                        { "@odata.type", "Microsoft.Dynamics.CRM.Label" },
                        { "LocalizedLabels", new JArray()
                            {
                                new JObject()
                                {
                                    {"@odata.type", "Microsoft.Dynamics.CRM.LocalizedLabel" },
                                    { "Label", "Examples" },
                                    { "LanguageCode", 1033 }
                                }
                            }
                        }
                    }
                },
                { "Description", new JObject()
                    {
                        { "@odata.type",  "Microsoft.Dynamics.CRM.Label" },
                        { "LocalizedLabels", new JArray()
                            {
                                new JObject()
                                {
                                    {"@odata.type", "Microsoft.Dynamics.CRM.LocalizedLabel" },
                                    { "Label", "A table for code samples."},
                                    { "LanguageCode", 1033 }
                                }
                            }
                        }
                    }
                },
                { "OwnershipType", "UserOwned" },
                { "TableType", "Elastic" },
                { "IsActivity", false },
                { "CanCreateCharts", new JObject()
                    {
                        { "Value", false },
                        { "CanBeChanged", true },
                        { "ManagedPropertyLogicalName", "cancreatecharts" }
                    }
                },
                { "HasActivities", false },
                { "HasNotes", false },
                { "Attributes", new JArray()
                    {
                        new JObject()
                        {
                            { "@odata.type", "Microsoft.Dynamics.CRM.StringAttributeMetadata"},
                            { "SchemaName", "sample_Name"},
                            { "AttributeType", "String" },
                            { "AttributeTypeName", new JObject()
                                {
                                    { "Value", "StringType" }
                                }
                            },
                            { "RequiredLevel", new JObject()
                                {
                                    { "Value", "None" },
                                    { "CanBeChanged", true},
                                    { "ManagedPropertyLogicalName", "canmodifyrequirementlevelsettings"}
                                }
                            },
                            { "MaxLength", 100 },
                            { "FormatName", new JObject()
                                {
                                    { "Value", "Text"}
                                }
                            },
                            { "DisplayName", new JObject()
                                {
                                    { "@odata.type", "Microsoft.Dynamics.CRM.Label" },
                                    { "LocalizedLabels", new JArray()
                                        {
                                            new JObject()
                                            {
                                                { "@odata.type", "Microsoft.Dynamics.CRM.LocalizedLabel"},
                                                { "Label", "Sensor Type" },
                                                { "LanguageCode", 1033}
                                            }
                                        }
                                    }
                                }
                            },
                            { "Description", new JObject()
                                {
                                    { "@odata.type", "Microsoft.Dynamics.CRM.Label" },
                                    { "LocalizedLabels", new JArray()
                                        {
                                            new JObject()
                                            {
                                                { "@odata.type", "Microsoft.Dynamics.CRM.LocalizedLabel"},
                                                { "Label", "Type of sensor emitting data" },
                                                { "LanguageCode", 1033 }
                                            }
                                        }
                                    }
                                }
                            },
                            { "IsPrimaryName", true }
                        }
                    }
                }
            };

                Dictionary<string, List<string>> customHeaders = new()
                {
                    ["Content-Type"] = new List<string>() { "application/json" },
                    ["Consistency"] = new List<string>() { "Strong" }
                };
                serviceClient.ExecuteWebRequest(
                    method: HttpMethod.Post,
                    queryString: "EntityDefinitions",
                    body: entityMetadataObject.ToString(),
                    customHeaders: customHeaders);

              
            }
            else
            {
                // Create standard table
                CreateEntityRequest createEntityRequest = new()
                {
                    Entity = new EntityMetadata
                    {
                        SchemaName = tableSchemaName,
                        DisplayName = new Label($"Example {(isElastic ? "(Elastic)" : "(Standard)")}", 1033),
                        DisplayCollectionName = new Label("Examples", 1033),
                        Description = new Label("A table for code samples.", 1033),
                        OwnershipType = OwnershipTypes.UserOwned,
                        IsActivity = false

                    },
                    PrimaryAttribute = new StringAttributeMetadata
                    {
                        SchemaName = "sample_Name",
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                        MaxLength = 100,
                        FormatName = StringFormatName.Text,
                        DisplayName = new Label("Example Name", 1033),
                        Description = new Label("The name of the example record.", 1033)
                    },
                    HasActivities = false,
                    HasNotes = false

                };
                serviceClient.Execute(createEntityRequest);

                if (createAlternateKey)
                {
                    // Create attributes that will form the alternate key
                    string keySchemaName = "sample_key";
                    string keyAttribute1 = "sample_keyattribute";


                    CreateAttributeRequest createKeyAttributeRequest = new()
                    {
                        EntityName = tableSchemaName.ToLower(),
                        Attribute = new StringAttributeMetadata
                        {
                            SchemaName = keyAttribute1,
                            MaxLength = 100,
                            FormatName = StringFormatName.Text,
                            DisplayName = new Label(keyAttribute1, 1033),
                        },
                    };
                    serviceClient.Execute(createKeyAttributeRequest);

                    // Create an alternate key on the entity using the attributes created
                    CreateEntityKeyRequest createEntityKeyRequest = new()
                    {
                        EntityName = tableSchemaName.ToLower(),
                        EntityKey = new EntityKeyMetadata
                        {
                            SchemaName = keySchemaName,
                            KeyAttributes = new string[1] { keyAttribute1 },
                            DisplayName = new Label(keySchemaName, 1033)
                        },
                    };
                    serviceClient.Execute(createEntityKeyRequest);
                }
            }

            Console.WriteLine($"\t{tableSchemaName} table created.");

            Console.WriteLine($"Adding 'sample_Description' column to {tableSchemaName} table...");
            CreateAttributeRequest createAttributeRequest = new()
            {
                Attribute = new StringAttributeMetadata
                {
                    SchemaName = "sample_Description",
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                    MaxLength = 1000,
                    FormatName = StringFormatName.TextArea,
                    DisplayName = new Label("Description", 1033),
                    Description = new Label("The description of the example record.", 1033)
                },
                EntityName = tableSchemaName.ToLower()
            };

            serviceClient.Execute(createAttributeRequest);
            Console.WriteLine($"\t'sample_Description' column created.");

            // Turning off for best performance
            serviceClient.ForceServerMetadataCacheConsistency = false;


        }

        /// <summary>
        /// Deletes the table used by projects in this solution.
        /// </summary>
        /// <param name="service">The IOrganizationService instance.</param>
        /// <param name="tableSchemaName">The SchemaName of the table to delete.</param>
        public static void DeleteExampleTable(IOrganizationService service, string tableSchemaName)
        {
            if (Settings.DeleteTable)
            {
#pragma warning disable CS0162 // Unreachable code detected: Based on configuration.
                if (TableExists(service, tableSchemaName.ToLower()))
                {
                    Console.WriteLine($"\nDeleting {tableSchemaName} table...");

                    DeleteEntityRequest request = new()
                    {
                        LogicalName = tableSchemaName.ToLower()
                    };

                    service.Execute(request);
                    Console.WriteLine($"\t{tableSchemaName} table deleted.");
                }
                else
                {
                    Console.WriteLine($"{tableSchemaName} table doesn't exist.");
                }
#pragma warning restore CS0162 // Unreachable code detected: Based on configuration.
            }
            else
            {
#pragma warning disable CS0162 // Unreachable code detected: Based on configuration.
                Console.WriteLine($"Not deleteing the {tableSchemaName} table based on setting.");
#pragma warning restore CS0162 // Unreachable code detected: Based on configuration.
            }

        }


        /// <summary>
        /// Alternate keys may not be active immediately after a solution defining them is installed.
        /// This method polls the metadata for a specific entity
        /// to delay execution of the rest of the sample until the alternate keys are ready.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        /// <param name="asyncJob">The system job that creates the index to support the alternate key</param>
        /// <param name="iteration">The number of times this method has been called.</param>
        /// 
        internal static bool VerifyAlternateKeyIsActive(IOrganizationService service, string tableLogicalName)
        {
            var waitTime = TimeSpan.FromMinutes(10);
            var startTime = DateTime.UtcNow;
            bool isKeyInMetadata = false;

            while (!isKeyInMetadata)
            {
                //Get whether the Entity Key index is active from the metadata
                var entityQuery = new EntityQueryExpression
                {
                    Criteria = new MetadataFilterExpression(LogicalOperator.And)
                    {
                        Conditions = { 
                            { 
                                new MetadataConditionExpression("LogicalName", MetadataConditionOperator.Equals, tableLogicalName) 
                            } 
                        }
                    },

                    Properties = new MetadataPropertiesExpression("Keys")
                };

                var request = new RetrieveMetadataChangesRequest() { Query = entityQuery };
                var response = (RetrieveMetadataChangesResponse)service.Execute(request);
                var alternateKey = response?.EntityMetadata?.FirstOrDefault()?.Keys.FirstOrDefault();

                if (alternateKey?.EntityKeyIndexStatus == EntityKeyIndexStatus.Active)
                {
                    isKeyInMetadata = true;
                }
                else
                {
                    var elapsed = DateTime.UtcNow - startTime;
                    if (elapsed > waitTime)
                    {
                        return false;
                    }

                    Thread.Sleep(100);
                }
            }

            return isKeyInMetadata;

        }


        /// <summary>
        /// Creates a number of records for the UpsertMultiple example
        /// </summary>
        /// <param name="numberOfRecords">The number of records to create</param>
        /// <param name="tableLogicalName">The logical name of the table</param>
        /// <param name="service">Specifies the service to connect to.</param>
        /// <param name="entities">The container of the records created.</param>
        /// <returns>The IDs of the created records</returns>
        public static Guid[] CreateRecordsUtility(int numberOfRecords, 
            string tableLogicalName, 
            IOrganizationService service, 
            out EntityCollection entities)
        {
            // Create a List of entity instances.
            Console.WriteLine($"\nPreparing {numberOfRecords} entities to create..");
            List<Entity> entityList = new();
            // Populate the list with the number of records to test.
            for (int i = 0; i < numberOfRecords; i++)
            {
                Entity entity = new(tableLogicalName);

                // Example: 'sample record 0000001'
                // Example key: 'sample record key 0000001'
                entity["sample_name"] = $"sample record {i + 1:0000000}";

                if (Settings.CreateAlternateKey)
                {
                    if (!Settings.UseElastic)
                    {
#pragma warning disable CS0162 // Unreachable code detected
                        entity["sample_keyattribute"] = $"sample pre-upsert record key {i + 1:0000000}";
#pragma warning restore CS0162 // Unreachable code detected
                    }
                    else
                    {
                        entity.KeyAttributes.Add(tableLogicalName + "id", Guid.NewGuid());
                        entity.KeyAttributes.Add("partitionid", $"sample pre-upsert record key {i + 1:0000000}");
                    }
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
                (CreateMultipleResponse)service.Execute(createMultipleRequest);

            Console.WriteLine($"\tCreated {numberOfRecords} records to upsert..\n");

            return createMultipleResponse.Ids;
        }
    }
}


