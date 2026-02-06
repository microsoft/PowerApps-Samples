---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates auditing capabilities in Dataverse including entity data auditing and user access auditing"
---

# Audit
Demonstrates auditing capabilities in Dataverse including entity data auditing and user access auditing

More information: [Audit](https://learn.microsoft.com/power-apps/developer/data-platform/auditing-overview)

## Samples

This folder contains the following samples:

|Sample folder|Description|Build target|
|---|---|---|
| | |.NET 6|

## Prerequisites

- Microsoft Visual Studio 2022
- Access to Dataverse with appropriate privileges for the operations demonstrated

## How to run samples

1. Clone or download the PowerApps-Samples repository
2. Navigate to `/dataverse/orgsvc/CSharp-NETCore/Audit/`
3. Open `Audit.sln` in Visual Studio 2022
4. Edit the `appsettings.json` file in the category folder root with your Dataverse environment details:
   - Set `Url` to your Dataverse environment URL
   - Set `Username` to your user account
5. Build and run the desired sample project

## appsettings.json

Each sample in this category references the shared `appsettings.json` file in the category root folder. The connection string format is:

```json
{
  "ConnectionStrings": {
    "default": "AuthType=OAuth;Url=https://yourorg.crm.dynamics.com;Username=youruser@yourdomain.com;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;RedirectUri=http://localhost;LoginPrompt=Auto"
  }
}
```

You can also set the `DATAVERSE_APPSETTINGS` environment variable to point to a custom appsettings.json file location if you prefer to keep your connection string outside the repository.

## See also

[SDK for .NET](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/overview)
