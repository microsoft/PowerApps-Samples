---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to use the CreateMultiple, UpdateMultiple and UpsertMultiple actions for standard and elastic tables using the Dataverse Web API."
---

# Create, Update, and Upsert Multiple Web API Sample

This sample shows how to perform bulk create, update and upsert operations using the
[CreateMultiple](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/createmultiple),
[UpdateMultiple](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/updatemultiple), and [UpsertMultiple](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/upsertmultiple)
actions. These messages are optimized to provide the most performant way to create or update records with Dataverse.

This sample provides the option to use *standard* or *elastic* tables so you can compare the different performance characteristics.

This sample or the [SDK version](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/orgsvc/CSharp-NETCore/BulkOperations/README.md) are prerequisites for the
[CreateMultiple and UpdateMultiple plug-ins Sample](https://github.com/microsoft/PowerApps-Samples/tree/master/dataverse/orgsvc/CSharp/xMultiplePluginSamples).

This sample is a Visual Studio .NET 6.0 solution that contains two projects (**CreateUpdateMultiple** and **ParallelCreateUpdateMultiple**) that perform the same operations in different ways so that you can compare the performance of each method.

The third **UpsertMultiple** project demonstrates bulk upsert of records and can use either standard or elastic tables.

You can find the sample [here](https://github.com/microsoft/PowerApps-Samples/tree/master/dataverse/webapi/CSharp-NETx/BulkOperations).

## Prerequisites

- Microsoft Visual Studio 2022
- Access to Dataverse with system administrator or system customizer privileges

## How to run this sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Open the `PowerApps-Samples\dataverse\webapi\CSharp-NETx\xMultipleSamples.sln` file using Visual Studio 2022.

   This solution contains two projects that include these samples:

   - **CreateUpdateMultiple**: Creates and updates a configurable number of records by sending two requests.
   - **ParallelCreateUpdateMultiple**: Creates and updates a configurable number of records by sending requests in parallel.
   - **UpsertMultiple**: Demonstrates *upserting* a configurable number of records.

   > [!NOTE]
   > The **WebAPIService** project is included so that each of the other projects can depend on the common helper code provided by the service. The samples use several classes in the `WebAPIService/Messages` folder.

   In **Solution Explorer**, right-click the project you want to run and choose **Set as Startup Project**.

1. In any project, edit the `appsettings.json` file to set the following property values.

   | Property | Instructions |
   |----------|--------------|
   | `Url` | The URL for your environment. Replace the placeholder `https://yourorg.api.crm.dynamics.com` value with the value for your environment. Learn how to find your URL in [View developer resources](https://learn.microsoft.com/power-apps/developer/data-platform/view-download-developer-resources). |
   | `UserPrincipalName` | Replace the placeholder `you@yourorg.onmicrosoft.com` value with the UPN value you use to access the environment. |
   | `Password` | Replace the placeholder `yourPassword` value with the password you use. |

1. Save the `appsettings.json` file.

   > [!NOTE]
   > Both projects refer to the same `appsettings.json` file, so you only need to replace with your values in one file to run either project.

1. Press **F5** to run the sample.

## What this sample does

This sample is a .NET 6.0 Visual Studio 2022 solution that contains 2 projects (**CreateUpdateMultiple** and **ParallelCreateUpdateMultiple**) that perform the same operations.

1. Creates a new custom table named `sample_example` if it doesn't already exist.

   The table is created as a standard table by default. To create an elastic table, change the `UseElastic` property in `Settings.cs` to true.

1. Prepares a configurable number of entity instances for the custom table representing records to create.
1. Create the records. Each project uses a different method.
1. Updates the set of entity instances created by appending text to the `sample_name` attribute.
1. Updates the records using the same method that created them.
1. Deletes the records using the [BulkDelete action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/bulkdelete).

1. Deletes the custom `sample_example` table created in the first step. You can optionally remove the deletion.

Each project uses a shared set of settings in the `Settings.cs` file that allow you to control:

- `UseElastic`: Whether to create and use an elastic table instead of a standard table.
- `NumberOfRecords`: The number of records to create. The default value is 100 but you can raise it to 1000 or higher.
- `StandardBatchSize`: The maximum number of records operations to send with `CreateMultiple` and `UpdateMultiple`.
- `ElasticBatchSize`: The recommended number of records operations to send with `CreateMultiple` and `UpdateMultiple` for elastic tables is 100. You can use a higher or lower number, but a higher batch size isn't necessarily going to provide higher throughput because there is no transaction with elastic tables.
- `BypassCustomPluginExecution`: Whether custom plugin logic should be bypassed. Bypassing is useful to observe the performance impact of plugins registered on events for the table.
- `DeleteTable`: Whether to delete the custom `sample_example` table at the end of the sample. If you want to test plug-ins that use events on this table, you can instead preserve the table to run the samples multiple times while testing plugins.

The `Settings.cs` file is included in both projects. Apply the change in one project and they're set for all projects.

### Supporting examples

The shared `Utility.cs` class contains static methods to perform operations that are common in each sample. These methods aren't the primary focus of the sample, but might be of interest:

| Method | Description |
|--------|-------------|
| `TableExists` | Detects whether a table with the specified schema name exists. |
| `BulkDeleteRecordsByIds` | Asynchronously deletes a group of records for a specified table by ID. |
| `IsMessageAvailable` | Detects whether a specified message is supported for the specified table. |
| `CreateExampleTable` | Creates the table used by projects in this solution if it doesn't already exist. Currently uses Web API to create an elastic table. |
| `DeleteExampleTable` | Deletes the table used by projects in this solution, unless the `DeleteTable` setting is false. |

## How this sample works

By default the **CreateUpdateMultiple** project is set as the startup project for the solution. To try the **ParallelCreateUpdateMultiple** or **UpsertMultiple** sample, right-click the project in Solution Explorer and choose **Set as startup project**.

### Demonstrate

This sample demonstrates how to create and update records in bulk for a custom table created by the sample. The sample creates a custom table and cleans up when you finish running the sample.

Both projects in this solution send requests that includes:

- The optional `tag` parameter so that the name of the project is available to plugins as a shared variable.
- The optional `BypassCustomPluginExecution` parameter to disable custom plugin execution so that the impact of any plugins registered for the operation can be compared.
- A [Stopwatch](https://learn.microsoft.com/dotnet/api/system.diagnostics.stopwatch?view=net-6.0) configured to capture the total duration of seconds for the request to complete.

Details about each project and the default output are described in their respective README files:

- [CreateUpdateMultiple/README.md](CreateUpdateMultiple/README.md)
- [ParallelCreateUpdateMultiple/README.md](ParallelCreateUpdateMultiple/README.md)
- [UpsertMultiple/README.md](UpsertMultiple/README.md)
