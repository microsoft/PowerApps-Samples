# Load the environment script
. "$PSScriptRoot\..\Common\EnterprisePolicyOperations.ps1"

function GetSubnetInjectionEnterprisePoliciesInResourceGroup
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
    $policies = GetEnterprisePoliciesInResourceGroup $subscriptionId "NetworkInjection" $resourceGroup
    $policies | Select-Object -Property ResourceId, Location, Name

}
GetSubnetInjectionEnterprisePoliciesInResourceGroup