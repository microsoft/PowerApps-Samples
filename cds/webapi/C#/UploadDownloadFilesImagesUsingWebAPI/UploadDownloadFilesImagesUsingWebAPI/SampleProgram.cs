using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;
using System.Collections.Generic;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        public static CrmServiceClient _service = null;
        public static string entitySchemaName;
        public static string columnSchemaName;
        public static Guid accountId = Guid.Empty;
        public static string solutionName = "FileImageUploadDownloadSolution";

        [STAThread] // Added to support UX
        static void Main(string[] args)
        {
            try
            {
                _service = SampleHelpers.Connect("Connect");
                if (_service.IsReady)
                {
                    SetUpSample(_service, solutionName);
                    accountId = CreateNewAccountRecord(_service);
                    var customizationPrefix = RetrieveDefaultPublisherCustomizationPrefix(_service);
                    BlobUploadDownloadUsingWebAPI(_service, customizationPrefix);
                    SampleHelpers.DeleteSolution(_service, solutionName);
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

        private static Guid CreateNewAccountRecord(CrmServiceClient service)
        {
            var entity = new Entity("account", new Guid());
            var accountName = "MyAccount" + DateTime.Now.ToFileTime();
            entity.Attributes["name"] = accountName;
            try
            {
                return accountId = _service.Create(entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }  
        }

        private static string RetrieveDefaultPublisherCustomizationPrefix(CrmServiceClient service)
        {
            var publisherFetchXML = $@"<fetch top='50'>
                                          <entity name='solution'>
                                            <attribute name='friendlyname' />
                                            <attribute name='uniquename' />
                                            <filter>
                                              <condition attribute='friendlyname' operator='eq' value='FileImageUploadDownloadSolution' />
                                            </filter>
                                            <link-entity name='publisher' from='publisherid' to='publisherid'>
                                              <attribute name='uniquename' />
                                              <attribute name='friendlyname' />
                                              <attribute name='description' />
                                              <attribute name='customizationprefix' />
                                            </link-entity>
                                          </entity>
                                        </fetch>";

            var publisherEntity = service.RetrieveMultiple(new FetchExpression(publisherFetchXML))?.Entities.FirstOrDefault();
            return ((AliasedValue)(publisherEntity?.Attributes["publisher1.customizationprefix"])).Value.ToString();
        }


        /// <summary>
        /// Uploads and downloads images and files using Web API
        /// </summary>
        /// <param name="service">A Crm servic client object</param>
        /// <param name="customizationPrefix">A publisher prefix</param>
        private static void BlobUploadDownloadUsingWebAPI(CrmServiceClient service, string customizationPrefix)
        {
            var blobUploadDownload = new WebApiBlobUploadDownload(service);

            Dictionary<string, string> columnFiles = new Dictionary<string, string>();
            columnFiles.Add($"{customizationPrefix}_file1", "Files\\25mb.pdf");
            columnFiles.Add($"{customizationPrefix}_image1primary", "Images\\25mb.jpg");

            foreach (var x in columnFiles)
            {
                blobUploadDownload.BlobUploadUsingAPI("accounts", accountId, x.Key, x.Value);
                blobUploadDownload.BlobDownloadUsingAPI("accounts", accountId, x.Key, x.Value);
            }
        }
    }
}
