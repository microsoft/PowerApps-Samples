using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
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

                    // Retrieve a recurring appointment series
                    RecurringAppointmentMaster retrievedRecurringAppointmentSeries = (RecurringAppointmentMaster)service.Retrieve(RecurringAppointmentMaster.EntityLogicalName, _recurringAppointmentMasterId, new ColumnSet(true));

                    // Use the DeleteOpenInstances message to end the series to the
                    // last occurring past instance date w.r.t. the series end date
                    // (i.e., 20 days from today). Effectively, that means that the 
                    // series will end after the third instance (day 14) as this
                    // instance is the last occuring past instance w.r.t the specified 
                    // series end date (20 days from today).
                    // Also specify that the state of past instances (w.r.t. the series 
                    // end date) be set to 'completed'.
                    DeleteOpenInstancesRequest endAppointmentSeries = new DeleteOpenInstancesRequest
                    {
                        Target = retrievedRecurringAppointmentSeries,
                        SeriesEndDate = DateTime.Today.AddDays(20),
                        StateOfPastInstances = (int)AppointmentState.Completed
                    };
                    service.Execute(endAppointmentSeries);

                    Console.WriteLine("The recurring appointment series has been ended after the third occurrence.");


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
