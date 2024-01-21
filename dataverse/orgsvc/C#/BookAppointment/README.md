# Book an Appointment

This sample shows how to book or schedule an appointment by using the [BookRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.bookrequest) message.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `BookRequest` message is intended to be used in a scenario to book or schedule an appointment.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
1. Gets the current user information and creates the ActivityParty instance.

### Demonstrate

Creates the appointment instance using the [BookRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.bookrequest) message and verifies that the appointment has been scheduled or not.

### Clean up

Display an option to delete the records created in the [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
