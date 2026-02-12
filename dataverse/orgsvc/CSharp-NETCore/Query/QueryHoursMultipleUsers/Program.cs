using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates querying the working hours of multiple users
    /// </summary>
    /// <remarks>
    /// This sample shows how to retrieve the working hours of multiple users
    /// by using the QueryMultipleSchedulesRequest message.
    ///
    /// IMPORTANT: This sample requires a user manually created in your environment:
    /// First Name: Kevin
    /// Last Name: Cook
    /// Security Role: Sales Manager
    /// UserName: kcook@yourorg.onmicrosoft.com
    ///
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// </remarks>
    class Program
    {
        // Store user IDs used in the sample
        private static Guid _currentUserId;
        private static Guid _otherUserId;

        #region Sample Methods

        private static void Setup(ServiceClient service)
        {
            Console.WriteLine("Setting up sample data...");

            // Get the current user's information
            var userRequest = new WhoAmIRequest();
            var userResponse = (WhoAmIResponse)service.Execute(userRequest);
            _currentUserId = userResponse.UserId;
            Console.WriteLine($"Current User ID: {_currentUserId}");

            // Try to retrieve the manually created user (Kevin Cook, Sales Manager)
            _otherUserId = RetrieveUserByName(service, "Kevin", "Cook");

            if (_otherUserId == Guid.Empty)
            {
                throw new Exception(
                    "Required user not found. Please manually create a user in your environment:\n" +
                    "First Name: Kevin\n" +
                    "Last Name: Cook\n" +
                    "Security Role: Sales Manager\n" +
                    "UserName: kcook@yourorg.onmicrosoft.com");
            }

            Console.WriteLine($"Found user Kevin Cook with ID: {_otherUserId}");
            Console.WriteLine("Setup complete.");
            Console.WriteLine();
        }

        private static void Run(ServiceClient service)
        {
            Console.WriteLine("Querying working hours of multiple users...");
            Console.WriteLine();

            // Create the request to query multiple schedules
            var scheduleRequest = new QueryMultipleSchedulesRequest
            {
                // Specify the resource IDs (users) to query
                ResourceIds = new Guid[] { _currentUserId, _otherUserId },

                // Set the time range for the query (now to 7 days from now)
                Start = DateTime.Now,
                End = DateTime.Today.AddDays(7),

                // Specify we want available time blocks
                TimeCodes = new TimeCode[] { TimeCode.Available }
            };

            // Execute the request
            var scheduleResponse = (QueryMultipleSchedulesResponse)service.Execute(scheduleRequest);

            // Verify if some data is returned for the availability of the users
            if (scheduleResponse.TimeInfos.Length > 0)
            {
                Console.WriteLine($"Successfully queried the working hours of {scheduleRequest.ResourceIds.Length} users.");
                Console.WriteLine($"Retrieved {scheduleResponse.TimeInfos.Length} time slot(s) with availability information.");
            }
            else
            {
                Console.WriteLine("No available time slots found for the users in the next 7 days.");
                Console.WriteLine("This may indicate that the users have no working hours configured.");
            }
        }

        private static void Cleanup(ServiceClient service, bool deleteCreatedRecords)
        {
            Console.WriteLine("Cleaning up...");

            // Note: This sample does not create any records that need to be deleted.
            // It only queries existing user working hours information.

            Console.WriteLine("No records to delete.");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Retrieves a user by first and last name
        /// </summary>
        /// <param name="service">The service client</param>
        /// <param name="firstName">The user's first name</param>
        /// <param name="lastName">The user's last name</param>
        /// <returns>The user's ID, or Guid.Empty if not found</returns>
        private static Guid RetrieveUserByName(ServiceClient service, string firstName, string lastName)
        {
            // Query for the user by first and last name
            var userQuery = new QueryExpression("systemuser")
            {
                ColumnSet = new ColumnSet("systemuserid", "firstname", "lastname", "domainname"),
                Criteria = new FilterExpression(LogicalOperator.And)
            };
            userQuery.Criteria.AddCondition("firstname", ConditionOperator.Equal, firstName);
            userQuery.Criteria.AddCondition("lastname", ConditionOperator.Equal, lastName);
            userQuery.Criteria.AddCondition("isdisabled", ConditionOperator.Equal, false);

            var results = service.RetrieveMultiple(userQuery);

            if (results.Entities.Count > 0)
            {
                return results.Entities[0].Id;
            }

            return Guid.Empty;
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

                // Display additional details for common issues
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
