using Newtonsoft.Json.Linq;
using PowerApps.Samples;
using PowerApps.Samples.Messages;
using PowerApps.Samples.Methods;
using System.Net.Http;
using System.Text;

namespace FileOperations
{
    internal class Program
    {
        static async Task Main()
        {
            Config config = App.InitializeApp();

            var service = new Service(config);

            //await WorkWithFilesSimple(service);

             

            // await FileOperations(service);

            // await FileOperationsWithActions(service);



            // await UploadChunkedFiles(service); 



        }

        private static async Task WorkWithFilesSimple(Service service)
        {
            string largeFileName = "4094kb.txt";
            string largeFilePath = $"Files\\{largeFileName}";
            string fileColumnLogicalName = "sample_filecolumn";

            #region create account

            JObject account = new() {

                { "name", "Test account upload file"},
            };

            EntityReference createdAccountRef = await service.Create("accounts", account);

            #endregion create account

            #region upload file

            UploadFileRequest uploadFileRequest = new(
                 entityReference: createdAccountRef,
                 columnName: fileColumnLogicalName,
                 fileContent: File.OpenRead(largeFilePath),
                 fileName: largeFileName);

            await service.SendAsync(uploadFileRequest);

            #endregion upload file

            #region download file

            DownloadFileRequest downloadFileRequest = new(
                entityReference: createdAccountRef, 
                property: fileColumnLogicalName);

            try
            {
                var downloadFileResponse = await service.SendAsync<DownloadFileResponse>(downloadFileRequest);

                // File written to FileOperations\bin\Debug\net6.0
                File.WriteAllBytes($"downloaded-{largeFileName}", downloadFileResponse.File);
            }
            catch (ServiceException se)
            {
                if (se.ODataError.Error.Code.Equals("0x80090001"))
                {
    //{
    //  "error": {
    //      "code": "0x80090001",
    //      "message": "Maximum file size supported for download is [16] MB. File of [24 MB] size may only be downloaded using staged chunk download."
    //    }
    //}
                    Console.WriteLine(se.ODataError.Error.Message);
                }                
            } 

            #endregion download file

            #region delete file
            DeleteColumnValueRequest deleteColumnValueRequest = new(
                entityReference: createdAccountRef,
                propertyName: fileColumnLogicalName);
            await service.SendAsync(deleteColumnValueRequest);
            #endregion delete file

            #region delete account

            await service.Delete(createdAccountRef);

            #endregion delete account

        }

        /// <summary>
        /// Demonstrates file operations without actions
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        private static async Task FileOperations(Service service)
        {

            string fileName = "25mb.pdf";
            string filePath = $"Files\\{fileName}";
            string fileColumnLogicalName = "sample_filecolumn";

            #region create account

            JObject account = new() {

                { "name", "Test account for file data sample"}
            };

            EntityReference createdAccountRef = await service.Create("accounts", account);

            #endregion create account

            #region upload file

            InitializeChunkedFileUploadRequest initializeChunkedFileUploadRequest = new(
                entityReference: createdAccountRef,
                fileColumnLogicalName: fileColumnLogicalName,
                uploadFileName: fileName);

            var initializeChunkedFileUploadResponse =
                await service.SendAsync<InitializeChunkedFileUploadResponse>(initializeChunkedFileUploadRequest);

            int uploadChunkSize = initializeChunkedFileUploadResponse.ChunkSize;
            Uri Url = initializeChunkedFileUploadResponse.Url;


            var fileBytes = await File.ReadAllBytesAsync(filePath);

            for (var offset = 0; offset < fileBytes.Length; offset += uploadChunkSize)
            {

                UploadFileChunkRequest uploadFileChunkRequest = new(
                    url: Url,
                    uploadFileName: fileName,
                    chunkSize: uploadChunkSize,
                    fileBytes: fileBytes,
                    offSet: offset);

                await service.SendAsync(uploadFileChunkRequest);
            }

            #endregion upload file 

            #region download file

            int downloadChunkSize = 4 * 1024 * 1024; // 4 MB
            int offSet = 0;
            var fileSize = 0;
            byte[] file = null;
            do
            {

                DownloadFileChunkRequest downloadFileChunkRequest = new(
                    entityReference: createdAccountRef,
                    fileColumnLogicalName: fileColumnLogicalName,
                    uploadFileName: fileName,
                    offSet: offSet,
                    chunkSize: downloadChunkSize);

                var downloadFileChunkResponse =
                    await service.SendAsync<DownloadFileChunkResponse>(downloadFileChunkRequest);

                if (file == null)
                {
                    fileSize = downloadFileChunkResponse.FileSize;
                    file = new byte[fileSize];
                }
                downloadFileChunkResponse.Data.CopyTo(file, offSet);

                offSet += downloadChunkSize;

            } while (offSet < fileSize);

            // File written to FileOperations\bin\Debug\net6.0
            File.WriteAllBytes($"downloaded-{fileName}", file);

            #endregion download File

            #region delete File

            DeleteColumnValueRequest deleteColumnValueRequest = new(
                entityReference: createdAccountRef, 
                propertyName: fileColumnLogicalName);
            await service.SendAsync(deleteColumnValueRequest);

            #endregion delete File

            #region delete account

            // Delete the account record.
            await service.Delete(createdAccountRef);

            #endregion delete account
        }



