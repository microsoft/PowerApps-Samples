<#
# Copyright (c) Microsoft Corporation.
# Licensed under the MIT license.
# These scripts are only supported in .NET framework compatiable powershell versions (5.1 - default with windows 10 & 11)
#>

# Verify powershell version is supported

if($PSVersionTable.PSVersion.Major -gt 5)
{
    Write-Host("Script is supported only in powershell versions 5 and below")
}

#install Azure powershell modules
$moduleAzure = Get-Module -ListAvailable -Name Az*

if ($null -eq $moduleAzure) 
{
   Install-Module -Name Az -AllowClobber -Force
}

# Pre load and resolve versions for all required assemblies

$OnAssemblyResolve = [System.ResolveEventHandler] {

  param($sender, $e)
  foreach($a in [System.AppDomain]::CurrentDomain.GetAssemblies())
  {
    if ($a.FullName -eq $e.Name)
    { 
      return $a
    }
  }
  return $null
}
[System.AppDomain]::CurrentDomain.add_AssemblyResolve($OnAssemblyResolve)

$dlls = (Get-ChildItem -path ".").fullname

# Load references assemblies needed by the helper module

$dlls = Get-ChildItem -path "." -Filter "*.dll"

foreach($dll in $dlls)
{
    try
    {
        $DLLName = $dll.Name
        $fullDLLPath = $dll.FullName
            [System.Reflection.Assembly]::LoadFrom($fullDLLPath)
        Write-Debug "Loaded $DLLName"
    }
    catch
    {
        $message = $_.Exception.GetBaseException().LoaderExceptions
        Write-Host "Error loading" $dll.name
        Write-Host "exception" $message
        exit
    }
}

$OnAssemblyResolve = [System.ResolveEventHandler] {

  param($sender, $e)
  foreach($a in [System.AppDomain]::CurrentDomain.GetAssemblies())
  {
    if ($a.FullName -eq $e.Name)
    {
      return $a
    }
  }
  return $null
}
[System.AppDomain]::CurrentDomain.add_AssemblyResolve($OnAssemblyResolve)

$helpersDll = 'Microsoft.PowerPlatform.Administration.Helpers.dll'

$dllPath = Join-Path $PSScriptRoot $helpersDll

Add-Type -Path $dllPath

<#
.SYNOPSIS 
Removes specified role from users 

.DESCRIPTION
Removes specified role from users in an environment / all environments in geo / all environments in the tenant.  

.PARAMETER usersFilePath 
Path to file containing list of user princiapl names (one per line)

.PARAMETER roleName
Localized role name in dataverse (Ex: System Administrator)

.PARAMETER environmentUrl
Url of Environment, if admin wants to get reports from only one environment

.PARAMETER processAllEnvironments
Removes roles from all environments the admin user has access to

.PARAMETER geo
Removes roles from environments in given geo. Valid Geo codes - [NA, EMEA, APAC, SAM, OCE, JPN, IND, CAN, GBR, FRA, UAE,ZAF, GER, CHE, KOR, NOR, SGP]

.PARAMETER outputLogsDirectory
Location folder for the logs & reports to be written to.
#>

