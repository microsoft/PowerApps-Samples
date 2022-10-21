using PowerApps.Samples.Metadata.Messages;
using PowerApps.Samples.Metadata.Types;

namespace PowerApps.Samples
{
    /// <summary>
    /// Contains metadata operation functions for the FileOperations samples.
    /// </summary>
    public class Utility
    {
        static public readonly string fileColumnSchemaName = "sample_FileColumn";

        /// <summary>
        /// Creates a custom file column on the account table named 'sample_FileColumn'.
        /// </summary>
        /// <param name="service">The service to use.</param>
        /// <returns></returns>
        public static async Task CreateFileColumn(Service service)
        {
            Console.WriteLine($"Creating file column named '{fileColumnSchemaName}' ...");

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
                entityLogicalName: "account",
                attributeMetadata: fileColumn
                );

            var createfileColumnResponse =
                await service.SendAsync<CreateAttributeResponse>(createfileColumnRequest);

            Console.WriteLine($"Created file column named '{fileColumnSchemaName}' in the account table with id:{createfileColumnResponse.AttributeId}");
        }

        /// <summary>
        /// Deletes a custom file column on the account table named 'sample_FileColumn'.
        /// </summary>
        /// <param name="service">The service to use</param>
        /// <returns></returns>
        public static async Task DeleteFileColumn(Service service) {

            DeleteAttributeRequest deletefileColumnRequest = new(
                entityLogicalName: "account", 
                logicalName: fileColumnSchemaName.ToLower(), 
                strongConsistency: true);

           await service.SendAsync(deletefileColumnRequest);

            Console.WriteLine($"Deleted the file column named '{fileColumnSchemaName}' in the account table.");
        }
    }
}

