class App {
	[bool]$IsCommonlyUsedApp
	[string]$ApplicationName
	[string]$AppId
}

# 1st party app list from:
# https://learn.microsoft.com/troubleshoot/azure/active-directory/verify-first-party-apps-sign-in#application-ids-of-commonly-used-microsoft-applications
function Get-FirstPartyAppList{
	@(
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='ACOM Azure Website';AppId='23523755-3a2b-41ca-9315-f81f3f566a95'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='AEM-DualAuth';AppId='69893ee3-dd10-4b1c-832d-4870354be3d8'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='App Service';AppId='7ab7862c-4c57-491e-8a45-d52a7e023983'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='ASM Campaign Servicing';AppId='0cb7b9ec-5336-483b-bc31-b15b5788de71'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Azure Advanced Threat Protection';AppId='7b7531ad-5926-4f2d-8a1d-38495ad33e17'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Azure Data Lake';AppId='e9f49c6b-5ce5-44c8-925d-015017e9f7ad'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Azure Lab Services Portal';AppId='835b2a73-6e10-4aa5-a979-21dfda45231c'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Azure Portal';AppId='c44b4083-3bb0-49c1-b47d-974e53cbdf3c'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='AzureSupportCenter';AppId='37182072-3c9c-4f6a-a4b3-b3f91cacffce'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Bing';AppId='9ea1ad79-fdb6-4f9a-8bc3-2b70f96e34c7'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='ContactsInferencingEmailProcessor';AppId='20a11fe0-faa8-4df5-baf2-f965f8f9972e'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='CPIM Service';AppId='bb2a2e3a-c5e7-4f0a-88e0-8e01fd3fc1f4'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='CRM Power BI Integration';AppId='e64aa8bc-8eb4-40e2-898b-cf261a25954f'},
		[App]@{IsCommonlyUsedApp=$true;ApplicationName='Dataverse';AppId='00000007-0000-0000-c000-000000000000'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Enterprise Roaming and Backup';AppId='60c8bde5-3167-4f92-8fdb-059f6176dc0f'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Exchange Admin Center';AppId='497effe9-df71-4043-a8bb-14cf78c4b63b'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='FindTime';AppId='f5eaa862-7f08-448c-9c4e-f4047d4d4521'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Focused Inbox';AppId='b669c6ea-1adf-453f-b8bc-6d526592b419'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='GroupsRemoteApiRestClient';AppId='c35cb2ba-f88b-4d15-aa9d-37bd443522e1'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='HxService';AppId='d9b8ec3a-1e4e-4e08-b3c2-5baf00c0fcb0'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='IAM Supportability';AppId='a57aca87-cbc0-4f3c-8b9e-dc095fdc8978'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='IrisSelectionFrontDoor';AppId='16aeb910-ce68-41d1-9ac3-9e1673ac9575'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='MCAPI Authorization Prod';AppId='d73f4b35-55c9-48c7-8b10-651f6f2acb2e'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Media Analysis and Transformation Service';AppId='944f0bd1-117b-4b1c-af26-804ed95e767e'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Media Analysis and Transformation Service #2';AppId='0cd196ee-71bf-4fd6-a57c-b491ffd4fb1e'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft 365 Support Service';AppId='ee272b19-4411-433f-8f28-5c13cb6fd407'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft App Access Panel';AppId='0000000c-0000-0000-c000-000000000000'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Approval Management';AppId='65d91a3d-ab74-42e6-8a2f-0add61688c74'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Approval Management #2';AppId='38049638-cc2c-4cde-abe4-4479d721ed44'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Authentication Broker';AppId='29d9ed98-a469-4536-ade2-f981bc1d605e'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Azure CLI';AppId='04b07795-8ddb-461a-bbee-02f9e1bf7b46'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Azure PowerShell';AppId='1950a258-227b-4e31-a9cf-717495945fc2'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='MicrosoftAzureActiveAuthn';AppId='0000001a-0000-0000-c000-000000000000'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Bing Search';AppId='cf36b471-5b44-428c-9ce7-313bf84528de'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Bing Search for Microsoft Edge';AppId='2d7f3606-b07d-41d1-b9d2-0d0c9296a6e8'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Bing Default Search Engine';AppId='1786c5ed-9644-47b2-8aa0-7201292175b6'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Defender for Cloud Apps';AppId='3090ab82-f1c1-4cdf-af2c-5d7a6f3e2cc7'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Docs';AppId='18fbca16-2224-45f6-85b0-f7bf2b39b3f3'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Dynamics ERP';AppId='00000015-0000-0000-c000-000000000000'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Edge Insider Addons Prod';AppId='6253bca8-faf2-4587-8f2f-b056d80998a7'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Exchange ForwardSync';AppId='99b904fd-a1fe-455c-b86c-2f9fb1da7687'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Exchange Online Protection';AppId='00000007-0000-0ff1-ce00-000000000000'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Exchange ProtectedServiceHost';AppId='51be292c-a17e-4f17-9a7e-4b661fb16dd2'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Exchange REST API Based Powershell';AppId='fb78d390-0c51-40cd-8e17-fdbfab77341b'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Forms';AppId='c9a559d2-7aab-4f13-a6ed-e7e9c52aec87'},
		[App]@{IsCommonlyUsedApp=$true;ApplicationName='Microsoft Graph';AppId='00000003-0000-0000-c000-000000000000'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Intune Web Company Portal';AppId='74bcdadc-2fdc-4bb3-8459-76d06952a0e9'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Intune Windows Agent';AppId='fc0f3af4-6835-4174-b806-f7db311fd2f3'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Office';AppId='d3590ed6-52b3-4102-aeff-aad2292ab01c'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Office 365 Portal';AppId='00000006-0000-0ff1-ce00-000000000000'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Office Web Apps Service';AppId='67e3df25-268a-4324-a550-0de1c7f97287'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Online Syndication Partner Portal';AppId='d176f6e7-38e5-40c9-8a78-3998aab820e7'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft password reset service';AppId='93625bc8-bfe2-437a-97e0-3d0060024faa'},
		[App]@{IsCommonlyUsedApp=$true;ApplicationName='Microsoft Power BI';AppId='871c010f-5e61-4fb1-83ac-98610a7e9110'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Storefronts';AppId='28b567f6-162c-4f54-99a0-6887f387bbcc'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Stream Portal';AppId='cf53fce8-def6-4aeb-8d30-b158e7b1cf83'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Substrate Management';AppId='98db8bd6-0cc0-4e67-9de5-f187f1cd1b41'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Support';AppId='fdf9885b-dd37-42bf-82e5-c3129ef5a302'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Teams';AppId='1fec8e78-bce4-4aaf-ab1b-5451cc387264'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Teams Services';AppId='cc15fd57-2c6c-4117-a88c-83b1d56b4bbe'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Teams Web Client';AppId='5e3ce6c0-2b1f-4285-8d4b-75ee78787346'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Microsoft Whiteboard Services';AppId='95de633a-083e-42f5-b444-a4295d8e9314'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='O365 SkypeSpaces Ingestion Service';AppId='dfe74da8-9279-44ec-8fb2-2aed9e1c73d0'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='O365 Suite UX';AppId='4345a7b9-9a63-4910-a426-35363201d503'},
		[App]@{IsCommonlyUsedApp=$true;ApplicationName='Office 365 Exchange Online';AppId='00000002-0000-0ff1-ce00-000000000000'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Office 365 Management';AppId='00b41c95-dab0-4487-9791-b9d2c32c80f2'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Office 365 Search Service';AppId='66a88757-258c-4c72-893c-3e8bed4d6899'},
		[App]@{IsCommonlyUsedApp=$true;ApplicationName='Office 365 SharePoint Online';AppId='00000003-0000-0ff1-ce00-000000000000'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Office Delve';AppId='94c63fef-13a3-47bc-8074-75af8c65887a'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Office Online Add-in SSO';AppId='93d53678-613d-4013-afc1-62e9e444a0a5'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Office Online Client AAD- Augmentation Loop';AppId='2abdc806-e091-4495-9b10-b04d93c3f040'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Office Online Client AAD- Loki';AppId='b23dd4db-9142-4734-867f-3577f640ad0c'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Office Online Client AAD- Maker';AppId='17d5e35f-655b-4fb0-8ae6-86356e9a49f5'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Office Online Client MSA- Loki';AppId='b6e69c34-5f1f-4c34-8cdf-7fea120b8670'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Office Online Core SSO';AppId='243c63a3-247d-41c5-9d83-7788c43f1c43'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Office Online Search';AppId='a9b49b65-0a12-430b-9540-c80b3332c127'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Office.com';AppId='4b233688-031c-404b-9a80-a4f3f2351f90'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Office365 Shell WCSS-Client';AppId='89bee1f7-5e6e-4d8a-9f3d-ecd601259da7'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='OfficeClientService';AppId='0f698dd4-f011-4d23-a33e-b36416dcb1e6'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='OfficeHome';AppId='4765445b-32c6-49b0-83e6-1d93765276ca'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='OfficeShredderWacClient';AppId='4d5c2d63-cf83-4365-853c-925fd1a64357'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='OMSOctopiPROD';AppId='62256cef-54c0-4cb4-bcac-4c67989bdc40'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='OneDrive SyncEngine';AppId='ab9b8c07-8f02-4f72-87fa-80105867a763'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='OneNote';AppId='2d4d3d8e-2be3-4bef-9f87-7875a61c29de'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Outlook Mobile';AppId='27922004-5251-4030-b22d-91ecd9a37ea4'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Partner Customer Delegated Admin Offline Processor';AppId='a3475900-ccec-4a69-98f5-a65cd5dc5306'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Password Breach Authenticator';AppId='bdd48c81-3a58-4ea9-849c-ebea7f6b6360'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='PeoplePredictions';AppId='35d54a08-36c9-4847-9018-93934c62740c'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Power BI Service';AppId='00000009-0000-0000-c000-000000000000'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Scheduling';AppId='ae8e128e-080f-4086-b0e3-4c19301ada69'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='SharedWithMe';AppId='ffcb16e8-f789-467c-8ce9-f826a080d987'},
		[App]@{IsCommonlyUsedApp=$true;ApplicationName='SharePoint Online Web Client Extensibility';AppId='08e18876-6177-487e-b8b5-cf950c1e598c'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Signup';AppId='b4bddae8-ab25-483e-8670-df09b9f1d0ea'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Skype for Business Online';AppId='00000004-0000-0ff1-ce00-000000000000'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='SpoolsProvisioning';AppId='61109738-7d2b-4a0b-9fe3-660b1ff83505'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Sticky Notes API';AppId='91ca2ca5-3b3e-41dd-ab65-809fa3dffffa'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Substrate Context Service';AppId='13937bba-652e-4c46-b222-3003f4d1ff97'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='SubstrateDirectoryEventProcessor';AppId='26abc9a8-24f0-4b11-8234-e86ede698878'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Substrate Search Settings Management Service';AppId='a970bac6-63fe-4ec5-8884-8536862c42d4'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Sway';AppId='905fcf26-4eb7-48a0-9ff0-8dcc7194b5ba'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Transcript Ingestion';AppId='97cb1f73-50df-47d1-8fb0-0271f2728514'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Universal Store Native Client';AppId='268761a2-03f3-40df-8a8b-c3db24145b6b'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Viva Engage (formerly Yammer)';AppId='00000005-0000-0ff1-ce00-000000000000'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='WeveEngine';AppId='3c896ded-22c5-450f-91f6-3d1ef0848f6e'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Windows Azure Active Directory';AppId='00000002-0000-0000-c000-000000000000'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Windows Azure Security Resource Provider';AppId='8edd93e1-2103-40b4-bd70-6e34e586362d'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Windows Azure Service Management API';AppId='797f4846-ba00-4fd7-ba43-dac1f8f63013'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='WindowsDefenderATP Portal';AppId='a3b79187-70b2-4139-83f9-6016c58cd27b'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Windows Search';AppId='26a7ee05-5602-4d76-a7ba-eae8b7b67941'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Windows Spotlight';AppId='1b3c667f-cde3-4090-b60b-3d2abd0117f0'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Windows Store for Business';AppId='45a330b1-b1ec-4cc1-9161-9f03992aa49f'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Yammer Web';AppId='c1c74fed-04c9-4704-80dc-9f79a2e515cb'},
		[App]@{IsCommonlyUsedApp=$false;ApplicationName='Yammer Web Embed';AppId='e1ef36fd-b883-4dbf-97f0-9ece4b576fc6'},
  		[App]@{IsCommonlyUsedApp=$true;ApplicationName='Azure DevOps';AppId='499b84ac-1321-427f-aa17-267ca6975798'},
		[App]@{IsCommonlyUsedApp=$true;ApplicationName='Azure Key Vault';AppId='cfa8b339-82a2-471a-a3c9-0fc0be7a4093'},
		[App]@{IsCommonlyUsedApp=$true;ApplicationName='Azure Storage';AppId='e406a681-f3d4-42a8-90b6-c2b029497af1'}
	) | Sort-Object -Property ApplicationName
}

Write-Host "########################################################"
Write-Host "# 'HTTP with Microsoft Entra ID' connector - Permission grant configuration"
Write-Host "# This script will guide you through the process of granting the required permissions"
Write-Host "# to the HttpWithAADApp Microsoft 1st party app 'ServiceApp_NoPreAuths' to access the selected resources."
Write-Host "########################################################"

Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
Set-PSRepository -Name 'PSGallery' -InstallationPolicy Trusted
Install-Module Microsoft.Graph -Scope CurrentUser -WarningAction Ignore
Import-Module Microsoft.Graph.Applications
Import-Module Microsoft.Graph.Identity.SignIns
$ErrorActionPreference = "Stop"

Disconnect-Graph -ErrorAction Ignore

if($Host.UI.PromptForChoice("Cloud selection", "Most customers access to the Global Azure environment. Do you want to connect using azure global or do you want to select from a list?", ('&Azure Global (recommended)', '&Select from a list (advanced)'), 0) -eq 0)
{
	$selectedEnvName = "Global"
}
else 
{
	$selectedEnv = Get-MgEnvironment | Out-GridView -Title "Choose Cloud Environment" -OutputMode Single

	If (!$selectedEnv)
	{
		Write-Warning "No environment selected. Please select an environment and try again."
		Exit
	}

	$selectedEnvName = $selectedEnv.Name
}

Connect-MgGraph -Environment $selectedEnvName -Scopes "User.ReadWrite.All Directory.AccessAsUser.All" -NoWelcome

# Find Service principal in the local tenant associated to the HttpWithAADApp Microsoft 1st party app
$HttpWithAADAppAppId = 'd2ebd3a9-1ada-4480-8b2d-eac162716601'
$HttpWithAADAppServicePrincipal = Get-MgServicePrincipal -Filter "appId eq '$HttpWithAADAppAppId'"

If (!$HttpWithAADAppServicePrincipal)
{
	Write-Host "No service principal was found in the current tenant with appId: $HttpWithAADAppAppId. Attempting to create one."
 	$AppIDForSpCreation=@{
   		"AppId" = "$HttpWithAADAppAppId"
   	}

	$HttpWithAADAppServicePrincipal = New-MgServicePrincipal -BodyParameter $AppIDForSpCreation

 	If (!$HttpWithAADAppServicePrincipal)
	{
		Write-Warning "Not able to create a service principal for appId : $HttpWithAADAppAppId."
		Exit
  	}
}

$HttpWithAADAppServicePrincipalId = $HttpWithAADAppServicePrincipal.Id
$HttpWithAADAppServicePrincipalDisplayName = $HttpWithAADAppServicePrincipal.DisplayName

Write-Host "HttpWithAADApp Service principal was found:"
$HttpWithAADAppServicePrincipal | Format-Table -wrap -auto

# Select 1st party app for scope selection
if($Host.UI.PromptForChoice("Resource and scope selection", "Most customers access to widely used resources (e.g. Graph, Sharepoint, Dataverse, etc.). Do you want to display only the commonly used apps?", ('&Commonly used Apps', '&All apps (advanced)'), 0) -eq 0)
{
	$filteredFirstPartyAppList = Get-FirstPartyAppList | Where-Object {$_.IsCommonlyUsedApp -eq $true}
}
else 
{
	$filteredFirstPartyAppList = Get-FirstPartyAppList
}

$selectedApp = $filteredFirstPartyAppList | Select-Object ApplicationName, AppId | Out-GridView -Title  "Choose 1st party app for resource and scope selection" -OutputMode Single

If (!$selectedApp)
{
	Write-Warning "No app selected. Please select an app and try again."
	Exit
}

Write-Host "The app was selected:"
$selectedApp | Format-Table -wrap -auto
$selectedAppId = $selectedApp.AppId

# Select scopes for the 1st party app selected
# Find SP associated to the selected app
$selectedSP = Get-MgServicePrincipal -Filter "appId eq '$selectedAppId'"

If (!$selectedSP)
{
	Write-Warning "No service principal found in the current tenant with appId: $selectedAppId"
	Exit
}

$selectedSPId = $selectedSP.Id

# List of Admin and User Scopes
$scopes = $selectedSP.Oauth2PermissionScopes | Sort-Object Value | Select-Object Type, Value, UserConsentDisplayName, UserConsentDescription
$selectedScopes = $scopes | Out-GridView -Title "Choose Scopes" -OutputMode Multiple

$joinedScopes = $selectedScopes | Join-String -Property {$_.Value} -Separator ' '
Write-Host "The following user scopes have been selected: $joinedScopes"

If (!$selectedScopes)
{
	Write-Warning "No scopes selected. Please select at least one and try again."
	Exit
}

# Select a consent type (AllPrincipals vs Principal)
if($Host.UI.PromptForChoice("Select consent type", "Do you want the service principal '$HttpWithAADAppServicePrincipalDisplayName' ($HttpWithAADAppServicePrincipalId) to be able to impersonate all users?", ('&Yes', '&No (I need to select a specific user)'), 0) -eq 0)
{
	$grantParams = @{
		clientId = $HttpWithAADAppServicePrincipalId
		consentType = "AllPrincipals"
		resourceId = $selectedSPId
		scope = $joinedScopes
	}
}
else 
{
	# let the user select a specific principal
	$users = Get-MgUser -All | Select-Object ID, DisplayName, Mail, UserPrincipalName
	$selectedUser = $users | Out-GridView -Title "Choose a user" -OutputMode Single

	$grantParams = @{
		clientId = $HttpWithAADAppServicePrincipalId
		consentType = "Principal"
		principalId = $selectedUser.Id
		resourceId = $selectedSPId
		scope = $joinedScopes
	}
}

# Display current grants for the service principal and resource
$existingOauth2PermissionGrant = Get-MgOauth2PermissionGrant -Filter "clientId eq '$HttpWithAADAppServicePrincipalId' and resourceId eq '$selectedSPId'"

if($existingOauth2PermissionGrant)
{
	Write-Host "The service principal '$HttpWithAADAppServicePrincipalDisplayName' ($HttpWithAADAppServicePrincipalId) has the following oAuth2PermissionGrant objects already defined for resourceId '$selectedSPId':"
	$existingOauth2PermissionGrant | Format-Table -wrap -auto

	# allow deletion of existing grants
	if($Host.UI.PromptForChoice("Grant deletion", "Do you want to delete any of the existing grants?", ('&No', '&Yes, I want to first delete existing grants'), 0) -eq 1)
	{
		# deletion flow
		$selectedGrantsToDelete = $existingOauth2PermissionGrant | Out-GridView -Title "Select the grants you want to delete" -OutputMode Multiple

		Write-Host "The following grants are going to be deleted:"
		$selectedGrantsToDelete | Format-Table -wrap -auto
		$selectedGrantsToDelete | ForEach-Object { Remove-MgOauth2PermissionGrant -OAuth2PermissionGrantId $_.Id }
	}
}
else
{
	Write-Host "No existing oAuth2PermissionGrant object were found for service principal '$HttpWithAADAppServicePrincipalDisplayName' ($HttpWithAADAppServicePrincipalId) and resourceId '$selectedSPId'"
}

Write-Host "The following grant is going to be persisted:"
$grantParams | Format-Table -wrap -auto

# Create/Update a delegated permission grant represented by an oAuth2PermissionGrant object (delete existing one if any)
if ($grantParams.consentType -eq "AllPrincipals")
{
	$existingOauth2PermissionGrant = Get-MgOauth2PermissionGrant -Filter "clientId eq '$HttpWithAADAppServicePrincipalId' and resourceId eq '$selectedSPId' and consentType eq 'AllPrincipals'"
	
	if($existingOauth2PermissionGrant)
	{
		Write-Warning "An existing oAuth2PermissionGrant object was found with the same key properties. (clientId: $HttpWithAADAppServicePrincipalId, resourceId: $selectedSPId, consentType: AllPrincipals)"
	}
}
elseif ($grantParams.consentType -eq "Principal")
{
	$grantParamsPrincipalId = $grantParams.principalId
	$existingOauth2PermissionGrant = Get-MgOauth2PermissionGrant -Filter "clientId eq '$HttpWithAADAppServicePrincipalId' and resourceId eq '$selectedSPId' and consentType eq 'Principal'" | Where-Object { $_.PrincipalId -eq $grantParamsPrincipalId }
	
	if($existingOauth2PermissionGrant)
	{
		Write-Warning "An existing oAuth2PermissionGrant object was found with the same key properties. (clientId: $HttpWithAADAppServicePrincipalId, resourceId: $selectedSPId, consentType: Principal, principalId: $grantParamsPrincipalId)"
	}
}

if($existingOauth2PermissionGrant)
{
	Write-Warning "This means that the existing oAuth2PermissionGrant object is about to be updated with the new parameters provided."
	Write-Warning "Existing permission grant:"
	$existingOauth2PermissionGrant | Format-Table -wrap -auto
	Write-Warning "New permission grant requested:"
	$grantParams | Format-Table -wrap -auto

	if($Host.UI.PromptForChoice("Confirm permission grant update", "Do you want to proceed and update the above permission grant?", ('&Yes', '&No'), 0) -eq 1)
	{
		Write-Warning "Execution terminated."
		Exit
	}

	Update-MgOauth2PermissionGrant -OAuth2PermissionGrantId $existingOauth2PermissionGrant.Id -BodyParameter $grantParams
}
else
{
	if($Host.UI.PromptForChoice("Confirm permission grant creation", "Do you want to proceed and create the permission grant?", ('&Yes', '&No'), 0) -eq 1)
	{
		Write-Warning "Execution terminated."
		Exit
	}

	New-MgOauth2PermissionGrant -BodyParameter $grantParams
}

Write-Host "A delegated permission grant was persisted with the following parameters:"
$grantParams | Format-Table -wrap -auto

Disconnect-MgGraph
Write-Host "Script execution completed successfully"
