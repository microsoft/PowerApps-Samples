Import-Module "$PSScriptRoot\..\Common\EnterprisePolicies" -Force

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

    if (-not(Connect-Azure))
    {
        return
    }

    $cmkPolicies = GetEnterprisePoliciesInResourceGroup $subscriptionId "Encryption" $resourceGroup
    $cmkPolicies | Select-Object -Property ResourceId, Location, Name

}
GetCMKEnterprisePoliciesInResourceGroup