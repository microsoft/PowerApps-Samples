# Power Apps component framework: Web API component

The web API component is designed to perform create, retrieve, update and delete actions. The component renders four buttons, which can be clicked to invoke different web API actions. The result of the web API call is injected into a HTML div element at the bottom of the code component.

## Before you can try the sample components

To try the sample components, you must first:

1. Install [Npm](https://www.npmjs.com/get-npm) (comes with Node.js) or [Node.js](https://nodejs.org/en/) (comes with npm). We recommend LTS (Long Term Support) version 10.15.3 LTS because it seems to be the most stable.

1. Install [.NET Framework 4.6.2 Developer Pack](https://dotnet.microsoft.com/download/dotnet-framework/net462). 

1. If you don’t already have Visual Studio 2017 or later, follow one of these options:
   - Option 1: Install [Visual Studio 2017](https://docs.microsoft.com/visualstudio/install/install-visual-studio?view=vs-2017) or later.
   - Option 2: Install [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1) and then install [Visual Studio Code](https://code.visualstudio.com/Download).

1. Install [Microsoft Power Apps CLI](https://aka.ms/PowerAppsCLI).
1. [Download](https://github.com/microsoft/PowerApps-Samples/tree/master/component-framework) the sample components so that you have a local copy.

## What this sample does

By default, in the sample, the component is configured to perform the create, retrieve, update actions on the Account entity and set the name and revenue fields in the web API examples.

The `createRecord` method renders three buttons, which allows you to create an account record with the revenue field set to different values (100, 200, 300). When you click one of the create buttons, the button’s `onClick` event handler checks the value of the button clicked and use the web API action to create an account record with the revenue field set to the button’s value. The name field of the account record will be set to web API code component (Sample) with a random int appended to the end of the string. The callback method from the web API call injects the result of the web API call (success or failure) into the custom control’s result div.

The `deleteRecord` method renders a button which opens a lookup dialog when clicked. The lookup dialog allows you to select the account record you want to delete. Once an account record is selected from the lookup dialog, it is passed to the deleteRecord to delete the record from the database. The callback method from the web API call injects the result of the web API call (success or failure) into the custom control’s result div.

The FetchXML `retrieveMultiple` method renders a button in the code component. onClick of this button, FetchXML is generated and passed to the `retrieveMultiple` method to calculate the average value of the revenue field for all the accounts records. The callback method from the web API call injects the result of the web API call (success or failure) into the custom control’s result div.

The OData retrieveMultiple method renders a button in the code component. onClick of this button, OData string is generated and passed to the `retrieveMultiple` method to retrieve all account records with a name field that is like `code component Web API (Sample)`, which is true for all account records created by this code component example.

On successful retrieve of the records, the code component has logic to count how many account records have the revenue field set to 100, 200 or 300, and display this count into an Odata status container div on the code component. The callback method from the web API call injects the result of the web API call (success or failure) into the custom control’s result div.

## How to run the sample

See [How to run the sample components](https://github.com/microsoft/PowerApps-Samples/blob/master/component-framework/README.md) for more information about how to build and import components into Microsoft Dataverse.

## See also

[Power Apps component framework overview](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/overview)<br/>
[Power Apps component framework API reference](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/reference/)<br/>
[Power Apps component framework manifest schema reference](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/manifest-schema-reference/)