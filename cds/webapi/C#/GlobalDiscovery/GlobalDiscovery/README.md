# Use the Web API Global Discovery Service

This sample shows how to use the Web API Global discovery Service

## How to run this sample

To run the sample:
1. Download or clone the sample so that you have a local copy.
2. Open the project solution file in Visual Studio 2017 and restore the Nuget packages.
3. Press F5 to run the sample.
4. The samples prompts a login window where you need to specify the Microsoft Dataverse user credentials. 

## What this sample does

This sample returns the available Dataverse instances for a given user credentials.

### Setup

This sample doesn't require any setup.


### Demonstrate

Uses a HttpClient to authenticate using ADAL and call the global discovery service to return information about available instances the user can connect to.

### Clean up

This sample doesn't require any clean up since it doesn't create any records.
