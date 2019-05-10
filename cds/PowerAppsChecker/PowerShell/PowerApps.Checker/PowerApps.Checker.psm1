$redirectUri = "urn:ietf:wg:oauth:2.0:oob"
$resourceUri = "https://api.advisor.powerapps.com"
$adalStrongName = "Microsoft.IdentityModel.Clients.ActiveDirectory, Version=4.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"

#region Internal helper functions

function script:Get-CommonHeadersDictionary
{
    param (
        [Guid]$CorrelationId,
        [Guid]$TenantId,
        [string]$OauthToken
    )

    $commonHeaders = @{}
    $commonHeaders.Add("x-ms-correlation-id", $CorrelationId.ToString())
    $commonHeaders.Add("x-ms-client-tenant-id", $TenantId.ToString())
    if ($OauthToken)
    {
        $commonHeaders.Add("Authorization", ("Bearer " + $OauthToken))
    }
    return $commonHeaders
}

function script:New-RuleList($Rule)
{
	# Loop through and add the desired rules
	$currentIndex = 0

	$rules = New-Object PSCustomObject[] $Rule.Count
	@($Rule) | ForEach-Object -Process {
		$rules[$currentIndex] = [PSCustomObject]@{ code=$_ }
		$currentIndex++
	}
	return $rules
}

function script:New-RulesetList($RulesetId)
{
	# Loop through and add the desired rulesets
	$currentIndex = 0

	$rulesets = New-Object PSCustomObject[] $RulesetId.Count
	@($RulesetId) | ForEach-Object -Process {
		$rulesets[$currentIndex] = [PSCustomObject]@{ id=$_.ToString() }
		$currentIndex++
	}
	return $rulesets
}

function script:Get-BaseUriFromGeography($Geography)
{
    $coreUri = "api.advisor.powerapps.com"
    switch ($Geography)
    {
        "Preview United States" { return "https://unitedstatesfirstrelease.$coreUri" }
        "United States" { return "https://unitedstates.$coreUri" }
        "Europe" { return "https://europe.$coreUri" }
        "Asia" { return "https://asia.$coreUri" }
        "Australia" { return "https://australia.$coreUri" }
        "Japan" { return "https://japan.$coreUri" }
        "India" { return "https://india.$coreUri" }
        "Canada" { return "https://canada.$coreUri" }
        "South America" { return "https://southamerica.$coreUri" }
        "United Kingdom" { return "https://unitedkingdom.$coreUri" }
    }
}

function script:Get-BearerTokenForInteractiveUser($ApplicationId, $TenantId, $Authority)
{
	Write-Verbose "Authority is $Authority"

    $authContext = New-Object -ArgumentList "$Authority/$TenantId" -TypeName "Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext, $adalStrongName"
    $acquireParams = New-Object -ArgumentList "SelectAccount" -TypeName "Microsoft.IdentityModel.Clients.ActiveDirectory.PlatformParameters, $adalStrongName"

    Write-Verbose "Creating the bearer token"
    $token = $authContext.AcquireTokenAsync($resourceUri, $ApplicationId, [Uri]::new($redirectUri), $acquireParams).GetAwaiter().GetResult()
    Write-Verbose "Token created for $($token.UserInfo.DisplayableId)"

    if ($token -eq $null -or [string]::IsNullOrWhiteSpace($token.AccessToken))
    {
        throw "Token unable to be obtained. Unknown issue."
    }

    return $token.AccessToken
}

