# Load the environment script
. "$PSScriptRoot\..\Common\EnterprisePolicyOperations.ps1"

function GetCMKEnterprisePoliciesInSubscription
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
    $cmkPolicies = GetEnterprisePoliciesInSubscription $subscriptionId "Encryption"
    $cmkPolicies | Select-Object -Property ResourceId, Location, Name

}
GetCMKEnterprisePoliciesInSubscription