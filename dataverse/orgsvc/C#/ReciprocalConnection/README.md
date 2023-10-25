# Create a reciprocal connection role (early bound)

This sample shows how to create the reciprocal connection roles. It creates a connection role for an account and a connection role for a contact, and then makes them reciprocal by associating them with each other.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `ConnectionRole` and `ConnectionRoleObjectTypeCode` messages are intended to be used in a scenario where they contain data that is required to create a new connection role and connection role object type.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. The `ConnectionRole` message creates connection roles required for the sample.
3. The `ConnectionRoleObjectTypeCode` message creates the connection role object type code record for account.
4. The `AssociateRequest` message associates the connection roles with each other.

### Demonstrate

1. Perform initial request and cache the results, including the `DataToken`
1. Update the records created in [Setup](#setup)
1. Perform a second request, this time passing the `DataVersion` with the `DataToken` value retrieved from the initial request.
1. Show the table changes returned by the second request

### Clean up

Display an option to delete the sample data that is created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
