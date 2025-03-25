<#
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS. MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
NO TECHNICAL SUPPORT IS PROVIDED. YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
#>

param(
    [Parameter(Mandatory, HelpMessage="The Policy subscription")]
    [string]$subscriptionId,

    [Parameter(Mandatory, HelpMessage="The Policy resource group")]
    [string]$resourceGroup,

    [Parameter(Mandatory, HelpMessage="The Policy name")]
    [string]$enterprisePolicyName,

    [Parameter(Mandatory, HelpMessage="The updated KeyVault ARM Id, enter N/A if no update is required for this field")]
    [string]$keyVaultId,

    [Parameter(Mandatory, HelpMessage="The Key name, enter N/A if no update is required for this field")]
    [string]$keyName,

    [Parameter(Mandatory,HelpMessage="The Key version, enter N/A if no update is required for this field")]
    [string]$keyVersion
)

$ErrorActionPreference = "Stop"

Import-Module "$PSScriptRoot\..\Common\EnterprisePolicies" -Force

if ($keyVaultId -eq "N/A" -and $keyName -eq "N/A" -and $keyVersion -eq "N/A")
{
    Write-Host "No change given as input..." -ForegroundColor Green
    return
}

if (-not(Connect-Azure))
{
    return
}

$policyArmId = "/subscriptions/$subscriptionId/resourceGroups/$resourceGroup/providers/Microsoft.PowerPlatform/enterprisePolicies/$enterprisePolicyName"
$policy = Get-EnterprisePolicy -PolicyArmId $policyArmId
if ($null -eq $policy)
{
     Write-Host "CMK Enterprise Policy not found for $policyArmId" -ForegroundColor Red 
     return
}

if ($policy.Kind -ne [PolicyType]::Encryption)
{
    $kindString = $policy.Kind | ConvertTo-Json
    Write-Host "Enterprise found for $policyArmId is not CMK Enterprise Policy. Policy is of type $kindString " -ForegroundColor Red 
    return
}

if ($null -eq $policy.Identity -or $policy.Identity.Type -ne "SystemAssigned")
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

$body = New-EnterprisePolicyBody -PolicyType [PolicyType]::Encryption -PolicyLocation $policy.Location -PolicyName $policy.Name -KeyVaultId $keyVaultIdUpdated -KeyName $keyNameUpdated -KeyVersion $keyVersionUpdated
$body.resources.identity.Add("principalId", $policy.Identity.PrincipalId)
$body.resources.identity.Add("tenantId", $policy.Identity.TenantId)

$result = Set-EnterprisePolicy -ResourceGroup $resourceGroup -Body $body
if ($result -eq $false)
{
   return
}

Write-Host "CMK Enterprise policy updated" -ForegroundColor Green 

$policy = Get-EnterprisePolicy -PolicyArmId $policyArmId
$policyString = $policy | ConvertTo-Json -Depth 7
Write-Host "The updated policy"
Write-Host $policyString