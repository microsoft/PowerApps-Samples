# Sample: One-way listener

This sample shows how to write a `Azure Service Bus` listener for a one-way endpoint contract.

This sample listener application registers a remote service plug-in that executes whenever a message is posted to a one-way endpoint on the `Azure Service Bus`. When the plug-in executes, it outputs to the console the contents of the execution context contained in the message. 
