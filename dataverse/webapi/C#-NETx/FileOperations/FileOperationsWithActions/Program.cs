using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json.Linq;
using PowerApps.Samples.Messages;
using PowerApps.Samples.Methods;
using System.Text;

namespace PowerApps.Samples
{
    internal class Program
    {
        static async Task Main()
        {
            Config config = App.InitializeApp();

            var service = new Service(config);

            string entityLogicalName = "account";
            string primaryKeyLogicalName = "accountid";
            string fileColumnSchemaName = "sample_FileColumn";
            string filePropertyName = fileColumnSchemaName.ToLower();
            string fileName = "25mb.pdf";
            string filePath = $"Files\\{fileName}";
            string fileMimeType = "application/pdf";
            int fileColumnMaxSizeInKb;
            Guid? fileId = null;

            // Create the File Column with 10MB limit
            await Utility.CreateFileColumn(service,entityLogicalName,fileColumnSchemaName);

            // Update the MaxSizeInKB value: Comment this line to get error about file too large for column.
             await Utility.UpdateFileColumnMaxSizeInKB(service, entityLogicalName, fileColumnSchemaName.ToLower(), 100 * 1024);

            // Retrieve the MaxSizeInKb value.
            fileColumnMaxSizeInKb = await Utility.GetFileColumnMaxSizeInKb(service, entityLogicalName, fileColumnSchemaName.ToLower());

            #region create account record

            JObject account = new() {

                { "name", "Test account for file data sample"}
            };

            EntityReference createdAccountRef = await service.Create("accounts", account);

            Console.WriteLine($"Created account record with accountid:{createdAccountRef.Id.Value}");

            #endregion create account record


            try
            {
                Console.WriteLine($"Uploading file {filePath} ...");

                // Upload the file
                fileId = await UploadFile(
                    service: service,
                    entityLogicalName: entityLogicalName,
                    primaryKeyLogicalName: primaryKeyLogicalName,
                    entityId: createdAccountRef.Id.Value,
                    filePropertyName: filePropertyName,
                    fileInfo: new FileInfo(filePath),
                    fileMimeType: fileMimeType, 
                    fileColumnMaxSizeInKb: fileColumnMaxSizeInKb);

                Console.WriteLine($"Uploaded file {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (fileId.HasValue)
            {
                // Download the file
                Console.WriteLine($"Downloading file from {createdAccountRef.Path}/{filePropertyName} ...");

                byte[] file = await DownloadFile(service: service,
                    entityLogicalName: entityLogicalName,
                    primaryKeyLogicalName: primaryKeyLogicalName,
                    entityId: createdAccountRef.Id.Value,
                    filePropertyName: filePropertyName);

                // File written to FileOperationsWithActions\bin\Debug\net6.0
                File.WriteAllBytes($"downloaded-{fileName}", file);
                Console.WriteLine($"Downloaded the file to {Environment.CurrentDirectory}//downloaded-{fileName}.");

                #region delete File

                DeleteFileRequest deleteFileRequest = new(fileId: fileId.Value);
                await service.SendAsync(deleteFileRequest);
                Console.WriteLine("Deleted the file using FileId.");

                #endregion delete File
            }



            // Delete the account record
            await service.Delete(createdAccountRef);
            Console.WriteLine("Deleted the account record.");

            // Delete the file column
            await Utility.DeleteFileColumn(service,entityLogicalName,fileColumnSchemaName.ToLower());

        }

        /// <summary>
        /// Uploads a file using Web API Actions
        /// </summary>
        /// <param name="service">The service</param>
        /// <param name="entityLogicalName">The logical name of the table</param>
        /// <param name="primaryKeyLogicalName">The logical name of the primary key for the table</param>
        /// <param name="entityId">The Id of the record.</param>
        /// <param name="filePropertyName">The name of the file column property.</param>
        /// <param name="fileInfo">Information about the file to upload.</param>
        /// <param name="fileMimeType">The mime type of the file, if known.</param>
        /// <returns></returns>
        private static async Task<Guid> UploadFile(Service service, 
                string entityLogicalName,
                string primaryKeyLogicalName,
                Guid entityId,
                string filePropertyName,
                FileInfo fileInfo,
                string? fileMimeType = null,
                int? fileColumnMaxSizeInKb = null) 
        {
            // Initialize the upload
            InitializeFileBlocksUploadRequest initializeFileBlocksUploadRequest = new(
                entityLogicalName: entityLogicalName,
                primaryKeyLogicalName: primaryKeyLogicalName,
                entityId: entityId,
                fileAttributeName: filePropertyName,
                fileName: fileInfo.Name);

            InitializeFileBlocksUploadResponse initializeFileBlocksUploadResponse =
                await service.SendAsync<InitializeFileBlocksUploadResponse>(initializeFileBlocksUploadRequest);
            string fileContinuationToken = initializeFileBlocksUploadResponse.FileContinuationToken;

            // Capture blockids while uploading
            List<string> blockIds = new();

            using Stream file = fileInfo.OpenRead();

            int blockSize = 4 * 1024 * 1024; // 4 MB

            byte[] buffer = new byte[blockSize];
            int bytesRead = 0;

            long fileSize = fileInfo.Length;

            if (fileColumnMaxSizeInKb.HasValue && (fileInfo.Length / 1024) > fileColumnMaxSizeInKb.Value)
            {
                throw new Exception($"The file is too large to be uploaded to this column.");
            }


            // The number of iterations that will be required:
            // int blocksCount = (int)Math.Ceiling(fileSize / (float)blockSize);

            int blockNumber = 0;

            // While there is unread data from the file
            while ((bytesRead = file.Read(buffer, 0, buffer.Length)) > 0)
            {
                // The file or final block may be smaller than 4MB
                if (bytesRead < buffer.Length)
                {
                    Array.Resize(ref buffer, bytesRead);
                }

                blockNumber++;
                // Generates base64 string blockId values based on a Guid value so they are always the same length.
                string blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));

                // List<string> is a reference type, so this will update the value passed into the function.
                blockIds.Add(blockId);

                // Prepare the request
                UploadBlockRequest uploadBlockRequest = new(
                    blockId: blockId,
                    blockData: buffer,
                    fileContinuationToken: fileContinuationToken);

                // Send the request
                await service.SendAsync(uploadBlockRequest);
            }

            // Try to get the mimetype if not provided.
            if (string.IsNullOrEmpty(fileMimeType))
            {
                var provider = new FileExtensionContentTypeProvider();

                if (!provider.TryGetContentType(fileInfo.Name, out fileMimeType))
                {
                    fileMimeType = "application/octet-stream";
                }
            }

            // Commit the upload
            CommitFileBlocksUploadRequest commitFileBlocksUploadRequest = new(
                fileName: fileInfo.Name,
                mimeType: fileMimeType,
                blockList: blockIds,
                fileContinuationToken: fileContinuationToken);

            CommitFileBlocksUploadResponse commitFileBlocksUploadResponse =
                await service.SendAsync<CommitFileBlocksUploadResponse>(commitFileBlocksUploadRequest);

            // Id can be used with DeleteFile message
            return commitFileBlocksUploadResponse.FileId;

        }

