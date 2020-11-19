# Power Apps component framework: Navigation API component

This sample component explores the various methods available as part of the Power Apps component framework navigation API. In this sample, you create a series of input elements of type buttons which calls into the respective methods of the navigation API that matches with the value displayed.

## Before you can try the sample components

To try the sample components, you must first:

1. Install [Npm](https://www.npmjs.com/get-npm) (comes with Node.js) or [Node.js](https://nodejs.org/en/) (comes with npm). We recommend LTS (Long Term Support) version 12.16.1 LTS because it seems to be the most stable.

1. Install [.NET Framework 4.6.2 Developer Pack](https://dotnet.microsoft.com/download/dotnet-framework/net462). 

1. If you don’t already have Visual Studio 2017 or later, follow one of these options:
   - Option 1: Install [Visual Studio 2017](https://docs.microsoft.com/visualstudio/install/install-visual-studio?view=vs-2017) or later.
   - Option 2: Install [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1) and then install [Visual Studio Code](https://code.visualstudio.com/Download).

1. Install [Microsoft Power Apps CLI](https://aka.ms/PowerAppsCLI).
1. [Download](https://github.com/microsoft/PowerApps-Samples/tree/master/component-framework) the sample components so that you have a local copy.

## What this sample does

The `openAlertDialog` method provides the capability to display an alert dialog containing a message and a button. You can also implement callback methods when the alert dialog is closed or if an error is encountered when loading the dialog. In this sample when you click on the `openAlertDialogButton` an alert dialog pops up and sets the value of it to `Alert dialog closed` when the dialog is closed either using the `OK` button or the `X` button.

The `openConfirmDialog` method provides the ability to display an alert dialog containing a message and two buttons. You can use this method to implement different logic based on the button clicked. You can implement the success callback which is called when the dialog is closed by clicking either of the buttons.   This sample shows you a confirm dialog when you click on the `openConfirmDialogButton `and sets the value of it to `Ok` or `Cancel`, or `X `depending on the button that was clicked.

The `openFil`e method provides the ability to open a file. You’d need to pass in the file object which has the filename, content, mimetype and the filesize. You can also pass in the optional parameter of the mode you want to open the file as 1 or 2, 1 being the default which opens the file in read or open mode.   This sample opens a file named `SampleDemo.txt` in save mode on clicking the openFileButton.

The `openUrl` method provides the ability to open a URL. You need to pass the URL as a string to the method and also pass the optional parameters of height, width and openInNewWindow as true if you want the URL to be opened in a new window.   This sample opens a new window and loads the microsoft.com home page on clicking the `openUrlButton`.

## How to run the sample

See [How to run the sample components](https://github.com/microsoft/PowerApps-Samples/blob/master/component-framework/README.md) for more information about how to build and import components into Microsoft Dataverse.

## See also

[Power Apps component framework overview](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/overview)<br/>
[Power Apps component framework API reference](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/reference/)<br/>
[Power Apps component framework manifest schema reference](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/manifest-schema-reference/)