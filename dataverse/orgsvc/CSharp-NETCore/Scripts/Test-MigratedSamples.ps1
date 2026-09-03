# Test-MigratedSamples.ps1
# Validates that all samples in a category build successfully

[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [string]$CategoryName,

    [Parameter(Mandatory=$false)]
    [switch]$TestSolution,

    [Parameter(Mandatory=$false)]
    [switch]$Verbose
)

$ErrorActionPreference = "Stop"

# Get the script's directory and navigate to CSharp-NETCore root
$scriptDir = Split-Path -Parent $PSCommandPath
$csharpNETCoreRoot = Split-Path -Parent $scriptDir

# Verify category exists
$categoryPath = Join-Path $csharpNETCoreRoot $CategoryName
if (-not (Test-Path $categoryPath)) {
    Write-Error "Category folder '$CategoryName' does not exist at $categoryPath"
    exit 1
}

Write-Host ""
Write-Host "Testing samples in category: $CategoryName" -ForegroundColor Cyan
Write-Host "Category path: $categoryPath"
Write-Host ""

$results = @()
$totalTests = 0
$passedTests = 0
$failedTests = 0

# Test solution file if requested
if ($TestSolution) {
    $slnPath = Join-Path $categoryPath "$CategoryName.sln"
    if (Test-Path $slnPath) {
        Write-Host "Testing solution: $CategoryName.sln" -ForegroundColor Yellow
        Write-Host "Running: dotnet build `"$slnPath`""
        Write-Host ""

        $totalTests++
        Push-Location $categoryPath
        try {
            if ($Verbose) {
                $buildOutput = dotnet build "$slnPath" 2>&1
                Write-Host $buildOutput
            } else {
                $buildOutput = dotnet build "$slnPath" 2>&1 | Out-String
            }

            if ($LASTEXITCODE -eq 0) {
                $passedTests++
                $results += @{
                    Name = "$CategoryName.sln"
                    Type = "Solution"
                    Status = "PASS"
                    Path = $slnPath
                }
                Write-Host "Solution build: PASS" -ForegroundColor Green
            } else {
                $failedTests++
                $results += @{
                    Name = "$CategoryName.sln"
                    Type = "Solution"
                    Status = "FAIL"
                    Path = $slnPath
                    Error = $buildOutput
                }
                Write-Host "Solution build: FAIL" -ForegroundColor Red
                if (-not $Verbose) {
                    Write-Host "Error output:" -ForegroundColor Red
                    Write-Host $buildOutput
                }
            }
        } catch {
            $failedTests++
            $results += @{
                Name = "$CategoryName.sln"
                Type = "Solution"
                Status = "FAIL"
                Path = $slnPath
                Error = $_.Exception.Message
            }
            Write-Host "Solution build: FAIL - $($_.Exception.Message)" -ForegroundColor Red
        } finally {
            Pop-Location
        }
        Write-Host ""
    } else {
        Write-Warning "Solution file not found: $CategoryName.sln"
        Write-Host ""
    }
}

# Find all .csproj files in sample directories
$projects = Get-ChildItem -Path $categoryPath -Filter "*.csproj" -Recurse | Where-Object {
    $_.FullName -notlike "*\bin\*" -and
    $_.FullName -notlike "*\obj\*"
}

if ($projects.Count -eq 0) {
    Write-Warning "No project files found in category '$CategoryName'"
    exit 0
}

Write-Host "Found $($projects.Count) project(s) to test"
Write-Host ""

# Test each project
foreach ($project in $projects) {
    $projectName = $project.BaseName
    $projectDir = $project.DirectoryName

    Write-Host "Testing: $projectName" -ForegroundColor Yellow
    Write-Host "Path: $($project.FullName)"
    Write-Host "Running: dotnet build"
    Write-Host ""

    $totalTests++
    Push-Location $projectDir
    try {
        if ($Verbose) {
            $buildOutput = dotnet build 2>&1
            Write-Host $buildOutput
        } else {
            $buildOutput = dotnet build 2>&1 | Out-String
        }

        if ($LASTEXITCODE -eq 0) {
            $passedTests++
            $results += @{
                Name = $projectName
                Type = "Project"
                Status = "PASS"
                Path = $project.FullName
            }
            Write-Host "Build result: PASS" -ForegroundColor Green
        } else {
            $failedTests++
            $results += @{
                Name = $projectName
                Type = "Project"
                Status = "FAIL"
                Path = $project.FullName
                Error = $buildOutput
            }
            Write-Host "Build result: FAIL" -ForegroundColor Red
            if (-not $Verbose) {
                Write-Host "Error output:" -ForegroundColor Red
                Write-Host $buildOutput
            }
        }
    } catch {
        $failedTests++
        $results += @{
            Name = $projectName
            Type = "Project"
            Status = "FAIL"
            Path = $project.FullName
            Error = $_.Exception.Message
        }
        Write-Host "Build result: FAIL - $($_.Exception.Message)" -ForegroundColor Red
    } finally {
        Pop-Location
    }

    Write-Host ""
    Write-Host ("=" * 80)
    Write-Host ""
}

# Display summary
Write-Host ""
Write-Host "Test Summary for $CategoryName" -ForegroundColor Cyan
Write-Host ("=" * 80) -ForegroundColor Cyan
Write-Host "Total tests:  $totalTests"
Write-Host "Passed:       $passedTests" -ForegroundColor Green
Write-Host "Failed:       $failedTests" -ForegroundColor $(if ($failedTests -gt 0) { "Red" } else { "Green" })
Write-Host ""

if ($passedTests -eq $totalTests) {
    Write-Host "All tests passed!" -ForegroundColor Green
    $exitCode = 0
} else {
    Write-Host "Some tests failed. Review the output above for details." -ForegroundColor Red
    $exitCode = 1

    Write-Host ""
    Write-Host "Failed projects:" -ForegroundColor Red
    foreach ($result in $results | Where-Object { $_.Status -eq "FAIL" }) {
        Write-Host "  - $($result.Name) ($($result.Type))"
    }
}

Write-Host ""

exit $exitCode
