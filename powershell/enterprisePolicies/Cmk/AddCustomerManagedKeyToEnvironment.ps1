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

# Load thescript
. "$PSScriptRoot\..\Common\EnvironmentEnterprisePolicyOperations.ps1"

if (![bool]$endpoint) {
	$endpoint = "prod"
}

LinkPolicyToEnv -policyType cmk -environmentId $environmentId -policyArmId $policyArmId  -endpoint $endpoint  
