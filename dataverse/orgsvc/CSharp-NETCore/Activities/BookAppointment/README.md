---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates how to book or schedule an appointment using the BookRequest message."
---

# Book an Appointment

Demonstrates how to book or schedule an appointment using the BookRequest message.

## What this sample does

The `BookRequest` message is used to book or schedule an appointment. This validates that the appointment can be scheduled according to the calendars of the required attendees and creates the appointment in the system.

## How this sample works

### Setup

This sample requires no setup. It uses the current user as both organizer and attendee.

### Run

The sample demonstrates the following steps:

1. Gets the current user information using WhoAmIRequest
2. Creates an ActivityParty for the current user
3. Creates an appointment entity with:
   - Subject and description
   - Scheduled start and end times
   - Location
   - Required attendees and organizer
4. Uses BookRequest to validate and create the appointment
5. Verifies the appointment was successfully booked

### Cleanup

The sample deletes the created appointment record by default.

## Demonstrates

- Using `WhoAmIRequest` to get current user information
- Creating an `ActivityParty` entity to represent appointment participants
- Using `BookRequest` message to book an appointment
- Working with appointment scheduling attributes

## Sample Output

```
Connected to Dataverse.

Booking an appointment...
Successfully booked appointment: Test Appointment
Appointment ID: 12345678-1234-1234-1234-123456789012

Deleting 1 created record(s)...
Records deleted.

Press any key to exit.
```

## See also

[BookRequest Class](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.bookrequest)
[Appointment entities](https://learn.microsoft.com/power-apps/developer/data-platform/appointment-entities)
[Activity entities](https://learn.microsoft.com/power-apps/developer/data-platform/activity-entities)
