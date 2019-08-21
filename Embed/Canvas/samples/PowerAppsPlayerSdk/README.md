
This topic introduces the capabilities of the PowerApps embedding SDKs and shows you how PowerApps can help customers extend your applications with experiences ranging from creating simple custom forms to adding a feature-rich screens.

## What this sample does

This sample uses the PowerApps embedding player SDK to play a canvas app in the host.

## How this sample works

To simulate creating or editing a canvas app from the host via the sample, you need to do the following steps:
The Player SDK enables developers to:
- Embed an app by App Id
- Embed an app by Logical name, Environment Id and Tenant Id
- Set data on the app 
- Register host methods that can be called in the App  
- Subscribe to events from Player (like App created, App loaded and App Errored) 

### Setup

1. Download or clone the repository so that you have a local copy.
2. To Play a PowerApp by AppId
- Replace `appid` in `PlayerSDK.ts` file
#### How to get AppId
- If you are using @microsoft/powerappsauthoringsdk, app Id is returned in appSaved and appPublished event.
- Else in powerapps.com, on the Apps tab, click or tap the ellipsis(…), then Details- copy the App ID (GUID). 
3. To Play a PowerApp by LogicalName
- Replace <logicalname>', '<environmentid>', '<tenantid> in PlayerSDK.ts file
- If using the @microsoft/powerappsauthoringsdk, these values will be returned in appSaved and appPublished events (only when the application is created within a solution).
#### How to get LogicalName, EnvironmentId, TenantId 
LogicalName 
   1. Go to powerapps.com.
   2. Click on the Solutions link in the left-hand side navigation 
   3. Open the solution in which the App was added or created. Copy the “Name”.

Environment Id 
   1. Go to powerapps.com.
   2. From top-right corner select the environment in which you application exists.
   3. Copy the id (GUID) after environments portion in URL.

Tenant Id
   1. Go to powerapps.com.
   2. Click on the Apps tab, click or tap the ellipses(...), then Details.
   3. In the Web Link section, copy the GUID after 'tenantid='.
   
3. There is a sample App (Player SDK sample) in the repository that can be imported into your environment to get started quickly.

## How to run this sample
 
1. Open Command Prompt and navigate to the folder where the sample code is cloned.
2. Run `npm install`
3. Run `npm run build`
4. Run `npm start`
5. Navigate to [http://localhost:9000/](https://localhost:9000)
  
