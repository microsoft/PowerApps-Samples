using Newtonsoft.Json.Linq;
using PowerApps.Samples;
using PowerApps.Samples.Messages;
using PowerApps.Samples.Metadata.Messages;
using PowerApps.Samples.Metadata.Types;
using PowerApps.Samples.Methods;
using PowerApps.Samples.Types;
using System.Diagnostics;

namespace PowerPlatform.Dataverse.CodeSamples
{
    public static class Utility
    {

        /// <summary>
        /// Detects whether a table with the specified schema name exists.
        /// </summary>
        /// <param name="service">The WebAPIService</param>
        /// <param name="tableSchemaName">The schema name of the table to check.</param>
        /// <returns></returns>
        private static async Task<bool> TableExists(Service service, string tableSchemaName)
        {
            EntityQueryExpression entityQueryExpression = new()
            {
                Properties = new MetadataPropertiesExpression("MetadataId"),
                Criteria = new MetadataFilterExpression(LogicalOperator.And)
                {
                    Conditions = {
                        {
                            new MetadataConditionExpression(
                                conditionOperator:MetadataConditionOperator.Equals,
                                // Actually testing LogicalName to avoid case sensitivity issues
                                propertyName:"LogicalName",
                                value: new PowerApps.Samples.Types.Object(
                                    type: ObjectType.String,
                                    value: tableSchemaName.ToLower()))
                        }
                    }
                }
            };
            RetrieveMetadataChangesRequest request = new()
            {
                Query = entityQueryExpression
            };

            var response = await service.SendAsync<RetrieveMetadataChangesResponse>(request);

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
        /// <param name="service">The WebAPIService</param>
        /// <param name="tableLogicalName">The logical name of the table.</param>
        /// <param name="iDs">The id values of the records to create.</param>
        /// <param name="jobName">A name for the system job to delete the records.</param>
        /// <returns></returns>
        public static async Task<string> BulkDeleteRecordsByIds(
            Service service,
            string tableLogicalName,
            Guid[] iDs,
            string jobName = "Records Deleted with BulkDeleteRecordsByIds sample method.")
        {
            // Assumption that every primary key name follows this format:
            string primarykeyName = $"{tableLogicalName}id";

            List<PowerApps.Samples.Types.Object> valuesParameter = new();
            foreach (Guid Id in iDs)
            {
                valuesParameter.Add(new PowerApps.Samples.Types.Object(type: ObjectType.Guid, value: Id.ToString()));
            }


            QueryExpression query = new(tableLogicalName)
            {
                ColumnSet = new ColumnSet(primarykeyName),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions = {
                        new ConditionExpression(
                            entityName: tableLogicalName,
                            attributeName:primarykeyName,
                            conditionOperator: ConditionOperator.In,
                            values: valuesParameter)
                    }
                }
            };


            BulkDeleteRequest bulkDeleteRequest = new()
            {
                QuerySet = new List<QueryExpression> { query },
                JobName = jobName,                   // Required
                ToRecipients = new List<JObject>(),  // Required
                CCRecipients = new List<JObject>(),  // Required
                SendEmailNotification = false,       // Required
                RecurrencePattern = "",              // Required
                StartDateTime = DateTime.Now,

            };

            Stopwatch deleteAsync = Stopwatch.StartNew();
            BulkDeleteResponse bulkDeleteResponse = await service.SendAsync<BulkDeleteResponse>(bulkDeleteRequest);

            int testLimit = 1000; // ~ 16 minutes
            int count = 0;
            string message = "\tAsynchronous job to delete {0} records completed in {1} seconds.";

            // Poll the system job every second to determine if it has finished.
            while (count < testLimit)
            {
                Task.Delay(1000).Wait(); // Wait a second             

                JObject job = await service.Retrieve(
                     entityReference: new EntityReference(
                            entitySetName: "asyncoperations",
                            id: bulkDeleteResponse.JobId),
                    query: "?$select=statecode,statuscode");

                // When it is completed
                if (((int)job.GetValue("statecode")) == 3)
                {

                    deleteAsync.Stop();
                    Console.WriteLine(string.Format(message, iDs.Length, Math.Round(deleteAsync.Elapsed.TotalSeconds)));

                    // Determine the status
                    return ((int)job.GetValue("statuscode")) switch
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
        /// Detect whether a specified message is supported for the specified table.
        /// </summary>
        /// <param name="service">The WebAPIService instance.</param>
        /// <param name="entityLogicalName">The logical name of the table.</param>
        /// <param name="messageName">The name of the message.</param>
        /// <returns></returns>
        public static async Task<bool> IsMessageAvailable(
            Service service,
            string entityLogicalName,
            string messageName)
        {
            string queryUri = $"sdkmessagefilters?$select=sdkmessagefilterid&$filter=sdkmessageid/name eq '{messageName}' and primaryobjecttypecode eq '{entityLogicalName}'";
            RetrieveMultipleRequest request = new(queryUri);

            request.Headers.Add("Consistency", "Strong");

            var response = await service.SendAsync<RetrieveMultipleResponse>(request);

            return response.Records.Count.Equals(1);
        }

        /// <summary>
        /// Creates the table used by projects in this solution.
        /// </summary>
        /// <param name="service">The WebAPIService instance.</param>
        /// <param name="tableSchemaName">The SchemaName of the table to create.</param>
        public static async Task CreateExampleTable(Service service, string tableSchemaName, bool isElastic = false)
        {

            // Don't create the table if it already exists
            if (await TableExists(service, tableSchemaName.ToLower()))
            {
                Console.WriteLine($"{tableSchemaName} table already exists.");
                return;
            }

            Console.WriteLine($"Creating {tableSchemaName} {(isElastic ? "Elastic" : "Standard")} table...");


            var entity = new EntityMetadata()
            {
                SchemaName = tableSchemaName,
                DisplayName = new Label($"Example {(isElastic ? "(Elastic)" : "(Standard)")}", 1033),
                DisplayCollectionName = new Label("Examples", 1033),
                Description = new Label("A table for code samples.", 1033),
                OwnershipType = OwnershipTypes.UserOwned,
                IsActivity = false,
                TableType = isElastic ? "Elastic" : "Standard",
                HasActivities = false,
                HasNotes = false,
                CanCreateCharts = new BooleanManagedProperty(value: false),
                Attributes = new List<AttributeMetadata>() {

                    {new StringAttributeMetadata()
                        {
                            SchemaName = "sample_Name",
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                            MaxLength = 100,
                            FormatName = new StringFormatName(StringFormatNameValues.Text),
                            DisplayName = new Label("Example Name", 1033),
                            Description = new Label("The name of the example record.", 1033),
                            IsPrimaryName = true
                        }
                    },
                    {new StringAttributeMetadata
                        {
                            SchemaName = "sample_Description",
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                            MaxLength = 1000,
                            FormatName = new StringFormatName(StringFormatNameValues.TextArea),
                            DisplayName = new Label("Description", 1033),
                            Description = new Label("The description of the example record.", 1033)
                        }}
                }
            };

            CreateEntityRequest createEntityRequest = new(entityMetadata: entity, solutionUniqueName: null, useStrongConsistency: true);
            await service.SendAsync<CreateEntityResponse>(createEntityRequest);

        }

        /// <summary>
        /// Deletes the table used by projects in this solution.
        /// </summary>
        /// <param name="service">The WebAPIService instance.</param>
        /// <param name="tableSchemaName">The SchemaName of the table to delete.</param>
        public static async Task DeleteExampleTable(Service service, string tableSchemaName)
        {
            if (Settings.DeleteTable)
            {
#pragma warning disable CS0162 // Unreachable code detected: Based on configuration.
                if (await TableExists(service, tableSchemaName.ToLower()))
                {
                    Console.WriteLine($"\nDeleting {tableSchemaName} table...");

                    Dictionary<string, string> keyAttributes = new() {
                        {"LogicalName",$"'{tableSchemaName.ToLower()}'" }
                    };

                    await service.Delete(new EntityReference(setName: "EntityDefinitions", keyAttributes: keyAttributes));


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


    }
}


