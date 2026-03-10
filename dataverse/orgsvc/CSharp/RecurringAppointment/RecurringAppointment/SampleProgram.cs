using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
   public partial class SampleProgram
    {
        // Define the IDs as well as strings needed for this sample.
        public static Guid _recurringAppointmentMasterId;
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
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate

                    // Retrieve the individual appointment instance that falls on or after
                    // 10 days from today. Basically this will be the second instance in the
                    // recurring appointment series.                                          

                    QueryExpression instanceQuery = new QueryExpression
                    {
                        EntityName = Appointment.EntityLogicalName,
                        ColumnSet = new ColumnSet
                        {
                            Columns = { "activityid", "scheduledstart", "scheduledend" }
                        },
                        Criteria = new FilterExpression
                        {
                            Conditions =
                        {
                            new ConditionExpression
                            {
                                AttributeName = "seriesid",
                                Operator = ConditionOperator.Equal,
                                Values = { _recurringAppointmentMasterId }
                            },
                            new ConditionExpression
                            {
                                 AttributeName = "scheduledstart",
                                 Operator = ConditionOperator.OnOrAfter,
                                 Values = { DateTime.Today.AddDays(10) }
                            }
                        }
                        }
                    };

                    EntityCollection individualAppointments = service.RetrieveMultiple(instanceQuery);
                    // Retrieve a recurring appointment series
                    


                    #region Reschedule an instance of recurring appointment

                    // Update the scheduled start and end dates of the appointment
                    // to reschedule it.
                    Appointment updateAppointment = new Appointment
                    {
                        ActivityId = individualAppointments.Entities.Select(x => (Appointment)x).First().ActivityId,
                        ScheduledStart = individualAppointments.Entities.Select(x => (Appointment)x).First().ScheduledStart.Value.AddHours(1),
                        ScheduledEnd = individualAppointments.Entities.Select(x => (Appointment)x).First().ScheduledEnd.Value.AddHours(2)
                    };

                    RescheduleRequest reschedule = new RescheduleRequest
                    {
                        Target = updateAppointment
                    };

                    RescheduleResponse rescheduled = (RescheduleResponse)service.Execute(reschedule);
                    Console.WriteLine("Rescheduled the second instance of the recurring appointment.");

                    #endregion Reschedule an instance of recurring appointment

                    #region Cancel an instance of recurring appointment

                    // Cancel the last instance of the appointment. The status of this appointment
                    // instance is set to 'Canceled'. You can view this appoinyment instance under
                    // the 'All Activities' view. 
                    SetStateRequest appointmentRequest = new SetStateRequest
                    {
                        State = new OptionSetValue((int)AppointmentState.Canceled),
                        Status = new OptionSetValue(4),
                        EntityMoniker = new EntityReference(Appointment.EntityLogicalName,
                            new Guid(individualAppointments.Entities.Select(x => (Appointment)x).Last().ActivityId.ToString()))
                    };

                    service.Execute(appointmentRequest);
                    Console.WriteLine("Canceled the last instance of the recurring appointment.");

                    #endregion Cancel an instance of recurring appointment

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
