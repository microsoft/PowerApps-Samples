# Load thescript
. "$PSScriptRoot\..\Common\EnterprisePolicyOperations.ps1"

function SetupVnetForSubnetDelegation
{
    param(
         [Parameter(
            Mandatory=$true,
            HelpMessage="The Policy subscription"
        )]
        [string]$virtualNetworkSubscriptionId,

        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]$virtualNetworkName,

        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]$subnetName
    )
    
    Write-Host "Logging In..." -ForegroundColor Green
    AzureLogin
    Write-Host "Logged In" -ForegroundColor Green

    $setSubscription = Set-AzContext -Subscription $virtualNetworkSubscriptionId
    
    Write-Host "Getting virtual network $virtualNetworkName" -ForegroundColor Green
    $virtualNetwork = Get-AzVirtualNetwork -Name $virtualNetworkName
    if ($null -eq  $virtualNetwork.Name)
    {
         Write-Host "Virtual network not reterieved" -ForegroundColor Red
         return
    }
    Write-Host "Virtual network reterieved" -ForegroundColor Green

    Write-Host "Getting virtual network subnet $subnetName" -ForegroundColor Green
    $subnet = Get-AzVirtualNetworkSubnetConfig -Name $subnetName -VirtualNetwork $virtualNetwork
    if ($null -eq  $subnet.Name)
    {
         Write-Host "Virtual network subnet not reterieved" -ForegroundColor Red
         return
    }
    Write-Host "Virtual network subnet reterieved" -ForegroundColor Green

    Write-Host "Adding delegation for Microsoft.PowerPlatform/enterprisePolicies to subnet $subnet.Name in vnet $virtualNetworkName" -ForegroundColor Green
    $subnet = Add-AzDelegation -Name "Microsoft.PowerPlatform/enterprisePolicies" -ServiceName "Microsoft.PowerPlatform/enterprisePolicies" -Subnet $subnet
    Set-AzVirtualNetwork -VirtualNetwork $virtualNetwork

    Write-Host "Added delegation for Microsoft.PowerPlatform/enterprisePolicies to subnet $subnet in vnet $virtualNetworkName" -ForegroundColor Green
    
        
}
SetupVnetForSubnetDelegation