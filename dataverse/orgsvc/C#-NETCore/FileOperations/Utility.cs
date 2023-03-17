using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace PowerPlatform.Dataverse.CodeSamples
{
    public class Utility
    {
        /// <summary>
        /// Creates a file column.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityLogicalName">The logical name of the table to create the file column in.</param>
        /// <param name="fileColumnSchemaName">The schema name of the file column.</param>
        public static void CreateFileColumn(IOrganizationService service, string entityLogicalName, string fileColumnSchemaName) {

            Console.WriteLine($"Creating file column named '{fileColumnSchemaName}' on the {entityLogicalName} table ...");

            FileAttributeMetadata fileColumn = new()
            {
                SchemaName = fileColumnSchemaName,
                DisplayName = new Label("Sample File Column", 1033),
                RequiredLevel = new AttributeRequiredLevelManagedProperty(
                      AttributeRequiredLevel.None),
                Description = new Label("Sample File Column for FileOperation samples", 1033),
                MaxSizeInKB = 10 * 1024 // 10 MB

            };

            CreateAttributeRequest createfileColumnRequest = new() {
                EntityName = entityLogicalName,
                Attribute = fileColumn                   
            };

            service.Execute(createfileColumnRequest);

            Console.WriteLine($"Created file column named '{fileColumnSchemaName}' in the {entityLogicalName} table.");

        }

        /// <summary>
        /// Update the MaxSizeInKB for a file column
        /// </summary>
        /// <param name="service">The service</param>
        /// <param name="entityLogicalName">The logical name of the table that has the column.</param>
        /// <param name="fileColumnLogicalName">The logical name of the file column.</param>
        /// <param name="maxSizeInKB">The new value for MaxSizeInKB</param>
        public static void UpdateFileColumnMaxSizeInKB(IOrganizationService service, string entityLogicalName, string fileColumnLogicalName, int maxSizeInKB) {

            RetrieveAttributeRequest retrieveAttributeRequest = new() { 
                 EntityLogicalName = entityLogicalName,
                 LogicalName = fileColumnLogicalName
            };

            var retrieveAttributeResponse = (RetrieveAttributeResponse)service.Execute(retrieveAttributeRequest);

            var fileColumn = (FileAttributeMetadata)retrieveAttributeResponse.AttributeMetadata;

            fileColumn.MaxSizeInKB = maxSizeInKB;

            UpdateAttributeRequest updateAttributeRequest = new() { 
                 EntityName= entityLogicalName,
                 Attribute= fileColumn
            };

            service.Execute(updateAttributeRequest);

        }


        /// <summary>
        /// Retrieves the MaxSizeInKb property of a file column.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityLogicalName">The logical name of the table that has the column</param>
        /// <param name="fileColumnLogicalName">The logical name of the file column.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static int GetFileColumnMaxSizeInKb(IOrganizationService service, string entityLogicalName, string fileColumnLogicalName) {

            RetrieveAttributeRequest retrieveAttributeRequest = new() { 
                 EntityLogicalName = entityLogicalName,
                 LogicalName = fileColumnLogicalName
            };

            RetrieveAttributeResponse retrieveAttributeResponse;
            try
            {
                 retrieveAttributeResponse = (RetrieveAttributeResponse)service.Execute(retrieveAttributeRequest);
            }
            catch (Exception)
            {
                throw;
            }

            if (retrieveAttributeResponse.AttributeMetadata is FileAttributeMetadata fileColumn)
            {
                return fileColumn.MaxSizeInKB.Value;
            }
            else
            {
                throw new Exception($"{entityLogicalName}.{fileColumnLogicalName} is not a file column.");
            }
        }

        public static void DeleteFileColumn(IOrganizationService service, string entityLogicalName, string fileColumnSchemaName) {

            Console.WriteLine($"Deleting the file column named '{fileColumnSchemaName}' on the {entityLogicalName} table ...");

            DeleteAttributeRequest deletefileColumnRequest = new() { 
                 EntityLogicalName = entityLogicalName,
                 LogicalName = fileColumnSchemaName.ToLower(),
                  
            };

            service.Execute(deletefileColumnRequest);

            Console.WriteLine($"Deleted the file column named '{fileColumnSchemaName}' in the {entityLogicalName} table.");

        }


    }
}
