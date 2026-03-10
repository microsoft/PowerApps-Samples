using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json.Linq;
using PowerApps.Samples.Messages;
using PowerApps.Samples.Methods;
using System.Text;
using System.Xml.Linq;

namespace PowerApps.Samples
{
    internal class Program
    {
        static async Task Main()
        {
            // Files used in this sample
            FileInfo wordDoc = new("Files/WordDoc.docx");
            FileInfo excelDoc = new("Files/ExcelDoc.xlsx");
            FileInfo pdfDoc = new("Files/25mb.pdf");// A large file over default size

            List<FileInfo> smallFiles = new() { wordDoc, excelDoc };
            List<FileInfo> allFiles = new() { wordDoc, excelDoc, pdfDoc };

            List<(string FileName, EntityReference ActivityMimeAttachmentRef)> reusableAttachments = new();

            // Get the configuration data from the app
            Config config = App.InitializeApp();
            // Create a WebAPIService service client using the configuration data
            var service = new Service(config);

            // Get current MaxUploadFileSize
            int originalMaxUploadFileSize = await Utility.GetMaxUploadFileSize(service);
            Console.WriteLine($"Current MaxUploadFileSize: {originalMaxUploadFileSize}");

            #region Create single-use attachments

            Console.WriteLine("Start: Create single-use attachments");

            // Create email activity
            JObject email = new() {
                {"subject", "This is an example email." }
            };

            EntityReference emailRef = await service.Create(
                entitySetName: "emails",
                record: email);

            Console.WriteLine("Created an email activity.");

            // Attach the small files to the email directly
            smallFiles.ForEach(smallFile => {

                JObject attachment = new() {
                        { "objectid_email@odata.bind", emailRef.Path},
                        { "objecttypecode", "email" },
                        { "subject", $"Sample attached {smallFile.Name}" },
                        { "body", Convert.ToBase64String(File.ReadAllBytes(smallFile.FullName)) },
                        { "filename", smallFile.Name},
                        { "mimetype", Utility.GetMimeType(smallFile)}
                };

                // Create synchronously while in loop.
                service.Create(
                    entitySetName: "activitymimeattachments",
                    record: attachment).GetAwaiter().GetResult();
            });

            Console.WriteLine("Created two e-mail attachments with small files for the e-mail activity.");

            // Set MaxUploadFileSize to the maximum value
            await Utility.SetMaxUploadFileSize(service, 131072000);

            Console.WriteLine($"Updated MaxUploadFileSize to: {await Utility.GetMaxUploadFileSize(service)}");

            // Prepare data for a large attachment
            JObject largeAttachment = new() {
                { "objectid_email@odata.bind", emailRef.Path},
                { "objecttypecode", "email" },
                { "subject", $"Sample attached {pdfDoc.Name}" },
                // Do not set the body
                { "filename", pdfDoc.Name},
                { "mimetype", Utility.GetMimeType(pdfDoc)}
            };

            Console.WriteLine($"Adding {pdfDoc.Name}...");

            // Creates the activitymimeattachment record with a file.
            CommitAttachmentBlocksUploadResponse uploadAttachmentResponse = await UploadAttachment(
                service: service,
                attachment: largeAttachment,
                fileInfo: pdfDoc);

            Console.WriteLine($"\tUploaded {pdfDoc.Name} as attachment. " +
                $"\n\t\tActivityMimeAttachmentId:{uploadAttachmentResponse.ActivityMimeAttachmentId} \n\t\tFileSizeInBytes: {uploadAttachmentResponse.FileSizeInBytes}");

            // Retrieve information about the attachments related to the email
            // See https://learn.microsoft.com/power-apps/developer/data-platform/webapi/retrieve-entity-using-web-api#retrieve-navigation-property-values
            RetrieveMultipleResponse relatedAttachmentsResponse = await service.RetrieveMultiple(
                queryUri: $"{emailRef.Path}/activity_pointer_activity_mime_attachment?$select=filename");

            Console.WriteLine("Download attached files:");

            foreach (JObject attachment in relatedAttachmentsResponse.Records.Cast<JObject>())
            {
                string filename = (string)attachment["filename"];
                Console.WriteLine($"\tDownloading filename: {filename}...");

                EntityReference attachmentRef = new(
                    entitySetName: "activitymimeattachments",
                    id: (Guid)attachment["activitymimeattachmentid"]);

                // Download synchronously while in loop.
                // Using blocks with Web API actions
                var (bytes, name) = DownloadAttachment(
                    service: service,
                    target: attachmentRef).GetAwaiter().GetResult();

                File.WriteAllBytes($"Downloaded{name}", bytes);
                Console.WriteLine($"\tSaved the attachment to \\bin\\Debug\\net6.0\\Downloaded{name}.");
            }

            // Download an attachment file with a single request

            DownloadAttachmentFileRequest downloadAttachmentFileRequest = new(activitymimeattachmentId: uploadAttachmentResponse.ActivityMimeAttachmentId);
            var downloadAttachmentFileResponse = await service.SendAsync<DownloadAttachmentFileResponse>(request: downloadAttachmentFileRequest);

            File.WriteAllBytes($"DownloadedAgain{pdfDoc.Name}", downloadAttachmentFileResponse.File);
            Console.WriteLine($"Saved the attachment to \\bin\\Debug\\net6.0\\DownloadedAgain{pdfDoc.Name}.");


            // Delete the email activity and the attachments will be deleted as well
            await service.Delete(emailRef);

            #endregion Create single-use attachments

            #region Create re-usable attachments

            Console.WriteLine("\nStart: Create re-usable attachments");

            // Create an email template to add the re-usable attachments to.
            // ActivityMimeAttachment ObjectId and ObjectTypeCode are SystemRequired.

            JObject template = new() {

                    { "body", "<?xml version=\"1.0\" ?>" +
                        "<xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\">" +
                        "<xsl:output method=\"text\" indent=\"no\"/>" +
                            "<xsl:template match=\"/data\">" +
                                "<![CDATA[<div>Text for the example account template.</div>]]>" +
                            "</xsl:template>" +
                        "</xsl:stylesheet>" },
                    { "description", "The description of the Example Account Template" },
                    { "ispersonal", false }, //Organization
                    { "languagecode", 1033 }, //English
                    { "presentationxml", "<template>" +
                            "<text>" +
                                "<![CDATA[<div>Text for the example account template.</div>]]>" +
                            "</text>" +
                        "</template>" },
                    { "safehtml", "<div>Text for the example account template.</div>\n" },
                    { "subject", "<?xml version=\"1.0\" ?>" +
                        "<xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\">" +
                        "<xsl:output method=\"text\" indent=\"no\"/>" +
                            "<xsl:template match=\"/data\">" +
                                "<![CDATA[Example Account Template Subject]]>" +
                            "</xsl:template>" +
                        "</xsl:stylesheet>" },
                    { "subjectpresentationxml", "<template>" +
                            "<text>" +
                                "<![CDATA[Example Account Template Subject]]>" +
                            "</text>" +
                        "</template>" },
                    { "subjectsafehtml", "Example Account Template Subject" },
                    { "templatetypecode", "account" },
                    { "title", "Example Account Template" }
            };

            // Create the template
            EntityReference templateRef = await service.Create("templates", template);

            Console.WriteLine("Created an email template.");

            // Add all files (large and small) as attachments to the template in the same way.
            allFiles.ForEach(file => {

                JObject attachment = new() {
                        { "objectid_template@odata.bind", templateRef.Path},
                        { "objecttypecode", "template" },
                        { "subject", $"Reusable attachment {file.Name}" },
                        // Does not include the body.
                        { "filename", file.Name},
                        { "mimetype", Utility.GetMimeType(file)}
                };

                // Create the attachment with upload synchronously while in loop.
                CommitAttachmentBlocksUploadResponse uploadAttachmentResponse2 = UploadAttachment(
                       service: service,
                       attachment: attachment,
                       fileInfo: file).GetAwaiter().GetResult();

                reusableAttachments.Add((FileName: file.Name,
                        ActivityMimeAttachmentRef: new EntityReference(
                            entitySetName: "activitymimeattachments",
                            id: uploadAttachmentResponse2.ActivityMimeAttachmentId)));

                Console.WriteLine($"\tAdded {file.Name} to the email template.");

            });

            Console.WriteLine("Added all files as attachment to email template.");

            // Create new email to re-use attachments from Template
            JObject email2 = new()
            {
                {"subject", "This is an example email with re-used attachments." }
            };

            EntityReference email2Ref = await service.Create("emails",email2);

            Console.WriteLine("Created a second email activity.");


            foreach ((string FileName, EntityReference ActivityMimeAttachmentRef) in reusableAttachments)
            {
                JObject attachment = new()
                {

                        { "objectid_email@odata.bind", email2Ref.Path},
                        { "objecttypecode", "email" },
                        { "subject", $"Sample attached {FileName}" },
                        { "attachmentid@odata.bind", ActivityMimeAttachmentRef.Path } // Only set attachmentid
                        // Do not set body, filename, or mimetype

                };

                await service.Create("activitymimeattachments", attachment);

                Console.WriteLine($"\tAttached {FileName} to the second email");

            }

            // Delete the second email

            await service.Delete(email2Ref);
            Console.WriteLine($"Deleted the second email.");

            // Verify the re-used attachments still exist
            foreach ((string FileName, EntityReference ActivityMimeAttachmentRef) in reusableAttachments) {

                JObject attachment = await service.Retrieve(ActivityMimeAttachmentRef, "?$select=filename");

                if ((string)attachment["filename"] == FileName)
                {
                    Console.WriteLine($"\tAttachment for {FileName} still exists.");
                }

            }

            // Clean up


            // Delete the template
            await service.Delete(templateRef); //Will delete re-usable attachments

            #endregion Create re-usable attachments

            // Return MaxUploadFileSize to the original value
            await Utility.SetMaxUploadFileSize(service, originalMaxUploadFileSize);

            Console.WriteLine($"Current MaxUploadFileSize: {await Utility.GetMaxUploadFileSize(service)}");
        }