function script:Get-BearerTokenForApp($ApplicationId, $AppSecret, [byte[]]$AppCertificate, $TenantId, $Authority)
{
	Write-Verbose "Authority is $Authority"

	$authContext = New-Object -ArgumentList "$Authority/$TenantId", $false -TypeName "Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext, $adalStrongName"
	if ($AppCertificate)
	{
		# Certificate
		$aadCert = New-Object -ArgumentList $AppCertificate -TypeName "System.Security.Cryptography.X509Certificates.X509Certificate2, $adalStrongName"
		$clientAssertion = New-Object - ArgumentList $ApplicationId, $aadCert -TypeName "Microsoft.IdentityModel.Clients.ActiveDirectory.ClientAssertionCertificate, $adalStrongName"
		Write-Verbose "Creating the bearer token with the certificate"
		$token = $authContext.AcquireTokenAsync($resourceUri, $clientAssertion, $true).GetAwaiter().GetResult()
		$advisorAadCert.Dispose()
	}
	else
	{
		# For obtaining an oauth token using a secret use the following
		$clientCredential = New-Object -ArgumentList $ApplicationId, $AppSecret -TypeName "Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential, $adalStrongName"
		Write-Verbose "Creating the bearer token with the secret"
		$token = $authContext.AcquireTokenAsync($resourceUri, $clientCredential).GetAwaiter().GetResult()
	}
	Write-Verbose "Bearer token created for AAD Client App: $ApplicationId"

    return $token.AccessToken
}

<#
    This is not needed in V6 of PowerShell, however, it is not included by default on machines so using to this approach.
