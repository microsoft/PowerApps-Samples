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

                    // Retrieve the working hours of the current and the other user.   
                    
                    QueryMultipleSchedulesRequest scheduleRequest = new QueryMultipleSchedulesRequest();
                    scheduleRequest.ResourceIds = new Guid[2];
                    scheduleRequest.ResourceIds[0] = _currentUserId;
                    scheduleRequest.ResourceIds[1] = _otherUserId;
                    scheduleRequest.Start = DateTime.Now;
                    scheduleRequest.End = DateTime.Today.AddDays(7);
                    scheduleRequest.TimeCodes = new TimeCode[] { TimeCode.Available };

                    QueryMultipleSchedulesResponse scheduleResponse = (QueryMultipleSchedulesResponse)service.Execute(scheduleRequest);

                    // Verify if some data is returned for the availability of the users
                    if (scheduleResponse.TimeInfos.Length > 0)
                    {
                        Console.WriteLine("Successfully queried the working hours of multiple users.");
                    }
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
