# Associate and disassociate table rows

This sample shows how to associate and disassociate tables rows using the [IOrganizationService.Associate](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.associate) and [IOrganization.Disassociate](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.disassociate) methods.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

Table records are associated to each other using lookup columns on the related table. The simplest way to associate two table records in a one-to-many relationship is to use an `EntityReference` to set the value of a lookup column on the related table.

The simplest way to disassociate two table records in a one-to-many relationship is to set the value of the lookup column to null.

Relationships using a many-to-many relationship depend on an intersect table that supports the many-to-many relationship. These relationships are defined by the existence of table records in that intersect table. While you can interact with the intersect table directly, it is much easier to use the API to do this for you.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Creates a contact record
2. Creates three account records

### Demonstrate

1. Creates a collection of entity references for the three accounts
2. Creates a relationship object that defines the relationship between contact and account
3. Uses the `Associate` method to associate the contact with the three accounts
4. Uses the `Disassociate` method to disassociate the contact from the accounts

### Clean up

Displays an option to delete all the data created in the sample. The deletion is optional in case you want to examine the data created by the sample. You can manually delete the data to achieve the same results.
