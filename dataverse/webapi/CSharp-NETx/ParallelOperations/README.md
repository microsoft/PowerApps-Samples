---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to achieve higher throughput by sending requests in parallel to the Dataverse Web API."
---

# Web API Parallel Operations sample

This .NET 6.0 sample demonstrates how to achieve higher throughput by sending requests in parallel to the Dataverse Web API.

Sending requests in parallel is a recommended strategy to increase total throughput when performing bulk operations with Dataverse. Learn more in the [Use multiple threads](https://learn.microsoft.com/power-apps/developer/data-platform/api-limits#use-multiple-threads) section of the [Service protection API limits](https://learn.microsoft.com/en-us/power-apps/developer/data-platform/api-limits?tabs=sdk) article.

This sample uses the common helper code in the [WebAPIService](../WebAPIService) class library project.

## Prerequisites

- Microsoft Visual Studio 2022
- Access to Dataverse with privileges to perform data operations

## How to run the sample

1. Clone or download the [PowerApps-Samples](../../../../../PowerApps-Samples) repository.
1. Open the [ParallelOperations.sln](ParallelOperations.sln) file using Visual Studio 2022.
1. Edit the [appsettings.json](../appsettings.json) file to set the following property values:

   | Property | Instructions |
   |----------|--------------|
   | `Url` | The URL for your environment. Replace the placeholder `https://yourorg.api.crm.dynamics.com` value with the value for your environment. Learn how to find your URL in [View developer resources](https://learn.microsoft.com/power-apps/developer/data-platform/view-download-developer-resources). |
   | `UserPrincipalName` | Replace the placeholder `you@yourorg.onmicrosoft.com` value with the UPN value you use to access your environment. |
   | `Password` | Replace the placeholder `yourPassword` value with the password you use. |

1. Save the `appsettings.json` file.
1. Press `F5` to run the sample.

## Demonstrates

This sample includes settings you can apply to optimize your connection.

This sample first sends a request to access the value of the `x-ms-dop-hint` response header to determine the recommended degrees of parallelism for this environment. When the maximum degree of parallelism is set equal to the value of the `x-ms-dop-hint` response header, you should achieve a steady state where throughput is optimized with a minimum of `429 TooManyRequests` service protection limit errors returned.

To encounter service protection limits with this sample you should raise the `numberOfRecords` variable to over 10,000 or whatever is needed for the sample to run for more than 5 minutes. You should also change the code to set the `maxDegreeOfParallelism` to be significantly greater than `x-ms-dop-hint` response header value. Then, using Fiddler you should be able to observe how WebAPIService retries the requests that return this error.

This sample uses the [Parallel.ForEachAsync Method](https://learn.microsoft.com/dotnet/api/system.threading.tasks.parallel.foreachasync) introduced with .NET 6.0.

This sample processes a list of requests to create account records, sending the requests in parallel and then uses the data returned to add requests to delete the created accounts to a [ConcurrentBag](https://learn.microsoft.com/dotnet/api/system.collections.concurrent.concurrentbag-1?view=net-6.0). After the records are created, the number of seconds to create the records is displayed.

Then, the delete requests in the `ConcurrentBag` are processed and the time spent deleting the records is displayed.

## Clean up

By default, this sample deletes all the records created in it.
