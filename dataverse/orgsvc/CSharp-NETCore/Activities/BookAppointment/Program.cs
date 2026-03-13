using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates how to book or schedule an appointment using the BookRequest message.
    /// </summary>
    /// <remarks>
    /// This sample shows how to create and book an appointment using the BookRequest message,
    /// which validates that the appointment can be scheduled and creates it in the system.
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
        /// Demonstrates how to book an appointment using BookRequest
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Console.WriteLine("Booking an appointment...");

            // Get the current user information
            var userRequest = new WhoAmIRequest();
            var userResponse = (WhoAmIResponse)service.Execute(userRequest);

            // Create the ActivityParty instance.
            var party = new Entity("activityparty")
            {
                ["partyid"] = new EntityReference("systemuser", userResponse.UserId)
            };

            // Create the appointment instance.
            var appointment = new Entity("appointment")
            {
                ["subject"] = "Test Appointment",
                ["description"] = "Test Appointment created using the BookRequest Message.",
                ["scheduledstart"] = DateTime.Now.AddHours(1),
                ["scheduledend"] = DateTime.Now.AddHours(2),
                ["location"] = "Office",
                ["requiredattendees"] = new EntityCollection(new List<Entity> { party }),
                ["organizer"] = new EntityCollection(new List<Entity> { party })
            };

            // Use the Book request message.
            var book = new BookRequest
            {
                Target = appointment
            };
            var booked = (BookResponse)service.Execute(book);
            Guid appointmentId = booked.ValidationResult.ActivityId;

            // Add to entityStore for cleanup
            entityStore.Add(new EntityReference("appointment", appointmentId));

            // Verify that the appointment has been scheduled.
            if (appointmentId != Guid.Empty)
            {
                Console.WriteLine("Successfully booked appointment: {0}", appointment["subject"]);
                Console.WriteLine("Appointment ID: {0}", appointmentId);
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
