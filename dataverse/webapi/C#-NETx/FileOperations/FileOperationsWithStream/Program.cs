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

            string fileName = "4094kb.txt";
            string filePath = $"Files\\{fileName}";
            string fileColumnLogicalName = Utility.fileColumnSchemaName.ToLower(); //sample_filecolumn

            // Create the File Column
            await Utility.CreateFileColumn(service);

            #region create account

            JObject account = new() {

                { "name", "Test account upload file"},
            };

            EntityReference createdAccountRef = await service.Create("accounts", account);

            Console.WriteLine($"Created account record with accountid:{createdAccountRef.Id.Value}");

            #endregion create account

            #region upload file

            UploadFileRequest uploadFileRequest = new(
                 entityReference: createdAccountRef,
                 columnName: fileColumnLogicalName,
                 fileContent: File.OpenRead(filePath),
                 fileName: fileName);

            await service.SendAsync(uploadFileRequest);

            Console.WriteLine($"Uploaded file {fileName}.");

            #endregion upload file

            #region download file

            DownloadFileRequest downloadFileRequest = new(
                entityReference: createdAccountRef,
                property: fileColumnLogicalName);

            try
            {
                var downloadFileResponse = await service.SendAsync<DownloadFileResponse>(downloadFileRequest);

                // File written to FileOperationsWithStream\bin\Debug\net6.0
                File.WriteAllBytes($"downloaded-{fileName}", downloadFileResponse.File);
            }
            catch (ServiceException se)
            {
                // Change fileName to 25mb.pdf to encounter this error

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

            Console.WriteLine($"Deleted file at: {deleteColumnValueRequest.RequestUri}.");

            #endregion delete file

            // Delete the account record
            await service.Delete(createdAccountRef);
            Console.WriteLine("Deleted the account record.");

            // Delete the file column
            await Utility.DeleteFileColumn(service);

        }



    }


}