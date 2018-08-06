using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
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
        private static string _timeZoneName;
        private static Guid? _timeZoneId;
        private static int? _localeId;
        private static int? _timeZoneCode;
        
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


            _timeZoneId = new Guid("42B2880D-BED5-4AA3-BD69-418052D38B7E");
            _timeZoneName = "Central Standard Time";
            RetrieveCurrentUsersSettings(service);
            RetrieveAllTimeZonesForLocale(service);
            GetTimeZoneCodeByLocaleAndName(service);
            RetrieveTimeZoneById(service);
            RetrieveTimeZonesLessThan50(service);
            RetrieveLocalTimeFromUTCTime(service, new DateTime(1981, 6, 6, 9, 5, 0));
            RetrieveUTCTimeFromLocalTime(service, new DateTime(2012, 1, 1, 0, 0, 0));

        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            // There is no need of clean up as this sample does not create any sample records
        }


        
        
                    /// <summary>
                    /// Retrieves the current users timezone code and locale id
                    /// </summary>
                    private static void RetrieveCurrentUsersSettings(CrmServiceClient service)
        {
            var currentUserSettings = service.RetrieveMultiple(
                new QueryExpression(UserSettings.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet("localeid", "timezonecode"),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                        {
                            new ConditionExpression("systemuserid", ConditionOperator.EqualUserId)
                        }
                    }
                }).Entities[0].ToEntity<UserSettings>();

            _localeId = currentUserSettings.LocaleId;
            _timeZoneCode = currentUserSettings.TimeZoneCode;
        }

        /// <summary>
        /// Uses the current locale id to retrieve all the timezones 
        /// </summary>
        private static void RetrieveAllTimeZonesForLocale(CrmServiceClient service)
        {
            if (!_localeId.HasValue)
                return;

            Console.WriteLine(String.Concat("Retrieving time zones for locale id: ",
                _localeId.Value));

            var response = (GetAllTimeZonesWithDisplayNameResponse)service.Execute(
                new GetAllTimeZonesWithDisplayNameRequest
                {
                    LocaleId = _localeId.Value,
                });

            for (int i = 0; i < response.EntityCollection.Entities.Count; i++)
            {
                var timeZoneDefinition = response.EntityCollection
                    .Entities[i].ToEntity<TimeZoneDefinition>();

                Console.WriteLine(String.Concat(timeZoneDefinition.UserInterfaceName,
                    " found for ", _localeId.Value));
            }
        }

        /// <summary>
        /// Retrieves a timezone by the name and the locale ID
        /// </summary>
        private static void GetTimeZoneCodeByLocaleAndName(CrmServiceClient service)
        {
            if (string.IsNullOrWhiteSpace(_timeZoneName) || !_localeId.HasValue)
                return;

            var request = new GetTimeZoneCodeByLocalizedNameRequest
            {
                LocaleId = _localeId.Value,
                LocalizedStandardName = _timeZoneName
            };

            var response = (GetTimeZoneCodeByLocalizedNameResponse)
                service.Execute(request);

            Console.WriteLine(String.Concat("Time zone code: ", response.TimeZoneCode,
                " retrieved for locale id: ", _localeId.Value, ", and time zone name: ",
                _timeZoneName));
        }

        /// <summary>
        /// Retieve time zone by id
        /// </summary>
        private static void RetrieveTimeZoneById(CrmServiceClient service)
        {
            if (!_timeZoneId.HasValue)
                return;

            // In addition to the RetrieveRequest message, 
            // you may use the IOrganizationService.Retrieve method
            var request = new RetrieveRequest
            {
                Target = new EntityReference(
                    TimeZoneDefinition.EntityLogicalName, _timeZoneId.Value),
                ColumnSet = new ColumnSet("standardname")
            };

            var response = (RetrieveResponse)service.Execute(request);

            Console.WriteLine(String.Concat("Retrieve request on time zone ",
                _timeZoneId.Value, ".  Name: ", response.Entity.
                ToEntity<TimeZoneDefinition>().StandardName));
        }

        /// <summary>
        /// Retrieve time zones less than 50.
        /// </summary>
        private static void RetrieveTimeZonesLessThan50(CrmServiceClient service)
        {
            // In addition to the RetrieveMultipleRequest message,
            // you may use the IOrganizationService.RetrieveMultiple method.
            var request = new RetrieveMultipleRequest
            {
                Query = new QueryExpression(TimeZoneDefinition.EntityLogicalName)
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

            Console.WriteLine("Retrieving all timezones with a timezonecode less than 50");
            var response = (RetrieveMultipleResponse)service.Execute(request);

            foreach (var item in response.EntityCollection.Entities)
            {
                var timeZone = item.ToEntity<TimeZoneDefinition>();

                Console.WriteLine(String.Concat("Time zone name: ",
                    timeZone.StandardName, ", time zone code: ",
                    !timeZone.TimeZoneCode.HasValue ? "null" :
                    timeZone.TimeZoneCode.Value.ToString()));
            }

        }

        /// <summary>
        /// Retrive the local time from the UTC time.
        /// </summary>
        /// <param name="utcTime"></param>
        private static void RetrieveLocalTimeFromUTCTime(CrmServiceClient service, DateTime utcTime)
        {
            if (!_timeZoneCode.HasValue)
                return;

            var request = new LocalTimeFromUtcTimeRequest
            {
                TimeZoneCode = _timeZoneCode.Value,
                UtcTime = utcTime.ToUniversalTime()
            };

            var response = (LocalTimeFromUtcTimeResponse)service.Execute(request);

            Console.WriteLine(String.Concat("Calling LocalTimeFromUtcTimeRequest.  UTC time: ",
                utcTime.ToString("MM/dd/yyyy HH:mm:ss"), ". Local time: ",
                response.LocalTime.ToString("MM/dd/yyyy HH:mm:ss")));
        }

        /// <summary>
        /// Retrieves the utc time from the local time
        /// </summary>
        /// <param name="localTime"></param>
        private static void RetrieveUTCTimeFromLocalTime(CrmServiceClient service, DateTime localTime)
        {
            if (!_timeZoneCode.HasValue)
                return;

            var request = new UtcTimeFromLocalTimeRequest
            {
                TimeZoneCode = _timeZoneCode.Value,
                LocalTime = localTime
            };

            var response = (UtcTimeFromLocalTimeResponse)service.Execute(request);

            Console.WriteLine(String.Concat("Calling UtcTimeFromLocalTimeRequest.  Local time: ",
                localTime.ToString("MM/dd/yyyy HH:mm:ss"), ". UTC time: ",
                response.UtcTime.ToString("MM/dd/yyyy HH:mm:ss")));
        }
    }
}
