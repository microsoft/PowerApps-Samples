<#
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS. MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
NO TECHNICAL SUPPORT IS PROVIDED. YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
#>

function New-PolicyToEnvLink
{
    param(
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [PolicyType]$PolicyType,

        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [String]$EnvironmentId,

        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [String]$PolicyArmId,

        [Parameter(Mandatory=$false)]
        [BAPEndpoint]$Endpoint = "prod"
    )

    if (-not(Connect-Bap -Endpoint $endpoint))
    {
        return
    }

    #Validate Environment
    $env = Get-Environment -EnvironmentId $environmentId

    if ($null -eq $env) 
    {
        return
    }
    Write-Host "Environment retrieved `n" -ForegroundColor Green

    #Validate Enterprise Policy
    $policySystemId = Get-EnterprisePolicySystemId -PolicyArmId $policyArmId
    if ($null -eq $policySystemId)
    {
        return
    }
    Write-Host "Enterprise Policy retrieved `n" -ForegroundColor Green

    $linkResult = New-EnterprisePolicyLink -Environment $env -PolicyType $policyType -PolicySystemId $policySystemId

    $linkResultString = $linkResult | ConvertTo-Json

    if ($null -eq $linkResult -or $linkResult.StatusCode -ne "202")
    {
        Write-Host "Linking of $policyType policy did not start for environment $environmentId"
        Write-Host "Error: $linkResultString"
        return
    }

    Write-Host "Linking of $policyType policy started for environment $environmentId"
    Invoke-PollOperation -Headers $linkResult.Headers
}

function Remove-PolicyToEnvLink
{
    param(
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [PolicyType]$PolicyType,

        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [String]$EnvironmentId,

        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [String]$PolicyArmId,

        [Parameter(Mandatory=$false)]
        [BAPEndpoint]$Endpoint = "prod"
    )

    if (-not(Connect-Bap -Endpoint $endpoint))
    {
        return
    }

    #Validate Environment
    $env = Get-Environment -EnvironmentId $EnvironmentId

    if ($null -eq $env) 
    {
        return
    }
    Write-Host "Environment retrieved `n" -ForegroundColor Green
    
    if ($null -eq $env.properties.enterprisePolicies -or $null -eq $env.properties.enterprisePolicies.$PolicyType)
    {
        Write-Host "No enterprise policy present to remove for environment $EnvironmentId"
        return
    }

    if (!$PolicyArmId.Equals($env.properties.enterprisePolicies.$PolicyType.id))
    {
        Write-Host "Given policyArmId $PolicyArmId not matching with $PolicyType policy ArmId for environment $EnvironmentId"
        return 
    }

    #Validate Enterprise Policy
    $policySystemId = Get-EnterprisePolicySystemId -PolicyArmId $PolicyArmId
    if ($null -eq $policySystemId)
    {
        return
    }
    Write-Host "Enterprise Policy retrieved `n" -ForegroundColor Green

    $unLinkResult = Remove-EnterprisePolicyLink $env $policyType $policySystemId

    $unLinkResultString = $UnLinkResult | ConvertTo-Json

    if ($null -eq $unLinkResult -or $unLinkResult.StatusCode -ne "202")
    {
        Write-Host "Unlinking of $policyType policy did not start for environment $environmentId"
        Write-Host "Error: $unLinkResultString"
        return 
    }

    Write-Host "Unlinking of $policyType policy started for environment $environmentId"
    Invoke-PollOperation -Headers $unLinkResult.Headers
}

function SwapPolicyForEnv 
{
    param(
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [PolicyType]$PolicyType,

        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [String]$EnvironmentId,

        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [String]$PolicyArmId,

        [Parameter(Mandatory=$false)]
        [BAPEndpoint]$Endpoint = "prod"
    )

    if (-not(Connect-Bap -Endpoint $Endpoint))
    {
        return
    }

    #Validate Environment
    $env = Get-Environment -EnvironmentId $EnvironmentId

    if ($null -eq $env) 
    {
        return
    }
    Write-Host "Environment retrieved `n" -ForegroundColor Green
    
    if ($null -eq $env.properties.enterprisePolicies -or $null -eq $env.properties.enterprisePolicies.$PolicyType)
    {
        Write-Host "No enterprise policy of $PolicyType present to swap for environment $EnvironmentId"
        return
    }

    #Validate Enterprise Policy
    $policySystemId = Get-EnterprisePolicySystemId -PolicyArmId $PolicyArmId
    if ($null -eq $policySystemId)
    {
        return
    }
    Write-Host "Enterprise Policy retrieved `n" -ForegroundColor Green

    $swapResult = New-EnterprisePolicyLink -Environment $env -PolicyType $PolicyType -PolicySystemId $PolicySystemId

    $swapResultString = $swapResult | ConvertTo-Json

    if ($null -eq $swapResult -or $swapResult.StatusCode -ne "202")
    {
        Write-Host "Swapping of $policyType policy did not start for environment $environmentId"
        Write-Host "Error: $swapResultString"
        return
    }

    Write-Host "Swapping of $policyType policy started for environment $environmentId"
    Invoke-PollOperation -Headers $swapResult.Headers
}


