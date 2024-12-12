using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.IO;
using System.Xml.Serialization;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        [STAThread] // Added to support UX
        static void Main(string[] args)
        {
            CrmServiceClient service = null;

            try
            {
                service = SampleHelpers.Connect("Connect");
                if (service.IsReady)
                {
                    #region Sample Code
                    ////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate


                    // Use the InstantiateTemplate message to create an e-mail message using a template.
                    var instTemplateReq = new InstantiateTemplateRequest
                    {
                        TemplateId = _templateId,
                        ObjectId = _accountId,
                        ObjectType = Account.EntityLogicalName
                    };

                    var instTemplateResp = (InstantiateTemplateResponse)service.Execute(instTemplateReq);

                    // Serialize the email message to XML, and save to a file.
                    var serializer = new XmlSerializer(typeof(InstantiateTemplateResponse));
                    string filename = "email-message.xml";
                    using (var writer = new StreamWriter(filename))
                    {
                        serializer.Serialize(writer, instTemplateResp);
                    }
                    Console.WriteLine("Created e-mail using the template.");

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up

                }
                #endregion Demonstrate
                #endregion Sample Code
                else
                {
                    const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Microsoft Dataverse";
                    if (service.LastCrmError.Equals(UNABLE_TO_LOGIN_ERROR))
                    {
                        Console.WriteLine("Check the connection string values in cds/App.config.");
                        throw new Exception(service.LastCrmError);
                    }
                    else
                    {
                        throw service.LastCrmException;
                    }
                }
            }
            catch (Exception ex)
            {
                SampleHelpers.HandleException(ex);
            }

            finally
            {
                if (service != null)
                    service.Dispose();

                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }
        }
    }
}
