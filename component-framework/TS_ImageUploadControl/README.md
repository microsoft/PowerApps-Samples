# Power Apps component framework: Image upload component

This sample component renders as an `Upload` button to upload the image and a default image when the component loads for the first time. When you click on the `Upload`, a file explorer pops up to pick an image. The selected image renders within the component. Meanwhile, the `Remove button` is shown if we need to reset. When you click on the `Remove` button, the default image is displayed. 

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

The `successCallback` method will be triggered and the resource content injects in the `successCallback`. Then you use the image element 'src' points to the content and the default image loads.

The `device.pickFile` method opens a dialog box to select files from your computer (web client) or mobile device (mobile clients). For desktop, it opens the file explorer, for the mobile client, it opens the library of the photo. When you click on the `Upload` button, the device API `pickFile` triggers and the user picks up the file. Once the file is successfully picked, the file's filename, file content will be injected in the `successCallback`.

## How to run the sample

See [How to run the sample components](https://github.com/microsoft/PowerApps-Samples/blob/master/component-framework/README.md) for more information about how to build and import components into Microsoft Dataverse.

## See also

[Power Apps component framework overview](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/overview)<br/>
[Power Apps component framework API reference](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/reference/)<br/>
[Power Apps component framework manifest schema reference](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/manifest-schema-reference/)