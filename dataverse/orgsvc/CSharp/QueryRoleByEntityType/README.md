# Query connection role by entity type code (early bound)

This sample shows how to use a query to find a connection role for an account table by specifying an entity type code.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

This sample shows how to use a query to find a connection role for an account table by specifying an entity type code.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks the version of the org.

### Demonstrate

1. Defines some anonymous types to define the range of possible connection property values.
2. The `ConnectionRole` creates a connection role.
3. The `QueryExpression` queries all the connection roles.
4. The `ConnectionRoleObjectTypeCode` creates a connection role object type code record for account table.

### Clean up

Display an option to delete the records in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
