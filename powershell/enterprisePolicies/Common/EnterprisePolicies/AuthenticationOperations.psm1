<#
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS. MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
NO TECHNICAL SUPPORT IS PROVIDED. YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
#>

function Connect-Azure{

    Write-Host "Logging In..." -ForegroundColor Green
    $connect = Connect-AzAccount

    if ($null -eq $connect)
    {
        Write-Error "Error connecting to Azure Account `n"
        return $false
    }
    Write-Host "Logged In..." -ForegroundColor Green
    return $true
}

function Connect-Bap {
    param(
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [BAPEndpoint]$Endpoint
    )

    Write-Host "Logging In..." -ForegroundColor Green
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
        $result = Add-PowerAppsAccount -Endpoint $Endpoint
        Write-Host $result
    }
    Write-Host "Logged In..." -ForegroundColor Green
    return $true
}