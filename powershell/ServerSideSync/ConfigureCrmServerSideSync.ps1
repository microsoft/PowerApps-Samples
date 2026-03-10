# CRM on-premises with Exchange Online server-side synchronization setup  
# PowerShell script to upload private key to Azure.  
# This script has to be invoked from the server that has the deployment tool feature installed

Param(
    [Parameter(Mandatory=$True, HelpMessage="Enter password for Certificate private key.")]
    [SecureString]$privateKeyPassword,

    [Parameter(Mandatory=$True, HelpMessage="Enter path of Personal Information Exchange file.")]
    [string]$pfxFilePath,

    [Parameter(Mandatory=$true, HelpMessage="Enter organization name.")]
    [string]$organizationName,

    [Parameter(Mandatory=$true, HelpMessage="Enter Microsoft Entra ID TenantId Or Domain Name.")]
    [string]$microsoftEntraIdTenantIdOrDomainName,

    [Parameter(Mandatory=$true, HelpMessage="Enter Reverse Hybrid setup App Id.")]
    [string]$ClientID,

    [Parameter(Mandatory=$true, HelpMessage="Enter Reverse Hybrid setup App client secret")]
    [string]$ClientSecret
)

#region Functions
function Update-ServicePrincipal {
    param (
        [Parameter(Mandatory=$true)]
        [hashtable]$ServicePrincipal,

        [Parameter(Mandatory=$true)]
        [string]$operationName,

        [Parameter(Mandatory=$true)]
        [string]$uri,

        [Parameter(Mandatory=$true)]
        [hashtable]$headers
    )

    $jsonString = $ServicePrincipal | ConvertTo-Json

    try {
            $response = Invoke-WebRequest -Uri $uri -Method PATCH -Headers $headers -Body $jsonString -UseBasicParsing
        }
        catch {
            # An error occurred, access the error response
            if ($_.Exception -is [System.Net.WebException]) {
                $response = $_.Exception.Response
                $reader = New-Object System.IO.StreamReader($response.GetResponseStream())
                $responseContent = $reader.ReadToEnd()
                ExitWithError("HTTP error response content: " + $responseContent +" for operation: " + $operationName)
            } else {
                # Handle other types of exceptions if necessary
                ExitWithError("An error occurred:" + $_.Exception.Message + " for operation: " + $operationName)
            }
    }
}


#Function to Display Error message 
function ExitWithError([string] $errorMessage)
{
        Write-host $errorMessage -foreground "red"
        Write-Host "Process Failed." -foreground "red"
        Exit 1
}

#endregion Functions

