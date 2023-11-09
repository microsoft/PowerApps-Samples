---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to get started using the Dataverse SDK for .NET."
---
# Get started with the Dataverse SDK for .NET

This console app demonstrates how to:

1. Connect to Dataverse using the SDK for .NET with a connection string
2. Execute a message request
3. Access some data in the response

Related article: [Quickstart: Execute an Organization service request](https://docs.microsoft.com/power-apps/developer/data-platform/org-service/quick-start-org-service-console-app)

The included code sample is listed below.

|Sample|Description|Build target|
|---|---|---|
|ConsoleApp|Demonstrates connecting to Dataverse and executing a simple message (`WhoAmI`).|.NET 6|

> **Note**: This example is the minimum amount of code necessary to demonstrate connecting to Dataverse and 
performing a data operation.
> 
> - Storing connection information in code isn't recommended
> - This example doesn't provide any error handling


## Instructions

1. Clone the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Locate the sample folder.
1. Open the solution file (*.sln) in Visual Studio 2022.
1. Edit the project's Program.cs file. Set the `url`, `username`, and `password` variable values as indicated by the `// TODO code` comment as appropriate for your test environment and user account.
1. Build and run the project.
