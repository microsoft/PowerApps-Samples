# Retrieve time zone information

This sample shows how to retrieve time zone information.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `RetrieveAllTimeZonesForLocale` method is intended to be used in a scenario where it uses the current locale id to retrieve all the time zones.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

1. The `RetrieveCurrentUSerSettings` method retrieves the current users timezone code and locale id.
2. The `RetrieveAllTimeZonesForLocale` method uses the current locale id and retrieves all the time zones.
3. The `GetTimeZoneCodeByLocaleAndName` method retrieves the timezone by name and locale id.
4. The `RetrieveTimeZoneById` method retrieves the timezone by id.
5. The `RetrieveTimeZonesLessThan50` method retrieves time zones less than 50.
6. The `RetrieveLocalTimeFromUTCTime` method retrieves the local time from UTC time.
7. The `RetrieveUTCTimeFromLocalTime` method retrieves the UTC time from the locale time.

### Clean up

Display an option to delete the sample data that is created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
