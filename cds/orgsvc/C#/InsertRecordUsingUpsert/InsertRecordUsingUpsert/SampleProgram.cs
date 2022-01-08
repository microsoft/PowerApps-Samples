using Microsoft.Xrm.Tooling.Connector;
using System;

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

                    // Processes the data in newsampleproduct.xml 
                    // to represent new products. Creates 13 records in sample_product entity.
                    // RecordCreated property returns true to indicate the records were created.

                    ProcessUpsert(service, ".\\newsampleproduct.xml");

                    // Processes the data in updatedsampleproduct.xml 
                    // to represent updates to products previously created. 
                    // Updates 6 existing records in sample_product entity.
                    // RecordCreated property returns false to indicate the existing records were updated.
                    ProcessUpsert(service, ".\\updatedsampleproduct.xml");

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
                    Console.WriteLine("The sample completed successfully");
                    return;
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
