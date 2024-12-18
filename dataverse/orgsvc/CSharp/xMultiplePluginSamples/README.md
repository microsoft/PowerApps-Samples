# CreateMultiple and UpdateMultiple plug-ins Sample

This sample provides several Plug-in types that demonstrate how to write plug-ins for the `CreateMultiple` and `UpdateMultiple` messages.

## Prerequisites

- Access to Dataverse with system administrator or system customizer privileges.
- Understanding of how to write and register plug-ins.

## How to run this sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Run the [Create and Update Multiple Sample](../../CSharp-NETCore/BulkOperations/CreateUpdateMultiple/README.md) with the setting `DeleteTable` set to `false`.

   **Note**: This will create the tables needed for the plug-ins in this sample.

1. Open the [PowerApps-Samples/dataverse/orgsvc/xMultiplePluginSamples/xMultiplePluginSamples.sln](https://github.com/microsoft/PowerApps-Samples/tree/master/dataverse/orgsvc/xMultiplePluginSamples/xMultiplePluginSamples.sln)
1. Build the solution
1. Use the Plug-in Registration tool (PRT) to upload the `\xMultiplePluginSamples\bin\Debug\net462\xMultiplePluginSamples.dll` to the environment you ran the sample in step 2.
1. Use the PRT to register the plug-ins listed in the [Plug-in Type list](#plug-in-type-list) below.

   **Important**: Make sure that the following two plug-ins are **not** enabled at the same time:
    - `FollowupPluginSingle` and `FollowupPluginMultiple`.
    - `UpdateSingle` and `UpdateMultiple`.

1. Run any of the projects in the [Create and Update Multiple Sample](../../CSharp-NETCore/BulkOperations/CreateUpdateMultiple/README.md), but set a break point before the records are deleted.
1. In [Power Apps](https://make.powerapps.com/), navigate to **Dataverse** > **Tables** and locate the **Example** table.
1. Click the **Edit** button to view the records created.
1. Add the **Description** column to the view.
1. Repeat and observe the differences when different projects in the [Create and Update Multiple Sample](../../CSharp-NETCore/BulkOperations/CreateUpdateMultiple/README.md) are run. The `Description` field should include the name of the project.
1. Turn on tracing and view the traces written to the `PluginTraceLog` Table. The [XrmToolBox Plugin Trace Viewer](https://jonasr.app/ptv/) is recommended for this. More information: [Tracing and logging](https://learn.microsoft.com/power-apps/developer/data-platform/logging-tracing).

## What this sample does

This sample provides an experience where you can observe and interact with plug-ins that are written for the `CreateMultiple` and `UpdateMultiple` messages.

This sample depends on the [Create and Update Multiple Sample](../../CSharp-NETCore/BulkOperations/CreateUpdateMultiple/README.md) so that the merged message processing pipeline behaviors can be verified.

The **Create and Update Multiple Sample** contains 4 separate projects that do the same thing in different ways.

1. Create a new custom table named `sample_example` if it doesn't already exist.
1. Prepare a configurable number of `sample_example` entity instances for the custom table representing records to create.
1. Create the `sample_example` records. Each project uses a different method.

   **Note**: Each project will pass a `tag` parameter with the name of the project so that it is available as a shared variable to the plug-in. This value will be used by the `CreateMultiplePreOp.cs` plug-in in this sample.
1. Update the set of entity instances that were created by appending text to the `sample_name` attribute.
1. Update the `sample_example` records using the same method they were created.
1. Use a [BulkDeleteRequest](xref:Microsoft.Crm.Sdk.Messages.BulkDeleteRequest) to delete the `sample_example` records created and report on the success of this request.
1. Delete the custom `sample_example` table created in the first step, unless configured not to.

> **Important**
> To run this sample you must first run the **Create and Update Multiple Sample** with the setting `DeleteTable` set to `false`. This will create the table this sample depends on.

### Plug-in Type list

This sample contains the following plug-in types designed to interact with the operations performed by the **Create and Update Multiple Sample**, or to help visualize the changes to the executioncontext with each message.

|Plug-in Type|Message|Stage|Description|
|---------|---------|---------|---------|
|`ContextWriter.cs`|Any|Any|Use this plug-in to write details of the `IPluginExecutionContext4` to the trace log so that you can see the values being passed. Add entity images in the step registration to view the content.|
|`CreateMultiplePreOp.cs`|CreateMultiple|PreOperation (20)|Sets the `sample_description` attribute value to `$"'tag' value for Create = '{tagValue}'."` where `tagValue` is the value set using the optional `tag` parameter. |
|`FollowupPluginMultiple.cs`|CreateMultiple|PostOperation (40)|This is the replacement for `FollowupPluginSingle.cs`.<br />Creates a `task` record associated with the `sample_example` record created.|
|`FollowupPluginSingle.cs`|Create|PostOperation (40)|Creates a `task` record associated with the `sample_example` record created.|
|`UpdateMultiple.cs`|UpdateMultiple|PreOperation (20)|This is the replacement for `UpdateSingle.cs`.<br />Uses an Entity image named `example_preimages` from the matching item in the `PreEntityImagesCollection` to compare the original `sample_name` value with the value in the update operation. When the values are different, append a message to the `sample_description` attribute value: `$"\\r\\n - 'sample_name' changed from '{oldName}' to '{newName}'."`, where `oldName` is the original value and `newName` is the new value.|
|`UpdateSingle.cs`|Update|PreOperation (20)|Uses a `PreEntityImage` named `example_preimage` to compare the original `sample_name` value with the value in the update operation. When the values are different, append a message to the `sample_description` attribute value: `$"\\r\\n - 'sample_name' changed from '{oldName}' to '{newName}'."`, where `oldName` is the original value and `newName` is the new value.|

## How this sample works

This sample includes plug-ins written for the `CreateMultiple` and `UpdateMultiple` messages using the guidance provided in [Write plug-ins for CreateMultiple and UpdateMultiple (Preview)](https://learn.microsoft.com/power-apps/developer/data-platform/write-plugin-multiple-operation).

Two of the plug-ins, `FollowupPluginSingle.cs` and `UpdateSingle.cs`, represent the 'before' plug-in written for the `Create` and `Update` messages. The `FollowupPluginMultiple.cs` and `UpdateMultiple.cs` plug-ins represent the 'after' plug-ins written for `CreateMultiple` and `UpdateMultiple` messages.

By contrasting these plug-ins, you can observe how logic applied for the single operation can be modified to work with the operation that includes multiple entities.

The `ContextWriter.cs` plug-in captures data from the [IPluginExecutionContext4 Interface](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.ipluginexecutioncontext4) and writes it to the [PluginTraceLog table](https://learn.microsoft.com/power-apps/developer/data-platform/reference/entities/plugintracelog) so that you can see the data passed to the plug-in.

## Demonstrates

- How to write plug-ins for the `CreateMultiple` and `UpdateMultiple` messages
- How to modify existing plug-ins using `Create` and `Update` to use `CreateMultiple` and `UpdateMultiple` messages.
- How to write information about the plug-in execution context to the plug-in trace log.
