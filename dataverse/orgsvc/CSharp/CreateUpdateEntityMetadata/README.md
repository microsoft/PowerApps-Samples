# Create and update table definitions

This topic shows how to programmatically create a custom user-owned table called **Bank Account** and add four different types of columns to it.

You can also create organization-owned custom entities. More information: [Table ownership](https://learn.microsoft.com/dynamics365/customerengagement/on-premises/developer/introduction-entities#entity-ownership)

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `CreateEntityRequest` message is intended to be used in a scenario where it contains  the data that is needed to create a custom table, and optionally, to add it to a specified unmanaged solution.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

1. The `createrequest` method creates the custom table. 
2. The `CreateEntityRequest` method is used to define the table.
3. The `StringAttributeMetadata` method defines the primary column of the table.
4. The `CreateBankNameAttributeRequest` method creates a string column to the table.
5. The `CreateBalanceAttributeRequest` method creates a money column to the table.
6. The `CreateCheckedDateRequest` method creates a DateTime column to the table.

### Clean up

Display an option to delete the records created in the [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
