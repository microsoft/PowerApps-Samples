using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates querying working hours schedules for users
    /// </summary>
    /// <remarks>
    /// This sample shows how to use QueryScheduleRequest to retrieve
    /// the working hours and availability of a user in Dataverse.
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// </remarks>
    class Program
    {
        #region Sample Methods

        private static void Setup(ServiceClient service)
        {
            // No setup required for this sample
            Console.WriteLine("Setup complete.");
            Console.WriteLine();
        }

        private static void Run(ServiceClient service)
        {
            Console.WriteLine("Querying working hours for current user...");
            Console.WriteLine();

            // Get the current user's information
            var userRequest = new WhoAmIRequest();
            var userResponse = (WhoAmIResponse)service.Execute(userRequest);

            Console.WriteLine($"Current User ID: {userResponse.UserId}");
            Console.WriteLine();

            // Retrieve the working hours of the current user
            var scheduleRequest = new QueryScheduleRequest
            {
                ResourceId = userResponse.UserId,
                Start = DateTime.Now,
                End = DateTime.Today.AddDays(7),
                TimeCodes = new TimeCode[] { TimeCode.Available }
            };

            var scheduleResponse = (QueryScheduleResponse)service.Execute(scheduleRequest);

            // Display the results
            if (scheduleResponse.TimeInfos.Length > 0)
            {
                Console.WriteLine("Successfully queried the working hours of the current user.");
                Console.WriteLine($"Found {scheduleResponse.TimeInfos.Length} time slot(s) with availability.");
                Console.WriteLine();

                // Display first few time slots as examples
                int displayCount = Math.Min(5, scheduleResponse.TimeInfos.Length);
                Console.WriteLine($"Displaying first {displayCount} available time slot(s):");
                Console.WriteLine();

                for (int i = 0; i < displayCount; i++)
                {
                    var timeInfo = scheduleResponse.TimeInfos[i];
                    Console.WriteLine($"Time Slot {i + 1}:");
                    Console.WriteLine($"  Start: {timeInfo.Start}");
                    Console.WriteLine($"  End: {timeInfo.End}");
                    Console.WriteLine($"  Time Code: {timeInfo.TimeCode}");
                    Console.WriteLine($"  Sub Code: {timeInfo.SubCode}");
                    Console.WriteLine();
                }

                if (scheduleResponse.TimeInfos.Length > displayCount)
                {
                    Console.WriteLine($"... and {scheduleResponse.TimeInfos.Length - displayCount} more time slot(s)");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("No available time slots found for the current user in the next 7 days.");
                Console.WriteLine("This may indicate that the user has no working hours configured.");
                Console.WriteLine();
            }
        }

        private static void Cleanup(ServiceClient service, bool deleteCreatedRecords)
        {
            Console.WriteLine("Cleaning up...");
            // No records created in this sample, nothing to clean up
            Console.WriteLine("No records to delete.");
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
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception: {0}", ex.InnerException.Message);
                }
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
