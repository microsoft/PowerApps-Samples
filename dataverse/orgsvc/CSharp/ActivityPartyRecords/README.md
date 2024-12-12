
# Work with activity party records

This sample creates some sample data to show you how to work with [activity party](https://learn.microsoft.com/power-apps/developer/data-platform/activityparty-entity?view=dataverse-latest) records.

For more information, see [ActivityParty entity](https://learn.microsoft.com/power-apps/developer/data-platform/activityparty-entity).

## How to run this sample

See [How to run Microsoft Dataverse samples?](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information on how to run this sample. These instructions help you setup a Dataverse connection and add credentials to the `dataverse/App.config` file of the `dataverse` repo.

## How this sample works

This sample performs the following tasks:

### Setup

Creates three contact records which are required for this sample.

### Demonstrate

1. Retrieves the contact records created from setup.
2. Creates the activity party records for each contact.
3. Creates letter activity and sets **From** and **To** columns to the respective activity party entities.

### Clean up

The console displays an option to delete the records created during setup. The deletion is optional. You might want to examine the entities and data created by the sample instead. You can manually delete the records to achieve the same result.
