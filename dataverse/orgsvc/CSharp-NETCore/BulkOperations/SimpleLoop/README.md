# SimpleLoop README

This project loops through the list of prepared `Entity` instances to perform create and update individual operations sequentially using the [CreateRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.createrequest) and [UpdateRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.updaterequest) classes.

The project depends on the common structure for other projects in this solution as described in [CreateUpdateMultiple/README.md](../README.md).

This sample represents the case where no effort is applied to maximize throughput. This sample should represent the worst case for performance.

You can expect this output for the project:

```cmd
Creating sample_Example Standard table...
        sample_Example table created.
Adding 'sample_Description' column to sample_Example table...
        'sample_Description' column created.

Preparing 100 records to create..
Sending create requests one at a time...
        Created 100 records in 10 seconds.

Preparing 100 records to update..
Sending update requests one at a time...
        Updated 100 records in 12 seconds.

Starting asynchronous bulk delete of 100 created records...
        Asynchronous job to delete 100 records completed in 40 seconds.
        Bulk Delete status: Succeeded

Deleting sample_Example table...
        sample_Example table deleted.
```
