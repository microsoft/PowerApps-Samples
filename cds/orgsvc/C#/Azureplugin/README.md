# Sample: Azure aware custom plug-in

The plug-in demonstrates how to obtain the execution context and the tracing service from the service provider parameter of the `Execute` method. The plug-in then posts the context to the Azure Service Bus endpoint and writes information to the trace log to facilitate debugging.

## How to run this sample

1. Download or clone the [Samples](https://github.com/Microsoft/PowerApps-Samples) repo so that you have a local copy.
2. Open the sample solution in Visual Studio and sign the assembly with a key.
3. Register the plug-in using the **Plugin Registration Tool**.

>[!NOTE]
> This sample requires a service endpoint to be created first, and its ID passed to the plug-in constructor through the unsecure configuration parameter when the plug-in step is registered.
