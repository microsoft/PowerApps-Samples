# CreateUpdateMultiple README

This project uses [CreateMultiple](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/createmultiple) and [UpdateMultiple](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/updatemultiple) actions to perform bulk create and update operations.

It depends on the common structure for other projects in this solution that is described in [BulkOperations/README.md](../README.md).

**This project sends only two requests**, each attempting to complete operations for the total configured number of records.

The output of this project will look like this:

```
Creating sample_Example Standard table...

Preparing 100 records to create..
Sending POST request to /sample_examples/Microsoft.Dynamics.CRM.CreateMultiple
        Created 100 records in 1 seconds.

Preparing 100 records to update..
Sending POST request to /sample_examples/Microsoft.Dynamics.CRM.UpdateMultiple
        Updated 100 records in 3 seconds.

Starting asynchronous bulk delete of 100 created records...
        Asynchronous job to delete 100 records completed in 32 seconds.
        Bulk Delete status: Succeeded

Deleting sample_Example table...
        sample_Example table deleted.
```