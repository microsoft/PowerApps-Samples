# Query connections by a record

This sample shows how to query connections for a particular record. It creates connections between a contact and two accounts, and then searches for the contact’s connections.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

This sample creates account and contact records and creates connection roles between them. The `QueryExpression` retrieves the connections for a particular record.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks the version of the org.
2. Defines some anonymous types to define the range of possible connection property values.
3. The `ConnectionRole` creates a connection role.
4. The `ConnectionRoleObjectType` creates a connection role object type code record for account table.
5. Creates few account and contact records for use in the connection.

### Demonstrate

The `QueryExpression` retrieves all the connections associated with the contact created in the sample.

### Clean up

Display an option to delete the records in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
