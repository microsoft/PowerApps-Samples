---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates the Azure ServiceBus interface to Dataverse."
---

# Azure ServiceBus hybrid relay listener app

This code sample demonstrates an Azure ServiceBus listener app using a hybrid relay connection to Microsoft Dataverse. The hybrid relay is the only connection type that Dataverse currently supports when using the newer Azure ServiceBus messaging APIs.

Our current Plug-in Registration Tool (PRT) and other listener app code samples support the deprecated [WindowsAzure.ServiceBus](https://www.nuget.org/packages/WindowsAzure.ServiceBus) package and Microsoft.ServiceBus namespace. This sample demonstrates how to write a listener app using the newer Azure messaging APIs [Microsoft.Azure.Relay](https://www.nuget.org/packages/Microsoft.Azure.Relay) while still being compatible with Dataverse and the PRT.

Related articles: [Azure integration](https://learn.microsoft.com/power-apps/developer/data-platform/azure-integration), [Use a hybrid relay connection](https://learn.microsoft.com/power-apps/developer/data-platform/azure-hybrid-relay-connection)

## About the sample code

The code samples demonstrate how to get started using the newer Azure messaging APIs when writing a ServiceBus listener app. Specifically, the samples demonstrate how to:

1. Connect to an Azure ServiceBus hybrid relay endpoint using a SAS key and ServiceBus Url.
1. Register a request handler that will read the Dataverse remote execution context posted to the Azure ServiceBus.
1. Serialize the received remote execution context data back into a [RemoteExecutionContext](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.remoteexecutioncontext) object and display that data on the console.
1. Send a response containing a string value from the listener app back to the Dataverse plug-in that posted the context on the ServiceBus.

|Sample|Description|Build target|
|---|---|---|
|azure-hybrid-receiver|Demonstrates using a hybrid relay connection to read and display the Dataverse remote execution context posted to the Azure ServiceBus.|.NET 8|

Some additional notes about the sample:

- Storing connection information in code isn't recommended. Typically you would store configuration data in an App.config or app settings JSON file.
- This sample provides minimal exception handling.
- The response string that is sent from the listener app back to the Dataverse service endpoint plug-in is ignored. You could register a custom [Azure-aware plug-in](https://learn.microsoft.com/power-apps/developer/data-platform/write-custom-azure-aware-plugin) if you want to make use of the listener response.

## Prerequisites

- Configured Azure ServiceBus hybrid relay connection. See [Configure an Azure namespace and connection](https://learn.microsoft.com/power-apps/developer/data-platform/azure-hybrid-relay-connections#configure-an-azure-namespace-and-connection)
- Dataverse service endpoint and step configured using the Plug-in Registration Tool. See [Register a Dataverse service endpoint](https://learn.microsoft.com/power-apps/developer/data-platform/azure-register-service-endpoint)

## How to build and run the code sample(s)

1. Clone the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Locate the sample folder.
1. Open the solution file (*.sln) in Visual Studio 2022.
1. Edit the project's Program.cs file. Below the // TODO comment, set the SAS key and Url values from the configured Azure ServiceBus hybrid relay connection.
1. Build and run the project.
1. Trigger the registered Dataverse service endpoint plug-in by performing whatever operation the step was registered for. The example shown in the documentation triggers on creation of an account. You can run a [program](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/orgsvc/C%23-NETCore/ServiceClient/CreateUpdateDelete/Program.cs) to create the account or use a PowerApp.

## Expected program output

The listener app's console output should look similar to the following example.

```console
Listener opened, press any key to exit...
Received request body
----------
UserId: xxxxxxxxxxx
OrganizationId: xxxxxxxxxxx
...
PostEntityImages: xxxxxxxxxxx
----------
```
