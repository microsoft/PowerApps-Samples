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

This sample will not open dialog to prompt you for connection information.

You must first set the `username` and `password` variables in the `Program.Main` method before running this sample.

## What this sample does

This sample uses the [Microsoft.PowerPlatform.Dataverse.Client](https://www.nuget.org/packages/Microsoft.PowerPlatform.Dataverse.Client/) [ServiceClient Class](https://learn.microsoft.com/dotnet/api/microsoft.powerplatform.dataverse.client.serviceclient) to query the global discovery service with a user's credentials to determine which environments they can connect with.

If one or more environments are returned, the sample will prompt the user to choose one, and then use a `WhoAmIRequest` to return the `SystemUser.UserId` for that environment.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

This sample requires no special setup except that there are valid user credential `username` and `password` to use.

In Cloud.cs there is a `Cloud` enumeration for each of the known discovery region. Each enumeration member is decorated with a `Description` notation. All of these members have the URL for the global discovery service for that cloud set as the description.

### Demonstrate

1. Using the user credentials and the `cloud` value, the program uses the `GetAllOrganizations` static method to retrieve all known environments for the user.
1. The `GetAllOrganizations` method extracts the data center global discovery service Url from the member `Description` decoration and uses it together with the user credentials to execute the [ServiceClient.DiscoverOnlineOrganizationsAsync Method](https://learn.microsoft.com/dotnet/api/microsoft.powerplatform.dataverse.client.serviceclient.discoveronlineorganizationsasync) to retrieve the environments the user has access to.
1. If any environments are returned by the `GetAllOrganizations` method, they will be listed in the console and you will be prompted to choose one by typing a number. If your choice is valid, the selected [OrganizationDetail](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.discovery.organizationdetail) data is passed to the `ShowUserId` method that executes a `WhoAmIRequest` and displays the `SystemUser.UserId` for the user in that environment.

### Clean up

This sample creates no records. No cleanup is required.