---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates the use of Task Parallel Library (TPL) Dataflow components using Dataverse Web API to improve throughput."
---

# Web API TPL DataFlow ParallelOperations sample

This .NET 6.0 sample demonstrates the use of Task Parallel Library (TPL) Dataflow components. Learn more in [Dataflow (Task Parallel Library)](https://learn.microsoft.com/dotnet/standard/parallel-programming/dataflow-task-parallel-library).

TPL provides capabilities to add parallelism and concurrency to applications. These capabilities are an important part of maximizing throughput when performing operations that add or update data within Dataverse.

This sample uses the common helper code in the [WebAPIService](../WebAPIService) class library project.

## Prerequisites

- Microsoft Visual Studio 2022
- Access to Dataverse with privileges to perform data operations

## How to run the sample

1. Clone or download the [PowerApps-Samples](../../../../../PowerApps-Samples) repository.
1. Open the [TPLDataFlowParallelOperations.sln](TPLDataFlowParallelOperations.sln) file using Visual Studio 2022.
1. Edit the [appsettings.json](../appsettings.json) file to set the following property values:

   | Property | Instructions |
   |----------|--------------|
   | `Url` | The URL for your environment. Replace the placeholder `https://yourorg.api.crm.dynamics.com` value with the value for your environment. Learn how to find your URL in [View developer resources](https://learn.microsoft.com/power-apps/developer/data-platform/view-download-developer-resources). |
   | `UserPrincipalName` | Replace the placeholder `you@yourorg.onmicrosoft.com` value with the UPN value you use to access the environment. |
   | `Password` | Replace the placeholder `yourPassword` value with your password. |

1. Save the `appsettings.json` file.
1. Press `F5` to run the sample.

## Demonstrates

This sample includes settings you can apply to optimize your connection.

This sample first sends a request to access the value of the `x-ms-dop-hint` response header to determine the recommended degrees of parallelism for this environment. When the maximum degree of parallelism is set equal to the value of the `x-ms-dop-hint` response header, you should achieve a steady state where throughput is optimized with a minimum of `429 TooManyRequests` service protection limit errors returned.

To encounter service protection limits with this sample you should raise the `numberOfRecords` variable to over 10,000 or whatever is needed for the sample to run for more than 5 minutes. You should also change the code to set the `maxDegreeOfParallelism` to be significantly greater than the `x-ms-dop-hint` response header value. Then, using Fiddler you can observe how WebAPIService retries the requests that return this error.

This sample creates a configurable number of account records, which then get deleted. This sample uses dataflow components to process the records and transform the results of the create operation into the next phase that deletes these records. Because of the nature of this data flow, delete operations for previously created records start before all the records to create are finished.

## Clean up

By default this sample deletes all the records created in it.
