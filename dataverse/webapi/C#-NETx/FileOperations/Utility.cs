using PowerApps.Samples.Metadata.Messages;
using PowerApps.Samples.Metadata.Types;

namespace PowerApps.Samples
{
    /// <summary>
    /// Contains metadata operation functions for the FileOperations samples.
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Creates a custom file column on the designated table named 'sample_FileColumn'.
        /// </summary>
        /// <param name="service">The service to use.</param>
        /// <returns></returns>
        public static async Task CreateFileColumn(Service service, string entityLogicalName, string fileColumnSchemaName)
        {
            Console.WriteLine($"Creating file column named '{fileColumnSchemaName}' on the {entityLogicalName} table ...");

            FileAttributeMetadata fileColumn = new()
            {
                SchemaName = fileColumnSchemaName,
                DisplayName = new Label("Sample File Column", 1033),
                RequiredLevel = new AttributeRequiredLevelManagedProperty(
                      AttributeRequiredLevel.None),
                Description = new Label("Sample File Column for FileOperation samples", 1033),
                MaxSizeInKB = 30 * 1024 // 30 MB

            };

            CreateAttributeRequest createfileColumnRequest = new(
                entityLogicalName: entityLogicalName,
                attributeMetadata: fileColumn
                );

            await service.SendAsync<CreateAttributeResponse>(createfileColumnRequest);

            Console.WriteLine($"Created file column named '{fileColumnSchemaName}' in the {entityLogicalName} table.");
        }

        /// <summary>
        /// Deletes a custom file column on the table 
        /// </summary>
        /// <param name="service">The service to use</param>
        /// <returns></returns>
        public static async Task DeleteFileColumn(Service service, string entityLogicalName, string fileColumnSchemaName)
        {

            Console.WriteLine($"Deleting the file column named '{fileColumnSchemaName}' on the {entityLogicalName} table ...");

            DeleteAttributeRequest deletefileColumnRequest = new(
                entityLogicalName: entityLogicalName,
                logicalName: fileColumnSchemaName.ToLower(),
                strongConsistency: true);

            await service.SendAsync(deletefileColumnRequest);

            Console.WriteLine($"Deleted the file column named '{fileColumnSchemaName}' in the {entityLogicalName} table.");
        }
    }
}

