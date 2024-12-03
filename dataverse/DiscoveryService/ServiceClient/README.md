---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample code shows how to use the Discovery service with Dataverse ServiceClient."
---
# Work with Discovery Service

This sample code shows how to use the discovery service with Dataverse ServiceClient.

## How to run this sample

You must first set the `username` and `password` variables in the `Program.Main` method of `Program.cs` before running this sample, because the sample doesn't open dialog to prompt you for connection information.

## What this sample does

This sample uses the [Microsoft.PowerPlatform.Dataverse.Client](https://www.nuget.org/packages/Microsoft.PowerPlatform.Dataverse.Client/) [ServiceClient Class](https://learn.microsoft.com/dotnet/api/microsoft.powerplatform.dataverse.client.serviceclient) to query the [global discovery service](https://learn.microsoft.com/en-us/power-apps/developer/data-platform/discovery-service#global-discovery-service) with user credentials to determine which environments are their connected environments.

If one or more environments are returned, the sample prompts the user to choose one, and then uses a `WhoAmIRequest` to return the `SystemUser.UserId` for that environment.

## How this sample works

### Setup

- This sample requires no special setup, other than the valid user credentials `username` and `password` you configure.
- You may need to update your project to the current .NET version with the [.NET Upgrade Assistant](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.upgradeassistant) extension. Once the extension is installed, right-click your project and select **Upgrade**.

In `Cloud.cs`, there's a `Cloud` enumeration for each of the known discovery regions. Each enumeration member is decorated with a `Description` notation. All members have the URL for the global discovery service for that cloud set as the description.

### Demonstrate

1. Using the user credentials and the `cloud` value, the program uses the `GetAllOrganizations` static method to retrieve all known environments for the user.
1. The `GetAllOrganizations` method extracts the data center global discovery service URL from the member `Description` decoration and uses it together with user credentials to execute the [ServiceClient.DiscoverOnlineOrganizationsAsync Method](https://learn.microsoft.com/dotnet/api/microsoft.powerplatform.dataverse.client.serviceclient.discoveronlineorganizationsasync) to retrieve the user's accessible environments.
1. If any environments are returned by the `GetAllOrganizations` method, they are listed in the console and you're prompted to choose one by typing a number. If your choice is valid, the selected [OrganizationDetail](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.discovery.organizationdetail) data is passed to the `ShowUserId` method that executes a `WhoAmIRequest` and displays the `SystemUser.UserId` for the user in that environment.

### Clean up

This sample creates no records. No cleanup is required.