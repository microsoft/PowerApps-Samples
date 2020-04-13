# Web API CDSWebApiService Async Parallel Operations Sample

This sample demonstrates using Task Parallel Library (TPL) dataflow components [Dataflow (Task Parallel Library)](https://docs.microsoft.com/dotnet/standard/parallel-programming/dataflow-task-parallel-library) with asynchronous requests.

TPL provides capabilities to add parallelism and concurrency to applications. These capabilties are an important part of maximizing throughput when performing operations that will add or update data within CDS.

This sample uses the CDSWebApiService class asynchronous methods within asynchronous operations. Because the CDSWebApiService class can manage Service Protection API limits, this code can be resilient to the transient 429 errors that clients should expect. It will retry a configurable number of times.

This sample simply creates a configurable number of account records to create, which it will in turn delete. This sample uses dataflow components to process the records and transform the results of the create operation into the next phase that deletes these records. Because of the nature of this data flow, delete operations for previously created records will start before all the records to create are finished.

## Note

If you want to use Fiddler to observe the expected service protection API limits, you will need to set the number of records to create to be around 10,000. They will start to appear after 5 minutes. 

Note how the application retries the failures and completes the flow of all the records.

## More information

For more information, see 

- [Dataflow (Task Parallel Library)](https://docs.microsoft.com/dotnet/standard/parallel-programming/dataflow-task-parallel-library)
- [Task Parallel Library (TPL)](https://docs.microsoft.com/dotnet/standard/parallel-programming/task-parallel-library-tpl)
- [Parallel Programming in .NET](https://docs.microsoft.com/dotnet/standard/parallel-programming/)