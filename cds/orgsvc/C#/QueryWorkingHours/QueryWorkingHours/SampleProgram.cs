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
                if (service.IsReady)
                {
                    #region Sample Code
                    //////////////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up

                    // Get the current user's information.
                    WhoAmIRequest userRequest = new WhoAmIRequest();
                    WhoAmIResponse userResponse = (WhoAmIResponse)service.Execute(userRequest);

                    // Retrieve the working hours of the current user.                                              
                    QueryScheduleRequest scheduleRequest = new QueryScheduleRequest
                    {
                        ResourceId = userResponse.UserId,
                        Start = DateTime.Now,
                        End = DateTime.Today.AddDays(7),
                        TimeCodes = new TimeCode[] { TimeCode.Available }
                    };
                    QueryScheduleResponse scheduleResponse = (QueryScheduleResponse)service.Execute(scheduleRequest);

                    // Verify if some data is returned for the availability of the current user
                    if (scheduleResponse.TimeInfos.Length > 0)
                    {
                        Console.WriteLine("Successfully queried the working hours of the current user.");
                    }
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
            #endregion Demonstrate
      

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
