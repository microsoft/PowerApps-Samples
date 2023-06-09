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
    if ($policy -eq $null)
    {
         Write-Host "CMK Enterprise Policy not found for $policyArmId" -ForegroundColor Red 
         return
    }

    if ($policy.Kind -ne "Encryption")
    {
        $kindString = $policy.Kind | ConvertTo-Json
        Write-Host "Enterprise found for $policyArmId is not CMK Enterprise Policy. Policy is of type $kindString " -ForegroundColor Red 
        return
    }

    if ($policy.Identity -eq $null -or $policy.Identity.Type -ne "SystemAssigned")
    {
        $identityString = $policy.Identity | ConvertTo-Json -Depth 7
        Write-Host "Enterprise found for $policyArmId is not having valid Identity property $identityString" -ForegroundColor Red 
        return
    }
    
    $keyVaultIdUpdated =  $policy.properties.encryption.keyVault.id
    $keyNameUpdated = $policy.properties.encryption.keyVault.key.name
    $keyVersionUpdated = $policy.properties.encryption.keyVault.key.version
    if ($keyVaultId -ne "N/A")
    {
        Write-Host "Updating KeyVaultId as $keyVaultId" -ForegroundColor Green
        $keyVaultIdUpdated = $keyVaultId
    }
    if ($keyName -ne "N/A")
    {
        Write-Host "Updating keyName as $keyName" -ForegroundColor Green
        $keyNameUpdated = $keyName
    }
    if ($keyVersion -ne "N/A")
    {
        Write-Host "Updating keyVersion as $keyVersion" -ForegroundColor Green
        $keyVersionUpdated = $keyVersion
    }

    $body = GenerateEnterprisePolicyBody -policyType "cmk" -policyLocation $policy.Location -policyName $policy.Name -keyVaultId $keyVaultIdUpdated -keyName $keyNameUpdated -keyVersion $keyVersionUpdated
    $body.resources.identity.Add("principalId", $policy.Identity.PrincipalId)
    $body.resources.identity.Add("tenantId", $policy.Identity.TenantId)

    $result = PutEnterprisePolicy $resourceGroup $body
    if ($result -eq $false)
    {
       return
    }

    Write-Host "CMK Enterprise policy updated" -ForegroundColor Green 

    $policy = GetEnterprisePolicy $policyArmId
    $policyString = $policy | ConvertTo-Json -Depth 7
    Write-Host "The updated policy"
    Write-Host $policyString
}
UpdateCMKEnterprisePolicy