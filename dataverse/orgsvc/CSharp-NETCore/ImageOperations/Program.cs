using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates working with data in image columns.
    /// </summary>
    /// <remarks>Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// You will be prompted in the default browser to enter a password.</remarks>
    /// <see cref="https://learn.microsoft.com/power-apps/developer/data-platform/xrm-tooling/use-connection-strings-xrm-tooling-connect#connection-string-parameters"/>
    /// <permission cref="https://github.com/microsoft/PowerApps-Samples/blob/master/LICENSE"
    /// <author>Jim Daly</author>
    class Program
    {
        /// <summary>
        /// Contains the application's configuration settings. 
        /// </summary>
        IConfiguration Configuration { get; }

        /// <summary>
        /// Constructor. Loads the application configuration settings from a JSON file.
        /// </summary>
        Program()
        {

            // Get the path to the appsettings file. If the environment variable is set,
            // use that file path. Otherwise, use the runtime folder's settings file.
            string? path = Environment.GetEnvironmentVariable("DATAVERSE_APPSETTINGS");
            if (path == null) path = "appsettings.json";

            // Load the app's configuration settings from the JSON file.
            Configuration = new ConfigurationBuilder()
                .AddJsonFile(path, optional: false, reloadOnChange: true)
                .Build();
        }


        static void Main()
        {
            Program app = new();

            // Create a Dataverse service client using the default connection string.
            ServiceClient service =
                new(app.Configuration.GetConnectionString("default"));

            string entityLogicalName = "account";
            string imageColumnSchemaName = "sample_ImageColumn";
            string imageColumnLogicalName = imageColumnSchemaName.ToLower();
            // Capture this so it can be set back at the end of the sample.
            string originalAccountPrimaryImageAttributeName = Utility.GetTablePrimaryImageName(service, entityLogicalName);
            List<Guid> accountsWithImagesIds = new();
            // The names of the image files in the Images folder
            List<string> fileNames = new() { "144x144.png", "144x400.png", "400x144.png", "400x500.png", "60x80.png" };


            // Create the Image Column with CanStoreFullImage = false.
            Utility.CreateImageColumn(service, entityLogicalName, imageColumnSchemaName);

            // Update the image column to set it as the primary image
            // Only primary image columns can be set during Create
            Utility.SetTablePrimaryImageName(service, entityLogicalName, imageColumnLogicalName);

            Console.WriteLine("Create 5 records while CanStoreFullImage is false.");

            // Create account records with each size image
            foreach (string fileName in fileNames)
            {
                string name = $"CanStoreFullImage false {fileName}";

                Entity account = new(entityLogicalName);
                account["name"] = name;
                account[imageColumnLogicalName] = File.ReadAllBytes($"Images\\{fileName}");
                accountsWithImagesIds.Add(service.Create(account));
                Console.WriteLine($"\tCreated account: '{name}'");
            }


            // Changing the CanStoreFullImage behavior
            Utility.UpdateCanStoreFullImage(
                service,
                entityLogicalName,
                imageColumnSchemaName,
                canStoreFullImage: true);

            Console.WriteLine("Create 5 records while CanStoreFullImage is true.");

            // Create account records with each size image
            foreach (string fileName in fileNames)
            {
                string name = $"CanStoreFullImage true {fileName}";

                Entity account = new(entityLogicalName);
                account["name"] = name;
                account[imageColumnLogicalName] = File.ReadAllBytes($"Images\\{fileName}");

                accountsWithImagesIds.Add(service.Create(account));
                Console.WriteLine($"\tCreated account: '{name}'");
            }

            //Retrieve the accounts just created
            QueryExpression query = new("account")
            {
                ColumnSet = new ColumnSet("name", imageColumnLogicalName, $"{imageColumnLogicalName}_url"),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions = {
                        new ConditionExpression(
                            attributeName: "accountid",
                            conditionOperator: ConditionOperator.In,
                            values: accountsWithImagesIds.ToArray())
                    }
                }
            };

            EntityCollection accountsWithImages = service.RetrieveMultiple(query);

            Console.WriteLine($"Retrieving records with thumbnail images:");
            // Images retrieved this way are always thumbnails
            foreach (Entity account in accountsWithImages.Entities)
            {
                string recordName = (string)account["name"];
                string downloadedFileName = $"{recordName}_retrieved.png";
                File.WriteAllBytes($"DownloadedImages\\{downloadedFileName}", (byte[])account[imageColumnLogicalName]);
                Console.WriteLine($"\tThumbnail-sized file column data saved to DownloadedImages\\{downloadedFileName}");
            }

            Console.WriteLine("Attempt to download full-size images for all 10 records. 5 should fail.");
            // Attempt to download the full image of the files.
            // Expect that 5 of 10 will fail because they were created while CanStoreFullImage was false.
            foreach (Entity account in accountsWithImages.Entities)
            {
                try
                {
                    byte[] downloadedFile = DownloadFile(service, account.ToEntityReference(), imageColumnLogicalName);

                    string recordName = (string)account["name"];
                    string downloadedFileName = $"{recordName}_downloaded.png";
                    File.WriteAllBytes($"DownloadedImages\\{downloadedFileName}", downloadedFile);
                    Console.WriteLine($"\tFull-sized file downloaded to DownloadedImages\\{downloadedFileName}");
                }
                catch (FaultException<OrganizationServiceFault> faultException)
                {
                    int errorCode = faultException.Detail.ErrorCode;
                    if (errorCode == -2147220969)
                    {
                        // ObjectDoesNotExist error
                        // No FileAttachment records found for imagedescriptorId: <guid> for image attribute: sample_imagecolumn of account record with id <guid>
                        // These 5 images were created while CanStoreFullImage was false
                        Console.WriteLine($"\tDownload failed: {faultException.Message}");
                    }
                    else
                    {
                        throw faultException;
                    }
                }
                catch (Exception)
                {

                    throw;
                }

            }

            // Delete Image data with Update.
            foreach (Entity account in accountsWithImages.Entities)
            {
                Entity deleteImageAccount = new("account");
                deleteImageAccount.Id= account.Id;
                deleteImageAccount[imageColumnLogicalName] = null;

                service.Update(deleteImageAccount);
            }

            // Verify that the images are deleted:
            // Retrieve the accounts again using the same query as before:
            EntityCollection accountsWithOutImages = service.RetrieveMultiple(query);

            foreach (Entity account in accountsWithOutImages.Entities)
            {
                if (account.Attributes.Contains(imageColumnLogicalName))
                {
                    // This should not occur
                    Console.WriteLine($"Error: {account["accountid"]} {imageColumnLogicalName} has an image value.");
                }
            }


            // Delete the records that were created by this sample
            foreach (Guid id in accountsWithImagesIds)
            {
                service.Delete("account", id);
            }
            Console.WriteLine("Deleted the records created for this sample.");


            // Set the account primaryImage back to the original value
            Utility.SetTablePrimaryImageName(
                service, 
                entityLogicalName, 
                originalAccountPrimaryImageAttributeName);

            // Delete the Image Column
            Utility.DeleteImageColumn(
                service, 
                entityLogicalName, 
                imageColumnSchemaName);

            Console.WriteLine("Sample completed.");
        }



        /// <summary>
        /// Downloads a full-sized file using InitializeFileBlocksDownload and DownloadBlock messages
        /// </summary>
        /// <param name="service">The service</param>
        /// <param name="entityReference">A reference to the record with the file column</param>
        /// <param name="fileAttributeName">The name of the file column</param>
        /// <returns></returns>
        private static byte[] DownloadFile(
                    IOrganizationService service,
                    EntityReference entityReference,
                    string fileAttributeName)
        {
            InitializeFileBlocksDownloadRequest initializeFileBlocksDownloadRequest = new()
            {
                Target = entityReference,
                FileAttributeName = fileAttributeName
            };

            var initializeFileBlocksDownloadResponse =
                (InitializeFileBlocksDownloadResponse)service.Execute(initializeFileBlocksDownloadRequest);

            string fileContinuationToken = initializeFileBlocksDownloadResponse.FileContinuationToken;
            long fileSizeInBytes = initializeFileBlocksDownloadResponse.FileSizeInBytes;

            List<byte> fileBytes = new((int)fileSizeInBytes);

            long offset = 0;
            long blockSizeDownload = 4 * 1024 * 1024; // 4 MB

            // File size may be smaller than defined block size
            if (fileSizeInBytes < blockSizeDownload)
            {
                blockSizeDownload = fileSizeInBytes;
            }

            while (fileSizeInBytes > 0)
            {
                // Prepare the request
                DownloadBlockRequest downLoadBlockRequest = new()
                {
                    BlockLength = blockSizeDownload,
                    FileContinuationToken = fileContinuationToken,
                    Offset = offset
                };

                // Send the request
                var downloadBlockResponse =
                           (DownloadBlockResponse)service.Execute(downLoadBlockRequest);

                // Add the block returned to the list
                fileBytes.AddRange(downloadBlockResponse.Data);

                // Subtract the amount downloaded,
                // which may make fileSizeInBytes < 0 and indicate
                // no further blocks to download
                fileSizeInBytes -= (int)blockSizeDownload;
                // Increment the offset to start at the beginning of the next block.
                offset += blockSizeDownload;
            }

            return fileBytes.ToArray();
        }


    }
}