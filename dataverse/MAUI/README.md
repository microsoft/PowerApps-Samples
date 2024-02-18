# DataverseMauiApp example app

## Prerequisites

- [Visual Studio 2022](https://visualstudio.microsoft.com/vs)
- [.NET 7](https://dotnet.microsoft.com/download/dotnet/7.0)
- [MAUI (Multi-platform App UI)](https://dotnet.microsoft.com/en-us/learn/maui/first-app-tutorial/install)

## How to run this sample

- Clone or download the PowerApps-Samples repository.
- Open the PowerApps-Samples\dataverse\MAUI\DataverseMauiApp\DataverseMauiApp.sln file using Visual Studio 2022.
- Press F5

## What this sample shows

- [MAUI (Multi-platform App UI)](https://dotnet.microsoft.com/en-us/apps/maui) app using .NET 8
- Single Sign On using [MSAL (Microsoft Authentication Library) WAM (Web Account Manager)](https://learn.microsoft.com/en-us/entra/msal/dotnet/acquiring-tokens/desktop-mobile/wam) for Microsoft accounts.
  - This can be turned on/off in MauiProgram.cs by modifying UseSingleSignOn property
- Using the Dataverse [Global Discovery Web API](https://learn.microsoft.com/en-us/power-apps/developer/data-platform/discovery-service) to get list of Dataverse environments

Welcome             |  Environments
:------------------:|:-------------------------:
![welcome](DataverseMauiApp/welcome.png)  |  ![environments](DataverseMauiApp/environments.png)
