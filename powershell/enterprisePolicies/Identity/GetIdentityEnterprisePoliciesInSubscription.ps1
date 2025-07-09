# Load the environment script
. "$PSScriptRoot\..\Common\EnterprisePolicyOperations.ps1"

function GetIdentityEnterprisePoliciesInSubscription
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
    $policies = GetEnterprisePoliciesInSubscription $subscriptionId "Identity"
    $policies | Select-Object -Property ResourceId, Location, Name

}
GetIdentityEnterprisePoliciesInSubscription