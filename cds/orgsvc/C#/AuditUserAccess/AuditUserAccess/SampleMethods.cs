using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        private static Guid _newAccountId;
        private static Guid _systemUserId;
        private static bool prompt = true;
        /// <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
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
        /// Creates any entity records that this sample requires.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // Create a new account entity. 
            var newAccount = new Account { Name = "Example Account" };
            _newAccountId = service.Create(newAccount);
        }

        private static bool EnableEntityAuditing(CrmServiceClient service, String entityLogicalName, bool flag)
        {
            // Retrieve the entity metadata.
            var entityRequest = new RetrieveEntityRequest
            {
                LogicalName = entityLogicalName,
                EntityFilters = EntityFilters.Attributes
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
                String answer = Console.ReadLine();

                deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y") || answer == String.Empty);
            }

            if (deleteRecords)
            {
                service.Delete(Account.EntityLogicalName, _newAccountId);
                Console.WriteLine("The account record has been deleted.");
            }

            if (prompt)
            {
                Console.WriteLine("\nDo you want to delete ALL audit records? (y/n) [n]: ");
                String answer = Console.ReadLine();

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
                // partition with the oldest end date is at index 0.  Also, if the partition's
                // end date is greater than the current date, neither the partition nor any
                // audit records contained in the partition can be deleted.
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
    }
}

