# Work with global option sets

This sample shows how to work with global option sets. 

Typically, you use global option sets to set fields so that different fields can share the same set of options, which are maintained in one location. Unlike local options sets which are defined only for a specific attribute, you can reuse global option sets. You will also see them used in request parameters in a manner similar to an enumeration.

When you define a global option set by using [CreateOptionSetRequest](https://docs.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.createoptionsetrequest?view=dynamics-general-ce-9), we recommend that you let the system assign a value. You do this by passing a null value when you create the new OptionMetadata instance. When you define an option, it will contain an option value prefix specific to the context of the publisher set for the solution that the option set is created in. This prefix helps reduce the chance of creating duplicate option sets for a managed solution, and in any option sets that are defined in organizations where your managed solution is installed. For more information, see [Merge option set options](https://docs.microsoft.com/powerapps/developer/common-data-service/understand-managed-solutions-merged#merge-option-set-options).

This sample uses the following message request classes to work with global option sets:

- [CreateOptionSetRequest](https://docs.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.createoptionsetrequest?view=dynamics-general-ce-9)
- [UpdateOptionSetRequest](https://docs.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.updateoptionsetrequest?view=dynamics-general-ce-9)
- [RetrieveOptionSetRequest](https://docs.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.retrieveoptionsetrequest?view=dynamics-general-ce-9)
- [DeleteOptionSetRequest](https://docs.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.deleteoptionsetrequest?view=dynamics-general-ce-9)

## What this sample does

The `CreateOptionSetRequest`, `UpdateOptionSetRequest`, `RetrieveOptionSetRequest`, and `DeleteOptionSetRequest` messages are intended to be used in a scenario where it contains the data that is needed to work with global option sets.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

1. The `CreateOptionSetRequest` message creates a global option set required for the sample.
2. The `CreateAttributeRequest` message creates a picklist linked to the global option set.
3. The `OptionSetMetadata` message is required to specify the `IsGlobal=true`, in order ti relate the picklist to the global option set.
4. The `UpdateOptionSetRequest` message updates the option set values.
5. The `PublishXmlRequest` messages publishes the global option set with the latest updates. 
6. The `RetrieveOptionSetRequest` message retrieves the global option set by it's name.

### Clean up

Displays an option to delete all the data created in the sample. The deletion is optional in case you want to examine the data created by the sample. You can manually delete the data to achieve same results.
