# ParallelCreateUpdateMultiple README

This project uses the 
[System.Threading.Tasks.Parallel.ForEachAsync Method](https://learn.microsoft.com/dotnet/api/system.threading.tasks.parallel.foreachasync?view=net-6.0)
and the `CreateMultiple` and `UpdateMultiple` actions to perform multiple create and update operations using multiple threads.

It depends on the common structure for other projects in this solution that is described in [BulkOperations/README.md](../README.md).

The number of threads used will depend on the value of the `x-ms-dop-hint` response header. 
The `x-ms-dop-hint` response header provides a hint for the Degree Of Parallelism (DOP) that represents a number of threads 
that should provide good results for a given environment.

The output of this project will look like this:

```
RecommendedDegreesOfParallelism:4

Creating sample_Example Standard table...

Preparing 100 records to create..
Sending POST requests to /sample_examples/Microsoft.Dynamics.CRM.CreateMultiple in parallel...
        Created 100 records in 1 seconds.

Preparing 100 records to update..
Sending POST requests to /sample_examples/Microsoft.Dynamics.CRM.UpdateMultiple in parallel...
        Updated 100 records in 2 seconds.

Starting asynchronous bulk delete of 100 created records...
        Asynchronous job to delete 100 records completed in 18 seconds.
        Bulk Delete status: Succeeded

Deleting sample_Example table...
        sample_Example table deleted.
```

