using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.IO;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        public static CrmServiceClient _service = null;
        public static string entitySchemaName;
        private static string columnSchemaName;
        private const string entityId = "da59721f-02b2-ea11-a812-000d3a1b14a2";
        private static string uploadedImageName;
        private static string downloadImageFolder;

        [STAThread]
        static void Main(string[] args)
        {
            Console.Write("Please enter Entity Logical Name (account): ");
            entitySchemaName = Console.ReadLine();

            Console.Write("Please enter a primary image column logical name that has maximum image size set to 30720, such as image3: ");
            columnSchemaName = Console.ReadLine();

            Console.Write("Please enter an uploaded file name, such as c:\\UploadedFiles\\22mb.png:  ");
            uploadedImageName = Console.ReadLine();

            Console.Write("Please enter a download path, such as c:\\DownloadedFiles\\:  ");
            downloadImageFolder = Console.ReadLine();

            try
            {
                _service = SampleHelpers.Connect("Connect");
                if (_service.IsReady)
                {
                    //      var publisherPrefix =  CreatePublisher(_service);
                    //      var solutionUniqueName = CreateSolution(_service, publisherPrefix);
                    var publisherPrefix = "mc";
                    var solutionUniqueName = "mycustomization";

                    var imageUploadDownload = new SampleImageUploadDownload(_service);
                    var entity = new Entity(entitySchemaName, new Guid(entityId));
                    var attributeName = publisherPrefix + "_" + columnSchemaName;

                    imageUploadDownload.Upload(entity, uploadedImageName, Path.GetFileName(uploadedImageName), attributeName);
                    imageUploadDownload.Download(entity, downloadImageFolder, attributeName);

                //    DeleteSolution(_service, solutionUniqueName);
                //    DeletePublisher(_service, publisherPrefix);
                }
                else
                {
                    const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Microsoft Dataverse";
                    if (_service.LastCrmError.Equals(UNABLE_TO_LOGIN_ERROR))
                    {
                        Console.WriteLine("Check the connection string values in cds/App.config.");
                        throw new Exception(_service.LastCrmError);
                    }
                    else
                    {
                        throw _service.LastCrmException;
                    }
                }
            }
            catch (Exception ex)
            {
                SampleHelpers.HandleException(ex);
            }
            finally
            {
                if (_service != null)
                    _service.Dispose();
                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }
        }
    }
}
