using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace PowerPlatform.Dataverse.CodeSamples
{
    public class Utility
    {
        public static void CreateFileColumn(IOrganizationService service, string entityLogicalName, string fileColumnSchemaName) {

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

            CreateAttributeRequest createfileColumnRequest = new() {
                EntityName = entityLogicalName,
                Attribute = fileColumn                   
            };

            service.Execute(createfileColumnRequest);

            Console.WriteLine($"Created file column named '{fileColumnSchemaName}' in the {entityLogicalName} table.");

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
