# Make a report available or unavailable to organization

This sample shows how to make a report available or unavailable to an organization.

## How to run this sample

See [How to run this sample](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## How this sample works

In order to simulate the scenario described above, the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. The `CreateRequiredRecords` method creates any table records that this sample requires.

### Demonstrate

1. The `service.Retrieve` method retrieves existing personal report.
2. Set `IsPersonal` parameter to false.
3. The `service.Update` method makes the report available for the organization.
4. Set `IsPersonal` parameter to true. This makes the report unavailable for the organization.

### Clean up

Display an option to delete the sample data that is created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
