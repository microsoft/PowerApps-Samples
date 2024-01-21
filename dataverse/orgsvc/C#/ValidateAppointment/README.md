# Validate an appointment

This sample shows how to validate an appointment using the [ValidateRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.validaterequest) message.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `ValidateRequest` message is intended to be used in the scenario where it contains data that is needed to verify that an appointment or service appointment (service activity) has valid available resources for the activity, duration, and site, as appropriate.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks the version of the org.
2. Creates sample contact and activity party records.
3. Creates sample appointment.

### Demonstrate

1. Retrieves the appointment to be validated.
2. The `ValidateRequest` message validates the appointment created in the Setup(#setup).

### Clean up

Display an option to delete the records in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
