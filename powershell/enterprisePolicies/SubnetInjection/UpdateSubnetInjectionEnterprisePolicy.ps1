# Load the environment script
. "$PSScriptRoot\..\Common\EnterprisePolicyOperations.ps1"
. "$PSScriptRoot\ValidateVnetLocationForEnterprisePolicy.ps1"


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
            HelpMessage="Primary virtual network Id, enter N/A if no update is required for this field"
        )]
        [string]$primaryVnetId,

        [Parameter(
            Mandatory=$true,
            HelpMessage="Primary subnet name, enter N/A if no update is required for this field"
        )]
        [string]$primarySubnetName,

        [Parameter(
            Mandatory=$true,
            HelpMessage="Secondary virtual network Id, enter N/A if no update is required for this field"
        )]
        [string]$secondaryVnetId,

        [Parameter(
            Mandatory=$true,
            HelpMessage="Secondary subnet name, enter N/A if no update is required for this field"
        )]
        [string]$secondarySubnetName  
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
    if ($policy -eq $null)
    {
        Write-Host "Enterprise Policy $policyArmId not found" -ForegroundColor Red
        return
    }
    if ($primaryVnetId -ne "N/A")
    {
        Write-Host "Updating primaryVnetId as $primaryVnetId" -ForegroundColor Green

        $primaryVnet = ValidateAndGetVnet -vnetId $primaryVnetId -enterprisePolicylocation $policy.Location
        if ($primaryVnet -eq $null)
        {
           Write-Host "Enterprise Policy not updated" -ForegroundColor Red
           return
        }
        $policy.properties.networkInjection.virtualNetworks[0].id = $primaryVnetId
    }
    if ($primarySubnetName -ne "N/A")
    {
        Write-Host "Updating primarySubnetName as $primarySubnetName" -ForegroundColor Green
        $policy.properties.networkInjection.virtualNetworks[0].subnet.name = $primarySubnetName
    }

    if ($secondaryVnetId -ne "N/A")
    {
        Write-Host "Updating secondaryVnetId as $secondaryVnetId" -ForegroundColor Green

        $secondaryVnet = ValidateAndGetVnet -vnetId $secondaryVnetId -enterprisePolicylocation $policy.Location
        if ($secondaryVnet -eq $null)
        {
           Write-Host "Enterprise Policy not updated" -ForegroundColor Red
           return
        }
        if ($policy.properties.networkInjection.virtualNetworks.length -lt 2)
        {
            Write-Host "There is no secondary vnet in enterprise policy $enterprisePolicyName. Adding a new secondaryVnet $secondaryVnetId" -ForegroundColor Green
            if ($secondarySubnetName -eq "N/A")
            {
                Write-Host "As there is no secondary vnet in enterprise policy $enterprisePolicyName, please provide a value for secondarySubnetName. Currently provided value is $secondarySubnetName" -ForegroundColor Red
                return
            }
            $policy.properties.networkInjection.virtualNetworks +=  @{
                                    "id" = $secondaryVnetId
                                    "subnet" = @{
                                        "name" = $secondarySubnetName
                                    }
                                }
        }
        else
        {
            $policy.properties.networkInjection.virtualNetworks[1].id = $secondaryVnetId          
        }
    }
    if ($secondarySubnetName -ne "N/A")
    {
        Write-Host "Updating secondarySubnetName as $secondarySubnetName" -ForegroundColor Green
        $policy.properties.networkInjection.virtualNetworks[1].subnet.name = $secondarySubnetName
    }
   
    $updatedPolicy = UpdateEnterprisePolicy $policy
    if ($updatedPolicy.ResourceId -eq $null)
    {
         Write-Host "Enterprise Policy not updated"
         return
    }
    $policyString = $updatedPolicy | ConvertTo-Json -Depth 7
    Write-Host "Enterprise Policy updated"
    Write-Host $policyString   
}
UpdateSubnetInjectionEnterprisePolicy