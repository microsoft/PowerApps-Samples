using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;

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
                    UploadDownloadBlob(_service, customizationPrefix, accountId);
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

        /// <summary>
        /// Retrieves a default publisher's customization prefix for a given crm service client
        /// </summary>
        /// <param name="service">A CrmServiceClient object</param>
        /// <returns>A default publisher's customziation prefix</returns>
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

            var publisherEntity =  service.RetrieveMultiple(new FetchExpression(publisherFetchXML))?.Entities.FirstOrDefault();
            return ((AliasedValue)(publisherEntity?.Attributes["publisher1.customizationprefix"])).Value.ToString();
        }

        /// <summary>
        /// Creates a new account record for a given crm service client
        /// </summary>
        /// <param name="service">A CrmServiceClient object</param>
        /// <returns>A Guid for a newly created account record</returns>
        private static Guid CreateNewAccountRecord(CrmServiceClient service)
        {
            var entity = new Entity("account", new Guid());
            var accountName = "MyAccount" + DateTime.Now.ToFileTime();
            entity.Attributes["name"] = accountName;
            try
            {
                var accountId = service.Create(entity);
                Console.WriteLine($"\nNew Account created: '{accountName} with Guid-{accountId}'");
                return accountId;
            }
            catch (Exception ex)
            {
                throw ex;
            } 
        }


        /// <summary>
        /// Uplaods and downloads a file for a given CRM service client, a publisher customziation prefix and an account guid Id
        /// </summary>
        /// <param name="service"></param>
        /// <param name="customizationPrefix"></param>
        /// <param name="accountId"></param>
        private static void UploadDownloadBlob(CrmServiceClient service, string customizationPrefix, Guid accountId)
        {
            var uploadDownload = new SampleBlobUploadDownload(service);
            var entity = new Entity("account", accountId);

            Dictionary<string, string> columnFiles = new Dictionary<string, string>();
            columnFiles.Add($"{customizationPrefix}_file1", "Files\\25mb.pdf");
            columnFiles.Add($"{customizationPrefix}_image1primary", "Images\\25mb.jpg");

            foreach (var x in columnFiles)
            {
                var attributeName = x.Key;
                
                uploadDownload.Upload(entity, Path.GetFullPath(x.Value).Replace("\\bin\\Debug", ""), Path.GetFileName(x.Value), attributeName);             
                var downloadFilePath = Path.Combine(Directory.GetCurrentDirectory());
                uploadDownload.Download(entity, downloadFilePath, attributeName);
            }
        }
    }
}
