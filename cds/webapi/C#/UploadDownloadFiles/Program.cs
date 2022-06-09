using System;
using System.Net.Http;
using System.Configuration;
using Newtonsoft.Json.Linq;

namespace PowerApps.Samples
{
    partial class Program
    {
        private static string _accountId;
        private static string _fileAttributeLogicalName;
        private static string _uploadFileName;
        private static string _downloadFolder;
        static void Main(string[] args)
        {
            try
            {
                //Get configuration data from App.config connectionStrings
                string connectionString = ConfigurationManager.ConnectionStrings["Connect"].ConnectionString;
                Console.Write("Enter an existing file attribute column logical name, like mc_file1:  ");
                _fileAttributeLogicalName = Console.ReadLine();

                Console.Write("Enter a file to be uploaded, like c:\\UploadedFiles\\document.pdf:   ");
                _uploadFileName = Console.ReadLine();

                Console.Write("Enter a download folder, like c:\\DownloadedFiles\\:   ");
                _downloadFolder = Console.ReadLine();

                using (HttpClient client = SampleHelpers.GetHttpClient(connectionString, SampleHelpers.clientId, SampleHelpers.redirectUrl))
                {
                    var urlForGetAccountId = $"{client.BaseAddress}accounts?$select=name,accountid&$top=10";

                    HttpResponseMessage response = client.GetAsync(urlForGetAccountId,
                            HttpCompletionOption.ResponseHeadersRead).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        _accountId = body["value"][0]["accountid"].ToString();

                        Console.WriteLine($"Uploading {_uploadFileName} in progress ...");
                        UploadFile(client, _fileAttributeLogicalName, _uploadFileName);
                        Console.WriteLine($"Upload succeeded");

                        Console.WriteLine($"Downloading {_uploadFileName} in progress ...");
                        DownloadFile(client, _fileAttributeLogicalName,_downloadFolder);
                        Console.WriteLine($"Download succeeded");

                        Console.WriteLine($"Deleting {_fileAttributeLogicalName} in progress ...");
                        DeleteFile(client, _fileAttributeLogicalName);
                        Console.WriteLine($"Delete {_fileAttributeLogicalName} succeeded.");
                    }
                }
            }
            catch (Exception ex)
            {
                SampleHelpers.DisplayException(ex);
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
            }           
        }
    }
}
