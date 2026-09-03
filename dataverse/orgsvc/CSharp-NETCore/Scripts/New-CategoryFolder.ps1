# New-CategoryFolder.ps1
# Creates a new category folder structure in CSharp-NETCore with standard files

[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [string]$CategoryName,

    [Parameter(Mandatory=$true)]
    [string]$Description,

    [Parameter(Mandatory=$false)]
    [string]$LearnMoreUrl = ""
)

$ErrorActionPreference = "Stop"

# Get the script's directory and navigate to CSharp-NETCore root
$scriptDir = Split-Path -Parent $PSCommandPath
$csharpNETCoreRoot = Split-Path -Parent $scriptDir

# Create category directory
$categoryPath = Join-Path $csharpNETCoreRoot $CategoryName
if (Test-Path $categoryPath) {
    Write-Warning "Category folder '$CategoryName' already exists at $categoryPath"
    $response = Read-Host "Do you want to continue? (y/n)"
    if ($response -ne 'y') {
        Write-Host "Operation cancelled."
        exit
    }
} else {
    Write-Host "Creating category folder: $categoryPath"
    New-Item -ItemType Directory -Path $categoryPath | Out-Null
}

# Create appsettings.json
$appsettingsPath = Join-Path $categoryPath "appsettings.json"
if (-not (Test-Path $appsettingsPath)) {
    Write-Host "Creating appsettings.json..."
    $appsettingsContent = @"
{
  "ConnectionStrings": {
    "default": "AuthType=OAuth;Url=https://yourorg.crm.dynamics.com;Username=youruser@yourdomain.com;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;RedirectUri=http://localhost;LoginPrompt=Auto"
  }
}
"@
    Set-Content -Path $appsettingsPath -Value $appsettingsContent -Encoding UTF8
}

# Create README.md
$readmePath = Join-Path $categoryPath "README.md"
if (-not (Test-Path $readmePath)) {
    Write-Host "Creating README.md..."

    $learnMoreSection = ""
    if ($LearnMoreUrl) {
        $learnMoreSection = "`n`nMore information: [$CategoryName]($LearnMoreUrl)"
    }

    $readmeContent = @"
---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "$Description"
---

# $CategoryName
$Description$learnMoreSection

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
2. Navigate to ``/dataverse/orgsvc/CSharp-NETCore/$CategoryName/``
3. Open ``$CategoryName.sln`` in Visual Studio 2022
4. Edit the ``appsettings.json`` file in the category folder root with your Dataverse environment details:
   - Set ``Url`` to your Dataverse environment URL
   - Set ``Username`` to your user account
5. Build and run the desired sample project

## appsettings.json

Each sample in this category references the shared ``appsettings.json`` file in the category root folder. The connection string format is:

``````json
{
  "ConnectionStrings": {
    "default": "AuthType=OAuth;Url=https://yourorg.crm.dynamics.com;Username=youruser@yourdomain.com;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;RedirectUri=http://localhost;LoginPrompt=Auto"
  }
}
``````

You can also set the ``DATAVERSE_APPSETTINGS`` environment variable to point to a custom appsettings.json file location if you prefer to keep your connection string outside the repository.

## See also

[SDK for .NET](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/overview)
"@
    Set-Content -Path $readmePath -Value $readmeContent -Encoding UTF8
}

# Create solution file
$slnPath = Join-Path $categoryPath "$CategoryName.sln"
if (-not (Test-Path $slnPath)) {
    Write-Host "Creating solution file: $CategoryName.sln..."
    Push-Location $categoryPath
    try {
        dotnet new sln -n $CategoryName | Out-Null
        Write-Host "Solution file created successfully."
    } catch {
        Write-Error "Failed to create solution file: $_"
    } finally {
        Pop-Location
    }
}

Write-Host ""
Write-Host "Category '$CategoryName' created successfully at: $categoryPath" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:"
Write-Host "1. Update the README.md with specific sample descriptions"
Write-Host "2. Use New-ModernSample.ps1 to add samples to this category"
Write-Host "3. Update appsettings.json with your environment details"
