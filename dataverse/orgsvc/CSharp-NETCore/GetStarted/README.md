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

To get started writing code that accesses Dataverse business data you must first authenticate with the web service, send a request to perform operations, and retrieve data from the response. This process can be repeated for the many web service requests that Dataverse supports.

## About the sample code

The samples demonstrate how to:

1. Connect to Dataverse using a connection string required for authentication.
2. Send a web service request, retrieving information about the signed-in user.
3. Access data, in this case the user's ID, in the web service response.

| Sample | Description | Build target |
|--------|-------------|--------------|
| ConsoleApp (public) | Connects to Dataverse and sends a simple message [WhoAmI](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.whoamirequest). A *public* client is used for the connection. | .NET 8 |
| ConsoleApp (confidential) |Connects to Dataverse and sends a simple message [WhoAmI](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.whoamirequest). A *confidential* client is used for the connection. | .NET 8 |

The public or confidential client for the web service connection is handled internally in the [ServiceClient](https://learn.microsoft.com/dotnet/api/microsoft.powerplatform.dataverse.client.serviceclient) class based on the connection string parameters that are passed. Learn more in [PublicClientApplicationBuilder](https://learn.microsoft.com/dotnet/api/microsoft.identity.client.publicclientapplicationbuilder) and [ConfidentialClientApplicationBuilder](https://learn.microsoft.com/dotnet/api/microsoft.identity.client.confidentialclientapplicationbuilder).

These examples provide the minimum amount of code necessary to demonstrate connecting to Dataverse and performing a data operation.

- The code should also work with a .NET Framework build target.
- Storing connection information in code isn't recommended. Typically you would store the password in an `App.config` or an `appsettings.json` file, or remove the password and let [Entra ID](https://learn.microsoft.com/en-us/entra/fundamentals/whatis) prompt you for your credentials.
- These examples don't provide any exception (error) handling.

## How to build and run the code samples

### ConsoleApp (public)

1. Clone the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Locate the `GetStarted` sample folder.
1. Open the `ConsoleApp.sln` solution file in Visual Studio 2022.
1. In `Program.cs` of the `ConsoleApp (public)` project, set the `url`, `userName`, and `password` values for your environment.
1. Build and run the `ConsoleApp (public)` project.

### ConsoleApp (confidential)

Sending requests confidentially requires you to have an app registration and client secret in Microsoft Entra ID, then bind your app user to your app registration.

You can create a new app registration, secret, and app user if you don't have them already:

1. [Create an app registration](https://learn.microsoft.com/en-us/power-apps/developer/data-platform/walkthrough-register-app-azure-active-directory#create-the-app-registration) and a client secret in [Microsoft Entra ID](https://entra.microsoft.com/).

   > [!IMPORTANT]
   > Once you create a new secret, copy the secret *value* (not the ID) for use in this sample. You can only copy it once, so save it somewhere on your computer.
1. [Create a new app user](https://learn.microsoft.com/en-us/power-apps/developer/data-platform/walkthrough-register-app-azure-active-directory#create-a-new-app-user) in Power Platform admin center. The target **Business unit** shown when you create a new app user is your environment organization name. This new app user is bound to your app registation when you add an app in the new app user steps.
1. Update the `ConsoleApp (confidential)` project's `Program.cs file` with your `ClientID` and `Secret` (value, not ID) as seen on your app registration page in the Entra ID admin center.
1. Set the `ConsoleApp (confidential)` project as your startup project and run it.

## Expected program output

You can expect a similar output for the project:

```
User ID is 8061643d-ebf7-e811-a974-000d3a1e1c9a.
Press the <Enter> key to exit.
```

## Related information

[Quickstart: Execute an SDK for .NET request (C#)](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/quick-start-org-service-console-app)
