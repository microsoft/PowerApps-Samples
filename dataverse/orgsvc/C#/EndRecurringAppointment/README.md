# End a recurring appointment series

The following sample shows how to end a recurring appointment series by using the [DeleteOpenInstancesRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.deleteopeninstancesrequest) message.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `DeleteOpenInstanceRequest` message is intended to be used in a scenario where it contains the data that is needed to delete instances of a recurring appointment master that have an "Open" state.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. Defines the anonymous types that define possible recurrence pattern values, possible values for days, and recurrence rule pattern end type values.
3. Creates a new recurring appointment that is required for the sample.

### Demonstrate

1. The `RecurringAppointmentMaster` message retrieves a recurring appointment series that is created in the [Setup](#setup).
2. The `DeleteOpenInstanceRequest` message ends the recurring appointment series to the last occurring past instance date w.r.t. the series end date.

### Clean up

Display an option to delete the sample data created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
