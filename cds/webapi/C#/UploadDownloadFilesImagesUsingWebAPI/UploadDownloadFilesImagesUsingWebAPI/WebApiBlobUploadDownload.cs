using Microsoft.Xrm.Tooling.Connector;
using PowerApps.Samples.UploadDownloadFiles;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace PowerApps.Samples
{
    public class WebApiBlobUploadDownload
    {
        CrmServiceClient _service = null;
      
        public WebApiBlobUploadDownload(CrmServiceClient service)
        {
            _service = service;
        }

        /// <summary>
        /// Uploads a file or image using Web API
        /// </summary>
        /// <param name="entityPluralName">entity name, such as accounts</param>
        /// <param name="accountId">an account record guid id</param>
        /// <param name="logicalName">a customized column logical name</param>
        /// <param name="blobName">a file or image to be uploaded</param>
        public void BlobUploadUsingAPI(string entityPluralName, Guid accountId, string logicalName, string blobName)
        {
            try
            {
                if (!_service.IsReady)
                {
                    Console.WriteLine($"Crm Service is not ready to use");
                    return;
                }

                var accessToken = _service.CurrentAccessToken;
                var blobUrl = $"{Constants.webApiUrl}/{entityPluralName}({accountId})/{logicalName}";
                var blobFullPath = Path.GetFullPath(blobName).Replace("\\bin\\Debug", "");
                var uploadBlobName = Path.GetFileName(blobFullPath);
                var fileStream = File.OpenRead(blobFullPath);

                var request = new HttpRequestMessage()
                {
                    Method = new HttpMethod("Patch"),
                    RequestUri = new Uri(blobUrl)
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Content = new StreamContent(fileStream);
                request.Content.Headers.Add("Content-Type", "application/octet-stream");
                request.Content.Headers.Add("x-ms-file-name", uploadBlobName);

                var httpClient = new HttpClient();
                Console.WriteLine($"\nPlease wait... uploading {uploadBlobName} in progress...");

                var result = httpClient.SendAsync(request)?.Result;
                Console.WriteLine($"{uploadBlobName} upload completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while uploading:" + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Downloads a file or image using Web API
        /// </summary>
        /// <param name="entityPluralName">an entity name, such as accounts</param>
        /// <param name="accountId">an account record guid id</param>
        /// <param name="logicalName">a customized column logical name</param>
        /// <param name="fileName">an image or file name</param>
        public void BlobDownloadUsingAPI(string entityPluralName, Guid accountId, string logicalName, string fileName)
        {
            try
            {
                var accessToken = _service.CurrentAccessToken;
                var blobURL = $"{Constants.webApiUrl}/{entityPluralName}({accountId})/{logicalName}/$value?size=full";
                var request = new HttpRequestMessage();
                request.Method = new HttpMethod("Get");
                request.RequestUri = new Uri(blobURL);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var httpClient = new HttpClient();
                Console.WriteLine($"Please wait... downloading {Path.GetFileName(fileName)} in progress...");
                var result = httpClient.SendAsync(request)?.Result;

                if (result.IsSuccessStatusCode)
                {
                    var responseContent = result.Content.ReadAsByteArrayAsync().Result;
                    var downloadFilePath = Path.Combine(Directory.GetCurrentDirectory());
                    var downloadFullyQualifedFileName = Path.Combine(downloadFilePath, fileName);
                    File.WriteAllBytes(downloadFullyQualifedFileName, responseContent);
                }
                Console.WriteLine($"{Path.GetFileName(fileName)} download completed");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error occurred while downloading:" + ex.Message);
                throw;
            }
        }
    }
}