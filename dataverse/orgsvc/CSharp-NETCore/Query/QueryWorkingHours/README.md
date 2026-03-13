---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates querying working hours schedules for users in Dataverse"
---

# QueryWorkingHours

Demonstrates querying working hours schedules for users in Dataverse

## What this sample does

This sample shows how to:
- Use WhoAmIRequest to get the current user's information
- Use QueryScheduleRequest to retrieve working hours and availability
- Query available time slots for a specific time period
- Process and display TimeInfo results from schedule queries

QueryScheduleRequest enables applications to retrieve calendar and schedule information, which is useful for appointment scheduling, resource allocation, and availability checking.

## How this sample works

### Setup

No setup is required for this sample. It queries the working hours of the currently authenticated user.

### Run

The main demonstration:
1. Executes WhoAmIRequest to get the current user's ID
2. Creates a QueryScheduleRequest with:
   - ResourceId set to the current user's ID
   - Start time set to now
   - End time set to 7 days from now
   - TimeCodes set to Available
3. Executes the QueryScheduleRequest
4. Displays available time slots, including:
   - Start and end times
   - Time codes (Available, Busy, etc.)
   - Sub codes for additional detail
5. Shows the first 5 available time slots as examples

### Cleanup

No cleanup is required. This sample does not create any records.

## Demonstrates

This sample demonstrates:
- **WhoAmIRequest**: Getting the current authenticated user's information
- **QueryScheduleRequest**: Querying calendar and schedule data
- **TimeCode enumeration**: Specifying which types of time slots to retrieve
- **TimeInfo processing**: Working with schedule query results
- **Calendar/Schedule queries**: Retrieving working hours and availability information

## Sample Output

```
Connected to Dataverse.

Setup complete.

Querying working hours for current user...

Current User ID: 12345678-1234-1234-1234-123456789012

Successfully queried the working hours of the current user.
Found 35 time slot(s) with availability.

Displaying first 5 available time slot(s):

Time Slot 1:
  Start: 2/6/2026 9:00:00 AM
  End: 2/6/2026 5:00:00 PM
  Time Code: Available
  Sub Code: Unspecified

Time Slot 2:
  Start: 2/7/2026 9:00:00 AM
  End: 2/7/2026 5:00:00 PM
  Time Code: Available
  Sub Code: Unspecified

Time Slot 3:
  Start: 2/10/2026 9:00:00 AM
  End: 2/10/2026 5:00:00 PM
  Time Code: Available
  Sub Code: Unspecified

Time Slot 4:
  Start: 2/11/2026 9:00:00 AM
  End: 2/11/2026 5:00:00 PM
  Time Code: Available
  Sub Code: Unspecified

Time Slot 5:
  Start: 2/12/2026 9:00:00 AM
  End: 2/12/2026 5:00:00 PM
  Time Code: Available
  Sub Code: Unspecified

... and 30 more time slot(s)

Cleaning up...
No records to delete.

Press any key to exit.
```

## See also

[QueryScheduleRequest Class](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.queryschedulerequest)
[Schedule and appointment entities](https://learn.microsoft.com/power-apps/developer/data-platform/schedule-appointment-entities)
[Service Scheduling entities](https://learn.microsoft.com/power-apps/developer/data-platform/schedule-collections-appointments-resources-services)
[WhoAmIRequest Class](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.whoamirequest)
