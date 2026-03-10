# Power Platform Data Loss Prevention (DLP) SDK 

This topic introduces the capabilities of the DLP SDKs and shows you how DLP can help you manage your tenant and environment policy with experiences ranging from creating, reading, updating to removing DLP policy. More information : [https://learn.microsoft.com/power-platform/admin/wp-data-loss-prevention]([https://learn.microsoft.com/power-platform/admin/wp-data-loss-prevention).

## What this sample does

This sample calls DLP APIs in Microsoft.PowerApps.Administration.PowerShell to create, read, update and remove DLP policy.

## How this sample works

Sample provides some DLP scenarios about how to call DLP APIs for your reference. You can run the sample and see the result.

## How to run this sample

1. Install the latest package (your powershell should be run as Admin): <br/>
   `Install-Module -Name Microsoft.PowerApps.Administration.PowerShell -Force`
2. Edit RunSamples.ps1 and make the following changes:
   - Replace $TenantAdminName value to your tenant admin account
   - Replace $TenantAdminPassword value to your tenant admin account password
   - Replace $EnvironmentAdminName value to your environment admin account
   - Replace $EnvironmentAdminPassword value to your environment admin account password
3. RunSamples.ps1