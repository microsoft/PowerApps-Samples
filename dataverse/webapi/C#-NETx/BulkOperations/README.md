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

This sample or the [SDK version](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/orgsvc/C%23-NETCore/BulkOperations/README.md) are prerequisites for the 
[CreateMultiple and UpdateMultiple plug-ins Sample](https://github.com/microsoft/PowerApps-Samples/tree/master/dataverse/orgsvc/C%23/xMultiplePluginSamples)

This sample is a Visual Studio .NET 6.0 solution that contains 2 projects (**CreateUpdateMultiple** and **ParallelCreateUpdateMultiple**) that perform the same operations in different ways so that you can compare the performance of each method.

The third **UpsertMultiple** project demonstrates bulk upsert of records and can use either standard or elastic tables.

You can find the sample [here](https://github.com/microsoft/PowerApps-Samples/tree/master/dataverse/webapi/C%23-NETx/BulkOperations).

## Prerequisites

- Microsoft Visual Studio 2022
- Access to Dataverse with system administrator or system customizer privileges.

## How to run this sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Open the `PowerApps-Samples\dataverse\webapi\C#-NETx\xMultipleSamples.sln` file using Visual Studio 2022.

   This solution contains two projects that include samples:

   - **CreateUpdateMultiple**: Creates and updates a configurable number of records by sending two requests.
   - **ParallelCreateUpdateMultiple**: Creates and updates a configurable number of records by sending requests in parallel.
   - **UpsertMultiple**: Demonstrates *upserting* a configurable number of records.
   
   **Note**: The **WebAPIService** project is included so that each of the other projects can depend on the common helper code provided by the service. The samples use several classes in the `WebAPIService/Messages` folder.
   
   In **Solution Explorer**, right-click the project you want to run and choose **Set as Startup Project**.

1. In any project, edit the `appsettings.json` file to set the following property values:

   |Property|Instructions  |
   |---------|---------|
   |`Url`|The Url for your environment. Replace the placeholder `https://yourorg.api.crm.dynamics.com` value with the value for your environment. See [View developer resources](https://learn.microsoft.com/power-apps/developer/data-platform/view-download-developer-resources) to find this. |
   |`UserPrincipalName`|Replace the placeholder `you@yourorg.onmicrosoft.com` value with the UPN value you use to access the environment.|
   |`Password`|Replace the placeholder `yourPassword` value with the password you use.|

1. Save the `appsettings.json` file.

   **Note**: Both projects refer to the same `appsettings.json` file, so you only need to do this one time to run either project.

1. Press **F5** to run the sample.

## What this sample does

This sample is a .NET 6.0 Visual Studio 2022 solution that contains 2 projects (**CreateUpdateMultiple** and **ParallelCreateUpdateMultiple**) that perform the same operations:

1. Create a new custom table named `sample_example` if it doesn't already exist.
   
   The table will be created as a standard table by default. To create an elastic table, change the `UseElastic` property in `Settings.cs` to true.

1. Prepare a configurable number of entity instances for the custom table representing records to create.
1. Create the records. Each project uses a different method.
1. Update the set of entity instances that were created by appending text to the `sample_name` attribute.
1. Update the records using the same method they were created.
1. Delete the records using the [BulkDelete action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/bulkdelete).

1. Delete the custom `sample_example` table created in the first step, unless configured not to.

Each project uses a shared set of settings in the `Settings.cs` file that allow you to control:

- `UseElastic` : Whether to create and use an elastic table instead of a standard table.
- `NumberOfRecords` : The number of records to create. The default value is 100 but you can raise it to 1000 or higher.
- `StandardBatchSize` : The maximum number of records operations to send with `CreateMultiple` and `UpdateMultiple`.
- `ElasticBatchSize` : The recommended number of records operations to send with `CreateMultiple` and `UpdateMultiple` for Elastic tables is 100. You can use a higher or lower number, but a higher batch size isn't necessarily going to provide higher throughput because there is no transaction with elastic tables.
- `BypassCustomPluginExecution` : Whether custom plug-in logic should be bypassed. This is useful to observe the performance impact of plug-ins registered on events for the table.
- `DeleteTable` : Whether to delete the custom `sample_example` table at the end of the sample. If you want to test plug-ins that use events on this table, this will preserve the table so you can run the samples multiple times while testing plug-ins.

The `Settings.cs` file is included in both projects. Apply the change in one project and they will be set for all of them.

### Supporting examples

The shared `Utility.cs` class contains static methods to perform operations that are common in each sample. These are not the primary focus of the sample, but might be of interest:

|Method  |Description  |
|---------|---------|
|`TableExists`|Detects whether a table with the specified schema name exists.|
|`BulkDeleteRecordsByIds`|Asynchronously deletes a group of records for a specified table by id.|
|`IsMessageAvailable`|Detect whether a specified message is supported for the specified table.|
|`CreateExampleTable`|Creates the table used by projects in this solution if it doesn't already exist. Currently uses Web API to create elastic table.|
|`DeleteExampleTable`|Deletes the table used by projects in this solution, unless the `DeleteTable` setting is false.|

## How this sample works

By default the **CreateUpdateMultiple** project should be set as the startup project for the solution. To try the **ParallelCreateUpdateMultiple** or **UpsertMultiple** sample, select the project in Solution Explorer and choose **Set as startup project**.

### Demonstrate

As mentioned in [What this sample does](#what-this-sample-does) above, this sample demonstrates how to create and update records in bulk for a custom table created by the sample. To do this, it must create a custom table and clean up when you finish running the sample.

Both projects in this solution sends requests in the following way:

- Includes the optional `tag` parameter so that the name of the project is available to plug-ins as a shared variable.
- Depending on the settings, may include the optional `BypassCustomPluginExecution` parameter to disable custom plug-in execution so that the impact of any plug-ins registered for the operation can be compared.
- Has a [Stopwatch](https://learn.microsoft.com/dotnet/api/system.diagnostics.stopwatch?view=net-6.0) configured to capture the total duration of seconds for the request to complete.

Details about each project and the default output are described in their respective README files:

- [CreateUpdateMultiple/README.md](CreateUpdateMultiple/README.md)
- [ParallelCreateUpdateMultiple/README.md](ParallelCreateUpdateMultiple/README.md)
- [UpsertMultiple/README.md](UpsertMultiple/README.md)

