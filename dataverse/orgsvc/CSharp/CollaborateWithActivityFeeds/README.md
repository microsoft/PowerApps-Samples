# Collaborate with Activity feeds

This sample shows how to create posts with comments and mentions and how to follow records. It also demonstrates how to retrieve information for the personal and record walls. 

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## How this sample works

In order to simulate the scenario described in above, the sample will do the following:

### Setup

1. Checks for the current version of the org.
1. Creates leads for relating activity feed records. Since there is an active post rule configuration of a new lead, creating the leads should add an auto post to the record wall for each of the leads.

### Demonstrate

1. The `ConfigureActivityFeeds` method gets the original system user configuration in order to keep a copy for reverting after the sample has completed. This method also creates or retrieves an instance of `msdyn_PostConfig` to enable activity feeds for leads. If a new `msdyn_PostConfig` record gets created, activity feeds for system users will be enabled automatically.
2. The `PostToRecordWalls` method posts records to the wall.
3. The `PostToPersonalWalls` method posts lead records on personal walls.
4. The `ShowRecordWalls` methods shows leads records on the wall.

### Clean up

Display an option to delete the sample data created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the sample data to achieve the same result.