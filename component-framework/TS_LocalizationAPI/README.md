# Power Apps component framework: Localization API component

This sample showcases how localization is done for code components. 

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

In this sample, we use the Increment component to localize the text that is displayed on the increment button based on the user’s selected language. Power Apps component framework uses the concept of implementing String(resx) web resources that is used to manage the localized strings shown on any user interface. 

To localize an existing project, all you need to do is to create additional resource(resx) files, one each for a specific language as mentioned in the string web resources and include them as part of the control’s manifest file under the `resources` node.

The framework identifies the user’s language and returns the strings from that language-specific resource file when you try to access the string using `context.resources.getString` method.

In this sample, two languages Spanish and Finnish with the language codes 3082 and 1035 are respectively defined. We made a copy of the `Increment component` sample and renamed it to Localization API. All the corresponding files including the files in the subfolders are renamed accordingly.

In the strings folder under `TS_LocalizationAPI`, two additional resx files with the suffixes corresponding to Spanish and Finnish as 3082 and 1035 are added. The new files created should have their file names ending as `{filename}.3082.resx` and `{filename}.1035.resx` because the framework relies on this naming convention to identify which resource file should be picked for reading the strings for the user.

Ensure that the keys used for strings in all these resource files share the same name across all the languages. Now, when the component is rendered on the UI, we see in the code that we retrieve the value to be displayed on the button using `context.resources.getString("PCF_LocalizationSample_ButtonLabel")`.

When this line of code is executed, the framework automatically identifies the language of the user and picks up the value for the button label using the key provided in the corresponding language file we defined.

## How to run the sample

See [How to run the sample components](https://github.com/microsoft/PowerApps-Samples/blob/master/component-framework/README.md) for more information about how to build and import components into Microsoft Dataverse.

## See also

[Power Apps component framework overview](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/overview)<br/>
[Power Apps component framework API reference](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/reference/)<br/>
[Power Apps component framework manifest schema reference](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/manifest-schema-reference/)