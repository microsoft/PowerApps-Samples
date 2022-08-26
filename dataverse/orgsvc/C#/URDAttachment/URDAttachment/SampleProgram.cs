using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                // Service implements IOrganizationService interface 
                if (service.IsReady)
                {

                    #region Sample Code
                    //////////////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate
                    
                    // Instantiate an Annotation object.
                    // See the Entity Metadata topic in the SDK documentation to determine
                    // which attributes must be set for each entity.
                    Annotation setupAnnotation = new Annotation()
                    {
                        Subject = "Example Annotation",
                        FileName = "ExampleAnnotationAttachment.txt",
                        DocumentBody = Convert.ToBase64String(
                            new UnicodeEncoding().GetBytes("Sample Annotation Text")),
                        MimeType = "text/plain"
                    };

                    // Create the Annotation object.
                    annotationId = service.Create(setupAnnotation);

                    Console.Write("{0} created with an attachment", setupAnnotation.Subject);

                    // Define columns to retrieve from the annotation record.
                    ColumnSet cols = new ColumnSet("filename", "documentbody");


                    // Retrieve the annotation record.
                    Annotation retrievedAnnotation =
                        (Annotation)service.Retrieve("annotation", annotationId, cols);
                    Console.WriteLine(", and retrieved.");
                    fileName = retrievedAnnotation.FileName;

                    // Download the attachment in the current execution folder.
                    using (FileStream fileStream = new FileStream(retrievedAnnotation.FileName, FileMode.OpenOrCreate))
                    {
                        byte[] fileContent = Convert.FromBase64String(retrievedAnnotation.DocumentBody);
                        fileStream.Write(fileContent, 0, fileContent.Length);
                    }

                    Console.WriteLine("Attachment downloaded.");
                     
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
