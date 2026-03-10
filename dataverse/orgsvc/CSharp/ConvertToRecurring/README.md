# Convert an appointment to a recurring appointment

This sample shows how to convert an appointment to an recurring appointment series using the [AddRecurrenceRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.addrecurrencerequest) message.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `AddRecurrenceRequest` message is intended to be used in a scenario where it contains the data that is needed to add recurrence information to an existing appointment.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Creates an sample appointment that is later converted into an recurring appointment.

### Demonstrate

1. Specifies the recurrence information that needs to be added to the appointment created in the [Setup](#setup).
2. Defines the anonymous types that define the possible recurrence pattern values and also possible values for days.
3. Defines the anonymous types that define the possible values for the recurrence rule pattern and type.
4. The `RecurringAppointmentMaster` specifies the recurrence information. 
5. The `AddRecurrence` message converts the created appointment into recurring appointment.

### Clean up

Display an option to delete the sample data created in [Setup](#setup). If you opt **Y**, it deletes all the records created. The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the sample data to achieve the same result.
