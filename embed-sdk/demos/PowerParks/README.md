# PowerApps Embedding Sdk build demo

This demo introduces the new PowerApps Embedding SDKs and show you how PowerApps can help your customers extend your applications with experiences ranging from creating simple custom forms, to adding feature rich screens.

## What this sample does

This sample uses the PowerApps Embedding Authoring Sdk to create/edit a new app from host.

## How this sample works
In order to simulate creating/editing an app from a host via the sample, you will need do the following steps:

### Setup
1. Download or clone the repo so that you have a local copy.
2. Import the "Map Address (SDK Build Demo).msapp" file in your environment.
   Sign in to https://web.powerapps.com, you can select "Create an app" -> "Canvas" -> "Open" -> "Browse", and then select the .msapp file from the repository.
3. Once the app is saved, copy the App ID  from "Details"
   On https://web.powerapps.com, select "Apps" in menu -> select More Commands(...) for the app you want to edit -> Select "Details" -> Record the App ID for later use
   ![Screenshot](screenshot.png)

## How run this sample
1. Open the sample solution in Visual Studio and press F5 to run the sample. 
   * If you want to edit an exisiting app, click on settings on the top right hand side and enter the App ID copied in previous step and click "Save"
   * This is optional - If you want the bing map to render in the app , set the BingAPIKey in PowerAppsDetails.js. Look for the comment "// Set the Bing API here" and set the key.
  
  Refer to this link to follow the above demo : https://mybuild.techcommunity.microsoft.com/sessions/77090?source=sessions#top-anchor