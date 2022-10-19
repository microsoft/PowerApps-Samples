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

            await TryCreateAccountWithFileColumn(service);

            //List<EntityReference> recordsToDelete = new();
            //bool deleteCreatedRecords = true;

            //Console.WriteLine("--Starting File Operations sample--");







            #region upload File
            #endregion upload 

            #region download File
            #endregion download File

            #region delete File
            #endregion delete File

        }

        private static async Task TryCreateAccountWithFileColumn(Service service) {

            byte[] fileBytes = File.ReadAllBytes("Files\\25mb.pdf");

            JObject account = new() {

                { "name", "Test account with file"},
                { "sample_filecolumn", Convert.ToBase64String(fileBytes) }

            };

            try
            {
                await service.Create("accounts", account);
            }
            catch (ServiceException se)
            {

                Console.WriteLine(se.Message);
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
            InitializeAnnotationBlocksUploadResponse initializeNoteUploadResponse =
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

            Console.WriteLine($"Test{fileSizeInBytes}");
            await service.Delete(createdAccount);

        }

        private static async Task UploadBlocks(Service service, string filePath, string fileContinuationToken, List<string> blockIds)
        {
            using Stream source = File.OpenRead(filePath);

            int blockSize = 4 * 1024 * 1024; // 4 MB

            byte[] buffer = new byte[blockSize];
            int bytesRead;
            FileInfo f = new(filePath);

            long fileSize = f.Length;
            int blocksCount = (int)Math.Ceiling(fileSize / (float)blockSize);
            int blockNumber = 0;

            while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                if (bytesRead < buffer.Length)
                {
                    Array.Resize(ref buffer, bytesRead);
                }

                blockNumber++;
                string blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(blockNumber.ToString().PadLeft(16, '0')));
                blockIds.Add(blockId);
                var blockData = new byte[buffer.Length];
                buffer.CopyTo(blockData, 0);

                UploadBlockRequest uploadBlockRequest = new(
                    blockId: blockId,
                    blockData: blockData,
                    fileContinuationToken: fileContinuationToken);

                await service.SendAsync(uploadBlockRequest);
            }
        }

        private static async Task TestFileUploadSample(Service service, Guid id)
        {

            string largeFileName = "25mb.pdf";
            string largeFilePath = $"Files\\{largeFileName}";
            string largeFileMimeType = "application/pdf";
            string smallFileName = "SampleExcelFile.xlsx";
            string smallFilePath = $"Files\\{smallFileName}";
            string smallFileMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            Uri urlPrefix = service.BaseAddress;
            string customEntitySetName = "accounts";
            string entityId = id.ToString();
            string entityFileOrAttributeAttributeLogicalName = "sample_filecolumn";
            string fileRootPath = "Files";
            string uploadFileName = largeFileName;

            var filePath = Path.Combine(fileRootPath, uploadFileName);
            var fileStream = File.OpenRead(filePath);
            var url = new Uri(urlPrefix, $"{customEntitySetName}({entityId})/{entityFileOrAttributeAttributeLogicalName}");

            using var request = new HttpRequestMessage(new HttpMethod("PATCH"), url);

            request.Content = new StreamContent(fileStream);
            request.Content.Headers.Add("Content-Type", "application/octet-stream");
            //request.Content.Headers.Add("Content-Type", "application/pdf");
            //request.Content.Headers.Add("Content-Type", smallFileMimeType);
            request.Content.Headers.Add("x-ms-file-name", uploadFileName);

            using var response = await service.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }


        private static async Task CreateAnnotationWithFile(Service service)
        {
            string largeFileName = "25mb.pdf";
            string largeFilePath = $"Files\\{largeFileName}";
            string largeFileMimeType = "application/pdf";
            string smallFileName = "SampleExcelFile.xlsx";
            string smallFilePath = $"Files\\{smallFileName}";
            string smallFileMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            byte[] fileBytes = File.ReadAllBytes(smallFilePath);
            string fileString = Convert.ToBase64String(fileBytes);



            JObject account = new() {

                { "name", "Test account with note"},
                { "Account_Annotation", new JArray{
                    {
                        new JObject{
                            {"subject","A note for the account record" },
                            {"notetext","<div data-wrapper=\"true\" style=\"font-size:9pt;font-family:'Segoe UI','Helvetica Neue',sans-serif;\"><div>Some text for the note.</div></div>" },
                            {"filename", smallFileName },
                            {"mimetype",smallFileMimeType},
                            {"documentbody",fileString}
                        }
                    }
                  }
               }
            };

            EntityReference createdAccount = await service.Create("accounts", account);

            JObject retrievedAccount = await service.Retrieve(createdAccount, "?$select=accountid&$expand=Account_Annotation($select=annotationid)");

            Guid annotationId = (Guid)retrievedAccount["Account_Annotation"][0]["annotationid"];

            //EntityReference annotationRef = new EntityReference("annotations", annotationId);

            UploadFileRequest uploadFileRequest = new(
                 entityReference: createdAccount,
                 columnName: "sample_filecolumn",
                 fileContent: File.OpenRead(largeFilePath),
                 fileName: largeFileName);

            await service.SendAsync(uploadFileRequest);

            //await TestFileUploadSample(service, createdAccount.Id.Value);

            Console.WriteLine("Test");



        }
    }
}