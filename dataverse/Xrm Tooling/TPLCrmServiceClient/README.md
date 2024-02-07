# Task Parallel Library sample with CrmServiceClient

Task Parallel Library (TPL) makes developers more productive by simplifying the process of adding parallelism and concurrency to applications.

Adding parallelism and concurrency can significantly improve the total throughput for applications that need to perform a large number of CDS operations in a short period of time.

Because the [Microsoft.Xrm.Tooling.Connector.CrmServiceClient Class](https://learn.microsoft.com/dotnet/api/microsoft.xrm.tooling.connector.crmserviceclient) includes handling for the transient errors thrown by the CDS service protection limits, the combination of TPL and CrmServiceClient is valuable to create applications that can optimize throughput while being resilient to the service protection limit errors by re-trying requests that are rejected due to these limits.

The [CrmServiceClient.Clone Method](https://learn.microsoft.com/dotnet/api/microsoft.xrm.tooling.connector.crmserviceclient.clone) enables TPL to use the client with multiple threads.

## Demonstrates

This simple sample will generate a number of account table records using the [System.Threading.Tasks.Parallel.ForEach Method](https://learn.microsoft.com/dotnet/api/system.threading.tasks.parallel.foreach).

Then it will use that technique again to delete the entities created.

**NOTE**:
> By default, this sample will create only 10 records, which is not enough to hit the service protection api limit errors. If you raise the `numberOfRecords` variable value to 10000, you can use Fiddler to observe how some of the requests will be rejected and re-tried.

This sample is not configured to disable the Azure Affinity cookies, which is another recommendation to improve throughput. To enable this, add the following to the App.config of your application:

```xml
  <appSettings>
    <add key="PreferConnectionAffinity"
         value="false" />
  </appSettings>
```


### More information

[Sample: Task Parallel Library with CrmServiceClient](https://learn.microsoft.com/power-apps/developer/data-platform/xrm-tooling/sample-tpl-crmserviceclient)  
[Task Parallel Library (TPL)](https://learn.microsoft.com/dotnet/standard/parallel-programming/task-parallel-library-tpl)