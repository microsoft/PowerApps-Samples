<#
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS. MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
NO TECHNICAL SUPPORT IS PROVIDED. YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
#>

function Get-EnterprisePolicySystemId {
    param (
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string] $PolicyArmId
    )

    $policy = Get-AzResource -ResourceId $PolicyArmId -ExpandProperties
    if ($null -eq $policy.ResourceId -or  $null -eq $policy.Properties)
    {
        Write-Error "Error getting Enterprise Policy for policyId $PolicyArmId `n"
        return $null
    }

    return $policy.Properties.systemId
}

function Set-EnterprisePolicy {
    param (
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string] $ResourceGroup,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $Body
    )

    $tmp = New-TemporaryFile
    $Body | ConvertTo-Json -Depth 7 | Out-File $tmp.FullName
    $policy = New-AzResourceGroupDeployment -DeploymentName "EPDeployment" -ResourceGroupName $ResourceGroup -TemplateFile $tmp.FullName

    Remove-Item $tmp.FullName
    if ($policy.ProvisioningState.Equals("Succeeded"))
    {
        return $true
    }
    $policyString = $policy | ConvertTo-Json
    Write-Error "Error creating/updating Enterprise policy $policyString `n"
    return $false
}

function Get-EnterprisePolicy {
    param (
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string] $PolicyArmId
    )

    $policy = Get-AZResource -ResourceId $PolicyArmId -ExpandProperties
    return $policy
}

function Get-EnterprisePoliciesInSubscription {
    param (
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string] $SubscriptionId,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [PolicyType] $PolicyType
    )

    Set-AzContext -Subscription $SubscriptionId | Out-Null
    $allPolicies = Get-AzResource -ResourceType "Microsoft.Powerplatform/enterprisePolicies"
    $requiredPolicies = @()
    foreach ($policy in $allPolicies)
    {
        if ($policy.kind -eq $PolicyType)
        {
           $requiredPolicies += $policy 
        }
    }
    return $requiredPolicies
}

function Get-EnterprisePoliciesInResourceGroup {
    param (
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string] $SubscriptionId,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string] $ResourceGroup,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [PolicyType] $PolicyType
    )

    Set-AzContext -Subscription $SubscriptionId | Out-Null
    $allPolicies = Get-AzResource -ResourceType "Microsoft.Powerplatform/enterprisePolicies" -ResourceGroupName $ResourceGroup
    $requiredPolicies = @()
    foreach ($policy in $allPolicies)
    {
        if ($policy.kind -eq $PolicyType)
        {
           $requiredPolicies += $policy 
        }
    }
    return $requiredPolicies

}

function Update-EnterprisePolicy {
    param (
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        $Policy
    )

    return $Policy | Set-AzResource -Force
}

function Remove-EnterprisePolicy {
    param (
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string] $PolicyArmId
    )

    return Remove-AzResource -ResourceId $PolicyArmId -Force
}

function New-EnterprisePolicyBody {
    param (
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [PolicyType] $PolicyType,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string] $PolicyLocation,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string] $PolicyName,
        [Parameter()]
        [ValidateNotNullOrEmpty()]
        [string] $KeyVaultId,
        [Parameter()]
        [ValidateNotNullOrEmpty()]
        [string] $KeyName,
        [Parameter()]
        [ValidateNotNullOrEmpty()]
        [string] $KeyVersion,
        [Parameter()]
        [ValidateNotNullOrEmpty()]
        [VnetInformation[]] $VnetInformation
    )

    switch($PolicyType){
        [PolicyType]::Encryption{
            $body = @{
                "`$schema" = "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"
                "contentVersion" = "1.0.0.0"
                "parameters"= @{}
                "resources" = @(
                    @{
                        "type" = "Microsoft.PowerPlatform/enterprisePolicies"
                        "apiVersion" = "2020-10-30"
                        "name" = $PolicyName
                        "location"= $PolicyLocation
                        "kind" = "Encryption"
                    
                        "identity" = @{
                            "type"= "SystemAssigned"
                        }
                    
                        "properties" = @{
                            "encryption" = @{
                                "state" = "Enabled"
                                "keyVault" = @{
                                    "id" = $KeyVaultId
                                    "key" = @{
                                        "name" = $KeyName
                                        "version" =  $KeyVersion
                                    }
                                }
                            }
                            "networkInjection" = $null
                        }
                    }
                )
            }
        }
        [PolicyType]::NetworkInjection{
            $body = @{
                "`$schema" = "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"
                "contentVersion" = "1.0.0.0"
                "parameters"= @{}
                "resources" = @(
                    @{
                        "type" = "Microsoft.PowerPlatform/enterprisePolicies"
                        "apiVersion" = "2020-10-30"
                        "name" = $PolicyName
                        "location"= $PolicyLocation
                        "kind" = "NetworkInjection"
                                   
                        "properties" = @{
                            "networkInjection" = @{
                                "virtualNetworks" = @()
                            }
                        }
                    }
                )
            }
    
            foreach($vnet in $VnetInformation)
            {
                $body.resources[0].properties.networkInjection.virtualNetworks += @{
                    "id" = $vnet.VnetId
                    "subnet" = @{
                        "name" = $vnet.SubnetName
                    }
                }
            }
        }
        Default { throw "The provided policy type is unsupported $PolicyType" }
    }
    return $body
}