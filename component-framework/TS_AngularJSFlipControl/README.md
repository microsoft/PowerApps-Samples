# Power Apps component framework: Angular flip component

This sample component shows how to use third-party libraries to create components in Power Apps component framework. The flip sample component is implemented based on angular.js, angular-ui, angular-animate, angular-sanitize, bootstrap. The code may not reveal the best practices for the mentioned third-party libraries.

## Before you can try the sample components

To try the sample components, you must first:

1. Install [Npm](https://www.npmjs.com/get-npm) (comes with Node.js) or [Node.js](https://nodejs.org/en/) (comes with npm). We recommend LTS (Long Term Support) version 12.16.1 LTS because it seems to be the most stable.

1. Install [.NET Framework 4.6.2 Developer Pack](https://dotnet.microsoft.com/download/dotnet-framework/net462). 

1. If you don't already have Visual Studio 2017 or later, follow one of these options:
   - Option 1: Install [Visual Studio 2017](https://docs.microsoft.com/visualstudio/install/install-visual-studio?view=vs-2017) or later.
   - Option 2: Install [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1) and then install [Visual Studio Code](https://code.visualstudio.com/Download).

1. Install [Microsoft Power Apps CLI](https://aka.ms/PowerAppsCLI).
1. [Download](https://github.com/microsoft/PowerApps-Samples/tree/master/component-framework) the sample components so that you have a local copy.

## What this sample does

This sample shows how to add dependencies for third-party libraries, showcasing how to perform data-binding between Power Apps component framework, component model and third-party inner data model in bi-direction.

The flip component sample consists of a label and a button. When you click on the button, the text on the label toggles.

- When the component is loaded, the label shows the text based on the bind attribute value. The `context.parameters.[property_name].attributes` contains the associated metadata.
For TwoOptions fields, `context.parameters.[property_name].Options` will include both true and false value option.
- Clicking on the Flip button, the label will update value using `notifyOutputEvents` method, getOutputs method will be called asynchronously and will flow to Power Apps component framework.
- ClientAPI updates the bind attribute value, and the updated value flows to the component label. You can also use `ClientAPI` to update an attribute value to trigger control's updateView method. The component then updates the third-party model and the label gets updated.

## How to run the sample

See [How to run the sample components](https://github.com/microsoft/PowerApps-Samples/blob/master/component-framework/README.md) for more information about how to build and import components into Microsoft Dataverse.

## See also

[Power Apps component framework overview](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/overview)<br/>
[Power Apps component framework API reference](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/reference/)<br/>
[Power Apps component framework manifest schema reference](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/manifest-schema-reference/)