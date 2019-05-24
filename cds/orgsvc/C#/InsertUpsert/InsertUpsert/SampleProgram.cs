using System;
using System.Collections.Generic;
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
                    #region Clean up

                    // Provides option to delete the ChangeTracking solution

                    CleanUpSample(service);

                    #endregion Clean up

                    //////////////////////////////////////////////

                    #endregion Sample Code
                    Console.WriteLine("The sample completed successfully");

                    return;

                }
                else

                {

                    const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Dynamics CRM";

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
