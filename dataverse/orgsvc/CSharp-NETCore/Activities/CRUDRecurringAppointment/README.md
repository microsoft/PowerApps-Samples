# Create, retrieve, update, and delete a recurring appointment

This sample shows how to create, retrieve, update, and delete a recurring appointment series. This sample uses the following common methods:

- [IOrganizationService.Create](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.create)
- [IOrganizationService.Retrieve](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.retrieve)
- [IOrganizationService.Update](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.update)
- [IOrganizationService.Delete](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.delete)

## How to run this sample

1. Navigate to the Activities folder: `cd CSharp-NETCore/Activities/`
2. Edit `appsettings.json` with your Dataverse environment connection string
3. Build: `dotnet build CRUDRecurringAppointment`
4. Run: `dotnet run --project CRUDRecurringAppointment`

## What this sample does

The `IOrganizationService` methods are used to perform CRUD operations on a recurring appointment series.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

No setup required for this sample.

### Demonstrate

1. Define values for recurrence pattern types, days of the week, and recurrence rule pattern end type.
2. The `Create` method creates a recurring appointment with weekly recurrence.
3. The `QueryExpression` method retrieves the newly created recurring appointment.
4. The `Update` method updates the subject, number of occurrences to 5, and appointment interval to 2 for the retrieved recurring appointment.

### Clean up

Display an option to delete the records created during execution. The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
