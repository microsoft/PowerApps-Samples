function BapLogin($endpoint) {

    $logIn = $false

    # Login - only needs to be run once per session
    if ($null -eq $global:currentSession.userId) {
        $logIn = $true
    }

    if (($null -eq $global:currentSession.expiresOn) -or (get-date $global:currentSession.expiresOn) -lt (Get-Date)) {
        $logIn = $true
    }

    $envSearch = $env + "*"

    if ($global:currentSession.bapEndpoint -notlike $envSearch) {
        $logIn = $true
    }

    if ($logIn) {
        $result = Add-PowerAppsAccount -Endpoint $endpoint
        echo $result
    }
}

function GetEnvironment ($environmentId) 
{
    $ApiVersion = "2016-11-01"
    $environmentResult = GetEnvironmentFromBAP $environmentId $ApiVersion "GET"

    if ($environmentResult.Id -eq $null) 
    {
        Write-Host "Error getting environment with $environmentId for endpoint $endpoint Error = $environmentResult `n" -ForegroundColor Red
        return $null
    }
    
    return $environmentResult
}

function GetEnvironmentFromBAP ($environmentId, $ApiVersion, $method, $body)
{
    $getEnvironmentUri = "https://{bapEndpoint}/providers/Microsoft.BusinessAppPlatform/environments/{environmentId}/?&api-version={apiVersion}" `
    | ReplaceMacro -Macro "{environmentId}" -Value $environmentId

    $environmentResult = InvokeApi -Method $method -Route $getEnvironmentUri -ApiVersion $ApiVersion -Body $body

    return $environmentResult
}

function CallBAPLinkOrUnlink ($environmentId, $ApiVersion, $method, $body, $isLink, $PolicyType)
{
    $operationName = switch ( $isLink )
    {
        true { "link" }
        false { "unlink" }
    }

    $policyTypeInUrl = switch ($policyType)
    {
        "cmk" { "Encryption" }
        "vnet" { "NetworkInjection" }
        "identity" { "Identity" }
    }

    $linkEnterprisePolicyUri = "https://{bapEndpoint}/providers/Microsoft.BusinessAppPlatform/environments/{environmentId}/enterprisePolicies/{policyTypeInUrl}/{operationName}?&api-version={apiVersion}" `
    | ReplaceMacro -Macro "{environmentId}" -Value $environmentId | ReplaceMacro -Macro "{operationName}" -Value $operationName | ReplaceMacro -Macro "{policyTypeInUrl}" -Value $policyTypeInUrl

    $linkEnterprisePolicyResult = InvokeApi -Method $method -Route $linkEnterprisePolicyUri -ApiVersion $ApiVersion -Body $body

    return $linkEnterprisePolicyResult
}

function LinkEnterprisePolicy ($environment, $policyType, $policySystemId)
{
    $ApiVersion = "2019-10-01"
    
    $body = [pscustomobject]@{
        "SystemId" = $policySystemId
        }

    $linkResult = CallBAPLinkOrUnlink $environment.Name $ApiVersion "Post" $body true $policyType $policyType
 
    return $linkResult
}

function UnLinkEnterprisePolicy ($environment, $policyType, $policySystemId)
{
    $ApiVersion = "2019-10-01"

    $body = [pscustomobject]@{
        "SystemId" = $policySystemId
        }

    $unlinkResult = CallBAPLinkOrUnlink $environment.Name $ApiVersion "Post" $body false $policyType $policyType


    return $unlinkResult
}

function PollLinkUnlinkOperation ($operationLink, $pollInterval)
{

    $run = $true
    while ($run)
    {
        $pollResult = InvokeApi -Method GET -Route $operationLink

        if ($null -eq $pollResult -or $null -eq $pollResult.id -or $null -eq $pollResult.state)
        {
            echo "Operation polling failed $pollResult"
            $run = $false
        }

        $operationState = $pollResult.state.id 
        if ($operationState.Equals("Failed") -or $operationState.Equals("Succeeded"))
        {
             echo "Operation finished with state $operationState"
             $run = $false
        }
        elseif ($operationState.Equals("Running"))
        {
            echo "Operation still running. Poll after $pollInterval seconds"
            start-sleep -seconds $pollInterval

        }
        else
        {
            echo "unknown operation state $operationState"
            $run = $false
        }
    }
}

function LinkEnterprisePolicyToPlatformAppsData ($policyType, $policySystemId)
{
    $ApiVersion = "2024-05-01"
    
    $body = [pscustomobject]@{
        "SystemId" = $policySystemId
        }

    $linkResult = CallBAPLinkOrUnlinkForPlatformAppsData $ApiVersion "Post" $body true $policyType
 
    return $linkResult
}

function CallBAPLinkOrUnlinkForPlatformAppsData ($ApiVersion, $method, $body, $isLink, $PolicyType)
{
    $operationName = switch ( $isLink )
    {
        true { "link" }
        false { "unlink" }
    }

    $policyTypeInUrl = switch ($policyType)
    {
        "cmk" { "Encryption" }
        "vnet" { "NetworkInjection" }
        "identity" { "Identity" }
    }

    $linkEnterprisePolicyUri = "https://{bapEndpoint}/providers/Microsoft.BusinessAppPlatform/platformapps/enterprisePolicies/{policyTypeInUrl}/{operationName}?&api-version={apiVersion}" `
    | ReplaceMacro -Macro "{operationName}" -Value $operationName | ReplaceMacro -Macro "{policyTypeInUrl}" -Value $policyTypeInUrl

    $linkEnterprisePolicyResult = InvokeApi -Method $method -Route $linkEnterprisePolicyUri -ApiVersion $ApiVersion -Body $body

    return $linkEnterprisePolicyResult
}


function UnLinkEnterprisePolicyForPlatformAppsData ($policyType, $policySystemId)
{
    $ApiVersion = "2024-05-01"

    $body = [pscustomobject]@{
        "SystemId" = $policySystemId
        }

    $unlinkResult = CallBAPLinkOrUnlinkForPlatformAppsData $ApiVersion "Post" $body false $policyType $policyType


    return $unlinkResult
}

function GetPlatformApps () 
{
    $ApiVersion = "2024-05-01"
    $method = "GET"

    $getPlatformAppsUri = "https://{bapEndpoint}/providers/Microsoft.BusinessAppPlatform/platformapps/status?&api-version={apiVersion}" `

    $platformAppsResult = InvokeApi -Method $method -Route $getPlatformAppsUri -ApiVersion $ApiVersion -Body $body

    if ($platformAppsResult -eq $null) 
    {
        Write-Host "Error getting platformapps for endpoint $endpoint Error = $platformAppsResult `n" -ForegroundColor Red
        return $null
    }
    
    return $platformAppsResult
}