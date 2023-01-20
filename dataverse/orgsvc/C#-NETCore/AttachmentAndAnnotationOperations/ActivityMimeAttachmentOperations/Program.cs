using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System.Net.Mail;
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
            if (path == null) path = "appsettings.json";

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

            List<Guid> reusableAttachmentIds = new();


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
            // Create email activity
            Entity email = new("email")
            {
                Attributes =
                {
                    {"subject", "This is an example email." }
                }
            };

            Guid emailid = serviceClient.Create(email);
            email.Id = emailid;

            Console.WriteLine("Created an email activity.");

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
            Console.WriteLine("Created two e-mail attachments for the e-mail activity.");

            // Set MaxUploadFileSize to the maximum value
            Utility.SetMaxUploadFileSize(serviceClient, 131072000);

            Console.WriteLine($"Updated MaxUploadFileSize to: {Utility.GetMaxUploadFileSize(serviceClient)}");

            Entity largeAttachment = new("activitymimeattachment")
            {
                Attributes =
                    {
                        { "objectid", new EntityReference("email", emailid)},
                        { "objecttypecode", "email" },
                        { "subject", $"Sample attached {pdfDoc.Name}" },
                        { "filename", pdfDoc.Name},
                        { "mimetype", Utility.GetMimeType(pdfDoc)}
                    }
            };

            // Creates the activitymimeattachment record with a file, but doesn't return id.
            int fileSizeInBytes = UploadAttachment(
                service: serviceClient,
                attachment: largeAttachment,
                fileInfo: pdfDoc);

            Console.WriteLine($"Uploaded {pdfDoc.Name} as attachment.");

            // Retrieve information about the attachments related to the email
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

            // TODO: Create an email template to add the re-usable attachments to
            // Attachment objectid and objecttypecode are required

            Entity template = new("template") { 
                Attributes = {
                    { "body", "<?xml version=\"1.0\" ?><xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\"><xsl:output method=\"text\" indent=\"no\"/><xsl:template match=\"/data\"><![CDATA[<div>Text for the example account template.</div>]]></xsl:template></xsl:stylesheet>" },
                    { "description", "The description of the Example Account Template" },
                    { "ispersonal", false }, //Organization
                    { "languagecode", 1033 }, //English
                    { "presentationxml", "<template><text><![CDATA[<div>Text for the example account template.</div>]]></text></template>" },
                    { "safehtml", "<div>Text for the example account template.</div>\n" },
                    { "subject", "<?xml version=\"1.0\" ?><xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\"><xsl:output method=\"text\" indent=\"no\"/><xsl:template match=\"/data\"><![CDATA[Example Account Template Subject]]></xsl:template></xsl:stylesheet>" },
                    { "subjectpresentationxml", "<template><text><![CDATA[Example Account Template Subject]]></text></template>" },
                    { "subjectsafehtml", "Example Account Template Subject" },
                    { "templatetypecode", "account" },
                    { "title", "Example Account Template" }
                }            
            };

            

            Guid templateId = serviceClient.Create(template);

            Console.WriteLine("Created an email template.");

            smallFiles.ForEach(smallFile =>
            {

                Entity attachment = new("activitymimeattachment")
                {
                    Attributes =
                    {
                        { "objectid", new EntityReference("template", templateId)},
                        { "objecttypecode", "template" },
                        { "subject", $"Reusable attachment {smallFile.Name}" },
                        { "body", Convert.ToBase64String(File.ReadAllBytes(smallFile.FullName)) },
                        { "filename", smallFile.Name},
                        { "mimetype", Utility.GetMimeType(smallFile)}
                    }
                };

                reusableAttachmentIds.Add(serviceClient.Create(attachment));

            });

            Console.WriteLine("Added small files as attachment to email template.");

            // Create large attachment

            Entity largeAttachmentForTemplate = new("activitymimeattachment")
            {
                Attributes =
                    {
                        { "objectid", new EntityReference("template", templateId)},
                        { "objecttypecode", "template" },
                        { "subject", $"Reusable attachment {pdfDoc.Name}" },
                        // Does not include the body
                        { "filename", pdfDoc.Name},
                        { "mimetype", Utility.GetMimeType(pdfDoc)}
                    }
            };
            Guid largeattachmentId = serviceClient.Create(largeAttachmentForTemplate);
            //Largeattachment.Id= largeattachmentId; //Set Id so UploadAttachment will work.
            Console.WriteLine("Create Large attachment with no body.");
            reusableAttachmentIds.Add(largeattachmentId);

            // Upload the larger file separately
            UploadAttachment(
                service: serviceClient, 
                attachment: largeAttachmentForTemplate, 
                fileInfo: pdfDoc);

            Console.WriteLine("Uploaded the large file to the existing attachment.");

            // clean up

            foreach (Guid id in reusableAttachmentIds)
            {
                serviceClient.Delete("activitymimeattachment", id);
            }

            //TODO: there are two 25mb.pdf attachments and I don't have the id for one of them.
            serviceClient.Delete("template", templateId);

            #endregion Create re-usable attachments
            // Return MaxUploadFileSize to the original value
            Utility.SetMaxUploadFileSize(serviceClient, originalMaxUploadFileSize);

            Console.WriteLine($"Current MaxUploadFileSize: {Utility.GetMaxUploadFileSize(serviceClient)}");
        }

        /// <summary>
        /// Creates or updates an activitymimeattachment with file.
        /// </summary>
        /// <param name="service">The IOrganizationService instance to use.</param>
        /// <param name="attachment">The activitymimeattachment data to create or update.</param>
        /// <param name="fileInfo">A reference to the file to upload.</param>
        /// <param name="fileMimeType">The mimetype of the file.</param>
        /// <returns>FileSizeInBytes</returns>
        /// <exception cref="ArgumentException">The attachment parameter must be an activitymimeattachment entity.</exception>
        static int UploadAttachment(
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

            // Commit the upload
            CommitAttachmentBlocksUploadRequest commitRequest = new()
            {
                BlockList = blockIds.ToArray(),
                FileContinuationToken = fileContinuationToken,
                Target = attachment
            };

            var commitResponse =
                (CommitAttachmentBlocksUploadResponse)service.Execute(commitRequest);

            return commitResponse.FileSizeInBytes;
        }

        /// <summary>
        /// Downloads the file for an activitymimeattachment
        /// </summary>
        /// <param name="service">The IOrganizationService instance to use.</param>
        /// <param name="target">A reference to the activitymimeattachment</param>
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