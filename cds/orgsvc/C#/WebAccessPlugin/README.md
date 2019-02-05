# Sample: Web access from a plug-in

This sample shows how to write a plug-in that can access web (network) resources like a web service or feed. You can download the sample from [here](https://github.com/Microsoft/PowerApps-Samples/tree/master/cds/orgsvc/C%23/WebAccessPlugin).

## How to run this sample

1. Download or clone the [Samples](https://github.com/Microsoft/PowerApps-Samples) repo so that you have a local copy. This sample is located under PowerApps-Samples-master\cds\orgsvc\C#\WebAccessPlugin.
2. Open the sample solution in Visual Studio, navigate to the project's properties, and verify the assembly will be signed during the build. Press F6 to build the sample's assembly (WebAccessPlugin.dll).
3. Run the Plug-in Registration tool and register the sample's assembly in the D365 server's sandbox and database. When registering a step, specify a web URI string (i.e., http://www.microsoft.com) in the unsecure configuration field.
4. Using the D365 app, perform the appropriate operation to invoke the message and entity request that you registered the plug-in on.
5. When the plug-in runs, the code on line 63 throws an exception. This results in an ISV error dialog being displayed in the D365 app. In that dialog, select **Download file** to view the error log which contains the output of the plug-in including the web service response.
6. When you are done testing, unregister the assembly and step.

## What this sample does

When executed, the plug-in downloads web page data from the specified web service address (or the default address) and then writes that data to an error log file or the tracing service.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks the constuctor's unsecure configuration parameter for a web address value; otherwise, the default value is used.
2. The `System.Net.WebClient` class is used by the plug-in's `Execute` method to download web page data.
3. An exception is thrown to make the web service response available through an error log download file for testing purposes.

### Demonstrate

1. The `WebClient.DownloadData` method downloads web page data using a web service request.
2. That response data is written to an exception error log or (optionally) the tracing service for later viewing in the D365 app.
3. The plug-in code also shows how to catch a WebException from the web service and process it.

### See also
[Access external web resources](https://docs.microsoft.com/en-us/powerapps/developer/common-data-service/access-web-services)<br/>
[Register a plug-in](https://docs.microsoft.com/en-us/powerapps/developer/common-data-service/register-plug-in)