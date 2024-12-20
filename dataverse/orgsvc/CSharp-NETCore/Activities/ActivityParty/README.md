---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to create a letter activity."
---

# Create a letter activity

Learn how to create a letter activity addressed to multiple contacts.

## About the sample code

| Sample | Description | Build target |
|--------|-------------|--------------|
| ActivityParty | Demonstrates creating a letter activity. | .NET 9 |

This code sample demonstrates how to create a letter activity:

1. Connect to Dataverse using a [connection string](https://learn.microsoft.com/power-apps/developer/data-platform/xrm-tooling/use-connection-strings-xrm-tooling-connect) that defines required connection information.
1. Create a [letter activity](https://learn.microsoft.com/power-apps/developer/data-platform/reference/entities/letter) to send to multiple [contacts](https://learn.microsoft.com/power-apps/developer/data-platform/reference/entities/contact).
1. Use the Dataverse [organization service context](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/organizationservicecontext) to process data changes.
1. Use [early-bound](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/early-bound-programming#early-bound) entity types.

The demo code is found in the `Program.CreateLetter()` method, which is invoked by `Program.Run()`.

The early-bound entity files in the **DataModel** project are generated using the following PAC CLI command:
`pac modelbuilder build`. Learn more in [pac modelbuilder](https://learn.microsoft.com/en-us/power-platform/developer/cli/reference/modelbuilder).

## How to build and run the code sample

1. Clone the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Locate the sample folder.
1. Open the solution file (*.sln) in Visual Studio.
1. Edit the project's `appsettings.json` file and set the `Url`value for your Dataverse test environment.
1. Build and run the project [F5].
1. You're prompted in a browser window for account login credentials to the target environment.

## Expected program output

In a successful run, the program's console output should look similar to the following example.

Otherwise, any errors or exceptions are displayed.

```
CreateLetter(): letter activity created with ID < >
Press any key to undo environment data changes.
```

## Related information

[Activity tables](https://learn.microsoft.com/power-apps/developer/data-platform/activity-entities)
[README-code-design](https://github.com/microsoft/PowerApps-Samples/tree/master/dataverse/orgsvc/CSharp-NETCore/README-code-design.md)
