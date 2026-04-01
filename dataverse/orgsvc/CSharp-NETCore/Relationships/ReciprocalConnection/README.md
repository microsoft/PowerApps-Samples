# Create a reciprocal connection role

This sample shows how to create the reciprocal connection roles. It creates a connection role for an account and a connection role for a contact, and then makes them reciprocal by associating them with each other.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `connectionrole` and `connectionroleobjecttypecode` entities are intended to be used in a scenario where they contain data that is required to create a new connection role and connection role object type.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

1. Creates two connection roles required for the sample
2. Creates the connection role object type code records for account and contact for both connection roles
3. Uses the `AssociateRequest` to associate the connection roles with each other (making them reciprocal)

### Clean up

Display an option to delete the sample data that is created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
