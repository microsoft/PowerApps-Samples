# Load the environment script
. "$PSScriptRoot\..\Common\EnterprisePolicyOperations.ps1"

function GetCMKEnterprisePoliciesInResourceGroup
{
     param(
        [Parameter(
            Mandatory=$true,
            HelpMessage="The subscriptionId"
        )]
        [string]$subscriptionId,

        [Parameter(
            Mandatory=$true,
            HelpMessage="The resource group"
        )]
        [string]$resourceGroup
    )

    Write-Host "Logging In..." -ForegroundColor Green
    $connect = AzureLogin
    if ($false -eq $connect)
    {
        return
    }

    Write-Host "Logged In..." -ForegroundColor Green
    $cmkPolicies = GetEnterprisePoliciesInResourceGroup $subscriptionId "Encryption" $resourceGroup
    $cmkPolicies | Select-Object -Property ResourceId, Location, Name

}
GetCMKEnterprisePoliciesInResourceGroup