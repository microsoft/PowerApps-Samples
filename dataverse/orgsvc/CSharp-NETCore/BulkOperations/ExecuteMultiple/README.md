# ExecuteMultiple README

This project uses the [ExecuteMultipleRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.executemultiplerequest) class to perform bulk create and update operations.

The project depends on the common structure for other projects in this solution that is described in [CreateUpdateMultiple/README.md](../README.md).

Because `ExecuteMultiple` is limited to 1000 requests and the sample may be configured to create more records than that, this project chunks the total number of requests into groups of 1000 and sends as many requests as needed to perform the respective operations.

You can expect this output for the project:

```cmd
Creating sample_Example Standard table...
        sample_Example table created.
Adding 'sample_Description' column to sample_Example table...
        'sample_Description' column created.

Preparing 100 records to create..
Sending 1 ExecuteMultipleRequest to create...
 Sending ExecuteMultipleRequest 1...
        Created 100 records in 4 seconds.

Preparing 100 records to update...
Sending 1 ExecuteMultipleRequest to update...
 Sending ExecuteMultipleRequest 1...
        Updated 100 records in 4 seconds.

Starting asynchronous bulk delete of 100 created records...
        Asynchronous job to delete 100 records completed in 40 seconds.
        Bulk Delete status: Succeeded

Deleting sample_Example table...
        sample_Example table deleted.
```
