---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample code shows how to use the discovery service using the OData service and HttpClient."
---
# Work with Discovery Service

This sample code shows how to use the discovery service using the OData service and HttpClient.

## How to run this sample

This sample opens a form in your default browser to prompt you for connection information based on your operating system credentials.

To use alternative credentials (not your operating system credentials), you can manually update the `username` and `password` fields of Program.cs in the `Program.Main` method before running this sample.

## What this sample does

This sample uses the OData [global discovery service](https://learn.microsoft.com/power-apps/developer/data-platform/discovery-service#global-discovery-service) with a user's credentials to determine your environment. This sample uses `HttpClient` class and the `Microsoft.Identity.Client` library to authenticate.

If one or more environments are returned, the sample prompts the user to choose one, and then uses a `WhoAmIRequest` to return the `SystemUser.UserId` for that environment.

## How this sample works

### Setup

- This sample requires no special setup except for the optional custom user credentials.
- You may need to update your project to the current .NET version with the [.NET Upgrade Assistant](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.upgradeassistant) extension. Once the extension is installed, right-click your project and select **Upgrade**.

In Cloud.cs there's a `Cloud` enumeration for each of the known discovery regions. Each enumeration member is decorated with a `Description` notation. All members have the URL for the global discovery service for that cloud set as the description.

Instance.cs contains an `Instance` class that describes the data returned by the discovery service.

### Demonstrate

This sample performs the following steps.

1. Using the user credentials and the `cloud` value, the program uses the `GetInstances` static method to retrieve all known environments for the user.
1. The `GetInstances` method extracts the data center global discovery service URL from the member `Description` decoration and uses it together with the user credentials to instantiate an HttpClient. 
1. The `GetToken` static method uses Microsoft.Identity.Client methods to return an access token with the user's credentials. If MFA is required for the tenant, a dialog opens for the user to add their username and password.
1. The data is deserialized into a `List<Instance>` and returned.
1. If any environments are returned by the `GetInstances` method, they're listed in the console and you're prompted to choose one by typing a number. If your choice is valid, the static `ShowUserId` method uses the instance data and the user's credentials to execute a `WhoAmIRequest` and return the `SystemUser.UserId` for the user in that environment.

### Clean up

This sample creates no records. No cleanup is required.
