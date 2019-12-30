# Create and update entity metadata

This topic shows how to programmatically create a custom user-owned entity called **Bank Account** and add four different types of attributes to it.

You can also create organization-owned custom entities. More information: [Entity ownership](https://docs.microsoft.com/dynamics365/customerengagement/on-premises/developer/introduction-entities#entity-ownership)

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/cds/README.md) for information about how to run this sample.

## What this sample does

The `CreateEntityRequest` message is intended to be used in a scenario where it contains  the data that is needed to create a custom entity, and optionally, to add it to a specified unmanaged solution.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

1. The `createrequest` method creates the custom entity. 
2. The `Entity` method is used to define the entity.
3. The `StringAttributeMetadata` method defines the primary attribute of the entity.
4. The `CreateBankNameAttributeRequest` method creates a string attribute to the entity.
5. The `CreateBalanceAttributeRequest` method creates a money attribute to the entity.
6. The `CreateCheckedDateRequest` method creates a DateTime attribute to the entity.

### Clean up

Display an option to delete the records created in the [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
