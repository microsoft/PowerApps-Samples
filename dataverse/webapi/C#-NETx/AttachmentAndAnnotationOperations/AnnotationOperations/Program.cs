using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Identity.Client;
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

            EntityReference accountRef = await service.Create("accounts", account);
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
                // This will be 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
                { "mimetype", Utility.GetMimeType(excelDoc)},
            };

            // Update the note
            await service.Update(noteRef, annotationForUpdate);

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

            // Upload large file
                int fileSizeInBytes = await UploadNote(
                    service: service,
                    annotationReference: noteRef,
                    fileInfo: pdfDoc) ;

                Console.WriteLine($"Uploaded {pdfDoc.Name} FileSizeInBytes: {fileSizeInBytes}");

            // Update the note with details about the uploaded file
            JObject updateNoteWithLargeFile = new() {
                    { "subject", "large PDF file" },
                    { "filename", pdfDoc.Name },
                    { "notetext", "Please see new attached pdf file." },
                    {"mimetype", Utility.GetMimeType(pdfDoc)},
            };
            await service.Update(entityReference: noteRef,record: updateNoteWithLargeFile);

            //Delete account, which will delete all notes associated with it
            await service.Delete(accountRef);
            Console.WriteLine("Deleted the account record.");

            // Return MaxUploadFileSize to the original value
            await Utility.SetMaxUploadFileSize(service, originalMaxUploadFileSize);

            Console.WriteLine($"Current MaxUploadFileSize: {await Utility.GetMaxUploadFileSize(service)}");
        }

        static async Task<int> UploadNote(
            Service service, 
            EntityReference annotationReference, 
            FileInfo fileInfo,
            string? fileMimeType = null) 
        {
            // Initialize the upload
            InitializeAnnotationBlocksUploadRequest initializeAnnotationBlocksUploadRequest = new(
                target: annotationReference);

            InitializeAnnotationBlocksUploadResponse initializeAnnotationBlocksUploadResponse =
                await service.SendAsync<InitializeAnnotationBlocksUploadResponse>(initializeAnnotationBlocksUploadRequest);
            string fileContinuationToken = initializeAnnotationBlocksUploadResponse.FileContinuationToken;

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

            // Commit the upload
            CommitAnnotationBlocksUploadRequest commitAnnotationBlocksUploadRequest = new(
                target: annotationReference, 
                blockList: blockIds,
                fileContinuationToken: fileContinuationToken);

            CommitAnnotationBlocksUploadResponse commitAnnotationBlocksUploadResponse =
                await service.SendAsync<CommitAnnotationBlocksUploadResponse>(commitAnnotationBlocksUploadRequest);

            return commitAnnotationBlocksUploadResponse.FileSizeInBytes;
        }
    }
}