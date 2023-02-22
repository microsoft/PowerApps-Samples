using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json.Linq;
using PowerApps.Samples.Messages;
using PowerApps.Samples.Methods;
using System.Text;

namespace PowerApps.Samples
{
    internal class Program
    {
        static async Task Main()
        {
            // Files used in this sample
            FileInfo wordDoc = new("Files/WordDoc.docx");
            FileInfo excelDoc = new("Files/ExcelDoc.xlsx");
            FileInfo pdfDoc = new("Files/25mb.pdf");

            Config config = App.InitializeApp();

            var service = new Service(config);

            // Get current MaxUploadFileSize
            int originalMaxUploadFileSize = await Utility.GetMaxUploadFileSize(service);
            Console.WriteLine($"Current MaxUploadFileSize: {originalMaxUploadFileSize}");

            // Create account to associate note with.
            JObject account = new() {
                {"name","Test Account for AnnotationOperations" }
            };

            EntityReference accountRef = await service.Create(
                entitySetName: "accounts",
                record: account);// To delete later
            Console.WriteLine("Created an account record to associate notes with.");

            // Create note
            JObject note = new() {
                    { "subject", "Example Note" },
                    { "filename", wordDoc.Name },
                    { "documentbody", Convert.ToBase64String(File.ReadAllBytes(wordDoc.FullName))},
                    { "notetext", "Please see attached file." },
                    // mimetype is optional. Will be set to "application/octet-stream" if not specified.
                    // This will be 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
                    { "mimetype", Utility.GetMimeType(wordDoc)},
                // Associate with the account
                    { "objectid_account@odata.bind", accountRef.Path }
            };

            EntityReference noteRef = await service.Create("annotations", note);
            Console.WriteLine($"Created note with attached Word document.");

            // Retrieve the note
            JObject retrievedNote = await service.Retrieve(
                entityReference: noteRef,
                query: "?$select=documentbody,mimetype,filename");

            Console.WriteLine($"\tRetrieved note with attached Word document.");

            //Save the file
            File.WriteAllBytes(
                path: $"Downloaded{retrievedNote["filename"]}",
                bytes: Convert.FromBase64String((string)retrievedNote["documentbody"]));

            Console.WriteLine($"\tSaved the Word document to \\bin\\Debug\\net6.0\\Downloaded{retrievedNote["filename"]}.");

            // File to update
            JObject annotationForUpdate = new()
            {
                { "annotationid", noteRef.Id },
                { "filename", excelDoc.Name },
                { "documentbody", Convert.ToBase64String(File.ReadAllBytes(excelDoc.FullName))},
                { "notetext", "Please see new attached file." },
                // mimetype will be 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
                { "mimetype", Utility.GetMimeType(excelDoc)},
            };

            // Update the note
            await service.Update(entityReference: noteRef, record: annotationForUpdate);

            Console.WriteLine($"Updated note with attached Excel document.");

            // Retrieve the note
            JObject retrievedUpdatedNote = await service.Retrieve(noteRef, "?$select=documentbody,filename");

            Console.WriteLine($"\tRetrieved note with attached Excel document.");

            //Save the file
            File.WriteAllBytes(
                path: $"Downloaded{retrievedUpdatedNote["filename"]}",
                bytes: Convert.FromBase64String((string)retrievedUpdatedNote["documentbody"]));

            Console.WriteLine($"\tSaved the Excel document to \\bin\\Debug\\net6.0\\Downloaded{retrievedUpdatedNote["filename"]}.");


            // Set MaxUploadFileSize to the maximum value
            await Utility.SetMaxUploadFileSize(service, 131072000);

            Console.WriteLine($"Updated MaxUploadFileSize to: {await Utility.GetMaxUploadFileSize(service)}");

            // Note to update with large file
            JObject updateNoteWithLargeFile = new() {
                    { "annotationid", noteRef.Id }, // Required
                    { "subject", "large PDF file" },
                    { "filename", pdfDoc.Name },
                    { "notetext", "Please see new attached pdf file." },
                    { "mimetype", Utility.GetMimeType(pdfDoc)}
                // Don't include documentbody
            };

            Console.WriteLine($"Uploading {pdfDoc.Name}...");

            // Upload large file
            CommitAnnotationBlocksUploadResponse uploadNoteResponse = await UploadNote(
                    service: service,
                    annotation: updateNoteWithLargeFile,
                    fileInfo: pdfDoc);

            Console.WriteLine($"Uploaded {pdfDoc.Name} " +
                $"\n\tAnnotationId: {uploadNoteResponse.AnnotationId} " +
                $"\n\tFileSizeInBytes: {uploadNoteResponse.FileSizeInBytes}");

            //Download the large file in chunks
            var (bytes, fileName) = await DownloadNote(
                service: service,
                target: noteRef);

            File.WriteAllBytes($"Downloaded{fileName}", bytes);
            Console.WriteLine($"\tSaved the PDF document to \\bin\\Debug\\net6.0\\Downloaded{fileName}.");

            // Download the file in a single request
            DownloadAnnotationFileRequest downloadFileRequest = new(annotationId: noteRef.Id.Value);

            var downloadFileResponse = 
                await service.SendAsync<DownloadAnnotationFileResponse>(downloadFileRequest);

            File.WriteAllBytes($"DownloadedAgain{fileName}", downloadFileResponse.File);
            Console.WriteLine($"\tSaved the PDF document to \\bin\\Debug\\net6.0\\DownloadedAgain{fileName}.");


            //Clean up

            //Delete account, which will delete all notes associated with it
            await service.Delete(accountRef);
            Console.WriteLine("Deleted the account record.");

            // Return MaxUploadFileSize to the original value
            await Utility.SetMaxUploadFileSize(service, originalMaxUploadFileSize);

            Console.WriteLine($"Current MaxUploadFileSize: {await Utility.GetMaxUploadFileSize(service)}");
        }

