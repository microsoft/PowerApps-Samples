# PowerApps.Checker sample PowerShell module

## What is it?
This sample demonstrates how to interact with the *PowerApps checker* 
services. This service is used by the *Solution checker* feature in the 
*PowerApps Maker Portal*. The included PowerShell module, 
*PowerApps.Checker*, is a sample script module and it contains what is 
needed to utilize. It includes a psm1 file that has the script logic.

## How to install
1. Download or clone the repo so that you have a local copy
2. You can install the module manually at this location, however, you will 
need to install it manually each time you need to run it or you can copy 
the folder to one of the folders in the $env:PSModulePath value. $env:PSModulePath
is a semicolon delimited set of paths in which PowerShell should look for modules.
You can view them easily by running the following PowerShell command.
	```script
	$env:PSModulePath -split ";"
	```
3. If you chose not to copy it to one of the paths in $env:PSModulePath,
you will need to run the command below.

	```script
	Install-Module [Path to the folder]\PowerApps.Checker
	```

	If you did copy it to one of those well known paths, then it may auto install when used 
	depending on your setup or you can manually install it with the method above. However,
	the full path is not needed, just the folder name, which is the name of the module.

	```script
	Install-Module PowerApps.Checker
	```
	
	You can read more [about module auto-loading in the PowerShell help](https://docs.microsoft.com/powershell/module/microsoft.powershell.core/about/about_modules?view=powershell-6#module-auto-loading).

## Security prerequisites

### Azure Active Directory application
In order to interact with the service APIs, you will need to have an Azure 
Active Directory (AAD) Application registered in your tenant that has PowerApps 
or Dynamics 365 licensing. Customers and partners who have licensing for the 
platform regardless of on premises or online will have an AAD tenant that has 
the necessary licensing. The APIs support tokens acquired as a service user or 
for an interactive as long. The primary intention of this module is to be used
from a background process, such as an automated build or deployment. It is recommended
that you use the sample module to create the AAD Application in your tenant. You can use 
the New-PowerAppsCheckerAzureADApplication function to begin the setup of the application user. 
An example usage is the following:

```script
New-PowerAppsCheckerAzureADApplication -ApplicationDisplayName "PowerApps Checker Client - Application Via Script" -TenantId $tenantId
```
The TenantId parameter is the tenant in which to install athe application.  This sets up 
the starting point for the application, however, you will need to manually add a secret or 
certificate in which to authenticate for a background process to leverage.
Aside from managing the credentials for the service, this should be a one-time
setup.

### TLS
All communications are to be performed using the TLS 1.2 standard. This is a strict requirement of the
service. In order to facility this in the sample, the following is set for each entry point.

```script
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12```
```

## Rulesets and rules
PowerApps checker requires a list of rules in which to run. This can be provided
in the form of individual rules or a grouping of rules, referred to as a ruleset.
A ruleset is a convenient way to specify a group of rules instead of having to
specify each rule. For example, the Solution checker feature uses a ruleset named
*Solution Checker*. As new rules are added or removed, the feature will include these
changes automatically without requiring a configuration change. If you require that
the list of rules not change automatically as described above, then the rules can
be specified individually.

## Why specify a geography?
The PowerApps checker service temporarily stores the data that you upload in
Azure along with the reports that are generated. By specifying a geography, you
are controlling where the data is being stored. The *geography* parameter
is setup to provide a list of the supported geographies. If no geography is
specified, then United States is the default.
**NOTE**-restricted to specific cloud geographies

## How to use
There are four functions included:
- New-PowerAppsCheckerAzureADApplication
- Get-PowerAppsCheckerRulesets
- Get-PowerAppsCheckerRules
- Invoke-PowerAppsChecker

Here is a sample of how these functions could be used in a background process.
They are described in more detail in the sections below.
```script
$rulesets = Get-PowerAppsCheckerRulesets
$checkerRuleId = ($rulesets | Where-Object { $_.name -eq "Solution Checker"}).id

$result = Invoke-PowerAppsChecker -TenantId '[Your tenant ID]' -ApplicationId '[Your application's ID]' -LocaleName es -RulesetId $checkerRuleId -ApplicationSecret '[Secret generated in AAD]' -OutputDirectory "E:\demo\out" -FileUnderAnalysis "E:\demo\MySampleSolution.zip" -Geography 'Preview United States'
```

### New-PowerAppsCheckerAzureADApplication
This function is likely only needed when preparing for running Invoke-PowerAppsChecker 
and is described in the [Security prerequisites section above](#security-prerequisites).

### Get-PowerAppsCheckerRulesets
This function can be used to obtain the list of rulesets currently supported. No parameters are necessary,
but there are a few optional.

### Get-PowerAppsCheckerRules
This function can be used to obtain the list of rules available, but the most common usage
is to obtain a list of rules contained within a rulesets. No parameters are necessary,
but there are a few optional. There are two noteworthy parameters available:

1. LocaleName can be provided signalling the language that you would like the
results to be listed, e.g.-es. The languages that are currently supported are
included in the validation set on the parameter.
2. IncludeMessageFormats is a switch parameter that can be provided to signal inclusion
all of the issue messages variations available for the rule in the results.

### Invoke-PowerAppsChecker
