using Newtonsoft.Json.Linq;
using PowerApps.Samples;
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

            string fileName = "25mb.pdf";
            string filePath = $"Files\\{fileName}";
            string fileMimeType = "application/pdf";
            string fileColumnLogicalName = Utility.fileColumnSchemaName.ToLower(); //sample_filecolumn

            // Create the File Column
            await Utility.CreateFileColumn(service);
     
            #region create account record

            JObject account = new() {

                { "name", "Test account for file data sample"}
            };

            EntityReference createdAccountRef = await service.Create("accounts", account);

            Console.WriteLine($"Created account record with accountid:{createdAccountRef.Id.Value}");

            #endregion create account record

            #region upload File

            // Initialize the upload
            InitializeFileBlocksUploadRequest initializeFileBlocksUploadRequest = new(
                entityLogicalName: "account",
                primaryKeyLogicalName: "accountid",
                entityId: createdAccountRef.Id.Value,
                fileAttributeName: fileColumnLogicalName,
                fileName: fileName);

            InitializeFileBlocksUploadResponse initializeFileBlocksUploadResponse =
                await service.SendAsync<InitializeFileBlocksUploadResponse>(initializeFileBlocksUploadRequest);
            string uploadFileContinuationToken = initializeFileBlocksUploadResponse.FileContinuationToken;

            Console.WriteLine($"Initialized upload of file {fileName}.");

            // Capture blockids while uploading
            List<string> blocks = new();

            // Upload the file in blocks
            await UploadBlocks(service, filePath, uploadFileContinuationToken, blocks);

            // Commit the upload
            CommitFileBlocksUploadRequest commitFileBlocksUploadRequest = new(
                fileName: fileName,
                mimeType: fileMimeType,
                blockList: blocks,
                fileContinuationToken: uploadFileContinuationToken);

            CommitFileBlocksUploadResponse commitFileBlocksUploadResponse =
                await service.SendAsync<CommitFileBlocksUploadResponse>(commitFileBlocksUploadRequest);

            Console.WriteLine($"Committed upload of file {fileName}.");

            // Id can be used with DeleteFile message
            Guid fileId = commitFileBlocksUploadResponse.FileId;

            Console.WriteLine($"Uploaded {fileName} to account {fileColumnLogicalName} column.");
            Console.WriteLine($"FileId:{fileId}" +
                $"\nFileSizeInBytes:{commitFileBlocksUploadResponse.FileSizeInBytes}");

            #endregion upload 

            #region download File

            InitializeFileBlocksDownloadRequest initializeFileBlocksDownloadRequest = new(
                entityLogicalName: "account",
                primaryKeyLogicalName: "accountid",
                entityId: createdAccountRef.Id.Value,
                fileAttributeName: fileColumnLogicalName);

            Console.WriteLine($"Initialized download of file from {fileColumnLogicalName} column of account with id {createdAccountRef.Id.Value}.");

            InitializeFileBlocksDownloadResponse initializeFileBlocksDownloadResponse =
                await service.SendAsync<InitializeFileBlocksDownloadResponse>(initializeFileBlocksDownloadRequest);

            string downloadfileContinuationToken = initializeFileBlocksDownloadResponse.FileContinuationToken;
            long fileSizeInBytes = initializeFileBlocksDownloadResponse.FileSizeInBytes;


            byte[] file = await DownloadBlocks(
                    service: service,
                    fileContinuationToken: downloadfileContinuationToken,
                    fileSizeInBytes: fileSizeInBytes);

            // File written to FileOperationsWithActions\bin\Debug\net6.0
            File.WriteAllBytes($"downloaded-{fileName}", file);


            #endregion download File

            #region delete File

            DeleteFileRequest deleteFileRequest = new(fileId: fileId);
            await service.SendAsync(deleteFileRequest);
            Console.WriteLine("Deleted file using FileId.");

            #endregion delete File

            // Delete the account record
            await service.Delete(createdAccountRef);
            Console.WriteLine("Deleted the account record.");

            // Delete the file column
            await Utility.DeleteFileColumn(service);

        }

        /// <summary>
        /// Calls the UploadBlock action as many times as needed to upload a file in blocks.
        /// </summary>
        /// <param name="service">The service</param>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="fileContinuationToken">The file continuation token value</param>
        /// <param name="blockIds">List of block Ids to write to</param>
        /// <returns></returns>
        private static async Task UploadBlocks(
            Service service,
            string filePath,
            string fileContinuationToken,
            List<string> blockIds)
        {
            using Stream file = File.OpenRead(filePath);

            int blockSize = 4 * 1024 * 1024; // 4 MB

            byte[] buffer = new byte[blockSize];
            int bytesRead = 0;
            FileInfo f = new(filePath);

            long fileSize = f.Length;
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
                // Generates string blockId value based on number:
                // 0 = "MDAwMDAwMDAwMDAwMDAwMA=="
                // 1 = "MDAwMDAwMDAwMDAwMDAwMQ=="
                // 2 = "MDAwMDAwMDAwMDAwMDAwMg=="
                string blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(blockNumber.ToString().PadLeft(16, '0')));
                
                // List<string> is a reference type, so this will update the value passed into the function.
                blockIds.Add(blockId);

                // Copy the next block of data to send.
                var blockData = new byte[buffer.Length];
                buffer.CopyTo(blockData, 0);

                // Prepare the request
                UploadBlockRequest uploadBlockRequest = new(
                    blockId: blockId,
                    blockData: blockData,
                    fileContinuationToken: fileContinuationToken);

                // Send the request
                await service.SendAsync(uploadBlockRequest);

                Console.WriteLine($"\tUploaded blockId {blockId}");
            }
        }

        /// <summary>
        /// Calls the DownloadBlock action as many times as needed to download a file in blocks.
        /// </summary>
        /// <param name="service">The Service</param>
        /// <param name="fileContinuationToken">The file continuation token</param>
        /// <param name="fileSizeInBytes">The total size of the file.</param>
        /// <returns>The file bytes</returns>
        private static async Task<byte[]> DownloadBlocks(
            Service service,
            string fileContinuationToken,
            long fileSizeInBytes)
        {

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

                Console.WriteLine($"\t Downloading block offset:{offset} blocklength:{blockSizeDownload}");

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