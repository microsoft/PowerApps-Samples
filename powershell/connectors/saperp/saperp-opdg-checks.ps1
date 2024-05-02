<#
.SYNOPSIS
    A script used to check various configs and settings for Power Platform SAP ERP connector.
    Must be ran on the on-premises data gateway in an elevated Powershell window.
    This script does not make any changes, it only reads current values from the Windows operating system and on-prem Active Directory.

.PARAMETER sapServicePrincipalName
    The Service Principal Name (SPN) of the SAP instance (eg. SAP/DV6). This is used to check if the SPN exists in Active Directory.

.PARAMETER sapServicePrincipal
    The Active Directory Service Principal of the SAP instance that is defined within the SAP tcode SPNEGO.

.PARAMETER opdgServicePrincipal
    The Active Directory Service Principal that is running the On-premises data gateway Windows service.

.PARAMETER sapCclDllPath
    The absolute path to the SAP Common Crpyto Library dll (eg. c:\sapcryptolib\sapcrypto.dll).

.PARAMETER sapCclIniPath
    The absolute path to the SAP Common Crpyto Library ini config file (eg. c:\sapcryptolib\sapcrypto.ini).

.PARAMETER sapHostName
    The hostname or IP address of the SAP server.

.PARAMETER sapPort
    The port of the SAP service to connect to.

.EXAMPLE
    .\saperp-opdg-checks.ps1 -sapServicePrincipalName SAP/DV6 -sapServicePrincipal ehpsdv6 -opdgServicePrincipal opdgsapclienttools -sapCclDllPath c:\sapcryptolib\sapcrypto.dll -sapCclIniPath c:\sapcryptolib\sapcrypto.ini
    A Kerberos example.

.EXAMPLE
    .\saperp-opdg-checks.ps1
    A prereq only check example.

.LINK
    https://go.microsoft.com/fwlink/?linkid=2240708

.LINK
    https://go.microsoft.com/fwlink/?linkid=2240537
#>

Param(
    [Parameter(Mandatory = $false, HelpMessage = "The SAP service principal name (eg. SAP/DV6)")] [string] $sapServicePrincipalName = "",
    [Parameter(Mandatory = $false, HelpMessage = "The Active Directory service principal that the OPDG service should be running as")] [string] $opdgServicePrincipal = "",
    [Parameter(Mandatory = $false, HelpMessage = "The Active Directory service principal defined within the SAP tcode SPNEGO")] [string] $sapServicePrincipal = "",
    [Parameter(Mandatory = $false, HelpMessage = "The absolute path to the SAP Crypto Common Library dll (eg. c:\sapcryptolib\sapcrypto.dll)")] [string] $sapCclDllPath = "",
    [Parameter(Mandatory = $false, HelpMessage = "The absolute path to the SAP Crypto Common Library ini (eg. c:\sapcryptolib\sapcrypto.ini)")] [string] $sapCclIniPath = "",
    [Parameter(Mandatory = $true, HelpMessage = "The hostname or IP address of the SAP server (eg. sap.contoso.com)")] [string] $sapHostName = "",
    [Parameter(Mandatory = $true, HelpMessage = "The port of the SAP service to connect to (eg. 3300)")] [int] $sapPort = 0
)

function main() {
    # Helper function that runs all of the other checks

    # Prereq checks
    checkHostConnection -hostName $sapHostName -port $sapPort
    checkVisualCPlusPlus
    checkNCo
    checkOnPremisesDataGateway
    $isActiveDirectoryModuleInstalled = checkActiveDirectoryPowerShellModule

    if (-not $isActiveDirectoryModuleInstalled) {
        return
    }

    # Kerberos checks
    checkServicePrincipalName($sapServicePrincipalName)
    checkServicePrincipalSupportedEncryptionTypes($sapServicePrincipal)
    checkAllowedToDelegateTo -servicePrincipal $opdgServicePrincipal -servicePrincipalName $sapServicePrincipalName
    checkTrustedToAuthForDelegation($opdgServicePrincipal)
    checkActAsPartOfOperatingSystem($opdgServicePrincipal)
    checkImpersonateAClientAfterAuthentication($opdgServicePrincipal)
    checkGatewayServiceRunsAs($opdgServicePrincipal)
    checkCommonCryptoLibVersion($sapCclDllPath)
    checkSapCryptoIniFile($sapCclIniPath)
}

