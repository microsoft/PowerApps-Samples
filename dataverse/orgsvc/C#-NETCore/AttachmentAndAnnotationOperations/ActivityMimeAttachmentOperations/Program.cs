using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
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

            // Create email activity
            Entity email = new("email")
            {
                Attributes =
                {
                    {"subject", "This is an example email." }
                }
            };

            Guid emailid = serviceClient.Create(email);
            Console.WriteLine("Created an email activity.");

            List<FileInfo> smallFiles = new() { wordDoc, excelDoc };
            List<Guid> emailAttachmentIds = new();

            smallFiles.ForEach(smallFile => {
                
                Entity attachment = new("activitymimeattachment") { 
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

                emailAttachmentIds.Add(serviceClient.Create(attachment));

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

            int fileSizeInBytes = UploadAttachment(
                service: serviceClient, 
                attachment: largeAttachment, 
                fileInfo: pdfDoc);


            // Delete the email activity
            serviceClient.Delete("email", emailid);

            // Return MaxUploadFileSize to the original value
            Utility.SetMaxUploadFileSize(serviceClient, originalMaxUploadFileSize);

            Console.WriteLine($"Current MaxUploadFileSize: {Utility.GetMaxUploadFileSize(serviceClient)}");
        }

        static int UploadAttachment(
                IOrganizationService service,
                Entity attachment,
                FileInfo fileInfo,
                string fileMimeType = null)
        {

            if (attachment.LogicalName != "activitymimeattachment")
            {
                throw new ArgumentException("The attachment parameter must be an activitymimeattachment entity", nameof(attachment));
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

            attachment["mimetype"] = fileMimeType;

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


        static (byte[] bytes, string fileName) DownloadAttachment(
            IOrganizationService service,
            EntityReference target)
        {
            if (target.LogicalName != "activitymimeattachment")
            {
                throw new ArgumentException("The target parameter must refer to an activitymimeattachment record.", nameof(target));
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