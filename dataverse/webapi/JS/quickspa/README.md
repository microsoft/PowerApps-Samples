---
languages:
- javascript
products:
- power-platform
- power-apps
page_type: sample
description: "This quick start sample demonstrates how to connect to the Dataverse Web API using a SPA application."
---
# Dataverse Web API with client-side JavaScript and Visual Studio Code

This project represents the completed steps described in [Quickstart: Web API with client-side JavaScript and Visual Studio Code](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/quick-start-js-spa). This sample provides a way to test a Microsoft Entra app registration that is configured for a [Single Page Application (SPA)](https://developer.mozilla.org/docs/Glossary/SPA) to connect to Dataverse.


## Prerequisites

| Prerequisite | Description |
|--------------|-------------|
| **Privileges to create an Entra App registration** | You can't complete this quickstart without the ability to create a Microsoft Entra app registration to enable it.<br /><br /> If you aren't sure if you can, try the first step to [Register a SPA application](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/quick-start-js-spa#register-a-spa-application) and find out. |
| **Visual Studio Code** | If Visual Studio Code isn't installed on your computer, you must [download and install Visual Studio Code](https://code.visualstudio.com/download) to run this quickstart. |
| **Node.js** | Node.js is a runtime environment that allows you to run JavaScript on the server side. This quickstart creates a SPA application that runs JavaScript on the client side in a browser rather than the Node.js runtime. But [Node Package Manager (npm)](https://www.npmjs.com/) is installed with Node.js, and you need npm to install Parcel and the MSAL.js library. See [Install Node.js](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/quick-start-js-spa#install-nodejs) for instructions.|
| **Parcel** | Modern web applications typically have a lot of dependencies on open source libraries distributed using npm as well as scripts that need to be managed and optimized during the build process. These tools are usually called 'bundlers'. The most common one is [webpack](https://webpack.js.org/). This quickstart uses [Parcel](https://parceljs.org/) because it offers a simplified experience. <br /><br />For quickstarts and samples that show SPA applications using different frameworks and bundlers, see [Microsoft Entra Single-page applications samples](https://learn.microsoft.com/entra/identity-platform/sample-v2-code#single-page-applications). You can adapt these samples to use Dataverse Web API with the information shown in this quickstart.|
| **Web Technologies** | Knowledge of HTML, JavaScript, and CSS are required to understand how this quickstart works. Understanding how to [make network requests with JavaScript](https://developer.mozilla.org/docs/Learn_web_development/Core/Scripting/Network_requests) is essential. |


## How to run the sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Open the `dataverse/webapi/JS/quickspa` folder using Visual Studio Code.
1. At the root of the `quickspa` folder, create a `.env` configuration file based on the `.env.example` file provided. Follow the steps in [Register a SPA application](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/quick-start-js-spa#register-a-spa-application) to get values to replace the `CLIENT_ID` and `TENANT_ID` placeholder values. Set the `BASE_URL` to the URL for the Dataverse environment you want to connect to. See [Create the .env file](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/quick-start-js-spa#create-the-env-file) for more details.
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
1. Select the **WhoAmI** button.

   The message '**Congratulations! You connected to Dataverse using the Web API.**' is displayed with your `UserId` value from the [WhoAmIResponse complex type](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/whoamiresponse).

## Demonstrates

This sample demonstrates how a properly configured Entra SPA Application configuration can be used together with the [Microsoft Authentication Library for JavaScript (MSAL.js)](https://learn.microsoft.com/javascript/api/overview/msal-overview) to connect to Dataverse.

You can use the `.env` file you create in completing this quick start with the [Web API Data operations Samples (Client-side JavaScript)](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/web-api-samples-client-side-javascript) because the design is the same.

## Clean up

This sample doesn't create any records, so there is nothing to clean up. You should delete the Entra application registration if you aren't planning to use it anymore.