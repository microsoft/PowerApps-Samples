# Promote an email message

This sample shows how to create an email activity instance from the specified email message in Dynamics 365 by using the [DeliverPromoteEmailRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.deliverpromoteemailrequest) message.

## How to run this sample

1. Navigate to the Activities folder: `cd CSharp-NETCore/Activities/`
2. Edit `appsettings.json` with your Dataverse environment connection string
3. Build: `dotnet build PromoteEmail`
4. Run: `dotnet run --project PromoteEmail`

## What this sample does

The `DeliverPromoteEmailRequest` message is intended to be used in a scenario where it contains data that is needed to create an email activity record from the specified email message.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

No setup required for this sample.

### Demonstrate

1. Creates a contact to send an email to (To: column).
2. The `WhoAmIRequest` retrieves the system user to send the email (From: column).
3. The `DeliverPromoteEmailRequest` message creates the request and also executes it.
4. Verify the success by querying the delivered email and verifying the status code is `sent`.

### Clean up

Display an option to delete the records created during execution. The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
