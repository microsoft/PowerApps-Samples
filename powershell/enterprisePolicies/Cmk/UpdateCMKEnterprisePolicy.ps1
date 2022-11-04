# Load the environment script
. "$PSScriptRoot\..\Common\EnterprisePolicyOperations.ps1"

function UpdateCMKEnterprisePolicy
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
            HelpMessage="The updated KeyVault ARM Id, enter N/A if no update is required for this field"
        )]
        [string]$keyVaultId,

        [Parameter(
            Mandatory=$true,
            HelpMessage="The Key name, enter N/A if no update is required for this field"
        )]
        [string]$keyName,

        [Parameter(
            Mandatory=$true,
            HelpMessage="The Key version, enter N/A if no update is required for this field"
        )]
        [string]$keyVersion     

    )

    if ($keyVaultId -eq "N/A" -and $keyName -eq "N/A" -and $keyVersion -eq "N/A")
    {
        Write-Host "No change given as input..." -ForegroundColor Green
        return
    }

    Write-Host "Logging In..." -ForegroundColor Green
    $connect = AzureLogin
    if ($false -eq $connect)
    {
        return
    }

    Write-Host "Logged In..." -ForegroundColor Green

    $policyArmId = "/subscriptions/$subscriptionId/resourceGroups/$resourceGroup/providers/Microsoft.PowerPlatform/enterprisePolicies/$enterprisePolicyName"
    $policy = GetEnterprisePolicy $policyArmId
    if ($keyVaultId -ne "N/A")
    {
        Write-Host "Updating KeyVaultId as $keyVaultId" -ForegroundColor Green
        $policy.properties.encryption.keyVault.id = $keyVaultId
    }
    if ($keyName -ne "N/A")
    {
        Write-Host "Updating keyName as $keyName" -ForegroundColor Green
        $policy.properties.encryption.keyVault.key.name = $keyName
    }
    if ($keyVersion -ne "N/A")
    {
        Write-Host "Updating keyVersion as $keyVersion" -ForegroundColor Green
        $policy.properties.encryption.keyVault.key.version = $keyVersion
    }

    $updatedPolicy = UpdateEnterprisePolicy $policy
    if ($updatedPolicy.ResourceId -eq $null)
    {
         Write-Host "Policy not updated"
         return
    }
    $policyString = $updatedPolicy | ConvertTo-Json -Depth 7
    Write-Host "Policy updated"
    Write-Host $policyString

}
UpdateCMKEnterprisePolicy