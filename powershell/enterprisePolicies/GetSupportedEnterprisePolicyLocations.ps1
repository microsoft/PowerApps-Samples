# Load thescript
. "$PSScriptRoot\Common\EnterprisePolicyOperations.ps1"

function GetSupportedEnterprisePolicyLocations() {

    Set-ExecutionPolicy unrestricted -Scope Process
    Write-Host "Logging In..." -ForegroundColor Green
    $connect = AzureLogin
    Write-Host "Logged In" -ForegroundColor Green
    Write-Host "Getting supported locations" -ForegroundColor Green
    ((Get-AzResourceProvider -ProviderNamespace Microsoft.PowerPlatform).ResourceTypes | Where-Object ResourceTypeName -eq enterprisePolicies).Locations

}

GetSupportedEnterprisePolicyLocations