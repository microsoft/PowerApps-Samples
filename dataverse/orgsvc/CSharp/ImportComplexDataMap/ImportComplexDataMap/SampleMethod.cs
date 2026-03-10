using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        private static DateTime _executionDate;

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

            _executionDate = DateTime.Today;
            ImportRecords(service);
        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, prompt);
        }

        /// <summary>
        /// Imports records to Microsoft Dynamics CRM from the specified .csv file.
        /// </summary>
        public static void ImportRecords(CrmServiceClient service)
        {
            // Create an import map.
            var importMap = new ImportMap()
            {
                Name = "Import Map " + DateTime.Now.Ticks.ToString(),
                Source = "import acccounts.csv",
                Description = "Description of data being imported",
                EntitiesPerFile =
                    new OptionSetValue((int)ImportMapEntitiesPerFile.SingleEntityPerFile),
                EntityState = EntityState.Created
            };
            Guid importMapId = service.Create(importMap);

            // Create column mappings.

            #region Column One Mappings
            // Create a column mapping for a 'text' type field.
            var colMapping1 = new ColumnMapping()
            {
                // Set source properties.
                SourceAttributeName = "src_name",
                SourceEntityName = "Account_1",

                // Set target properties.
                TargetAttributeName = "name",
                TargetEntityName = Account.EntityLogicalName,

                // Relate this column mapping with the data map.
                ImportMapId =
                    new EntityReference(ImportMap.EntityLogicalName, importMapId),

                // Force this column to be processed.
                ProcessCode =
                    new OptionSetValue((int)ColumnMappingProcessCode.Process)
            };

            // Create the mapping.
            Guid colMappingId1 = service.Create(colMapping1);
            #endregion

            #region Column Two Mappings
            // Create a column mapping for a 'lookup' type field.
            var colMapping2 = new ColumnMapping()
            {
                // Set source properties.
                SourceAttributeName = "src_parent",
                SourceEntityName = "Account_1",

                // Set target properties.
                TargetAttributeName = "parentaccountid",
                TargetEntityName = Account.EntityLogicalName,

                // Relate this column mapping with the data map.
                ImportMapId =
                    new EntityReference(ImportMap.EntityLogicalName, importMapId),

                // Force this column to be processed.
                ProcessCode =
                    new OptionSetValue((int)ColumnMappingProcessCode.Process),
            };

            // Create the mapping.
            Guid colMappingId2 = service.Create(colMapping2);

            // Because we created a column mapping of type lookup, we need to specify lookup details in a lookupmapping.
            // One lookupmapping will be for the parent account, and the other for the current record.
            // This lookupmapping is important because without it the current record
            // cannot be used as the parent of another record.

            // Create a lookup mapping to the parent account.  
            var parentLookupMapping = new LookUpMapping()
            {
                // Relate this mapping with its parent column mapping.
                ColumnMappingId =
                    new EntityReference(ColumnMapping.EntityLogicalName, colMappingId2),

                // Force this column to be processed.
                ProcessCode =
                    new OptionSetValue((int)LookUpMappingProcessCode.Process),

                // Set the lookup for an account entity by its name attribute.
                LookUpEntityName = Account.EntityLogicalName,
                LookUpAttributeName = "name",
                LookUpSourceCode =
                    new OptionSetValue((int)LookUpMappingLookUpSourceCode.System)
            };

            // Create the lookup mapping.
            Guid parentLookupMappingId = service.Create(parentLookupMapping);

            // Create a lookup on the current record's "src_name" so that this record can
            // be used as the parent account for another record being imported.
            // Without this lookup, no record using this account as its parent will be imported.
            var currentLookUpMapping = new LookUpMapping()
            {
                // Relate this lookup with its parent column mapping.
                ColumnMappingId =
                    new EntityReference(ColumnMapping.EntityLogicalName, colMappingId2),

                // Force this column to be processed.
                ProcessCode =
                    new OptionSetValue((int)LookUpMappingProcessCode.Process),

                // Set the lookup for the current record by its src_name attribute.
                LookUpAttributeName = "src_name",
                LookUpEntityName = "Account_1",
                LookUpSourceCode =
                    new OptionSetValue((int)LookUpMappingLookUpSourceCode.Source)
            };

            // Create the lookup mapping
            Guid currentLookupMappingId = service.Create(currentLookUpMapping);
            #endregion

            #region Column Three Mappings
            // Create a column mapping for a 'picklist' type field
            var colMapping3 = new ColumnMapping()
            {
                // Set source properties
                SourceAttributeName = "src_addresstype",
                SourceEntityName = "Account_1",

                // Set target properties
                TargetAttributeName = "address1_addresstypecode",
                TargetEntityName = Account.EntityLogicalName,

                // Relate this column mapping with its parent data map
                ImportMapId =
                    new EntityReference(ImportMap.EntityLogicalName, importMapId),

                // Force this column to be processed
                ProcessCode =
                    new OptionSetValue((int)ColumnMappingProcessCode.Process)
            };

            // Create the mapping
            Guid colMappingId3 = service.Create(colMapping3);

            // Because we created a column mapping of type picklist, we need to specify picklist details in a picklistMapping
            var pickListMapping1 = new PickListMapping()
            {
                SourceValue = "bill",
                TargetValue = 1,

                // Relate this column mapping with its column mapping data map
                ColumnMappingId =
                    new EntityReference(ColumnMapping.EntityLogicalName, colMappingId3),

                // Force this column to be processed
                ProcessCode =
                    new OptionSetValue((int)PickListMappingProcessCode.Process)
            };

            // Create the mapping
            Guid picklistMappingId1 = service.Create(pickListMapping1);

            // Need a picklist mapping for every address type code expected
            var pickListMapping2 = new PickListMapping()
            {
                SourceValue = "ship",
                TargetValue = 2,

                // Relate this column mapping with its column mapping data map
                ColumnMappingId =
                    new EntityReference(ColumnMapping.EntityLogicalName, colMappingId3),

                // Force this column to be processed
                ProcessCode =
                    new OptionSetValue((int)PickListMappingProcessCode.Process)
            };

            // Create the mapping
            Guid picklistMappingId2 = service.Create(pickListMapping2);
            #endregion

            // Create Import
            var import = new Import()
            {
                // IsImport is obsolete; use ModeCode to declare Create or Update.
                ModeCode = new OptionSetValue((int)ImportModeCode.Create),
                Name = "Importing data"
            };
            Guid importId = service.Create(import);

            // Create Import File.
            var importFile = new ImportFile()
            {
                Content = BulkImportHelper.ReadCsvFile("import accounts.csv"), // Read contents from disk.
                Name = "Account record import",
                IsFirstRowHeader = true,
                ImportMapId = new EntityReference(ImportMap.EntityLogicalName, importMapId),
                UseSystemMap = false,
                Source = "import accounts.csv",
                SourceEntityName = "Account_1",
                TargetEntityName = Account.EntityLogicalName,
                ImportId = new EntityReference(Import.EntityLogicalName, importId),
                EnableDuplicateDetection = false,
                FieldDelimiterCode =
                    new OptionSetValue((int)ImportFileFieldDelimiterCode.Comma),
                DataDelimiterCode =
                    new OptionSetValue((int)ImportFileDataDelimiterCode.DoubleQuote),
                ProcessCode =
                    new OptionSetValue((int)ImportFileProcessCode.Process)
            };

            // Get the current user to set as record owner.
            var systemUserRequest = new WhoAmIRequest();
            var systemUserResponse =
                (WhoAmIResponse)service.Execute(systemUserRequest);

            // Set the owner ID.				
            importFile.RecordsOwnerId =
                new EntityReference(SystemUser.EntityLogicalName, systemUserResponse.UserId);

            Guid importFileId = service.Create(importFile);

            // Retrieve the header columns used in the import file.
            GetHeaderColumnsImportFileRequest headerColumnsRequest = new GetHeaderColumnsImportFileRequest()
            {
                ImportFileId = importFileId
            };
            GetHeaderColumnsImportFileResponse headerColumnsResponse =
                (GetHeaderColumnsImportFileResponse)service.Execute(headerColumnsRequest);

            // Output the header columns.
            int columnNum = 1;
            foreach (string headerName in headerColumnsResponse.Columns)
            {
                Console.WriteLine("Column[" + columnNum.ToString() + "] = " + headerName);
                columnNum++;
            }

            // Parse the import file.
            var parseImportRequest = new ParseImportRequest()
            {
                ImportId = importId
            };
            ParseImportResponse parseImportResponse =
                (ParseImportResponse)service.Execute(parseImportRequest);
            Console.WriteLine("Waiting for Parse async job to complete");
            BulkImportHelper.WaitForAsyncJobCompletion(service, parseImportResponse.AsyncOperationId);
            BulkImportHelper.ReportErrors(service, importFileId);

            // Retrieve the first two distinct values for column 1 from the parse table.
            // NOTE: You must create the parse table first using the ParseImport message.
            // The parse table is not accessible after ImportRecordsImportResponse is called.
            var distinctValuesRequest = new GetDistinctValuesImportFileRequest()
            {
                columnNumber = 1,
                ImportFileId = importFileId,
                pageNumber = 1,
                recordsPerPage = 2,
            };
            GetDistinctValuesImportFileResponse distinctValuesResponse =
                (GetDistinctValuesImportFileResponse)service.Execute(distinctValuesRequest);

            // Output the distinct values.  In this case: (column 1, row 1) and (column 1, row 2).
            int cellNum = 1;
            foreach (string cellValue in distinctValuesResponse.Values)
            {
                Console.WriteLine("(1, " + cellNum.ToString() + "): " + cellValue);
                Console.WriteLine(cellValue);
                cellNum++;
            }

            // Retrieve data from the parse table.
            // NOTE: You must create the parse table first using the ParseImport message.
            // The parse table is not accessible after ImportRecordsImportResponse is called.
            var parsedDataRequest = new RetrieveParsedDataImportFileRequest()
            {
                ImportFileId = importFileId,
                PagingInfo = new PagingInfo()
                {
                    // Specify the number of entity instances returned per page.
                    Count = 2,
                    // Specify the number of pages returned from the query.
                    PageNumber = 1,
                    // Specify a total number of entity instances returned.
                    PagingCookie = "1"
                }
            };

            RetrieveParsedDataImportFileResponse parsedDataResponse =
                (RetrieveParsedDataImportFileResponse)service.Execute(parsedDataRequest);

            // Output the first two rows retrieved.
            int rowCount = 1;
            foreach (string[] rows in parsedDataResponse.Values)
            {
                int colCount = 1;
                foreach (string column in rows)
                {
                    Console.WriteLine("(" + rowCount.ToString() + "," + colCount.ToString() + ") = " + column);
                    colCount++;
                }
                rowCount++;
            }

            // Transform the import
            var transformImportRequest = new TransformImportRequest()
            {
                ImportId = importId
            };
            TransformImportResponse transformImportResponse =
                (TransformImportResponse)service.Execute(transformImportRequest);
            Console.WriteLine("Waiting for Transform async job to complete");
            BulkImportHelper.WaitForAsyncJobCompletion(service, transformImportResponse.AsyncOperationId);
            BulkImportHelper.ReportErrors(service, importFileId);

            // Upload the records.
            var importRequest = new ImportRecordsImportRequest()
            {
                ImportId = importId
            };
            ImportRecordsImportResponse importResponse =
                (ImportRecordsImportResponse)service.Execute(importRequest);
            Console.WriteLine("Waiting for ImportRecords async job to complete");
            BulkImportHelper.WaitForAsyncJobCompletion(service, importResponse.AsyncOperationId);
            BulkImportHelper.ReportErrors(service, importFileId);
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
                // Retrieve all account records created in this sample.
                QueryExpression query = new QueryExpression()
                {
                    EntityName = Account.EntityLogicalName,
                    Criteria = new FilterExpression()
                    {
                        Conditions =
                        {
                            new ConditionExpression("createdon", ConditionOperator.OnOrAfter, _executionDate),
                        }
                    },
                    ColumnSet = new ColumnSet(false)
                };
                var accountsCreated = service.RetrieveMultiple(query).Entities;

                // Delete all records created in this sample.
                foreach (var account in accountsCreated)
                {
                    service.Delete(Account.EntityLogicalName, account.Id);
                }

                Console.WriteLine("Entity record(s) have been deleted.");
            }
        }
    }
}
