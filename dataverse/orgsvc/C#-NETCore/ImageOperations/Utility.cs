using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace PowerPlatform.Dataverse.CodeSamples
{
    public class Utility
    {

        /// <summary>
        /// Creates an image column.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityLogicalName">The logical name of the table to create the image column in.</param>
        /// <param name="imageColumnSchemaName">The schema name of the image column.</param>
        public static void CreateImageColumn(IOrganizationService service, string entityLogicalName, string imageColumnSchemaName)
        {

            Console.WriteLine($"Creating image column named '{imageColumnSchemaName}' on the {entityLogicalName} table ...");

            ImageAttributeMetadata imageColumn = new()
            {
                SchemaName = imageColumnSchemaName,
                DisplayName = new Label("Sample Image Column", 1033),
                RequiredLevel = new AttributeRequiredLevelManagedProperty(
                      AttributeRequiredLevel.None),
                Description = new Label("Sample Image Column for ImageOperation samples", 1033),
                MaxSizeInKB = 10 * 1024 // 10 MB

            };

            CreateAttributeRequest createfileColumnRequest = new()
            {
                EntityName = entityLogicalName,
                Attribute = imageColumn
            };

            service.Execute(createfileColumnRequest);

            Console.WriteLine($"Created image column named '{imageColumnSchemaName}' in the {entityLogicalName} table.");

        }


        /// <summary>
        /// Deletes an image column
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityLogicalName">The logical name of the table the image column exists in.</param>
        /// <param name="imageColumnSchemaName">The schema name of the image column.</param>
        public static void DeleteImageColumn(IOrganizationService service, string entityLogicalName, string imageColumnSchemaName)
        {

            Console.WriteLine($"Deleting the image column named '{imageColumnSchemaName}' on the {entityLogicalName} table ...");

            DeleteAttributeRequest deleteImageColumnRequest = new()
            {
                EntityLogicalName = entityLogicalName,
                LogicalName = imageColumnSchemaName.ToLower(),

            };

            service.Execute(deleteImageColumnRequest);

            Console.WriteLine($"Deleted the image column named '{imageColumnSchemaName}' in the {entityLogicalName} table.");

        }

    }
}
