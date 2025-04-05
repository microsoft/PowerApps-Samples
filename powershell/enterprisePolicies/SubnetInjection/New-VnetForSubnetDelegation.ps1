<#
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS. MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
NO TECHNICAL SUPPORT IS PROVIDED. YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
#>

param(
    [Parameter(Mandatory, HelpMessage="The subscription where the subnet is located")]
    [string]$SubscriptionId,

    [Parameter(Mandatory, HelpMessage="The name of the virtual network the subnet belongs to")]
    [ValidateNotNullOrEmpty()]
    [String]$VirtualNetworkName,

    [Parameter(Mandatory, HelpMessage="The name of the subnet to delegate")]
    [ValidateNotNullOrEmpty()]
    [String]$SubnetName
)

$ErrorActionPreference = "Stop"

Import-Module "$PSScriptRoot\..\Common\EnterprisePolicies" -Force

if (-not(Connect-Azure))
{
    return
}

Set-AzContext -Subscription $SubscriptionID | Out-Null

Write-Host "Getting virtual network $VirtualNetworkName" -ForegroundColor Green
$virtualNetwork = Get-AzVirtualNetwork -Name $VirtualNetworkName
if ($null -eq $virtualNetwork.Name)
{
     Write-Error "Virtual network not retrieved"
}
Write-Host "Virtual network retrieved" -ForegroundColor Green

Write-Host "Getting virtual network subnet $SubnetName" -ForegroundColor Green
$subnet = Get-AzVirtualNetworkSubnetConfig -Name $SubnetName -VirtualNetwork $virtualNetwork
if ($null -eq $subnet.Name)
{
     Write-Error "Virtual network subnet not retrieved"
}
Write-Host "Virtual network subnet retrieved" -ForegroundColor Green

Write-Host "Adding delegation for Microsoft.PowerPlatform/enterprisePolicies to subnet $subnet.Name in vnet $VirtualNetworkName" -ForegroundColor Green
$subnet = Add-AzDelegation -Name "Microsoft.PowerPlatform/enterprisePolicies" -ServiceName "Microsoft.PowerPlatform/enterprisePolicies" -Subnet $subnet
Set-AzVirtualNetwork -VirtualNetwork $virtualNetwork

Write-Host "Added delegation for Microsoft.PowerPlatform/enterprisePolicies to subnet $subnet in vnet $VirtualNetworkName" -ForegroundColor Green