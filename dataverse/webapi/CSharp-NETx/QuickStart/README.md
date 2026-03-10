---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample shows how to authenticate with the Microsoft Dataverse web service and invoke a Dataverse Web API."
---

# A quickstart to the Dataverse Web API

This sample shows how to authenticate with the Microsoft Dataverse web service and invoke a Dataverse Web API. The sample uses the [Microsoft Authentication Library](https://learn.microsoft.com/azure/active-directory/develop/msal-overview) (MSAL) for web service authentication, and invokes the Web API function [WhoAmI](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/whoamiresponse).

Learn more in [Quick Start: Web API sample (C#)](https://learn.microsoft.com/powerapps/developer/data-platform/webapi/quick-start-console-app-csharp?tabs=msal).

## How to run this sample

1. Download or clone the repo so that you have a local copy.
1. Open the solution (.sln) file in Visual Studio 2019.
1. Edit line 19 of Program.cs to set the URL for your Dataverse test environment.<br/>
    `string resource = "https://<env-name>.api.<region>.dynamics.com";`
1. Press `F5` to run the sample.
1. You're prompted to choose a valid Dataverse user and enter your password.
1. When the sample is finished, press any key to exit.

## What this sample does

This sample returns the `UserId` value of the logged on user by invoking the `WhoAmI` unbound Web API function.

## How this sample works

This program makes use of a `clientID` and `redirectUri` shared by all Web API samples.

### Setup

This sample doesn't require any setup other than specifying the test environment URL.

### Demonstrates

- Microsoft Authentication Library (MSAL) for service authentication
- Web client configuration
- Web API unbound function invocation and web service response parsing

### Clean up

This sample doesn't require any clean up, since it doesn't create any Dataverse table rows.
