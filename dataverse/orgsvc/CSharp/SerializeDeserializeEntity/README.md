# Serialize and deserialize a table 

This sample shows how to serialize early-bound and late-bound table instances into an XML format, and how to de-serialize from an XML format to an early-bound table instance.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `DataContractSerializer` message is intended to be used in a scenario where it Serializes and deserializes an instance of a type into an XML stream or document using a supplied data contract. This class cannot be inherited.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
1. The `CreateRequiredRecords` method creates required sample data for the sample.

### Demonstrate

1. The `DataContractSerializer` method serializes the contact records into XML and write it to the hard drive. 
1. The `earlyBoundSerializer` method deserializes the table instance.

### Clean up

Display an option to delete the records created in the [Setup](#setup).The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.