function checkActiveDirectoryPowerShellModule() {
    # Required for Kerberos checks
    $module = Get-Module -ListAvailable -Name ActiveDirectory

    if ($module) {
        Write-Host -ForegroundColor Green "[Kerberos] ActiveDirectory PowerShell module is installed"
        return $true
    }
    else {
        Write-Host -ForegroundColor Yellow "[Kerberos] ActiveDirectory PowerShell module is not installed. Kerberos checks will not be performed."
        Write-Host "`Install => https://go.microsoft.com/fwlink/?linkid=2240709"
        return $false
    }
}

function checkVisualCPlusPlus() {
    # Prereq for NCo 3.0
    $installedSoftware = Get-ChildItem "HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall"
    $name = "Microsoft Visual C++ 2010  x64 Redistributable - 10.0.40219"
    $obj = $installedSoftware | Where-Object { $_.GetValue("DisplayName") -eq $name }

    if ($obj) {
        Write-Host -Foreground Green "[Prereq] $name is installed"
    }
    else {
        Write-Host -Foreground Red "[Prereq] $name is not installed"
        Write-Host "`tDownload => https://go.microsoft.com/fwlink/?linkid=2240459"
    }
}

function checkNCo() {
    # .NET SDK for SAP
    $currentLocation = Get-Location
    $location = "C:\Windows\Microsoft.NET\assembly"
    $ncoAssemblies = Get-ChildItem -Path $location -Filter sapnco.dll -Recurse | Select-Object -ExpandProperty VersionInfo
    $location = "C:\Program Files\SAP\SAP_DotNetConnector3_Net40_x64"
    $ncoAssemblies += Get-ChildItem -Path $location -Filter sapnco.dll -Recurse | Select-Object -ExpandProperty VersionInfo
    $counter = 0

    foreach ($assembly in $ncoAssemblies) {
        $fileName = $assembly.FileName

        if ($assembly.FileVersion.StartsWith("3.1.")) {
            Write-Host -Foreground Red "[Prereq] NCo 3.1 found"
            Write-Host "`tUninstall => $fileName"
        }

        if ($fileName -like "*GAC_32*") {
            Write-Host -Foreground Red "[Prereq] NCo 32-bit found"
            Write-Host "`tUninstall => $fileName"
        }

        if (($fileName -like "*GAC_64*" -or $fileName -like "*x64*") -and $assembly.FileVersion.StartsWith("3.0.")) {
            $counter++
            Write-Host -Foreground Green "[Prereq] NCo 64-bit 3.0 found"
        }
    }

    if ($counter -eq 0) {
        Write-Host -Foreground Red "[Prereq] NCo 3.0 64bit not found."
        Write-Host "`tDownload => https://go.microsoft.com/fwlink/?linkid=2240708"
    }

    Set-Location $currentLocation
}

function checkOnPremisesDataGateway() {
    # Check for On-premises data gateway install
    $minimumVersion = "3000.150.11" # November 2022
    $serviceName = "PBIEgwService"
    $service = Get-Service $serviceName -ErrorAction SilentlyContinue

    if ($service) {
        $servicePath = (Get-WmiObject Win32_Service | Where-Object { $_.Name -eq $serviceName }).PathName
        $exePath = $servicePath.Trim('"')
        $versionInfo = (Get-Command $exePath).FileVersionInfo
        $versionNumber = $versionInfo.ProductVersion

        if ([version]$versionNumber -ge [version]$minimumVersion) {
            Write-Host -ForegroundColor Green "[Prereq] On-premises data gateway is installed and version ($versionNumber) is greater than or equal to ($minimumVersion)"
        }
        else {
            Write-Host -ForegroundColor Red "[Prereq] On-premises data gateway doesn't meet the minimum version, please upgrade"
            Write-Host "`tDownload => https://go.microsoft.com/fwlink/?linkid=2240537"
        }
    }
    else {
        Write-Host -ForegroundColor Red "[Prereq] On-premises data gateway is not installed"
        Write-Host "`tDownload => https://go.microsoft.com/fwlink/?linkid=2240537"
    }
}