#>
function script:Invoke-MultipartFormDataUpload
{
    [CmdletBinding()]
    PARAM
    (        
        [parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string]
        $InFile,
        [string]
        $ContentType,
        [parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [Uri]
        $Uri,
        [HashTable]
        $Headers
    )

    if (-not $ContentType)
    {
        Add-Type -AssemblyName System.Web

        $mimeType = [System.Web.MimeMapping]::GetMimeMapping($InFile)

        if ($mimeType)
        {
            $ContentType = $mimeType
        }
        else
        {
            $ContentType = "application/octet-stream"
        }
    }

    Add-Type -AssemblyName System.Net.Http

    $httpClientHandler = New-Object System.Net.Http.HttpClientHandler

    $httpClient = New-Object System.Net.Http.Httpclient $httpClientHandler

    # Add headers when provided
    $Headers.Keys | ForEach-Object -Process { $httpClient.DefaultRequestHeaders.Add($_, $Headers.Item($_)) }

    $fileStream = New-Object System.IO.FileStream @($InFile, [System.IO.FileMode]::Open)

    $dispositionHeaderValue = New-Object System.Net.Http.Headers.ContentDispositionHeaderValue "form-data"
    $dispositionHeaderValue.Name = "fileData"
    $dispositionHeaderValue.FileName = (Split-Path $InFile -leaf)

    $streamContent = New-Object System.Net.Http.StreamContent $fileStream 
    $streamContent.Headers.ContentDisposition = $dispositionHeaderValue
    $streamContent.Headers.ContentType = New-Object System.Net.Http.Headers.MediaTypeHeaderValue $ContentType

    $content = New-Object System.Net.Http.MultipartFormDataContent
    $content.Add($streamContent)

    try
    {
        $response = $httpClient.PostAsync($Uri, $content).GetAwaiter().GetResult()

        if (!$response.IsSuccessStatusCode)
        {
            $responseBody = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
            $errorMessage = "Status code {0}. Reason {1}. Server reported the following message: {2}." -f $response.StatusCode, $response.ReasonPhrase, $responseBody

            throw [System.Net.Http.HttpRequestException] $errorMessage
        }

        $responseBody = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()

        return $responseBody
    }
    finally
    {
        if($null -ne $httpClient)
        {
            $httpClient.Dispose()
        }

        if($null -ne $response)
        {
            $response.Dispose()
        }
    }
}

#endregion

<#
	Invoke-PowerAppsChecker
#>
function Invoke-PowerAppsChecker
{
	[CmdletBinding()]
    param(
		[Parameter(Mandatory = $true, ParameterSetName = "OpenUri")]
		[Parameter(Mandatory = $false, ParameterSetName = "CertificateBased")]
		[Parameter(Mandatory = $false, ParameterSetName = "SecretBased")]
        [Parameter(Mandatory = $false, ParameterSetName = "SasUri")]
        [Parameter(Mandatory = $false, ParameterSetName = "LocalFile")]
        [string]
		$RootUri,

		[Parameter(Mandatory = $true, ParameterSetName = "PresetUri")]
		[Parameter(Mandatory = $false, ParameterSetName = "CertificateBased")]
		[Parameter(Mandatory = $false, ParameterSetName = "SecretBased")]
        [Parameter(Mandatory = $false, ParameterSetName = "SasUri")]
        [Parameter(Mandatory = $false, ParameterSetName = "LocalFile")]
        [ValidateSet("Preview United States", "United States", "Europe", "Asia", "Australia", "Japan", "India", "Canada", "South America", "United Kingdom")]
        [string]
        $Geography = "United States",

        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]		
        [Guid]
		$TenantId,

        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]		
		[Guid]
		$ApplicationId,	
		
		[ValidateScript({[string]::IsNullOrWhiteSpace($_) -or (Test-Path $_)})]
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = "LocalFile")]
        [ValidateNotNullOrEmpty()]		
		[string[]]
		$FileUnderAnalysis,

        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = "SasUri")]
		[Uri[]]
		$FileUnderAnalysisSasUri,

        [Parameter(Mandatory = $true)]
		[ValidateScript({[string]::IsNullOrWhiteSpace($_) -or (Test-Path $_ -PathType Container)})]
		[string]
		$OutputDirectory,

        [Parameter(Mandatory = $false)]
        [ValidateSet("bg","ca","cs","da","de","el","en","es","et","eu","fi","fr","gl","hi","hr","hu","id","it","ja","kk","ko","lt","lv","ms","nb","nl","pl","pt-BR","pt-pt","ro","ru","sk","sl","sr-Cyrl-RS","sr-Latn-RS","sv","th","tr","uk","vi","zh-HANS","zh-HANT")]
        [string]
        $LocaleName = "en",

        [Parameter(Mandatory = $false)]
        [string]
        $Authority = "https://login.microsoftonline.com",

        [string[]]
		$Rule,

        [Guid[]]
		$RulesetId,
		
        [Parameter(Mandatory = $false)]	
		[Guid]
		$TrackingId = (New-Guid),

        [string[]]
		$ExcludedFilePattern,

		[int]
		[ValidateRange(15, 60)]
		$SecondsBetweenStatusChecks = 15,

		[Parameter(Mandatory = $true, ParameterSetName = "Interactive")]
        [Parameter(Mandatory = $false, ParameterSetName = "SasUri")]
        [Parameter(Mandatory = $false, ParameterSetName = "LocalFile")]
		[Parameter(Mandatory = $false, ParameterSetName = "OpenUri")]
		[Parameter(Mandatory = $false, ParameterSetName = "PresetUri")]
		[switch]
		[bool]
		$InteractiveLogon,
		
		[Parameter(Mandatory = $true, ParameterSetName = "SecretBased")]
        [Parameter(Mandatory = $false, ParameterSetName = "SasUri")]
        [Parameter(Mandatory = $false, ParameterSetName = "LocalFile")]
		[Parameter(Mandatory = $false, ParameterSetName = "OpenUri")]
		[Parameter(Mandatory = $false, ParameterSetName = "PresetUri")]
        [string]
		$ApplicationSecret,
		
		[Parameter(Mandatory = $true, ParameterSetName = "CertificateBased")]
        [Parameter(Mandatory = $false, ParameterSetName = "SasUri")]
        [Parameter(Mandatory = $false, ParameterSetName = "LocalFile")]
		[Parameter(Mandatory = $false, ParameterSetName = "OpenUri")]
		[Parameter(Mandatory = $false, ParameterSetName = "PresetUri")]
        [byte[]]
		$ApplicationCertificate
    )

	# Force TLS 1.2
	[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

    # Ensure that we are arrays as if one item is passed it is not an array, 
    # but is when multiple are passed. These are items that we use ForEach-Object on.
    if ($FileUnderAnalysisSasUri -ne $null) { $FileUnderAnalysisSasUri = @($FileUnderAnalysisSasUri) }
    if ($FileUnderAnalysis -ne $null) { $FileUnderAnalysis = @($FileUnderAnalysis) }
    if ($Rule -ne $null) { $Rule = @($Rule) }
    if ($RulesetId -ne $null) { $RulesetId = @($RulesetId) }
    
    if ([string]::IsNullOrWhiteSpace($RootUri))
    {
        $RootUri = Get-BaseUriFromGeography -Geography $Geography
    }
   
    Write-Host "TrackingId: $TrackingId"

	# Remove any extra path characters since we assume it is not there.
	$RootUri = $RootUri.TrimEnd("/")

    # Load up the ADAL libraries
    [System.Reflection.Assembly]::LoadFrom("$PSScriptRoot/Microsoft.IdentityModel.Clients.ActiveDirectory.dll") | Out-Null

    # Get oauth token
    $oauthToken = $null
    if ($InteractiveLogon)
    {
        Write-Verbose "Getting oauth token for the interactive user"
        $oauthToken = Get-BearerTokenForInteractiveUser -ApplicationId $ApplicationId -TenantId $TenantId -Authority $Authority
    }
    else
    {
        Write-Verbose "Getting oauth token for the preset AAD App"
        $oauthToken = Get-BearerTokenForApp -ApplicationId $ApplicationId -AppSecret $ApplicationSecret -AppCertificate $ApplicationCertificate -TenantId $TenantId -Authority $Authority
    }

    Write-Host "oauth token obtained"
    Write-Verbose "Base endpoint is $RootUri"

    if ($FileUnderAnalysis -ne $null)
    {
        Write-Verbose "Uploading the artifacts"
    
	    # Upload artifacts
        $uploadHeaders = Get-CommonHeadersDictionary -CorrelationId $TrackingId -TenantId $TenantId -OauthToken $oauthToken
        $uploadResponse = $null

	    # Initialize the container for all of the results
	    $sasUris = New-Object string[] $FileUnderAnalysis.Count
	    $currentIndex = 0

        Write-Output "Uploading $($FileUnderAnalysis.Count) artifacts"

	    # Loop through and add the desired files
	    $FileUnderAnalysis | ForEach-Object -Process {
		    try
		    {
			    $uploadResponse = Invoke-MultipartFormDataUpload -InFile $_ -Uri "$RootUri/api/upload" -Headers $uploadHeaders
		    }
		    catch # [System.Net.WebException]
		    {
			    Write-Verbose "An exception was caught: $($_.Exception.ToString())"
			    Write-Host "Received a failure result from the analyze API. Stopping."
                $PSCmdlet.ThrowTerminatingError($_)
			    return
		    }
		    $sasUri = $uploadResponse | ConvertFrom-Json
		    $sasUris[$currentIndex] = $sasUri
		    Write-Verbose "Uploaded $_ which is at $sasUri"
		
		    $currentIndex++
	    }

        Write-Output "Artifacts are uploaded"
    }
    else
    {
        Write-Verbose "No artifacts to upload"
        $sasUris = $FileUnderAnalysisSasUri | ForEach-Object { $_.AbsoluteUri }
    }
    
    # Construct the AnalysisRequest object required for processing against the analyze route
    $analysisRequest = [PSCustomObject] @{
        fileExclusions = [string[]]$ExcludedFilePattern
        sasUriList = [string[]]$sasUris # Either ones we uploaded or they passed
    }

	if ($RulesetId -ne $null)
	{
		Write-Verbose "Adding rulesets"
		$analysisRequest | Add-Member NoteProperty -Name "ruleSets" -value @(New-RulesetList -RulesetId $RulesetId)
	}

	if ($Rule -ne $null)
	{		
		Write-Verbose "Adding rules"
		$analysisRequest | Add-Member NoteProperty -Name "ruleCodes" -value (New-RuleList -Rule $Rule)
	}

	$analysisRequestJson = $analysisRequest | ConvertTo-Json

	# Analyze

    Write-Verbose "Initiating analysis"

    $analyzeHeaders = Get-CommonHeadersDictionary -CorrelationId $TrackingId -TenantId $TenantId -OauthToken $oauthToken
    # Need this header to return V2 of Sarif for now. In the near future this will be default, but currently V1 is returned by default.
    $analyzeHeaders.Add("Accept", "application/json,application/x-ms-sarif-v2")
    $analyzeHeaders.Add("Accept-Language", $LocaleName)
    $analyzeResponse = $null

    try 
    {
        $analyzeResponse = Invoke-WebRequest -Uri "$RootUri/api/analyze" -Headers $analyzeHeaders -Method Post -Body $analysisRequestJson -ContentType "application/json" -DisableKeepAlive -UseBasicParsing
    }
    catch [System.Net.WebException]
    {
        Write-Verbose "An exception was caught: $($_.Exception.ToString())"
        Write-Error "Received a failure result from the analyze API, $($_.Exception.Message). Stopping."
        return
    }

    $analyzeResponseCode = 0
    $analyzeResponseCode = $analyzeResponse.StatusCode
    if ($analyzeResponseCode -ne 200 -and $analyzeResponseCode -ne 202)
    {
        Write-Error "Received a failure result from the analyze API, $analyzeResponseCode. Stopping."
        return
    }

    Write-Host "Analysis running"

    # Get the URI to query for status from the response
    $statusCheckUri = $analyzeResponse.Headers.Location

    # Check status
    $i = 0
	# Consider making this a parameter in the future
    $maxLoops = 15
    $statusCheckHeaders = Get-CommonHeadersDictionary -CorrelationId $TrackingId -TenantId $TenantId -OauthToken $oauthToken
    $statusCheckStopWatch = [System.Diagnostics.Stopwatch]::StartNew()
    $percentComplete = 0
    $statusResponseCode = 0
    Write-Host "Checking for status using $statusCheckUri"

    do
    {
        $i++
        Start-Sleep -Seconds $SecondsBetweenStatusChecks
        $elapsed = $statusCheckStopWatch.Elapsed

        if ($i -gt 1) { $percentComplete = $statusCheckResponseBody.Progress }

        $formattedOutput = "Running for {0:hh}:{0:mm}:{0:ss} at {1}% complete." -f $elapsed, $percentComplete
        Write-Host $formattedOutput
        $statusCheckResponse = $null

        try
        {
            # The response will be 202 while still running. When finished it will be 200.
            $statusCheckResponse = Invoke-WebRequest -Uri $statusCheckUri -Headers $statusCheckHeaders -Method Get -ContentType "application/json" -UseBasicParsing
        }
        catch [System.Net.WebException]
        {
            Write-Verbose "An exception was caught: $($_.Exception.ToString())"             
            Write-Error "Received a failure result from the status API, $($_.Exception.Message). Stopping."
            Return
        }
        $statusResponseCode = $statusCheckResponse.StatusCode
        if ($statusResponseCode -ne 200 -and $statusResponseCode -ne 202)
        {
            Write-Error "Received a failure result from the status check API, $statusResponseCode. Stopping."
            Return
        }

        $statusCheckResponseBody = $statusCheckResponse.Content | ConvertFrom-Json
    }
    until($i -ge $maxLoops -or $statusResponseCode -eq 200)

    $elapsed = $statusCheckStopWatch.Elapsed
    $formattedOutput = "Ran for {0:hh}:{0:mm}:{0:ss}" -f $elapsed 
    Write-Host $formattedOutput

    $resultUris = $statusCheckResponseBody.ResultFileUris

    Write-Host "Downloading the results to $OutputDirectory"

    # Initialize the array list for adding downloaded file names
    [System.Collections.ArrayList]$resultFiles = @()

    # Download each result
    foreach ($resultUri in $resultUris)
    {
        # Pull out the file name from the SAS URI
        $startingPoint = $resultUri.LastIndexOf("/") + 1
        $length = $resultUri.IndexOf("?") - $startingPoint
        $fileName = $resultUri.Substring($startingPoint, $length)

        # Construct the output file location
        $outFile = [System.IO.Path]::Combine($OutputDirectory, $fileName)

		Write-Verbose "Downloading $resultUri to $outFile"

        Invoke-WebRequest -Uri $resultUri -OutFile $outFile

        $resultFiles.Add($outFile)
    }

    # Construct a concise result object instead of returning the response from the web call
    # There is only high, medium, and low currently even though the response may have properties
    # for critical and informational
    $result = [PSCustomObject]@{
        HighIssueCount = $statusCheckResponseBody.issueSummary.highIssueCount
        MediumIssueCount = $statusCheckResponseBody.issueSummary.mediumIssueCount
        LowIssueCount = $statusCheckResponseBody.issueSummary.lowIssueCount
        Status = $statusCheckResponseBody.status
        DownloadedResultFiles = $resultFiles
        ResultFileUris = $resultUris
    }

    Write-Host "Files are downloaded to $OutputDirectory"
	Write-Host "Finished"

    return $result
}

