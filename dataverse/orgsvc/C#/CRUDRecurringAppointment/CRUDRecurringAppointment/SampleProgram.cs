using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
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

                    // Define an anonymous type to define the possible recurrence pattern values.
                    var RecurrencePatternTypes = new
                    {
                        Daily = 0,
                        Weekly = 1,
                        Monthly = 2,
                        Yearly = 3
                    };

                    // Define an anonymous type to define the possible values for days 
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

                    // Define an anonymous type to define the possible values  
                    // for the recurrence rule pattern end type.
                    var RecurrenceRulePatternEndType = new
                    {
                        NoEndDate = 1,
                        Occurrences = 2,
                        PatternEndDate = 3
                    };

                    // Create a recurring appointment
                    //RecurringAppointmentMaster newRecurringAppointment = new RecurringAppointmentMaster
                    var newRecurringAppointment = new RecurringAppointmentMaster
                    {
                        Subject = "Sample Recurring Appointment",
                        StartTime = DateTime.Now.AddHours(1),
                        EndTime = DateTime.Now.AddHours(2),
                        RecurrencePatternType = new OptionSetValue(RecurrencePatternTypes.Weekly),
                        Interval = 1,
                        DaysOfWeekMask = DayOfWeek.Thursday,
                        PatternStartDate = DateTime.Today,
                        Occurrences = 10,
                        PatternEndType = new OptionSetValue(RecurrenceRulePatternEndType.Occurrences)
                    };

                    recurringAppointmentMasterId = service.Create(newRecurringAppointment);
                    Console.WriteLine("Created {0}.", newRecurringAppointment.Subject);

                    // Retrieve the newly created recurring appointment
                     var recurringAppointmentQuery = new QueryExpression
                    {
                        EntityName = RecurringAppointmentMaster.EntityLogicalName,
                        ColumnSet = new ColumnSet("subject"),
                        Criteria = new FilterExpression
                        {
                            Conditions =
                        {
                            new ConditionExpression
                            {
                                AttributeName = "subject",
                                Operator = ConditionOperator.Equal,
                                Values = { "Sample Recurring Appointment" }
                            },
                            new ConditionExpression
                            {
                                AttributeName = "interval",
                                Operator = ConditionOperator.Equal,
                                Values = { 1 }
                            }
                        }
                        },
                        PageInfo = new PagingInfo
                        {
                            Count = 1,
                            PageNumber = 1
                        }
                    };

                   var retrievedRecurringAppointment =
                        service.RetrieveMultiple(recurringAppointmentQuery).
                        Entities.Select(x => (RecurringAppointmentMaster)x).FirstOrDefault();

                    Console.WriteLine("Retrieved the recurring appointment.");

                    // Update the recurring appointment.
                    // Update the following for the retrieved recurring appointment series:
                    // 1. Update the subject.
                    // 2. Update the number of occurences to 5.
                    // 3. Update the appointment interval to 2.

                    retrievedRecurringAppointment.Subject = "Updated Recurring Appointment";
                    retrievedRecurringAppointment.Occurrences = 5;
                    retrievedRecurringAppointment.Interval = 2;
                    service.Update(retrievedRecurringAppointment);

                    Console.WriteLine("Updated the subject, occurrences, and interval of the recurring appointment.");

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
