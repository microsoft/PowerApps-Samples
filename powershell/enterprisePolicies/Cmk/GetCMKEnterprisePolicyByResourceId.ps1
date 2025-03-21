Import-Module "$PSScriptRoot\..\Common\EnterprisePolicies" -Force

function GetCMKEnterprisePolicyByResourceId
{
     param(
        [Parameter(
            Mandatory=$true,
            HelpMessage="The Policy Id"
        )]
        [string]$enterprisePolicyArmId
    )

    if (-not(Connect-Azure))
    {
        return
    }

    $policy = GetEnterprisePolicy $enterprisePolicyArmId
    $policyString = $policy | ConvertTo-Json -Depth 7
    Write-Host $policyString

}
GetCMKEnterprisePolicyByResourceId