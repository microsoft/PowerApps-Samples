using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates how to create, retrieve, update, and delete a recurring appointment series.
    /// </summary>
    /// <remarks>
    /// This sample shows how to perform CRUD operations on a recurring appointment series
    /// using the IOrganizationService.Create, Retrieve, Update, and Delete methods.
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
            // No setup required for this sample
        }

        /// <summary>
        /// Demonstrates CRUD operations on a recurring appointment series
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Console.WriteLine("Creating recurring appointment...");

            // Define recurrence pattern values
            int RecurrencePatternTypeWeekly = 1;
            int DayOfWeekThursday = 0x10;
            int RecurrenceRulePatternEndTypeOccurrences = 2;

            // Create a recurring appointment
            var newRecurringAppointment = new Entity("recurringappointmentmaster")
            {
                ["subject"] = "Sample Recurring Appointment",
                ["starttime"] = DateTime.Now.AddHours(1),
                ["endtime"] = DateTime.Now.AddHours(2),
                ["recurrencepatterntype"] = new OptionSetValue(RecurrencePatternTypeWeekly),
                ["interval"] = 1,
                ["daysofweekmask"] = DayOfWeekThursday,
                ["patternstartdate"] = DateTime.Today,
                ["occurrences"] = 10,
                ["patternendtype"] = new OptionSetValue(RecurrenceRulePatternEndTypeOccurrences)
            };

            Guid recurringAppointmentMasterId = service.Create(newRecurringAppointment);
            entityStore.Add(new EntityReference("recurringappointmentmaster", recurringAppointmentMasterId));

            Console.WriteLine("Created {0}.", newRecurringAppointment["subject"]);

            // Retrieve the newly created recurring appointment
            Console.WriteLine("\nRetrieving recurring appointment...");
            var recurringAppointmentQuery = new QueryExpression
            {
                EntityName = "recurringappointmentmaster",
                ColumnSet = new ColumnSet("subject", "interval", "occurrences"),
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

            EntityCollection results = service.RetrieveMultiple(recurringAppointmentQuery);
            Entity retrievedRecurringAppointment = results.Entities.FirstOrDefault();

            if (retrievedRecurringAppointment != null)
            {
                Console.WriteLine("Retrieved the recurring appointment.");

                // Update the recurring appointment
                Console.WriteLine("\nUpdating recurring appointment...");
                // Update the subject, number of occurrences to 5, and appointment interval to 2
                retrievedRecurringAppointment["subject"] = "Updated Recurring Appointment";
                retrievedRecurringAppointment["occurrences"] = 5;
                retrievedRecurringAppointment["interval"] = 2;
                service.Update(retrievedRecurringAppointment);

                Console.WriteLine("Updated the subject, occurrences, and interval of the recurring appointment.");
            }
            else
            {
                Console.WriteLine("Could not retrieve the recurring appointment.");
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
