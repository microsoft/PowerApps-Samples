languages:
- typescript
products:
- powerapps
page_type: sample
description: "Sample that shows how to create an increment component using PowerApps component framework

# PowerApps component framework: Implementing increment component

This sample component shows how to bind data with PowerApps component framework and error handling.

## Prerequisites

1. Install [Npm](https://www.npmjs.com/get-npm) (comes with Node.js) or [Node.js](https://nodejs.org/en/) (comes with npm). We recommend LTS (Long Term Support) version 10.15.3 LTS because it seems to be the most stable.

1. Install [.NET Framework 4.6.2 Developer Pack](https://dotnet.microsoft.com/download/dotnet-framework/net462). 

1. If you don’t already have Visual Studio 2017 or later, follow one of these options:
   - Option 1: Install [Visual Studio 2017](https://docs.microsoft.com/visualstudio/install/install-visual-studio?view=vs-2017) or later.
   - Option 2: Install [.NET Core 2.2 SDK](https://dotnet.microsoft.com/download/dotnet-core/2.2) and then install [Visual Studio Code](https://code.visualstudio.com/Download).

1. Install [Microsoft PowerApps CLI](https://aka.ms/PowerAppsCLI).

## How to run the sample

1. Download or clone the repo so that you have a local copy.
2. To see the sample component in runtime, you need to create a solution zip file, import into Common Data Service. To create a solution zip file:

   - Go into the directory where you have downloaded the folder. 
   - Go into the sample component, which you want to see it in runtime (for example, Increment component).
   - Run the command `npm install` to install all the required dependencies.
   - Create a new folder inside the sample component folder (for example, Increment component) and navigate into the folder. 
   - Create a new solution project inside the folder using the command `pac solution init --publisher-name <Name of the publisher> --publisher-prefix <Publisher prefix>`.
 
   - Once the new solution project is created, you need to refer to the location where the sample component is located. You can add the reference using the command `pac solution add-reference --path <Path to the root of the sample component>`.

   - To generate a zip file from your solution project, you need to `cd` into your solution project directory and build the project using the command `msbuild /t:restore`.
   - Again, run the command `msbuild`.
   - The generated solution zip file is located in the `Solution\bin\debug` folder.
   - Manually [import the solution into Common Data Service](https://docs.microsoft.com/en-us/dynamics365/customer-engagement/customize/import-update-upgrade-solution) using the web portal once the zip file is ready or see the [Authenticating to your organization](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/import-custom-controls#authenticating-to-your-organization) and [Deployment](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/import-custom-controls#deploying-code-components) sections to import using PowerApps CLI commands.
   - To add code components to model-driven apps and canvas apps, see topics [Add components to model-driven apps](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/add-custom-controls-to-a-field-or-entity) and [Add components to canvas apps](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/component-framework-for-canvas-apps#add-components-to-a-canvas-app)

## What this sample does

The increment component renders as a textbox with an `Increment` button in the runtime. The text box shows the current value and the `Increment` button is clickable. Whenever you click on the button, the value within the textbox is increased by 1. You can change the increment value when you are configuring the component to the field on the form.

The increment value can be changed to any number you wish. The updated value flows to the framework through the *notifyOutputChanged* method.

If the value in the text box is a valid integer, then it updates the value to the component framework. You can continuously click the `Increment` button and update it. If it’s an invalid integer, an error message pops out.

## See also

[PowerApps component framework overview](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/overview)<br/>
[PowerApps component framework API reference](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/reference/)<br/>
[PowerApps component framework manifest schema reference](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/manifest-schema-reference/)