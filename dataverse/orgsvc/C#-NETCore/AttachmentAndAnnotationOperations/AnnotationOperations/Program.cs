using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Text;

namespace PowerPlatform.Dataverse.CodeSamples
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
            path ??= "appsettings.json";

            // Load the app's configuration settings from the JSON file.
            Configuration = new ConfigurationBuilder()
                .AddJsonFile(path, optional: false, reloadOnChange: true)
                .Build();
        }


        static void Main()
        {
            // Files used in this sample
            FileInfo wordDoc = new("Files/WordDoc.docx");
            FileInfo excelDoc = new("Files/ExcelDoc.xlsx");
            FileInfo pdfDoc = new("Files/25mb.pdf");


            Program app = new();

            // Create a Dataverse service client using the default connection string.
            ServiceClient serviceClient =
                new(app.Configuration.GetConnectionString("default"))
                {
                    UseWebApi = false
                };

            // Get current MaxUploadFileSize
            int originalMaxUploadFileSize = Utility.GetMaxUploadFileSize(serviceClient);
            Console.WriteLine($"Current MaxUploadFileSize: {originalMaxUploadFileSize}");

            // Create account to associate note with.
            Entity account = new("account")
            {
                Attributes =
                {
                   { "name","Test Account for AnnotationOperations" }
                }
            };

            Guid accountid = serviceClient.Create(entity: account); // To delete later
            Console.WriteLine("Created an account record to associate notes with.");

            // Create note
            Entity note = new("annotation")
            {
                Attributes =
                {
                    { "subject", "Example Note" },
                    { "filename", wordDoc.Name },
                    { "documentbody", Convert.ToBase64String(File.ReadAllBytes(wordDoc.FullName))},
                    { "notetext", "Please see attached file." },
                    // mimetype is optional. Will be set to "application/octet-stream" if not specified.
                    // This will be 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
                    { "mimetype", Utility.GetMimeType(wordDoc)},
                    // Associate with the account
                    { "objectid", new EntityReference("account", accountid) }
                }
            };

            // Create note
            Guid annotationid = serviceClient.Create(entity:note);
            Console.WriteLine($"Created note with attached Word document.");


            // Retrieve the note
            Entity retrievedNote = serviceClient.Retrieve(
                entityName: "annotation",
                id: annotationid,
                columnSet: new ColumnSet("documentbody", "mimetype", "filename"));

            Console.WriteLine($"\tRetrieved note with attached Word document.");

            //Save the file
            File.WriteAllBytes(
                path: $"Downloaded{retrievedNote["filename"]}",
                bytes: Convert.FromBase64String((string)retrievedNote["documentbody"]));

            Console.WriteLine($"\tSaved the Word document to \\bin\\Debug\\net6.0\\Downloaded{retrievedNote["filename"]}.");

            // File to update
            Entity annotationForUpdate = new("annotation")
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

            // Update the note
            serviceClient.Update(annotationForUpdate);

            Console.WriteLine($"Updated note with attached Excel document.");

            // Retrieve the note
            Entity retrievedUpdatedNote = serviceClient.Retrieve(
                entityName: "annotation",
                id: annotationid,
                columnSet: new ColumnSet("documentbody", "filename"));

            Console.WriteLine($"\tRetrieved note with attached Excel document.");

            //Save the file
            File.WriteAllBytes(
                path: $"Downloaded{(string)retrievedUpdatedNote["filename"]}",
                bytes: Convert.FromBase64String((string)retrievedUpdatedNote["documentbody"]));

            Console.WriteLine($"\tSaved the Excel document to \\bin\\Debug\\net6.0\\Downloaded{retrievedUpdatedNote["filename"]}.");

            // Set MaxUploadFileSize to the maximum value
            Utility.SetMaxUploadFileSize(serviceClient, 131072000);

            Console.WriteLine($"Updated MaxUploadFileSize to: {Utility.GetMaxUploadFileSize(serviceClient)}");


            // Note to update with large file
            Entity updateNoteWithLargeFile = new("annotation")
            {
                Attributes =
                {
                    { "annotationid", annotationid }, // Required
                    { "subject", "large PDF file" },
                    { "filename", pdfDoc.Name },
                    { "notetext", "Please see new attached pdf file." },
                    { "mimetype", Utility.GetMimeType(pdfDoc)}
                    // Don't include documentbody
                }
            };

            Console.WriteLine($"Uploading {pdfDoc.Name}...");

            // Upload large file
            CommitAnnotationBlocksUploadResponse uploadNoteResponse = UploadNote(
            service: serviceClient,
            annotation: updateNoteWithLargeFile,
            fileInfo: pdfDoc);

            Console.WriteLine($"\tUploaded {pdfDoc.Name} " +
                $"\n\t\tAnnotationId: {uploadNoteResponse.AnnotationId} " +
                $"\n\t\tFileSizeInBytes: {uploadNoteResponse.FileSizeInBytes}");

            //Download the large file
            (byte[] bytes, string fileName) = DownloadNote(
                service: serviceClient,
                target: retrievedUpdatedNote.ToEntityReference());

            File.WriteAllBytes($"Downloaded{fileName}", bytes);
            Console.WriteLine($"\tSaved the PDF document to \\bin\\Debug\\net6.0\\Downloaded{fileName}.");

            //Clean up

            //Delete account, which will delete all notes associated with it
            serviceClient.Delete("account", accountid);
            Console.WriteLine("Deleted the account record.");

            // Return MaxUploadFileSize to the original value
            Utility.SetMaxUploadFileSize(serviceClient, originalMaxUploadFileSize);

            Console.WriteLine($"Current MaxUploadFileSize: {Utility.GetMaxUploadFileSize(serviceClient)}");

        }

        /// <summary>
        /// Uploads an note record and updates annotation.
        /// </summary>
        /// <param name="service">The IOrganizationService to use.</param>
        /// <param name="annotation">The data to update for an existing note record.</param>
        /// <param name="fileInfo">A reference to the file to upload.</param>
        /// <param name="fileMimeType">The mimetype for the file, if known.</param>
        /// <returns>Tuple AnnotationId and FileSizeInBytes</returns>
        static CommitAnnotationBlocksUploadResponse UploadNote(
                IOrganizationService service,
                Entity annotation,
                FileInfo fileInfo,
                string? fileMimeType = null)
        {

            if (annotation.LogicalName != "annotation")
            {
                throw new ArgumentException(
                    message: "The annotation parameter must be an annotation entity",
                    paramName: nameof(annotation));
            }
            if (!annotation.Attributes.Contains("annotationid") || annotation.Id != Guid.Empty)
            {
                throw new ArgumentException(
                    message: "The annotation parameter must include a valid annotationid value.",
                    paramName: nameof(annotation));
            }

            // documentbody value in annotation not needed. Remove if found.
            if (annotation.Contains("documentbody"))
            {
                annotation.Attributes.Remove("documentbody");
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
            // Don't override what might be included in the annotation.
            if (!annotation.Contains("mimetype")) {
                annotation["mimetype"] = fileMimeType;
            }
           
            // Initialize the upload
            InitializeAnnotationBlocksUploadRequest initializeRequest = new()
            {
                Target = annotation
            };

            var initializeResponse =
                (InitializeAnnotationBlocksUploadResponse)service.Execute(initializeRequest);

            string fileContinuationToken = initializeResponse.FileContinuationToken;

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
                // Generates base64 string blockId values based on a Guid value so they are always the same length.
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

            // Commit the upload
            CommitAnnotationBlocksUploadRequest commitRequest = new()
            {
                BlockList = blockIds.ToArray(),
                FileContinuationToken = fileContinuationToken,
                Target = annotation
            };
         
              return  (CommitAnnotationBlocksUploadResponse)service.Execute(commitRequest);
        }

        /// <summary>
        /// Downloads the documentbody and filename of an note.
        /// </summary>
        /// <param name="service">The IOrganizationService to use.</param>
        /// <param name="target">A reference to the note record that has the file.</param>
        /// <returns>Tuple containing bytes and filename.</returns>
        static (byte[] bytes, string fileName) DownloadNote(
            IOrganizationService service,
            EntityReference target)
        {
            if (target.LogicalName != "annotation")
            {
                throw new ArgumentException("The target parameter must refer to an note record.", nameof(target));
            }

            InitializeAnnotationBlocksDownloadRequest initializeRequest = new()
            {
                Target = target
            };

            var response =
                (InitializeAnnotationBlocksDownloadResponse)service.Execute(initializeRequest);

            string fileContinuationToken = response.FileContinuationToken;
            int fileSizeInBytes = response.FileSizeInBytes;
            string fileName = response.FileName;

            List<byte> fileBytes = new(fileSizeInBytes);

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