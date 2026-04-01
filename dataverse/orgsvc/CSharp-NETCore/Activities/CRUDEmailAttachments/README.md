---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample shows how to create, retrieve, update, and delete email attachments in Microsoft Dataverse."
---

# Create, retrieve, update, and delete an email attachment

This sample shows how to create, retrieve, update, and delete email attachments using the following methods:

- [IOrganizationService.Create](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.create)
- [IOrganizationService.Retrieve](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.retrieve)
- [IOrganizationService.Update](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.update)
- [IOrganizationService.Delete](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.delete)

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `IOrganizationService` methods are intended to be used in a scenario where they provide programmatic access to the metadata and data for an organization.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Creates an email activity that is required for the sample.

### Demonstrate

1. Creates three email attachments (activitymimeattachment entities) with the following properties:
   - objectid: Reference to the email activity
   - objecttypecode: Set to "email"
   - subject: Attachment subject/description
   - body: Base64-encoded attachment content
   - filename: Name of the attachment file
2. Retrieves a single attachment including its id, subject, filename, and body using the Retrieve method.
3. Updates the filename of the retrieved attachment from "ExampleAttachment0.txt" to "ExampleAttachmentUpdated.txt".
4. Retrieves all attachments associated with the email activity using QueryExpression with filters on objectid and objecttypecode.

### Clean up

Display an option to delete the records created in the [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