function checkServicePrincipalName($servicePrincipalName) {
    # Prereq for Kerberos. Checks Active Directory for the existence of Service Principal Name
    if (-not $servicePrincipalName) {
        Write-Host -Foreground Yellow "[Kerberos] Skipping ServicePrincipalName check"
        return
    }

    try {
        $servicePrincipal = Get-ADUser -Filter { ServicePrincipalNames -like $servicePrincipalName } -Properties ServicePrincipalNames -ErrorAction Stop
    }
    catch {
        Write-Host -Foreground Red "[Kerberos] ServicePrincipalName error occurred: $_"
        return
    }

    if ($servicePrincipal) {
        Write-Host -Foreground Green "[Kerberos] Service Principal Name ($servicePrincipalName) exists on => $servicePrincipal"
    }
    else {
        Write-Host -Foreground Red "[Kerberos] Service Principal Name ($servicePrincipalName) does not exist"
    }
}

function checkServicePrincipalSupportedEncryptionTypes($servicePrincipal) {
    # Preqreq for Kerberos. Checks if AES is enabled for the servicePrincipal.
    if (-not $servicePrincipal) {
        Write-Host -Foreground Yellow "[Kerberos] Skipping ServicePrincipalSupportedEncryptionTypes check"
        return
    }

    # All the decimal values that contain AES encryption types
    # https://go.microsoft.com/fwlink/?linkid=2240296
    $aesEncryptionTypes = @(8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31)

    try {
        $supportedEncryptionTypes = Get-ADUser $servicePrincipal -Properties msDS-SupportedEncryptionTypes -ErrorAction Stop
        $value = $supportedEncryptionTypes."msDS-SupportedEncryptionTypes"
    }
    catch {
        Write-Host -Foreground Red "[Kerberos] ServicePrincipalSupportedEncryptionTypes error occurred: $_"
        return
    }

    if ($aesEncryptionTypes.Contains($value)) {
        Write-Host -Foreground Green "[Kerberos] AES is enabled (msDS-SupportedEncryptionTypes => $value) for $servicePrincipal"
    }
    else {
        Write-Host -Foreground Red "[Kerberos] AES is missing (msDS-SupportedEncryptionTypes) for $servicePrincipal"
    }
}

function checkAllowedToDelegateTo($servicePrincipal, $servicePrincipalName) {
    # Checks if OPDG servicePrincipal is allowed to delegate to the SAP service principal name
    if (-not $servicePrincipal -or -not $servicePrincipalName) {
        Write-Host -Foreground Yellow "[Kerberos] Skipping AllowedToDelegate check"
        return
    }

    try {
        $allowedToDelegateTo = Get-ADUser $servicePrincipal -Properties msDS-AllowedToDelegateTo -ErrorAction Stop
        $value = $allowedToDelegateTo."msDS-AllowedToDelegateTo"
    }
    catch {
        Write-Host -Foreground Red "[Kerberos] AllowedToDelegateTo error occurred: $_"
        return
    }

    if ($null -ne $value -and $value.Contains($servicePrincipalName)) {
        Write-Host -Foreground Green "[Kerberos] Delegation enabled (msDS-AllowedToDelegateTo => $value) for $servicePrincipal"
    }
    else {
        Write-Host -Foreground Red "[Kerberos] Delegation disabled (msDS-AllowedToDelegateTo) for $servicePrincipal"
        Write-Host "`tCheck the $servicePrincipal account in Active Directory and add $servicePrincipalName on the Delegation tab."
    }
}

function checkTrustedToAuthForDelegation($servicePrincipal) {
    # Checks if OPDG service principal in Active Directory has "Use any authentication protocol" enabled on Delegation tab
    if (-not $servicePrincipal) {
        Write-Host -Foreground Yellow "[Kerberos] Skipping TrustedToAuthForDelegation check"
        return
    }

    try {
        $trustedToAuthForDelegation = Get-ADUser -Identity $servicePrincipal -Properties TrustedToAuthForDelegation -ErrorAction Stop | Select-Object TrustedToAuthForDelegation
        $value = $trustedToAuthForDelegation."trustedToAuthForDelegation"
    }
    catch {
        Write-Host -Foreground Red "[Kerberos] TrustedToAuthForDelegation error occurred: $_"
        return
    }

    if ($value) {
        Write-Host -Foreground Green "[Kerberos] TrustedToAuthForDelegation enabled ($value) for $servicePrincipal"
    }
    else {
        Write-Host -Foreground Red "[Kerberos] TrustedToAuthForDelegation disabled for $servicePrincipal"
        Write-Host "`tCheck the $servicePrincipal account in AD and enable 'Use any authentication protocol' on Delegation the tab."
    }
}