        /// <summary>
        /// Demonstrates file operations using actions
        /// </summary>
        /// <param name="service">The service</param>
        /// <returns></returns>
        private static async Task FileOperationsWithActions(Service service)
        {

            string fileName = "25mb.pdf";
            string filePath = $"Files\\{fileName}";
            string fileMimeType = "application/pdf";
            string fileColumnLogicalName = "sample_filecolumn";

            #region create account

            JObject account = new() {

                { "name", "Test account for file data sample"}
            };

            EntityReference createdAccount = await service.Create("accounts", account);

            #endregion create account

            #region upload File

            // Initialize the upload
            InitializeFileBlocksUploadRequest initializeFileBlocksUploadRequest = new(
                entityLogicalName: "account",
                primaryKeyLogicalName: "accountid",
                entityId: createdAccount.Id.Value,
                fileAttributeName: fileColumnLogicalName,
                fileName: fileName);

            InitializeFileBlocksUploadResponse initializeFileBlocksUploadResponse =
                await service.SendAsync<InitializeFileBlocksUploadResponse>(initializeFileBlocksUploadRequest);
            string uploadFileContinuationToken = initializeFileBlocksUploadResponse.FileContinuationToken;


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

            // Id can be used with DeleteFile message
            Guid fileId = commitFileBlocksUploadResponse.FileId;

            Console.WriteLine($"FileId:{fileId}" +
                $"\nFileSizeInBytes:{commitFileBlocksUploadResponse.FileSizeInBytes}");

            #endregion upload 

            #region download File

            InitializeFileBlocksDownloadRequest initializeFileBlocksDownloadRequest = new(
                entityLogicalName: "account",
                primaryKeyLogicalName: "accountid",
                entityId: createdAccount.Id.Value,
                fileAttributeName: fileColumnLogicalName);

            InitializeFileBlocksDownloadResponse initializeFileBlocksDownloadResponse =
                await service.SendAsync<InitializeFileBlocksDownloadResponse>(initializeFileBlocksDownloadRequest);

            string downloadfileContinuationToken = initializeFileBlocksDownloadResponse.FileContinuationToken;
            long fileSizeInBytes = initializeFileBlocksDownloadResponse.FileSizeInBytes;


            byte[] file = await DownloadBlocks(
                    service: service,
                    fileContinuationToken: downloadfileContinuationToken,
                    fileSizeInBytes: fileSizeInBytes);

            // File written to FileOperations\bin\Debug\net6.0
            File.WriteAllBytes($"downloaded-{fileName}", file);


            #endregion download File

            #region delete File

            DeleteFileRequest deleteFileRequest = new(fileId: fileId);
            await service.SendAsync(deleteFileRequest);

            #endregion delete File

            // Delete the account record.
            await service.Delete(createdAccount);
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
                // Generates string blockid value based on number:
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
            }
        }


