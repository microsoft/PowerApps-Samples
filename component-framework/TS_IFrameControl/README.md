# Power Apps component framework: IFRAME component

This sample describes how to bind a code component to different fields on the form and use the value of these fields as input properties to the component.

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

This sample component renders an `IFRAME` which displays `Bing Maps UR`L. The component is bound to two floating point fields on the form, which are passed as parameters to the component and injected into the `IFRAME URL` to update the Bing Map to the latitude and longitude of the provided inputs.

Update the manifest file to include binding to two additional fields on the form. This change informs the Power Apps component framework that these bound fields need to be passed to the component during initialization and whenever one of the values is updated.

Additional bound properties may be required or not. This will be enforced during the component configuration when the component is being bound to the form. This can be configured by setting the `required` attribute of the property node in the component manifest. Set the value to false if you don't want to require the component property be bound to a field.

`ComponentFramework.d.ts` needs to be updated to add two fields to `IInputs` interface. This is the format the Power Apps component framework passes the field values. Adding these values to the IInputs interface allows your TypeScript file to reference the values and compile successfully.

The initial rendering generates an `IFRAME` element and appends it to the controls container. This `IFRAM`E is used to display the Bing Map. The url of the IFRAME is set to a Bing Map URL and includes the bound fields (latitudeValue and longitudeValue) in the url to center the map at the provided location.

The `updateView` method is invoked whenever one of these fields are updated on the form. This method updates the url of the Bing Map IFRAME to use the new latitude and longitude values passed to the component. To view this component in run time, bind the component to a field on the form like any other code component.

## How to run the sample

See [How to run the sample components](https://github.com/microsoft/PowerApps-Samples/blob/master/component-framework/README.md) for more information about how to build and import components into Microsoft Dataverse.

## See also

[Power Apps component framework overview](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/overview)<br/>
[Power Apps component framework API reference](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/reference/)<br/>
[Power Apps component framework manifest schema reference](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/manifest-schema-reference/)