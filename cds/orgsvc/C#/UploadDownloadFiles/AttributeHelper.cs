using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;

namespace UploadDownloadFiles
{
    public static class AttributeHelper
    {
        public static void CreateImageAttribute(CrmServiceClient service,
            string solutionUniqueName,
            string publisherUniqueName,
            string entityLogicalName,
            string columnLogicalName,
            AttributeRequiredLevel requiredLevel)
        {
            //Create an Image attribute for the custom entity
            CreateAttributeRequest createImageAttributeRequest = new CreateAttributeRequest
            {
                EntityName = entityLogicalName, // account
                Attribute = new ImageAttributeMetadata
                {
                    SchemaName = $"{publisherUniqueName}_{columnLogicalName}", //The name is always EntityImage
                                                                               //Required level must be AttributeRequiredLevel.None
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(requiredLevel),
                    DisplayName = new Label(columnLogicalName, 1033),
                    Description = new Label("File attribute.", 1033),
                    CanStoreFullImage = true
                },
                SolutionUniqueName = solutionUniqueName,
            };

            service.Execute(createImageAttributeRequest);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="solutionUniqueName"></param>
        /// <param name="customizationPrefix">Prefix of the selected publisher</param>
        /// <param name="entityLogicalName"></param>
        /// <param name="columnLogicalName"></param>
        /// <param name="requiredLevel"></param>
        public static void CreateFileAttribute(CrmServiceClient service,
            string solutionUniqueName,
            string customizationPrefix,
            string entityLogicalName,
            string columnLogicalName,
            AttributeRequiredLevel requiredLevel)
        {
            //Create an Image attribute for the custom entity
            CreateAttributeRequest createFileAttributeRequest = new CreateAttributeRequest
            {
                EntityName = entityLogicalName, // account
                Attribute = new FileAttributeMetadata
                {
                    SchemaName = $"{customizationPrefix}_{columnLogicalName}", //The name is always EntityImage
                                                                               //Required level must be AttributeRequiredLevel.None
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(requiredLevel),
                    DisplayName = new Label(columnLogicalName, 1033),
                    Description = new Label("A file attribute.", 1033),
                },
                SolutionUniqueName = solutionUniqueName,
            };

            service.Execute(createFileAttributeRequest);      
        }
    }
}
