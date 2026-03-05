# End a recurring appointment series

The following sample shows how to end a recurring appointment series by using the [DeleteOpenInstancesRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.deleteopeninstancesrequest) message.

## How to run this sample

1. Navigate to the Activities folder: `cd CSharp-NETCore/Activities/`
2. Edit `appsettings.json` with your Dataverse environment connection string
3. Build: `dotnet build EndRecurringAppointment`
4. Run: `dotnet run --project EndRecurringAppointment`

## What this sample does

The `DeleteOpenInstanceRequest` message is intended to be used in a scenario where it contains the data that is needed to delete instances of a recurring appointment master that have an "Open" state.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Defines the recurrence pattern values, possible values for days, and recurrence rule pattern end type values.
2. Creates a new recurring appointment that is required for the sample.

### Demonstrate

1. Retrieves the recurring appointment series that was created in Setup.
2. The `DeleteOpenInstanceRequest` message ends the recurring appointment series to the last occurring past instance date w.r.t. the series end date.

### Clean up

Display an option to delete the sample data created in Setup. The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
