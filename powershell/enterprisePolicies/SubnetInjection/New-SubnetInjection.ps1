<#
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS. MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
NO TECHNICAL SUPPORT IS PROVIDED. YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
#>

param(
    [Parameter(Mandatory)]
    [ValidateNotNullOrEmpty()]
    [String]$EnvironmentId,

    [Parameter(Mandatory)]
    [ValidateNotNullOrEmpty()]
    [String]$PolicyArmId,

    [Parameter(Mandatory=$false)]
    [ValidateSet("tip1", "tip2", "prod")]
    [String]$Endpoint = "prod"
)

$ErrorActionPreference = "Stop"

Import-Module "$PSScriptRoot\..\Common\EnterprisePolicies" -Force

if (-not(Connect-Azure))
{
    return
}

LinkPolicyToEnv -policyType vnet -environmentId $EnvironmentId -policyArmId $PolicyArmId -endpoint $Endpoint 