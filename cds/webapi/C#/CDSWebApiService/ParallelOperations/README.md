# Web API CDSWebApiService Parallel Operations Sample

This example shows how to use a Parallel.ForEach loop to enable data parallelism over a set of records to create in CDS. 

It is based on the [How to: Write a simple Parallel.ForEach loop](https://docs.microsoft.com/dotnet/standard/parallel-programming/how-to-write-a-simple-parallel-foreach-loop) example, but modified to perform create and delete operations with CDS entities using the synchronous methods provided by the CDSWebApiService class.

Because the CDSWebApiService class methods enable managing the Service Protection API limits, this example demonstrates techniques to achieve higher throughput with an application using multiple threads that is resilient to the transient 429 errors an application should expect.

## Note

If you want to use Fiddler to observe the expected service protection API limits, you will need to set the number of records to create to be around 10,000. They will start to appear after 5 minutes. 

Note how the application retries the failures and completes the flow of all the records.

## More information

For more information, see 

- [Task Parallel Library (TPL)](https://docs.microsoft.com/dotnet/standard/parallel-programming/task-parallel-library-tpl)
- [Parallel Programming in .NET](https://docs.microsoft.com/dotnet/standard/parallel-programming/)