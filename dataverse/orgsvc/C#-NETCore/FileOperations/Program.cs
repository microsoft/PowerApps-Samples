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

            List<EntityReference> recordsToDelete = new();
            bool deleteCreatedRecords = true;

            Console.WriteLine("--Starting File Operations sample--");

            #region upload Note

            #region create account with note


            string filePath = "Files\\25mb.pdf";
            string fileMimeType = "application/pdf";

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


            Console.WriteLine($"Test{fileSizeInBytes}");




            #endregion upload file to note

            #endregion upload Note

            #region download Note

            #endregion download Note

            #region delete Note

            await service.Delete(createdAccount);
            #endregion delete Note

            #region upload Attachment
            #endregion upload Attachment

            #region download Attachment
            #endregion download Attachment

            #region delete Attachment
            #endregion delete Attachment

            #region upload File
            #endregion upload 

            #region download File
            #endregion download File

            #region delete File
            #endregion delete File

        }

        private static async Task UploadBlocks(Service service, string filePath, string fileContinuationToken, List<string> blockIds)
        {
            using (Stream source = File.OpenRead(filePath))
            {

                int blockSize = 4 * 1024 * 1024; // 4 MB

                byte[] buffer = new byte[blockSize];
                int bytesRead;
                FileInfo f = new FileInfo(filePath);

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
        }
    }
}