        /// <summary>
        /// Creates an activitymimeattachment with file.
        /// </summary>
        /// <param name="service">The WebAPIService instance to use.</param>
        /// <param name="attachment">The activitymimeattachment data to create.</param>
        /// <param name="fileInfo">A reference to the file to upload.</param>
        /// <param name="fileMimeType">The mimetype of the file.</param>
        /// <returns>CommitAttachmentBlocksUploadResponse containing ActivityMimeAttachmentId and FileSizeInBytes.</returns>
        static async Task<CommitAttachmentBlocksUploadResponse> UploadAttachment(
        Service service,
        JObject attachment,
        FileInfo fileInfo,
        string? fileMimeType = null)
        {
            if (!attachment.ContainsKey("@odata.type"))
            {
                attachment.Add("@odata.type", "Microsoft.Dynamics.CRM.activitymimeattachment");
            }

            // body value in attachment not needed. Remove if found.
            if (attachment.ContainsKey("body"))
            {
                attachment.Remove("body");
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
            // Don't override what might be included in the attachment.
            if (!attachment.ContainsKey("mimetype"))
            {
                attachment.Add("mimetype", fileMimeType);
            }

            // Initialize the upload
            InitializeAttachmentBlocksUploadRequest initializeRequest = new(
                target: attachment);

            var initializeResponse =
                await service.SendAsync<InitializeAttachmentBlocksUploadResponse>(
                    request: initializeRequest);

            string fileContinuationToken =
                initializeResponse.FileContinuationToken;

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

                // Prepare the request
                UploadBlockRequest uploadBlockRequest = new(
                    blockId: blockId,
                    blockData: buffer,
                    fileContinuationToken: fileContinuationToken);


                // Send the request
                await service.SendAsync(uploadBlockRequest);
            }

            // Commit the upload
            CommitAttachmentBlocksUploadRequest commitRequest = new(
                target: attachment, 
                blockList: blockIds,
                fileContinuationToken: fileContinuationToken);


            return await service.SendAsync<CommitAttachmentBlocksUploadResponse>(commitRequest);


        }

        /// <summary>
        /// Downloads the file for an activitymimeattachment.
        /// </summary>
        /// <param name="service">The WebAPIService instance to use.</param>
        /// <param name="target">A reference to the activitymimeattachment containing the file.</param>
        /// <returns>Tuple of bytes and fileName</returns>
        /// <exception cref="ArgumentException">The target parameter must refer to an activitymimeattachment record.</exception>
        static async Task<(byte[] bytes, string fileName)> DownloadAttachment(
            Service service,
            EntityReference target)
        {
            if (target.SetName != "activitymimeattachments")
            {
                throw new ArgumentException(
                    "The target parameter must refer to an activitymimeattachment record.",
                    nameof(target));
            }

            InitializeAttachmentBlocksDownloadRequest initializeRequest = new(target: target);

            var response = await service.SendAsync<InitializeAttachmentBlocksDownloadResponse>(initializeRequest);

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
                    blockLength: blockSizeDownload,
                    fileContinuationToken: fileContinuationToken);

                // Send the request
                var downloadBlockResponse =
                           await service.SendAsync<DownloadBlockResponse>(downLoadBlockRequest);

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