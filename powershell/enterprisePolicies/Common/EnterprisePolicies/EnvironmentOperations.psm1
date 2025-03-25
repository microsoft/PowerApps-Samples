<#
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS. MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
NO TECHNICAL SUPPORT IS PROVIDED. YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
#>

function Get-Environment
{
    param (
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string]$EnvironmentId
    )

    $ApiVersion = "2016-11-01"

    $getEnvironmentUri = "https://{bapEndpoint}/providers/Microsoft.BusinessAppPlatform/environments/$EnvironmentId/?&api-version={apiVersion}" `

    $environmentResult = InvokeApi -Method $method -Route $getEnvironmentUri -ApiVersion $ApiVersion -Body $body

    if ($null -eq $environmentResult.Id) 
    {
        Write-Host "Error getting environment with $environmentId for endpoint $endpoint Error = $environmentResult `n" -ForegroundColor Red
        return $null
    }
    
    return $environmentResult
}

function Invoke-BAPLinkOrUnlink
{
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string]$EnvironmentId,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string]$ApiVersion,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string]$Method,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [PSCustomObject]$Body,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [LinkOperation]$LinkOperation,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [PolicyType]$PolicyType
    )

    $linkEnterprisePolicyUri = "https://{bapEndpoint}/providers/Microsoft.BusinessAppPlatform/environments/$EnvironmentId/enterprisePolicies/$PolicyType/$LinkOperation?&api-version={apiVersion}" `

    $linkEnterprisePolicyResult = InvokeApi -Method $Method -Route $linkEnterprisePolicyUri -ApiVersion $ApiVersion -Body $Body

    return $linkEnterprisePolicyResult
}

function New-EnterprisePolicyLink
{
    param(
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty]
        [PSCustomObject] $Environment,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty]
        [PolicyType] $PolicyType,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty]
        [string] $PolicySystemId
    )

    $ApiVersion = "2019-10-01"
    
    $body = [PSCustomObject]@{
        "SystemId" = $policySystemId
    }

    $linkResult = Invoke-BAPLinkOrUnlink -Environment $Environment.Name -ApiVersion $ApiVersion -Method "Post" -Body $body -LinkOperation [LinkOperation]::Link -PolicyType $policyType
 
    return $linkResult
}

function Remove-EnterprisePolicyLink
{
    param(
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty]
        [string] $Environment,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty]
        [PolicyType] $PolicyType,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty]
        [string] $PolicySystemId
    )

    $ApiVersion = "2019-10-01"

    $body = [PSCustomObject]@{
        "SystemId" = $policySystemId
        }

    $unlinkResult = Invoke-BAPLinkOrUnlink -Environment $environment.Name $ApiVersion "Post" $body false $policyType $policyType

    return $unlinkResult
}

function New-EnterprisePolicyToPlatformAppsData
{
    param (
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [PolicyType] $PolicyType,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string] $PolicySystemId
    )

    $ApiVersion = "2024-05-01"
    
    $body = [PSCustomObject]@{
        "SystemId" = $PolicySystemId
    }

    $linkResult = Invoke-BAPLinkOrUnlinkForPlatformAppsData -ApiVersion $ApiVersion -Method "Post" -Body $body -LinkOperation [LinkOperation]::Link -PolicyType $PolicyType
 
    return $linkResult
}

function Invoke-BAPLinkOrUnlinkForPlatformAppsData
{
    param (
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string] $ApiVersion,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string] $Method,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [PSCustomObject] $Body,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [LinkOperation] $LinkOperation,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [PolicyType] $PolicyType
    )

    $linkEnterprisePolicyUri = "https://{bapEndpoint}/providers/Microsoft.BusinessAppPlatform/platformapps/enterprisePolicies/$PolicyType/$LinkOperation?&api-version={apiVersion}" `

    $linkEnterprisePolicyResult = InvokeApi -Method $Method -Route $linkEnterprisePolicyUri -ApiVersion $ApiVersion -Body $Body

    return $linkEnterprisePolicyResult
}


function Remove-EnterprisePolicyForPlatformAppsData
{
    param (
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [PolicyType] $PolicyType,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string] $PolicySystemId
    )

    $ApiVersion = "2024-05-01"

    $body = [PSCustomObject]@{
        "SystemId" = $PolicySystemId
    }

    $unlinkResult = Invoke-BAPLinkOrUnlinkForPlatformAppsData -ApiVersion $ApiVersion -Method "Post" -Body $body -LinkOperation [LinkOperation]::unlink -PolicyType $policyType

    return $unlinkResult
}

function Get-PlatformApps
{
    $ApiVersion = "2024-05-01"
    $method = "GET"

    $getPlatformAppsUri = "https://{bapEndpoint}/providers/Microsoft.BusinessAppPlatform/platformapps/status?&api-version={apiVersion}" `

    $platformAppsResult = InvokeApi -Method $method -Route $getPlatformAppsUri -ApiVersion $ApiVersion -Body $body

    if ($null -eq $platformAppsResult) 
    {
        Write-Host "Error getting platformapps for endpoint $endpoint Error = $platformAppsResult `n" -ForegroundColor Red
        return $null
    }
    
    return $platformAppsResult
}