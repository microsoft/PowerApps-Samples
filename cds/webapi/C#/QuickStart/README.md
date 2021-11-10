# A quick start to the Dataverse Web API

| **C#** | **.NET Framework 4.6** |

This sample shows how to authenticate with the Dataverse web service and invoke a Web API. The solution contains two projects that differ only in the authentication library employed (either ADAL or MSAL) for web service authentication.

## How to run this sample

1. Download or clone the repo so that you have a local copy.
1. Open the sample solution in Visual Studio 2019.
1. Edit line 14 of Program.cs to set the URL for your Dataverse test environment.
1. Press F5 to run the sample.
1. You will be prompted to choose a valid Dataverse user, and then enter your logon password.
1. When the sample is finished, press any key to exit.

## What this sample does

This sample returns the `UserId` value of the logged on user by invoking the `WhoAmI` unbound Web API function.

## How this sample works

This program makes use of a clientID and redirectUri that is shared by all Web API samples.

### Setup

This sample doesn't require any setup other that specifying the test environment URL.

### Demonstrate

- Active Directory Authentication Library (ADAL) or Microsoft Authentication Library (MSAL) for service authentication
- Web client configuration
- Web API unbound function invocation and web service response parsing

### Clean up

This sample doesn't require any clean up since it doesn't create any Dataverse table rows.
