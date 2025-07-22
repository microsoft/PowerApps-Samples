# Load thescript
. "$PSScriptRoot\..\Common\EnvironmentEnterprisePolicyOperations.ps1"


function RevertSubnetInjection 
{
    param(
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]$environmentId,

        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]$policyArmId,

        [Parameter(Mandatory=$false)]
        [ValidateSet("tip1", "tip2", "prod", "usgovhigh", "dod", "china")]
        [String]$endpoint

    )
    
    if (![bool]$endpoint) {
        $endpoint = "prod"
    }

    UnLinkPolicyFromEnv -policyType vnet -environmentId $environmentId -policyArmId $policyArmId -endpoint $endpoint 
}
RevertSubnetInjection