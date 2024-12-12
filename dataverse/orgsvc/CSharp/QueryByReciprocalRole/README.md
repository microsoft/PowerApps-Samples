# Query connections by reciprocal roles (early bound)

This sample shows how to create matching roles and then find a matching role for a particular role.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

This sample shows how to create matching roles and then find a matching role for a particular role.

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. Defines some anonymous types to define the range of possible connection property values.
3. The `ConnectionRole`creates the primary connection role instance.
4. The `ConnectionRoleObjectTypeCode` creates a connection role object type code record for account and contact table.
5. The `AssociateRequest` associates the connection roles.

### Demonstrate

The `QueryExpression` retrieves all connection roles that have this role listed as reciprocal role.

### Clean up

Display an option to delete the records in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
