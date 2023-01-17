using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using System.Text;

namespace AttachmentAndAnnotationOperations
{
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
            FileInfo wordDoc = new("Files/WordDoc.docx");
            FileInfo excelDoc = new("Files/ExcelDoc.xlsx");

            Program app = new();

            // Create a Dataverse service client using the default connection string.
            ServiceClient serviceClient =
                new(app.Configuration.GetConnectionString("default"));

            serviceClient.UseWebApi = false;

            int maxuploadfilesize = Utility.GetMaxUploadFileSize(serviceClient);
            Console.WriteLine($"MaxUploadFileSize: {maxuploadfilesize}");

            Utility.SetMaxUploadFileSize(serviceClient, 131072000);

            Console.WriteLine($"MaxUploadFileSize: {Utility.GetMaxUploadFileSize(serviceClient)}");

            Utility.SetMaxUploadFileSize(serviceClient, 5120000);

            Console.WriteLine($"MaxUploadFileSize: {Utility.GetMaxUploadFileSize(serviceClient)}");

            // Create account
            Entity account = new("account")
            {
                Attributes =
                {
                   { "name","Test Account" }
                }
            };

            Guid accountid = serviceClient.Create(account);

            // Create Note
            Entity note = new("annotation")
            {
                Attributes =
                {
                    { "subject", "Example Note" },
                    { "filename", wordDoc.Name },
                    // Byte[] value can be set directly using late-bound style when serviceClient.UseWebApi = true;
                    { "documentbody", Convert.ToBase64String(File.ReadAllBytes(wordDoc.FullName))},
                    { "notetext", "Please see attached file." },
                    // mimetype is optional. Will be set to "application/octet-stream" if not specified.
                    // This will be 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
                    { "mimetype", Utility.GetMimeType(wordDoc)},
                    // Associate with the account
                    { "objectid", new EntityReference("account", accountid) }
                }
            };

            // Create annotation
            Guid annotationid = serviceClient.Create(note);



            // Retrieve the note
            Entity retrievedNote = serviceClient.Retrieve(
                entityName: "annotation",
                id: annotationid,
                columnSet: new ColumnSet("documentbody", "mimetype"));

            // mimetype uses the value saved: 
            Console.WriteLine($"mimetype: {retrievedNote["mimetype"]}");

            //Save the file
            File.WriteAllBytes("DownloadedWordDoc.docx", Convert.FromBase64String((string)retrievedNote["documentbody"]));

            // File to update
            Entity updateNoteWithSmallFile = new("annotation")
            {
                Attributes =
                {
                    { "annotationid", annotationid },
                    { "filename", excelDoc.Name },
                    { "documentbody", Convert.ToBase64String(File.ReadAllBytes(excelDoc.FullName))},
                    { "notetext", "Please see new attached file." },
                    // This will be 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
                    { "mimetype", Utility.GetMimeType(excelDoc)},
                }
            };

            serviceClient.Update(updateNoteWithSmallFile);

            //// File to update
            //Entity updateNoteWithLargeFile = new("annotation")
            //{
            //    Attributes =
            //    {
            //        { "annotationid", annotationid },
            //        { "filename", excelDoc.Name },
            //        { "notetext", "Please see new attached file." },
            //        // This will be 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
            //        { "mimetype", Utility.GetMimeType(excelDoc)},
            //    }
            //};


            //// Can only be used for existing Note. Target Must contain annotationid
            //int fileSizeInBytes = UploadNote(serviceClient, updateNoteWithLargeFile, excelDoc);
            //Console.WriteLine($"FileSizeInBytes: {fileSizeInBytes}");



            // Retrieve the note
            Entity retrievedNote2 = serviceClient.Retrieve(
                entityName: "annotation",
                id: annotationid,
                columnSet: new ColumnSet("documentbody", "mimetype"));

            // mimetype uses the value saved: 
            Console.WriteLine($"mimetype: {retrievedNote2["mimetype"]}");