function Remove-RoleAssignmentFromUsers
{
    [CmdletBinding()]

    Param(

        [Parameter(Mandatory=$true,
                    HelpMessage = "Role name in Dataverse. Ex : 'System Administrator', 'System Customizer' etc.,")]
        [ValidateNotNullOrEmpty()]
        [String]$roleName,

        [Parameter(Mandatory=$false,
                    HelpMessage = "Geo name if you want to clean up role assignments only in specific geo.Processes all geos by default. Valid Geo codes - [NA, EMEA, APAC, SAM, OCE, JPN, IND, CAN, GBR, FRA, UAE,ZAF, GER, CHE, KOR, NOR, SGP]")]
        [String]$geo = "",

        [Parameter(Mandatory=$false,
                    HelpMessage = "Url of the environment to clean up role assignments from.")]
        [String]$environmentUrl = "",

        [Parameter(Mandatory=$false,
                    HelpMessage = "Clean up role assignments in all environments. Clean up only in specific geo if geo is supplied, otherwise global")]
        [boolean]$processAllEnvironments = $false,

        [Parameter(Mandatory=$true,
                    HelpMessage = "File path to the list of users (one user principal name per line) to remove role assignments from")]
        [ValidateNotNullOrEmpty()]
        [String]$usersFilePath,

        [Parameter(Mandatory=$true,
                    HelpMessage = "Directory to write output logs and reports to")]
        [ValidateNotNullOrEmpty()]
        [String]$outputLogsDirectory
    )

    try
    {
        if($PSBoundParameters.ContainsKey('environmentUrl') -eq $false -and $PSBoundParameters.ContainsKey('processAllEnvironments') -eq $false)
        {
            Write-Host "One of the following parameters must be provided : environmentUrl, processAllEnvironments";
            return;
        }

        if($PSBoundParameters.ContainsKey('environmentUrl') -eq $true -and $PSBoundParameters.ContainsKey('processAllEnvironments') -eq $true)
        {
            Write-Host "ONLY one of the following parameters must be provided : environmentUrl, processAllEnvironments";
            return;
        }

        if (Test-Path $outputLogsDirectory) {
   
            Write-Host "Output Directory Exists"
        }
        else
        {
  
            #PowerShell Create directory if not exists
            New-Item $outputLogsDirectory -ItemType Directory
            Write-Host "Output Directory created successfully"
        }

        $outputLogsDirectory = Convert-Path -LiteralPath $outputLogsDirectory

        $usersFilePath = Convert-Path -LiteralPath $usersFilePath

        $tenantOperationsHelper = New-Object -TypeName 'Microsoft.PowerPlatform.Administration.Helpers.TenantOperationsHelper' 
        
        $credentials = Get-Credential -Message "Provide user credentials to connect to dataverse"

        $userName = $credentials.UserName
        $password = $credentials.GetNetworkCredential().Password

        #Connect to Azure AD
        Connect-AzureAD -Credential $credentials

        $tenantOperationsHelper.RemoveRoleAssignmentFromUsers($credentials.UserName, $password, $roleName, $usersFilePath, $environmentUrl, $geo, $processAllEnvironments, $outputLogsDirectory);
    }catch
    {        
        $message = $_.Exception.GetBaseException();        
        Write-Host $message
    }
}

<#
.SYNOPSIS 
Adds specified role to users 

.DESCRIPTION
Adds specified role to users in an environment / all environments in geo / all environments in the tenant.  

.PARAMETER usersFilePath 
Path to file containing list of user princiapl names (one per line)

.PARAMETER roleName
Localized role name in dataverse (Ex: System Administrator)

.PARAMETER environmentUrl
Url of Environment, if admin wants to get reports from only one environment

.PARAMETER processAllEnvironments
Generate reports for all environments the admin user has access to

.PARAMETER geo
Adds roles to users for environments in given geo. Valid Geo codes - [NA, EMEA, APAC, SAM, OCE, JPN, IND, CAN, GBR, FRA, UAE,ZAF, GER, CHE, KOR, NOR, SGP]

.PARAMETER outputLogsDirectory
Location folder for the logs & reports to be written to.
#>

function Add-RoleToUsers
{
    [CmdletBinding()]

    Param(

        [Parameter(Mandatory=$true,
                    HelpMessage = "Role name in Dataverse. Ex : 'System Administrator', 'System Customizer' etc.,")]
        [ValidateNotNullOrEmpty()]
        [String]$roleName,

        [Parameter(Mandatory=$false,
                    HelpMessage = "Adds roles to users for environments in given geo. Valid Geo codes - [NA, EMEA, APAC, SAM, OCE, JPN, IND, CAN, GBR, FRA, UAE,ZAF, GER, CHE, KOR, NOR, SGP]")]
        [String]$geo = "",

        [Parameter(Mandatory=$false,
                    HelpMessage = "Url of the environment to add role assignments from.")]
        [String]$environmentUrl = "",

        [Parameter(Mandatory=$false,
                    HelpMessage = "Add roles to users in all environments. Addonly in specific geo if geo is supplied, otherwise from all geos")]
        [boolean]$processAllEnvironments = $false,

        [Parameter(Mandatory=$true,
                    HelpMessage = "File path to the list of users (one user principal name per line) to add role")]
        [ValidateNotNullOrEmpty()]
        [String]$usersFilePath,

        [Parameter(Mandatory=$true,
                    HelpMessage = "Directory to write output logs and reports to")]
        [ValidateNotNullOrEmpty()]
        [String]$outputLogsDirectory
    )

    try
    {
        if($PSBoundParameters.ContainsKey('environmentUrl') -eq $false -and $PSBoundParameters.ContainsKey('processAllEnvironments') -eq $false)
        {
            Write-Host "One of the following parameters must be provided : environmentUrl, processAllEnvironments";
            return;
        }

        if($PSBoundParameters.ContainsKey('environmentUrl') -eq $true -and $PSBoundParameters.ContainsKey('processAllEnvironments') -eq $true)
        {
            Write-Host "ONLY one of the following parameters must be provided : environmentUrl, processAllEnvironments";
            return;
        }

        if (Test-Path $outputLogsDirectory) {
   
            Write-Host "Output Directory Exists"
        }
        else
        {
  
            #PowerShell Create directory if not exists
            New-Item $outputLogsDirectory -ItemType Directory
            Write-Host "Output Directory created successfully"
        }

        $outputLogsDirectory = Convert-Path -LiteralPath $outputLogsDirectory

        $usersFilePath = Convert-Path -LiteralPath $usersFilePath

        $tenantOperationsHelper = New-Object -TypeName 'Microsoft.PowerPlatform.Administration.Helpers.TenantOperationsHelper' 

        $credentials = Get-Credential -Message "Provide user credentials to connect to dataverse"

        $userName = $credentials.UserName
        $password = $credentials.GetNetworkCredential().Password

        #Connect to Azure AD
        Connect-AzureAD -Credential $credentials

        $tenantOperationsHelper.AddRoleToUsers($credentials.UserName, $password, $roleName, $usersFilePath, $environmentUrl, $geo, $processAllEnvironments, $outputLogsDirectory);
    
    }catch
    {        
        $message = $_.Exception.GetBaseException();
        Write-Host $message
    }
}

