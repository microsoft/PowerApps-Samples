<#
.SYNOPSIS
Configures and grants consent for all Microsoft Graph application permissions
required for synching emails from Exchange online mailboxes into another Tenant's dataverse.

.DESCRIPTION
Adds the complete Graph application-permission set to an existing Microsoft
Entra app registration, preserves its other API permissions, and grants the
corresponding app roles to its enterprise application.

The script grants Mail.ReadWrite, Mail.Send, Calendars.ReadWrite,
Contacts.ReadWrite, MailboxSettings.ReadWrite, Tasks.ReadWrite.All,
Chat.Read.All, OnlineMeetings.ReadWrite.All, CallRecords.Read.All,
MailboxConfigItem.ReadWrite, and User.Read.All. There are no workload switches.

PreRequisites:
1. Install Microsoft Graph PowerShell:
   Install-Module Microsoft.Graph -Scope CurrentUser
2. Sign in with an activated Global
   Administrator role.
3. Confirm that the application is already created in the MSFT tenant.

Use -WhatIf first to review the proposed changes. The script prompts for
confirmation before changing the app registration and before granting each
permission. Every run writes a timestamped PowerShell transcript under the
same directory as this script.

.PARAMETER TenantId
The Microsoft Entra tenant ID containing the app registration and Exchange
Online mailboxes.

.PARAMETER ClientId
The Application (client) ID of the existing cross-tenant SSS app registration.

.EXAMPLE
.\Grant-SssGraphApplicationPermissions.ps1 `
    -TenantId "11111111-2222-3333-4444-555555555555" `
    -ClientId "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee" `
    -WhatIf

Previews every proposed app-registration and consent change without applying
it.

.EXAMPLE
.\Grant-SssGraphApplicationPermissions.ps1 `
    -TenantId "11111111-2222-3333-4444-555555555555" `
    -ClientId "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"

Connects to Microsoft Graph, configures all required application permissions,
prompts for confirmation, grants administrator consent, and displays the final
consent state.

.NOTES
This script doesn't create the app registration, create credentials, configure
Exchange Application RBAC, or deploy Dynamics 365 App for Outlook.
#>

[CmdletBinding(SupportsShouldProcess, ConfirmImpact = "High")]
param(
    [Parameter(Mandatory)]
    [guid]$TenantId,

    [Parameter(Mandatory)]
    [guid]$ClientId
)

$ErrorActionPreference = "Stop"
$transcriptTimestamp = (Get-Date).ToUniversalTime().ToString("yyyyMMdd-HHmmss")
$transcriptPath = Join-Path $PSScriptRoot (
    "Grant-SssGraphApplicationPermissions-{0}-{1}.log" -f
        $transcriptTimestamp,
        $PID
)

Start-Transcript -Path $transcriptPath -IncludeInvocationHeader
Write-Host "Transcript: $transcriptPath"

try {
$graphAppId = "00000003-0000-0000-c000-000000000000"
$requiredPermissions = @(
    "Mail.ReadWrite"
    "Mail.Send"
    "Calendars.ReadWrite"
    "Contacts.ReadWrite"
    "MailboxSettings.ReadWrite"
    "Tasks.ReadWrite.All"
    "Chat.Read.All"
    "OnlineMeetings.ReadWrite.All"
    "CallRecords.Read.All"
    "MailboxConfigItem.ReadWrite"
    "User.Read.All"
)

$requiredCommands = @(
    "Connect-MgGraph"
    "Get-MgApplication"
    "Update-MgApplication"
    "Get-MgServicePrincipal"
    "Get-MgServicePrincipalAppRoleAssignment"
    "New-MgServicePrincipalAppRoleAssignment"
)

$missingCommands = $requiredCommands | Where-Object {
    -not (Get-Command $_ -ErrorAction SilentlyContinue)
}

if ($missingCommands) {
    throw @"
Microsoft Graph PowerShell is required. Install it, then rerun this script:

Install-Module Microsoft.Graph -Scope CurrentUser

Missing commands: $($missingCommands -join ", ")
"@
}

Connect-MgGraph `
    -TenantId $TenantId `
    -Scopes "Application.ReadWrite.All", "AppRoleAssignment.ReadWrite.All" `
    -NoWelcome

$context = Get-MgContext
if ($context.TenantId -ne "$TenantId") {
    throw "Connected to tenant '$($context.TenantId)', but '$TenantId' was requested."
}

$application = @(
    Get-MgApplication `
        -Filter "appId eq '$ClientId'" `
        -Property "id,appId,displayName,requiredResourceAccess"
)
if ($application.Count -ne 1) {
    throw "Expected one app registration with client ID '$ClientId'; found $($application.Count)."
}
$application = $application[0]

