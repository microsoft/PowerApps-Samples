# Create a connection (early bound)

This sample shows how to create a connection between an account and a contact table that have matching connection roles.  
  
## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

This sample shows how to create a connection between an account and a contact that have matching connection roles.  

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Creates a connection role for account and contact table.
2. Creates a related connection role object type code for account and contact table.
3. Associates the connection role with itself.

### Demonstrate

1. Creates a connection between account and contact table.
2. Assigns a connection role to a record.

### Clean up

Display an option to delete the records created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
