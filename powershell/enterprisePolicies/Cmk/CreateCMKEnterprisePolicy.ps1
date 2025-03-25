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

    [Parameter(Mandatory, HelpMessage="The Policy location")]
    [string]$enterprisePolicyLocation,

    [Parameter(Mandatory, HelpMessage="The KeyVault ARM Id")]
    [string]$keyVaultId,

    [Parameter(Mandatory, HelpMessage="The Key name")]
    [string]$keyName,

    [Parameter(Mandatory, HelpMessage="The Key version")]
    [string]$keyVersion
)

$ErrorActionPreference = "Stop"

Import-Module "$PSScriptRoot\..\Common\EnterprisePolicies" -Force

if (-not(Connect-Azure))
{
    return
}

if ($keyVersion -eq "N/A")
{
    $keyVersion = $null
}

$body = New-EnterprisePolicyBody -PolicyType [PolicyType]::Encryption -PolicyLocation $enterprisePolicyLocation -PolicyName $enterprisePolicyName -KeyVaultId $keyVaultId -KeyName $keyName -KeyVersion $keyVersion

$result = Set-EnterprisePolicy -ResourceGroup $resourceGroup -Body $body
if ($result -eq $false)
{
   return
}
Write-Host "CMK Enterprise policy created" -ForegroundColor Green 

$policyArmId = "/subscriptions/$subscriptionId/resourceGroups/$resourceGroup/providers/Microsoft.PowerPlatform/enterprisePolicies/$enterprisePolicyName"
$policy = Get-EnterprisePolicy -PolicyArmId $policyArmId
$policyString = $policy | ConvertTo-Json -Depth 7
Write-Host "Policy created"
Write-Host $policyString