function checkActAsPartOfOperatingSystem($servicePrincipal) {
    # Checks OPDG operating system that service servicePrincipal has "Act as part of the operating system" privilege
    if (-not $servicePrincipal) {
        Write-Host -Foreground Yellow "[Kerberos] Skipping ActAsPartOfOperatingSystem check"
        return
    }

    try {
        $accountSid = Get-ADUser -Identity $servicePrincipal -Properties ObjectGUID | Select-Object SID | Out-Null
    }
    catch {
        Write-Host -Foreground Red "[Kerberos] ActAsPartOfOperatingSystem error occurred: $_"
        return
    }

    # This is not a PowerShell cmdlet, so we need to use the old command line tool
    secedit /export /cfg $env:TEMP\secpol.cfg | Out-Null

    # Checks if the secedit command was successful or not
    if ($LASTEXITCODE -ne 0) {
        Write-Host -Foreground Red "[Kerberos] ActAsPartOfOperatingSystem failed. Try running with an elevated PowerShell prompt."
        return
    }

    $value = (Get-Content $env:TEMP\secpol.cfg | Select-String "SeTcbPrivilege").ToString()
    Remove-Item $env:TEMP\secpol.cfg

    if ($value.Contains($accountSid.SID.Value)) {
        Write-Host -Foreground Green "[Kerberos] ActAsPartOfOperatingSystem enabled ($value) for $servicePrincipal "
    }
    else {
        Write-Host -Foreground Red "[Kerberos] ActAsPartOfOperatingSystem disabled ($value) for $servicePrincipal "
        Write-Host "`tCheck the 'Act as part of the operating system' property of the $servicePrincipal in the Local Security Policy console"
    }
}

function checkImpersonateAClientAfterAuthentication($servicePrincipal) {
    # Checks OPDG operating system that service principal has "Impersonate a client after authentication" privilege
    if (-not $servicePrincipal) {
        Write-Host -Foreground Yellow "[Kerberos] Skipping ImpersonateAClientAfterAuthentication check"
        return
    }

    try {
        $accountSid = Get-ADUser -Identity $servicePrincipal -Properties ObjectGUID -ErrorAction Stop | Select-Object SID
    }
    catch {
        Write-Host -Foreground Red "[Kerberos] ImpersonateAClientAfterAuthentication error occurred: $_"
        return
    }

    # This is not a PowerShell cmdlet, so we need to use the old command line tool
    secedit /export /cfg $env:TEMP\secpol.cfg | Out-Null

    # Checks if the secedit command was successful or not
    if ($LASTEXITCODE -ne 0) {
        Write-Host -Foreground Red "[Kerberos] ImpersonateAClientAfterAuthentication failed. Try running with an elevated PowerShell prompt."
        return
    }

    $value = (Get-Content $env:TEMP\secpol.cfg | Select-String "SeImpersonatePrivilege").ToString()
    Remove-Item $env:TEMP\secpol.cfg

    if ($value.Contains($accountSid.SID.Value)) {
        Write-Host -Foreground Green "[Kerberos] ImpersonateAClientAfterAuthentication enabled ($value) for $servicePrincipal"
    }
    else {
        Write-Host -Foreground Red "[Kerberos] ImpersonateAClientAfterAuthentication disabled ($value) for $servicePrincipal"
        Write-Host "`tCheck the 'Impersonate a client after authentication' property of the $servicePrincipal in the Local Security Policy console"
    }
}

