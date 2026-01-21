---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to assign an entity record to a team."
---

# Assign an entity record to a team

Learn how to assign an entity record to a team by using the [IOrganizationService.Update](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.update) message.

Related article(s):

- [Sharing and assigning](https://learn.microsoft.com/power-apps/developer/data-platform/security-sharing-assigning&tabs=sdk#assigning-records)

## About the sample code

|Sample|Description|Build target|
|---|---|---|
|[AssignRecordToTeam](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/orgsvc/CSharp-NETCore/Ownership/AssignRecordToTeam)|Assign an entity record to a team.|.NET 9|

The code samples demonstrates how to change the ownership of an entity record. Specifically, the samples demonstrates how to:

1. Connect to Dataverse using a [connection string](https://learn.microsoft.com/power-apps/developer/data-platform/xrm-tooling/use-connection-strings-xrm-tooling-connect) that defines required connection information
1. Assign an account record to a new owner (a team) by changing the `OwnerId` attribute of the account record.

Additional information can be found in [README-code-design](https://github.com/microsoft/PowerApps-Samples/tree/master/dataverse/orgsvc/CSharp-NETCore/README-code-design.md) file.

## How to build and run the code sample(s)

1. Clone the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Locate the sample folder.
1. Open the solution file (*.sln) in Visual Studio.
1. Edit the project's appsettings.json file and set the `Url`value as appropriate for your Dataverse test environment.
1. Build and run the project [F5].
1. You will be prompted in a browser window for account logon credentials to the target environment.

## Expected program output

For a successful run, the program's console output should look similar to the following example.
Otherwise, any errors or exceptions will be displayed.

```console
Created account Example Account.
Created team Team First Coffee.
Created role Custom Role.
Retrieved prvReadAccount
Added privilege to role.
Assigned team to role.
The account is now owned by the team.
Press any key to undo environment data changes.
```
