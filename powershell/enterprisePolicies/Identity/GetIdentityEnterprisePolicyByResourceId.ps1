# Load the environment script
. "$PSScriptRoot\..\Common\EnterprisePolicyOperations.ps1"

function GetIdentityEnterprisePolicyByResourceId
{
     param(
        [Parameter(
            Mandatory=$true,
            HelpMessage="The Policy Id"
        )]
        [string]$enterprisePolicyArmId
    )

    Write-Host "Logging In..." -ForegroundColor Green
    $connect = AzureLogin
    if ($false -eq $connect)
    {
        return
    }

    Write-Host "Logged In..." -ForegroundColor Green

    $policy = GetEnterprisePolicy $enterprisePolicyArmId
    $policyString = $policy | ConvertTo-Json -Depth 7
    Write-Host $policyString

}
GetIdentityEnterprisePolicyByResourceId