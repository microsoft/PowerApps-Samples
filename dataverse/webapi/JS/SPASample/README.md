---
languages:
- javascript
products:
- power-platform
- power-apps
page_type: sample
description: "This project contains samples that demonstrates how to use client-side JavaScript to perform data operation with the Dataverse Web API"
---
# Web API Data operations Samples (Client-side JavaScript)

This project provides a common [Single Page Application (SPA)](https://developer.mozilla.org/docs/Glossary/SPA) runtime experience for multiple client-side JavaScript samples that use the Dataverse Web API. See the table of samples in the [Demonstrates](#demonstrates) section for details.

This sample requires a `.env` file that contains configurations to run the sample. The steps to get the values for this configuration are described in  [Quickstart: Web API with client-side JavaScript and Visual Studio Code](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/quick-start-js-spa). If you have trouble connecting with this sample, try connecting with the simpler quick start sample.


## Prerequisites

| Prerequisite | Description |
|--------------|-------------|
| **Privileges to create an Entra App registration** | You can't run this sample application without a Microsoft Entra app registration to enable it.<br /><br /> Complete the steps in [Quickstart: Web API with client-side JavaScript and Visual Studio Code](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/quick-start-js-spa) to get an `.env` file you can use to run this sample application.|
| **Visual Studio Code** | If Visual Studio Code isn't installed on your computer, you must [download and install Visual Studio Code](https://code.visualstudio.com/download) to run this sample. |
| **Node.js** | Node.js is a runtime environment that allows you to run JavaScript on the server side. This sample creates a SPA application that runs JavaScript on the client side in a browser rather than the Node.js runtime. But [Node Package Manager (npm)](https://www.npmjs.com/) is installed with Node.js, and you need npm to install Parcel and the MSAL.js library. See [Install Node.js](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/quick-start-js-spa#install-nodejs) for instructions.|
| **Parcel** | Modern web applications typically have a lot of dependencies on open source libraries distributed using npm as well as scripts that need to be managed and optimized during the build process. These tools are usually called 'bundlers'. The most common one is [webpack](https://webpack.js.org/). This application uses [Parcel](https://parceljs.org/) because it offers a simplified experience.|
| **Web Technologies** | Knowledge of HTML, JavaScript, and CSS are required to understand how this quickstart works. Understanding how to [make network requests with JavaScript](https://developer.mozilla.org/docs/Learn_web_development/Core/Scripting/Network_requests) is essential. |


## How to run the sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Open the `dataverse/webapi/JS/SPASample` folder using Visual Studio Code.
1. At the root of the `SPASample` folder, create a `.env` configuration file based on the `.env.example` file provided. Follow the steps in [Register a SPA application](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/quick-start-js-spa#register-a-spa-application) to get values to replace the `CLIENT_ID` and `TENANT_ID` placeholder values. Set the `BASE_URL` to the URL for the Dataverse environment you want to connect to. See [Create the .env file](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/quick-start-js-spa#create-the-env-file) for more details.
1. Open a terminal window in Visual Studio Code.
1. Type `npm install` and press <kbd>Enter</kbd> to install the `devDependencies` and `dependencies` items from the `package.json` file. These include `[parcel](https://www.npmjs.com/package/parcel)`, `[dotenv](https://github.com/motdotla/dotenv#readme)`, the `[@azure/msal-browser](https://www.npmjs.com/package/@azure/msal-browser)` library and others.
1. Type `npm start` and press <kbd>Enter</kbd> to start the local web server on port 1234.

 You should expect output to the terminal that looks like this:

   ```
   Server running at http://localhost:1234
   Built in 1.08s
   ```

1. Press <kbd>Ctrl</kbd> + click the [http://localhost:1234](http://localhost:1234) link to open your browser.
1. In your browser, select the **Login** button.

   The **Sign in to your account** dialog opens.

1. Select the account that has access to Dataverse.

   The first time you access using a new application (client) ID value, you see this **Permissions requested** dialog:

   ![Permissions requested dialog](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/media/dataverse-web-api-quickstart-spa-permissions-requested.png)

1. Select **Accept** on the **Permissions requested** dialog.

   You are now logged in and can select from the available samples to run them and view the output. 

   ![Dataverse Web API JavaScript SPA sample application](https://learn.microsoft.com/power-apps/developer/data-platform/media/dataverse-web-api-javascript-spa-sample-app.png)

1. Click the button to run the sample you want and observe the output. Other samples can't run until a running sample completes.

   While the sample runs, open the browser developer tools and observe the network traffic.


## Demonstrates

This sample provides a shell where multiple samples can run in a common application. This table describes each of the samples currently available:

|Sample|FileName|Description|
|---|---|---|
|**Template**|`TemplateSample.js`|This sample demonstrates the common interface that samples in this application exposes. New samples can be created by copying the `TemplateSample.js` file, renaming the file and implementing new logic in the sample. [Learn more about the sample interface](#sample-interface)|
|**Basic Operations**|`BasicOperationsSample.js`|This sample implements the operations specified by the [Web API Basic Operations Sample](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/web-api-basic-operations-sample).|
|**Query Data**|`QueryDataSample.js`|This sample implements the operations specified by the [Web API query data sample](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/web-api-query-data-sample).|
|**Conditional Operations**|`ConditionalOperationsSample.js`|This sample implements the operations specified by the [Web API Conditional Operations Sample](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/web-api-conditional-operations-sample).|
|**Functions and Actions**|`FunctionsAndActions.js`|This sample implements the operations specified by the [Web API Functions and Actions Sample](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/web-api-functions-actions-sample).|
|**Batch Operations**|`BatchSample.js`|This sample demonstrates the `DataverseWebAPI.Client.Batch` method by creating several account records in and outside changeset included batch operation as described by [Execute batch operations using the Web API](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/execute-batch-operations-using-web-api) |


## Sample interface

Each sample is defined as a [JavaScript Class](https://developer.mozilla.org/docs/Web/JavaScript/Reference/Classes) which has a constructor that requires a `DataverseWebAPI.Client` instance and a reference to a `container` element in the UI where it will write output. 

Samples may import from other libraries. All of them import the `Util.js` library because it includes methods to write to the UI. The **Functions and Actions** sample also includes the `src\solutions\IsSystemAdminFunction_1_0_0_0_managed.js` library because it contains the base64 encoded contents of a solution that the sample needs to import.

Each sample has the following public methods:

|Method|Description|
|---|---|
|`SetUp`|Responsible for creating any data necessary for the code that will be included in the `Run` method. Adds any new records created to the `#entityStore` array to be referenced in the `Run` method and deleted in the `CleanUp` method.|
|`Run`|Contains the code demonstrating the data operations included in the sample.<br />Adds any new records created to the `#entityStore` array to be deleted in the `CleanUp` method.<br />Each operation is defined by a private asynchronous method so that this method can manage the operations within a try/catch block. If any errors occur, the `CleanUp` method is called to delete any record created before the error.|
|`CleanUp`|Responsible for deleting any records referenced by the `#entityStore` array items.|

## Clean up

Each sample will make every effort to delete any records created by the sample. If an error occurs during the `Run` method, the `CleanuUp` method is used to delete any records up to that point.

You should delete the Entra application registration if you aren't planning to use it anymore.