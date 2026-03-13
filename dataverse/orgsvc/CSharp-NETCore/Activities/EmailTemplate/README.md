# Create an email using a template

This sample shows how to instantiate an email record by using [InstantiateTemplateRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.instantiatetemplaterequest) message.

## How to run this sample

1. Navigate to the Activities folder: `cd CSharp-NETCore/Activities/`
2. Edit `appsettings.json` with your Dataverse environment connection string
3. Build: `dotnet build EmailTemplate`
4. Run: `dotnet run --project EmailTemplate`

## What this sample does

The `InstantiateTemplateRequest` message is intended to be used in a scenario where it instantiates an email record.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Creates an account record.
2. Defines the body and subject of the email template in XML format.
3. Creates an email template.

### Demonstrate

1. The `InstantiateTemplateRequest` message is used to create an email message using a template.
2. Serialize the email message to XML and save to a file.

### Clean up

Display an option to delete the records created in Setup. The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
