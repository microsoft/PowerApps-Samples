<#
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS. MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
NO TECHNICAL SUPPORT IS PROVIDED. YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
#>

$SupportedVnetLocations = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
$SupportedVnetLocations.Add("centraluseuap", "eastus|westus")
$SupportedVnetLocations.Add("eastus2euap", "eastus|westus")
$SupportedVnetLocations.Add("unitedstateseuap", "eastus|westus")
$SupportedVnetLocations.Add("unitedstates", "eastus|westus")
$SupportedVnetLocations.Add("southafrica", "southafricanorth|southafricawest")
$SupportedVnetLocations.Add("uk", "uksouth|ukwest")
$SupportedVnetLocations.Add("japan", "japaneast|japanwest")
$SupportedVnetLocations.Add("india", "centralindia|southindia")
$SupportedVnetLocations.Add("france", "francecentral|francesouth")
$SupportedVnetLocations.Add("europe", "westeurope|northeurope")
$SupportedVnetLocations.Add("germany", "germanynorth|germanywestcentral")
$SupportedVnetLocations.Add("switzerland", "switzerlandnorth|switzerlandwest")
$SupportedVnetLocations.Add("canada", "canadacentral|canadaeast")
$SupportedVnetLocations.Add("brazil", "brazilsouth|southcentralus")
$SupportedVnetLocations.Add("australia", "australiasoutheast|australiaeast")
$SupportedVnetLocations.Add("asia", "eastasia|southeastasia")
$SupportedVnetLocations.Add("uae", "uaecentral|uaenorth")
$SupportedVnetLocations.Add("korea", "koreasouth|koreacentral")
$SupportedVnetLocations.Add("norway", "norwaywest|norwayeast")
$SupportedVnetLocations.Add("singapore", "southeastasia")
$SupportedVnetLocations.Add("sweden", "swedencentral")

function Assert-AzureRegionIsSupported
{
    param (
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string] $PowerPlatformRegion,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string] $AzureRegion
    )

    $vnetLocationsAllowed = $SupportedVnetLocations[$PowerPlatformRegion].Split("|")
    if (-not($vnetLocationsAllowed.Contains($AzureRegion)))
    {
        Write-Error "The location $AzureRegion is not supported for enterprise policy location $PowerPlatformRegion`n"
        $vnetLocationsAllowedString = $vnetLocationsAllowed -join ","
        Write-Error "The supported vnet location for enterprise policy location $PowerPlatformRegion are $vnetLocationsAllowedString`n"
        return $null
    }
}
function Assert-PowerPlatformRegionIsSupported
{
    param (
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string] $PowerPlatformRegion
    )

    if(-not($SupportedVnetLocations.ContainsKey($PowerPlatformRegion)))
    {
        throw "The PowerPlatform region [$PowerPlatformRegion] is not supported. The supported enterprise policy locations are $($SupportedVnetLocations.Keys -join ",")`n"
    }
}

function Get-SupportedVnetRegionsForPowerPlatformRegion
{
    param (
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string] $PowerPlatformRegion
    )

    Assert-PowerPlatformRegionIsSupported -PowerPlatformRegion $PowerPlatformRegion
    return $SupportedVnetLocations[$PowerPlatformRegion].Split("|")
}

function Get-Vnet{
    param(
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string] $VnetId,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string] $EnterprisePolicyLocation
    )

    $vnetResource = Get-AzResource -ResourceId $vnetId
    if ($null -eq $vnetResource.ResourceId)
    {
        Write-Error "Error getting virtual network for $vnetId `n"
        return $null
    }

    Assert-PowerPlatformRegionIsSupported -PowerPlatformRegion $EnterprisePolicyLocation

    Assert-AzureRegionIsSupported -PowerPlatformRegion $EnterprisePolicyLocation -AzureRegion $vnetResource.Location

    return $vnetResource
}