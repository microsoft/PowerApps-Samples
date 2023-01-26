# Load the environment script
. "$PSScriptRoot\..\Common\EnterprisePolicyOperations.ps1"

function UpdateSubnetInjectionEnterprisePolicy
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
            HelpMessage="The virtual network Id, enter N/A if no update is required for this field"
        )]
        [string]$vnetId,

        [Parameter(
            Mandatory=$true,
            HelpMessage="The subnet name, enter N/A if no update is required for this field"
        )]
        [string]$subnetName  

    )

    if ($vnetId -eq "N/A" -and $subnetName -eq "N/A")
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
    if ($vnetId -ne "N/A")
    {
        Write-Host "Updating vnetId as $vnetId" -ForegroundColor Green
        $policy.properties.networkInjection.virtualNetworks[0].id = $vnetId
    }
    if ($subnetName -ne "N/A")
    {
        Write-Host "Updating subnetName as $subnetName" -ForegroundColor Green
        $policy.properties.networkInjection.virtualNetworks[0].subnet.name = $subnetName
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
UpdateSubnetInjectionEnterprisePolicy