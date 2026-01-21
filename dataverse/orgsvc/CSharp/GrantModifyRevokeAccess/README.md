# Share records using GrantAccess, ModifyAccess and RevokeAccess messages

This sample shows how to share a record using the following messages:

[GrantAccessRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.grantaccessrequest)

[ModifyAccessRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.modifyaccessrequest)

[RevokeAccessRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.revokeaccessrequest)

This sample requires additional users that are not in your system. Create the required users manually in **Office 365** in order to run the sample without any errors. For this sample create 2 user profiles **as is** shown below. 

**First Name**: Dan<br/>
**Last Name**: Wilson<br/>
**Security Role**: Delegate<br/>
**UserName**: dwilson@yourorg.onmicrosoft.com<br/>

**First Name**: Christen<br/>
**Last Name**: Anderson<br/>
**Security Role**: Delegate<br/>
**UserName**: canderson@yourorg.onmicrosoft.com<br/>

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `GrantAccessRequest`, `ModifyAccessRequest`, `RevokeAccessRequest` messages are intended to be used in a scenario where it contains data that is needed to grant, modigy and revoke access.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. Creates a unique identifier for preventing name conflicts.
3. Retrieves the user created manually in **Office 365** for this sample.
4. Retrieves the root business unit for creating the team for the sample.
5. The `WhoAMIRequest` gets the current user information.
6. Creates the team and add the users to the team. 
7. Creates an account record and also creates a task, letter to associate to the account.

### Demonstrate

1. Retrieves and displays the access that the calling user has to the created account.
2. Retrieves and displays the access that the first user has to the created account. 
3. The `GrantAccessRequest` method grants the first user `read` access to the created account.
4. The `ModifyAccessRequest` method grants the first user `delete` access to the created account.
5. The `RevokeAccessRequest` method grants the first user `revoke` access to the created account.

### Clean up

Display an option to delete the sample data that is created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
