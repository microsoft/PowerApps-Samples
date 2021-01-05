# Power Apps component framework: Linear input component

This sample component changes the user experience of interacting with numeric types on the form. Instead of typing in the numbers, the linear input component provides a linear slider using which the value of the attribute can be set on the form.

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

In this sample, a `type-group` is defined and named it as `numbers` which includes decimal, whole, floating and currency value types into that group in the manifest. This group is used to bind the component property.

An input element of type range with min and max value set to 1 and 1000, respectively is created. A label element is created which shows the value that is relative to the position of the slider. Attach a function `refreshData` to the `eventlistener` on the input of the component.

Create a local variable for saving the `context` and `notifyOutputChanged`. Assign the `context` and `notifyOutputChanged` from the parameters that are passed as part of the `init` function.

Implement the logic for the `refreshData` function. As you can see in the sample, we take the value from the inputElement and set the value of the component, `innerHTML` property of the `labelElement` and then call the `notifyOutputChanged` so that the changes are cascaded up above the framework layer.

In the `updateView` method, we get the value of the attribute from the `context.parameters` and then set it to the value variable which stores the component value and also the input elements in the component.

## How to run the sample

See [How to run the sample components](https://github.com/microsoft/PowerApps-Samples/blob/master/component-framework/README.md) for more information about how to build and import components into Microsoft Dataverse.

## See also

[Power Apps component framework overview](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/overview)<br/>
[Power Apps component framework API reference](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/reference/)<br/>
[Power Apps component framework manifest schema reference](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/manifest-schema-reference/)