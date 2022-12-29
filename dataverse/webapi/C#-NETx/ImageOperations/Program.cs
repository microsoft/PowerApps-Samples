using Newtonsoft.Json.Linq;
using PowerApps.Samples;
using PowerApps.Samples.Messages;
using PowerApps.Samples.Methods;

namespace ImageOperations
{
    internal class Program
    {
        static async Task Main()
        {
            Config config = App.InitializeApp();

            var service = new Service(config);

            string entityLogicalName = "account";
            string entitySetName = "accounts";
            string imageColumnSchemaName = "sample_ImageColumn";
            string imageColumnLogicalName = imageColumnSchemaName.ToLower();
            string originalAccountPrimaryImageAttributeName = await Utility.GetTablePrimaryImageName(service, entityLogicalName);
            List<Guid> accountsWithImagesIds = new();
            List<string> fileNames = new() { "144x144.png", "144x400.png", "400x144.png", "400x500.png", "60x80.png" };

            // Create the Image Column
            await Utility.CreateImageColumn(service, entityLogicalName, imageColumnSchemaName);

            // Update the image column to set it as the primary image
            // Only primary image columns can be set during Create
            await Utility.SetTablePrimaryImageName(service, entityLogicalName, imageColumnLogicalName, isPrimaryImage: true);


            Console.WriteLine("Create 5 records while CanStoreFullImage is false.");

            // Create account records with each size image
            foreach (string fileName in fileNames)
            {
                string name = $"CanStoreFullImage false {fileName}";
                JObject account = new()
                {
                    { "name", name},
                    { imageColumnLogicalName, File.ReadAllBytes($"Images\\{fileName}") }
                };

                EntityReference accountReference = await service.Create(entitySetName: "accounts", account);

                accountsWithImagesIds.Add(accountReference.Id.Value);
                Console.WriteLine($"\tCreated account: '{name}'");
            }

            // Changing the CanStoreFullImage behavior
            await Utility.UpdateCanStoreFullImage(service, entityLogicalName, imageColumnSchemaName, canStoreFullImage: true);

            Console.WriteLine("Create 5 records while CanStoreFullImage is true.");

            // Create account records with each size image
            foreach (string fileName in fileNames)
            {
                string name = $"CanStoreFullImage true {fileName}";

                JObject account = new()
                { 
                    { "name", name},
                    { imageColumnLogicalName, File.ReadAllBytes($"Images\\{fileName}") }
                };

                EntityReference accountReference = await service.Create(entitySetName: "accounts", account);

                accountsWithImagesIds.Add(accountReference.Id.Value);
                Console.WriteLine($"\tCreated account: '{name}'");

            }

           
            //Retrieve the accounts just created
            string query = $"accounts?" +
                $"$select=name,{imageColumnLogicalName},{imageColumnLogicalName}_url&" +
                $"$filter=Microsoft.Dynamics.CRM.In(PropertyName=@p1,PropertyValues=@p2)&" +
                $"@p1='accountid'&" +
                $"@p2=['{string.Join("','", accountsWithImagesIds)}']";

            RetrieveMultipleResponse accountsWithImagesResponse = await service.RetrieveMultiple(queryUri: query);

            // Images retrieved this way are always thumbnails
            foreach (JObject account in accountsWithImagesResponse.Records.Cast<JObject>())
            {
                string recordName = (string)account["name"];
                string downloadedFileName = $"{recordName}_retrieved.png";
                File.WriteAllBytes($"DownloadedImages\\{downloadedFileName}", (byte[])account[imageColumnLogicalName]);
            }

            Console.WriteLine("Attempt to download full-size images for all 10 records.");
            // Attempt to download the full image of the files.
            Console.WriteLine("Download full-sized files with actions:");
            // Expect that 5 of 10 will fail because they were created while CanStoreFullImage was false.
            foreach (JObject account in accountsWithImagesResponse.Records.Cast<JObject>())
            {
                try
                {
                    // It is not possible to request thumbnail sized images using actions.
                    byte[] downloadedFile = await DownloadImageWithActions(
                        service,
                        entityLogicalName: entityLogicalName,
                        primaryKeyLogicalName: "accountid",
                        entityId: (Guid)account["accountid"],
                        imagePropertyName: imageColumnLogicalName);

                    string recordName = (string)account["name"];
                    string downloadedFileName = $"{recordName}_downloaded_with_actions.png";
                    File.WriteAllBytes($"DownloadedImages\\{downloadedFileName}", downloadedFile);
                }
                catch (ServiceException se)
                {
                    string errorCode = se.ODataError.Error.Code;
                    if (errorCode == "0x80040217")
                    {
                        // ObjectDoesNotExist error
                        // No FileAttachment records found for imagedescriptorId: <guid> for image attribute: sample_imagecolumn of account record with id <guid>
                        // These 5 images were created while CanStoreFullImage was false
                        Console.WriteLine($"\tDownload failed: {se.Message}");
                    }
                    else
                    {
                        throw se;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            // Attempt to download the full-sized image of the files with chunks
            Console.WriteLine("Download full-sized files with chunks:");
            // Expect that 5 of 10 will fail because they were created while CanStoreFullImage was false.
            foreach (JObject account in accountsWithImagesResponse.Records.Cast<JObject>())
            {
                try
                {
                    byte[] downloadedFile = await DownloadImageWithChunks(
                        service,
                        entitySetName: entitySetName,
                        entityId: (Guid)account["accountid"],
                        imagePropertyName: imageColumnLogicalName,
                        returnFullSizeImage: true);

                    string recordName = (string)account["name"];
                    string downloadedFileName = $"{recordName}_downloaded_with_chunks_full-sized.png";
                    File.WriteAllBytes($"DownloadedImages\\{downloadedFileName}", downloadedFile);
                }
                catch (Exception ex)
                {
                    if (ex.Message == "No content returned with request")
                    {

                        Console.WriteLine("No image data returned because record was created while CanStoreFullImage was false.");
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }

            // Attempt to download the full-sized image of the files as stream
            Console.WriteLine("Download full-sized files in single requests:");
            // Expect that 5 of 10 will fail because they were created while CanStoreFullImage was false.
            foreach (JObject account in accountsWithImagesResponse.Records.Cast<JObject>())
            {
                try
                {
                    byte[] downloadedFile = await DownloadImageWithStream(
                        service,
                        entitySetName: entitySetName,
                        entityId: (Guid)account["accountid"],
                        imagePropertyName: imageColumnLogicalName,
                        returnFullSizeImage: true);

                    string recordName = (string)account["name"];
                    string downloadedFileName = $"{recordName}_downloaded_with_stream_full-sized.png";
                    File.WriteAllBytes($"DownloadedImages\\{downloadedFileName}", downloadedFile);
                }
                catch (Exception ex)
                {
                    if (ex.Message == "No content returned with request")
                    {

                        Console.WriteLine("No image data returned because record was created while CanStoreFullImage was false.");
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }

            // Delete the records that were created by this sample
            foreach (Guid id in accountsWithImagesIds)
            {
                await service.Delete(new EntityReference("accounts", id));
            }
            Console.WriteLine("Deleted the records created for this sample.");

            // Set the account primaryImage back to the original value
            await Utility.SetTablePrimaryImageName(service, entityLogicalName, originalAccountPrimaryImageAttributeName, isPrimaryImage: true);

            // Delete the Image Column
            await Utility.DeleteImageColumn(service, entityLogicalName, imageColumnSchemaName);

            Console.WriteLine("Sample completed.");
        }

        /// <summary>
        /// Downloads an image using Web API InitializeFileBlocksDownload and DownloadBlock Actions
        /// </summary>
        /// <param name="service">The service</param>
        /// <param name="entityLogicalName">The logical name of the table</param>
        /// <param name="primaryKeyLogicalName">The logical name of the primary key for the table</param>
        /// <param name="entityId">The Id of the record.</param>
        /// <param name="imagePropertyName">The name of the image column property.</param>
        /// <returns>The requested Image</returns>
        private static async Task<byte[]> DownloadImageWithActions(Service service,
                string entityLogicalName,
                string primaryKeyLogicalName,
                Guid entityId,
                string imagePropertyName)
        {
            InitializeFileBlocksDownloadRequest initializeFileBlocksDownloadRequest = new(
                entityLogicalName: entityLogicalName,
                primaryKeyLogicalName: primaryKeyLogicalName,
                entityId: entityId,
                fileAttributeName: imagePropertyName);

            InitializeFileBlocksDownloadResponse initializeFileBlocksDownloadResponse =
                await service.SendAsync<InitializeFileBlocksDownloadResponse>(initializeFileBlocksDownloadRequest);

            string fileContinuationToken = initializeFileBlocksDownloadResponse.FileContinuationToken;
            long fileSizeInBytes = initializeFileBlocksDownloadResponse.FileSizeInBytes;

            List<byte> bytes = new();

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
                DownloadBlockRequest downLoadBlockRequest = new(
                    offset: offset,
                    blockLength: blockSizeDownload,
                    fileContinuationToken: fileContinuationToken);

                // Send the request
                DownloadBlockResponse downloadBlockResponse =
                           await service.SendAsync<DownloadBlockResponse>(downLoadBlockRequest);

                // Add the block returned to the list
                bytes.AddRange(downloadBlockResponse.Data);

                // Subtract the amount downloaded,
                // which may make fileSizeInBytes < 0 and indicate
                // no further blocks to download
                fileSizeInBytes -= (int)blockSizeDownload;
                // Increment the offset to start at the beginning of the next block.
                offset += blockSizeDownload;
            }
            return bytes.ToArray();
        }

        /// <summary>
        /// Downloads an image in chunks using Web API
        /// </summary>
        /// <param name="service">The service</param>
        /// <param name="entitySetName">The entity set name of the table</param>
        /// <param name="entityId">The Id of the record.</param>
        /// <param name="imagePropertyName">The name of the image column property.</param>
        /// <param name="returnFullSizeImage">Whether to return the full-sized image. Otherwise the thumbnail-sized image will be retrieved.</param>
        /// <returns>The requested Image</returns>
        private static async Task<byte[]> DownloadImageWithChunks(Service service,
                string entitySetName,
                Guid entityId,
                string imagePropertyName,
                bool returnFullSizeImage) 
        {

            EntityReference entityReference = new(entitySetName, entityId);

            int downloadChunkSize = 4 * 1024 * 1024; // 4 MB
            int offSet = 0;
            var fileSize = 0;
            byte[] file = null;
            do
            {
                DownloadFileChunkRequest downloadFileChunkRequest = new(
                    entityReference: entityReference,
                    fileColumnLogicalName: imagePropertyName,
                    offSet: offSet,
                    chunkSize: downloadChunkSize,
                    returnFullSizedImage: returnFullSizeImage);

                var downloadFileChunkResponse =
                    await service.SendAsync<DownloadFileChunkResponse>(downloadFileChunkRequest);

                if (downloadFileChunkResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    throw new Exception("No content returned with request");
                }

                if (file == null)
                {
                    fileSize = downloadFileChunkResponse.FileSize;
                    file = new byte[fileSize];
                }
                downloadFileChunkResponse.Data.CopyTo(file, offSet);

                offSet += downloadChunkSize;

            } while (offSet < fileSize);

            return file;

        }

        /// <summary>
        /// Downloads an image in one request using Web API
        /// </summary>
        /// <param name="service">The service</param>
        /// <param name="entitySetName">The entity set name of the table</param>
        /// <param name="entityId">The Id of the record.</param>
        /// <param name="imagePropertyName">The name of the image column property.</param>
        /// <param name="returnFullSizeImage">Whether to return the full-sized image. Otherwise the thumbnail-sized image will be retrieved.</param>
        /// <returns>The requested Image</returns>
        private static async Task<byte[]> DownloadImageWithStream(Service service,
                string entitySetName,
                Guid entityId,
                string imagePropertyName,
                bool returnFullSizeImage) 
        {
            EntityReference entityReference = new(entitySetName, entityId);

            DownloadFileRequest downloadFileRequest = new(
                entityReference: entityReference,
                property: imagePropertyName, 
                returnFullSizedImage: returnFullSizeImage);

            var downloadFileResponse = await service.SendAsync<DownloadFileResponse>(downloadFileRequest);

            if (downloadFileResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                throw new Exception("No content returned with request");
            }

            return downloadFileResponse.File;

        }

    }
}
