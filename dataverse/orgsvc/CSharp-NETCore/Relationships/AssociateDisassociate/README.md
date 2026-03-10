---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to create and then remove a relationship between three accounts and a contact."
---

# Establish an entity relationship through association

Learn how to create a relationship between two or more entities through association.

Related article(s): [Associate and disassociate table rows using the SDK for .NET](https://learn.microsoft.com/en-us/power-apps/developer/data-platform/org-service/entity-operations-associate-disassociate)

## About the sample code

|Sample|Description|Build target|
|---|---|---|
|AssociateTableRows|Demonstrates creating and removing an association between entities.|.NET 9|

The code samples demonstrates how to work with a relationship between entities. Specifically, the samples demonstrates how to:

1. Connect to Dataverse using a [connection string](https://learn.microsoft.com/power-apps/developer/data-platform/xrm-tooling/use-connection-strings-xrm-tooling-connect) that defines required connection information
1. Create a relationship between a contact and several accounts.
1. Associate and then disassociate the entities.
1. Use [early-bound](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/early-bound-programming#early-bound) entity types

The code being demonstrated can be found in the `Program.AssociateDisassociate()` method, which is invoked by `Program.Run()`.

Additional general information can be found in [README-code-design](https://github.com/microsoft/PowerApps-Samples/tree/master/dataverse/orgsvc/CSharp-NETCore/README-code-design.md) file.

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
Created contact 'John Doe'
Created account 'Example Account 1'
Created account 'Example Account 2'
Created account 'Example Account 3'
AssociateDisassociate(): The entities have been associated.
AssociateDisassociate(): The entities have been disassociated.
Press any key to undo environment data changes.
```
