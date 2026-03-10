using Microsoft.Crm.Sdk.Messages;
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

                if (service != null)
                {
                    // Service implements IOrganizationService interface 
                    if (service.IsReady)
                    {
                        #region Sample Code
                        //////////////////////////////////////////////
                        #region SetUp
                        //Add any data required to support the core sample.
                        SetUpSample(service);
                        #endregion SetUp
                        #region Demonstrate
                        //Add sample code here

                        Console.WriteLine("Your UserID: {0}", ((WhoAmIResponse)service.Execute(new WhoAmIRequest())).UserId);

                        #endregion Demonstrate
                        #region CleanUp
                        //Remove any data created within the sample
                        CleanUpSample(service);
                        #endregion CleanUp
                        //////////////////////////////////////////////
                        #endregion Sample Code

                        Console.WriteLine("The sample completed successfully");
                        return;
                    }
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
