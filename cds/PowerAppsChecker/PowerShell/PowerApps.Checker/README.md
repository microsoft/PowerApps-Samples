# PowerApps.Checker sample PowerShell module

## What is it?
This sample demonstrates how to interact with the *PowerApps checker* 
service. This service is used by the [*solution checker*](https://docs.microsoft.com/powerapps/maker/common-data-service/use-powerapps-checker) 
feature in the [*PowerApps maker portal*](https://docs.microsoft.com/powerapps/maker/canvas-apps/intro-maker-portal) along with the tooling for ISV and Dynamics 365 package
certifications. This sample PowerShell module, *PowerApps.Checker*, is a script 
module that provides an example of interacting with this service. The logic is 
contained in the psm1 file making it readily available for viewing.

## Version restrictions
The service supports solutions built for Microsoft CRM 2011 through the current 
versions of Microsoft Dynamics 365 CE and the Common Data Service. There is no
direct tie to an online organization, which allows for supporting solutions that target
online and/or on premises organizations.

## How to install
1. Download or clone the repo so that you have a local copy
2. You can install the module manually at this location, however, you will 
need to install it manually each time you load a new PowerShell host. A longer term
option would be to copy the folder to one of those included in the $env:PSModulePath 
value. $env:PSModulePath is a semicolon delimited set of paths in which PowerShell 
searches for modules. You can view them easily by running the following PowerShell command.
	```script
	$env:PSModulePath -split ";"
	```
3. If you chose not to copy it to one of the paths in $env:PSModulePath,
you will need to run the command below.

	```script
	Install-Module [Path to the folder]\PowerApps.Checker
	```

	If you did copy it to one of those well known paths, then it may auto install when used, 
	depending on your PowerShell configuration or you can manually install it with the method above. 
    One difference is that the full path is not needed, just the folder name, which is the name of the module.

	```script
	Install-Module PowerApps.Checker
	```
	
	You can read more [about module auto-loading in the PowerShell help](https://docs.microsoft.com/powershell/module/microsoft.powershell.core/about/about_modules?view=powershell-6#module-auto-loading).

## Security prerequisites

### Azure Active Directory application
In order to interact with the service's APIs, you will an Azure 
Active Directory (AAD) Application registered in a tenant that has PowerApps 
or Dynamics 365 licensing. Customers and partners who have licensing for the 
platform, regardless of on premises or online, should have an AAD tenant that has 
the necessary licensing. The APIs support tokens acquired as an application login by
providing a secret or a certification or acquired from an interactive user login. 
The primary intention of this module is to be used within a background process, such as an 
automated build or deployment. It is recommended that you use this sample
as the starting point of establishing the required AAD Application in your tenant. You can use the 
New-PowerAppsCheckerAzureADApplication function to begin the setup of the application user. 
An example usage is as follows:

```script
New-PowerAppsCheckerAzureADApplication -ApplicationDisplayName "PowerApps Checker Client - Application Via Script" -TenantId $tenantId
```
The TenantId parameter is for the tenant in which to create the application.  This sets up 
the starting point for the application, however, you will need to manually add a secret or 
certificate to be used during authentication for a background process to leverage.
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
specify each rule. For example, the solution checker feature uses a ruleset named
*Solution Checker*. As new rules are added or removed, the feature will include these
changes automatically without requiring any change by the consuming application. If 
you require that the list of rules not change automatically as described above, then 
the rules can be specified individually.

## Why specify a geography?
The PowerApps checker service temporarily stores the data that you upload in
Azure along with the reports that are generated. By specifying a geography, you
are controlling where the data is being stored. The *geography* parameter
is setup to provide a list of the supported geographies. If no geography is
specified, then United States is the default.
**NOTE** - the service is restricted to specific cloud geographies, which are 
listed in the parameter's validation set.

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

```script
$rulesets = Get-PowerAppsCheckerRulesets
```

### Get-PowerAppsCheckerRules
This function can be used to obtain the list of rules available, but the most common usage
is to obtain a list of rules included in a ruleset. No parameters are necessary,
but there are a few optional. There are two noteworthy parameters available:

1. LocaleName can be provided signaling the language that you would like the
results to be listed, e.g.-es. The languages that are currently supported are
included in the validation set of the parameter.
2. IncludeMessageFormats is a switch parameter that can be provided to signal inclusion
of all of the issue message variations available for the resulting rules.

```script
$rules = Get-PowerAppsCheckerRules -RulesetId $checkerRuleId -LocaleName es
```

### Invoke-PowerAppsChecker
This function is the most important provided in the sample. It makes multiple web API calls
and can include the following flow:

1. Uploads any file provided to in the FileUnderAnalysis (see [restrictions](#current-limitations) below)
2. Initiates the analysis job
3. Monitors for status updates
4. When finished or finished with errors downloads the report

The time required to complete this flow will vary due to the size and complexity 
of the provided customizations. It can be as quick as two minutes or as long as six 
or seven minutes. Usually, the end to end processing completes within a five minutes.

#### Expected return
|Property name|description|
|--|--|
|HighIssueCount|Number of issues that should be considered as highly recommended changes|
|MediumIssueCount|Number of issues that should be considered as recommended changes|
|LowIssueCount|Number of issues that should be considered as changes that would be nice to have or are limited risk of causing problems in an environment|
|Status|Failed, Finished, or FinishedWithErrors where Failed means that something terminitating happened during processing. Finished means that there were no issues running the analysis. FinishedWithErrors means that there were one or more rules that failed to complete successfully. This usually indicates that the report delivered is a partially complete one. A future run may report more issues once the problem with the rule is addressed. These errors are actively monitored in the service.|
|DownloadedResultFiles|Location of the file(s) that was/were downloaded locally|
|ResultFileUris|Location of the file(s) in the service's blob storage|

#### Report format
Reports are delivered in a ZIP compressed format. The report is a JSON formatted file using the SARIF standard. We are currently using version 2.0.
Refer to the [SARIF website](https://sarifweb.azurewebsites.net/) for viewing tools and references to SDKs for parsing and manipulation.

## Current limitations
- The size of the file passed to the FileUnderAnalysis parameter is restricted to less than 30 MBs.
Larger files may be processed, however, they must be uploaded to a storage mechanism available
to the service over the internet, such as Azure blob storage.
- While the service is deployed worldwide, there are limitations to the current
data center support (e.g. - not available in government clouds)