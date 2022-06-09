using Microsoft.Xrm.Sdk;
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
        private const string entityId = "39a9c6f1-20dd-ec11-a7b6-0022482410a9";
        private static string uploadedFileName;

        [STAThread]
        static void Main(string[] args)
        {
            Console.Write("Please enter Entity Logical Name (account): ");
            entitySchemaName = Console.ReadLine();

            Console.Write("Please enter column logical name (file1): ");
            columnSchemaName = Console.ReadLine();

            Console.Write("Please enter an uploaded file name, such as c:\\UploadedFiles\\document.pdf");
            uploadedFileName = Console.ReadLine();

            try
            {
                _service = SampleHelpers.Connect("Connect");
                if (_service.IsReady)
                {
                    var publisherPrefix = CreatePublisher(_service);
                    var solutionUniqueName = CreateSolution(_service, publisherPrefix);
                    var fileUploadDownload = new SampleFileUploadDownload(_service);
                    var entity = new Entity(entitySchemaName, new Guid(entityId));
                    var attributeName = publisherPrefix + "_" + columnSchemaName;

                    fileUploadDownload.Upload(entity, uploadedFileName, Path.GetFileName(uploadedFileName), attributeName);

                    var downloadFilePath = Path.Combine(Directory.GetCurrentDirectory());
                    fileUploadDownload.Download(entity, downloadFilePath, attributeName);

                    DeleteSolution(_service, solutionUniqueName);
                    DeletePublisher(_service, publisherPrefix);
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
