using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates how to reschedule and cancel appointment instances in a recurring appointment series.
    /// </summary>
    /// <remarks>
    /// This sample shows how to reschedule and cancel appointment instances in a recurring appointment series
    /// using the RescheduleRequest message and SetStateRequest message.
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

            // Create a new recurring appointment
            var newRecurringAppointment = new Entity("recurringappointmentmaster")
            {
                ["subject"] = "Sample Recurring Appointment",
                ["starttime"] = DateTime.Now.AddHours(1),
                ["endtime"] = DateTime.Now.AddHours(2),
                ["recurrencepatterntype"] = new OptionSetValue(1), // Weekly
                ["interval"] = 1,
                ["daysofweekmask"] = 16, // Thursday
                ["patternstartdate"] = DateTime.Today,
                ["occurrences"] = 5,
                ["patternendtype"] = new OptionSetValue(2) // Occurrences
            };

            Guid recurringAppointmentMasterId = service.Create(newRecurringAppointment);
            entityStore.Add(new EntityReference("recurringappointmentmaster", recurringAppointmentMasterId));
            Console.WriteLine("Created recurring appointment: {0}", newRecurringAppointment["subject"]);
        }

        /// <summary>
        /// Demonstrates how to reschedule and cancel instances in a recurring appointment series
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Console.WriteLine("\nDemonstrating recurring appointment operations...");

            // Get the recurring appointment master ID
            Guid recurringAppointmentMasterId = entityStore[0].Id;

            // Retrieve the individual appointment instance that falls on or after
            // 10 days from today. Basically this will be the second instance in the
            // recurring appointment series.
            var instanceQuery = new QueryExpression
            {
                EntityName = "appointment",
                ColumnSet = new ColumnSet("activityid", "scheduledstart", "scheduledend"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression
                        {
                            AttributeName = "seriesid",
                            Operator = ConditionOperator.Equal,
                            Values = { recurringAppointmentMasterId }
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

            if (individualAppointments.Entities.Count > 0)
            {
                #region Reschedule an instance of recurring appointment

                // Update the scheduled start and end dates of the appointment
                // to reschedule it.
                var firstAppointment = individualAppointments.Entities[0];
                var updateAppointment = new Entity("appointment")
                {
                    Id = firstAppointment.Id,
                    ["scheduledstart"] = ((DateTime)firstAppointment["scheduledstart"]).AddHours(1),
                    ["scheduledend"] = ((DateTime)firstAppointment["scheduledend"]).AddHours(2)
                };

                var reschedule = new RescheduleRequest
                {
                    Target = updateAppointment
                };

                service.Execute(reschedule);
                Console.WriteLine("Rescheduled the second instance of the recurring appointment.");

                #endregion Reschedule an instance of recurring appointment

                #region Cancel an instance of recurring appointment

                // Cancel the last instance of the appointment. The status of this appointment
                // instance is set to 'Canceled'. You can view this appointment instance under
                // the 'All Activities' view.
                var lastAppointment = individualAppointments.Entities.Last();
                var appointmentRequest = new SetStateRequest
                {
                    State = new OptionSetValue(3), // Canceled
                    Status = new OptionSetValue(4),
                    EntityMoniker = new EntityReference("appointment", lastAppointment.Id)
                };

                service.Execute(appointmentRequest);
                Console.WriteLine("Canceled the last instance of the recurring appointment.");

                #endregion Cancel an instance of recurring appointment
            }
            else
            {
                Console.WriteLine("No appointment instances found to reschedule or cancel.");
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
