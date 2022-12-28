using PowerApps.Samples;
using PowerApps.Samples.Metadata.Messages;
using PowerApps.Samples.Metadata.Types;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOperations
{
    internal class Utility
    {
        /// <summary>
        /// Creates an image column that is the primary image column.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityLogicalName">The logical name of the table to create the image column in.</param>
        /// <param name="imageColumnSchemaName">The schema name of the image column.</param>
        public static async Task CreateImageColumn(Service service, string entityLogicalName, string imageColumnSchemaName) {

            Console.WriteLine($"Creating image column named '{imageColumnSchemaName}' on the {entityLogicalName} table ...");

            ImageAttributeMetadata imageColumn = new()
            {
                SchemaName = imageColumnSchemaName,
                DisplayName = new Label("Sample Image Column", 1033),
                RequiredLevel = new AttributeRequiredLevelManagedProperty(
              AttributeRequiredLevel.None),
                Description = new Label("Sample Image Column for ImageOperation samples", 1033),
                //IsPrimaryImage = true, // Overriding current primary image column?? Doesn't seem to work on Create, at least when another primary image attribute exists.
                MaxSizeInKB = 30 * 1024 // 30 MB is maximum size.

            };

            CreateAttributeRequest createfileColumnRequest = new(entityLogicalName: entityLogicalName, attributeMetadata: imageColumn);

            try
            {
                await service.SendAsync(createfileColumnRequest);
                Console.WriteLine($"Created image column named '{imageColumnSchemaName}' in the {entityLogicalName} table.");
            }
            catch (Exception ex)
            {
                if(ex is ServiceException serviceException)
                {
                    string errorCode = serviceException.ODataError.Error.Code;
                    if (errorCode == "0x80047013")
                    {
                        // DuplicateAttributeSchemaName error
                        Console.WriteLine($"Column named '{imageColumnSchemaName}' already exists in the {entityLogicalName} table.");
                    }
                    else
                    {
                        throw ex;
                    }
                }
                else
                {
                    throw ex;
                }
            }

        }

        /// <summary>
        /// Deletes an image column
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityLogicalName">The logical name of the table the image column exists in.</param>
        /// <param name="imageColumnSchemaName">The schema name of the image column.</param>
        public static async Task DeleteImageColumn(Service service, string entityLogicalName, string imageColumnSchemaName) {

            Console.WriteLine($"Deleting the image column named '{imageColumnSchemaName}' on the {entityLogicalName} table ...");

            DeleteAttributeRequest deleteImageColumnRequest = new(entityLogicalName: entityLogicalName, logicalName: imageColumnSchemaName.ToLower());

            await service.SendAsync(deleteImageColumnRequest);

            Console.WriteLine($"Deleted the image column named '{imageColumnSchemaName}' in the {entityLogicalName} table.");
        }


        /// <summary>
        /// Update the CanStoreFullImage for a image column
        /// </summary>
        /// <param name="service">The service</param>
        /// <param name="entityLogicalName">The logical name of the table that has the column.</param>
        /// <param name="imageColumnSchemaName">The logical name of the image column.</param>
        /// <param name="canStoreFullImage">The new value for CanStoreFullImage</param>
        public static async Task UpdateCanStoreFullImage(Service service, string entityLogicalName, string imageColumnSchemaName, bool canStoreFullImage) {

            RetrieveAttributeRequest retrieveAttributeRequest = new(
                entityLogicalName: entityLogicalName,
                logicalName: imageColumnSchemaName.ToLower(),
                type: AttributeType.ImageAttributeMetadata);

            var retrieveAttributeResponse = await service.SendAsync<RetrieveAttributeResponse<ImageAttributeMetadata>>(retrieveAttributeRequest);

            var imageColumn = retrieveAttributeResponse.AttributeMetadata;

            imageColumn.CanStoreFullImage = canStoreFullImage;

            UpdateAttributeRequest updateAttributeRequest = new(
                entityLogicalName: entityLogicalName, 
                attributeLogicalName: imageColumnSchemaName.ToLower(), 
                attributeMetadata: imageColumn);

            await service.SendAsync(updateAttributeRequest);

            Console.WriteLine($"Set the CanStoreFullImage property to {canStoreFullImage}");
        }

        /// <summary>
        /// Gets the name of the primary image column for the table
        /// </summary>
        /// <param name="service">The service</param>
        /// <param name="entityLogicalName">The logical name of the table that has the column.</param>
        /// <returns>The EntityMetadata.PrimaryImageAttribute value.</returns>
        public static async Task<string> GetTablePrimaryImageName(Service service, string entityLogicalName) {

            RetrieveEntityDefinitionRequest request = new(
                logicalName: entityLogicalName, 
                query: "?$select=PrimaryImageAttribute");

            RetrieveEntityDefinitionResponse response = await service.SendAsync<RetrieveEntityDefinitionResponse>(request);

            return response.EntityMetadata.PrimaryImageAttribute;

        }

        /// <summary>
        /// Sets an ImageAttribute IsPrimaryImage property
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityLogicalName">The logical name of the table that has the image column.</param>
        /// <param name="imageAttributeName">The logical name of the image column.</param>
        /// <param name="isPrimaryImage">The value to set.</param>
        /// <returns></returns>
        public static async Task SetTablePrimaryImageName(Service service, string entityLogicalName, string imageAttributeName, bool isPrimaryImage) {

            RetrieveAttributeRequest retrieveRequest = new(
                entityLogicalName: entityLogicalName, 
                logicalName: imageAttributeName,
                type: AttributeType.ImageAttributeMetadata);

            var retrieveResponse = await service.SendAsync<RetrieveAttributeResponse<ImageAttributeMetadata>>(retrieveRequest);

            ImageAttributeMetadata imageColumnDefinition = retrieveResponse.AttributeMetadata;

            imageColumnDefinition.IsPrimaryImage = isPrimaryImage;

            UpdateAttributeRequest updateAttributeRequest = new UpdateAttributeRequest(
                entityLogicalName: entityLogicalName, 
                attributeLogicalName: imageAttributeName, 
                attributeMetadata: imageColumnDefinition);

            await service.SendAsync(updateAttributeRequest);
        }
    }
}
