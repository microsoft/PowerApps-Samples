using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
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
            FileInfo pdfDoc = new("Files/25mb.pdf"); // A large file over default size

            List<FileInfo> smallFiles = new() { wordDoc, excelDoc };
            List<FileInfo> allFiles = new() { wordDoc, excelDoc, pdfDoc };

            List<(string FileName, Guid ActivityMimeAttachmentId)> reusableAttachments = new();


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

            #region Create single-use attachments

            Console.WriteLine("Start: Create single-use attachments");

            // Create email activity
            Entity email = new("email")
            {
                Attributes =
                {
                    {"subject", "This is an example email." }
                }
            };

            Guid emailid = serviceClient.Create(email);
            email.Id= emailid; //Added so that ToEntityReference will work later.

            Console.WriteLine("Created an email activity.");


            // Attach the small files to the email directly
            smallFiles.ForEach(smallFile =>
            {

                Entity attachment = new("activitymimeattachment")
                {
                    Attributes =
                    {
                        { "objectid", new EntityReference("email", emailid)},
                        { "objecttypecode", "email" },
                        { "subject", $"Sample attached {smallFile.Name}" },
                        { "body", Convert.ToBase64String(File.ReadAllBytes(smallFile.FullName)) },
                        { "filename", smallFile.Name},
                        { "mimetype", Utility.GetMimeType(smallFile)}
                    }
                };

                serviceClient.Create(attachment);

            });

            Console.WriteLine("Created two e-mail attachments with small files for the e-mail activity.");

            // Set MaxUploadFileSize to the maximum value
            Utility.SetMaxUploadFileSize(serviceClient, 131072000);

            Console.WriteLine($"Updated MaxUploadFileSize to: {Utility.GetMaxUploadFileSize(serviceClient)}");

            // Prepare data for a large attachment
            Entity largeAttachment = new("activitymimeattachment")
            {
                Attributes =
                    {
                        { "objectid", new EntityReference("email", emailid)},
                        { "objecttypecode", "email" },
                        { "subject", $"Sample attached {pdfDoc.Name}" },
                        // Do not set the body
                        { "filename", pdfDoc.Name},
                        { "mimetype", Utility.GetMimeType(pdfDoc)}
                    }
            };

            Console.WriteLine($"Adding {pdfDoc.Name}...");

            // Creates the activitymimeattachment record with a file.
            CommitAttachmentBlocksUploadResponse uploadAttachmentResponse = UploadAttachment(
                service: serviceClient,
                attachment: largeAttachment,
                fileInfo: pdfDoc);

            Console.WriteLine($"\tUploaded {pdfDoc.Name} as attachment. " +
                $"\n\t\tActivityMimeAttachmentId:{uploadAttachmentResponse.ActivityMimeAttachmentId} \n\t\tFileSizeInBytes: {uploadAttachmentResponse.FileSizeInBytes}");

            // Retrieve information about the attachments related to the email.
            // See https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-retrieve#retrieve-with-related-rows
            RelationshipQueryCollection relationshipQueryCollection = new();

            // The named relationship between email and activitymimeattachment
            Relationship email_attachments = new("email_activity_mime_attachment");
            // Details about what to retrieve
            QueryExpression relatedAttachments = new("activitymimeattachment")
            {
                ColumnSet = new ColumnSet("filename")
            };

            relationshipQueryCollection.Add(
                key: email_attachments,
                value: relatedAttachments);

            RetrieveRequest retrieveRequest = new()
            {
                ColumnSet = new ColumnSet("activityid"),
                RelatedEntitiesQuery = relationshipQueryCollection,
                Target = email.ToEntityReference()
            };

            var retrieveResponse = (RetrieveResponse)serviceClient.Execute(retrieveRequest);
            Entity retrievedEmail = retrieveResponse.Entity;

            EntityCollection attachments = retrievedEmail.RelatedEntities[email_attachments];

            Console.WriteLine("Download attached files:");

            foreach (Entity attachment in attachments.Entities)
            {
                string filename = (string)attachment["filename"];
                Console.WriteLine($"\tDownloading filename: {filename}...");

                var (bytes, name) = DownloadAttachment(
                    service: serviceClient,
                    target: attachment.ToEntityReference());

                File.WriteAllBytes($"Downloaded{name}", bytes);
                Console.WriteLine($"\tSaved the attachment to \\bin\\Debug\\net6.0\\Downloaded{name}.");
            }

            // Delete the email activity and the attachments will be deleted as well
            serviceClient.Delete("email", emailid);

            #endregion Create single-use attachments

            #region Create re-usable attachments

            Console.WriteLine("\nStart: Create re-usable attachments");

            // Create an email template to add the re-usable attachments to.
            // ActivityMimeAttachment ObjectId and ObjectTypeCode are SystemRequired.

            Entity template = new("template")
            {
                Attributes = {
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
                    "       <![CDATA[Example Account Template Subject]]>" +
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
                }
            };

            // Create the template
            Guid templateId = serviceClient.Create(template);

            Console.WriteLine("Created an email template.");

            // Add all files (large and small) as attachments to the template in the same way.
            allFiles.ForEach(file =>
            {
                Entity attachment = new("activitymimeattachment")
                {
                    Attributes =
                    {
                        { "objectid", new EntityReference("template", templateId)},
                        { "objecttypecode", "template" },
                        { "subject", $"Reusable attachment {file.Name}" },
                        // Does not include the body.
                        { "filename", file.Name},
                        { "mimetype", Utility.GetMimeType(file)}
                    }
                };

                // Create the attachment with upload
                CommitAttachmentBlocksUploadResponse uploadAttachmentResponse = UploadAttachment(
                    service: serviceClient,
                    attachment: attachment,
                    fileInfo: file);

                reusableAttachments.Add((file.Name, uploadAttachmentResponse.ActivityMimeAttachmentId));
                Console.WriteLine($"\tAdded {file.Name} to the email template.");

            });

            Console.WriteLine("Added all files as attachment to email template.");

            // Create new email to re-use attachments from Template
            Entity email2 = new("email")
            {
                Attributes =
                {
                    {"subject", "This is an example email with re-used attachments." }
                }
            };

            Guid email2Id = serviceClient.Create(email2);

            Console.WriteLine("Created a second email activity.");

            foreach ((string FileName, Guid ActivityMimeAttachmentId) in reusableAttachments)
            {
                Entity attachment = new("activitymimeattachment")
                {
                    Attributes =
                    {
                        { "objectid", new EntityReference("email", email2Id)},
                        { "objecttypecode", "email" },
                        { "subject", $"Sample attached {FileName}" },
                        { "attachmentid", new EntityReference("activitymimeattachment", ActivityMimeAttachmentId) } // Only set attachmentid
                        // Do not set body, filename, or mimetype
                    }
                };

                serviceClient.Create(attachment);

                Console.WriteLine($"\tAttached {FileName} to the second email");

            }

            // Delete the second email
            serviceClient.Delete("email", email2Id);
            Console.WriteLine($"Deleted the second email.");

            // Verify the re-used attachments still exist
            foreach ((string FileName, Guid ActivityMimeAttachmentId) in reusableAttachments)
            {
                Entity attachment = serviceClient.Retrieve(
                    entityName: "activitymimeattachment",
                    id: ActivityMimeAttachmentId,
                    columnSet: new ColumnSet("filename"));

                if ((string)attachment["filename"] == FileName)
                {
                    Console.WriteLine($"\tAttachment for {FileName} still exists.");
                }
            }

            // Clean up

            // Delete the template
            serviceClient.Delete("template", templateId); //Will delete re-usable attachments


            #endregion Create re-usable attachments

            // Return MaxUploadFileSize to the original value
            Utility.SetMaxUploadFileSize(serviceClient, originalMaxUploadFileSize);

            Console.WriteLine($"Current MaxUploadFileSize: {Utility.GetMaxUploadFileSize(serviceClient)}");
        }

        /// <summary>
        /// Creates an activitymimeattachment with file.
        /// </summary>
        /// <param name="service">The IOrganizationService instance to use.</param>
        /// <param name="attachment">The activitymimeattachment data to create.</param>
        /// <param name="fileInfo">A reference to the file to upload.</param>
        /// <param name="fileMimeType">The mimetype of the file.</param>
        /// <returns>CommitAttachmentBlocksUploadResponse containing ActivityMimeAttachmentId and FileSizeInBytes.</returns>
        /// <exception cref="ArgumentException">The attachment parameter must be an activitymimeattachment entity.</exception>
        static CommitAttachmentBlocksUploadResponse UploadAttachment(
                IOrganizationService service,
                Entity attachment,
                FileInfo fileInfo,
                string fileMimeType = null)
        {

            if (attachment.LogicalName != "activitymimeattachment")
            {
                throw new ArgumentException(
                    "The attachment parameter must be an activitymimeattachment entity.",
                    nameof(attachment));
            }

            // body value in activitymimeattachment not needed. Remove if found.
            if (attachment.Contains("body"))
            {
                attachment.Attributes.Remove("body");
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
            // Don't overwrite mimetype value if it exists
            if (!attachment.Contains("mimetype"))
            {
                attachment["mimetype"] = fileMimeType;
            }

            // Initialize the upload
            InitializeAttachmentBlocksUploadRequest initializeRequest = new()
            {
                Target = attachment
            };

            var initializeResponse =
                (InitializeAttachmentBlocksUploadResponse)service.Execute(initializeRequest);

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
            CommitAttachmentBlocksUploadRequest commitRequest = new()
            {
                BlockList = blockIds.ToArray(),
                FileContinuationToken = fileContinuationToken,
                Target = attachment
            };
           
             return  (CommitAttachmentBlocksUploadResponse)service.Execute(commitRequest);

        }

        /// <summary>
        /// Downloads the file for an activitymimeattachment.
        /// </summary>
        /// <param name="service">The IOrganizationService instance to use.</param>
        /// <param name="target">A reference to the activitymimeattachment containing the file.</param>
        /// <returns>Tuple of bytes and fileName</returns>
        /// <exception cref="ArgumentException">"The target parameter must refer to an activitymimeattachment record."</exception>
        static (byte[] bytes, string fileName) DownloadAttachment(
            IOrganizationService service,
            EntityReference target)
        {
            if (target.LogicalName != "activitymimeattachment")
            {
                throw new ArgumentException(
                    "The target parameter must refer to an activitymimeattachment record.",
                    nameof(target));
            }

            InitializeAttachmentBlocksDownloadRequest initializeRequest = new()
            {
                Target = target
            };

            var response =
                (InitializeAttachmentBlocksDownloadResponse)service.Execute(initializeRequest);

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