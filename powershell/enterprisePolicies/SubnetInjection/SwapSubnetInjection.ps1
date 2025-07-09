# Load thescript
. "$PSScriptRoot\..\Common\EnvironmentEnterprisePolicyOperations.ps1"

function SwapSubnetInjection 
{
    param(
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]$environmentId,

        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]$newPolicyArmId,

        [Parameter(Mandatory=$false)]
        [ValidateSet("tip1", "tip2", "prod", "usgovhigh", "dod", "china")]
        [String]$endpoint

    )
    
    if (![bool]$endpoint) {
        $endpoint = "prod"
    }
    SwapPolicyForEnv -policyType vnet -environmentId $environmentId -policyArmId $newPolicyArmId  -endpoint $endpoint  
}
SwapSubnetInjection