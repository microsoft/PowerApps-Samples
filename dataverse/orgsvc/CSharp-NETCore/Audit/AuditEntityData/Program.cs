using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates how to enable auditing on an entity and retrieve the change history
    /// </summary>
    /// <remarks>
    /// This sample shows how to:
    /// - Enable auditing on the organization and account entity
    /// - Create and update an account record to generate audit history
    /// - Retrieve record change history
    /// - Retrieve attribute change history
    /// - Retrieve audit details
    ///
    /// Prerequisites:
    /// - System Administrator or System Customizer role
    /// - Auditing feature available in your Dataverse environment
    ///
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// </remarks>
    class Program
    {
        private static readonly List<EntityReference> entityStore = new();
        private static bool organizationAuditingFlag;
        private static bool accountAuditingFlag;

        #region Sample Methods

        /// <summary>
        /// Sets up sample data by enabling auditing and creating an account
        /// </summary>
        private static void Setup(ServiceClient service)
        {
            Console.WriteLine("Enabling auditing on the organization and account entities...");

            // Enable auditing on the organization
            Guid orgId = ((WhoAmIResponse)service.Execute(new WhoAmIRequest())).OrganizationId;

            // Retrieve the organization's record to get current audit setting
            var retrievedOrg = service.Retrieve("organization", orgId, new ColumnSet("isauditenabled"));

            // Cache the value to restore it later
            organizationAuditingFlag = retrievedOrg.GetAttributeValue<bool>("isauditenabled");

            // Enable auditing on the organization
            var orgToUpdate = new Entity("organization")
            {
                Id = orgId,
                ["isauditenabled"] = true
            };
            service.Update(orgToUpdate);

            // Enable auditing on account entities
            accountAuditingFlag = EnableEntityAuditing(service, "account", true);

            // Create an account
            Console.WriteLine("Creating an account...");
            var newAccount = new Entity("account")
            {
                ["name"] = "Example Account"
            };
            Guid accountId = service.Create(newAccount);
            entityStore.Add(new EntityReference("account", accountId));

            Console.WriteLine("Updating the account...");
            var accountToUpdate = new Entity("account")
            {
                Id = accountId,
                ["accountnumber"] = "1-A",
                ["accountcategorycode"] = new OptionSetValue(1), // Preferred Customer
                ["telephone1"] = "555-555-5555"
            };
            service.Update(accountToUpdate);

            Console.WriteLine("Setup complete.");
            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrates auditing operations including retrieving change history
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Guid accountId = entityStore[0].Id;

            // Retrieve the record change history
            Console.WriteLine("Retrieving the account change history...");
            var changeRequest = new RetrieveRecordChangeHistoryRequest
            {
                Target = new EntityReference("account", accountId)
            };

            var changeResponse = (RetrieveRecordChangeHistoryResponse)service.Execute(changeRequest);
            var details = changeResponse.AuditDetailCollection;

            foreach (AuditDetail detail in details.AuditDetails)
            {
                DisplayAuditDetails(service, detail);
            }

            // Update the Telephone1 attribute to generate more audit history
            Console.WriteLine("Updating the Telephone1 field in the Account entity...");
            var accountToUpdate = new Entity("account")
            {
                Id = accountId,
                ["telephone1"] = "123-555-5555"
            };
            service.Update(accountToUpdate);

            // Retrieve the attribute change history
            Console.WriteLine("Retrieving the attribute change history for Telephone1...");
            var attributeChangeHistoryRequest = new RetrieveAttributeChangeHistoryRequest
            {
                Target = new EntityReference("account", accountId),
                AttributeLogicalName = "telephone1"
            };

            var attributeChangeHistoryResponse =
                (RetrieveAttributeChangeHistoryResponse)service.Execute(attributeChangeHistoryRequest);

            // Display the attribute change history
            var attributeDetails = attributeChangeHistoryResponse.AuditDetailCollection;

            foreach (var detail in attributeDetails.AuditDetails)
            {
                DisplayAuditDetails(service, detail);
            }

            // Retrieve audit details for a specific audit record
            if (attributeDetails.AuditDetails.Count > 0)
            {
                Guid auditSampleId = attributeDetails.AuditDetails[0].AuditRecord.Id;

                Console.WriteLine("Retrieving audit details for an audit record...");
                var auditDetailsRequest = new RetrieveAuditDetailsRequest
                {
                    AuditId = auditSampleId
                };

                var auditDetailsResponse = (RetrieveAuditDetailsResponse)service.Execute(auditDetailsRequest);
                DisplayAuditDetails(service, auditDetailsResponse.AuditDetail);
            }

            Console.WriteLine("Audit operations complete.");
        }

        /// <summary>
        /// Cleans up sample data and restores audit settings
        /// </summary>
        private static void Cleanup(ServiceClient service, bool deleteCreatedRecords)
        {
            Console.WriteLine("Restoring audit settings...");

            // Restore organization auditing setting
            Guid orgId = ((WhoAmIResponse)service.Execute(new WhoAmIRequest())).OrganizationId;
            var orgToUpdate = new Entity("organization")
            {
                Id = orgId,
                ["isauditenabled"] = organizationAuditingFlag
            };
            service.Update(orgToUpdate);

            // Restore account entity auditing setting
            EnableEntityAuditing(service, "account", accountAuditingFlag);

            Console.WriteLine("Audit settings restored.");

            // Delete created records
            if (deleteCreatedRecords && entityStore.Count > 0)
            {
                Console.WriteLine($"Deleting {entityStore.Count} created record(s)...");
                foreach (var entityRef in entityStore)
                {
                    service.Delete(entityRef.LogicalName, entityRef.Id);
                }
                Console.WriteLine("Records deleted.");
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Enable or disable auditing on an entity
        /// </summary>
        /// <param name="service">The service client</param>
        /// <param name="entityLogicalName">The logical name of the entity</param>
        /// <param name="flag">True to enable auditing, false to disable</param>
        /// <returns>The previous value of the IsAuditEnabled attribute</returns>
        private static bool EnableEntityAuditing(ServiceClient service, string entityLogicalName, bool flag)
        {
            // Retrieve the entity metadata
            var entityRequest = new RetrieveEntityRequest
            {
                LogicalName = entityLogicalName,
                EntityFilters = EntityFilters.Entity
            };

            var entityResponse = (RetrieveEntityResponse)service.Execute(entityRequest);

            // Enable or disable auditing on the entity
            EntityMetadata entityMetadata = entityResponse.EntityMetadata;
            bool oldValue = entityMetadata.IsAuditEnabled?.Value ?? false;
            entityMetadata.IsAuditEnabled = new BooleanManagedProperty(flag);

            var updateEntityRequest = new UpdateEntityRequest { Entity = entityMetadata };
            service.Execute(updateEntityRequest);

            return oldValue;
        }

        /// <summary>
        /// Displays audit change history details on the console
        /// </summary>
        /// <param name="service">The service client</param>
        /// <param name="detail">The audit detail to display</param>
        private static void DisplayAuditDetails(ServiceClient service, AuditDetail detail)
        {
            var record = detail.AuditRecord;

            Console.WriteLine($"\nAudit record created on: {record.GetAttributeValue<DateTime>("createdon").ToLocalTime()}");

            var objectId = record.GetAttributeValue<EntityReference>("objectid");
            string action = record.FormattedValues.ContainsKey("action") ? record.FormattedValues["action"] : "N/A";
            string operation = record.FormattedValues.ContainsKey("operation") ? record.FormattedValues["operation"] : "N/A";

            Console.WriteLine($"Entity: {objectId?.LogicalName}, Action: {action}, Operation: {operation}");

            var userId = record.GetAttributeValue<EntityReference>("userid");
            Console.WriteLine($"Operation performed by {userId?.Name ?? userId?.Id.ToString() ?? "Unknown"}");

            // Show additional details for AttributeAuditDetail
            if (detail is AttributeAuditDetail attributeDetail)
            {
                string oldValue = "(no value)", newValue = "(no value)";

                // Display the old and new attribute values
                if (attributeDetail.NewValue != null)
                {
                    foreach (var attribute in attributeDetail.NewValue.Attributes)
                    {
                        if (attributeDetail.OldValue?.Contains(attribute.Key) == true)
                        {
                            oldValue = GetTypedValueAsString(attributeDetail.OldValue[attribute.Key]);
                        }

                        newValue = GetTypedValueAsString(attributeDetail.NewValue[attribute.Key]);

                        Console.WriteLine($"Attribute: {attribute.Key}, old value: {oldValue}, new value: {newValue}");
                    }
                }

                if (attributeDetail.OldValue != null)
                {
                    foreach (var attribute in attributeDetail.OldValue.Attributes)
                    {
                        if (attributeDetail.NewValue?.Contains(attribute.Key) != true)
                        {
                            newValue = "(no value)";
                            oldValue = GetTypedValueAsString(attributeDetail.OldValue[attribute.Key]);

                            Console.WriteLine($"Attribute: {attribute.Key}, old value: {oldValue}, new value: {newValue}");
                        }
                    }
                }
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Returns a string representation of an attribute value
        /// </summary>
        /// <param name="typedValue">The attribute value</param>
        /// <returns>String representation of the value</returns>
        private static string GetTypedValueAsString(object? typedValue)
        {
            if (typedValue == null) return "(null)";

            return typedValue switch
            {
                OptionSetValue o => o.Value.ToString(),
                EntityReference e => $"LogicalName:{e.LogicalName},Id:{e.Id},Name:{e.Name}",
                _ => typedValue.ToString() ?? "(null)"
            };
        }

        #endregion

        #region Application Setup

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

        static void Main(string[] args)
        {
            Program app = new();

            // Create a Dataverse service client using the default connection string.
            ServiceClient serviceClient =
                new(app.Configuration.GetConnectionString("default"));

            if (!serviceClient.IsReady)
            {
                Console.WriteLine("Failed to connect to Dataverse.");
                Console.WriteLine("Error: {0}", serviceClient.LastError);
                return;
            }

            Console.WriteLine("Connected to Dataverse.");
            Console.WriteLine();

            bool deleteCreatedRecords = true;

            try
            {
                Setup(serviceClient);
                Run(serviceClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred:");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Cleanup(serviceClient, deleteCreatedRecords);

                Console.WriteLine();
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                serviceClient.Dispose();
            }
        }

        #endregion
    }
}
