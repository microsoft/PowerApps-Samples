# Enhanced quick start to the Dataverse Web API

This sample shows how to authenticate with the Dataverse web service using Microsoft Authentication Library for .NET (MSAL.NET) and invoke a Web API unbound function (WhoAmI).

## How to run this sample

1. Download or clone the repo so that you have a local copy.
1. Open the sample solution in Visual Studio 2019.
1. Edit the App.config file to set the URL for your Dataverse test environment, logon username and password.
1. Press F5 to run the sample.
1. If you do not provide a username/password in the App.config file, you will be prompted for logon.
1. When the sample is finished, press any key to exit.

## What this sample does

This sample returns the `UserId` value of the logged on user by invoking the `WhoAmI` unbound Web API function.

## How this sample works

This program makes use of a clientID and redirectUri that is shared by all Web API samples.

### Setup

This sample doesn't require any setup other that specifying the test environment URL.

### Demonstrate

- Microsoft Authentication Library for .NET online authentication
- Web client configuration
- Refreshing the authentication access token on every web service call
- Web API unbound function invocation and web service response parsing

### Clean up

This sample doesn't require any clean up since it doesn't create any Dataverse table rows.