<#
.SYNOPSIS 
Generates user role assignment reports for environments 

.DESCRIPTION
Generates a report of users having the supplied role in an environment / all environments in geo / all environments in the tenant.  


.PARAMETER roleName
Localized role name in dataverse (Ex: System Administrator)

.PARAMETER environmentUrl
Url of Environment, if admin wants to get reports from only one environment

.PARAMETER processAllEnvironments
Generate reports for all environments the admin user has access to

.PARAMETER geo
Generate reports for environments in given geo. Valid Geo codes - [NA, EMEA, APAC, SAM, OCE, JPN, IND, CAN, GBR, FRA, UAE,ZAF, GER, CHE, KOR, NOR, SGP]

.PARAMETER outputLogsDirectory
Location folder for the logs & reports to be written to.
#>

function Get-UsersWithRoleAssignment
{
    [CmdletBinding()]

    Param(

        [Parameter(Mandatory=$true,
                    HelpMessage = "Role name in Dataverse. Ex : 'System Administrator', 'System Customizer' etc.,")]
        [ValidateNotNullOrEmpty()]
        [String]$roleName,

        [Parameter(Mandatory=$false,
                    HelpMessage = "Generate reports for environments in given geo. Valid Geo codes - [NA, EMEA, APAC, SAM, OCE, JPN, IND, CAN, GBR, FRA, UAE,ZAF, GER, CHE, KOR, NOR, SGP]")]
        [String]$geo = "",

        [Parameter(Mandatory=$false,
                    HelpMessage = "Url of environment to get role assignments from")]
        [String]$environmentUrl = "",

        [Parameter(Mandatory=$false,
                    HelpMessage = "Get reports from all environments. Gets geo specific reports if geo is supplied, processes all geos otherwise")]
        [boolean]$processAllEnvironments = $false,

        [Parameter(Mandatory=$true,
                    HelpMessage = "Directory to write logs & reports to")]
        [ValidateNotNullOrEmpty()]
        [String]$outputLogsDirectory
    )

    try
    {
        if($PSBoundParameters.ContainsKey('environmentUrl') -eq $false -and $PSBoundParameters.ContainsKey('processAllEnvironments') -eq $false)
        {
            Write-Host "One of the following parameters must be provided : environmentUrl, processAllEnvironments";
            return;
        }

        if($PSBoundParameters.ContainsKey('environmentUrl') -eq $true -and $PSBoundParameters.ContainsKey('processAllEnvironments') -eq $true)
        {
            Write-Host "ONLY one of the following parameters must be provided : environmentUrl, processAllEnvironments";
            return;
        }

        if (Test-Path $outputLogsDirectory) {
   
            Write-Host "Output Directory Exists"
        }
        else
        {
  
            #PowerShell Create directory if not exists
            New-Item $outputLogsDirectory -ItemType Directory
            Write-Host "Output Directory created successfully"
        }

        $outputLogsDirectory = Convert-Path -LiteralPath $outputLogsDirectory

        $tenantOperationsHelper = New-Object -TypeName 'Microsoft.PowerPlatform.Administration.Helpers.TenantOperationsHelper'    

        $credentials = Get-Credential -Message "Provide user credentials to connect to dataverse"

        $userName = $credentials.UserName
        $password = $credentials.GetNetworkCredential().Password

        #Connect to Azure AD
        Connect-AzureAD -Credential $credentials

        $tenantOperationsHelper.GetUsersWithRoleAssignment($credentials.UserName, $password, $roleName, $environmentUrl, $geo, $processAllEnvironments, $outputLogsDirectory);
        
    }catch
    {        
        $message = $_.Exception.GetBaseException();        
        Write-Host $message
    }
}

