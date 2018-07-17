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
        private static Guid _accountRecordId;
        private static Guid _secretPhoneId;
        private static Guid _secretHomeId;
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
        /// Creates any entity records that this sample requires.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            #region Create custom fields in account entity

            // Create secure custom field #1
            CreateAttributeRequest attrReq = new CreateAttributeRequest()
            {
                Attribute = new StringAttributeMetadata()
                {
                    LogicalName = "secret_home",
                    DisplayName = new Microsoft.Xrm.Sdk.Label("SecretHome", 1033),
                    SchemaName = "Secret_Home",
                    MaxLength = 500,
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(
                        AttributeRequiredLevel.Recommended),
                    IsSecured = true
                },
                EntityName = Account.EntityLogicalName
            };
            CreateAttributeResponse attributeResponse =
                (CreateAttributeResponse)service.Execute(attrReq);
            _secretHomeId = attributeResponse.AttributeId;
            Console.WriteLine("Secret_Home custom field created.");

            // Create secure custom field #2
            attrReq = new CreateAttributeRequest()
            {
                Attribute = new StringAttributeMetadata()
                {
                    LogicalName = "secret_phone",
                    DisplayName = new Microsoft.Xrm.Sdk.Label("SecretPhone", 1033),
                    SchemaName = "Secret_Phone",
                    MaxLength = 500,
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(
                        AttributeRequiredLevel.Recommended),
                    IsSecured = true
                },
                EntityName = Account.EntityLogicalName
            };
            attributeResponse = (CreateAttributeResponse)service.Execute(attrReq);
            _secretPhoneId = attributeResponse.AttributeId;
            Console.WriteLine("Secret_Phone custom field created.");

            #endregion Create custom fields in account entity

            Console.WriteLine();
        }

        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the records created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
            bool toBeDeleted = true;

            if (prompt)
            {
                // Ask the user if the created entities should be deleted.
                Console.Write("\nDo you want these entity records deleted? (y/n) [y]: ");
                String answer = Console.ReadLine();
                if (answer.StartsWith("y") ||
                    answer.StartsWith("Y") ||
                    answer == String.Empty)
                {
                    toBeDeleted = true;
                }
                else
                {
                    toBeDeleted = false;
                }
            }

            if (toBeDeleted)
            {
                // Delete all records created in this sample.
                // Delete the secured custom field #1.
                DeleteAttributeRequest deleteRequest = new DeleteAttributeRequest()
                {
                    EntityLogicalName = Account.EntityLogicalName,
                    LogicalName = "secret_phone",
                    RequestId = _secretPhoneId
                };
                service.Execute(deleteRequest);
                // Delete the secured custom field #2.
                deleteRequest = new DeleteAttributeRequest()
                {
                    EntityLogicalName = Account.EntityLogicalName,
                    LogicalName = "secret_home",
                    RequestId = _secretHomeId
                };
                service.Execute(deleteRequest);

                // Delete the account record.
                service.Delete(Account.EntityLogicalName, _accountRecordId);

                // We don't need to delete POAA records, because
                // they were deleted when we deleted the account record.

                Console.WriteLine("Entity record(s) have been deleted.");
            }
        }
    }
}