try
{
    if(!(Get-Pssnapin | Where-Object {$_.name -like "Microsoft.Crm.PowerShell"} ))
    {
        Add-Pssnapin microsoft.crm.powershell
        Write-Host "Added CRM powershell Pssnapin." -foreground "Green"
    }

    if(!(Test-Path -Path $pfxFilePath -PathType Leaf))
    {
       ExitWithError("The specified value of the pfxFilePath parameter isn't valid. Please enter the correct path of the Personal Information Exchange file.")
    }
    else
    {

      $extentionPfx = (Get-Item $pfxFilePath ).Extension 
      if($extentionPfx -ne ".pfx")
      {
           ExitWithError("The specified value of the pfxFilePath parameter isn't valid. Please enter the path of the .pfx file.")
      }
    }

    $graphBaseUri = "https://graph.microsoft.com"

    #region AcquireMsGraphToken
    $Body =  @{
        Grant_Type    = "client_credentials"
        Scope         = "$graphBaseUri/.default"
        Client_Id     = $ClientID
        Client_Secret = $ClientSecret
    }

    $tokenResponse = Invoke-RestMethod `
        -Uri https://login.microsoftonline.com/$microsoftEntraIdTenantIdOrDomainName/oauth2/v2.0/token `
        -Method POST `
        -Body $body

        $headers = @{
                'Authorization'="Bearer $($tokenResponse.access_token)"
                "Content-Type" = "application/json"
             }

    $organizationResponse = Invoke-WebRequest `
        -Uri $graphBaseUri/v1.0/organization `
        -Method GET `
        -Headers $headers

    $TenantID = ($organizationResponse.Content | ConvertFrom-Json).Value.id
    #endregion AcquireMsGraphToken 

    #region SetCertificateInfo
    $securePassword = $privateKeyPassword
    $PfxData = Get-PfxData -FilePath $pfxFilePath -Password $securePassword
    $certificateInfo = $PfxData.EndEntityCertificates[0]
    $certificateBin = $certificateInfo.GetRawCertData() 
    $credentialValue = [System.Convert]::ToBase64String($certificateBin)

    $currentDateTime = Get-Date

    if ($certificateInfo.NotAfter -lt $currentDateTime)
    {
        throw "Certificate with thumbprint $($certificateInfo.Thumbprint) has already expired on $($certificateInfo.NotAfter)."
    }

    $keyCredentialsJsonPayload = @{
            endDateTime = $certEndDateTime
            startDateTime = $certStartDateTime
            type = "AsymmetricX509Cert"
            usage = "Verify"
            key = $credentialValue
            displayName = $certificateInfo.Subject
        }
    $keyCredentialsJsonPayload = New-Object -TypeName PSObject -Property $keyCredentialsJsonPayload


    Write-Host "Done with setting up certificate information." 
    #endregion SetCertificateInfo

    #region ServicePrincipalOperations
    #Set CRM Principal Name in Microsoft Entra ID

    $BaseUri = "$graphBaseUri/v1.0/servicePrincipals"

    $crmAppId =  "00000007-0000-0000-c000-000000000000"

    $findServicePrincipalsQuery = "$BaseUri`?`$filter=appId eq '$crmAppId'&`$select=id,appId,servicePrincipalNames,keyCredentials"
    $findServicePrincipalsResponse = Invoke-WebRequest -Headers $headers -Uri $findServicePrincipalsQuery
    $findServicePrincipalsResponseJson = ($findServicePrincipalsResponse.Content | ConvertFrom-Json).Value

    $servicePrincipalCredentials = $findServicePrincipalsResponseJson.keyCredentials
    if ($null -ne $servicePrincipalCredentials -and -not ($servicePrincipalCredentials -is [array])) {
        $servicePrincipalCredentials = @($servicePrincipalCredentials)
    }

    $servicePrincipalNames = $findServicePrincipalsResponseJson.servicePrincipalNames
    if ($null -ne $servicePrincipalNames -and -not ($servicePrincipalNames -is [array])) {
        $servicePrincipalNames = @($servicePrincipalNames)
    }

    if ($null -eq $servicePrincipalNames) {
        $servicePrincipalNames = [System.Collections.Generic.List[object]]::new()
    }

    $servicePrincipalId = $findServicePrincipalsResponseJson.id

    $servicePrincipalCredentialsWorkingCollection = [System.Collections.Generic.List[object]]::new()
    if ($null -ne $servicePrincipalCredentials -and $servicePrincipalCredentials.Count -gt 0) {
        $servicePrincipalCredentialsWorkingCollection.AddRange($servicePrincipalCredentials)
    }

    $createServicePrincipalCredential = $true
    $patchServicePrincipalCredential = $false

    if ($null -ne $servicePrincipalCredentials)
    {
        for ($i = 0; $i -lt $servicePrincipalCredentials.Count; $i++)
        {
            if ($servicePrincipalCredentials[$i].endDateTime -lt $currentDateTime)
            {
                Write-Output("Certificate '" + $servicePrincipalCredentials[$i].displayName + "', with thumbprint '" + $servicePrincipalCredentials[$i].customKeyIdentifier + "' has expired on "+ $servicePrincipalCredentials[$i].endDateTime +". Removing the certificate principal from CRM app with id '" + $crmAppId +"'.")
                $removeID = $servicePrincipalCredentials[$i].keyId
                $servicePrincipalCredentialsWorkingCollection = $servicePrincipalCredentialsWorkingCollection | Where-Object { $_.keyId -ne $removeID }
                $patchServicePrincipalCredential = $true
            }
            else
            {
                if ($servicePrincipalCredentials[$i].key -eq $credentialValue)
                {
                    $createServicePrincipalCredential = $false
                    Write-Output("Given the certificate is already associated with the principal linked to the appId " + $crmAppId + ". Cert thumbprint " + $certificateInfo.Thumbprint + ". Not adding the cert principal.")
                }
            }
        }
    }

    $servicePrincipalCredentialsForPatch = [System.Collections.Generic.List[object]]::new()

    $servicePrincipalCredentialsWorkingCollection | ForEach-Object {
        $obj = $_ | Select-Object * -ExcludeProperty key, displayName
        # Add the modified object to the list
        $servicePrincipalCredentialsForPatch.Add([PSCustomObject]$obj)
    }

    if ($createServicePrincipalCredential)
    {
        $servicePrincipalCredentialsForPatch += $keyCredentialsJsonPayload
        Write-Output("Adding new certificate principal credential for appId " + $crmAppId +". Cert thumbprint " +$certificateInfo.Thumbprint)
    }

    $crmServicePrincipalUri = "$BaseUri`/$servicePrincipalId"

    if ($createServicePrincipalCredential -Or $patchServicePrincipalCredential)
    {
        $jsonPayload = @{
            keyCredentials = $servicePrincipalCredentialsForPatch
        }
        Update-ServicePrincipal $jsonPayload "createOrUpdateServicePrincipalCredential" -uri $crmServicePrincipalUri $headers 
        Write-Output("Successfully updated key Credentials for app "+ $crmAppId +". Cert thumbprint " +$certificateInfo.Thumbprint)
    }

    #Add CRM App Id to the servicePrincipalNames if needed
    if ($servicePrincipalNames.Where({ $_ -eq $crmAppId }, 'First').Count -eq 0)
    {
        $servicePrincipalNames += $crmAppId

        # Create a hashtable that represents JSON structure
        $jsonPayload = @{
            servicePrincipalNames = $servicePrincipalNames
        }
        Update-ServicePrincipal -ServicePrincipal $jsonPayload -operationName "updateServicePrincipalNames" -uri $crmServicePrincipalUri -headers $headers    
        Write-Output("Added new service principal name: " + $crmAppId)
    }
    #endregion ServicePrincipalOperations

    #Configure CRM server for server-based authentication with Online Exchange
       
    $setting = New-Object "Microsoft.Xrm.Sdk.Deployment.ConfigurationEntity" 
    $setting.LogicalName = "ServerSettings" 
    $setting.Attributes = New-Object "Microsoft.Xrm.Sdk.Deployment.AttributeCollection" 
    $attribute1 = New-Object "System.Collections.Generic.KeyValuePair[String, Object]" ("S2SDefaultAuthorizationServerPrincipalId", "00000001-0000-0000-c000-000000000000") 
    $setting.Attributes.Add($attribute1) 
    $attribute2 = New-Object "System.Collections.Generic.KeyValuePair[String, Object]" ("S2SDefaultAuthorizationServerMetadataUrl","https://accounts.accesscontrol.windows.net/metadata/json/1") 
    $setting.Attributes.Add($attribute2) 
    Set-CrmAdvancedSetting -Entity $setting 

    Write-Host "Done with configuration of CRM server for server-based authentication with Online Exchange."

    try
    {
        $orgInfo = Get-CrmOrganization  -Name $organizationName
        $ID =  $orgInfo.id 
    }Catch
    {
        ExitWithError("The specified organization "+$organizationName+" is not a valid CRM organization.")
    }
    if($ID)
    {
         Set-CrmAdvancedSetting -ID $orgInfo.ID -configurationEntityName "Organization" -setting "S2STenantId" -value $TenantID
    }

    Write-Host "S2S Exchange Online Tenant ID is populated in configDB: " $TenantID
    Write-Host "Process succeeded."  -foreground "green"
}
Catch 
{
    Write-Host "Failure Details: $_" -foreground "red"
    ExitWithError($_.Exception.Message)
}
