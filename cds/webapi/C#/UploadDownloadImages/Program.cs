using System;
using System.Net.Http;
using System.Configuration;
using Newtonsoft.Json.Linq;

namespace PowerApps.Samples
{
    partial class Program
    {
        private static string _accountId;
        private static string _imageAttributeLogicalName;
        private static string _uploadImageName;
        private static string _downloadFolder;
        static void Main(string[] args)
        {
            try
            {
                //Get configuration data from App.config connectionStrings
                string connectionString = ConfigurationManager.ConnectionStrings["Connect"].ConnectionString;
                Console.Write("Enter an existing primary image attribute logical name, like mc_image3, making sure that maximum image size is set to 30720:  ");
                _imageAttributeLogicalName = Console.ReadLine();

                Console.Write("Enter an image to be uploaded, like c:\\UploadedFiles\\22mb.png:   ");
                _uploadImageName = Console.ReadLine();

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

                        Console.WriteLine($"Uploading {_uploadImageName} in progress ...");
                        UploadImage(client, _imageAttributeLogicalName, _uploadImageName);
                        Console.WriteLine($"Upload succeeded");

                        Console.WriteLine($"Downloading {_uploadImageName} in progress ...");
                        DownloadImage(client, _imageAttributeLogicalName,_downloadFolder);
                        Console.WriteLine($"Download succeeded");

                        Console.WriteLine($"Deleting {_imageAttributeLogicalName} in progress ...");
                        DeleteImage(client, _imageAttributeLogicalName);
                        Console.WriteLine($"Delete {_imageAttributeLogicalName} succeeded.");
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
