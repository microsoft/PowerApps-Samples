param(
    [Parameter(Mandatory=$true)]
    [ValidateNotNullOrEmpty()]
    [String]$PolicyArmId,

    [Parameter(Mandatory=$false)]
    [BAPEndpoint]$Endpoint = "prod"
)

$ErrorActionPreference = "Stop"

Import-Module "$PSScriptRoot\..\Common\EnterprisePolicies" -Force

if (-not(Connect-Azure))
{
    return
}

New-PolicyToPlatformAppsDataLink -PolicyType [PolicyType]::Encryption -PolicyArmId $PolicyArmId -Endpoint $Endpoint
