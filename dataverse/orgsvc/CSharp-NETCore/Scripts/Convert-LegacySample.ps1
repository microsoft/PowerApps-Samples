# Convert-LegacySample.ps1
# Assists with transforming legacy code to modern patterns
# Performs automated text replacements and outputs transformed code for review

[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [string]$LegacySamplePath,

    [Parameter(Mandatory=$false)]
    [string]$OutputPath = ""
)

$ErrorActionPreference = "Stop"

# Validate legacy sample path exists
if (-not (Test-Path $LegacySamplePath)) {
    Write-Error "Legacy sample path does not exist: $LegacySamplePath"
    exit 1
}

Write-Host "Analyzing legacy sample: $LegacySamplePath" -ForegroundColor Cyan
Write-Host ""

# Find source files
$csFiles = Get-ChildItem -Path $LegacySamplePath -Filter "*.cs" -Recurse | Where-Object {
    $_.FullName -notlike "*\obj\*" -and
    $_.FullName -notlike "*\bin\*" -and
    $_.Name -ne "AssemblyInfo.cs"
}

if ($csFiles.Count -eq 0) {
    Write-Error "No C# source files found in $LegacySamplePath"
    exit 1
}

Write-Host "Found $($csFiles.Count) source file(s):"
$csFiles | ForEach-Object { Write-Host "  - $($_.Name)" }
Write-Host ""

# Function to transform code
function Transform-Code {
    param(
        [string]$content
    )

    # Replace CrmServiceClient with ServiceClient
    $content = $content -replace '\bCrmServiceClient\b', 'ServiceClient'

    # Replace using Microsoft.Xrm.Tooling.Connector
    $content = $content -replace 'using Microsoft\.Xrm\.Tooling\.Connector;', '// Removed: using Microsoft.Xrm.Tooling.Connector;'

    # Replace namespace PowerApps.Samples with PowerPlatform.Dataverse.CodeSamples
    $content = $content -replace '\bnamespace PowerApps\.Samples\b', 'namespace PowerPlatform.Dataverse.CodeSamples'

    # Replace service.IsReady checks (common pattern)
    $content = $content -replace 'if \(service\.IsReady\)', 'if (!service.IsReady)'

    # Comment out SampleHelpers usage
    $content = $content -replace '\bSampleHelpers\.Connect\b', '// TODO: Replace with ServiceClient initialization from appsettings.json - SampleHelpers.Connect'
    $content = $content -replace '\bSampleHelpers\.\w+', '// TODO: Review and update - $&'

    # Comment out SystemUserProvider usage
    $content = $content -replace '\bSystemUserProvider\.\w+', '// TODO: Replace with modern pattern - $&'

    # Add TODO markers for WhoAmIRequest pattern (common)
    if ($content -match 'WhoAmIRequest') {
        $content = "// TODO: Review WhoAmIRequest usage for modern pattern`r`n" + $content
    }

    return $content
}

# Process each file
$transformedFiles = @()

foreach ($file in $csFiles) {
    Write-Host "Processing: $($file.Name)" -ForegroundColor Yellow

    $content = Get-Content -Path $file.FullName -Raw -Encoding UTF8
    $originalSize = $content.Length

    $transformed = Transform-Code -content $content

    $transformedFiles += @{
        OriginalPath = $file.FullName
        OriginalName = $file.Name
        Content = $transformed
        OriginalSize = $originalSize
        NewSize = $transformed.Length
        Reduction = [math]::Round((($originalSize - $transformed.Length) / $originalSize) * 100, 2)
    }

    Write-Host "  Original size: $originalSize bytes"
    Write-Host "  Transformed size: $($transformed.Length) bytes"
    Write-Host ""
}

# Display summary
Write-Host ""
Write-Host "Transformation Summary" -ForegroundColor Green
Write-Host "=====================" -ForegroundColor Green
Write-Host "Files processed: $($transformedFiles.Count)"
Write-Host ""

# Output transformed files
if ($OutputPath) {
    # Create output directory if specified
    if (-not (Test-Path $OutputPath)) {
        New-Item -ItemType Directory -Path $OutputPath | Out-Null
    }

    Write-Host "Writing transformed files to: $OutputPath" -ForegroundColor Cyan
    Write-Host ""

    foreach ($file in $transformedFiles) {
        $outPath = Join-Path $OutputPath $file.OriginalName
        Set-Content -Path $outPath -Value $file.Content -Encoding UTF8
        Write-Host "  Created: $outPath"
    }

    Write-Host ""
    Write-Host "Transformation complete!" -ForegroundColor Green
} else {
    # Display transformed content to console
    Write-Host "Transformed Code (review and manually apply changes):" -ForegroundColor Cyan
    Write-Host "========================================================" -ForegroundColor Cyan
    Write-Host ""

    foreach ($file in $transformedFiles) {
        Write-Host ""
        Write-Host "// ========================================" -ForegroundColor Magenta
        Write-Host "// File: $($file.OriginalName)" -ForegroundColor Magenta
        Write-Host "// ========================================" -ForegroundColor Magenta
        Write-Host $file.Content
        Write-Host ""
    }
}

Write-Host ""
Write-Host "Manual Review Checklist:" -ForegroundColor Yellow
Write-Host "========================" -ForegroundColor Yellow
Write-Host "- [ ] Extract core logic to Setup/Run/Cleanup methods"
Write-Host "- [ ] Replace SampleHelpers.Connect with ServiceClient + appsettings.json"
Write-Host "- [ ] Remove WPF login UI code (ExampleLoginForm)"
Write-Host "- [ ] Add entity tracking to entityStore for cleanup"
Write-Host "- [ ] Update error handling to modern try-catch-finally pattern"
Write-Host "- [ ] Convert early-bound types to late-bound if possible"
Write-Host "- [ ] Test with 'dotnet build' and 'dotnet run'"
Write-Host ""
Write-Host "Use New-ModernSample.ps1 to create the target project structure first."
