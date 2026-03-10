using Newtonsoft.Json.Linq;
using PowerApps.Samples.Messages;
using PowerApps.Samples.Methods;

namespace PowerApps.Samples
{
    internal class Program
    {
        static async Task Main()
        {
            Config config = App.InitializeApp();

            var service = new Service(config);

            string entityLogicalName = "account";
            string fileColumnSchemaName = "sample_FileColumn";
            string filePropertyName = fileColumnSchemaName.ToLower();
            string fileName = "25mb.pdf";
            string filePath = $"Files\\{fileName}";
            int fileColumnMaxSizeInKb;
            bool fileUploaded = false;

            // Create the File Column with 10MB limit
            await Utility.CreateFileColumn(service, entityLogicalName, fileColumnSchemaName);

            // Update the MaxSizeInKB value: Comment this line to get error about file too large for column.
            await Utility.UpdateFileColumnMaxSizeInKB(service, entityLogicalName, fileColumnSchemaName.ToLower(), 100 * 1024);

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
                fileUploaded = await UploadFile(
                    service: service,
                    filePropertyName: filePropertyName,
                    fileInfo: new FileInfo(filePath),
                    entityReference: createdAccountRef,
                    fileColumnMaxSizeInKb: fileColumnMaxSizeInKb);

                Console.WriteLine($"Uploaded file {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (fileUploaded)
            {

                // Download the file
                Console.WriteLine($"Downloading file from {createdAccountRef.Path}/{filePropertyName} ...");

                byte[] file = await DownloadFile(
                    service: service,
                    filePropertyName: filePropertyName,
                    entityReference: createdAccountRef);

                // File written to FileOperationsWithChunks\bin\Debug\net6.0
                File.WriteAllBytes($"downloaded-{fileName}", file);
                Console.WriteLine($"Downloaded the file to {Environment.CurrentDirectory}//downloaded-{fileName}.");


                #region delete File

                DeleteColumnValueRequest deleteColumnValueRequest = new(
                    entityReference: createdAccountRef,
                    propertyName: filePropertyName);
                await service.SendAsync(deleteColumnValueRequest);

                Console.WriteLine($"Deleted file at: {deleteColumnValueRequest.RequestUri}.");

                #endregion delete File            

            }




            // Delete the account record.
            await service.Delete(createdAccountRef);
            Console.WriteLine("Deleted the account record.");

            // Delete the file column
            await Utility.DeleteFileColumn(service, entityLogicalName, fileColumnSchemaName.ToLower());

        }


        /// <summary>
        /// Uploads a file in chunks
        /// </summary>
        /// <param name="service">The service</param>
        /// <param name="filePropertyName">The logical name of the file column</param>
        /// <param name="fileInfo">Information about the file to upload.</param>
        /// <param name="entityReference">A reference to the record that has the file.</param>
        /// <param name="fileColumnMaxSizeInKb">The size limit of the column, if known.</param>
        /// <returns></returns>
        private static async Task<bool> UploadFile(
            Service service,
            string filePropertyName,
            FileInfo fileInfo,
            EntityReference entityReference,
            int? fileColumnMaxSizeInKb = null)
        {
            InitializeChunkedFileUploadRequest initializeChunkedFileUploadRequest = new(
                            entityReference: entityReference,
                            fileColumnLogicalName: filePropertyName,
                            uploadFileName: fileInfo.Name);

            var initializeChunkedFileUploadResponse =
                await service.SendAsync<InitializeChunkedFileUploadResponse>(initializeChunkedFileUploadRequest);

            int uploadChunkSize = initializeChunkedFileUploadResponse.ChunkSize;
            Uri Url = initializeChunkedFileUploadResponse.Url;

            var fileBytes = await File.ReadAllBytesAsync(fileInfo.FullName);

            if (fileColumnMaxSizeInKb.HasValue && (fileBytes.Length / 1024) > fileColumnMaxSizeInKb.Value)
            {
                throw new Exception($"The file is too large to be uploaded to this column.");
            }

            for (var offset = 0; offset < fileBytes.Length; offset += uploadChunkSize)
            {
                UploadFileChunkRequest uploadFileChunkRequest = new(
                    url: Url,
                    uploadFileName: fileInfo.Name,
                    chunkSize: uploadChunkSize,
                    fileBytes: fileBytes,
                    offSet: offset);

                await service.SendAsync(uploadFileChunkRequest);
            }
            return true;
        }

        /// <summary>
        /// Downloads a file in chunks
        /// </summary>
        /// <param name="service">The service</param>
        /// <param name="filePropertyName">The name of the column property.</param>
        /// <param name="entityReference">A reference to the record that has the file.</param>
        /// <returns></returns>
        private static async Task<byte[]> DownloadFile(
            Service service,
            string filePropertyName,
            EntityReference entityReference)
        {
            int downloadChunkSize = 4 * 1024 * 1024; // 4 MB
            int offSet = 0;
            var fileSize = 0;
            byte[] file = null;
            do
            {
                DownloadFileChunkRequest downloadFileChunkRequest = new(
                    entityReference: entityReference,
                    fileColumnLogicalName: filePropertyName,
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

            return file;
        }
    }
}