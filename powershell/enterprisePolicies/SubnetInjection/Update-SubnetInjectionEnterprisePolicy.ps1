<#
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS. MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
NO TECHNICAL SUPPORT IS PROVIDED. YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
#>

param(
    [Parameter(Mandatory, HelpMessage="The Policy subscription")]
    [ValidateNotNullOrEmpty()]
    [string]$SubscriptionId,

    [Parameter(Mandatory, HelpMessage="The Policy resource group")]
    [ValidateNotNullOrEmpty()]
    [string]$ResourceGroup,

    [Parameter(Mandatory, HelpMessage="The Policy name")]
    [ValidateNotNullOrEmpty()]
    [string]$EnterprisePolicyName,

    [Parameter(Mandatory, HelpMessage="The id of the virtual network that should be updated", ParameterSetName="UpdateVnet")]
    [ValidateNotNullOrEmpty()]
    [string]$ExistingVnetIdToUpdate,

    [Parameter(Mandatory, HelpMessage="The virtual network Id", ParameterSetName="UpdateVnet")]
    [Parameter(Mandatory, HelpMessage="The virtual network Id", ParameterSetName="AddMissingVnet")]
    [ValidateNotNullOrEmpty()]
    [string]$VnetId,

    [Parameter(Mandatory, HelpMessage="The subnet name", ParameterSetName="UpdateVnet")]
    [Parameter(Mandatory, HelpMessage="The subnet name", ParameterSetName="AddMissingVnet")]
    [ValidateNotNullOrEmpty()]
    [string]$SubnetName,

    [Parameter(ParameterSetName="AddMissingVnet")]
    [switch]$AddMissingVnet
)

$ErrorActionPreference = "Stop"

Import-Module "$PSScriptRoot\..\Common\EnterprisePolicies" -Force

if (-not(Connect-Azure))
{
    return
}

$policyArmId = "/subscriptions/$SubscriptionId/resourceGroups/$ResourceGroup/providers/Microsoft.PowerPlatform/enterprisePolicies/$EnterprisePolicyName"
$policy = Get-EnterprisePolicy -PolicyArmId $policyArmId
if ($null -eq $policy)
{
    Write-Error "Enterprise Policy $policyArmId not found"
}

$Vnet = Get-Vnet -VnetId $VnetId -EnterprisePolicyLocation $policy.Location
if ($null -eq $Vnet)
{
    Write-Error "There was an issue retrieving or validating the Vnet."
}

if($AddMissingVnet)
{
    if($policy.properties.networkInjection.virtualNetworks.Count -ge (Get-SupportedVnetRegionsForPowerPlatformRegion).Count)
    {
        Write-Error "Unable to do add additional vnet as the limit has been reached. If you want to update an existing Vnet don't include the -AddMissingVnet switch."
    }

    Write-Host "Adding new vnet with Id [$VnetId] and subnet with name [$SubnetName]"
}
else
{
    if (-not($policy.properties.networkInjection.virtualNetworks | Where-Object { $_.id -eq $ExistingVnetIdToUpdate} ))
    {
        Write-Error "There is no vnet with id [$ExistingVnetIdToUpdate] in the enterprise policy $EnterprisePolicyName. If you want to add an additional Vnet use the -AddMissingVnet switch."
    }

    $policy.properties.networkInjection.virtualNetworks = $policy.properties.networkInjection.virtualNetworks | Where-Object { $_.id -ne $ExistingVnetIdToUpdate}

    if($ExistingVnetIdToUpdate -eq $VnetId)
    {
        Write-Host "Updating the subnet name [$SubnetName] for vnet with id [$VnetId]"
    }
    else
    {
        Write-Host "Replacing vnet [$ExistingVnetIdToUpdate] with new vnet with Id [$VnetId] and subnet with name [$SubnetName]"
    }
}

$policy.properties.networkInjection.virtualNetworks += @{
    "id" = $VnetId
    "subnet" = @{
        "name" = $SubnetName
    }
}

$updatedPolicy = Update-EnterprisePolicy -Policy $policy
if ($null -eq $updatedPolicy.ResourceId)
{
    Write-Host "Enterprise Policy not updated"
    return
}
$policyString = $updatedPolicy | ConvertTo-Json -Depth 7
Write-Host "Enterprise Policy updated"
Write-Host $policyString