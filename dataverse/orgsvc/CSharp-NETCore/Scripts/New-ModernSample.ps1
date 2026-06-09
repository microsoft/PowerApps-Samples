# New-ModernSample.ps1
# Creates a new modern sample project within a category folder

[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [string]$CategoryName,

    [Parameter(Mandatory=$true)]
    [string]$SampleName,

    [Parameter(Mandatory=$true)]
    [string]$Description,

    [Parameter(Mandatory=$false)]
    [string]$LearnMoreUrl = ""
)

$ErrorActionPreference = "Stop"

# Get the script's directory and navigate to CSharp-NETCore root
$scriptDir = Split-Path -Parent $PSCommandPath
$csharpNETCoreRoot = Split-Path -Parent $scriptDir

# Verify category exists
$categoryPath = Join-Path $csharpNETCoreRoot $CategoryName
if (-not (Test-Path $categoryPath)) {
    Write-Error "Category folder '$CategoryName' does not exist. Run New-CategoryFolder.ps1 first."
    exit 1
}

# Create sample directory
$samplePath = Join-Path $categoryPath $SampleName
if (Test-Path $samplePath) {
    Write-Error "Sample folder '$SampleName' already exists in category '$CategoryName'"
    exit 1
}

Write-Host "Creating sample folder: $samplePath"
New-Item -ItemType Directory -Path $samplePath | Out-Null

# Create .csproj file
$csprojPath = Join-Path $samplePath "$SampleName.csproj"
Write-Host "Creating $SampleName.csproj..."
$csprojContent = @"
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <RootNamespace>PowerPlatform.Dataverse.CodeSamples</RootNamespace>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <Content Include="..\appsettings.json" Link="appsettings.json">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="6.0.1" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="6.0.0" />
    <PackageReference Include="Microsoft.PowerPlatform.Dataverse.Client" Version="1.1.14" />
  </ItemGroup>

</Project>
"@
Set-Content -Path $csprojPath -Value $csprojContent -Encoding UTF8

# Create Program.cs skeleton
$programPath = Join-Path $samplePath "Program.cs"
Write-Host "Creating Program.cs..."
$programContent = @"
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// $Description
    /// </summary>
    /// <remarks>
    /// TODO: Add detailed description and prerequisites
    ///
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// </remarks>
    class Program
    {
        private static readonly List<EntityReference> entityStore = new();

        #region Sample Methods

        /// <summary>
        /// Sets up sample data required for the demonstration
        /// </summary>
        private static void Setup(ServiceClient service)
        {
            Console.WriteLine("Setting up sample data...");
            // TODO: Create any entities or data needed for the Run() method
            // Add created entities to entityStore for cleanup
        }

        /// <summary>
        /// Demonstrates the main sample functionality
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Console.WriteLine("Running sample...");
            // TODO: Add primary demonstration code here
        }

        /// <summary>
        /// Cleans up sample data created during execution
        /// </summary>
        private static void Cleanup(ServiceClient service, bool deleteCreatedRecords)
        {
            Console.WriteLine("Cleaning up...");
            if (deleteCreatedRecords && entityStore.Count > 0)
            {
                Console.WriteLine(`$"Deleting {entityStore.Count} created records...");
                foreach (var entityRef in entityStore)
                {
                    service.Delete(entityRef.LogicalName, entityRef.Id);
                }
            }
        }

        #endregion

        #region Application Setup

        /// <summary>
        /// Contains the application's configuration settings.
        /// </summary>
        IConfiguration Configuration { get; }

        /// <summary>
        /// Constructor. Loads the application configuration settings from a JSON file.
        /// </summary>
        Program()
        {
            // Get the path to the appsettings file. If the environment variable is set,
            // use that file path. Otherwise, use the runtime folder's settings file.
            string? path = Environment.GetEnvironmentVariable("DATAVERSE_APPSETTINGS");
            if (path == null) path = "appsettings.json";

            // Load the app's configuration settings from the JSON file.
            Configuration = new ConfigurationBuilder()
                .AddJsonFile(path, optional: false, reloadOnChange: true)
                .Build();
        }

        static void Main(string[] args)
        {
            Program app = new();

            // Create a Dataverse service client using the default connection string.
            ServiceClient serviceClient =
                new(app.Configuration.GetConnectionString("default"));

            if (!serviceClient.IsReady)
            {
                Console.WriteLine("Failed to connect to Dataverse.");
                Console.WriteLine("Error: {0}", serviceClient.LastError);
                return;
            }

            Console.WriteLine("Connected to Dataverse.");
            Console.WriteLine();

            bool deleteCreatedRecords = true;

            try
            {
                Setup(serviceClient);
                Run(serviceClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred:");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Cleanup(serviceClient, deleteCreatedRecords);

                Console.WriteLine();
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                serviceClient.Dispose();
            }
        }

        #endregion
    }
}
"@
Set-Content -Path $programPath -Value $programContent -Encoding UTF8

# Create README.md
$readmePath = Join-Path $samplePath "README.md"
Write-Host "Creating README.md..."

$seeAlsoSection = ""
if ($LearnMoreUrl) {
    $seeAlsoSection = @"

## See also

[$Description]($LearnMoreUrl)
"@
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

# $SampleName

$Description

## What this sample does

TODO: Describe the functionality demonstrated by this sample.

## How this sample works

### Setup

TODO: Describe any setup operations performed.

### Run

TODO: Describe the main operations performed.

### Cleanup

TODO: Describe cleanup operations.

## Demonstrates

TODO: List the key SDK classes, messages, or patterns demonstrated:
- Key class or message used
- Important pattern demonstrated

## Sample Output

TODO: Include example console output

``````
Connected to Dataverse.

Setting up sample data...
Running sample...
[Sample output here]
Cleaning up...

Press any key to exit.
``````$seeAlsoSection
"@
Set-Content -Path $readmePath -Value $readmeContent -Encoding UTF8

# Add project to solution
$slnPath = Join-Path $categoryPath "$CategoryName.sln"
if (Test-Path $slnPath) {
    Write-Host "Adding project to solution..."
    Push-Location $categoryPath
    try {
        dotnet sln add "$SampleName\$SampleName.csproj" | Out-Null
        Write-Host "Project added to solution successfully."
    } catch {
        Write-Warning "Failed to add project to solution: $_"
    } finally {
        Pop-Location
    }
} else {
    Write-Warning "Solution file not found. Project not added to solution."
}

Write-Host ""
Write-Host "Sample '$SampleName' created successfully in category '$CategoryName'" -ForegroundColor Green
Write-Host "Location: $samplePath" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:"
Write-Host "1. Edit Program.cs to implement the sample logic"
Write-Host "2. Update README.md with detailed documentation"
Write-Host "3. Test with 'dotnet build' and 'dotnet run'"
Write-Host "4. Update the category README.md to include this sample in the table"