        /// <summary>
        /// Uploads an note record and updates annotation.
        /// </summary>
        /// <param name="service">The WebAPIService to use.</param>
        /// <param name="annotation">The data to update for an existing note record.</param>
        /// <param name="fileInfo">A reference to the file to upload.</param>
        /// <returns>The FileSizeInBytes</returns>
        static async Task<CommitAnnotationBlocksUploadResponse> UploadNote(
            Service service,
            JObject annotation,
            FileInfo fileInfo,
            string? fileMimeType = null)
        {

            if (!annotation.ContainsKey("@odata.type"))
            {
                annotation.Add("@odata.type", "Microsoft.Dynamics.CRM.annotation");
            }

            // documentbody value in annotation not needed. Remove if found.
            if (annotation.ContainsKey("documentbody"))
            {
                annotation.Remove("documentbody");
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
            if (!annotation.ContainsKey("mimetype"))
            {
                annotation.Add("mimetype", fileMimeType);
            }

            // Initialize the upload
            InitializeAnnotationBlocksUploadRequest initializeRequest = new(
                target: annotation);

            var initializeResponse =
                await service.SendAsync<InitializeAnnotationBlocksUploadResponse>(
                    request: initializeRequest);

            string fileContinuationToken =
                initializeResponse.FileContinuationToken;

            // Capture blockids while uploading
            List<string> blockIds = new();

            using Stream file = fileInfo.OpenRead();

            int blockSize = 4 * 1024 * 1024; // 4 MB

            byte[] buffer = new byte[blockSize];
            int bytesRead = 0;

            long fileSize = fileInfo.Length;

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
                // Generates base64 string blockId values based on a Guid value so they are always the same length.
                string blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));

                // List<string> is a reference type, so this will update the value passed into the function.
                blockIds.Add(blockId);

                // Prepare the request
                UploadBlockRequest uploadBlockRequest = new(
                    blockId: blockId,
                    blockData: buffer,
                    fileContinuationToken: fileContinuationToken);

                // Send the request
                await service.SendAsync(uploadBlockRequest);
            }

            // Commit the upload
            CommitAnnotationBlocksUploadRequest commitRequest = new(
                target: annotation,
                blockList: blockIds,
                fileContinuationToken: fileContinuationToken);

            return await service.SendAsync<CommitAnnotationBlocksUploadResponse>(commitRequest);
        }

        /// <summary>
        /// Downloads the documentbody and filename of an note.
        /// </summary>
        /// <param name="service">The WebAPIService to use.</param>
        /// <param name="target">A reference to the note record that has the file.</param>
        /// <returns>Tuple containing bytes and filename.</returns>
        static async Task<(byte[] bytes, string fileName)> DownloadNote(
            Service service,
            EntityReference target)
        {
            InitializeAnnotationBlocksDownloadRequest initializeRequest = new(target: target);

            var response =
                await service.SendAsync<InitializeAnnotationBlocksDownloadResponse>(request: initializeRequest);

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
                DownloadBlockRequest downLoadBlockRequest = new(
                    offset: offset,
                    blockLength: blockSizeDownload, fileContinuationToken: fileContinuationToken);


                // Send the request
                var downloadBlockResponse =
                           await service.SendAsync<DownloadBlockResponse>(request: downLoadBlockRequest);

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