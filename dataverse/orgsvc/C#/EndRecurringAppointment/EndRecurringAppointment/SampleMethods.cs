using Microsoft.Xrm.Sdk;
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

        public static Guid _recurringAppointmentMasterId;
        private static bool prompt = true;
        /// <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        /// 
        private static void SetUpSample(CrmServiceClient service)
        {
            // Check that the current version is greater than the minimum version
            if (!SampleHelpers.CheckVersion(service, new Version("7.1.0.0")))
            {
                //The environment version is lower than version 7.1.0.0
                return;
            }

            CreateRequiredRecords(service);
        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, prompt);
        }

        /// <summary>
        /// This method creates any entity records that this sample requires.
        /// Create a new recurring appointment.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
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
                PatternEndDate = 3,
            };

            // Create a new recurring appointment
            RecurringAppointmentMaster newRecurringAppointment =
                new RecurringAppointmentMaster
                {
                    Subject = "Sample Recurring Appointment",
                    StartTime = DateTime.Now.AddHours(1),
                    EndTime = DateTime.Now.AddHours(2),
                    RecurrencePatternType = new OptionSetValue(
                        RecurrencePatternTypes.Weekly),
                    Interval = 1,
                    DaysOfWeekMask = DayOfWeek.Thursday,
                    PatternStartDate = DateTime.Today,
                    Occurrences = 5,
                    PatternEndType = new OptionSetValue(
                        RecurrenceRulePatternEndType.Occurrences)

                };

            _recurringAppointmentMasterId = service.Create(newRecurringAppointment);
            Console.WriteLine("Created {0} with {1} occurrences.", newRecurringAppointment.Subject, newRecurringAppointment.Occurrences);

            return;
        }

        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user to delete 
        /// the records created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
            bool deleteRecords = true;

            if (prompt)
            {
                Console.WriteLine("\nDo you want these entity records to be deleted? (y/n)");
                String answer = Console.ReadLine();

                deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y"));
            }

            if (deleteRecords)
            {
                service.Delete(RecurringAppointmentMaster.EntityLogicalName,
                    _recurringAppointmentMasterId);

                Console.WriteLine("Entity records have been deleted.");
            }
        }
    }
}
