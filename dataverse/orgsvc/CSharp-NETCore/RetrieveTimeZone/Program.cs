using System;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform_Dataverse_CodeSamples
{
    /// <summary>
    /// Demonstrates how to retrieve time zone information from Dataverse.
    /// </summary>
    /// <remarks>
    /// This sample shows various operations for working with time zones:
    /// - Retrieving current user's time zone settings
    /// - Getting all time zones for a locale
    /// - Finding time zones by name and locale
    /// - Retrieving time zones by ID
    /// - Converting between UTC and local time
    ///
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// </remarks>
    /// <see cref="https://learn.microsoft.com/power-apps/developer/data-platform/xrm-tooling/use-connection-strings-xrm-tooling-connect#connection-string-parameters"/>
    /// <permission cref="https://github.com/microsoft/PowerApps-Samples/blob/master/LICENSE"/>
    class Program
    {
        private static string _timeZoneName = "Central Standard Time";
        private static Guid _timeZoneId = new Guid("42B2880D-BED5-4AA3-BD69-418052D38B7E");
        private static int? _localeId;
        private static int? _timeZoneCode;

        #region Sample demonstration methods

        /// <summary>
        /// Retrieves the current user's time zone code and locale ID
        /// </summary>
        private static void RetrieveCurrentUsersSettings(ServiceClient serviceClient)
        {
            Console.WriteLine("--- Retrieving Current User Settings ---");

            var currentUserSettings = serviceClient.RetrieveMultiple(
                new QueryExpression("usersettings")
                {
                    ColumnSet = new ColumnSet("localeid", "timezonecode"),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                        {
                            new ConditionExpression("systemuserid", ConditionOperator.EqualUserId)
                        }
                    }
                }).Entities[0];

            _localeId = currentUserSettings.GetAttributeValue<int>("localeid");
            _timeZoneCode = currentUserSettings.GetAttributeValue<int>("timezonecode");

            Console.WriteLine($"Current user's locale ID: {_localeId}");
            Console.WriteLine($"Current user's time zone code: {_timeZoneCode}");
            Console.WriteLine();
        }

        /// <summary>
        /// Uses the current locale ID to retrieve all the time zones
        /// </summary>
        private static void RetrieveAllTimeZonesForLocale(ServiceClient serviceClient)
        {
            if (!_localeId.HasValue)
            {
                Console.WriteLine("Locale ID is not set. Skipping RetrieveAllTimeZonesForLocale.");
                return;
            }

            Console.WriteLine("--- Retrieving All Time Zones for Locale ---");
            Console.WriteLine($"Retrieving time zones for locale ID: {_localeId.Value}");

            var response = (GetAllTimeZonesWithDisplayNameResponse)serviceClient.Execute(
                new GetAllTimeZonesWithDisplayNameRequest
                {
                    LocaleId = _localeId.Value,
                });

            Console.WriteLine($"Found {response.EntityCollection.Entities.Count} time zones.");

            // Display first 5 time zones as examples
            int displayCount = Math.Min(5, response.EntityCollection.Entities.Count);
            for (int i = 0; i < displayCount; i++)
            {
                var timeZone = response.EntityCollection.Entities[i];
                string displayName = timeZone.GetAttributeValue<string>("userinterfacename");
                Console.WriteLine($"  - {displayName}");
            }

            if (response.EntityCollection.Entities.Count > 5)
            {
                Console.WriteLine($"  ... and {response.EntityCollection.Entities.Count - 5} more");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Retrieves a time zone by the name and the locale ID
        /// </summary>
        private static void GetTimeZoneCodeByLocaleAndName(ServiceClient serviceClient)
        {
            if (string.IsNullOrWhiteSpace(_timeZoneName) || !_localeId.HasValue)
            {
                Console.WriteLine("Time zone name or locale ID is not set. Skipping GetTimeZoneCodeByLocaleAndName.");
                return;
            }

            Console.WriteLine("--- Retrieving Time Zone by Name and Locale ---");

            var request = new GetTimeZoneCodeByLocalizedNameRequest
            {
                LocaleId = _localeId.Value,
                LocalizedStandardName = _timeZoneName
            };

            var response = (GetTimeZoneCodeByLocalizedNameResponse)serviceClient.Execute(request);

            Console.WriteLine($"Time zone code: {response.TimeZoneCode}");
            Console.WriteLine($"Locale ID: {_localeId.Value}");
            Console.WriteLine($"Time zone name: {_timeZoneName}");
            Console.WriteLine();
        }

        /// <summary>
        /// Retrieve time zone by ID
        /// </summary>
        private static void RetrieveTimeZoneById(ServiceClient serviceClient)
        {
            Console.WriteLine("--- Retrieving Time Zone by ID ---");

            var request = new RetrieveRequest
            {
                Target = new EntityReference("timezonedefinition", _timeZoneId),
                ColumnSet = new ColumnSet("standardname")
            };

            var response = (RetrieveResponse)serviceClient.Execute(request);
            string standardName = response.Entity.GetAttributeValue<string>("standardname");

            Console.WriteLine($"Time zone ID: {_timeZoneId}");
            Console.WriteLine($"Standard name: {standardName}");
            Console.WriteLine();
        }

        /// <summary>
        /// Retrieve time zones with time zone code less than 50.
        /// </summary>
        private static void RetrieveTimeZonesLessThan50(ServiceClient serviceClient)
        {
            Console.WriteLine("--- Retrieving Time Zones with Code < 50 ---");

            var request = new RetrieveMultipleRequest
            {
                Query = new QueryExpression("timezonedefinition")
                {
                    ColumnSet = new ColumnSet("timezonecode", "standardname"),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                        {
                            new ConditionExpression("timezonecode",
                                ConditionOperator.LessThan, 50)
                        }
                    }
                }
            };

            var response = (RetrieveMultipleResponse)serviceClient.Execute(request);

            Console.WriteLine($"Found {response.EntityCollection.Entities.Count} time zones with code < 50:");

            foreach (var entity in response.EntityCollection.Entities)
            {
                int? timeZoneCode = entity.GetAttributeValue<int?>("timezonecode");
                string standardName = entity.GetAttributeValue<string>("standardname");

                Console.WriteLine($"  - {standardName} (Code: {timeZoneCode?.ToString() ?? "null"})");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Retrieve the local time from the UTC time.
        /// </summary>
        private static void RetrieveLocalTimeFromUTCTime(ServiceClient serviceClient, DateTime utcTime)
        {
            if (!_timeZoneCode.HasValue)
            {
                Console.WriteLine("Time zone code is not set. Skipping RetrieveLocalTimeFromUTCTime.");
                return;
            }

            Console.WriteLine("--- Converting UTC Time to Local Time ---");

            var request = new LocalTimeFromUtcTimeRequest
            {
                TimeZoneCode = _timeZoneCode.Value,
                UtcTime = utcTime.ToUniversalTime()
            };

            var response = (LocalTimeFromUtcTimeResponse)serviceClient.Execute(request);

            Console.WriteLine($"UTC time: {utcTime:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Local time: {response.LocalTime:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Time zone code: {_timeZoneCode.Value}");
            Console.WriteLine();
        }

        /// <summary>
        /// Retrieves the UTC time from the local time
        /// </summary>
        private static void RetrieveUTCTimeFromLocalTime(ServiceClient serviceClient, DateTime localTime)
        {
            if (!_timeZoneCode.HasValue)
            {
                Console.WriteLine("Time zone code is not set. Skipping RetrieveUTCTimeFromLocalTime.");
                return;
            }

            Console.WriteLine("--- Converting Local Time to UTC Time ---");

            var request = new UtcTimeFromLocalTimeRequest
            {
                TimeZoneCode = _timeZoneCode.Value,
                LocalTime = localTime
            };

            var response = (UtcTimeFromLocalTimeResponse)serviceClient.Execute(request);

            Console.WriteLine($"Local time: {localTime:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"UTC time: {response.UtcTime:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Time zone code: {_timeZoneCode.Value}");
            Console.WriteLine();
        }

        #endregion

        #region Application setup and Main method

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

            try
            {
                // Run all time zone demonstrations
                RetrieveCurrentUsersSettings(serviceClient);
                RetrieveAllTimeZonesForLocale(serviceClient);
                GetTimeZoneCodeByLocaleAndName(serviceClient);
                RetrieveTimeZoneById(serviceClient);
                RetrieveTimeZonesLessThan50(serviceClient);
                RetrieveLocalTimeFromUTCTime(serviceClient, new DateTime(1981, 6, 6, 9, 5, 0));
                RetrieveUTCTimeFromLocalTime(serviceClient, new DateTime(2012, 1, 1, 0, 0, 0));
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred:");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Pause program execution before resource cleanup.
                Console.WriteLine();
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                serviceClient.Dispose();
            }
        }

        #endregion
    }
}
