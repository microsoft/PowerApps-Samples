# Sample: Import files as web resources 

This sample shows how to import files as web resources.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

This sample will show how to use the `SolutionUniqueName` optional parameter to associate a web resource with a specific solution when it is created.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. The `CreateRequiredRecords` class creates a publisher and a solution required for the sample when adding the web resources.


### Demonstrate

1. The `XDocument` method reads the descriptive data from the XML files. 
1. The `WebResource` is used to set the web resource properties.
1. The `CreateRequest` method is used to add optional parameters.

### Clean up

Display an option to delete the sample data that is created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.

