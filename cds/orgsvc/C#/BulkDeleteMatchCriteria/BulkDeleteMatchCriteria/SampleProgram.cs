using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;

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
                    //////////////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate

                    // This statments checks whether Standard Email templates are present
                    _context = new ServiceContext(service);
                    var emailTemplateId = (
                                       from emailTemplate in _context.TemplateSet
                                       where emailTemplate.Title == "Contact Reconnect"
                                       select emailTemplate.Id
                                      ).FirstOrDefault();

                    if (emailTemplateId != Guid.Empty)
                    {
                        CreateRequiredRecords(service);

                        // Perform the bulk delete.  If you want to perform a recurring delete
                        // operation, then leave this as it is.  Otherwise, pass in false as the
                        // first parameter.
                        PerformBulkDelete(service, true, prompt);
                    }
                    else
                    {
                        throw new ArgumentException("Standard Email Templates are missing");
                    }


                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up

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
            #endregion Sample Code
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
