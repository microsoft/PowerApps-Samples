---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to use the CreateMultiple, UpdateMultiple, UpsertMultiple, and DeleteMultiple messages for standard and elastic tables using the Dataverse SDK for .NET."
---

# Bulk Operations Sample

This sample shows how to perform bulk create and update operations using several different approaches including the use of these classes:

- [CreateMultipleRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.createmultiplerequest)
- [UpdateMultipleRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.updatemultiplerequest)
- [UpsertMultipleRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.upsertmultiplerequest)

Examples using elastic tables use the `DeleteMultiple` message without an SDK class. These messages are optimized in performance to create, update, or delete records with Dataverse.

You can use the *standard* or *elastic* tables to compare the different performance characteristics.

This sample is a prerequisite for the [CreateMultiple and UpdateMultiple plug-ins Sample](https://github.com/microsoft/PowerApps-Samples/tree/master/dataverse/orgsvc/CSharp/xMultiplePluginSamples)

This sample is a Visual Studio .NET 6.0 solution that contains five different projects that perform the same operations in different ways so that you can compare the performance of each method.

> **NOTE**:
> The **UpsertMultiple** project is a little different. See the [UpsertMultiple/README.md](UpsertMultiple/README.md) for details.

You can access the full sample in the [BulkOperations](https://github.com/microsoft/PowerApps-Samples/tree/master/dataverse/orgsvc/CSharp-NETCore/BulkOperations) folder.

## Prerequisites

- Microsoft Visual Studio 2022
- Access to Dataverse with system administrator or system customizer privileges.

## How to run this sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Open the `PowerApps-Samples/dataverse/orgsvc/CSharp-NETCore/CreateUpdateMultiple/CreateUpdateMultiple.sln` file using Visual Studio 2022.
1. Edit the `appsettings.json` file. Set the connection string `Url` and `Username` parameters for your test environment.
1. Your environment URL can be found in the Power Platform admin center and has the form `https://<environment-name>.crm.dynamics.com`.
1. Build the solution, select the desired project as the startup project and press F5 to run the console application in debug mode.

When the sample runs, you're prompted in a default browser to select an environment user account and enter a password. To avoid doing this every time you run a sample, insert a `Password` parameter into the connection string in the `appsettings.json` file. For example:

```json
{
"ConnectionStrings": {
    "default": "AuthType=OAuth;Url=https://myorg.crm.dynamics.com;Username=someone@myorg.onmicrosoft.com;Password=mypassword;RedirectUri=http://localhost;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;LoginPrompt=Auto"
  }
}
```

> **TIP**
> You can set a user environment variable named `DATAVERSE_APPSETTINGS` to the file path of the `appsettings.json` file stored anywhere on your computer. The samples use that appsettings file if the environment variable exists and isn't null. Log out and back in again after you define the variable for it to take affect. You can manually set an environment variable by typing `env` in your system search bar, then choose **Environment variables** in **System properties** window.

## What this sample does

This sample is a .NET 6.0 Visual Studio 2022 solution that contains five projects that perform the same operations:

1. Create a new custom table named `sample_example` if it doesn't already exist.

   The table is created as a standard table by default. To create an elastic table, change the `UseElastic` property in `Settings.cs` to `true`.

1. Prepare a configurable number of entity instances for the custom table representing records to create.
1. Create the records. Each project uses a different method.
1. Update the set of entity instances that were created by appending text to the `sample_name` attribute.
1. Update the records using the same method they were created.
1. Delete the records.

   - If `Settings.UseElastic` is `false`, use the the `BulkDeleteRequest` to delete the records created and report on the success of this request.
   - Otherwise use the `DeleteMultiple` message when the table is an elastic table.

1. Delete the custom `sample_example` table created in the first step.

Each project uses a shared set of settings in the `Settings.cs` file that allow you to control:

- `UseElastic`: Whether to create and use an elastic table instead of a standard table.
- `NumberOfRecords`: The number of records to create. The default value is `100` but you can raise it to `1000` or higher.
- `StandardBatchSize`: The maximum number of records operations to send with `ExecuteMultiple`, `CreateMultiple`, and `UpdateMultiple`. `ExecuteMultiple` cannot exceed `1000`.
- `ElasticBatchSize`: The recommended number of records operations to send with `CreateMultiple`, `UpdateMultiple` and `DeleteMultiple` for Elastic tables is `100`. You can use a higher or lower number, but a higher batch size isn't necessarily going to provide higher throughput because there's no transaction with elastic tables.
- `BypassCustomPluginExecution`: Whether custom plug-in logic should be bypassed. This is useful to observe the performance impact of plug-ins registered on events for the table.
- `DeleteTable`: Whether to delete the custom `sample_example` table at the end of the sample. If you want to test plug-ins that use events on this table, you can preserve the table so you can run the samples multiple times while testing plug-ins.
- `CreateAlternateKey`: Used by the **UpsertMultiple** project only. This setting specifies whether an alternate key should be created with the table.

The `Settings.cs` file is included in each project. If you change one project, those settings apply to all samples.

### Supporting examples

The shared `Utility.cs` class contains static methods to perform operations that are common in each sample. These methods aren't the primary focus of the sample, but might be of interest:

| Method | Description |
|--------|-------------|
| `TableExists` | Detects whether a table with the specified schema name exists. |
| `BulkDeleteRecordsByIds` | Asynchronously deletes a group of records for a specified table by ID. |
| `IsMessageAvailable` | Detect whether a specified message is supported for the specified table. |
| `CreateExampleTable` | Creates the table used by projects in this solution, if it doesn't already exist. Currently uses Web API to create an elastic table. |
| `DeleteExampleTable` | Deletes the table used by projects in this solution, unless the `DeleteTable` setting is false. |

## How this sample works

By default the **CreateUpdateMultiple** project should be set as the startup project for the solution. To try any of the other samples, right-click the project in Solution Explorer and choose **Set as startup project**.

### Demonstrate

How each project in this solution sends requests:

- Includes the optional `tag` parameter, so the name of the project is available to plug-ins as a shared variable.
- Depending on the settings, might include the optional `BypassCustomPluginExecution` parameter to disable custom plug-in execution, so the impact of any plug-ins registered for the operation can be compared.
- Has a [Stopwatch](https://learn.microsoft.com/dotnet/api/system.diagnostics.stopwatch?view=net-6.0) configured to capture the total duration of seconds for the request to complete.

Details about each project and the default output are described in their respective README files:

- [CreateUpdateMultiple/README.md](CreateUpdateMultiple/README.md)
- [ExecuteMultiple/README.md](ExecuteMultiple/README.md)
- [ParallelCreateUpdate/README.md](ParallelCreateUpdate/README.md)
- [ParallelCreateUpdateMultiple.md](ParallelCreateUpdateMultiple/README.md)
- [SimpleLoop/README.md](SimpleLoop/README.md)
- [UpsertMultiple/README.md](UpsertMultiple/README.md)
