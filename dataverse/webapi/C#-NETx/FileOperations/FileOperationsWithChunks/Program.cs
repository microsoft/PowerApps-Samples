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

            string fileName = "25mb.pdf";
            string filePath = $"Files\\{fileName}";
            string fileColumnLogicalName = Utility.fileColumnSchemaName.ToLower(); //sample_filecolumn

            // Create the File Column
            await Utility.CreateFileColumn(service);

            #region create account

            JObject account = new() {

                { "name", "Test account for file data sample"}
            };

            EntityReference createdAccountRef = await service.Create("accounts", account);

            Console.WriteLine($"Created account record with accountid:{createdAccountRef.Id.Value}");

            #endregion create account

            #region upload file

            InitializeChunkedFileUploadRequest initializeChunkedFileUploadRequest = new(
                entityReference: createdAccountRef,
                fileColumnLogicalName: fileColumnLogicalName,
                uploadFileName: fileName);

            var initializeChunkedFileUploadResponse =
                await service.SendAsync<InitializeChunkedFileUploadResponse>(initializeChunkedFileUploadRequest);

            Console.WriteLine($"Initialized upload of file {fileName}.");

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

                Console.WriteLine($"\tUploaded chunk offset:{offset} ChunkSize:{uploadChunkSize}");
            }

            #endregion upload file 

            #region download file

            Console.WriteLine($"Starting download of file from {fileColumnLogicalName} column of account with id {createdAccountRef.Id.Value}.");

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

                Console.WriteLine($"\tDownloaded chunk offSet:{offSet} ChunkSize:{downloadChunkSize}");

                if (file == null)
                {
                    fileSize = downloadFileChunkResponse.FileSize;
                    file = new byte[fileSize];
                }
                downloadFileChunkResponse.Data.CopyTo(file, offSet);

                offSet += downloadChunkSize;

            } while (offSet < fileSize);

            // File written to FileOperationsWithChunks\bin\Debug\net6.0
            File.WriteAllBytes($"downloaded-{fileName}", file);

            #endregion download File

            #region delete File

            DeleteColumnValueRequest deleteColumnValueRequest = new(
                entityReference: createdAccountRef,
                propertyName: fileColumnLogicalName);
            await service.SendAsync(deleteColumnValueRequest);

            Console.WriteLine($"Deleted file at: {deleteColumnValueRequest.RequestUri}.");

            #endregion delete File            

            // Delete the account record.
            await service.Delete(createdAccountRef);
            Console.WriteLine("Deleted the account record.");

            // Delete the file column
            await Utility.DeleteFileColumn(service);

        }
    }
}