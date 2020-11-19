# Power Apps component framework: Data set grid component

This sample component shows how to change the user experience of interacting with the dataset. For example, you only see the home page grid on an entity homepage as a table. You can build your code component that can display the data as per your choice. This sample shows the records as tiles instead of the regular tabular grid.

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

In this sample, we have the input parameter defined in component manifest file with the data-set tag. This is the input property that gets bound to the component. 

This component has two important containers that are added onto the main div which is added onto the div that is passed onto the component.  The first container holds the tiles that show the record data from the view and the second container is for the `Load More button` that shows when there are records that need more area that can fit in one page.

Both the containers are generated and refreshed whenever the `updateView` method is called. For the first container, we generate the tiles based on the information in the columns and the number of records. This ensures we display a tile for each record along with its information on it.

If there exists a following page for the records, the load more button is displayed i.e., the second container is visible and is hidden if there are no more pages in the result set. 

On the click of load more button, we load the next page of records and append it to the existing result set and the logic to hide or show the button remains same as earlier as shown in the code. This is taken care by the `onLoadMoreButtonClick` method which is bound to the button.

The `toggleLoadMoreButtonWhenNeeded` function takes the input as the data set and checks, whether the data set, has next page, and if the button is hidden or visible and respectively hides or shows the button. 

The `onRowClick` function attaches the context of the record using its GUID value and invokes the openForm method of the NavigationAPI to open that respective record. This method is bound to each tile that gets generated as part of the `createGridBody` method.

The `getSortedColumnsOnView` method returns the list of columns based on the defined order on the view.

## How to run the sample

See [How to run the sample components](https://github.com/microsoft/PowerApps-Samples/blob/master/component-framework/README.md) for more information about how to build and import components into Microsoft Dataverse.

## See also

[Power Apps component framework overview](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/overview)<br/>
[Power Apps component framework API reference](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/reference/)<br/>
[Power Apps component framework manifest schema reference](https://docs.microsoft.com/en-us/powerapps/developer/component-framework/manifest-schema-reference/)