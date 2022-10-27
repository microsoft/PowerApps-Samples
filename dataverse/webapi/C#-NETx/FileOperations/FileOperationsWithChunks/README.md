# Web API File Operations with chunks sample

This document describes the how the `FileOperationsWithChunks` project implements the common operations described in [Web API File Operations sample](../README.md). See that document for an overview and how to run this sample.

## Demonstrates

The `FileOperationsWithChunks` project demonstrates how to work with files using standard Http methods and the Dataverse Web API.

This sample uses classes defined within the [WebAPIService project Messages folder](../../WebAPIService/Messages/) so that they can be re-used. These classes inherit from the .NET [HttpRequestMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httprequestmessage?view=net-6.0) and [HttpResponseMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httpresponsemessage?view=net-6.0) classes. These classes have constructors that accept the relevant variables to compose the requests to send, or to deserialize the responses into standard properties.

The code for this sample is in the [Program.cs](Program.cs) file.

## Upload File

To upload a PDF file named `25MB.pdf` to the `sample_FileColumn` file column on the account record, this sample uses an `UploadFile` static method that accepts all the parameters needed to make the following requests:

1. Initialize the upload with the [InitializeChunkedFileUploadRequest class](../../WebAPIService/Messages/InitializeChunkedFileUploadRequest.cs)
1. Process the response with the [InitializeChunkedFileUploadResponse class](../../WebAPIService/Messages/InitializeChunkedFileUploadResponse.cs) to get the URL from the `Location` header to send subsequent requests.
1. Split the file up into 4MB block and send each block using the [UploadFileChunkRequest class](../../WebAPIService/Messages/UploadFileChunkRequest.cs). This request doesn't have any response value to process.


## Download File

To download the PDF file named `25MB.pdf` that was just uploaded to the `sample_FileColumn` file column on the account record, this sample uses an `DownloadFile` static method that accepts all the parameters needed to make the following requests:

1. If the file was succesfully uploaded, instantiate a `byte[]` variable to capture the contents of the file.
1. Send a series of [DownloadFileChunkRequest class](../../WebAPIService/Messages/DownloadFileChunkRequest.cs) while there are chunks remaining to download.
1. Process each response with the [DownloadFileChunkResponse class](../../WebAPIService/Messages/DownloadFileChunkResponse.cs) to get `byte[]` data passed in the content of the response. This response also extracts the size of the file from the `x-ms-file-size` so the total number of iterations required can be known. Copy the data from each response into the `byte[]` variable.
1. After all the blocks are recieved, return the `byte[]` variable.

## Delete File

If the file was succesfully uploaded, use the [DeleteColumnValueRequest class](../../WebAPIService/Messages/DeleteColumnValueRequest.cs) to delete the file.This method can be used to delete the value of any type of column. This request doesn't have any response value to process.