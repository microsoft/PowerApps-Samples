using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    public class Utility
    {

        /// <summary>
        /// Gets the mimetype for a file
        /// </summary>
        /// <param name="file">A reference to the file.</param>
        /// <returns>The mimetype</returns>
        public static string GetMimeType(FileInfo file)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (provider.TryGetContentType(file.Name, out string fileMimeType))
                return fileMimeType;
            else
                return "application/octet-stream";
        }

        /// <summary>
        /// Gets the MaxUploadFileSize in bytes
        /// </summary>
        /// <param name="service">The IOrganizationService to use.</param>
        /// <returns>The current system configured MaxUploadFileSize</returns>
        public static int GetMaxUploadFileSize(IOrganizationService service) {
            QueryExpression query = new("organization") { 
                 ColumnSet = new ColumnSet("maxuploadfilesize")
            };

            EntityCollection organizations = service.RetrieveMultiple(query);

            // There is only one row in organization table
            return (int)organizations.Entities.FirstOrDefault()["maxuploadfilesize"];
        }

        /// <summary>
        /// Set the current system configured MaxUploadFileSize
        /// </summary>
        /// <param name="service">The IOrganizationService to use.</param>
        /// <param name="maxUploadFileSizeInBytes">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The maxUploadFileSizeInBytes parameter must be less than 131072000 bytes and greater than 0 bytes.</exception>
        public static void SetMaxUploadFileSize(IOrganizationService service, int maxUploadFileSizeInBytes)
        {
            if (maxUploadFileSizeInBytes > 131072000 || maxUploadFileSizeInBytes < 1) {
                throw new ArgumentOutOfRangeException(nameof(maxUploadFileSizeInBytes), "The maxUploadFileSizeInBytes parameter must be less than 131072000 bytes and greater than 0 bytes.");
            }

            QueryExpression query = new("organization")
            {
                ColumnSet = new ColumnSet("organizationid")
            };

            EntityCollection organizations = service.RetrieveMultiple(query);

            // There is only one row in organization table
            Entity organization = organizations.Entities.FirstOrDefault();
            organization["maxuploadfilesize"] = maxUploadFileSizeInBytes;

            service.Update(organization);
        }
    }
}
