using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {

        private static Guid _newAccountId;
        private static bool prompt = true;

        /// <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        /// 
        private static void SetUpSample(CrmServiceClient service)
        {
            // Check that the current version is greater than the minimum version
            if (!SampleHelpers.CheckVersion(service, new Version("7.1.0.0")))
            {
                //The environment version is lower than version 7.1.0.0
                return;
            }
            CreateRequiredRecords(service);


        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, prompt);
        }


        /// <summary>
        /// Displays audit change history details on the console.
        /// </summary>
        /// <param name="detail"></param>
        private static void DisplayAuditDetails(CrmServiceClient service, AuditDetail detail)
        {
            // Write out some of the change history information in the audit record. 
            var record = (Audit)detail.AuditRecord;

            Console.WriteLine($"\nAudit record created on: {record.CreatedOn.Value.ToLocalTime()}");
            Console.WriteLine($"Entity: {record.ObjectId.LogicalName}, Action: {record.FormattedValues["action"]}, Operation: {record.FormattedValues["operation"]}");
            Console.WriteLine($"Operation performed by {record.UserId.Name}");

            // Show additional details for certain AuditDetail sub-types.
            var detailType = detail.GetType();
            if (detailType == typeof(AttributeAuditDetail))
            {
                var attributeDetail = (AttributeAuditDetail)detail;
                string oldValue = "(no value)", newValue = "(no value)";

                // Display the old and new attribute values.
                foreach (KeyValuePair<string, object> attribute in attributeDetail.NewValue.Attributes)
                {

                    if (attributeDetail.OldValue.Contains(attribute.Key))
                    {
                        oldValue = GetTypedValueAsString(attributeDetail.OldValue[attribute.Key]);
                    }

                    newValue = GetTypedValueAsString(attributeDetail.NewValue[attribute.Key]);

                    Console.WriteLine($"Attribute: {attribute.Key}, old value: {oldValue}, new value: {newValue}");

                }

                foreach (KeyValuePair<String, object> attribute in attributeDetail.OldValue.Attributes)
                {
                    if (!attributeDetail.NewValue.Contains(attribute.Key))
                    {
                        newValue = "(no value)";

                        oldValue = GetTypedValueAsString(attributeDetail.OldValue[attribute.Key]);

                        Console.WriteLine($"Attribute: {attribute.Key}, old value: {oldValue}, new value: {newValue}");
                    }
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Enable auditing on an entity.
        /// </summary>
        /// <param name="entityLogicalName">The logical name of the entity.</param>
        /// <param name="flag">True to enable auditing, otherwise false.</param>
        /// <returns>The previous value of the IsAuditEnabled attribute.</returns>
        private static bool EnableEntityAuditing(CrmServiceClient service, string entityLogicalName, bool flag)
        {
            // Retrieve the entity metadata.
            var entityRequest = new RetrieveEntityRequest
            {
                LogicalName = entityLogicalName,
                EntityFilters = EntityFilters.Entity
            };

            var entityResponse =
                (RetrieveEntityResponse)service.Execute(entityRequest);

            // Enable auditing on the entity. By default, this also enables auditing
            // on all the entity's attributes.
            EntityMetadata entityMetadata = entityResponse.EntityMetadata;

            bool oldValue = entityMetadata.IsAuditEnabled.Value;
            entityMetadata.IsAuditEnabled = new BooleanManagedProperty(flag);

            var updateEntityRequest = new UpdateEntityRequest { Entity = entityMetadata };

            var updateEntityResponse =
                (UpdateEntityResponse)service.Execute(updateEntityRequest);

            return oldValue;
        }

        /// <summary>
        /// Creates any entity records that this sample requires.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            Console.Write("Creating an account, ");

            // Account entity category codes.
            var Categories = new { PreferredCustomer = 1, Standard = 2 };

            // Create a new account entity. 
            var newAccount = new Account { Name = "Example Account" };
            _newAccountId = service.Create(newAccount);

            Console.WriteLine("then updating the account.");

            var accountToUpdate = new Account
            {
                AccountId = _newAccountId,
                AccountNumber = "1-A",
                AccountCategoryCode = new OptionSetValue(Categories.PreferredCustomer),
                Telephone1 = "555-555-5555"
            };

            service.Update(accountToUpdate);
        }
        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the records created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
            bool deleteRecords = true;

            if (prompt)
            {
                Console.WriteLine("\nDo you want to delete the account record? (y/n) [y]: ");
                string answer = Console.ReadLine();

                deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y") || answer == string.Empty);
            }

            if (deleteRecords)
            {
                service.Delete(Account.EntityLogicalName, _newAccountId);
                Console.WriteLine("The account record has been deleted.");
            }

            if (prompt)
            {
                Console.WriteLine("\nDo you want to delete ALL audit records? (y/n) [n]: ");
                string answer = Console.ReadLine();

                deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y"));
            }

            if (deleteRecords)
            {
                // Get the list of audit partitions.
                var partitionRequest =
                    (RetrieveAuditPartitionListResponse)service.Execute(new RetrieveAuditPartitionListRequest());
                AuditPartitionDetailCollection partitions = partitionRequest.AuditPartitionDetailCollection;

                // Create a delete request with an end date earlier than possible.
                var deleteRequest = new DeleteAuditDataRequest();
                deleteRequest.EndDate = new DateTime(2000, 1, 1);

                // Check if partitions are not supported as is the case with SQL Server Standard edition.
                if (partitions.IsLogicalCollection)
                {
                    // Delete all audit records created up until now.
                    deleteRequest.EndDate = DateTime.Now;
                }

                // Otherwise, delete all partitions that are older than the current partition.
                // Hint: The partitions in the collection are returned in sorted order where the 
                // partition with the oldest end date is at index 0.
                else
                {
                    for (int n = partitions.Count - 1; n >= 0; --n)
                    {
                        if (partitions[n].EndDate <= DateTime.Now && partitions[n].EndDate > deleteRequest.EndDate)
                        {
                            deleteRequest.EndDate = (DateTime)partitions[n].EndDate;
                            break;
                        }
                    }
                }

                // Delete the audit records.
                if (deleteRequest.EndDate != new DateTime(2000, 1, 1))
                {
                    service.Execute(deleteRequest);
                    Console.WriteLine("Audit records have been deleted.");
                }
                else
                    Console.WriteLine("There were no audit records that could be deleted.");
            }
        }
        /// <summary>
        /// Returns a string value for the type
        /// </summary>
        /// <param name="typedValue"></param>
        /// <returns></returns>
        public static string GetTypedValueAsString(object typedValue)
        {

            string value = string.Empty;

            switch (typedValue)
            {
                case OptionSetValue o:
                    value = o.Value.ToString();
                    break;
                case EntityReference e:
                    value = $"LogicalName:{e.LogicalName},Id:{e.Id},Name:{e.Name}";
                    break;
                default:
                    value = typedValue.ToString();
                    break;
            }

            return value;

        }
    }
}