function Get-PowerAppsCheckerRulesets
{
    [CmdletBinding()]
    param(
		[Parameter(Mandatory = $false)]
        [ValidateSet("Preview United States", "United States", "Europe", "Asia", "Australia", "Japan", "India", "Canada", "South America", "United Kingdom")]
        [string]
        $Geography = "United States",      
        
		[Parameter(Mandatory = $false)]
        [string]
		$RootUri,

        [Parameter(Mandatory = $false)]
        [Guid]
		$TenantId = "00000000-0000-0000-0000-000000000000"
    )

	# Force TLS 1.2
	[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    
    if ([string]::IsNullOrWhiteSpace($RootUri))
    {
        $RootUri = Get-BaseUriFromGeography -Geography $Geography
    }
     
    $response = $null
    try 
    {
        $response = Invoke-WebRequest -Uri "$RootUri/api/ruleset" -Headers @{"x-ms-tenant-id" = $TenantId} -Method Get -ContentType "application/json" -DisableKeepAlive -UseBasicParsing
    }
    catch [System.Net.WebException]
    {
        Write-Verbose "An exception was caught: $($_.Exception.ToString())"
        Write-Error "Received a failure result from the analyze API, $($_.Exception.Message). Stopping."
        return
    }
    return $response | ConvertFrom-Json
}

function Get-PowerAppsCheckerRules
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $false)]
        [ValidateSet("Preview United States", "United States", "Europe", "Asia", "Australia", "Japan", "India", "Canada", "South America", "United Kingdom")]
        [string]
        $Geography = "United States",
        
		[Parameter(Mandatory = $false)]
        [string]
		$RootUri,

        [ValidateSet("bg","ca","cs","da","de","el","en","es","et","eu","fi","fr","gl","hi","hr","hu","id","it","ja","kk","ko","lt","lv","ms","nb","nl","pl","pt-BR","pt-pt","ro","ru","sk","sl","sr-Cyrl-RS","sr-Latn-RS","sv","th","tr","uk","vi","zh-HANS","zh-HANT")]
        [string]
        $LocaleName = 'en',

        [Parameter(Mandatory = $false)]
        [Guid[]]
        $RulesetId,

        [Parameter(Mandatory = $false)]
        [Guid]
		$TenantId = "00000000-0000-0000-0000-000000000000",

        [switch]
        $IncludeMessageFormats
    )

	# Force TLS 1.2
	[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

    if ($RulesetId -ne $null) { $RulesetId = @($RulesetId) }
    
    if ([string]::IsNullOrWhiteSpace($RootUri))
    {
        $RootUri = Get-BaseUriFromGeography -Geography $Geography
    }
    
    $uri = "$RootUri/api/rule?includeMessageFormats=$IncludeMessageFormats"
    if ($RulesetId -ne $null)
    {
        $uri = "$uri&ruleset=$([string]::Join(",", ($RulesetId | ForEach-Object { $_.ToString() })))"
    }

    $response = $null
    try 
    {
        $response = Invoke-WebRequest -Uri $uri -Headers @{"Accept-Language" = $LocaleName; "x-ms-tenant-id" = $TenantId} -Method Get -ContentType "application/json" -DisableKeepAlive -UseBasicParsing
    }
    catch [System.Net.WebException]
    {
        Write-Verbose "An exception was caught: $($_.Exception.ToString())"
        Write-Error "Received a failure result from the analyze API, $($_.Exception.Message). Stopping."
        return
    }

    return $response | ConvertFrom-Json
}

