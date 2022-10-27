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

This sample will not open dialog to prompt you for connection information.

You must first set the `username` and `password` variables in the `Program.Main` method before running this sample.

## What this sample does

This sample uses the OData global discovery service with a user's credentials to determine which environments they can connect with. This sample uses HttpClient and the Microsoft.Identity.Client libaries to authenticate.

If one or more environments are returned, the sample will prompt the user to choose one, and then use a `WhoAmIRequest` to return the `SystemUser.UserId` for that environment.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

This sample requires no special setup except that there are valid user credential `username` and `password` to use.

In Cloud.cs there is a `Cloud` enumeration for each of the known discovery region. Each enumeration member is decorated with a `Description` notation. All of these members have the URL for the global discovery service for that cloud set as the description.

Instance.cs contains an `Instance` class that describes the data returned by the discovery service.

### Demonstrate

1. Using the user credentials and the `cloud` value, the program uses the `GetInstances` static method to retrieve all known environments for the user.
1. The `GetInstances` method extracts the data center global discovery service Url from the member `Description` decoration and uses it together with the user credentials to instantiate an HttpClient. 
1. The `GetToken` static method uses the Microsoft.Identity.Client methods to return an access token with the user's credentials. If MFA is required for the tenant, a dialog will open for the user to add their username and password.
1. The data is deserialized into a  `List<Instance>` and returned.
1. If any environments are returned by the `GetInstances` method, they will be listed in the console and you will be prompted to choose one by typing a number. If your choice is valid, the static `ShowUserId` method uses the  instance data and the user's credentials to execute a `WhoAmIRequest` and return the `SystemUser.UserId` for the user in that environment.

### Clean up

This sample creates no records. No cleanup is required.