# Retrieve column sharing records

This sample shows how to retrieve the `PrincipalObjectAttributeAccess` (column sharing) records for a table.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `PrincipleObjectAttributeAccess` message is intended to be used in a scenario where it retrieves the column sharing records for a table.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. The `CreateAttributeRequest` method creates the custom columns required for the sample.

### Demonstrate

1. The `WhoAMIRequest` retrieves the current user's information.
2. The `RetrieveUserPrivilegesRequest` message checks if the current user has `prvReadPOAA`.
3. The `PrincipalObjectAttributeAccess` creates `POAA` table for the custom columns created in the Setup(#setup).
4. Using the `QueryExpression` retrieve user shared permissions.

### Clean up

Display an option to delete the sample data that is created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