function Get-EnterprisePolicyForEnvironment
{
    param(
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [PolicyType]$PolicyType,

        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [String]$EnvironmentId,

        [Parameter(Mandatory=$false)]
        [BAPEndpoint]$Endpoint = "prod"
    )

    if (-not(Connect-Bap -Endpoint $Endpoint))
    {
        return
    }

    #Validate Environment
    $env = Get-Environment -EnvironmentId $EnvironmentId

    if ($null -eq $env) 
    {
        return
    }
    Write-Host "Environment retrieved `n" -ForegroundColor Green
    
    if ($null -eq $env.properties.enterprisePolicies -or $null -eq $env.properties.enterprisePolicies.$PolicyType)
    {
        Write-Host "No enterprise policy present of $PolicyType in environment $EnvironmentId"
        return
    }

    Write-Host "Enterprise Policy of type $PolicyType retrieved for environment $EnvironmentId `n" -ForegroundColor Green
    $policyArmId = $env.properties.enterprisePolicies.$PolicyType.id
    Write-Host "Enterprise Policy Arm Id $policyArmId"
}

function LinkPolicyToPlatformAppsData 
{
    param(
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [PolicyType]$policyType,

        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [String]$policyArmId,

        [Parameter(Mandatory=$false)]
        [BAPEndpoint]$Endpoint = "prod"
    )

    if (-not(Connect-Bap -Endpoint $endpoint))
    {
        return
    }

     #Validate PlatformApps enrollment
    $platformAppsStatus = Get-PlatformApps

    if ($null -eq $platformAppsStatus -or $platformAppsStatus.enrollmentState -ne "Enrolled") 
    {
        Write-Host "PlatformApps not enrolled"
        return
    }
    Write-Host "PlatformApps enrolled `n" -ForegroundColor Green

    #Validate Enterprise Policy
    $policySystemId = Get-EnterprisePolicySystemId -PolicyArmId $policyArmId
    if ($null -eq $policySystemId)
    {
        return
    }
    Write-Host "Enterprise Policy retrieved `n" -ForegroundColor Green


    $linkResult = New-EnterprisePolicyToPlatformAppsData -PolicyType $policyType -PolicySystemId $policySystemId

    $linkResultString = $linkResult | ConvertTo-Json

    if ($null -eq $linkResult -or $linkResult.StatusCode -ne "202")
    {
        Write-Host "Linking of $policyType policy did not start for platformapps"
        Write-Host "Error: $linkResultString"
        return 
    }

    Write-Host "Linking of $policyType policy started for platformapps"
}


function UnLinkPolicyFromPlatformAppsData 
{
    param(
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [PolicyType]$policyType,

        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [String]$policyArmId,

        [Parameter(Mandatory=$false)]
        [BAPEndpoint]$Endpoint = "prod"
    )

    if (-not(Connect-Bap -Endpoint $endpoint))
    {
        return
    }

    #Validate PlatformApps enrollment
    $platformAppsStatus = Get-PlatformApps

    if ($null -eq $platformAppsStatus -or $platformAppsStatus.enrollmentState -ne "Enrolled") 
    {
        Write-Host "PlatformApps not enrolled"
        return
    }
    Write-Host "PlatformApps enrolled `n" -ForegroundColor Green
  
    if ($null -eq $platformAppsStatus.enterprisePolicies -or $null -eq $platformAppsStatus.enterprisePolicies.$PolicyType)
    {
        Write-Host "No enterprise policy present of type $policyType to remove from PlatformApps"
        return
    }

    if (!$policyArmId.Equals($platformAppsStatus.enterprisePolicies.$PolicyType.id))
    {
        Write-Host "Given policyArmId $policyArmId not matching with $policyType policy ArmId for Platformapps"
        return 
    }
    
    #Validate Enterprise Policy
    $policySystemId = Get-EnterprisePolicySystemId -PolicyArmId $policyArmId
    if ($null -eq $policySystemId)
    {
        return
    }
    Write-Host "Enterprise Policy retrieved `n" -ForegroundColor Green

    $unLinkResult = Remove-EnterprisePolicyForPlatformAppsData -PolicyType $policyType -PolicySystemId $policySystemId

    $unLinkResultString = $unLinkResult | ConvertTo-Json

    if ($null -eq $unLinkResult -or $unLinkResult.StatusCode -ne "202")
    {
        Write-Host "Unlinking of $policyType policy did not start for platformapps"
        Write-Host "Error: $unLinkResultString"
        return 
    }

    Write-Host "Unlinking of $policyType policy started for platformapps"
}




