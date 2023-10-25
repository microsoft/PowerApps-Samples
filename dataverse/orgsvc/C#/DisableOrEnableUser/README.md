
# Sample: Disable a user

This sample shows how to disable and enable a system user account in an online or on-premise/IFD environment.

## How to run this sample

The Customer Engagement user account under which you run this program must have the System Administrator role in order to enable/disable a system user.

Before building this sample, open the solution in Visual Studio and select **View** > **Task List**. There are two TODO comments that you must follow to provide the required information about an existing system user in your organization.

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The sample obtains the identifier of an existing system user and either disables or enables that user account.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Retrieves the identifier of an existing system user that you specify (see [How to run this sample](#how-to-run-this-sample)).

### Demonstrate

Demonstrates using`SetStateRequest` to disable and enable a system user. Also shows how to retrieve information about a system user.

To view the summary of the specified system user in Customer Engagement, navigate to **Settings** > **Security** > **Users** and select the target system user account in the list. If desired, choose the **Disabled Users** system view to filter the list of all users. The user's status should be "Disabled".

### Clean up

1. Displays an option to enable the user account that was disabled in the `Main()` method.

    Answering "yes" is optional in case you want to examine the disabled user account in Customer Engagement. You can manually enable the user account in Active Directory or Office 365 to achieve the same result.
