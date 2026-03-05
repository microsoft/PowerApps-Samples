using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates how to convert an appointment to a recurring appointment series.
    /// </summary>
    /// <remarks>
    /// This sample shows how to use the AddRecurrenceRequest message to convert an existing
    /// appointment into a recurring appointment master with a defined recurrence pattern.
    ///
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// </remarks>
    class Program
    {
        private static readonly List<EntityReference> entityStore = new();

        #region Sample Methods

        /// <summary>
        /// Sets up sample data required for the demonstration
        /// </summary>
        private static void Setup(ServiceClient service)
        {
            Console.WriteLine("Setting up sample data...");

            // Create a sample appointment
            var appointment = new Entity("appointment")
            {
                ["subject"] = "Sample Appointment",
                ["location"] = "Office",
                ["scheduledstart"] = DateTime.Now.AddHours(1),
                ["scheduledend"] = DateTime.Now.AddHours(2)
            };

            Guid appointmentId = service.Create(appointment);
            entityStore.Add(new EntityReference("appointment", appointmentId));
            Console.WriteLine("Created Sample Appointment");
        }

        /// <summary>
        /// Demonstrates converting an appointment to a recurring appointment
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Console.WriteLine("\nConverting appointment to recurring appointment...");

            // Get the appointment ID from entityStore
            var appointmentRef = entityStore.FirstOrDefault(e => e.LogicalName == "appointment");
            if (appointmentRef == null)
            {
                Console.WriteLine("No appointment found to convert.");
                return;
            }

            // Define recurrence pattern types
            var RecurrencePatternTypes = new
            {
                Daily = 0,
                Weekly = 1,
                Monthly = 2,
                Yearly = 3
            };

            // Define days of the week
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

            // Define recurrence rule pattern end type
            var RecurrenceRulePatternEndType = new
            {
                NoEndDate = 1,
                Occurrences = 2,
                PatternEndDate = 3
            };

            // Create a recurring appointment master object with recurrence information
            var newRecurringAppointmentInfo = new Entity("recurringappointmentmaster")
            {
                ["starttime"] = DateTime.Now.AddHours(2),
                ["endtime"] = DateTime.Now.AddHours(3),
                ["recurrencepatterntype"] = new OptionSetValue(RecurrencePatternTypes.Weekly),
                ["interval"] = 1,
                ["daysofweekmask"] = DayOfWeek.Thursday,
                ["patternstartdate"] = DateTime.Today,
                ["patternendtype"] = new OptionSetValue(RecurrenceRulePatternEndType.Occurrences),
                ["occurrences"] = 5
            };

            // Use the AddRecurrence message to convert the existing appointment
            // to a recurring appointment master
            var recurringInfoRequest = new AddRecurrenceRequest
            {
                Target = newRecurringAppointmentInfo,
                AppointmentId = appointmentRef.Id
            };

            var recurringInfoResponse = (AddRecurrenceResponse)service.Execute(recurringInfoRequest);
            Guid recurringAppointmentMasterId = recurringInfoResponse.id;

            // Add the recurring appointment master to entityStore for cleanup
            // Remove the original appointment reference as it's been deleted
            entityStore.Remove(appointmentRef);
            entityStore.Add(new EntityReference("recurringappointmentmaster", recurringAppointmentMasterId));

            // Verify that the newly created recurring appointment master has the same subject
            var retrievedMasterAppointment = service.Retrieve(
                "recurringappointmentmaster",
                recurringAppointmentMasterId,
                new ColumnSet("subject"));

            if (retrievedMasterAppointment.GetAttributeValue<string>("subject") == "Sample Appointment")
            {
                Console.WriteLine("Sample Appointment is converted to a recurring appointment.");
                Console.WriteLine("Recurring Appointment Master ID: {0}", recurringAppointmentMasterId);
            }
        }

        /// <summary>
        /// Cleans up sample data created during execution
        /// </summary>
        private static void Cleanup(ServiceClient service, bool deleteCreatedRecords)
        {
            if (deleteCreatedRecords && entityStore.Count > 0)
            {
                Console.WriteLine("\nDeleting {0} created record(s)...", entityStore.Count);
                foreach (var entityRef in entityStore)
                {
                    service.Delete(entityRef.LogicalName, entityRef.Id);
                }
                Console.WriteLine("Records deleted.");
            }
        }

        #endregion

        #region Application Setup

        /// <summary>
        /// Contains the application's configuration settings.
        /// </summary>
        IConfiguration Configuration { get; }

        /// <summary>
        /// Constructor. Loads the application configuration settings from a JSON file.
        /// </summary>
        Program()
        {
            // Get the path to the appsettings file. If the environment variable is set,
            // use that file path. Otherwise, use the runtime folder's settings file.
            string? path = Environment.GetEnvironmentVariable("DATAVERSE_APPSETTINGS");
            if (path == null) path = "appsettings.json";

            // Load the app's configuration settings from the JSON file.
            Configuration = new ConfigurationBuilder()
                .AddJsonFile(path, optional: false, reloadOnChange: true)
                .Build();
        }

        static void Main(string[] args)
        {
            Program app = new();

            // Create a Dataverse service client using the default connection string.
            ServiceClient serviceClient =
                new(app.Configuration.GetConnectionString("default"));

            if (!serviceClient.IsReady)
            {
                Console.WriteLine("Failed to connect to Dataverse.");
                Console.WriteLine("Error: {0}", serviceClient.LastError);
                return;
            }

            Console.WriteLine("Connected to Dataverse.");
            Console.WriteLine();

            bool deleteCreatedRecords = true;

            try
            {
                Setup(serviceClient);
                Run(serviceClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred:");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Cleanup(serviceClient, deleteCreatedRecords);

                Console.WriteLine();
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                serviceClient.Dispose();
            }
        }

        #endregion
    }
}
