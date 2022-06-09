using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;

namespace PowerApps.Samples
{
    public class SampleFileUploadDownload
    {
        private CrmServiceClient _service;
        private const int _chunkSize = 4 * 1024 * 1024; //4 MB
        public SampleFileUploadDownload(CrmServiceClient service)
        {
            _service = service;
        }

        /// <summary>
        /// Upload a file 
        /// </summary>
        /// <param name="e">The Entity object to use</param>
        /// <param name="filePath">The file path of the uploaded file</param>
        /// <param name="fileName">The file name of the uploaded file</param>
        /// <param name="fileAttributeName">The file column logical name</param>
        /// <returns></returns>
        public Guid Upload(Entity e, string filePath, string fileName, string fileAttributeName)
        {
            var uploadToken = InitializeFileUpload(e, fileName, fileAttributeName);
            var chunksLists = UploadFile(e, uploadToken, filePath, fileName);
            return CommitFile(e, chunksLists, uploadToken, fileName);
        }

        private string InitializeFileUpload(Entity e, string fileName, string fileAttributeName)
        {
            string requestName = null;
            OrganizationRequest initializeUploadRequest = null;
            if (e.Attributes.Contains("activitymimeattachmentid"))
            {
                requestName = "InitializeAttachmentBlocksUpload";
                initializeUploadRequest = new InitializeAttachmentBlocksUploadRequest
                {
                    Target = e
                };
            }
            else if (e.Attributes.Contains("annotationid"))
            {
                initializeUploadRequest = new InitializeAnnotationBlocksUploadRequest
                {
                    Target = e
                };
            }
            else
            {
                requestName = "InitializeFileBlocksUpload";
                initializeUploadRequest = new InitializeFileBlocksUploadRequest
                {
                    Target = e.ToEntityReference(),
                    FileAttributeName = fileAttributeName,
                    FileName = fileName
                };
            }

            Console.WriteLine("Calling {0} for: {1}", requestName, fileName);

            var response = _service.Execute(initializeUploadRequest);
            var token = (string)response.Results["FileContinuationToken"];
            Console.WriteLine("Upload Token: " + token);

            return token;
        }

        private List<string> UploadFile(Entity e, string continuationToken, string filePath, string fileName)
        {
            var chunksLists = new List<string>();
            var requestName = "UploadBlock";

            Console.WriteLine("Calling {0} for: {1}", requestName, fileName);

            using (Stream source = File.OpenRead(filePath))
            {
                int chunkSize = _chunkSize;

                byte[] buffer = new byte[chunkSize];
                int bytesRead;
                var f = new FileInfo(filePath);
                long fileSize = f.Length;
                int chunksCount = (int)Math.Ceiling((fileSize / (float)chunkSize));
                int chunkNumber = 0;
                Console.WriteLine("Calling {0} with chunks for: {1}", requestName, fileName);

                var uploadTasks = new List<System.Threading.Tasks.Task>();

                Console.WriteLine($"Queuing chunks from file {fileName}:");
                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (bytesRead < buffer.Length)
                    {
                        Array.Resize(ref buffer, bytesRead);
                    }
                    chunkNumber++;
                    var chunkId = Convert.ToBase64String(Encoding.UTF8.GetBytes(chunkNumber.ToString().PadLeft(16, '0')));
                    chunksLists.Add(chunkId);
                    var blockData = new byte[buffer.Length];
                    buffer.CopyTo(blockData, 0);

                    var uploadRequest = new UploadBlockRequest
                    {
                        BlockId = chunkId,
                        BlockData = blockData,
                        FileContinuationToken = continuationToken,
                    };

                    Console.WriteLine($"Queuing chunk {chunkNumber} of {chunksCount}; ", ConsoleColor.Yellow);
                    uploadTasks.Add(System.Threading.Tasks.Task.Run(() => _service.Execute(uploadRequest)));
                }

                Console.WriteLine("Sending all chunks from file {0}..", fileName);
                System.Threading.Tasks.Task.Run(async () => await System.Threading.Tasks.Task.WhenAll(uploadTasks.ToArray()).ConfigureAwait(false)).GetAwaiter().GetResult();
                Console.WriteLine($"Chunks sent! Chunks:{chunksCount}; File:{fileName}", ConsoleColor.Green);
                source.Close();
                source.Dispose();
            }

            return chunksLists;
        }

        private Guid CommitFile(Entity e, List<string> chunksLists, string continuationToken, string fileName)
        {
            Guid fileAttributeValue = Guid.Empty;
            var chunks = chunksLists.ToArray();

            string requestName = null;
            OrganizationRequest commitRequest = null;

            if (e.Attributes.Contains("activitymimeattachmentid"))
            {
                requestName = "CommitAttachmentBlocksUpload";
                commitRequest = new CommitAttachmentBlocksUploadRequest
                {
                    Target = e,
                    BlockList = chunks,
                    FileContinuationToken = continuationToken
                };
            }
            else if (e.Attributes.Contains("annotationid"))
            {
                requestName = "CommitAnnotationBlocksUpload";
                commitRequest = new CommitAnnotationBlocksUploadRequest
                {
                    Target = e,
                    BlockList = chunks,
                    FileContinuationToken = continuationToken
                };
            }
            else
            {
                requestName = "CommitFileBlocksUpload";
                commitRequest = new CommitFileBlocksUploadRequest
                {
                    BlockList = chunks,
                    FileContinuationToken = continuationToken,
                    FileName = fileName,
                    MimeType = MimeMapping.GetMimeMapping(fileName),
                };
            }

            Console.WriteLine("Calling {0} for: {1}", requestName, fileName);
            var resp2 = _service.Execute(commitRequest);
            if (String.Equals(requestName, "CommitFileBlocksUpload"))
            {
                fileAttributeValue = (Guid)resp2.Results["FileId"];
                Console.WriteLine("File Attribute value {0}", fileAttributeValue);
            }

            Console.WriteLine("File committed to Storage and org db.", ConsoleColor.Green);
            return fileAttributeValue;
        }

