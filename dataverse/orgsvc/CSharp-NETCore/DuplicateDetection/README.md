---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates duplicate detection capabilities including enabling duplicate detection rules and detecting duplicate records"
---

# DuplicateDetection
Demonstrates duplicate detection capabilities including enabling duplicate detection rules and detecting duplicate records

More information: [DuplicateDetection](https://learn.microsoft.com/power-apps/developer/data-platform/detect-duplicate-data)

## Samples

This folder contains the following samples:

|Sample folder|Description|Build target|
|---|---|---|
|[EnableDuplicateDetection](EnableDuplicateDetection)|Demonstrates how to enable duplicate detection at the organization and entity levels, publish rules, and retrieve duplicate records|.NET 6|
|[DetectMultipleDuplicateRecords](DetectMultipleDuplicateRecords)|Demonstrates how to use BulkDetectDuplicatesRequest to detect duplicates across multiple records asynchronously|.NET 6|
|[UseDuplicatedetectionforCRUD](UseDuplicatedetectionforCRUD)|Demonstrates using the SuppressDuplicateDetection parameter to control duplicate detection during Create and Update operations|.NET 6|

## Prerequisites

- Microsoft Visual Studio 2022
- Access to Dataverse with appropriate privileges for the operations demonstrated

## How to run samples

1. Clone or download the PowerApps-Samples repository
2. Navigate to `/dataverse/orgsvc/CSharp-NETCore/DuplicateDetection/`
3. Open `DuplicateDetection.sln` in Visual Studio 2022
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
