# Query the working hours of multiple users

This sample shows how to retrieve the working hours of multiple users by using the [QueryMultipleSchedulesRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.querymultipleschedulesrequest) message.

This sample requires an additional user that is not present in your system. Create the required user manually **as is** shown below in **Office 365** before you run the sample.

**First Name**: Kevin
**Last Name**: Cook
**Security Role**: Sales Manager
**UserName**: kcook@yourorg.onmicrosoft.com

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `QueryMultipleSchedulesRequest` message is intended to be used in a scenario where it contains data that is needed to search multiple resources for available time blocks that match the specified parameters.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Retrieves the current user's information using `WhoAmIRequest`.
2. Queries for the manually created user (Kevin Cook) by first and last name.
3. Validates that the required user exists before proceeding.

### Demonstrate

1. Creates a `QueryMultipleSchedulesRequest` with:
   - Resource IDs for both the current user and Kevin Cook
   - Time range from now to 7 days in the future
   - Time code set to `Available` to retrieve available time blocks
2. Executes the request to retrieve working hours information.
3. Displays the returned time information, including:
   - Number of time info records
   - Start and end times for each available block
   - Resource (user) information

### Clean up

This sample does not create any records that need to be deleted. It only queries existing user working hours information.

## Key Concepts

- **QueryMultipleSchedulesRequest**: Used to query available time blocks for multiple users simultaneously
- **TimeCode.Available**: Specifies that we want to retrieve available time slots
- **Working Hours**: The query returns information about when users are scheduled to be available for work
- **Resource Scheduling**: This functionality is useful for scheduling meetings, appointments, or resource allocation

## See Also

[Query schedules](https://learn.microsoft.com/dynamics365/customerengagement/on-premises/developer/schedule-collections-appointments)
[QueryMultipleSchedulesRequest Class](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.querymultipleschedulesrequest)
[Service calendar and scheduling](https://learn.microsoft.com/dynamics365/customer-service/basics-service-service-scheduling)