            //Download the file
            var (bytes, fileName) = DownloadNote(service: serviceClient, retrievedNote2.ToEntityReference());

            File.WriteAllBytes($"Downloaded{fileName}", bytes);
            
            //Delete account
            serviceClient.Delete("account", accountid);

        }

        /// <summary>
        /// Uploads an annotation record with file.
        /// </summary>
        /// <param name="service">The IOrganizationService to use.</param>
        /// <param name="annotation">The data to update for an existing annotation record.</param>
        /// <param name="fileInfo">A reference to the file to upload.</param>
        /// <param name="fileMimeType">The mimetype for the file, if known.</param>
        /// <returns>The FileSizeInBytes</returns>
        static int UploadNote(
                IOrganizationService service,
                Entity annotation,
                FileInfo fileInfo,
                string fileMimeType = null)
        {

            if (annotation.LogicalName != "annotation")
            {
                throw new ArgumentException("The annotation parameter must be an annotation entity", nameof(annotation));
            }
            if (!annotation.Attributes.Contains("annotationid") || annotation.Id == Guid.Empty) {

                throw new ArgumentException("The annotation parameter must include a valid annotationid value.", nameof(annotation));
            }


            // Initialize the upload
            InitializeAnnotationBlocksUploadRequest initializeAnnotationBlocksUploadRequest = new()
            {
                Target = annotation
            };

            var initializeAnnotationBlocksUploadResponse =
                (InitializeAnnotationBlocksUploadResponse)service.Execute(initializeAnnotationBlocksUploadRequest);

            string fileContinuationToken = initializeAnnotationBlocksUploadResponse.FileContinuationToken;

            // Capture blockids while uploading
            List<string> blockIds = new();

            using Stream uploadFileStream = fileInfo.OpenRead();

            int blockSize = 4 * 1024 * 1024; // 4 MB

            byte[] buffer = new byte[blockSize];
            int bytesRead = 0;

            long fileSize = fileInfo.Length;

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

                string blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));

                blockIds.Add(blockId);

                // Copy the next block of data to send.
                var blockData = new byte[buffer.Length];
                buffer.CopyTo(blockData, 0);

                // Prepare the request
                UploadBlockRequest uploadBlockRequest = new()
                {
                    BlockData = blockData,
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
            CommitAnnotationBlocksUploadRequest commitAnnotationBlocksUploadRequest = new()
            {
                BlockList = blockIds.ToArray(),
                FileContinuationToken = fileContinuationToken,
                Target = annotation
            };

            var commitFileBlocksUploadResponse =
                (CommitAnnotationBlocksUploadResponse)service.Execute(commitAnnotationBlocksUploadRequest);

            return commitFileBlocksUploadResponse.FileSizeInBytes;

        }

        /// <summary>
        /// Downloads the documentbody of an annotation.
        /// </summary>
        /// <param name="service">The IOrganizationService to use.</param>
        /// <param name="target">A reference to the annotation record that has the file.</param>
        /// <returns></returns>
        static (byte[] bytes, string fileName) DownloadNote(IOrganizationService service, EntityReference target) {

            if (target.LogicalName != "annotation") { 
                throw new ArgumentException("The target parameter must refer to an annotation record", nameof(target));
            }

            InitializeAnnotationBlocksDownloadRequest initializeAnnotationBlocksDownloadRequest = new()
            {
                Target = target
            };

            var initializeAnnotationBlocksDownloadResponse =
                (InitializeAnnotationBlocksDownloadResponse)service.Execute(initializeAnnotationBlocksDownloadRequest);

            string fileContinuationToken = initializeAnnotationBlocksDownloadResponse.FileContinuationToken;
            int fileSizeInBytes = initializeAnnotationBlocksDownloadResponse.FileSizeInBytes;
            string fileName = initializeAnnotationBlocksDownloadResponse.FileName;

            List<byte> fileBytes = new();

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

            return (fileBytes.ToArray(), fileName);

        }
    }
}