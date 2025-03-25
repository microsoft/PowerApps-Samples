<#
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS. MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
NO TECHNICAL SUPPORT IS PROVIDED. YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
#>

param(
    [Parameter(Mandatory, HelpMessage="The Policy subscription")]
    [string]$SubscriptionId,

    [Parameter(Mandatory,HelpMessage="The Policy resource group")]
    [string]$ResourceGroup,

    [Parameter(Mandatory, HelpMessage="The Policy name")]
    [string]$EnterprisePolicyName,

    [Parameter(Mandatory, HelpMessage="The Policy location")]
    [string]$EnterprisePolicyLocation,

    [Parameter(Mandatory, HelpMessage="Virtual network Id 1")]
    [string]$VnetId1,

    [Parameter(Mandatory, HelpMessage="Subnet name 1")]
    [string]$SubnetName1,

    [Parameter(Mandatory=$false, HelpMessage="Virtual network Id 2")]
    [string]$VnetId2,

    [Parameter(Mandatory=$false, HelpMessage="Subnet name 2")]
    [string]$SubnetName2
)

$ErrorActionPreference = "Stop"

Import-Module "$PSScriptRoot\..\Common\EnterprisePolicies" -Force

if (-not(Connect-Azure))
{
    return
}

Write-Host "Creating Enterprise policy..." -ForegroundColor Green

[VnetInformation[]] $vnetInformation

$Vnet1 = Get-Vnet -VnetId $VnetId1 -EnterprisePolicyLocation $EnterprisePolicyLocation
if ($null -eq $Vnet1)
{
    Write-Error "There was an issue retrieving or validating the Vnet."
}

$vnetInformation += [VnetInformation] @{
    VnetId = $VnetId1
    SubnetName = $SubnetName1
}

if((Get-SupportedVnetRegionsForPowerPlatformRegion -PowerPlatformRegion $EnterprisePolicyLocation).Count -eq 2)
{
    if([string]::IsNullOrWhiteSpace($VnetId2) -and [string]::IsNullOrWhiteSpace($SubnetNam2))
    {
        throw "The region [$EnterprisePolicyLocation] requires that information for 2 subnets be provided."
    }

    $Vnet2 = Get-Vnet -VnetId $VnetId2 -EnterprisePolicyLocation $EnterprisePolicyLocation
    if ($null -eq $Vnet2)
    {
        Write-Error "There was an issue retrieving or validating the Vnet."
    }

    $vnetInformation += [VnetInformation] @{
        VnetId = $VnetId2
        SubnetName = $SubnetName2
    }
}

$body = New-EnterprisePolicyBody -PolicyType [PolicyType]::NetworkInjection -PolicyLocation $EnterprisePolicyLocation -PolicyName $EnterprisePolicyName -VnetInformation $vnetInformation

$result = Set-EnterprisePolicy -ResourceGroup $ResourceGroup -Body $body
if (-not($result))
{
    Write-Error "Subnet Injection Enterprise policy not created"
}
Write-Host "Subnet Injection Enterprise policy created" -ForegroundColor Green 

$policyArmId = "/subscriptions/$subscriptionId/resourceGroups/$resourceGroup/providers/Microsoft.PowerPlatform/enterprisePolicies/$EnterprisePolicyName"
$policy = Get-EnterprisePolicy -PolicyArmId $policyArmId
$policyString = $policy | ConvertTo-Json -Depth 7
Write-Host "Policy created"
Write-Host $policyString