# Sample: Azure aware custom workflow activity

This sample obtains the data context from the current operation and posts it to the Azure Service Bus.

## How to run samples

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/cds/README.md) for information about how to run this sample.

## What this sample does

This sample shows how to write a custom workflow activity that can post the data context from the current CDS for Apps operation to the Azure Service Bus. The posting of the data context is done through the [Execute(EntityReference,IExecutionContext )](https://docs.microsoft.com/en-us/dotnet/api/microsoft.xrm.sdk.iserviceendpointnotificationservice.execute?view=dynamics-general-ce-9#Microsoft_Xrm_Sdk_IServiceEndpointNotificationService_Execute_Microsoft_Xrm_Sdk_EntityReference_Microsoft_Xrm_Sdk_IExecutionContext_)
