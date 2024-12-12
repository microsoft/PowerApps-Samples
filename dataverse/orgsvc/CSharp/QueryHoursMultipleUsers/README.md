# Query the working hours of multiple users

This sample shows how to retrieve the working hours of multiple users by using the [QueryMultipleSchedulesRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.querymultipleschedulesrequest) message.

This sample requires additional users that are not present in your system. Create the required user manually **as is** shown below in **Office 365** before you run the sample.

**First Name**: Kevin<br/>
**Last Name**: Cook<br/>
**Security Role**: Sales Manager<br/>
**UserName**: kcook@yourorg.onmicrosoft.com<br/>

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `QueryMultipleScheduleRequest` message is intended to be used in a scenario where it contains data that is needed to search multiple resources for available time block that match the specified parameters.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. Retrieves the current user's information and also the user, that you have created manually in **Office 365**.

### Demonstrate

1. The `QueryMultipleScheduleRequest` message retrieves the working hours of the current user and the user that you have created manually.

### Clean up

Display an option to delete the records created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
