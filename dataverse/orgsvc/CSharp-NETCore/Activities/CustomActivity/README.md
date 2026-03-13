# Create a custom activity

The following code example demonstrates how to create a custom activity using [CreateEntityRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.createentityrequest) and [CreateAttributeRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.createattributerequest).

## How to run this sample

1. Navigate to the Activities folder: `cd CSharp-NETCore/Activities/`
2. Edit `appsettings.json` with your Dataverse environment connection string
3. Build: `dotnet build CustomActivity`
4. Run: `dotnet run --project CustomActivity`

## What this sample does

The `CreateEntityRequest` message and `CreateAttributeRequest` message is intended to be used in a scenario to create custom activity.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

No setup required for this sample.

### Demonstrate

1. Creates the custom activity table using the `CreateEntityRequest` message.
2. Creates custom attributes (FontFamily, FontColor, FontSize) to the custom activity table using `CreateAttributeRequest` message.

### Clean up

Display an option to delete the custom entity created during execution. The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
