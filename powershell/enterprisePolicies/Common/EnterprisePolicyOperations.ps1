function AzureLogin {
    param(
        [Parameter(Mandatory=$false)]
        [ValidateSet("tip1", "tip2", "prod", "usgovhigh", "dod", "china")]
        [String]$endpoint
    )    

    $environment = "AzureCloud"
    if (($endpoint -eq "usgovhigh") -or ($endpoint -eq "dod")) {
        $environment = "AzureUSGovernment"
    }
    elseif ($endpoint -eq "china") {
        $environment = "AzureChinaCloud"
    }
    $connect = Connect-AzAccount -Environment $environment

    if ($null -eq $connect)
    {
        Write-Host "Error connecting to Azure Account `n" -ForegroundColor Red
        return $false
    }

    return $true
}

function GetEnterprisePolicySystemId($policyArmId) {

    $policy = Get-AzResource -ResourceId $policyArmId -ExpandProperties
    if ($policy.ResourceId -eq $null -or  $policy.Properties -eq $null)
    {
        Write-Host "Error getting Enterprise Policy for policyId $policyArmId `n" -ForegroundColor Red
        return $null
    }

    return $policy.Properties.systemId

}

function PutEnterprisePolicy($resourceGroup, $body)
 {

    $tmp = New-TemporaryFile
    $body | ConvertTo-Json -Depth 7 | Out-File $tmp.FullName
    $policy = New-AzResourceGroupDeployment -DeploymentName "EPDeployment" -ResourceGroupName $resourceGroup -TemplateFile $tmp.FullName

    Remove-Item $tmp.FullName
    if ($policy.ProvisioningState.Equals("Succeeded"))
    {
        return $true
    }
    $policyString = $policy | ConvertTo-Json
    Write-Host "Error creating/updating Enterprise policy $policyString `n" -ForegroundColor Red
    return $false


}

function GetEnterprisePolicy($policyArmId)
 {

    $policy = Get-AZResource -ResourceId $policyArmId -ExpandProperties
    return $policy

}

function GetEnterprisePoliciesInSubscription($subscriptionId, $policyType)
{

    $setSubscription = Set-AzContext -Subscription $subscriptionId
    $allPolicies = Get-AZResource -ResourceType Microsoft.Powerplatform/enterprisePolicies
    $requiredPolicies = @()
    foreach ($policy in $allPolicies)
    {
        if ($policy.kind -eq $policyType)
        {
           $requiredPolicies += $policy 
        }
    }
    return $requiredPolicies

}

function GetEnterprisePoliciesInResourceGroup($subscriptionId, $policyType, $resourceGroup)
{

    $setSubscription = Set-AzContext -Subscription $subscriptionId
    $allPolicies = Get-AZResource -ResourceType Microsoft.Powerplatform/enterprisePolicies -ResourceGroupName $resourceGroup
    $requiredPolicies = @()
    foreach ($policy in $allPolicies)
    {
        if ($policy.kind -eq $policyType)
        {
           $requiredPolicies += $policy 
        }
    }
    return $requiredPolicies

}

function UpdateEnterprisePolicy($policy)
{

    return $policy | Set-AzResource -Force

}

function RemoveEnterprisePolicy($policyArmId)
{

    return Remove-AzResource -ResourceId $policyArmId -Force

}

function GenerateEnterprisePolicyBody ($policyType, $policyLocation, $policyName, $keyVaultId, $keyName, $keyVersion, $primaryVnetId, $primarySubnetName, $secondaryVnetId, $secondarySubnetName)
{   
    if ("cmk" -eq $policyType)
    {
        $body = @{
            "`$schema" = "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"
            "contentVersion" = "1.0.0.0"
            "parameters"= @{}
            "resources" = @(
                @{
                    "type" = "Microsoft.PowerPlatform/enterprisePolicies"
                    "apiVersion" = "2020-10-30"
                    "name" = $policyName
                    "location"= $policyLocation
                    "kind" = "Encryption"
                
                    "identity" = @{
                        "type"= "SystemAssigned"
                    }
                
                    "properties" = @{
                        "encryption" = @{
                            "state" = "Enabled"
                            "keyVault" = @{
                                "id" = $keyVaultId
                                "key" = @{
                                    "name" = $keyName
                                    "version" =  $keyVersion
                                }
                            }
                        }
                        "networkInjection" = $null
                    }
                }
            )
            
        }
        
    }
    
    elseif ("vnet" -eq $policyType)
    {
        $virtualNetworks = @(
            @{
                "id" = $primaryVnetId
                "subnet" = @{
                    "name" = $primarySubnetName
                }
            }
        )

        if ($null -ne $secondaryVnetId)
        {
            $virtualNetworks += @{
                "id" = $secondaryVnetId
                "subnet" = @{
                    "name" = $secondarySubnetName
                }
            }
        }

        $body = @{
            "`$schema" = "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"
            "contentVersion" = "1.0.0.0"
            "parameters"= @{}
            "resources" = @(
                @{
                    "type" = "Microsoft.PowerPlatform/enterprisePolicies"
                    "apiVersion" = "2020-10-30"
                    "name" = $policyName
                    "location"= $policyLocation
                    "kind" = "NetworkInjection"
                               
                    "properties" = @{
                        "networkInjection" = @{
                            "virtualNetworks" = $virtualNetworks
                        }
                    }
                }
            )
            
        }
    }

    elseif ("identity" -eq $policyType)
    {
        $body = @{
            "`$schema" = "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"
            "contentVersion" = "1.0.0.0"
            "parameters"= @{}
            "resources" = @(
                @{
                    "type" = "Microsoft.PowerPlatform/enterprisePolicies"
                    "apiVersion" = "2020-10-30"
                    "name" = $policyName
                    "location"= $policyLocation
                    "kind" = "Identity"
                
                    "identity" = @{
                        "type"= "SystemAssigned"
                    }               
                   
                }
            )
            
        }
    }

   return $body
}
