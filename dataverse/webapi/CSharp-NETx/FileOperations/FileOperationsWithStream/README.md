# Web API File Operations with Stream sample

This document describes the how the `FileOperationsWithStream` project implements the common operations described in [Web API File Operations sample](../README.md). See that document for an overview and how to run this sample.

## Demonstrates

The `FileOperationsWithStream` project demonstrates how to upload or download files with a single request using Http methods and the Dataverse Web API.

> **Important**
> This method can only be used to upload files less than 128 MB.

This sample uses classes defined within the [WebAPIService project Messages folder](../../WebAPIService/Messages/) so that they can be re-used. These classes inherit from the .NET [HttpRequestMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httprequestmessage?view=net-6.0) and [HttpResponseMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httpresponsemessage?view=net-6.0) classes. These classes have constructors that accept the relevant variables to compose the requests to send, or to deserialize the responses into standard properties.

The code for this sample is in the [Program.cs](Program.cs) file.

## Upload File

To upload a PDF file named `25mb.pdf` to the `sample_FileColumn` file column on the account record, this sample uses the [UploadFileRequest class](../../WebAPIService/Messages/UploadFileRequest.cs). This request doesn't have any response value to process.


## Download File

If the file was succesfully uploaded, to download the text file named `25mb.pdf` that was just uploaded to the `sample_FileColumn` file column on the account record, this sample uses the [DownloadFileRequest class](../../WebAPIService/Messages/DownloadFileRequest.cs).

To process the response the [DownloadFileResponse class](../../WebAPIService/Messages/DownloadFileResponse.cs) simply reads the response content as a byte[] to get the entire file.


## Delete File

If the file was succesfully uploaded, use the [DeleteColumnValueRequest class](../../WebAPIService/Messages/DeleteColumnValueRequest.cs) to delete the file.This method can be used to delete the value of any type of column. This request doesn't have any response value to process.