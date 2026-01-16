# Retrieve time zone information

This sample shows how to retrieve time zone information using the modern .NET 6+ SDK.

## How to run this sample

1. Update the connection string in `appsettings.json` in the parent directory with your Dataverse environment URL and credentials.

2. Build and run the sample:
   ```bash
   dotnet build
   dotnet run
   ```

## What this sample does

This sample demonstrates various time zone operations in Dataverse:

- **RetrieveCurrentUsersSettings**: Retrieves the current user's time zone code and locale ID
- **RetrieveAllTimeZonesForLocale**: Uses the current locale ID to retrieve all available time zones
- **GetTimeZoneCodeByLocaleAndName**: Retrieves a time zone by name and locale ID
- **RetrieveTimeZoneById**: Retrieves a specific time zone by its unique identifier
- **RetrieveTimeZonesLessThan50**: Retrieves time zones with a time zone code less than 50
- **RetrieveLocalTimeFromUTCTime**: Converts UTC time to local time for the user's time zone
- **RetrieveUTCTimeFromLocalTime**: Converts local time to UTC time

## How this sample works

The sample connects to Dataverse using the `ServiceClient` class and executes various message requests to work with time zone information:

1. First, it retrieves the current user's time zone settings (locale ID and time zone code)
2. It demonstrates retrieving all time zones for the user's locale
3. It shows how to find a specific time zone by name ("Central Standard Time")
4. It retrieves a time zone by its GUID
5. It queries for time zones with specific criteria (code < 50)
6. It demonstrates time conversions between UTC and local time

## Key APIs Used

- `GetAllTimeZonesWithDisplayNameRequest` / `GetAllTimeZonesWithDisplayNameResponse`
- `GetTimeZoneCodeByLocalizedNameRequest` / `GetTimeZoneCodeByLocalizedNameResponse`
- `LocalTimeFromUtcTimeRequest` / `LocalTimeFromUtcTimeResponse`
- `UtcTimeFromLocalTimeRequest` / `UtcTimeFromLocalTimeResponse`
- `QueryExpression` with `FilterExpression` and `ConditionExpression`

## Configuration

The sample uses `appsettings.json` for configuration. Example:

```json
{
  "ConnectionStrings": {
    "default": "AuthType=OAuth;Url=https://myorg.crm.dynamics.com;Username=someone@myorg.onmicrosoft.com;RedirectUri=http://localhost;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;LoginPrompt=Auto"
  }
}
```

## More information

- [Time zone entities](https://learn.microsoft.com/power-apps/developer/data-platform/time-zone-entities)
- [ServiceClient class](https://learn.microsoft.com/dotnet/api/microsoft.powerplatform.dataverse.client.serviceclient)
- [Use messages with the SDK for .NET](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/use-messages)