        /// <summary>
        /// Downloads a file using Web API Actions
        /// </summary>
        /// <param name="service">The service</param>
        /// <param name="entityLogicalName">The logical name of the table</param>
        /// <param name="primaryKeyLogicalName">The logical name of the primary key for the table</param>
        /// <param name="entityId">The Id of the record.</param>
        /// <param name="filePropertyName">The name of the file column property.</param>
        /// <returns></returns>
        private static async Task<byte[]> DownloadFile(Service service,
                string entityLogicalName,
                string primaryKeyLogicalName,
                Guid entityId,
                string filePropertyName) 
        {
            InitializeFileBlocksDownloadRequest initializeFileBlocksDownloadRequest = new(
                entityLogicalName: entityLogicalName,
                primaryKeyLogicalName: primaryKeyLogicalName,
                entityId: entityId,
                fileAttributeName: filePropertyName);

            InitializeFileBlocksDownloadResponse initializeFileBlocksDownloadResponse =
                await service.SendAsync<InitializeFileBlocksDownloadResponse>(initializeFileBlocksDownloadRequest);

            string fileContinuationToken = initializeFileBlocksDownloadResponse.FileContinuationToken;
            long fileSizeInBytes = initializeFileBlocksDownloadResponse.FileSizeInBytes;

            List<byte> bytes = new((int)fileSizeInBytes);

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
    }
}