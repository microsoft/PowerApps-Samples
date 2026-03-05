using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates how to end a recurring appointment series.
    /// </summary>
    /// <remarks>
    /// This sample shows how to use the DeleteOpenInstancesRequest message to end a
    /// recurring appointment series to the last occurring past instance date.
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

            // Define recurrence pattern values
            int RecurrencePatternTypeWeekly = 1;
            int DayOfWeekThursday = 0x10;
            int RecurrenceRulePatternEndTypeOccurrences = 2;

            // Create a new recurring appointment
            var newRecurringAppointment = new Entity("recurringappointmentmaster")
            {
                ["subject"] = "Sample Recurring Appointment",
                ["starttime"] = DateTime.Now.AddHours(1),
                ["endtime"] = DateTime.Now.AddHours(2),
                ["recurrencepatterntype"] = new OptionSetValue(RecurrencePatternTypeWeekly),
                ["interval"] = 1,
                ["daysofweekmask"] = DayOfWeekThursday,
                ["patternstartdate"] = DateTime.Today,
                ["occurrences"] = 5,
                ["patternendtype"] = new OptionSetValue(RecurrenceRulePatternEndTypeOccurrences)
            };

            Guid recurringAppointmentMasterId = service.Create(newRecurringAppointment);
            entityStore.Add(new EntityReference("recurringappointmentmaster", recurringAppointmentMasterId));
            Console.WriteLine("Created {0} with {1} occurrences.", newRecurringAppointment["subject"], newRecurringAppointment["occurrences"]);
        }

        /// <summary>
        /// Demonstrates ending a recurring appointment series
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Console.WriteLine("\nEnding recurring appointment series...");

            // Get the recurring appointment ID from entityStore
            Guid recurringAppointmentMasterId = entityStore.First(e => e.LogicalName == "recurringappointmentmaster").Id;

            // Retrieve the recurring appointment series
            var retrievedRecurringAppointmentSeries = service.Retrieve(
                "recurringappointmentmaster",
                recurringAppointmentMasterId,
                new ColumnSet(true));

            // Use the DeleteOpenInstances message to end the series to the
            // last occurring past instance date w.r.t. the series end date
            // (i.e., 20 days from today). Effectively, that means that the
            // series will end after the third instance (day 14) as this
            // instance is the last occurring past instance w.r.t the specified
            // series end date (20 days from today).
            // Also specify that the state of past instances (w.r.t. the series
            // end date) be set to 'completed'.
            var endAppointmentSeries = new DeleteOpenInstancesRequest
            {
                Target = retrievedRecurringAppointmentSeries,
                SeriesEndDate = DateTime.Today.AddDays(20),
                StateOfPastInstances = 2 // AppointmentState.Completed
            };
            service.Execute(endAppointmentSeries);

            Console.WriteLine("The recurring appointment series has been ended after the third occurrence.");
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