        public void Download(Entity e, string downloadLocation, string fileAttributeName = null)
        {
            InitializeFileDownload(e, fileAttributeName, out string downloadToken, out Int64 fileSizeDownload, out string downloadFileName);
            DownloadFile(downloadToken, fileSizeDownload, downloadLocation, downloadFileName);
        }

        private void InitializeFileDownload(Entity e, string fileAttributeName, out string token, out long fileSize, out string downloadFileName)
        {
            string requestName = null;
            OrganizationRequest initializeDownloadRequest = null;

            if (e.Attributes.Contains("activitymimeattachmentid"))
            {
                requestName = "InitializeAttachmentBlocksDownload";
                initializeDownloadRequest = new InitializeAttachmentBlocksDownloadRequest
                {
                    Target = e.ToEntityReference()
                };
            }
            else if (e.Attributes.Contains("annotationid"))
            {
                requestName = "InitializeAnnotationBlocksDownload";
                initializeDownloadRequest = new InitializeAnnotationBlocksDownloadRequest
                {
                    Target = e.ToEntityReference()
                };
            }
            else
            {
                requestName = "InitializeFileBlocksDownload";
                initializeDownloadRequest = new InitializeFileBlocksDownloadRequest
                {
                    Target = e.ToEntityReference(),
                    FileAttributeName = fileAttributeName
                };
            }

            Console.WriteLine("Calling {0} ", requestName);

            var resp3 = _service.Execute(initializeDownloadRequest);

            token = (string)resp3.Results["FileContinuationToken"];

            Console.WriteLine("Download token: " + token);

            if (string.Equals(requestName, "InitializeFileBlocksDownload"))
            {
                fileSize = (long)resp3.Results["FileSizeInBytes"];
            }
            else
            {
                fileSize = (int)resp3.Results["FileSizeInBytes"];
            }
            downloadFileName = (string)resp3.Results["FileName"];
        }

        private void DownloadFile(string downloadToken, long fileSizeDownload, string downloadLocation, string downloadFileName)
        {
            var requestName = "DownloadBlock";

            long offset = 0;
            long chunkSizeDownload = 4 * 1024 * 1024; // 4 MB
            Console.WriteLine($"File download size: {fileSizeDownload} Bytes. Chunk size: {chunkSizeDownload} MB", ConsoleColor.Cyan);
            int chunksCountDownload = (int)(fileSizeDownload / (float)chunkSizeDownload) + 1;
            int chunkNumberDownload = 0;

            using (FileStream outputFile = new FileStream(downloadLocation + "//" + downloadFileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                Console.WriteLine("Calling {0} with chunks for: {1}", requestName, downloadFileName);
                var downloadTasks = new List<System.Threading.Tasks.Task>();
                int counter = 0;
                byte[] emptyBytes = new byte[fileSizeDownload];
                while (fileSizeDownload > 0)
                {
                    chunkNumberDownload++;
                    Console.WriteLine("Downloading chunk {0}/{1} from file {2}", chunkNumberDownload, chunksCountDownload, downloadFileName);
                    if (fileSizeDownload < chunkSizeDownload)
                    {
                        chunkSizeDownload = (int)fileSizeDownload;
                    }

                    var downloadRequest = new DownloadBlockRequest
                    {
                        FileContinuationToken = downloadToken,
                        Offset = offset,
                        BlockLength = chunkSizeDownload
                    };


                    downloadTasks.Add(System.Threading.Tasks.Task.Factory.StartNew((object data) =>
                    {
                        var data1 = (dynamic)data;
                        var c = Convert.ToInt32(data1.Item2);
                        var fileDataOffset = Convert.ToInt32(data1.Item1);

                        var response3 = _service.Execute(downloadRequest);

                        var result = (byte[])response3.Results["Data"];

                        Console.WriteLine($"Counter:{c}; Offset: {fileDataOffset}; Length: {result.Length}", ConsoleColor.Cyan);
                        Buffer.BlockCopy(result, 0, emptyBytes, fileDataOffset, result.Length);
                    }, new Tuple<long, int>(offset, counter)));

                    offset += chunkSizeDownload;
                    fileSizeDownload -= chunkSizeDownload;
                    counter++;
                }

                System.Threading.Tasks.Task.Run(async () =>
                {
                    await System.Threading.Tasks.Task.WhenAll(downloadTasks.ToArray()).ConfigureAwait(false);
                }).GetAwaiter().GetResult();

                outputFile.Write(emptyBytes, 0, emptyBytes.Length);
                outputFile.Flush();

                Console.WriteLine("Download complete");
            }
        }
        public void DeleteFile(Guid fileAttributeValue)
        {
            Console.WriteLine("Calling DeleteFile for FileAttributeValue : {0}", fileAttributeValue);
            var deleteRequest = new DeleteFileRequest
            {
                FileId = fileAttributeValue
            };

            var resp3 = _service.Execute(deleteRequest);
            Console.WriteLine("File Deleted");
        }
    }
}