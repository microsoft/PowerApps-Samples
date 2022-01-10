using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
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
                    #region Demonstrate

                    // Specify the recurrence information that needs to be added to the
                    // existing appointment.
                    // 1.  Define an anonymous type to define the possible recurrence pattern values.
                    var RecurrencePatternTypes = new
                    {
                        Daily = 0,
                        Weekly = 1,
                        Monthly = 2,
                        Yearly = 3
                    };

                    // 2.  Define an anonymous type to define the possible values for days 
                    // of the week
                    var DayOfWeek = new
                    {
                        Sunday = 0x01,
                        Monday = 0x02,
                        Tuesday = 0x04,
                        Wednesday = 0x08,
                        Thursday = 0x10,
                        Friday = 0x20,
                        Saturday = 0x40
                    };

                    // 3.  Define an anonymous type to define the possible values  
                    // for the recurrence rule pattern end type.
                    var RecurrenceRulePatternEndType = new
                    {
                        NoEndDate = 1,
                        Occurrences = 2,
                        PatternEndDate = 3
                    };

                    // 4.  Finally, use a recurring appointment master object to specify
                    //     the recurrence information. Other appointment details such as
                    //     'subject' and 'location' are copied from the existing appointment
                    //     to the recurring appointment master object.

                    RecurringAppointmentMaster newRecurringAppointmentInfo = new RecurringAppointmentMaster
                    {
                        StartTime = DateTime.Now.AddHours(2),
                        EndTime = DateTime.Now.AddHours(3),
                        RecurrencePatternType = new OptionSetValue(RecurrencePatternTypes.Weekly),
                        Interval = 1,
                        DaysOfWeekMask = DayOfWeek.Thursday,
                        PatternStartDate = DateTime.Today,
                        PatternEndType = new OptionSetValue(RecurrenceRulePatternEndType.Occurrences),
                        Occurrences = 5
                    };


                    // Use the AddRecurrence message to convert the existing appointment
                    // object to a recurring appointment master object. The existing
                    // appointment object is deleted thereafter.

                    AddRecurrenceRequest recurringInfoRequest = new AddRecurrenceRequest
                    {
                        Target = newRecurringAppointmentInfo,
                        AppointmentId = _appointmentId
                    };

                    AddRecurrenceResponse recurringInfoResponse = (AddRecurrenceResponse)service.Execute(recurringInfoRequest);
                    __recurringAppointmentMasterId = recurringInfoResponse.id;

                    // Verify that the newly created recurring appointment master has same 'subject' 
                    // as the existing appointment.

                    RecurringAppointmentMaster retrievedMasterAppointment = (RecurringAppointmentMaster)service.Retrieve(RecurringAppointmentMaster.EntityLogicalName, __recurringAppointmentMasterId, new ColumnSet(true));
                    if (retrievedMasterAppointment.Subject == "Sample Appointment")
                    {
                        Console.WriteLine("Sample Appointment is converted to a recurring appointment.");
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
            #endregion Sample code

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
