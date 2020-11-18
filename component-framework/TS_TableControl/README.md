# Power Apps component framework: Table component

This sample component renders a table with two columns. The left column shows the name of the API method or property, and the right column shows the value returned by the API. You can open this component on the different type of devices or modify your language or user settings to see the values adjust correctly in the table.

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

This sample provides examples on how to use methods from the `Client`, `UserSettings`, `Utility`, and `Formatting` interfaces. This component also showcases two utility functions, `setFullScreen` and `lookupObjects`. These functions are invoked by clicking the button rendered as part of the code component. The `setFullScreen` button toggles the component in and out of full screen mode. The `lookupObjects` button opens a lookup dialog, and then inject the selected record as text into div.

In this sample, we render an HTML button and attach an onClick event handler `onLookupObjectsButtonClick` to the button. On click of this button, we invoke `context.utils.lookupObjects()` method and pass as a parameter an array of entity names.

This method returns a Promise object, representing the completion or failure of the call to the lookup dialog. If the promise is resolved successfully, the lookup object which the user selected is passed as a parameter into the callback method and can be referenced as `data.id`, `data.name`, `data.entityType`.

The callback method injects this information as HTML into a div rendered on the code component to showcase the selected results to the user. If the promise is rejected, the error callback method is invoked where your component can handle the error scenario accordingly.

## How to run the sample

See [How to run the sample components](https://github.com/microsoft/PowerApps-Samples/blob/master/component-framework/README.md) for more information about how to build and import components into Microsoft Dataverse.

## See also

[Power Apps component framework overview](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/overview)<br/>
[Power Apps component framework API reference](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/reference/)<br/>
[Power Apps component framework manifest schema reference](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/manifest-schema-reference/)