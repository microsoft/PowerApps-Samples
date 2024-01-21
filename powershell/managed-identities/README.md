# Managed identities for Azure PowerShell scripts to use for Azure Synapse Link for Dataverse

These scripts are used to configure Azure Synapse Link for Dataverse to use managed identities for Azure.
</br>
Please note that these scripts are provided under MIT license and its usage is the sole responsibility of the user.

## How to run these scripts

Follow the instructions in the article [Use managed identities for Azure with your Azure data lake storage (preview)](https://learn.microsoft.com/power-apps/maker/data-platform/azure-synapse-link-msi)

## What the scripts do

### SetupSubscriptionForPowerPlatform.ps1
This script registers the Azure subscription for Microsoft.PowerPlatform resource provider and also allow lists the subscription for the enterprise policies preview feature.

### CreateIdentityEnterprisePolicy.ps1
This script generates a Microsoft.PowerPlatform enterprise policy under a certain Azure subscription and resource group.

### NewIdentity.ps1
This script links or swaps the generated Microsoft.PowerPlatform enterprise policy to a certain Dataverse environment.

### GetIdentityEnterprisePolicyforEnvironment.ps1
This script outputs the linked Microsoft.PowerPlatform enterprise policy ID for a given Dataverse environment.

### RevertIdentity.ps1
This script unlinks a certain Microsoft.PowerPlatform enterprise policy for a given Dataverse environment.

### RemoveIdentityEnterprisePolicy.ps1
This script removes an unlinked Microsoft.PowerPlatform enterprise policy under a certain Azure subscription and resource group.
