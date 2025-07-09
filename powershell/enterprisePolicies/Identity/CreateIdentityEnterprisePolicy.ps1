# Load the environment script
. "$PSScriptRoot\..\Common\EnterprisePolicyOperations.ps1"

function CreateIdentityEnterprisePolicy
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
        [string]$enterprisePolicylocation

    )

    Write-Host "Logging In..." -ForegroundColor Green
    $connect = AzureLogin
    if ($false -eq $connect)
    {
        Write-Host "Error Logging In..." -ForegroundColor Red
        return
    }

    Write-Host "Logged In..." -ForegroundColor Green
    Write-Host "Creating Enterprise policy..." -ForegroundColor Green

    $body = GenerateEnterprisePolicyBody -policyType "identity" -policyLocation $enterprisePolicyLocation -policyName $enterprisePolicyName

    $result = PutEnterprisePolicy $resourceGroup $body
    if ($result -eq $false)
    {
       Write-Host "Identity Enterprise policy not created" -ForegroundColor Red
       return
    }
    Write-Host "Identity Enterprise policy created" -ForegroundColor Green 

    $policyArmId = "/subscriptions/$subscriptionId/resourceGroups/$resourceGroup/providers/Microsoft.PowerPlatform/enterprisePolicies/$enterprisePolicyName"
    $policy = GetEnterprisePolicy $policyArmId
    $policyString = $policy | ConvertTo-Json -Depth 7
    Write-Host "Policy created"
    Write-Host $policyString

}
CreateIdentityEnterprisePolicy