function checkGatewayServiceRunsAs($servicePrincipal) {
    # Check what servicePrincipal the OPDG service is running as
    if (-not $servicePrincipal) {
        Write-Host -Foreground Yellow "[Kerberos] Skipping GatewayServiceRunsAs check"
        return
    }

    $service = Get-WmiObject -Class Win32_Service -Filter "Name='PBIEgwService'"

    if ($null -ne $service -and $service.StartName -ilike "*$servicePrincipal*") {
        Write-Host -Foreground Green "[Kerberos] GatewayServiceRunsAs is running as $servicePrincipal"
    }
    else {
        Write-Host -Foreground Red "[Kerberos] GatewayServiceRunsAs is not running as $servicePrincipal"
        Write-Host "`tCheck the 'Log On As' property of the 'On-premises data gateway service' in the Windows Services console. It should be running as $servicePrincipal"
    }
}

function checkCommonCryptoLibVersion($path) {
    # Check what version of SAP CommonCryptoLib is installed
    if (-not $path) {
        Write-Host -Foreground Yellow "[Kerberos] Skipping CommonCryptoLibVersion check"
        return
    }

    $minimumVersion = "8.5.25.0"

    try {
        $actualVersion = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($path).FileVersion
    }
    catch {
        Write-Host -Foreground Red "[Kerberos] CommonCryptoLibVersion error occurred: $_"
        Write-Host "`tDid you specify the correct path to the SAP CommonCryptoLib DLL?"
        return
    }

    if ([version]$actualVersion -ge [version]$minimumVersion) {
        Write-Host -Foreground Green "[Kerberos] SAP Common Crypto Library ($path) is running version $actualVersion"
    }
    else {
        Write-Host -Foreground Red "[Kerberos] SAP Common Crypto Library is running an old version $actualVersion"
        Write-Host "`tDownload the latest version of the SAP Common Crypto Library from SAP"
    }
}

function checkSapCryptoIniFile($path) {
    # Check that the SAP Crypto INI file exists and has the correct parameters
    if (-not $path) {
        Write-Host -Foreground Yellow "[Kerberos] Skipping SapCryptoIniFile check"
        return
    }

    $clientRole = "ccl/snc/enable_kerberos_in_client_role=1"

    try {
        $iniContent = Get-Content -Path $path -ErrorAction Stop
    }
    catch {
        Write-Host -Foreground Red "[Kerberos] SapCryptoIniFile error occurred: $_"
        Write-Host "`tDid you specify the correct path to the sapcrypto.ini file?"
        return
    }

    if ($iniContent -match [regex]::Escape($clientRole).Replace('=', '\s*=\s*')) {
        Write-Host -Foreground Green "[Kerberos] sapcrypto.ini ($path) has the correct parameters: $clientRole"
    }
    else {
        Write-Host -Foreground Red "[Kerberos] $path is missing the correct parameters"
        Write-Host "`tModify $path and add $clientRole"
    }

    checkSystemEnvVariable -variableName "CCL_PROFILE"
}

function checkSystemEnvVariable($variableName) {
    if (-not $variableName) {
        Write-Host -Foreground Yellow "[Kerberos] Skipping SystemEnvVariable check"
        return
    }

    $variableValue = [Environment]::GetEnvironmentVariable($variableName, "Machine")

    if ($variableValue -ne $null) {
        Write-Host -ForegroundColor Green "[Kerberos] System environment variable $variableName exists with value $variableValue"
    }
    else {
        Write-Host -ForegroundColor Red "[Kerberos] System environment variable $variableName does not exist"
    }
}

function checkHostConnection($hostName, $port) {
    $timeout = 25
    Write-Host -Foreground Green "[Prereq] Testing tcp connection to ${hostName}:$port please wait" -NoNewline

    $job = Start-Job -ScriptBlock {
        param($hostName, $port)
        Test-NetConnection -ComputerName $hostName -Port $port
    } -ArgumentList $hostName, $port

    for ($i = 0; $i -lt $timeout; $i++) {
        Start-Sleep -Seconds 1
        Write-Host "." -NoNewline
    }

    if (Wait-Job -Job $job -Timeout $timeout) {
        $result = Receive-Job -Job $job

        if ($result.TcpTestSucceeded) {
            Write-Host -Foreground Green "success"
        }
        else {
            Write-Host -Foreground Red "failed"
        }
    }
    else {
        Write-Host -Foreground Red "timed out after $timeout seconds"
    }

    Remove-Job -Force -Job $job
}
main
