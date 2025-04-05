<#
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS. MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
NO TECHNICAL SUPPORT IS PROVIDED. YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
#>

function Invoke-PollOperation{
    param (
        [Parameter(Mandatory, ParameterSetName="Header")]
        $Headers,
        [Parameter(Mandatory, ParameterSetName="Manual")]
        [ValidateNotNullOrEmpty()]
        $PollEndpoint,
        [Parameter(Mandatory, ParameterSetName="Manual")]
        [ValidateNotNullOrEmpty()]
        $PollInterval
    )

    Write-Host "Do you want to poll the operation (y/n)"
    $poll = Read-Host

    if ("n" -eq $poll)
    {
        return
    }

    if($PSCmdlet.ParameterSetName -eq "Header")
    {
        $PollEndpoint = $Headers.'operation-location'
        $PollInterval = $Headers.'Retry-After'
    }

    Write-Host "Polling the operation every $PollInterval seconds."

    $run = $true
    while ($run)
    {
        $pollResult = InvokeApi -Method GET -Route $PollEndpoint

        if ($null -eq $pollResult -or $null -eq $pollResult.id -or $null -eq $pollResult.state)
        {
            Write-Host "Operation polling failed $pollResult"
            $run = $false
        }

        $operationState = $pollResult.state.id 
        if ($operationState.Equals("Failed") -or $operationState.Equals("Succeeded"))
        {
            Write-Host "Operation finished with state $operationState"
            $run = $false
        }
        elseif ($operationState.Equals("Running"))
        {
            Write-Host "Operation still running. Poll after $PollInterval seconds"
            Start-Sleep -Seconds $PollInterval
        }
        else
        {
            Write-Host "Unknown operation state $operationState"
            $run = $false
        }
    }
}