function New-PowerAppsCheckerAzureADApplication
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [Guid]
        $TenantId,
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string]
        $ApplicationDisplayName
    )

    # Using AzureAD as the RM modules, AzureRm.Resource and AzureRm.Profile, do not allow for setting RequiredResourceAccess
    Import-Module AzureAD | Out-Null
    try
    {
        Connect-AzureAD -TenantId $TenantId | Out-Null
    }
    catch [Microsoft.Open.Azure.AD.CommonLibrary.AadAuthenticationFailedException]
    {
        Write-Error "Received a failure result from the connecting to AzureAD, $($_.Exception.Message). Stopping."
        return
    }
    $graphSignInAndReadProfileRequirement = New-Object -TypeName "Microsoft.Open.AzureAD.Model.RequiredResourceAccess"
    $acc1 = New-Object -TypeName "Microsoft.Open.AzureAD.Model.ResourceAccess" -ArgumentList "e1fe6dd8-ba31-4d61-89e7-88639da4683d","Scope"
    $graphSignInAndReadProfileRequirement.ResourceAccess = $acc1
    $graphSignInAndReadProfileRequirement.ResourceAppId = "00000003-0000-0000-c000-000000000000"

    $powerAppsCheckerApiRequirement = New-Object -TypeName "Microsoft.Open.AzureAD.Model.RequiredResourceAccess"
    $acc1 = New-Object -TypeName "Microsoft.Open.AzureAD.Model.ResourceAccess" -ArgumentList "d533b86d-8f67-45f0-b8bb-c0cee8da0356","Scope"
    $acc2 = New-Object -TypeName "Microsoft.Open.AzureAD.Model.ResourceAccess" -ArgumentList "640bd519-35de-4a25-994f-ae29551cc6d2","Scope"
    $powerAppsCheckerApiRequirement.ResourceAccess = $acc1,$acc2
    $powerAppsCheckerApiRequirement.ResourceAppId = "c9299480-c13a-49db-a7ae-cdfe54fe0313"

    $application = New-AzureADApplication -DisplayName $ApplicationDisplayName -PublicClient $true -ReplyUrls "urn:ietf:wg:oauth:2.0:oob" -RequiredResourceAccess $graphSignInAndReadProfileRequirement, $powerAppsCheckerApiRequirement
    if ($application -eq $null -or $application.AppId -eq $null)
    {
        Write-Error "Unable to create the application as requested."
    }
    else
    {
        Write-Host "The application $($application.AppId):$ApplicationDisplayName was created in the tenant, $TenantId. You may need to have an administrator grant consent. If running in a userless process, you will need to add a secret or certificate in which to authenticate." 
    }
}