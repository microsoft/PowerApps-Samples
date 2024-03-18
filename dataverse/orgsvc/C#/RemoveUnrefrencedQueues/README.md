# Sample: Create a basic plug-in

This sample shows how to write a simple plug-in that updates the activity parties on an email removing any related item's created by ARC that are no longer party to the email.  This assumes that the org has enabled ARC's Multi-Related for Email feature.

## How to run this sample

1. Download or clone the [Samples](https://github.com/Microsoft/PowerApps-Samples) repo so that you have a local copy. This sample is located under PowerApps-Samples-master\cds\orgsvc\C#\RemoveUnrefrencedQueues.
2. Open the sample solution in Visual Studio, navigate to the project's properties, and verify the assembly will be signed during the build. Press F6 to build the sample's assembly (RemoveUnrefrencedQueues.dll).
3. Run the Plug-in Registration tool and register the sample's assembly in the D365 server's sandbox and database. When registering a step, specify the Create message, email table, and syncronous mode on Post Create stage.
4. Using the D365 app, perform the appropriate operation to invoke the message and table request that you registered the plug-in on (create an email).
6. When you are done testing, unregister the assembly and step.

## What this sample does

When executed upon creation of an email, the plugin removes any ARC created activity parties that are no longer party to the email. 

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

- Look at an email
- Find the related activity parties on the email
- Find the originating queue for any related entities
- Remove any related activity parties where the originating queue is no longer party to this email. Ex. remove the case created by the supprot queue if this email no longer has the Support queue in the to/cc/bcc fields

### Demonstrates

1. How to get the relatied parties
2. How to find the originating queue for the related parties created by ARC
3. How to updat the related parties on the email

### See also

[Write a plug-in](https://docs.microsoft.com/en-us/powerapps/developer/common-data-service/write-plug-in)  
[Register a plug-in](https://docs.microsoft.com/en-us/powerapps/developer/common-data-service/register-plug-in)