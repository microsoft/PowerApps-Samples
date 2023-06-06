# Load the environment script
. "$PSScriptRoot\..\Common\EnterprisePolicyOperations.ps1"

function CreateCMKEnterprisePolicy
{
     param(
        [Parameter(
            Mandatory=$true,
            HelpMessage="The Policy subscription"
        )]
        [string]$subscriptionId,

        [Parameter(
            Mandatory=$true,
            HelpMessage="The Policy resource group"
        )]
        [string]$resourceGroup,

        [Parameter(
            Mandatory=$true,
            HelpMessage="The Policy name"
        )]
        [string]$enterprisePolicyName,

        [Parameter(
            Mandatory=$true,
            HelpMessage="The Policy location"
        )]
        [string]$enterprisePolicyLocation,

        [Parameter(
            Mandatory=$true,
            HelpMessage="The KeyVault ARM Id"
        )]
        [string]$keyVaultId,

        [Parameter(
            Mandatory=$true,
            HelpMessage="The Key name"
        )]
        [string]$keyName,

        [Parameter(
            Mandatory=$true,
            HelpMessage="The Key version"
        )]
        [string]$keyVersion     

    )

    Write-Host "Logging In..." -ForegroundColor Green
    $connect = AzureLogin
    if ($false -eq $connect)
    {
        return
    }

    Write-Host "Logged In..." -ForegroundColor Green

    if ($keyVersion -eq "N/A")
    {
        $keyVersion = $null
    }
	
    $body = GenerateEnterprisePolicyBody -policyType "cmk" -policyLocation $enterprisePolicyLocation -policyName $enterprisePolicyName -keyVaultId $keyVaultId -keyName $keyName -keyVersion $keyVersion

    $result = PutEnterprisePolicy $resourceGroup $body
    if ($result -eq $false)
    {
       return
    }
    Write-Host "CMK Enterprise policy created" -ForegroundColor Green 

    $policyArmId = "/subscriptions/$subscriptionId/resourceGroups/$resourceGroup/providers/Microsoft.PowerPlatform/enterprisePolicies/$enterprisePolicyName"
    $policy = GetEnterprisePolicy $policyArmId
    $policyString = $policy | ConvertTo-Json -Depth 7
    Write-Host "Policy created"
    Write-Host $policyString

}
CreateCMKEnterprisePolicy