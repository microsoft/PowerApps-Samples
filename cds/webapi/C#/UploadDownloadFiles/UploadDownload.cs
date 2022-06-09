using System;
using System.IO;
using System.Net.Http;

namespace PowerApps.Samples
{
    /// <summary>
    /// Partial class
    /// </summary>
    partial class Program
    {

        /// <summary>
        /// Upload a file
        /// </summary>
        /// <param name="client">The HttpClient object to use</param>
        /// <param name="logicalName">The file column logical name</param>
        /// <param name="fileName">The name of the uploaded file</param>
        /// <exception cref="Exception">Exception thrown</exception>
        public static void UploadFile(HttpClient client, string logicalName, string fileName)
        {
            var fileUrl = $"{client.BaseAddress}accounts({_accountId})/{logicalName}";
            var fileStream = File.OpenRead(fileName);
            var request = new HttpRequestMessage()
            {
                Method = new HttpMethod("Patch"),
                RequestUri = new Uri(fileUrl)
            };
            request.Content = new StreamContent(fileStream);
            request.Content.Headers.Add("Content-Type", "application/octet-stream");
            request.Content.Headers.Add("x-ms-file-name", Path.GetFileName(fileName));
            HttpResponseMessage response = client.SendAsync(request)?.Result;

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"File {fileName} successfully uploaded.");
            }
            else
            {
                throw new Exception($"The UploadFile request failed with a status of '{response.ReasonPhrase}')");
            }
        }


        /// <summary>
        /// Download a file
        /// </summary>
        /// <param name="client">The HttpClient object to use</param>
        /// <param name="logicalName">The file column logical name</param>
        /// <param name="downloadFolder">The download folder</param>
        /// <exception cref="Exception">Exception thrown</exception>
        public static void DownloadFile(HttpClient client, string logicalName, string downloadFolder)
        {
            var fileUrl = $"{client.BaseAddress}accounts({_accountId})/{logicalName}/$value?size=full";
            var request = new HttpRequestMessage()
            {
                Method = new HttpMethod("Get"),
                RequestUri = new Uri(fileUrl)
            };

            var response = client.SendAsync(request)?.Result;
            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsByteArrayAsync().Result;
                var downloadFilePath = Path.Combine(downloadFolder, $"{DateTime.Now.ToString("yyyyMMddTHHmmss")}{Path.GetFileName(_uploadFileName)}");
                Console.WriteLine($"Saving download in {downloadFilePath}");
                File.WriteAllBytes(downloadFilePath, responseContent);
            }
            else
            {
                throw new Exception($"The DownloadFile request failed with a status of '{response.ReasonPhrase}')");
            }
        }



        /// <summary>
        /// Delete a file
        /// </summary>
        /// <param name="client">The HttpClient object to use</param>
        /// <param name="logicalName">The file attribute logical name</param>
        /// <exception cref="Exception">Exception thrown</exception>
        public static void DeleteFile(HttpClient client, string logicalName)
        {
            var fileUrl = $"{client.BaseAddress}accounts({_accountId})/{logicalName}";
            var request = new HttpRequestMessage()
            {
                Method = new HttpMethod("Delete"),
                RequestUri = new Uri(fileUrl)
            };

            var response = client.SendAsync(request)?.Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                throw new Exception($"The DeleteFile request failed with a status of '{response.ReasonPhrase}'");
            }
        }
    }
}
