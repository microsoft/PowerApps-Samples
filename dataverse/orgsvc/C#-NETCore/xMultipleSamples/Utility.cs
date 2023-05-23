using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
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
        /// <param name="service">The IOrganizationService instance.</param>
        /// <param name="tableSchemaName">The SchemaName of the table to create.</param>
        public static void CreateExampleTable(IOrganizationService service, string tableSchemaName)
        {
            // Don't create the table if it already exists
            if (TableExists(service, tableSchemaName.ToLower()))
            {
                Console.WriteLine($"{tableSchemaName} table already exists.");
                return;
            }

            Console.WriteLine($"Creating {tableSchemaName} table...");

            CreateEntityRequest createEntityRequest = new()
            {
                Entity = new EntityMetadata
                {
                    SchemaName = tableSchemaName,
                    DisplayName = new Label("Example", 1033),
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
                HasActivities = true

            };

            service.Execute(createEntityRequest);
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
                EntityName = tableSchemaName.ToLower(),
            };

            service.Execute(createAttributeRequest);
            Console.WriteLine($"\t'sample_Description' column created.");


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
                    Console.WriteLine($"Deleting {tableSchemaName} table...");

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


    }
}


