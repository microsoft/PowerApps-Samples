using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using System.Text;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates working with data in file columns.
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
            ServiceClient serviceClient =
                new(app.Configuration.GetConnectionString("default"));

            string entityLogicalName = "account";
            string fileColumnSchemaName = "sample_FileColumn";
            string fileColumnLogicalName = fileColumnSchemaName.ToLower();
            string fileName = "25mb.pdf";
            string filePath = $"Files\\{fileName}";
            FileInfo fileInfo = new(filePath);
            string fileMimeType = "application/pdf";
            int fileColumnMaxSizeInKb;
            Guid? fileId = null;

            // Create the File Column
            Utility.CreateFileColumn(serviceClient, entityLogicalName, fileColumnSchemaName);

            // Update the MaxSizeInKB: Comment this line to get error about file too large for column.
            Utility.UpdateFileColumnMaxSizeInKB(serviceClient, entityLogicalName, fileColumnSchemaName.ToLower(), 100 * 1024);

            //Get the configured size of the column in KB
            fileColumnMaxSizeInKb = Utility.GetFileColumnMaxSizeInKb(serviceClient, entityLogicalName, fileColumnSchemaName.ToLower());

            #region create account record

            Entity account = new("account")
            {
                Attributes = {
                    { "name", "Test account for file data sample"}
                }
            };

            Guid accountid = serviceClient.Create(account);

            EntityReference accountReference = new("account", accountid);

            Console.WriteLine($"Created account record with accountid:{accountid}");

            #endregion create account record

            Console.WriteLine($"Uploading file {filePath} ...");

            // Upload the file
            try
            {
                fileId = UploadFile(service: serviceClient,
                   entityReference: accountReference,
                   fileAttributeName: fileColumnLogicalName,
                   fileInfo: fileInfo,
                   fileMimeType: fileMimeType,
                   fileColumnMaxSizeInKb: fileColumnMaxSizeInKb);

                Console.WriteLine($"Uploaded file {filePath}");
            }
            catch (Exception ex)
            {                
                Console.WriteLine(ex.Message);
            }

            // Download the file

            if (fileId.HasValue) {

                Console.WriteLine($"Downloading file...");

                byte[] downloadedFile = DownloadFile(
                    service: serviceClient,
                    entityReference: accountReference,
                    fileAttributeName: fileColumnLogicalName);

                // File written to FileOperations\bin\Debug\net6.0
                File.WriteAllBytes($"downloaded-{fileName}", downloadedFile);
                Console.WriteLine($"Downloaded the file to {Environment.CurrentDirectory}//downloaded-{fileName}.");

                // Delete the file
                DeleteFileRequest deleteFileRequest = new()
                {
                    FileId = fileId.Value
                };

                serviceClient.Execute(deleteFileRequest);
                Console.WriteLine("Deleted file using FileId.");
                // There is no other way to delete a file using the SDK.

            }

            // Delete the account record
            serviceClient.Delete(accountReference.LogicalName, accountReference.Id);
            Console.WriteLine("Deleted the account record.");


            // Delete the file column
            Utility.DeleteFileColumn(serviceClient, entityLogicalName, fileColumnSchemaName);

            Console.WriteLine("\nSample complete.");
        }

        /// <summary>
        /// Uploads a file
        /// </summary>
        /// <param name="service">The service</param>
        /// <param name="entityReference">A reference to the record with the file column</param>
        /// <param name="fileAttributeName">The name of the file column</param>
        /// <param name="fileInfo">Information about the file to upload.</param>
        /// <param name="fileMimeType">The mime type of the file, if known.</param>
        /// <param name="fileColumnMaxSizeInKb">The size limit of the column, if known.</param>
        /// <returns></returns>
        static Guid UploadFile(
                IOrganizationService service,
                EntityReference entityReference,
                string fileAttributeName,
                FileInfo fileInfo,
                string fileMimeType = null,
                int? fileColumnMaxSizeInKb = null)
        {

            // Initialize the upload
            InitializeFileBlocksUploadRequest initializeFileBlocksUploadRequest = new()
            {
                Target = entityReference,
                FileAttributeName = fileAttributeName,
                FileName = fileInfo.Name
            };

            var initializeFileBlocksUploadResponse =
                (InitializeFileBlocksUploadResponse)service.Execute(initializeFileBlocksUploadRequest);

            string fileContinuationToken = initializeFileBlocksUploadResponse.FileContinuationToken;

            // Capture blockids while uploading
            List<string> blockIds = new();

            using Stream uploadFileStream = fileInfo.OpenRead();

            int blockSize = 4 * 1024 * 1024; // 4 MB

            byte[] buffer = new byte[blockSize];
            int bytesRead = 0;

            long fileSize = fileInfo.Length;

            if (fileColumnMaxSizeInKb.HasValue && (fileInfo.Length/1024) > fileColumnMaxSizeInKb.Value) {
                throw new Exception($"The file is too large to be uploaded to this column.");
            }


            // The number of iterations that will be required:
            // int blocksCount = (int)Math.Ceiling(fileSize / (float)blockSize);
            int blockNumber = 0;

            // While there is unread data from the file
            while ((bytesRead = uploadFileStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                // The file or final block may be smaller than 4MB
                if (bytesRead < buffer.Length)
                {
                    Array.Resize(ref buffer, bytesRead);
                }

                blockNumber++;

                //string blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(blockNumber.ToString().PadLeft(16, '0')));
                string blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));

                blockIds.Add(blockId);

                // Prepare the request
                UploadBlockRequest uploadBlockRequest = new()
                {
                    BlockData = buffer,
                    BlockId = blockId,
                    FileContinuationToken = fileContinuationToken,
                };

                // Send the request
                service.Execute(uploadBlockRequest);
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
            CommitFileBlocksUploadRequest commitFileBlocksUploadRequest = new()
            {
                BlockList = blockIds.ToArray(),
                FileContinuationToken = fileContinuationToken,
                FileName = fileInfo.Name,
                MimeType = fileMimeType
            };

            var commitFileBlocksUploadResponse =
                (CommitFileBlocksUploadResponse)service.Execute(commitFileBlocksUploadRequest);

            return commitFileBlocksUploadResponse.FileId;

        }

        /// <summary>
        /// Downloads a file
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