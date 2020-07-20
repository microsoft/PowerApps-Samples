# Sample: Late-bound entity operations

This sample demonstrates the create, retrieve, update, and delete operations on an account using the late bound Entity class.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/cds/README.md) for information about how to run this sample.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

1. Instantiates the account object.
1. Creates an account record.
1. Retrieves the account and its attributes.
1. Updates the postal1 code attribute and set the postal2 code to null.
1. Update the account. 
1. Prompts to delete the account records created.

### Clean up

There is no clean up required, since all the sample records that are created are deleted in the demonstrate section.
