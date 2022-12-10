# Power Platform Enterprise Policies PowerShell Scripts

These scripts automate managing (create, update, get, delete) Power Platform Enterprise Policies as Azure resources.</br>
In addition, we are providing sample scripts on how to associate these policies with Power Platform environments.

## How to run setup scripts

1. **Install modules script** : This script installs the required modules to run Enterprise Policies scripts.</br>
Script name: InstallPowerAppsCmdlets.ps1</br>
Run the script to import required PowerShell modules.

2. **Setup Azure subscription for Microsoft.PowerPlatform** : This script registers the Azure subscription for Microsoft.PowerPlatform resource provider </br>
and also allow lists the subscription for enterprisePoliciesPreview feature.</br>
Script name : SetupSubscriptionForPowerPlatform.ps1</br>
Run the script to setup Azure subscription for Microsoft.PowerPlatform

## How to run CMK scripts

The CMK scripts are present in folder Cmk at current location

1. **Create CMK Enterprise Policy** : This script creates a CMK enterprise policy</br>
Script name : CreateCMKEnterprisePolicy.ps1</br>
Input parameters :
    - subscriptionId : The subscriptionId where CMK enterprise policy needs to be created
    - resourceGroup : The resource group where CMK enterprise policy needs to be created
    - enterprisePolicyName : The name of the CMK enterprise policy resource
    - enterprisePolicyLocation : The Azure geo where CMK enterprise policy needs to be created. Example: unitedstates, europe, australia.</br>
      To get the complete supported locations for enterprise policy, below as command can be used:</br>
      ((Get-AzResourceProvider -ProviderNamespace Microsoft.PowerPlatform).ResourceTypes | Where-Object ResourceTypeName -eq enterprisePolicies).Locations
    - keyVaultId : The ARM resource ID of the key vault used for CMK
    - keyName : The name of the key in the key vault used for CMK
    - keyVersion: The version of the key in the key vault used for CMK

Sample Input :</br>
![alt text](./ReadMeImages/CreateCMKEP1.png)</br>

Sample Output : </br>
![alt text](./ReadMeImages/CreateCMKEP2.png)</br>

2. **Get CMK Enterprise Policy By ResourceId** : The script gets a CMK enterprise policy by ARM resourceId</br>
Script name : GetCMKEnterprisePolicyByResourceId.ps1</br>
Input parameter :
    - enterprisePolicyArmId : The ARM resource ID of the CMK Enterprise Policy

Sample Input :</br>
![alt text](./ReadMeImages/GetCMKByResourceId1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/GetCMKByResourceId2.png)</br>

3. **Get CMK Enterprise Policies in Subscription** : The script gets all CMK enterprise policies in an Azure subscription</br>
Script name : GetCMKEnterprisePoliciesInSubscription.ps1</br>
Input parameter :
    - subscriptionId: : The Azure subscription Id

Sample Input :</br>
![alt text](./ReadMeImages/GetCMKInSub1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/GetCMKInSub2.png)</br>

4. **Get CMK Enterprise Policies in Resource Group** : The script gets all CMK enterprise policies in an Azure resource group</br>
Script name : GetCMKEnterprisePoliciesInResourceGroup.ps1</br>
Input parameters :
    - subscriptionId : The Azure subscription Id
    - resourceGroup : The Azure resource group

Sample Input : </br>
![alt text](./ReadMeImages/GetCMKInResourceGroup1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/GetCMKInResourceGroup2.png)</br>

5. **Validate Azure Key Vault** : This script checks if the Key Vault is setup correctly according to the pre-requisites required by the Power Platform CMK Enterprise Policy</br>
    - Soft-delete : if not enabled, then a warning message is displayed (soft-delete is a read only property and can’t be fixed). Follow the instructions at
      https://docs.microsoft.com/en-us/azure/key-vault/general/soft-delete-change to update the soft delete property.
    - Purge protection - if not enabled, then enables it for the customer
    - Adds the access policies to the Key Vault with permission GET, UNWRAPKEY, WRAPKEY for the given enterprise policy</br>

Script name : ValidateKeyVaultForCMK.ps1</br>
Input parameters :
    - subscriptionId : The Azure subscription Id of the Key Vault
    - keyVaultName : The name of the key Vault
    - enterprisePolicyArmId : The CMK enterprise policy ARM Id which should have permission GET, UNWRAPKEY, WRAPKEY for the key vault

Sample Input : </br>
![alt text](./ReadMeImages/ValidateKeyVault1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/ValidateKeyVault2.png)</br>

6. **Update CMK Enterprise Policy** : This script updates a CMK Enterprise Policy. The updates allowed are for keyVaultId, keyName, keyVersion.</br>
If you are changing only some of the allowed parameter values, provide “N/A” when prompted for the parameters that you don’t want to change.</br>
 **If the enterprise policy is associated with one or more environments, the update operation will fail, and the script will return an error.**</br>
Script name : UpdateCMKEnterprisePolicy.ps1</br>
Input parameters :
    - subscriptionId : The Azure subscription Id of the CMK Enterprise Policy
    - resourceGroup : The Azure resource group of the CMK Enterprise Policy
    - enterprisePolicyName : The name of the CMK enterprise policy that needs to be updated
    - keyVaultId : The ARM resource ID of the key vault if it needs to be updated. Provide "N/A" if update is not required for key vault Id
    - keyName: The name of the key if it needs to be updated. Provide "N/A" if update is not required for name of the key
    - keyVersion: The version of the key if it needs to be updated. Provide "N/A" if update is not required for version of the key

Sample Input : </br>
![alt text](./ReadMeImages/UpdateCMKEP1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/UpdateCMKEP2.png)</br>

7. **Delete CMK Enterprise Policy** : This script deletes the CMK Enterprise Policy for a given policy Id. </br>
**If the CMK enterprise policy is associated with one or more environments, the delete operation will fail, and the script will return an error.**</br>
Script name : RemoveCMKEnterprisePolicy.ps1</br>
Input parameter :
    - policyArmId : The ARM ID of the CMK enterprise policy to be deleted

Sample Input : </br>
![alt text](./ReadMeImages/RemoveCMKEP1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/RemoveCMKEP2.png)</br>

8. **Set CMK for an environment** : This script applies a CMK enterprise policy to a given Power Platform environment.</br>
The script adds the environment to the enterprise policy and optionally polls for the operation outcome.</br>
Script name : AddCustomerManagedKeyToEnvironment.ps1</br>
Input parameters :
    - environmentId : The Power Platform environment ID
    - policyArmId : The ARM ID of the CMK Enterprise Policy

Sample Input :</br>
![alt text](./ReadMeImages/AddCMKToEnv1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/AddCMKToEnv2.png)</br>

9. **Get CMK for an environment** : This script returns the CMK enterprise policy if applied to a given Power Platform environment.</br>
Script name : GetCMKEnterprisePolicyForEnvironment.ps1</br>
Input parameter :
    - environmentId : The Power Platform environment ID

Sample Input :</br>
![alt text](./ReadMeImages/GetCMKForEnv1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/GetCMKForEnv2.png)</br>

10. **Remove CMK from an environment** : The script removes the CMK enterprise policy from an environment, </br>
which results on data to be encrypted with a Microsoft managed encryption key.</br>
Script name : RemoveCustomerManagedKeyFromEnvironment.ps1</br>
Input parameters :
    - environmentId : The Power Platform environment ID
    - policyArmId: The ARM ID of the CMK Enterprise Policy

Sample Input :</br>
![alt text](./ReadMeImages/RemoveCMKFromEnv1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/RemoveCMKFromEnv2.png)</br>