        private static async Task CreateAccountWithAnnotation(Service service)
        {
            string filePath = "Files\\25mb.pdf";
            string fileMimeType = "application/pdf";

            #region create account with note

            JObject account = new() {

                { "name", "Test account with note"},
                { "Account_Annotation", new JArray{
                    {
                        new JObject{
                            {"subject","A note for the account record" },
                            {"notetext","<div data-wrapper=\"true\" style=\"font-size:9pt;font-family:'Segoe UI','Helvetica Neue',sans-serif;\"><div>Some text for the note.</div></div>" },
                            {"filename", "25mb.pdf"},
                            {"mimetype",fileMimeType}
                        }
                    }
                  }
               }
            };

            EntityReference createdAccount = await service.Create("accounts", account);

            JObject retrievedAccount = await service.Retrieve(createdAccount, "?$select=accountid&$expand=Account_Annotation($select=annotationid)");

            Guid annotationId = (Guid)retrievedAccount["Account_Annotation"][0]["annotationid"];

            EntityReference annotationRef = new EntityReference("annotations", annotationId);

            #endregion create account with note

            #region upload file to note

            InitializeAnnotationBlocksUploadRequest initializeNoteUploadRequest = new(target: annotationRef);
            var initializeNoteUploadResponse =
                await service.SendAsync<InitializeAnnotationBlocksUploadResponse>(initializeNoteUploadRequest);

            string fileContinuationToken = initializeNoteUploadResponse.FileContinuationToken;

            List<string> blocks = new();

            await UploadBlocks(service, filePath, fileContinuationToken, blocks);

            CommitAnnotationBlocksUploadRequest commitAnnotationBlocksUploadRequest = new(
                target: annotationRef,
                blockList: blocks,
                fileContinuationToken: fileContinuationToken);

            CommitAnnotationBlocksUploadResponse commitAnnotationBlocksUploadResponse =
                await service.SendAsync<CommitAnnotationBlocksUploadResponse>(commitAnnotationBlocksUploadRequest);

            int fileSizeInBytes = commitAnnotationBlocksUploadResponse.FileSizeInBytes;
            #endregion upload file to note

            // Delete the account
            await service.Delete(createdAccount);

        }







        private static async Task UploadChunkedFiles(Service service)
        {
            string largeFileName = "25mb.pdf";
            string largeFilePath = $"Files\\{largeFileName}";

            JObject account = new() {

                { "name", "Test account upload file"},
            };

            EntityReference createdAccount = await service.Create("accounts", account);

            await ChunkedUploadAsync(service, createdAccount, "sample_filecolumn", "Files", largeFileName);


            await service.Delete(createdAccount);

        }

        /// <summary>
        /// Upload example from docs
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entityReference"></param>
        /// <param name="fileColumnLogicalName"></param>
        /// <param name="fileRootPath"></param>
        /// <param name="uploadFileName"></param>
        /// <returns></returns>
        static async Task ChunkedUploadAsync(Service service,
            EntityReference entityReference,
            string fileColumnLogicalName,
            string fileRootPath,
            string uploadFileName)
        {
            var filePath = Path.Combine(fileRootPath, uploadFileName);
            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var url = new Uri($"{entityReference.Path}/{fileColumnLogicalName}", UriKind.Relative);

            var chunkSize = 0;

            // Initialize Chunked File Upload
            using (var request = new HttpRequestMessage(HttpMethod.Patch, url))
            {
                request.Headers.Add("x-ms-transfer-mode", "chunked");
                request.Headers.Add("x-ms-file-name", uploadFileName);
                using var response = await service.SendAsync(request);
                response.EnsureSuccessStatusCode();
                url = response.Headers.Location;
                chunkSize = int.Parse(response.Headers.GetValues("x-ms-chunk-size").First());
            }

            for (var offset = 0; offset < fileBytes.Length; offset += chunkSize)
            {
                // Upload file chunk
                var count = (offset + chunkSize) > fileBytes.Length ? fileBytes.Length % chunkSize : chunkSize;
                using var content = new ByteArrayContent(fileBytes, offset, count);
                using var request = new HttpRequestMessage(HttpMethod.Patch, url);
                content.Headers.Add("Content-Type", "application/octet-stream");
                content.Headers.ContentRange = new System.Net.Http.Headers.ContentRangeHeaderValue(offset, offset + (count - 1), fileBytes.Length);
                request.Headers.Add("x-ms-file-name", uploadFileName);
                request.Content = content;
                using var response = await service.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}