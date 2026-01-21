# ParallelCreateUpdateMultiple README

This project uses methods and classes to perform multiple create and update operations using multiple threads:

- [System.Threading.Tasks.Parallel.ForEachAsync](https://learn.microsoft.com/dotnet/api/system.threading.tasks.parallel.foreachasync?view=net-6.0)
- [Microsoft.PowerPlatform.Dataverse.Client.ServiceClient.ExecuteAsync](https://learn.microsoft.com/dotnet/api/microsoft.powerplatform.dataverse.client.serviceclient.executeasync)
- [CreateMultipleRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.createmultiplerequest)
- [UpdateMultipleRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.updatemultiplerequest)

This project depends on the common structure for other projects in this solution as described in [CreateUpdateMultiple/README.md](../README.md).

The number of threads used depend on the [ServiceClient.RecommendedDegreesOfParallelism](https://learn.microsoft.com/dotnet/api/microsoft.powerplatform.dataverse.client.serviceclient.recommendeddegreesofparallelism) property, which is based on the value of the `x-ms-dop-hint` response header. This header provides a hint for the Degree Of Parallelism (DOP) that represents a number of threads that should provide good results for a given environment.

You can expect this output for the project:

```cmd
RecommendedDegreesOfParallelism:4

Creating sample_Example Standard table...
        sample_Example table created.
Adding 'sample_Description' column to sample_Example table...
        'sample_Description' column created.

Preparing 100 records to create..
Sending create requests in parallel...
        Created 100 records in 1 seconds.

Preparing 100 records to update..
Sending update requests in parallel...
        Updated 100 records in 3 seconds.

Starting asynchronous bulk delete of 100 created records...
        Asynchronous job to delete 100 records completed in 19 seconds.
        Bulk Delete status: Succeeded

Deleting sample_Example table...
        sample_Example table deleted.
```
