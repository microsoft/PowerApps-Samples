# Sample: Create a basic plug-in

This sample shows how to write a simple plug-in that creates a follow-up activity. You can obtain the sample from [here](https://github.com/Microsoft/PowerApps-Samples/tree/master/dataverse/orgsvc/CSharp/FollowupPlugin).

## How to run this sample

1. Download or clone the [Samples](https://github.com/Microsoft/PowerApps-Samples) repo so that you have a local copy. This sample is located under PowerApps-Samples\dataverse\orgsvc\CSharp\FollowupPlugin.
2. Open the sample solution in Visual Studio, navigate to the project's properties, and verify the assembly will be signed during the build. Press F6 to build the sample's assembly (FollowupPluginXX.dll).
3. Run the Plug-in Registration tool and register the sample's assembly in the Dataverse server's sandbox and database. When registering a step, specify the Create message, account table, and asynchronous mode.
4. In Power Apps, navigate to the environment that you registered the plug-in with. Perform the appropriate data operation to invoke the message and table request that you registered the plug-in on (create an account).
5. After the plug-in runs, you should see a new trace log entry "FollowupPlugin: Successfully created the task activity" and a new activity with the subject "Send e-mail to the new customer." that is scheduled to activate in 7 days.
6. When you are done testing, unregister the assembly and step in your environment.

## What this sample does

When executed upon creation of an account, the plug-in creates an activity to remind the user to follow-up with the account customer in 7 days.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Demonstrate

1. How to create a task activity and schedule it for a future date.
2. How to use the tracing service to log run-time information.
3. How to catch exceptions from the web service and process it.

## More information

The Visual Studio solution contains two projects that demonstrate a simple Microsoft Dataverse plug-in. The difference between the projects is that in one project the `FollowupPlugin` class implements the `IPlugin` interface while the other project's `FollowupPlugin` class inherits from the `PluginBase` class. The core functionality of each plug-in is the same.

### See also

[Write a plug-in](https://learn.microsoft.com/powerapps/developer/common-data-service/write-plug-in)  
[Register a plug-in](https://learn.microsoft.com/powerapps/developer/common-data-service/register-plug-in)
