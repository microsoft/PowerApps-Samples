# Convert an appointment to a recurring appointment

This sample shows how to convert an appointment to a recurring appointment series using the [AddRecurrenceRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.addrecurrencerequest) message.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `AddRecurrenceRequest` message is intended to be used in a scenario where it contains the data that is needed to add recurrence information to an existing appointment.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Creates a sample appointment that is later converted into a recurring appointment.

### Demonstrate

1. Specifies the recurrence information that needs to be added to the appointment created in the [Setup](#setup).
2. Defines the possible recurrence pattern values (Daily, Weekly, Monthly, Yearly).
3. Defines the possible values for days of the week.
4. Defines the possible values for the recurrence rule pattern end type (NoEndDate, Occurrences, PatternEndDate).
5. Creates a `RecurringAppointmentMaster` entity with the recurrence information set to weekly on Thursdays for 5 occurrences.
6. Uses the `AddRecurrenceRequest` message to convert the created appointment into a recurring appointment.
7. Verifies that the recurring appointment master was created successfully with the same subject as the original appointment.

### Clean up

Display an option to delete the sample data created in [Setup](#setup). If you opt **Y**, it deletes all the records created. The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the sample data to achieve the same result.
