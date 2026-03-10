using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.IO;
using System.Xml.Serialization;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {

        // Define the IDs needed for this sample.        
        private static Guid _accountId;
        private static Guid _templateId;
        private static bool prompt = true;

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
                    ////////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up

                    
                    // Use the InstantiateTemplate message to create an e-mail message using a template.
                    InstantiateTemplateRequest instTemplateReq = new InstantiateTemplateRequest
                    {
                        TemplateId = _templateId,
                        ObjectId = _accountId,
                        ObjectType = Account.EntityLogicalName
                    };
                    InstantiateTemplateResponse instTemplateResp = (InstantiateTemplateResponse)service.Execute(instTemplateReq);

                    // Serialize the email message to XML, and save to a file.
                    XmlSerializer serializer = new XmlSerializer(typeof(InstantiateTemplateResponse));
                    string filename = "email-message.xml";
                    using (StreamWriter writer = new StreamWriter(filename))
                    {
                        serializer.Serialize(writer, instTemplateResp);
                    }
                    Console.WriteLine("Created e-mail using the template.");


                    #region Clean Up
                    CleanUpSample(service);
                    #endregion Clean Up
                }

                #endregion Demonstrate

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
