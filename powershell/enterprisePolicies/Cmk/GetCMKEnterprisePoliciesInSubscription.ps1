Import-Module "$PSScriptRoot\..\Common\EnterprisePolicies" -Force

function GetCMKEnterprisePoliciesInSubscription
{
     param(
        [Parameter(
            Mandatory=$true,
            HelpMessage="The subscriptionId"
        )]
        [string]$subscriptionId
    )

    if (-not(Connect-Azure))
    {
        return
    }

    $cmkPolicies = GetEnterprisePoliciesInSubscription $subscriptionId "Encryption"
    $cmkPolicies | Select-Object -Property ResourceId, Location, Name

}
GetCMKEnterprisePoliciesInSubscription