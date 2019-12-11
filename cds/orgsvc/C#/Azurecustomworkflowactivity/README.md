# Sample: Azure aware custom workflow activity

This sample obtains the data context from the current Common Data Service (CDS) operation and posts it to the Azure Service Bus.

## How to run this sample

1. Follow the [Setup](#Setup) instructions below.
2. Execute the workflow.
3. Using the Azure portal web interface, examine the Azure message queue for the posted CDS message.

## What this sample does

This sample shows how to write a custom workflow activity that can post the data context from the current CDS operation to the Azure Service Bus. The posting of the data context is accomplished through the [Execute(EntityReference,IExecutionContext )](https://docs.microsoft.com/en-us/dotnet/api/microsoft.xrm.sdk.iserviceendpointnotificationservice.execute?view=dynamics-general-ce-9#Microsoft_Xrm_Sdk_IServiceEndpointNotificationService_Execute_Microsoft_Xrm_Sdk_EntityReference_Microsoft_Xrm_Sdk_IExecutionContext_) call.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

1. Obtains the execution context from the Execute() method parameter.
2. Obtains a reference to the service endpoint notification service.
3. Posts the context to the Service Bus using the notification service.

### Setup

1. Download or clone the [Samples](https://github.com/Microsoft/PowerApps-Samples) repo so that you have a local copy of the sample.
2. Open the sample solution in Visual Studio and build the sample.
3. Register the compiled custom activity assembly in partial trust using the **Plug-in Registration** tool.
4. Using the Azure web portal, create an Azure Service Bus queue to receive the custom activity context from CDS.
5. Register an Azure service endpoint, specifying the queue from step #4, using the **Plug-in Registration** tool.
6. Create a workflow and add a step using the AzureAwareWorkflowActivity action. When adding the step, make sure to specify the GUID of the registered service endpoint as the Input Id parameter value.

### Demonstrate

A message containing the context of the current CDS operation will be available in the Azure message queue. 

### Clean up

1. Un-register the activity assembly, and then the service bus endpoint, using the **Plug-in Registration** tool.
2. Delete the Azure service bus queue.

### See Also

[Configure Azure integration with Common Data Service](https://docs.microsoft.com/en-us/powerapps/developer/common-data-service/configure-azure-integration)
[Workflow extensions](https://docs.microsoft.com/en-us/powerapps/developer/common-data-service/workflow/workflow-extensions)