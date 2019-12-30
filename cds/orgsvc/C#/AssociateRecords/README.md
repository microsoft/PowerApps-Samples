# Associate and disassociate entities

This sample shows how to associate and disassociate entities using the [IOrganizationService.Associate](https://docs.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.associate?view=dynamics-general-ce-9) and [IOrganization.Disassociate](https://docs.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.disassociate?view=dynamics-general-ce-9) messages. 

## How to run this sample

See [How to run this sample](https://github.com/microsoft/PowerApps-Samples/blob/master/cds/README.md) for information about how to run this sample.

## What this sample does

Entity records are associated to each other using lookup attributes on the related entity. The simplest way to associate two entity records in a one-to-many relationship is to use an EntityReference to set the value of a lookup attribute on the related entity.

The simplest way to disassociate two entity records in a one-to-many relationship is to set the value of the lookup attribute to null.

Relationships using an many-to-many relationship also depend on lookup attributes on the intersect entity that supports the many-to-many relationship. These relationship are defined by the existence of entity records in that intersect entity. While you can interact with the intersect entity directly, it is much easier to use the API to do this for you.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current  version of the org. 
1. Creates required data that this sample requires.

### Demonstrate

1. The `RetrieveUserQueueRequest` message gets the known private queues for the user.
1. The `AddToQueueRequest` message moves the record from source queue to destination queue. 

### Clean up

Displays an option to delete all the data created in the sample. The deletion is optional in case you want to examine the data created by the sample. You can manually delete the data to achieve same results.