<#
.SYNOPSIS 
Bulk assign user records to users 

.DESCRIPTION
Bulk assign user records to users  in an environment / all environments in geo / all environments in the tenant.  

.PARAMETER usersFilePath 
Path to file containing list of user princiapl names (source and target user principals separated by commas)

.PARAMETER environmentUrl
Url of Environment, if admin wants to get reports from only one environment

.PARAMETER processAllEnvironments
Removes roles from all environments the admin user has access to

.PARAMETER geo
Removes roles from environments in given geo. Valid Geo codes - [NA, EMEA, APAC, SAM, OCE, JPN, IND, CAN, GBR, FRA, UAE,ZAF, GER, CHE, KOR, NOR, SGP]

.PARAMETER outputLogsDirectory
Location folder for the logs & reports to be written to.
#>

function Add-BulkRecordsToUsers
{
    [CmdletBinding()]

    Param(

        [Parameter(Mandatory=$false,
                    HelpMessage = "Geo name if you want to clean up role assignments only in specific geo.Processes all geos by default. Valid Geo codes - [NA, EMEA, APAC, SAM, OCE, JPN, IND, CAN, GBR, FRA, UAE,ZAF, GER, CHE, KOR, NOR, SGP]")]
        [String]$geo = "",

        [Parameter(Mandatory=$false,
                    HelpMessage = "Url of the environment to clean up role assignments from.")]
        [String]$environmentUrl = "",

        [Parameter(Mandatory=$false,
                    HelpMessage = "Clean up role assignments in all environments. Clean up only in specific geo if geo is supplied, otherwise global")]
        [boolean]$processAllEnvironments = $false,

        [Parameter(Mandatory=$true,
                    HelpMessage = "File path to the list of users (source user principal and target user principal separated by comma) to assign records from")]
        [ValidateNotNullOrEmpty()]
        [String]$usersFilePath,

        [Parameter(Mandatory=$true,
                    HelpMessage = "Directory to write output logs and reports to")]
        [ValidateNotNullOrEmpty()]
        [String]$outputLogsDirectory
    )

    try
    {
        if($PSBoundParameters.ContainsKey('environmentUrl') -eq $false -and $PSBoundParameters.ContainsKey('processAllEnvironments') -eq $false)
        {
            Write-Host "One of the following parameters must be provided : environmentUrl, processAllEnvironments";
            return;
        }

        if($PSBoundParameters.ContainsKey('environmentUrl') -eq $true -and $PSBoundParameters.ContainsKey('processAllEnvironments') -eq $true)
        {
            Write-Host "ONLY one of the following parameters must be provided : environmentUrl, processAllEnvironments";
            return;
        }

        if (Test-Path $outputLogsDirectory) {
   
            Write-Host "Output Directory Exists"
        }
        else
        {
  
            #PowerShell Create directory if not exists
            New-Item $outputLogsDirectory -ItemType Directory
            Write-Host "Output Directory created successfully"
        }

        $outputLogsDirectory = Convert-Path -LiteralPath $outputLogsDirectory

        $usersFilePath = Convert-Path -LiteralPath $usersFilePath

        $tenantOperationsHelper = New-Object -TypeName 'Microsoft.PowerPlatform.Administration.Helpers.TenantOperationsHelper' 
        
        $credentials = Get-Credential -Message "Provide user credentials to connect to dataverse"

        $userName = $credentials.UserName
        $password = $credentials.GetNetworkCredential().Password

        #Connect to Azure AD
        Connect-AzureAD -Credential $credentials

        $tenantOperationsHelper.BulkAssignRecordsToUsers($credentials.UserName, $password, $usersFilePath, $environmentUrl, $geo, $processAllEnvironments, $outputLogsDirectory);
    }catch
    {        
        $message = $_.Exception.GetBaseException();        
        Write-Host $message
    }
}