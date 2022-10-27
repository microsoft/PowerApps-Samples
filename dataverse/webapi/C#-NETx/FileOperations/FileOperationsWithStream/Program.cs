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
            bool fileUploaded = false;
            int? fileColumnMaxSizeInKb;

            // Create the File Column with 10MB limit
            await Utility.CreateFileColumn(service, entityLogicalName, fileColumnSchemaName);

            // Update the MaxSizeInKB value to 100MB. Comment this line to get error about file too large for column.
            await Utility.UpdateFileColumnMaxSizeInKB(service, entityLogicalName, fileColumnSchemaName.ToLower(), 100 * 1024);

            //Get the configured size of the column in KB
            fileColumnMaxSizeInKb = await Utility.GetFileColumnMaxSizeInKb(service, entityLogicalName, fileColumnSchemaName.ToLower());

            #region create account

            JObject account = new() {

                { "name", "Test account upload file"},
            };

            EntityReference createdAccountRef = await service.Create("accounts", account);

            Console.WriteLine($"Created account record with accountid:{createdAccountRef.Id.Value}");

            #endregion create account


            try
            {
                Console.WriteLine($"Uploading file {filePath} ...");

                // Upload file
                UploadFileRequest uploadFileRequest = new(
                     entityReference: createdAccountRef,
                     columnName: filePropertyName,
                     fileContent: File.OpenRead(filePath),
                     fileName: fileName,
                     fileColumnMaxSizeInKb: fileColumnMaxSizeInKb);

                await service.SendAsync(uploadFileRequest);

                Console.WriteLine($"Uploaded file {filePath}");

                fileUploaded = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (fileUploaded)
            {
                Console.WriteLine($"Downloading file from {createdAccountRef.Path}/{filePropertyName} ...");
                // Download file
                DownloadFileRequest downloadFileRequest = new(
                    entityReference: createdAccountRef,
                    property: filePropertyName);

                var downloadFileResponse = await service.SendAsync<DownloadFileResponse>(downloadFileRequest);

                // File written to FileOperationsWithStream\bin\Debug\net6.0
                File.WriteAllBytes($"downloaded-{fileName}", downloadFileResponse.File);
                Console.WriteLine($"Downloaded the file to {Environment.CurrentDirectory}//downloaded-{fileName}.");


                // Delete file
                DeleteColumnValueRequest deleteColumnValueRequest = new(
                    entityReference: createdAccountRef,
                    propertyName: filePropertyName);
                await service.SendAsync(deleteColumnValueRequest);

                Console.WriteLine($"Deleted file at: {deleteColumnValueRequest.RequestUri}.");
            }

            // Delete the account record
            await service.Delete(createdAccountRef);
            Console.WriteLine("Deleted the account record.");

            // Delete the file column
            await Utility.DeleteFileColumn(service, entityLogicalName, fileColumnSchemaName.ToLower());

        }
    }
}