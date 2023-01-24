# Load the environment script
. "$PSScriptRoot\..\Common\EnterprisePolicyOperations.ps1"

function GetSubnetInjectionEnterprisePoliciesInSubscription
{
     param(
        [Parameter(
            Mandatory=$true,
            HelpMessage="The subscriptionId"
        )]
        [string]$subscriptionId
    )

    Write-Host "Logging In..." -ForegroundColor Green
    $connect = AzureLogin
    if ($false -eq $connect)
    {
        return
    }

    Write-Host "Logged In..." -ForegroundColor Green
    $policies = GetEnterprisePoliciesInSubscription $subscriptionId "NetworkInjection"
    $policies | Select-Object -Property ResourceId, Location, Name

}
GetSubnetInjectionEnterprisePoliciesInSubscription