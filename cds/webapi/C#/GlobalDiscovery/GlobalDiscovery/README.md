# Demonstrates the Global Discovery Service

This sample shows how to access the Global Discovery Service using the Web API.

## How to run this sample

To run the sample:
1. Download or clone the sample so that you have a local copy.
2. Open the project solution file in Visual Studio.
3. Press F5 to build and run the sample.

## What this sample does

When run, the sample opens a logon form where you must specify the Microsoft Dataverse credentials for an enabled user. The program then displays to the console window a list of Dataverse environment instances that the specified user is a member of.

Other important aspects of this sample include:
1. Handles breaking API changes in different versions of Azure Active Directory Authentication Library (ADAL)
1. No dependency on helper code or a helper library since all required code, including authentication code, is provided.

### Setup

This sample doesn't require any setup.


### Demonstrate

Uses an HttpClient to authenticate using ADAL and then call the global Discovery Service to return information about available instances the user is a member of.

### Clean up

This sample doesn't require any clean up since it doesn't create any records.
