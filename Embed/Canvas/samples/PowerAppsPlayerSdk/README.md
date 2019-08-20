# PowerApps Embedding SDK 

This topic introduces the capabilities of the PowerApps embedding SDKs and shows you how PowerApps can help your customers extend your applications with experiences ranging from creating simple custom forms to adding a feature-rich screens.

## What this sample does

This sample uses the PowerApps embedding player SDK to play a canvas app in the host.

## How this sample works

To simulate creating or editing a canvas app from the host via the sample, you need to do the following steps:
The PowerApps Player SDK enables developers to:
	1. Embed an app by App Id
	2. Embed an app by Logical name, Environment Id and Tenant Id. 
	3. Set data on the app 
	4. Register host methods that can be called in the App. Schema for the methods and data can be set using PowerApps Authoring SDK.
	5. Subscribe to events from Player(like App created, App loaded and App Errored) 

### Setup

1. Download or clone the repository so that you have a local copy.
2. Replace <appid> in PlayerSDK.ts file to play the App you would like to.
3. There is a sample App(Player SDK sample) in the repository that can be imported in your environment to quickly get started.

## How to run this sample

1. Navigate to the folder where the sample code is cloned
2. Run npm install
3. Run npm run build
4. Run npm start
5. Navigate to [http://localhost:9000/](https://localhost:9000)
  