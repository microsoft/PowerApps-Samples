# Enable field security for a table

This sample shows how to enable field security for a table.

This sample requires additional users that are not in your system. Create the required users manually in **Office 365** in order to run the sample without any errors. For this sample create a user profile **as is** shown below. 

**First Name**: Samantha <br/>
**Last Name**: Smith<br/>
**Security Role**: Marketing Manager<br/>
**UserName**: ssmith@yourorg.onmicrosoft.com<br/>

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Get the user that you have created manually in **Office 365**.
2. Retrieve the security role needed to assign to the user.
3. Retrieve the default business unit needed to create the team.
4. Instantiate a team table record and set its property values.

### Demonstrate

1. Creates field security profile and create the request object and set the monikers with the teamprofiles_assocation relationship.
2. Creates custom activity table and columns using the `CreateEntityRequest` and `CreateAttributeRequest` message.
3. Create the field permission for the identity column.

### Clean up

Display an option to delete the records in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