$clientServicePrincipal = @(
    Get-MgServicePrincipal `
        -Filter "appId eq '$ClientId'" `
        -Property "id,appId,displayName"
)
if ($clientServicePrincipal.Count -ne 1) {
    throw "Expected one enterprise application with client ID '$ClientId'; found $($clientServicePrincipal.Count)."
}
$clientServicePrincipal = $clientServicePrincipal[0]

$graphServicePrincipal = @(
    Get-MgServicePrincipal `
        -Filter "appId eq '$graphAppId'" `
        -Property "id,appId,displayName,appRoles"
)
if ($graphServicePrincipal.Count -ne 1) {
    throw "Expected one Microsoft Graph service principal; found $($graphServicePrincipal.Count)."
}
$graphServicePrincipal = $graphServicePrincipal[0]

$resolvedRoles = foreach ($permission in $requiredPermissions) {
    $matches = @(
        $graphServicePrincipal.AppRoles | Where-Object {
            $_.Value -eq $permission -and
            $_.IsEnabled -and
            $_.AllowedMemberTypes -contains "Application"
        }
    )

    if ($matches.Count -ne 1) {
        throw "Could not uniquely resolve the Microsoft Graph application permission '$permission'."
    }

    $matches[0]
}

$requiredResourceAccess = @(
    foreach ($resource in $application.RequiredResourceAccess) {
        if ($resource.ResourceAppId -ne $graphAppId) {
            @{
                ResourceAppId  = $resource.ResourceAppId
                ResourceAccess = @(
                    foreach ($access in $resource.ResourceAccess) {
                        @{ Id = $access.Id; Type = $access.Type }
                    }
                )
            }
        }
    }
)

$existingGraphAccess = @(
    $application.RequiredResourceAccess |
        Where-Object ResourceAppId -eq $graphAppId |
        ForEach-Object ResourceAccess
)
$graphAccessByKey = @{}
foreach ($access in $existingGraphAccess) {
    $graphAccessByKey["$($access.Id):$($access.Type)"] = @{
        Id   = $access.Id
        Type = $access.Type
    }
}
foreach ($role in $resolvedRoles) {
    $graphAccessByKey["$($role.Id):Role"] = @{
        Id   = $role.Id
        Type = "Role"
    }
}

$requiredResourceAccess += @{
    ResourceAppId  = $graphAppId
    ResourceAccess = @($graphAccessByKey.Values)
}

if ($PSCmdlet.ShouldProcess(
        "$($application.DisplayName) ($ClientId)",
        "Add requested Microsoft Graph application permissions"
    )) {
    Update-MgApplication `
        -ApplicationId $application.Id `
        -RequiredResourceAccess $requiredResourceAccess
}

$existingAssignments = @(
    Get-MgServicePrincipalAppRoleAssignment `
        -ServicePrincipalId $clientServicePrincipal.Id `
        -All
)
$existingRoleIds = @(
    $existingAssignments |
        Where-Object ResourceId -eq $graphServicePrincipal.Id |
        ForEach-Object { "$($_.AppRoleId)" }
)

foreach ($role in $resolvedRoles) {
    if ("$($role.Id)" -in $existingRoleIds) {
        Write-Host "Already granted: $($role.Value)"
        continue
    }

    if ($PSCmdlet.ShouldProcess(
            "$($clientServicePrincipal.DisplayName) ($ClientId)",
            "Grant admin consent for Microsoft Graph application permission $($role.Value)"
        )) {
        $body = @{
            PrincipalId = $clientServicePrincipal.Id
            ResourceId  = $graphServicePrincipal.Id
            AppRoleId    = $role.Id
        }

        New-MgServicePrincipalAppRoleAssignment `
            -ServicePrincipalId $clientServicePrincipal.Id `
            -BodyParameter $body | Out-Null
        Write-Host "Granted: $($role.Value)"
    }
}

$grantedRoleIds = @(
    Get-MgServicePrincipalAppRoleAssignment `
        -ServicePrincipalId $clientServicePrincipal.Id `
        -All |
        Where-Object ResourceId -eq $graphServicePrincipal.Id |
        ForEach-Object { "$($_.AppRoleId)" }
)

$results = foreach ($role in $resolvedRoles) {
    [pscustomobject]@{
        Permission = $role.Value
        Required   = $true
        Consented  = ("$($role.Id)" -in $grantedRoleIds)
    }
}

$results | Sort-Object Permission | Format-Table -AutoSize

if ($results.Consented -contains $false -and -not $WhatIfPreference) {
    throw "One or more permissions were not granted."
}
}
finally {
    Stop-Transcript
}
