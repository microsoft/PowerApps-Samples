
This topic demonstrates the capabilities of PowerApps embedding SDKs and shows how PowerApps can help customers extend their applications with experiences ranging from creating simple custom forms to adding a feature-rich screens.

## What this sample does

This sample uses the PowerApps embedding player SDK to play a canvas app in the host.

## How this sample works

To simulate creating or editing a canvas app from the host via the sample, you need to do the following steps:

- Embed an app by **App Id**
- Embed an app by **Logical name**, **Environment Id**, and **Tenant Id**
- Set data on the app 
- Register the host methods that can be called in the app  
- Subscribe to the events from Player (like App created, App loaded and App Errored) 

### Setup

1. Download or clone the repository so that you have a local copy.

2. To Play a PowerApp by AppId
   - Replace `appid` in `PlayerSDK.ts` file
   - To get appid value, log into [PowerApps](https://powerapps.com), click on **Apps** tab from the left navigation bar and click or tap the ellipsis(…) and then select **Details**. In the page,copy the App ID value. 

3. To Play a PowerApp by LogicalName
   - Replace `logicalname`, `environmentid`, `tenantid` in PlayerSDK.ts file
    - To get the `logicalName`, go to [PowerApps](https://powerapps.com), click on the **Solutions** in the left navigation bar
    - Open the solution in which the app is added or created. Copy the **Name**.
    - To get the environmentid, go to [PowerApps](https://powerapps.com)
    - From top-right corner select the environment in which you application exists.
    - Copy the id (GUID) after environments portion in URL.
    - To get the tenantid, go to [PowerApps](https://powerapps.com)   
    - Click on the **Apps tab**, click or tap the ellipses(...) and then **Details**.
    - In the **Web Link** section, copy the GUID after `tenantid=`.
   
4. There is a sample App (Player SDK sample) in the repository that can be imported into your environment to get started quickly.

## How to run this sample
 
1. Open Command Prompt and navigate to the folder where the sample code is cloned.
2. Run `npm install`
3. Run `npm run build`
4. Run `npm start`
5. Navigate to [http://localhost:9000/](https://localhost:9000)
  
