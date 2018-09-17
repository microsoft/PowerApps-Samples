# Book an Appointment

This sample shows how to book or schedule an appointment by using the [BookRequest](https://docs.microsoft.com/en-us/dotnet/api/microsoft.crm.sdk.messages.bookrequest?view=dynamics-general-ce-9) message.

## How to run this sample

See [How to run samples](../../../How-to-run-samples.md) for information about how to run this sample.

## What this sample does

The `BookRequest` message is intended to be used in a scenario to book or schedule an appointment.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Gets the current user information and creates the ActivityParty instance.

### Demonstrate

1. Creates the appointment instance using the [BookRequest](https://docs.microsoft.com/en-us/dotnet/api/microsoft.crm.sdk.messages.bookrequest?view=dynamics-general-ce-9) message and verifies that the appointment has been scheduled or not.

### Clean up

1. Display an option to delete the records created in the [Setup](#setup).

    The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
