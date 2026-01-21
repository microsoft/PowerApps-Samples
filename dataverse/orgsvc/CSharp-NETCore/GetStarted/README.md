---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to get started using the Dataverse SDK for .NET."
---
# Get started using the Dataverse SDK for .NET

To get started writing code that accesses Dataverse business data you must first be able to authenticate with the web service, execute (send) a request to perform some operation, and then retrieve data from the response. This process can be repeated for the many web service requests that Dataverse supports.

Related article: [Quickstart: Execute an SDK for .NET request (C#)](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/quick-start-org-service-console-app)

## About the sample code

The code samples demonstrate how to get started using the Dataverse SDK for .NET. Specifically, the samples demonstrate how to:

1. Connect to Dataverse using a connection string that defines required authentication and connection information
2. Execute a simple web service request that retrieves information about the logged on user
3. Access data, in this case the user's ID, in the web service response

|Sample|Description|Build target|
|---|---|---|
|ConsoleApp (public)|Demonstrates connecting to Dataverse and executing a simple message [WhoAmI](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.whoamirequest). A public client is used for the connection.|.NET 8|
|ConsoleApp (confidential)|Demonstrates connecting to Dataverse and executing a simple message [WhoAmI](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.whoamirequest). A confidential client is used for the connection.|.NET 8|

Use of a public or confidential client for the web service connection is handled internally in the [ServiceClient](https://learn.microsoft.com/dotnet/api/microsoft.powerplatform.dataverse.client.serviceclient) class based on the connection string parameters that are passed. 
More information: [PublicClientApplicationBuilder](https://learn.microsoft.com/dotnet/api/microsoft.identity.client.publicclientapplicationbuilder), [ConfidentialClientApplicationBuilder](https://learn.microsoft.com/dotnet/api/microsoft.identity.client.confidentialclientapplicationbuilder)

These examples provide the minimum amount of code necessary to demonstrate connecting to Dataverse and performing a data operation.
- The code should also work with a .NET Framework build target.
- Storing connection information in code isn't recommended. Typically you would store the password in an App.config or app settings file, or remove the password and let Entra ID prompt you for logon credentials. 
- These examples don't provide any exception (error) handling.

## How to build and run the code sample(s)

1. Clone the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Locate the sample folder.
1. Open the solution file (*.sln) in Visual Studio 2022.
1. Edit the project's Program.cs file. Set the `url`, `username`, and `password` variable values as indicated by the `// TODO` code comment as appropriate for your test environment and user account.
1. Build and run the ConsoleApp (public) project.

To run the ConsoleApp (confidential) project, you must first create an app registration for your tenant in Microsoft Entra ID and then update the project's Program.cs file with the client ID and client secret values defined in the app registration. 

More information: [Tutorial: Register an app with Microsoft Entra ID](https://learn.microsoft.com/power-apps/developer/data-platform/walkthrough-register-app-azure-active-directory).

## Expected program output

The program's console output should look similar to the following example.

```console
User ID is 8061643d-ebf7-e811-a974-000d3a1e1c9a.
Press the <Enter> key to exit.
```
