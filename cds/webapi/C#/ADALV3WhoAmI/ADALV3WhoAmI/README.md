# Use the Web API with ADAL v3 library

This sample shows how to use the Web API with the v3 Azure Active Directory Authentication library

## How to run this sample

1. Download or clone the repo so that you have a local copy.
1. Open the sample solution in Visual Studio 
1. Edit line 15 to set the URL for your CDS for Apps instance.

    Change this: `string resource = "https://<your org>.dynamics.com";`
1. Press F5 to run the sample.
1. If you have not run any CDS for Apps samples before, you will be asked to give consent.
1. You will be prompted to choose a valid user and enter your password.
1. When the sample is finished, press any key to exit.


## What this sample does

This sample returns the `UserId` value from the `WhoAmI` function.


## How this sample works

This sample will use the credentials entered by the user with the ADAL v3.x library for .NET Clients.

### Setup

This sample doesn't require any setup.


### Demonstrate

Uses an ADAL (v3.x) and to open a popup for the user to enter their credentials and authenticate, then uses an [HttpClient](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=netframework-4.6.2) to using call the `WhoAmI` function to retrieve the users `UserId`.


### Clean up

This sample doesn't require any clean up since it doesn't create any records.