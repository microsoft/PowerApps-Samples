# CreateUpdateMultiple README

This project uses [CreateMultipleRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.createmultiplerequest) and [UpdateMultipleRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.updatemultiplerequest) classes to perform bulk create and update operations.

It depends on the common structure for other projects in this solution that is described in [CreateUpdateMultiple/README.md](../README.md).

**This project sends only two requests**, each attempting to complete operations for the total configured number of records.

The output of this project will look like this:

```
Creating sample_Example Standard table...
        sample_Example table created.
Adding 'sample_Description' column to sample_Example table...
        'sample_Description' column created.

Preparing 100 records to create..
Sending CreateMultipleRequest...
        Created 100 records in 1 seconds.

Preparing 100 records to update..
Sending UpdateMultipleRequest...
        Updated 100 records in 3 seconds.

Starting asynchronous bulk delete of 100 created records...
        Asynchronous job to delete 100 records completed in 40 seconds.
        Bulk Delete status: Succeeded

Deleting sample_Example table...
        sample_Example table deleted.
```