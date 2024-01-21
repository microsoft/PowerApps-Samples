# Work with choice columns

This sample shows how to work with choice. 

Typically, you use choice column to set columns so that different columns can share the same set of options, which are maintained in one location. Unlike local choice columns which are defined only for a specific column, you can reuse choice. You will also see them used in request parameters in a manner similar to an enumeration.

When you define a choice column by using [CreateOptionSetRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.createoptionsetrequest), we recommend that you let the system assign a value. You do this by passing a null value when you create the new OptionMetadata instance. When you define an option, it will contain an option value prefix specific to the context of the publisher set for the solution that the option set is created in. This prefix helps reduce the chance of creating duplicate option sets for a managed solution, and in any option sets that are defined in organizations where your managed solution is installed. For more information, see [Merge choice values](https://learn.microsoft.com/powerapps/developer/common-data-service/understand-managed-solutions-merged#merge-option-set-options).

This sample uses the following message request classes to work with choice columns:

- [CreateOptionSetRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.createoptionsetrequest)
- [UpdateOptionSetRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.updateoptionsetrequest)
- [RetrieveOptionSetRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.retrieveoptionsetrequest)
- [DeleteOptionSetRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.deleteoptionsetrequest)

## What this sample does

The `CreateOptionSetRequest`, `UpdateOptionSetRequest`, `RetrieveOptionSetRequest`, and `DeleteOptionSetRequest` messages are intended to be used in a scenario where it contains the data that is needed to work with choice columns.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

1. The `CreateOptionSetRequest` message creates a choice column  required for the sample.
2. The `CreateAttributeRequest` message creates values linked to the choice column.
3. The `OptionSetMetadata` message is required to specify the `IsGlobal=true`, in order to relate the values to the choice column.
4. The `UpdateOptionSetRequest` message updates the choice values.
5. The `PublishXmlRequest` messages publishes the choice column with the latest updates. 
6. The `RetrieveOptionSetRequest` message retrieves the choice column by it's name.

### Clean up

Displays an option to delete all the data created in the sample. The deletion is optional in case you want to examine the data created by the sample. You can manually delete the data to achieve same results.
